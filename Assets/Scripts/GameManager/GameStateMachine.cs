using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : StateMachine
{
    BoidBehavior caughtFish;
    GameObject caughtFishDisplay;

    //GameStates Scripts made with the IState interface
    //Assets -> Scripts -> GameManager -> States
    public GameIdleState IdleState {get; private set;}
    public GameCastingState CastingState {get; private set;}
    public GameCastedState CastedState {get; private set;}
    public GameBiteState BiteState {get; private set;}
    public GameReelingState ReelingState {get; private set;}
    public GameLandingState LandingState {get; private set;}
    public GameFightingState FightingState {get; private set;}
    public GameScoringState ScoringState {get; private set;}

    private void Awake()
    {
        //instantiate states, then connect them to this script
        IdleState = new GameIdleState(this);
        CastingState = new GameCastingState(this);
        CastedState = new GameCastedState(this);
        BiteState = new GameBiteState(this);
        ReelingState = new GameReelingState(this);
        LandingState = new GameLandingState(this);
        FightingState = new GameFightingState(this);
        ScoringState = new GameScoringState(this);

        caughtFishDisplay = GameObject.Find("CaughtFishDisplayObject");
    }

    void Start()
    {
        //defined in the StateMachine base class
        //Start in idle mode
        ChangeState(IdleState);
    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            ChangeState(IdleState);
        }

        if (Input.GetKeyDown("2"))
        {
            ChangeState(CastingState);
        }

        if (Input.GetKeyDown("3"))
        {
            ChangeState(CastedState);
        }

        if (Input.GetKeyDown("4"))
        {
            ChangeState(BiteState);
        }

        if (Input.GetKeyDown("5"))
        {
            ChangeState(ReelingState);
        }

        if (Input.GetKeyDown("6"))
        {
            ChangeState(LandingState);
        }

        if (Input.GetKeyDown("7"))
        {
            ChangeState(FightingState);
        }

        if (Input.GetKeyDown("8"))
        {
            ChangeState(ScoringState);
        }
    }

    public void Idle()
    {
        ChangeState(IdleState);
    }

    public void Casting()
    {
        ChangeState(CastingState);
    }

    public void Casted()
    {
        ChangeState(CastedState);
    }

    public void Bite()
    {
        ChangeState(BiteState);
    }
    public void Reeling()
    {
        ChangeState(ReelingState);
    }
    public void Landing(BoidBehavior caught)
    {
        ChangeState(LandingState);

        caughtFish = caught;
        StartCoroutine(LandingCoroutine());
        
    }
    public void Fighting()
    {
        ChangeState(FightingState);

        
        Debug.Log("You caught a x" + caughtFish.sizeMultiplier + "-sized " + caughtFish.maidenName + " fish! It Was Lvl: " + caughtFish.foodScore);
    }
    public void Scoring()
    {
        ChangeState(ScoringState);
        caughtFishDisplay.GetComponent<RotateObject>().rotateEnabled = false;
    }

    IEnumerator LandingCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 2f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Instantiate(caughtFish.mesh, caughtFishDisplay.transform.position, caughtFishDisplay.transform.rotation, caughtFishDisplay.transform);
        caughtFishDisplay.GetComponent<RotateObject>().rotateEnabled = true;
        Fighting();
    }
}
