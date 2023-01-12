using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks//for using PUN2 Network callback
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
    }//Connect to PhotonNetwork based on UsingSetting
    public override void OnConnectedToMaster()
    {
        connectingText.text="Joining Lobby...";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
    public override void OnJoinedLobby()
    {
        connectUI.SetActive(false);
        roomUI.SetActive(true);
    }// Called on entering a lobby on the Master Server. The actual room-list updates will call OnRoomListUpdate.
    
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectUI.SetActive(true);
        roomUI.SetActive(false);
        connectingText.text="Disconnected..."+cause.ToString();

    }
    #region ButtonClicks
    public void CreateBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers=4;
        PhotonNetwork.CreateRoom(createRoom.text,roomOptions,TypedLobby.Default,null);//take above room options
    }
    public void JoinBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers=4;
        PhotonNetwork.JoinOrCreateRoom(joinRoom.text,roomOptions,TypedLobby.Default,null);
    }
    #endregion
}
