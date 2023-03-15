using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;

using Photon.Pun;
using Photon.Realtime;

public class ChatMessageHandler : MonoBehaviour
{
    public Text messageReceived;
    enum EventCodes//다른이벤트에 다른 코드를 가질 수 있다
    {
        chatmessage ,
    }

    private void OnEnable() 
    {
        PhotonNetwork.NetworkingClient.EventReceived+=OnEvent;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived-=OnEvent;
    }
    public void OnEvent(EventData photonEvent)//포톤이벤트를 받는다
    {
        byte eventCode=photonEvent.Code;
        object content=photonEvent.CustomData;//메세지 내용을 받는다
        EventCodes code=(EventCodes)eventCode;
        if(code==EventCodes.chatmessage)//일어난 이벤트가 chatmessage라면
        {
            object[] datas=content as object[];
            messageReceived.text=(string)datas[0];
        }
    }//여기서 메세지를 받아준다
    public void SendMsg(string msg)//메세지를 보낸다
    {
        object[] datas=new object[] {msg};
        RaiseEventOptions options = new RaiseEventOptions
        {
            CachingOption=EventCaching.DoNotCache,//캐시를 허용하지않는다
            //이렇게 안해두면 플레이어가 들어올 때마다 계속 이벤트를 받아낸다
            Receivers = ReceiverGroup.All
            //모든 플레이어가 받게
        };
        SendOptions sendOp=new SendOptions();
        sendOp.Reliability=true;//Reliable한 송신 옵션

        PhotonNetwork.RaiseEvent((byte)EventCodes.chatmessage,datas,options,sendOp);
        //이벤트를 일으킨다
    }
}
