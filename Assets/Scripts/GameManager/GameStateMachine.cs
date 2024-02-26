using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateMachine : StateMachine
{
    FishingRodSpawner rodSpawner;

    BoidBehavior caughtFish;        //the fish data that was caught
    GameObject caughtFishDisplay;   //the object that parents the trophy
    GameObject trophy;              //the instance of the caughtFish that is displayed
    public bool resetReady = false;

    [SerializeField] GameObject UI_EquipPrompt;
    [SerializeField] GameObject UI_CastPrompt;
    [SerializeField] GameObject UI_ReelPrompt;
    [SerializeField] GameObject UI_CaughtPrompt;

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
        rodSpawner = GameObject.Find("FishingRodSpawner").GetComponent<FishingRodSpawner>();

        UI_EquipPrompt.SetActive(false);
        UI_CastPrompt.SetActive(false);
        UI_ReelPrompt.SetActive(false);
        UI_CaughtPrompt.SetActive(false);


    }
    private void Start()
    {
        //defined in the StateMachine base class
        //Start in idle mode
        Idle();
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

        if (resetReady)
        {
            if(Input.GetButtonDown("X"))
            {
                rodSpawner.SpawnRod();
                resetReady = false;
                UI_EquipPrompt.SetActive(false);
                UI_CastPrompt.SetActive(true);
            }
        }
    }

    public void Idle()
    {
        ChangeState(IdleState);

        AudioManager.instance.AmbienceDock();
        AudioManager.instance.MusicPeaceful();

        resetReady = true;

        UI_EquipPrompt.SetActive(true);

        if (trophy != null)
        {
            Destroy(trophy);
        }
    }

    public void Casting()
    {
        ChangeState(CastingState);
        UI_CastPrompt.SetActive(false);
    }

    public void Casted()
    {
        ChangeState(CastedState);
    }

    public void Bite()
    {
        ChangeState(BiteState);
        AudioManager.instance.MusicAction();
        StartCoroutine(BiteCoroutine());
    }
    public void Reeling()
    {
        ChangeState(ReelingState);
        UI_ReelPrompt.SetActive(false);
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
        StartCoroutine(FightingCoroutine());
    }
    public void Scoring()
    {
        ChangeState(ScoringState);
        
    }

    IEnumerator BiteCoroutine()
    {
        float elapsedTime = 0f;

        UI_ReelPrompt.SetActive(true);

        while (elapsedTime < 4f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Reeling();
    }
    IEnumerator LandingCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < .9f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Fighting();
    }

    IEnumerator FightingCoroutine()
    {
        float elapsedTime = 0f;

        if (trophy != null)
        {
            Destroy(trophy);
        }
        trophy = Instantiate(caughtFish.mesh, caughtFishDisplay.transform.position, caughtFishDisplay.transform.rotation, caughtFishDisplay.transform);
        caughtFishDisplay.GetComponent<RotateObject>().rotateEnabled = true;

        AudioManager.instance.MusicFishCaught();

        UI_CaughtPrompt.SetActive(true);
        //GameObject.Find("CaughtData").GetComponent<TMP_Text>().text = "You caught a x" + caughtFish.sizeMultiplier + "-sized " + caughtFish.maidenName + " fish! It Was Lvl: " + caughtFish.foodScore;
        GameObject.Find("CaughtData_Breed").GetComponent<TMP_Text>().text = caughtFish.maidenName + " Fish";
        GameObject.Find("CaughtData_Size").GetComponent<TMP_Text>().text = caughtFish.sizeMultiplier.ToString();
        GameObject.Find("CaughtData_Level").GetComponent<TMP_Text>().text = caughtFish.foodScore.ToString();
        GameObject.Find("CaughtData_Song").GetComponent<TMP_Text>().text = caughtFish.favoriteSong;

        //Debug.Log("You caught a x" + caughtFish.sizeMultiplier + "-sized " + caughtFish.maidenName + " fish! It Was Lvl: " + caughtFish.foodScore);

        while (elapsedTime < 5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        caughtFishDisplay.GetComponent<RotateObject>().rotateEnabled = false;
        UI_CaughtPrompt.SetActive(false);
        resetReady = true;
        Idle();
    }
}
