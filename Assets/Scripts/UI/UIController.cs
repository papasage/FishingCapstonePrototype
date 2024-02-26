using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
//using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    FishingRod currentRod;

    [Header("Boid Counter Data")]
    [SerializeField] TMP_Text UIBoidCountText;
    private List<GameObject> boids;

    [Header("State Tracker Data")]
    [SerializeField] TMP_Text UIStateText;
    [SerializeField] Image UIStateSprite;
    [SerializeField] Sprite _stateIdle;
    [SerializeField] Sprite _stateCasting;
    [SerializeField] Sprite _stateCasted;
    [SerializeField] Sprite _stateBite;
    [SerializeField] Sprite _stateReeling;
    [SerializeField] Sprite _stateLanding;
    [SerializeField] Sprite _stateFighting;
    [SerializeField] Sprite _stateScoring;

    [Header("Fishing Line Tension Data")]
    public bool rodIsEquipped;
    [SerializeField] SpringJoint RodToBobber;
    [SerializeField] SpringJoint BobberToHook;
    [SerializeField] TMP_Text RTBForce;
    [SerializeField] TMP_Text BTHForce;
    [SerializeField] TMP_Text RTBDamage;
    [SerializeField] TMP_Text BTHDamage;
    [SerializeField] TMP_Text RTBLength;
    [SerializeField] TMP_Text BTHLength;

    [Header("Fishing Line Distance Meter")]
    [SerializeField] public GameObject ProgressBarParent;
    [SerializeField] public Slider lineDistance;
    private Image progressBarFill;
    private Color lerpedColor;
    [SerializeField] public Color colorHealthMax;
    [SerializeField] public Color colorHealthDepleated;

    [Header("Debug Menu")]
    public bool debugMode = false;
    [SerializeField] GameObject debugMenu;


    private void OnEnable()
    {
        GameIdleState.onStateIdle += UIIdle;
        GameCastingState.onStateCasting += UICasting;
        GameCastedState.onStateCasted += UICasted;
        GameBiteState.onStateBite += UIBite;
        GameReelingState.onStateReeling += UIReeling;
        GameLandingState.onStateLanding += UILanding;
        GameFightingState.onStateFighting += UIFighting;
        GameScoringState.onStateScoring += UIScoring;

        FishSpawner.onSpawn += UpdateBoidCount;
        BoidBehavior.onDeath += UpdateBoidCount;

        //handle progress bar
        GameCastedState.onStateCasted += ShowReelProgressBar;
        GameLandingState.onStateLanding += HideReelProgressBar;
        GameIdleState.onStateIdle += HideReelProgressBar;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        UpdateBoidCount();

    }

    private void Update()
    {
        CalculateLineData();

        if (ProgressBarParent.activeInHierarchy)
        {
            ProgressBarColor();
        }

        if (rodIsEquipped)
        {
            if(currentRod.RTBLineSnapped || currentRod.BTHLineSnapped)
            {
                HideReelProgressBar();
            }
            CalculateLineLength();
        }

        if (!debugMode)
        {
            debugMenu.SetActive(false);
        }
        else debugMenu.SetActive(true);


    }

    void UpdateBoidCount()
    {
        boids = new List<GameObject>(GameObject.FindObjectsOfType<BoidBehavior>().Select(boid => boid.gameObject));
        
        int UICount = 0;
        foreach (GameObject boid in boids)
        {
            BoidBehavior boidBehavior = boid.GetComponent<BoidBehavior>();
            
            if (boidBehavior != null)
            {
                if (boidBehavior.isDead == false)
                {
                    UICount++;
                }
            }
        }
        //Debug.Log("Boids Initialized! Found Boids: " + boids.Count);
        UIBoidCountText.text = UICount.ToString();
    }

    void UIIdle()
    {
        UIStateSprite.sprite = _stateIdle;
        UIStateText.text = "Idle";
    }
    void UICasting()
    {
        UIStateSprite.sprite = _stateCasting;
        UIStateText.text = "Casting";
    }
    void UICasted()
    {
        UIStateSprite.sprite = _stateCasted;
        UIStateText.text = "Casted";
    }
    void UIBite()
    {
        UIStateSprite.sprite = _stateBite;
        UIStateText.text = "Bite";
    }
    void UIReeling()
    {
        UIStateSprite.sprite = _stateReeling;
        UIStateText.text = "Reeling";
    }
    void UILanding()
    {
        UIStateSprite.sprite = _stateLanding;
        UIStateText.text = "Landing";
    }
    void UIFighting()
    {
        UIStateSprite.sprite = _stateFighting;
        UIStateText.text = "Caught!";
    }
    void UIScoring()
    {
        UIStateSprite.sprite = _stateScoring;
        UIStateText.text = "Shop";
    }

    void CalculateLineData()
    {
        if (rodIsEquipped)
        {
            if (currentRod.RTBLineSnapped)
            {
                RTBDamage.color = Color.red;
                RTBDamage.text = "BREAK";
                return;
            }

            if (currentRod.BTHLineSnapped)
            {
                BTHDamage.color = Color.red;
                BTHDamage.text = "BREAK";
                return;
            }

            RTBForce.text = Mathf.Round(RodToBobber.currentForce.magnitude).ToString();
            BTHForce.text = Mathf.Round(BobberToHook.currentForce.magnitude).ToString();

            if (RodToBobber.currentForce.magnitude > currentRod.maxLineTension)
            {
                RTBForce.color = Color.red;
            }
            else RTBForce.color = Color.white;

            if (BobberToHook.currentForce.magnitude > currentRod.maxLineTension)
            {
                BTHForce.color = Color.red;
            }
            else BTHForce.color = Color.white;

            lineDistance.value = RodToBobber.maxDistance;

            if (currentRod != null)
            {
                RTBDamage.color = Color.white;
                RTBDamage.text = currentRod.rodToBobberLineHealth.ToString();
                BTHDamage.color = Color.white;
                BTHDamage.text = currentRod.bobberToHookLineHealth.ToString();
            }
            
        }
        else
        {
            RTBForce.text = "-";
            BTHForce.text = "-";
            RTBDamage.text = "-";
            BTHDamage.text = "-";
        }
    }
    void CalculateLineLength()
    {
        if (currentRod.rodToBobberString != null)
        {
            RTBLength.text = Mathf.Round(currentRod.rodToBobberString.maxDistance).ToString();
        }

        if (currentRod.bobberToHookString != null)
        {
            BTHLength.text = Mathf.Round(currentRod.bobberToHookString.maxDistance).ToString();
        }
    }
    public void InitializeRodUI(float lineSlack)
    {
        RodToBobber = GameObject.Find("Rod").GetComponent<SpringJoint>();
        BobberToHook = GameObject.Find("Bobber").GetComponent<SpringJoint>();

        lineDistance.maxValue = lineSlack;

        currentRod = GameObject.Find("FishingRod").GetComponent<FishingRod>();
    }
     void ShowReelProgressBar()
    {
        ProgressBarParent.SetActive(true);
        progressBarFill = GameObject.Find("ReelBarFillColor").GetComponent<Image>();
    }
     void HideReelProgressBar()
    {
        ProgressBarParent.SetActive(false);
    }

    void ProgressBarColor()
    {
        if (rodIsEquipped && currentRod != null)
        {
            float BTHHealthPercentage = currentRod.bobberToHookLineHealth / currentRod.lineMaxHealth;
            float RTBHealthPercentage = currentRod.rodToBobberLineHealth / currentRod.lineMaxHealth;

            float healthPercentage = Mathf.Min(BTHHealthPercentage, RTBHealthPercentage);

            // Ensure the percentage is between 0 and 1
            healthPercentage = Mathf.Clamp01(healthPercentage);

            // Lerp the color based on the percentage
            lerpedColor = Color.Lerp(colorHealthDepleated, colorHealthMax, healthPercentage);

            // Assign the lerped color to the progressBarFill
            progressBarFill.color = lerpedColor;
        }
    }
}
