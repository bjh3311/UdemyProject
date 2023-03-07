using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class CarManager : MonoBehaviourPun , IPunOwnershipCallbacks
{
    public GameObject CarInCanvas;
    [Space]
    public GameObject playerCanvas;
    public GameObject CarCanvas;
    [Space]
    public GameObject carCamera;

    GameObject player;
    public bool iscarFree=true;

    private void OnTriggerEnter(Collider collision)//자동차 주변 범위 안에 들어왔으면
    {
        if(collision.CompareTag("Player"))
        {
            if(iscarFree)//자동차가 아무도 안 타 있는 상태라면
            {
                collisionWithPlayer(collision.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider collision) //일정범위 밖으로 나가면 자동차 타고 내리는 버튼 비활성화
    {
        if(collision.CompareTag("Player"))
        {
            CarInCanvas.SetActive(false);
        }
    }
    void collisionWithPlayer(GameObject carplayer)//일정범위 안에 들어왔으면 자동차 타고 내리는 버튼 활성화
    {
        CarInCanvas.SetActive(true);
        player=carplayer;
    }

    public void OnClick_CarButton()//자동차 타고 내리는 버튼 눌렀을때 실행
    {
        if(iscarFree)
        {
            GetIn();
        }
        else
        {
            GetOut();
        }
    }
    private void GetIn()//자동차에 태우기
    {
        base.photonView.RequestOwnership();//이 함수가 실행되면IPunOwnershipCallbacks.OnOwnershipRequest실행
        playerCanvas.SetActive(false);//기본 플레이어 UI 끄기
        CarCanvas.SetActive(true);//자동차 UI켜기
        player.transform.SetParent(this.transform);//플레이어를 자동차의 하위 하이라키로 옮긴다
        player.SetActive(false);//플레이어를 화면에서 없애야 한다
        iscarFree=false;//자동차에 탔으므로 iscarFree를 false로 해준다
        carCamera.SetActive(true);// 자동차를 따라가야 한다
    }
    private void GetOut()//자동차에서 내리기
    {
        playerCanvas.SetActive(true);
        CarCanvas.SetActive(false);
        player.transform.parent=null;
        player.SetActive(true);
        iscarFree=true;
        carCamera.SetActive(false);
    }

    void IPunOwnershipCallbacks.OnOwnershipRequest(Photon.Pun.PhotonView targetView, Photon.Realtime.Player requestingPlayer)
    {
        // OnOwnershipRequest gets called on every script that implements it every time a request for ownership transfer of any object occurs
        // So, firstly, only continue if this callback is getting called because *this* object is being transferred
        if(targetView != base.photonView)
        {
            return;
        }
        if(iscarFree)//자동차가 자유롭다면
        {
            base.photonView.TransferOwnership(requestingPlayer);
            //이 자동차의 소유권을 요청한 플레이어에게 넘긴다
        }
    }//소유권 요청
    void IPunOwnershipCallbacks.OnOwnershipTransfered(Photon.Pun.PhotonView targetView, Photon.Realtime.Player previousOwner)
    {
        if(targetView != base.photonView)
        {
            return;
        }
        throw new System.NotImplementedException();
    }//위 소유권 요청이 잘 실행되었으면 이 함수도 실행된다
    void IPunOwnershipCallbacks.OnOwnershipTransferFailed(Photon.Pun.PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
    {
        
    }
}
