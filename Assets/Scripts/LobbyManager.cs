using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks
{
   [Header("---UI Screens---")]
   public GameObject roomUI;
   public GameObject connectUI;

   [Header("---UI Text---")]
   public Text statusText;
   public Text connectingText;

    [Header("---UI InputFields---")]
    public InputField createRoom;
    public InputField joinRoom;

    private void Awake() 
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    
}
