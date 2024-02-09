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
    [SerializeField] SpringJoint RodToBobber;
    [SerializeField] SpringJoint BobberToHook;
    [SerializeField] TMP_Text RTBForce;
    [SerializeField] TMP_Text RTBTorque;
    [SerializeField] TMP_Text BTHForce;
    [SerializeField] TMP_Text BTHTorque;

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
    }

    private void Start()
    {
        UpdateBoidCount();
    }

    private void Update()
    {
        RTBForce.text = Mathf.Round(RodToBobber.currentForce.magnitude).ToString();
        RTBTorque.text = Mathf.Round(RodToBobber.currentTorque.magnitude).ToString();
        BTHForce.text = Mathf.Round(RodToBobber.currentForce.magnitude).ToString();
        BTHTorque.text = Mathf.Round(RodToBobber.currentTorque.magnitude).ToString();
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
}
