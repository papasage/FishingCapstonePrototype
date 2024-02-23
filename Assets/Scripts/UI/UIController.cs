using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIController : MonoBehaviour
{
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

    [Header("Fishing Line Distance Meter")]
    [SerializeField] public GameObject ProgressBarParent;
    [SerializeField] public Slider lineDistance;


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

    private void Start()
    {
        UpdateBoidCount();
    }

    private void Update()
    {
        CalculateLineData();
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
            RTBForce.text = Mathf.Round(RodToBobber.currentForce.magnitude).ToString();
            BTHForce.text = Mathf.Round(BobberToHook.currentForce.magnitude).ToString();

            if (RodToBobber.currentForce.magnitude > 60f)
            {
                RTBForce.color = Color.red;
            }
            else RTBForce.color = Color.white;

            if (BobberToHook.currentForce.magnitude > 60f)
            {
                BTHForce.color = Color.red;
            }
            else BTHForce.color = Color.white;

            lineDistance.value = RodToBobber.maxDistance;
        }
        else
        {
            RTBForce.text = "-";
            BTHForce.text = "-";
        }
    }
    public void InitializeRodUI(float lineSlack)
    {
        RodToBobber = GameObject.Find("Rod").GetComponent<SpringJoint>();
        BobberToHook = GameObject.Find("Bobber").GetComponent<SpringJoint>();

        lineDistance.maxValue = lineSlack;
    }
     void ShowReelProgressBar()
    {
        ProgressBarParent.SetActive(true);
    }
     void HideReelProgressBar()
    {
        ProgressBarParent.SetActive(false);
    }
}
