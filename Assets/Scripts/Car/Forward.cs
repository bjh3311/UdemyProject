using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.Vehicles.Car;

public class Forward : MonoBehaviour ,IPointerDownHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    public static Forward instance=null;

    CarUserControl car;
    void Awake() 
    {
        if(instance==null)
        {
            instance=this;
        }   
    }
    public void SetPlayer(GameObject player)
    {
        car=player.GetComponent<CarUserControl>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        car.Forward();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        car.PointerUp();
    }
}
