using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public Transform playerSpawnPosition;
    void Awake()//First of all, make the player. It makes camera can track the player.
    {
        PhotonNetwork.Instantiate(player.name,playerSpawnPosition.position,playerSpawnPosition.rotation);
    }

}
