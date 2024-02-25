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
    FishingRodSpawner rodSpawner;

    public GameObject hookedFish;

    [Header("Casting")]
    public float launchForce = 100f;
    public Vector3 launchDirection = new Vector3(1f, 0f, 0f);

    [Header("Reeling")]
    public float reelForce = .05f;

    [Header("Strings")]
    SpringJoint rodToBobberString;
    SpringJoint bobberToHookString;
    public float rodToBobberStringSlack = 20f; // this is how long the line is for casting. USED IN CASTING AS WELL AS LINE DISTANCE IN UICONTROLLER (PROGRESS BAR MAX)
    public float bobberToHookStringSlack = 5f; // this is how deep the line will go under the water

    [Header("Line Durability")]
    public float maxLineTension = 60f;
    public float lineMaxHealth = 100f;
    public float rodToBobberLineHealth;
    public float bobberToHookLineHealth;
    public bool RTBLineSnapped;
    public bool BTHLineSnapped;

    [Header("Line Renderer")]
    LineRenderer lineRenderer;
    GameObject hook;
    GameObject firePoint;

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
        rodSpawner = GameObject.Find("FishingRodSpawner").GetComponent<FishingRodSpawner>();

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
                        //Destroy(this.gameObject);
                        rodSpawner.DespawnRod();
                        //rodSpawner.SpawnRod();
                    }
                }

            }

            if (floater.isFloating == true)
            {
                isCasting = false;

                if (isCasted == true)
                {
                    if (!RTBLineSnapped && !BTHLineSnapped)
                    {
                    CalculateLineTension(); // calc damage
                    CalculateLineHealth(); // check damage vs health
                    }

                    return;
                }
                else
                {
                    DropHook(bobberToHookStringSlack);
                    RumbleManager.instance.RumblePulse(0.5f, .1f, 0.25f);
                    AudioManager.instance.RodBobberSplash();
            }
            }

    }

    public void InitializeRod()
    {
        //init GameManager
        gamestate = GameObject.Find("GameManager").GetComponent<GameStateMachine>();

        // init bobber
        bobber = GameObject.Find("Bobber");
        floater = bobber.GetComponent<Floater>();

        // init lines
        lineRenderer = bobber.GetComponent<LineRenderer>();
        rodToBobberString = GameObject.Find("Rod").GetComponent<SpringJoint>();
        bobberToHookString = bobber.GetComponent<SpringJoint>();

        // init hook
        hook = GameObject.Find("Hook");
        hookArt = GameObject.Find("HookArt");

        // init casting launch point
        firePoint = GameObject.Find("FirePoint");

        //Set Bools
        isReeled = true;
        isCasted = false;
        hookHasFish = false;
        RTBLineSnapped = false;
        BTHLineSnapped = false;

        //Set Line Healths
        rodToBobberLineHealth = lineMaxHealth;
        bobberToHookLineHealth = lineMaxHealth;


    Debug.Log("Rod Initialized");
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

        AudioManager.instance.RodCast();

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
        if (isCasted && !RTBLineSnapped && !BTHLineSnapped)
        {
            //isCasted = false;
            bobberToHookString.maxDistance = 0;
        }

        if (!isReeled)
        {
            //gamestate.Reeling();

            AudioManager.instance.RodReel();
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
        AudioManager.instance.FishHooked();
        if (hookArt != null)
        {
            hookArt.SetActive(false);
        }
    }

    void SetLineRendererPositions()
    {
        //if no lines are snapped
        if (!RTBLineSnapped && !BTHLineSnapped)
        {
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, firePoint.transform.position);
            lineRenderer.SetPosition(1, bobber.transform.position);
            lineRenderer.SetPosition(2, hook.transform.position);

            if (hookedFish != null)
            {
                lineRenderer.SetPosition(2, hookedFish.transform.position);
            }
        }

        //if rod to bobber snapped
        if (RTBLineSnapped)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, bobber.transform.position);
            lineRenderer.SetPosition(1, hook.transform.position);

            if (hookedFish != null)
            {
                lineRenderer.SetPosition(1, hookedFish.transform.position);
            }
        }

        //if bobber to hook snapped
        if (BTHLineSnapped)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, firePoint.transform.position);
            lineRenderer.SetPosition(1, bobber.transform.position);
        }

    }

    void CalculateLineTension()
    {
        if (Mathf.Round(rodToBobberString.currentForce.magnitude) > maxLineTension)
        {
            rodToBobberLineHealth--;
        }

        if (Mathf.Round(bobberToHookString.currentForce.magnitude) > maxLineTension)
        {
            bobberToHookLineHealth--;
        }
    }

    void CalculateLineHealth()
    {
        if (rodToBobberLineHealth <= 0)
        {
            SnapRTBLine();
        }
        
        if (bobberToHookLineHealth <= 0)
        {
            SnapBTHLine();
        }
    }

    void SnapRTBLine()
    {
        hookedFish.GetComponent<BoidBehavior>().Unhook();

        RTBLineSnapped = true;
        rodToBobberString.breakForce = 0;
        AudioManager.instance.RodLineBreak();

        // Change the name of the bobber to avoid conflicts
        bobber.name = "BrokenBobber";

        bobber.transform.SetParent(null);

        isReeled = true;
        isCasted = false;
        gamestate.resetReady = true;
        gamestate.Idle();

        lineRenderer.positionCount = 0;

        rodSpawner.DespawnRod();
    }
    void SnapBTHLine()
    {
        hookedFish.GetComponent<BoidBehavior>().Unhook();

        BTHLineSnapped = true;
        bobberToHookString.breakForce = 0;
        AudioManager.instance.RodLineBreak();
    }

}
