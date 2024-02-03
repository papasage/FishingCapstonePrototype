using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XboxControllerDebug : MonoBehaviour
{
    public int playerNumber = 1; // Set the player number for the controller

    void Update()
    {
        float leftStickX = Input.GetAxis("LeftStickX_" + playerNumber);
        float leftStickY = Input.GetAxis("LeftStickY_" + playerNumber);
        float rightStickX = Input.GetAxis("RightStickX_" + playerNumber);
        float rightStickY = Input.GetAxis("RightStickY_" + playerNumber);

        bool buttonA = Input.GetButton("A_" + playerNumber);
        bool buttonB = Input.GetButton("B_" + playerNumber);
        bool buttonX = Input.GetButton("X_" + playerNumber);
        bool buttonY = Input.GetButton("Y_" + playerNumber);

        // Print values to the console
        Debug.Log($"Left Stick: ({leftStickX}, {leftStickY})");
        Debug.Log($"Right Stick: ({rightStickX}, {rightStickY})");
        Debug.Log($"Buttons: A={buttonA}, B={buttonB}, X={buttonX}, Y={buttonY}");
    }
}
