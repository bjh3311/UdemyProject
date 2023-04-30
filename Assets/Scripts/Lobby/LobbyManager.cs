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
   public GameObject lobbyUI;//로비 UI통째로

   [Header("---UI Text---")]
   public TMP_Text statusText;
   public Text connectingText;
   public Text startBtnText;
   public Text lobbyText;

    [Header("---UI InputFields---")]
    public InputField createRoom;
    public InputField joinRoom;
    public Button startButton;
    public InputField userName;

    private void Awake() 
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene=true;// 방장이 플레이 씬으로 넘어가면 다른 클라이언트도 넘어가게
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
    
    public override void OnJoinedRoom()//LocalPlayer가 방에 들어갈 때 실행된다
    {
        int sizeOfPlayers=PhotonNetwork.CurrentRoom.PlayerCount;
        //AssignTeam(sizeOfPlayers);
        lobbyUI.SetActive(true);//로비 UI를 켜준다
        roomUI.SetActive(false);
        foreach(Player temp in PhotonNetwork.CurrentRoom.Players.Values)//방에 들어와 있는 플레이어를 추가한다.
        {
            GetComponent<LobbyPlayer>().AddPlayer(temp.NickName);
        }

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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        GetComponent<LobbyPlayer>().AddPlayer(newPlayer.NickName);//새로들어온 플레이어를 추가해준다
    }//다른 플레이어가 방에 들어올 때 마다 실행된다

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        GetComponent<LobbyPlayer>().RemovePlayer(otherPlayer.NickName);
    }//다른 플레이어가 방에서 나갈때 마다 실행된다
    #region ButtonClicks
    public void OnClick_CreateBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers=4;
        PhotonNetwork.CreateRoom(createRoom.text,roomOptions,TypedLobby.Default,null);//take above room options
    }
    public void OnClick_JoinBtn()
    {
        PhotonNetwork.JoinRoom(joinRoom.text,null);
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

    /* #region My_Functions

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
    */

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
            if(count==4)//모든 플레이어가(4명) 준비 완료되었다면
            {
                lobbyText.text="All Set : Play the Game Scene";
                PhotonNetwork.LoadLevel(1);//1번째 씬을 불러온다
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
            if(PhotonNetwork.IsMasterClient)//내가 마스터클라이언트라면
            {
                count++;
                if(count==4)//모두 준비완료가 되었다면
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
    public void Exit()
    {
        #if UNITY_EDITOR //유니티 에디터에서 작동하게
            UnityEditor.EditorApplication.isPlaying=false;
        #else //실제 플레이환경에서 작동하게
            Application.Quit();
        #endif
    }

    #endregion

}
