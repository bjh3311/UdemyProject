using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class GameManager : MonoBehaviour
{

    public static GameManager instance =null;
    // Start is called before the first frame update
    public GameObject player;
    public Transform playerSpawnPosition;

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
        PhotonNetwork.Instantiate(player.name,playerSpawnPosition.position,playerSpawnPosition.rotation);
    }
    void Start()
    {
        PhotonNetwork.SendRate=25;
        PhotonNetwork.SerializationRate=15;
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
        if(daedPlayer==totalPlayer-1)//한명 빼고 다 죽었다는게 
        {
            foreach(GameObject temp in players)
            {
                temp.GetPhotonView().RPC("Iwin",RpcTarget.All);
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
        PhotonNetwork.LoadLevel(0);//1번째 씬을 불러온다
    }
}
