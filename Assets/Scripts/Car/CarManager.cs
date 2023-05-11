using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using Photon.Pun;
using Photon.Realtime;

public class CarManager : MonoBehaviourPun 
{
    public GameObject CarInCanvas;
    [Space]
    public GameObject playerCanvas;
    public GameObject CarCanvas;
    [Space]
    public GameObject carCamera;

    GameObject player;
    public bool iscarFree=true;

    private GameObject crosshair;

    private void OnTriggerEnter(Collider collision)//자동차 주변 범위 안에 들어왔으면
    {
        if(collision.CompareTag("Player")&&collision.GetComponent<PhotonView>().IsMine)
        {
            if(iscarFree)//자동차가 아무도 안 타 있는 상태라면
            {
                collisionWithPlayer(collision.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider collision) //일정범위 밖으로 나가면 자동차 타고 내리는 버튼 비활성화
    {
        if(collision.CompareTag("Player")&&collision.GetComponent<PhotonView>().IsMine)
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
        this.gameObject.GetComponent<CarAudio>().enabled=true;//자동차에 탓을때만 자동차의 Audio를 켜준다
        if(crosshair==null)
        {
            crosshair=GameObject.Find("CrossHairCanvas(Clone)");
            crosshair.SetActive(false);
        }
        else
        {
            crosshair.SetActive(false);
        }
        playerCanvas.SetActive(false);//기본 플레이어 UI 끄기
        CarCanvas.SetActive(true);//자동차 UI켜기
        player.transform.SetParent(this.transform);//플레이어를 자동차의 하위 하이라키로 옮긴다
        player.GetPhotonView().RPC("SetFalsePlayer",RpcTarget.All);//플레이어를 화면에서 없애야 한다
        photonView.RPC("SetCarState",RpcTarget.AllBufferedViaServer,false);//모든 클라이언트에서 false로 바뀌어야한다
        //늦게들어온 플레이어도 받아야하므로 AllBufferedViaServer
        carCamera.SetActive(true);// 자동차를 따라가야 한다
    }
    private void GetOut()//자동차에서 내리기
    {
        playerCanvas.SetActive(true);
        this.gameObject.GetComponent<CarAudio>().StopSound();
        this.gameObject.GetComponent<CarAudio>().enabled=false;
        crosshair.SetActive(true);
        player.GetPhotonView().RPC("RevealPlayerMesh",RpcTarget.All);
        CarCanvas.SetActive(false);
        player.transform.parent=null;
        photonView.RPC("SetCarState",RpcTarget.AllBufferedViaServer,true);//모든 클라이언트에서 true로 바뀌어야한다
        carCamera.SetActive(false);
    }
    [PunRPC]
    void SetCarState(bool status)
    {
        iscarFree=status;
    }
}
