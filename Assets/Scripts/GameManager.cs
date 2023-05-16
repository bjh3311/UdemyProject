using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon;
using Photon.Pun;


public class GameManager :  MonoBehaviourPunCallbacks
{

    public static GameManager instance =null;
    // Start is called before the first frame update
    public GameObject player;

    public GameObject deathScreen;
    public Text totalAlive;

    public GameObject spectateContainer;
    public GameObject spectateObject;
    private int totalPlayer=0;
    public GameObject winScreen;
    public GameObject loseScreen;
    void Awake()//First of all, make the player. It makes camera can track the player.
    {
        instance=this;
        float random_x=Random.Range(-25,25);
        float random_z=Random.Range(-25,25);
        Vector3 position=new Vector3(random_x,5,random_z);
        Quaternion rotation3=new Quaternion(0f,0f,0f,0f);
        PhotonNetwork.Instantiate(player.name,position,rotation3);//랜덤으로 플레이어가 생성되게
    }
    void Start()
    {
        totalPlayer=PhotonNetwork.PlayerList.Length;
    }
    public void Spectate()
    {
        deathScreen.SetActive(true);
        FindAllPlayer();
    }
    void FindAllPlayer()
    {
        int daedPlayer=0;
        GameObject[] players=GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject temp in players)
        {
            if(temp.name.Contains("Car"))
            {
                continue;
            }
            if(!temp.GetComponent<MyPlayer>().isDead)
            { 
                GameObject so =Instantiate(spectateObject,spectateContainer.transform);
                so.transform.Find("PlayerName").GetComponent<Text>().text=temp.GetPhotonView().Owner.NickName;
                so.transform.Find("SpectateBtn").GetComponent<SpectateButton>().target = temp;
            }
            else
            {
                daedPlayer++;
            } 
        }
        if(daedPlayer==totalPlayer-1)//한명 빼고 다 죽었다는게 확인되면
        {
            foreach(GameObject temp in players)
            {
                temp.GetPhotonView().RPC("Iwin",RpcTarget.All);
            }
        }
    }
    [PunRPC]
    public void RemovePlayer(string NickName)
    {
        if(!deathScreen.activeSelf)//death Screen이 활성화되어 있을때만 실행해준다
        {
            return;
        }
        GameObject[] so=GameObject.FindGameObjectsWithTag("PlayerName");
        foreach(GameObject temp in so)
        {
            if(temp.GetComponent<Text>().text==NickName)
            {
                temp.transform.parent.gameObject.SetActive(false);
            }
        }
        
    }
    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene=false;
    }
    public void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene=false;
    }
    public void GoToLobby()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()//PhotonNetwork.LeaveRoom()이 실행되면 callback 된다
    {
        SceneManager.LoadScene(0);
    }
}
