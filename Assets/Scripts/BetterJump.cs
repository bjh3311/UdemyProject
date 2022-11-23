using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{

    private float fallMultiplayer = 2.5f;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb=this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.y<0)//떨어지는 와중이라면
        {
            rb.velocity+=Vector3.up*-9.8F*(fallMultiplayer)*Time.deltaTime;
        }
    }
}
