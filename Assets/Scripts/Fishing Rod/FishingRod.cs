using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishingRod : MonoBehaviour
{

    GameStateMachine gamestate;
    GameObject bobber; //the bobber is what gets launched on cast
    Floater floater; //the floater is the part of the bobber that helps it float. 
    GameObject hookArt;
    BoidBehavior caughtFish;
    FishingRodSpawner rodSpawner;
    LineRenderer lineRenderer;
    GameObject hook;
    GameObject firePoint;
    public GameObject hookedFish;
    ControllerInputManager inputManager;

    [Header("Casting")]
    public float launchForce = 100f;
    public Vector3 launchDirection = new Vector3(1f, 0f, 0f);

    [Header("Reeling")]
    public float reelForce = .05f;

    [Header("Strings")]
    public SpringJoint rodToBobberString;
    public SpringJoint bobberToHookString;
    public float rodToBobberStringSlack = 20f; // this is how long the line is for casting. USED IN CASTING AS WELL AS LINE DISTANCE IN UICONTROLLER (PROGRESS BAR MAX)
    public float bobberToHookStringSlack = 5f; // this is how deep the line will go under the water

    [Header("Line Durability")]
    public float maxLineTension = 60f;
    public float lineMaxHealth = 100f;
    public float rodToBobberLineHealth;
    public float bobberToHookLineHealth;
    public bool RTBLineSnapped;
    public bool BTHLineSnapped;
    bool isRegeneratingRTB = false;
    bool isRegeneratingBTH = false;

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
        inputManager = GameObject.Find("PlayerManager").GetComponent<ControllerInputManager>();

    }

    private void OnEnable()
    {
        ControllerInputManager.onReel += ReelInput;
    }

    private void Update()
    {
        if (Input.GetButtonDown("A") && isReeled)
        {
            Cast(launchForce);
        }

        if (!RTBLineSnapped && !BTHLineSnapped)
        {
            CalculateLineHealth();
        }
            
        if (floater.isFloating == true)
        {
            isCasting = false;

            if (isCasted == true)
            {
                if (!RTBLineSnapped && !BTHLineSnapped)
                {
                    //CalculateLineTension();
                    //Debug.Log("CalculateLineHealth()");
                    //CalculateLineHealth(); // check damage vs health
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
    void FixedUpdate()
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
        if (isCasted)
        {
            //isCasted = false;
           // if (bobberToHookString != null && bobberToHookString.maxDistance != 0)
           // {
           //     bobberToHookString.maxDistance = 0;
           // }
        }

        if (!isReeled)
        {
            //gamestate.Reeling();

            AudioManager.instance.RodReel();
            RumbleManager.instance.RumblePulse(0.1f, .5f, 0.05f);
            rodToBobberString.maxDistance -= strength;
            CalculateLineTension(); // calc damage
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
        //bobberToHookString.spring = 70f;
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

        if (rodToBobberString != null)
        {
            if (Mathf.Round(rodToBobberString.currentForce.magnitude) > maxLineTension)
            {
                if (isRegeneratingRTB)
                {
                    StopCoroutine(RegenerateRTBHealth());
                    isRegeneratingRTB = false;
                }
                if (hookHasFish && inputManager.isReeling && rodToBobberLineHealth > 0)
                {
                    rodToBobberLineHealth--; // HIT ONCE

                    if (Mathf.Round(rodToBobberString.currentForce.magnitude) > maxLineTension * 2 && rodToBobberLineHealth > 0) // is the magnitude more than double the max tension?
                    {
                        rodToBobberLineHealth--; // HIT TWICE
                    }
                }

            }
        }
        
        if (bobberToHookString != null)
        {
            if (Mathf.Round(bobberToHookString.currentForce.magnitude) > maxLineTension)
            {
                if (isRegeneratingBTH)
                {
                    StopCoroutine(RegenerateBTHHealth());
                    isRegeneratingBTH = false;
                }
                if (hookHasFish && inputManager.isReeling && bobberToHookLineHealth > 0)
                {
                    bobberToHookLineHealth--; // HIT ONCE

                    if (Mathf.Round(bobberToHookString.currentForce.magnitude) > maxLineTension * 2 && bobberToHookLineHealth > 0) // is the magnitude more than double the max tension?
                    {
                        bobberToHookLineHealth--; // HIT TWICE
                    }
                }

            }
        }
        
    }
    void CalculateLineHealth()
    {
        //Debug.Log(rodToBobberLineHealth.ToString());
        //Debug.Log(bobberToHookLineHealth.ToString());

        // CALCULATE ROD-TO-BOBBER HEALTH
        if (rodToBobberLineHealth <= 0)
        {
            //Debug.Log("RTB Health Depleated: Stop Regen Coroutine");
            StopCoroutine(RegenerateRTBHealth());
            //Debug.Log("RTB Health Depleated: Firing SNAP Method");
            SnapRTBLine();
        }
        else if (rodToBobberLineHealth > lineMaxHealth)
        {
            rodToBobberLineHealth = lineMaxHealth;
        }
        else if (rodToBobberLineHealth < lineMaxHealth && !isRegeneratingRTB && !inputManager.isReeling && rodToBobberLineHealth > 0f)
        {
            StartCoroutine(RegenerateRTBHealth());
        }

        // CALCULATE BOBBER-TO-HOOK HEALTH
        if (bobberToHookLineHealth <= 0)
        {
            //Debug.Log("BTH Health Depleated: Stop Regen Coroutine");
            StopCoroutine(RegenerateBTHHealth());
            //Debug.Log("BTH Health Depleated: Firing SNAP Method");
            SnapBTHLine();
        }
        else if (bobberToHookLineHealth > lineMaxHealth)
        {
            bobberToHookLineHealth = lineMaxHealth;
        }
        else if (bobberToHookLineHealth < lineMaxHealth && !isRegeneratingBTH && !inputManager.isReeling && bobberToHookLineHealth > 0f)
        {
            StartCoroutine(RegenerateBTHHealth());
        }

    }
    void SnapRTBLine()
    {
        if (hookedFish != null)
        {
            hookedFish.GetComponent<BoidBehavior>().Unhook();
        }
      
        RTBLineSnapped = true;
        rodToBobberString.breakForce = 0;
        AudioManager.instance.RodLineBreak();
        RumbleManager.instance.RumblePulse(0.8f, .9f, 0.5f);

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
        if (hookedFish != null)
        {
            hookedFish.GetComponent<BoidBehavior>().Unhook();
        }

        BTHLineSnapped = true;

        bobberToHookString.breakForce = 0;

        AudioManager.instance.MusicFailure();
        AudioManager.instance.RodLineBreak();

        RumbleManager.instance.RumblePulse(0.8f, .9f, 0.5f);
    }
    IEnumerator RegenerateRTBHealth()
    {
        isRegeneratingRTB = true;
        yield return new WaitForSeconds(2f);

        while (rodToBobberLineHealth < lineMaxHealth && !inputManager.isReeling)
        {
            yield return new WaitForSeconds(0.1f);
            rodToBobberLineHealth++;
        }
        isRegeneratingRTB = false;
    }
    IEnumerator RegenerateBTHHealth()
    {
        isRegeneratingBTH = true;
        yield return new WaitForSeconds(2f);

        while (bobberToHookLineHealth < lineMaxHealth && !inputManager.isReeling)
        {
            yield return new WaitForSeconds(0.1f);
            bobberToHookLineHealth++;
        }
        isRegeneratingBTH = false;
    }
}
