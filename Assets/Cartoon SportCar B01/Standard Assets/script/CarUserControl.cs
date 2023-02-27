using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour 
    {
        private int input;
        private CarController m_Car; // the car controller we want to use

        //ForwardButton forward;
        //BackwardButton backward;

        public GameObject carUI;
        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }
        private void Start() 
        {
            //forward=GameObject.Find("Forward").GetComponent<ForwardButton>();
            //backward=GameObject.Find("Backward").GetComponent<BackwardButton>();
            
            ForwardButton.instance.SetPlayer(this.gameObject);
            BackwardButton.instance.SetPlayer(this.gameObject);
            carUI.SetActive(false);
        }
        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = SimpleInput.GetAxis("Horizontal");
            float v = SimpleInput.GetAxis("Vertical");
            #if !MOBILE_INPUT
            float handbrake;
            if(Input.GetKey(KeyCode.Space))
            {
                handbrake=1;
            }
            else
            {
                handbrake=0;
            }
            m_Car.Move(h, input, input, handbrake);
            #else
            m_Car.Move(h, input, input, 0f);
            #endif
        }
        public void Forward()
        {
            input=1;
        }
        public void Backward()
        {
            input=-1;
        }
        public void PointerUp()
        {
            input=0;       
        }
    }
}
