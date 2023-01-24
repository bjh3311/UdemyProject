using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyCamera : MonoBehaviour
{
    public float Yaxis;
    public float Xaxis;
    
    private Transform target;//Player

    private float rotSensitive=0.3f;//카메라 회전 감도
    private float dis=2f;//카메라와 플레이어사이의 거리
    private float RotationMin=-10f;//카메라 회전각도 최소
    private float RotationMax=80f;//카메라 회전각도 최대
    private float smoothTime=0.12f;//카메라가 회전하는데 걸리는 시간
    //위 5개의 value는 개발자의 취향껏 알아서 설정해주자
    private Vector3 targetRotation;
    private Vector3 currentVel;
    public bool enableMobile=false;
    //모바일 환경인지 아닌지 나타내는 변수, 단순히 개발을 위한 변수이다
    private FixedTouchField touchField;
    private GameObject player;
    private void Start() 
    {
        player=GetLocalPlayer();
        if(player.GetComponent<PhotonView>().IsMine)
        {   
            touchField=GameObject.Find("TouchPanel").GetComponent<FixedTouchField>();
            target=player.transform.GetChild(3);
            //CameraTarget오브젝트가 Player의 4번째에 붙어있으므로 인덱스상 GetChild(3)이 맞다.
        }
        else
        {
            this.gameObject.SetActive(false);
        }//There are many cameras if you start game.So turn all of them off except your camera.
    }
    
    void LateUpdate()//Player가 움직이고 그 후 카메라가 따라가야 하므로 LateUpdate
    {
        if(enableMobile)//모바일 환경이라면
        {
            Yaxis=Yaxis+touchField.TouchDist.x*rotSensitive;//터치패널 x축 입력받아서 카메라의 Y축을 회전시킨다
            Xaxis=Xaxis-touchField.TouchDist.y*rotSensitive;//터치패널 y축 입력받아서 카메라의 X축을 회전시킨다
        }
        else
        {
            Yaxis=Yaxis+Input.GetAxis("Mouse X")*rotSensitive;//마우스 좌우움직임을 입력받아서 카메라의 Y축을 회전시킨다
            Xaxis=Xaxis-Input.GetAxis("Mouse Y")*rotSensitive;//마우스 상하움직임을 입력받아서 카메라의 X축을 회전시킨다
        }
        //Xaxis는 마우스를 아래로 했을때(음수값이 입력 받아질때) 값이 더해져야 카메라가 아래로 회전한다 

        Xaxis=Mathf.Clamp(Xaxis,RotationMin,RotationMax);
        //X축회전이 한계치를 넘지않게 제한해준다.

        targetRotation=Vector3.SmoothDamp(targetRotation,new Vector3(Xaxis,Yaxis),ref currentVel,smoothTime);
        this.transform.eulerAngles=targetRotation;
        //SmoothDamp를 통해 부드러운 카메라 회전

        Vector3 _offset=target.position-transform.forward*dis;
        _offset.y=1.22f;
        transform.position=_offset;
        //카메라의 위치는 플레이어보다 설정한 값만큼 떨어져있게
    }
    GameObject GetLocalPlayer()
    {
        GameObject[] players =GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject temp in players)
        {
            if(temp.GetComponent<PhotonView>().IsMine)
            {
                
                return temp;
            }
        }
        return null;
    }

}
