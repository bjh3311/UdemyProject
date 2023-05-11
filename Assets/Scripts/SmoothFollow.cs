﻿using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    public bool shouldRotate = true;

    // The target we are following
    [HideInInspector]
    public Transform target;
    // The distance in the x-z plane to the target
    private float distance = 4.0f;
    // the height we want the camera to be above the target
    private float height = 3.0f;
    // How much we
    private float heightDamping = 1.0f;
    private float rotationDamping = 2.0f;
    float wantedRotationAngle;
    float wantedHeight;
    float currentRotationAngle;
    float currentHeight;
    Quaternion currentRotation;
    void LateUpdate()
    {
        if (target)//target이 있다면
        {
            // Calculate the current rotation angles
            wantedRotationAngle = target.eulerAngles.y;
            wantedHeight = target.position.y + height;
            currentRotationAngle = transform.eulerAngles.y;
            currentHeight = transform.position.y;
            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
            // Convert the angle into a rotation
            currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = target.position;
            transform.position -= currentRotation * Vector3.forward * distance;
            // Set the height of the camera
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
            // Always look at the target
            if (shouldRotate)
            {
                transform.LookAt(target);
            }            
        }
    }
}
