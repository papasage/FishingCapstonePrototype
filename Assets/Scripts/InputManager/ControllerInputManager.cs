using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControllerInputManager : MonoBehaviour
{
    public delegate void OnCast();
    public static OnCast onCast;

    public delegate void OnEquip();
    public static OnEquip onEquip;

    public delegate void OnReel();
    public static OnReel onReel;

    [SerializeField] float joystickDeadzone = 0.5f;
    [SerializeField] float reelingSensitivity = 0.5f;

    //reel motion vaiables
    private Vector2 lastLeftStickValue = Vector2.zero;
    public bool isReeling;
    private bool canReel;


    void Update()
    {
        LeftStick();
        RightStick();
        
        AButton();
        BButton();
        XButton();
        YButton();
        StartButton();
        SelectButton();
        
        Reeling();

    }

    void LeftStick()
    {
        if (Input.GetAxisRaw("LeftStickX") < -joystickDeadzone)
        {
            //Debug.Log("Left Stick: Left");
        }

        if (Input.GetAxisRaw("LeftStickX") > joystickDeadzone)
        {
            //Debug.Log("Left Stick: Right");
        }

        if (Input.GetAxisRaw("LeftStickY") < -joystickDeadzone)
        {
            //Debug.Log("Left Stick: Up");
        }

        if (Input.GetAxisRaw("LeftStickY") > joystickDeadzone)
        {
            //Debug.Log("Left Stick: Down");
        }
    }
    void RightStick()
    {
        if (Input.GetAxisRaw("RightStickX") < -joystickDeadzone)
        {
            //Debug.Log("Right Stick: Left");
        }

        if (Input.GetAxisRaw("RightStickX") > joystickDeadzone)
        {
            //Debug.Log("Right Stick: Right");
        }

        if (Input.GetAxisRaw("RightStickY") < -joystickDeadzone)
        {
            //Debug.Log("Right Stick: Up");
        }

        if (Input.GetAxisRaw("RightStickY") > joystickDeadzone)
        {
            //Debug.Log("Right Stick: Down");
        }
    }
    void AButton()
    {
        if (Input.GetButtonDown("A"))
        {
            //Debug.Log("A Down");
            //onCast();
        }
    }
    void BButton()
    {
        if (Input.GetButtonDown("B"))
        {
            //Debug.Log("B Down");
        }
    }
    void XButton()
    {
        if (Input.GetButtonDown("X"))
        {
            //Debug.Log("X Down");
            //onEquip();
        }
    }
    void YButton()
    {
        if (Input.GetButtonDown("Y"))
        {
            //Debug.Log("Y Down");
        }
    }
    void StartButton()
    {
        if (Input.GetButtonDown("Start"))
        {
            //toggle debug mode

        }
    }
    void SelectButton()
    {
        if (Input.GetButtonDown("Select"))
        {
            //toggle debug mode
            UIController.instance.debugMode = !UIController.instance.debugMode;

        }
    }

    void Reeling()
    {
        // Check the left stick input
        Vector2 leftStickValue = new Vector2(Input.GetAxis("LeftStickX"), Input.GetAxis("LeftStickY"));

        // Check if there is any change in left stick input beyond the reelingSensitivity setting.
        if (leftStickValue.magnitude > reelingSensitivity && Vector2.Distance(leftStickValue, lastLeftStickValue) > reelingSensitivity)
        {
            // Trigger reeling
            isReeling = true;
        }
        else
        {
            // Stop reeling
            isReeling = false;
        }

        // Save the current left stick value for the next frame
        lastLeftStickValue = leftStickValue;

        if (isReeling)
        {
            Debug.Log("Reeling Motion Detected!");
            onReel();
        }
    }
}
