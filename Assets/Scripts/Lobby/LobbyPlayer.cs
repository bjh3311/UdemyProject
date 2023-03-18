using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    public GameObject playersContainer;
    public GameObject playerPrefab;
    public void AddPlayer(string playerName)
    {
        GameObject temp =Instantiate(playerPrefab,Vector3.zero,Quaternion.identity);
        temp.transform.GetChild(0).GetComponent<Text>().text=playerName;
        temp.transform.SetParent(playersContainer.transform);
        temp.transform.localScale=Vector3.one;
        temp.name=playerName;//이름 재지정
    }
    public void RemovePlayer(string playerName)
    {
        int playerCount=playersContainer.transform.childCount;
        for(int i=0; i<playerCount;i++)
        {
            if(playersContainer.transform.GetChild(i).name==playerName)
            {
                Destroy(playersContainer.transform.GetChild(i).gameObject);
                return ;
            }//각 플레이어의 Name 을 살펴보고 삭제해준다
        }
    }
}
