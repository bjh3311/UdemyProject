using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player라는 스크립트가 이미 Photon 폴더 안에 있다
public class MyPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector2 input=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        //GetAxisRaw("Horizontal") :오른쪽 방향키누르면 1을 반환, 아무것도 안누르면 0, 왼쪽방향키는 -1 반환
        //GetAxis("Horizontal"):-1과 1 사이의 실수값을 반환
        //Vertical은 위쪽방향키 누를시 1,아무것도 안누르면 0, 아래쪽방향키는 -1 반환

        Vector2 inputDir=input.normalized;
        //벡터 정규화. 만약 input=new Vector2(1,1) 이면 오른쪽위 대각선으로 움직인다.
        //방향을 찾아준다

        if(inputDir!=Vector2.zero)//화살표에서 손을 떼면 다시 처음 각도로 되돌아가는걸 막기위함
        {
            transform.eulerAngles=Vector3.up*Mathf.Atan2(inputDir.x,inputDir.y)*Mathf.Rad2Deg;
        }
        //각도를 구해주는 코드, 플레이어가 오른쪽 위 대각선으로 움직일시 그 방향을 바라보게 해준다
        //Mathf.Atan2는 라디안을 return하기에 다시 각도로 바꿔주는 Mathf.Rad2Deg를 곱해준다
        //Vector.up은 y axis를 의미한다

        transform.Translate(transform.forward*(5f*inputDir.magnitude)*Time.deltaTime,Space.World);
    }
}
