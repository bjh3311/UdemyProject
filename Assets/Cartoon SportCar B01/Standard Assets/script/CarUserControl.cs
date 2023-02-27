using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private int input;
        private CarController m_Car; // the car controller we want to use


        Forward forward;
        Backward backward;
        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }
        private void Start() 
        {
            forward=GameObject.Find("Forward").GetComponent<Forward>();
            backward=GameObject.Find("Backward").GetComponent<Backward>();

            Forward.instance.SetPlayer(this.gameObject);
            Backward.instance.SetPlayer(this.gameObject);
            
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
            m_Car.Move(h, v, v, 0f);
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
