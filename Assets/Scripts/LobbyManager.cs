using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks//for using PUN2 Network callback
{
   [Header("---UI Screens---")]
   public GameObject roomUI;
   public GameObject connectUI;

   [Header("---UI Text---")]
   public TMP_Text statusText;
   public TMP_Text connectingText;

    [Header("---UI InputFields---")]
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;

    private void Awake() 
    {
        PhotonNetwork.ConnectUsingSettings();
    }//Connect to PhotonNetwork based on UsingSetting
    //You can check your UsingSetting on your asset folder
    
    public override void OnConnectedToMaster()
    {
        connectingText.text="Joining Lobby...";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
    public override void OnJoinedLobby()
    {
        connectUI.SetActive(false);
        roomUI.SetActive(true);
        statusText.text="Joined To Lobby...";
    }// Called on entering a lobby on the Master Server. The actual room-list updates will call OnRoomListUpdate.
    
    public override void OnJoinedRoom()
    {
        int sizeOfPlayers=PhotonNetwork.CountOfPlayersInRooms;
        AssignTeam(sizeOfPlayers);
        PhotonNetwork.LoadLevel(1);
        //Loading the index 1 scene
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectUI.SetActive(true);
        roomUI.SetActive(false);
        connectingText.text="Disconnected..."+cause.ToString();
    }//If fail to connect...
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        int roomName=Random.Range(0,10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers=4;
        PhotonNetwork.CreateRoom(roomName.ToString(),roomOptions,TypedLobby.Default,null);//take above room options
    }//If this function is executed because of failure of Onclick_PlayNow()
    //Creating room with random number and join it.


    #region ButtonClicks
    public void OnClick_CreateBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers=4;
        PhotonNetwork.CreateRoom(createRoom.text,roomOptions,TypedLobby.Default,null);//take above room options
    }
    public void OnClick_JoinBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers=4;
        PhotonNetwork.JoinOrCreateRoom(joinRoom.text,roomOptions,TypedLobby.Default,null);
    }
    public void OnClick_PlayNow()
    {
        PhotonNetwork.JoinRandomRoom();
        statusText.text="Creating Room...Please Wait...";
        //Joining Random room if it is available
        //if it is not available and failed to connect, OnJoinRandomFailed function will be executed

    }//function for Button PlayNow

    #endregion

    #region My_Functions

    void AssignTeam(int sizeOfPlayer)
    {

        ExitGames.Client.Photon.Hashtable hash=new ExitGames.Client.Photon.Hashtable();
        if(sizeOfPlayer%2==0)
        {
            hash.Add("Team",0);
        }
        else
        {
            hash.Add("Team",1);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    #endregion


}
