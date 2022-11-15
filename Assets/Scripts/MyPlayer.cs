using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
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

    public bool enableMobile=false;
    //모바일 환경인지 아닌지 나타내는 변수, 단순히 개발을 위한 변수이다
    public FixedJoystick joystick;

    private Transform cameraTransform;
    private Animator anim;

    public Transform rayOrigin;//플레이어의 오른쪽 손

    private void Start() 
    {
        cameraTransform =Camera.main.transform;
        anim=this.GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
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
        }
        //각도를 구해주는 코드, 플레이어가 오른쪽 위 대각선으로 움직일시 그 방향을 바라보게 해준다
        //Mathf.Atan2는 라디안을 return하기에 다시 각도로 바꿔주는 Mathf.Rad2Deg를 곱해준다
        //Vector.up은 y axis를 의미한다
        //SmoothDampAngle을 이용해서 부드럽게 플레이어의 각도를 바꿔준다.

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
        transform.Translate(transform.forward*currentSpeed*Time.deltaTime,Space.World);
    }
    public void Fire()
    {
        anim.SetTrigger("fire");
        RaycastHit hit;
        if(Physics.Raycast(rayOrigin.position,Camera.main.transform.forward,out hit,25f))
        {
            Debug.Log(hit.transform.gameObject.name);
        }//플레이어 위치에서 카메라의 방향으로 25f거리만큼 Ray를 쏜다

        Debug.DrawRay(rayOrigin.position,Camera.main.transform.forward*25f,Color.green);
        //위의 레이캐스트를 눈으로 볼 수 있게 해준다.
    }

}
