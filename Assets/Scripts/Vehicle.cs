//Adapted from Unity Game Development Cookbook, published by O'Reilly Media in March 2019. My update to this allows the 
//player to slow down and drive backwards.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Wheel
{
    public WheelCollider collider;

    public bool powered;

    public bool steerable;

    public bool hasBrakes;
}
public class Vehicle : MonoBehaviour
{
    [SerializeField] Wheel[] wheels = { };

    [SerializeField] float motorTorque = 1000;

    [SerializeField] float brakeTorque = 2000;

    [SerializeField] float steeringAngle = 45;
    string verticalInput;
    string horizontalInput;

    Rigidbody rb;
    Transform resetTransform;
    public enum Player { PlayerOne, playerTwo};

    public Player vehiclesPlayer;  //If set to PlayerOne, use WASD. Otherwise, use arrow keys

    private void Start()
    {
        switch (vehiclesPlayer)
        {
            case Player.PlayerOne:
                verticalInput = "VerticalOne";
                horizontalInput = "HorizontalOne";
                break;

            case Player.playerTwo:
                verticalInput = "VerticalTwo";
                horizontalInput = "HorizontalTwo";
                break;
        }
        rb = GetComponent<Rigidbody>();
        resetTransform = transform;
    }
    private void Update()
    {
        var vertical = Input.GetAxis(verticalInput);


        float motorTorqueToApply;
        float brakeTorqueToApply;

        var currentSteeringAngle = Input.GetAxis(horizontalInput) * steeringAngle;
    
        if (transform.rotation.z >= 90 || transform.rotation.z <= -90)
        {
            StartCoroutine(ResetPosition(resetTransform));
        }
        if (vertical != 0) //Pressing forward or backwards
        {
            
            motorTorqueToApply = vertical * motorTorque;
            brakeTorqueToApply = 0;

            if (vertical < 0 && rb.velocity.y < -0.5) //Invert turning if moving backwards
            {
                currentSteeringAngle = -currentSteeringAngle;
            }
        }
        else 
        {
            motorTorqueToApply = 0;
            brakeTorqueToApply = Mathf.Abs(vertical) * brakeTorque;
        }
        for (int wheelNum = 0; wheelNum < wheels.Length; wheelNum++)
        {
            var wheel = wheels[wheelNum];

            if (wheel.powered)
            {
                wheel.collider.motorTorque = motorTorqueToApply;
            }

            if (wheel.steerable)
            {
                wheel.collider.steerAngle = currentSteeringAngle;
            }

            if (wheel.hasBrakes)
            {
                wheel.collider.brakeTorque = brakeTorqueToApply; 
            }
        }
    }
    /// <summary>
    /// Reset position time set to 5 seconds by default.
    /// </summary>
    /// <param name="positionToResetTo"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    IEnumerator ResetPosition(Transform positionToResetTo, float seconds = 5f)
    {
        Debug.Log($"{transform.name} flipped over. Resetting position...");

        yield return new WaitForSeconds(seconds);

        transform.position = positionToResetTo.position;
        transform.rotation = positionToResetTo.rotation;

        Debug.Log("Be careful next time!");
    }
}
