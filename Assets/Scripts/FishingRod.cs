using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{

    GameStateMachine gamestate;
    GameObject bobber; //the bobber is what is controlling the line renderers and gets launched on cast
    Floater floater; //the floater is the part of the bobber that helps it float. 
    GameObject hook;

    [Header("Casting")]
    public float launchForce = 1f;
    public Vector3 launchDirection = new Vector3(1f, 0f, 0f);

    [Header("Reeling")]
    public float reelForce = .05f;

    [Header("Strings")]
    SpringJoint rodToBobberString;
    SpringJoint bobberToHookString;
    public float rodToBobberStringSlack = 20f;
    public float bobberToHookStringSlack = 5f;

    [Header("Audio")]
    AudioSource audioReeling;
    AudioSource audioCasting;

    [Header("Bools")]
    public bool isReeled;
    public bool isCasting;
    public bool isCasted;
    public bool hookSet;
    public bool bobberFloating;



    // Start is called before the first frame update
    void Awake()
    {
        gamestate = GameObject.Find("GameManager").GetComponent<GameStateMachine>();

        bobber = GameObject.Find("Bobber");
        floater = bobber.GetComponent<Floater>();
        hook = GameObject.Find("Hook");

        rodToBobberString = GameObject.Find("Rod").GetComponent<SpringJoint>();
        bobberToHookString = bobber.GetComponent <SpringJoint>();
        

        audioReeling = GameObject.Find("Reeling").GetComponent<AudioSource>();
        audioCasting = GameObject.Find("Casting").GetComponent<AudioSource>();

        isReeled = true;
        isCasted = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && isReeled)
        {
            Cast(launchForce);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Reel(reelForce);
        }

        if (rodToBobberString.maxDistance == 0)
        {
            isReeled = true;
            isCasted = false;
        }

        if (floater.isFloating == true)
        {
            isCasting = false;

            if (isCasted == true)
            {
                return;
            }
            else
            {
                DropHook(bobberToHookStringSlack);
            }
        }
    }

    void Cast(float strength)
    {
        isCasting = true;
        isReeled = false;
        gamestate.Casting();
        audioCasting.Play();
        bobber.GetComponent<Rigidbody>().AddForce(launchDirection.normalized * strength, ForceMode.Impulse);
        rodToBobberString.maxDistance = rodToBobberStringSlack;
    }

    void DropHook(float hookWeight)
    {
        isCasted = true;
        gamestate.Casted();
        bobberToHookString.maxDistance = hookWeight;
    }

    void Reel(float strength)
    {
        if (isCasted)
        {
            //isCasted = false;
            bobberToHookString.maxDistance = 0;
        }

        if (!isReeled)
        {
            gamestate.Reeling();
            audioReeling.Play();
            rodToBobberString.maxDistance -= strength;
        }
    }
}
