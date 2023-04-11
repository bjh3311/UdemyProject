using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpectateButton : MonoBehaviour ,IPointerClickHandler
{
    public GameObject target;
    GameObject spectateCamera;
    public void OnPointerClick(PointerEventData eventData)
    {
        spectateCamera = GameObject.Find("SpectateCamera");
        spectateCamera.GetComponent<Camera>().enabled=true;
        SmoothFollow smoothFollow = spectateCamera.GetComponent<SmoothFollow>();
        smoothFollow.target=target.transform;
        smoothFollow.enabled=true;
    }
}
