using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{

    GameStateMachine gamestate;
    GameObject bobber; //the bobber is what gets launched on cast
    Floater floater; //the floater is the part of the bobber that helps it float. 
    GameObject hookArt;
    BoidBehavior caughtFish;

    public GameObject hookedFish;

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

    [Header("Line Renderer")]
    LineRenderer lineRenderer;
    GameObject hook;
    GameObject firePoint;


    [Header("Audio")]
    AudioSource audioReeling;
    AudioSource audioCasting;

    [Header("Bools")]
    public bool isReeled;
    public bool isCasting;
    public bool isCasted;
    public bool hookHasFish;
    public bool bobberFloating;


        // Start is called before the first frame update
        void Start()
    {
        InitializeRod();
    }

    private void OnEnable()
    {
        ControllerInputManager.onReel += ReelInput;
        //ControllerInputManager.onCast += CastInput;
    }

    // Update is called once per frame
    void Update()
    {
        if (hook != null && firePoint != null)
        {
            SetLineRendererPositions();
        }
        else
        {
            lineRenderer = GetComponent<LineRenderer>();
            hook = GameObject.Find("Hook");
            firePoint = GameObject.Find("FirePoint");
        }

        if (Input.GetButtonDown("A") && isReeled)
            {
                Cast(launchForce);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                //Reel(reelForce);
            }

            if (rodToBobberString != null && rodToBobberString.maxDistance == 0)
            {
                if (!isReeled)
                {
                    isReeled = true;
                    isCasted = false;

                    if (!hookHasFish)
                    {
                        gamestate.resetReady = true;
                        gamestate.Idle();
                        Destroy(this.gameObject);
                    }
                }

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
                    RumbleManager.instance.RumblePulse(0.5f, .1f, 0.25f);
                }
            }

    }

    public void InitializeRod()
    {
        Debug.Log("Initialize Rod");

        gamestate = GameObject.Find("GameManager").GetComponent<GameStateMachine>();

        bobber = GameObject.Find("Bobber");
        floater = bobber.GetComponent<Floater>();
        hookArt = GameObject.Find("HookArt");

        rodToBobberString = GameObject.Find("Rod").GetComponent<SpringJoint>();
        bobberToHookString = bobber.GetComponent<SpringJoint>();

        lineRenderer = bobber.GetComponent<LineRenderer>();
        hook = GameObject.Find("Hook");
        firePoint = GameObject.Find("FirePoint");

        audioReeling = GameObject.Find("Reeling").GetComponent<AudioSource>();
        audioCasting = GameObject.Find("Casting").GetComponent<AudioSource>();

        isReeled = true;
        isCasted = false;
        hookHasFish = false;
    }
    public void CastInput()
    {
        Cast(launchForce);
    }
    void Cast(float strength)
    {
        isCasting = true;
        isReeled = false;
        gamestate.Casting();

        audioCasting.Play();
        bobber.GetComponent<Rigidbody>().AddForce(launchDirection.normalized * strength, ForceMode.Impulse);
        rodToBobberString.maxDistance = rodToBobberStringSlack;
        RumbleManager.instance.RumblePulse(0.1f, .1f, 1.5f);
    }

    void DropHook(float hookWeight)
    {
        isCasted = true;
        gamestate.Casted();
        bobberToHookString.maxDistance = hookWeight;
    }
    public void ReelInput()
    {
        Reel(reelForce);
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
            //gamestate.Reeling();
            audioReeling.Play();
            RumbleManager.instance.RumblePulse(0.1f, .5f, 0.05f);
            rodToBobberString.maxDistance -= strength;
        }
    }

    public void Catch(BoidBehavior caught)
    {
        caughtFish = caught;
        gamestate.Landing(caughtFish);
    }

    public void Bite()
    {
        gamestate.Bite();
        RumbleManager.instance.RumblePulse(1f, 1f, 1f);
        if (hookArt != null)
        {
            hookArt.SetActive(false);
        }
    }

    void SetLineRendererPositions()
    {
        // Set the positions of the line renderer to the positions of the start and end objects
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, firePoint.transform.position);
        lineRenderer.SetPosition(1, bobber.transform.position);
        lineRenderer.SetPosition(2, hook.transform.position);

        if (hookedFish != null)
        {
            lineRenderer.SetPosition(2, hookedFish.transform.position);
        }
    }


}
