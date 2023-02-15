using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

using Photon;

public class MyPlayer : MonoBehaviourPun , IPunObservable
{
    [SerializeField]//인스펙터에서만 참조 가능하게
    private float smoothRotationTime;//target 각도로 회전하는데 걸리는 시간
    [SerializeField]
    private float smoothMoveTime;//target 속도로 바뀌는데 걸리는 시간
    [SerializeField]
    private float moveSpeed;//움직이는 속도
    private float rotationVelocity;//The current velocity, this value is modified by the function every time you call it.
    private float speedVelocity;//The current velocity, this value is modified by the function every time you call it.
    private float currentSpeed;
    private float targetSpeed;

    [SerializeField]
    private float JumpForce;

    public bool enableMobile=false;
    //모바일 환경인지 아닌지 나타내는 변수, 단순히 개발을 위한 변수이다
    private FixedJoystick joystick;
    private FireBtn firebtn;
    private FixedButton jump;

    private Transform cameraTransform;
    private Animator anim;

    public Transform rayOrigin;//플레이어의 오른쪽 손

    private GameObject crossHair;
    private Rigidbody rb;

    //sounds
    public AudioSource shootSound;
    public AudioSource runSound;

    private bool fire;

    private ParticleSystem muzzle;

    public PhotonView PV;

    //health
    public Image fillImage;
    public float playerHealth = 1f;

    public float damage=0.01f;


    private void Start() 
    {
        PV=this.gameObject.GetPhotonView();
        if(PV.IsMine)
        {
            joystick=GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
            GameObject.Find("Shoot").GetComponent<FireBtn>().SetPlayer(this);
            GameObject.Find("Jump").GetComponent<FixedButton>().SetPlayer(this);
            muzzle=rayOrigin.Find("SciFiRifle(Clone)/GunMuzzle").GetComponent<ParticleSystem>();
            crossHair=Resources.Load("CrosshairCanvas") as GameObject;
            cameraTransform =Camera.main.transform;
            anim=this.GetComponent<Animator>();
            crossHair=Instantiate(crossHair);
            rb=this.gameObject.GetComponent<Rigidbody>();
        }
        else
        {
            this.gameObject.GetComponent<BetterJump>().enabled=false;
        }
    }
    void LateUpdate()
    {
        if(PV.IsMine)
        {
            PositionCrossHair();
        }
    }

    void Update()
    {
        if(!PV.IsMine)
        {
            return;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        Vector2 input =Vector2.zero;
        if(enableMobile)//모바일 환경이라면
        {
            input=new Vector2(joystick.input.x,joystick.input.y);
        }
        else//pc환경이라면 
        {
            input=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        }
        //GetAxisRaw("Horizontal") :오른쪽 방향키누르면 1을 반환, 아무것도 안누르면 0, 왼쪽방향키는 -1 반환
        //GetAxis("Horizontal"):-1과 1 사이의 실수값을 반환
        //Vertical은 위쪽방향키 누를시 1,아무것도 안누르면 0, 아래쪽방향키는 -1 반환

        Vector2 inputDir=input.normalized;
        //벡터 정규화. 만약 input=new Vector2(1,1) 이면 오른쪽위 대각선으로 움직인다.
        //방향을 찾아준다

        if(inputDir!=Vector2.zero)//움직임을 멈췄을 때 다시 처음 각도로 돌아가는걸 막기위함
        {
            float rotation=Mathf.Atan2(inputDir.x,inputDir.y)*Mathf.Rad2Deg+cameraTransform.eulerAngles.y;
            transform.eulerAngles=Vector3.up*Mathf.SmoothDampAngle(transform.eulerAngles.y,rotation,ref rotationVelocity,smoothRotationTime);
        
            if(!runSound.isPlaying)
            {
                runSound.Play();
            }
        }
        else
        {
            runSound.Stop();
        }
        //각도를 구해주는 코드, 플레이어가 오른쪽 위 대각선으로 움직일시 그 방향을 바라보게 해준다
        //Mathf.Atan2는 라디안을 return하기에 다시 각도로 바꿔주는 Mathf.Rad2Deg를 곱해준다
        //Vector.up은 y axis를 의미한다
        //SmoothDampAngle을 이용해서 부드럽게 플레이어의 각도를 바꿔준다.
        if (fire)
        {
            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * rotation;
        }//Player must change its rotation when it is shooting a gun.
        targetSpeed=moveSpeed*inputDir.magnitude;
        currentSpeed=Mathf.SmoothDamp(currentSpeed,targetSpeed,ref speedVelocity,smoothMoveTime);
        if(inputDir.magnitude>0)
        {
            anim.SetBool("running",true);
        }
        else if (inputDir.magnitude==0)
        {
            anim.SetBool("running",false);
        }
        //현재스피드에서 타겟스피드까지 smoothMoveTime 동안 변한다

        if(!fire)//Player can't move while shooting a gun.
        {
            transform.Translate(transform.forward*currentSpeed*Time.deltaTime,Space.World);
        }
    }
    void PositionCrossHair()//크로스헤어를 카메라의 중앙에 둔다
    {
        RaycastHit hit;
        Ray ray=Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f));
        //ViewportPointToRay는 0~1범위 내에서 정규화 된 좌표에 광선을 쏜다
        // 0.5f,0.5f니깐 카메라에서 딱 정중앙에 광선을 쏜다

