using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks//for using PUN2 Network callback
{
   [Header("---UI Screens---")]
   public GameObject roomUI;
   public GameObject connectUI;
   public GameObject lobbyUI;

   [Header("---UI Text---")]
   public TMP_Text statusText;
   public TMP_Text connectingText;
   public Text startBtnText;
   public Text lobbyText;

    [Header("---UI InputFields---")]
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;
    public Button startButton;
    public TMP_InputField userName;

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
        userName.text="Player" + Random.Range(100,999);
    }// Called on entering a lobby on the Master Server. The actual room-list updates will call OnRoomListUpdate.
    
    public override void OnJoinedRoom()
    {
        int sizeOfPlayers=PhotonNetwork.CountOfPlayersInRooms;
        AssignTeam(sizeOfPlayers);
        lobbyUI.SetActive(true);
        if(PhotonNetwork.IsMasterClient)//만약 내가 마스터 클라이언트라면
        {
            startBtnText.text="waiting for players";
        }
        else
        {
            startBtnText.text="Ready!";
        }
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
        if(string.IsNullOrEmpty(userName.text))//유저네임이 비어있다면
        {
            userName.text="Uses" + Random.Range(100,999);
        }
        PhotonNetwork.LocalPlayer.NickName = userName.text;
        //로컬플레이어의 닉네임설정

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

    public void OnClickStartButton()
    {
        if(!PhotonNetwork.IsMasterClient)//방장이 아닌 참가자라면
        {
            SendMsg();
            startButton.interactable=false;//한번 레디 누르면 못 바꾼다
            startBtnText.text="Wait...";
        }
        else
        {
            if(count==4)
            {
                lobbyText.text="All Set : Play the Game Scene";
                PhotonNetwork.LoadLevel(1);
            }
        }
    }

    #region RasieEvent

    private void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived+=OnEvent;
    }
    private void OnDisable() 
    {
        PhotonNetwork.NetworkingClient.EventReceived-=OnEvent;
    }
    enum EventCodes
    {
        ready=1
    }
    int count =1;
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object content=photonEvent.CustomData;
        EventCodes code=(EventCodes)eventCode;
        if(code==EventCodes.ready)//받은 이벤트의 분류가 ready라면
        {
            object[] datas=content as object[];
            if(PhotonNetwork.IsMasterClient)
            {
                count++;
                if(count==4)
                {
                    startBtnText.text="START !";
                }
                else
                {
                    startBtnText.text="Only "+ count + " / 4 players are Ready";
                }
            }
        }
    }
    
    private void SendMsg()
    {
        string message = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        object[] datas=new object[] { message };
        RaiseEventOptions options = new RaiseEventOptions
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.MasterClient,//마스터 클라이언트(방장)에게만 보낸다
        };
        SendOptions sendOp=new SendOptions();
        sendOp.Reliability=true;

        PhotonNetwork.RaiseEvent((byte)EventCodes.ready,datas,options,sendOp);
    }

    #endregion

}
