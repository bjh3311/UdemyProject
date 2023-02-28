using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CarManager : MonoBehaviour , IPunOwnershipCallbacks
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
        if(collision.tag=="Player")
        {
            if(iscarFree)//자동차가 아무도 안 타 있는 상태라면
            {
                collisionWithPlayer(collision.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider collision) 
    {
        CarInCanvas.SetActive(false);
    }
    void collisionWithPlayer(GameObject carplayer)
    {
        CarInCanvas.SetActive(true);
        player=carplayer;
    }

    public void OnClick_CarButton()
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
        playerCanvas.SetActive(false);
        CarCanvas.SetActive(true);
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

    }
    void IPunOwnershipCallbacks.OnOwnershipTransfered(Photon.Pun.PhotonView targetView, Photon.Realtime.Player previousOwner)
    {

    }
    void IPunOwnershipCallbacks.OnOwnershipTransferFailed(Photon.Pun.PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
    {
        
    }
}
