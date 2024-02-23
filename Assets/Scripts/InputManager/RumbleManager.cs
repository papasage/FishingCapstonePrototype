using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager instance;
    private Gamepad pad;

    private Coroutine stopRumbleAfterTimeCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void RumblePulse(float lowFrequencySpeed, float highFrequencySpeed, float duration)
    {
        pad = Gamepad.current;

        if (pad != null)
        {
            //start the rumble 
            pad.SetMotorSpeeds(lowFrequencySpeed, highFrequencySpeed);


            //stop the rumble
            stopRumbleAfterTimeCoroutine = StartCoroutine(StopRumble(duration, pad));
        }
    }    

    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        //once duration is over:
        pad.SetMotorSpeeds(0,0);

    }

}
