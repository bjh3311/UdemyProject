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

    private GameObject deathScreen;
    public Text totalAlive;

    public Text pingrateTextx;
    void Awake()//First of all, make the player. It makes camera can track the player.
    {
        instance=this;
        PhotonNetwork.Instantiate(player.name,playerSpawnPosition.position,playerSpawnPosition.rotation);
    }
    

    private void Update() 
    {
        pingrateTextx.text=PhotonNetwork.GetPing().ToString();
    }
    public void Spectate();
    {
        deathScreen.SetActive(true);
    }

}
