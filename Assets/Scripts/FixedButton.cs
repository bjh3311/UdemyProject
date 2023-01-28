using UnityEngine;
using UnityEngine.EventSystems;

public class FixedButton : MonoBehaviour,IPointerClickHandler
{
    public MyPlayer player;//Reference MyPlayer

    public void SetPlayer(MyPlayer _player)
    {
        player = _player;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        player.Jump();
    }

  

   
}
