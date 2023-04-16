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

    public Text pingrateTextx;

    public GameObject spectateContainer;
    public GameObject spectateObject;
    void Awake()//First of all, make the player. It makes camera can track the player.
    {
        instance=this;
        PhotonNetwork.Instantiate(player.name,playerSpawnPosition.position,playerSpawnPosition.rotation);
    }
    

    private void Update() 
    {
        pingrateTextx.text=PhotonNetwork.GetPing().ToString();
    }


    public void Spectate()
    {
        deathScreen.SetActive(true);
        FindAllPlayer();
    }
    void FindAllPlayer()
    {
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
                so.transform.Find("SpectateBtn").GetComponent<SpectateButton>().target = player;
            }
        }
    }

}
