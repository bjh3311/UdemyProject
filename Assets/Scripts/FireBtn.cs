using UnityEngine.EventSystems;
using UnityEngine;

public class FireBtn : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    public MyPlayer player;//Reference MyPlayer
    private bool firing =false;
    public void SetPlayer(MyPlayer _player)
    {
        player = _player;
    }
    private void Update() 
    {
        if(firing)
        {
            player.Fire();
        }    
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        firing=true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.FireUp();
        firing=false;
    }
}