        int layer_mask=LayerMask.GetMask("Default");
        //Default Layer인 물체만 감지하기 위해

        if(Physics.Raycast(ray,out hit,100f,layer_mask))
        {
            Vector3 hitpos=hit.point;//광선이 맞은 위치
            crossHair.transform.position=hitpos;
            crossHair.transform.LookAt(Camera.main.transform);//무조건 카메라를 보게
        }
    }
    public void Fire()//Fire버튼 누르면
    {
        fire=true;
        anim.SetBool("fire",true);

        RaycastHit hit;
        if(Physics.Raycast(rayOrigin.position,Camera.main.transform.forward,out hit,100f))
        {
            if(!hit.transform.GetComponent<PhotonView>().IsMine&&hit.transform.tag=="Player" )//Not hitting myself
            {
                hit.transform.GetComponent<PhotonView>().RPC("GetDamage",RpcTarget.AllBuffered,damage);
            }
        }//플레이어 위치에서 카메라의 방향으로 100f거리만큼 Ray를 쏜다
        Debug.DrawRay(rayOrigin.position,Camera.main.transform.forward*100f,Color.green);
        //위의 레이캐스트를 눈으로 볼 수 있게 해준다.

        shootSound.loop=true;
        shootSound.Play();

        muzzle.Play();//총구섬광 플레이
    }
    public void FireUp()//Fire버튼에서 손을 떼면
    {
        fire=false;
        if(photonView.IsMine)
        {
            anim.SetBool("fire",false);
        }
        shootSound.loop=false;
        shootSound.Stop();

        if(muzzle==null)
        {
            muzzle=rayOrigin.Find("SciFiRifle(Clone)/GunMuzzle").GetComponent<ParticleSystem>();
        }
        muzzle.Stop();//총구섬광 정지

    }
    public void Jump()
    {
        anim.SetTrigger("jump");
        rb.velocity=Vector3.zero;
        rb.angularVelocity=Vector3.zero;
        rb.AddForce(Vector3.up*JumpForce,ForceMode.Impulse);
    }

    public void MuzzleFlash()
    {
        if(muzzle==null)
        {
            muzzle=rayOrigin.Find("SciFiRifle(Clone)/GunMuzzle").GetComponent<ParticleSystem>();
        }
        muzzle.Play();
    }

    [PunRPC]
    public void GetDamage(float amount)
    {
        playerHealth=playerHealth-amount;
    }

    public void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        if(stream.IsWriting)//in the case of my local player...
        {
            stream.SendNext(fire);
        }
        else
        {
            if((bool)stream.ReceiveNext())
            {
                MuzzleFlash();
            }
            else
            {
                FireUp();
            }
        }
    }


}
