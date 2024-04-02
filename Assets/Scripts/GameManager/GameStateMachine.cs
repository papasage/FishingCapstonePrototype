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
    public bool rodSpawnerReady = false;
    public bool playerCaughtInputReady = false;

    [SerializeField] GameObject UI_EquipPrompt;
    [SerializeField] GameObject UI_CastPrompt;
    [SerializeField] GameObject UI_ReelPrompt;
    [SerializeField] GameObject UI_CaughtPrompt;
    [SerializeField] GameObject UI_CaughtCombo;

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


    new void Update()
    {

        if (playerCaughtInputReady)
        {
            if (Input.GetButtonDown("A"))
            {
                //CLEAR OUT THE CATCH WINDOW
                caughtFishDisplay.GetComponent<RotateObject>().rotateEnabled = false;
                UI_CaughtCombo.SetActive(false);
                UI_CaughtPrompt.SetActive(false);
                playerCaughtInputReady = false;
                rodSpawnerReady = true;
                Idle();
            }

            if (Input.GetButtonDown("B"))
            {
                //CLEAR OUT THE CATCH WINDOW
                caughtFishDisplay.GetComponent<RotateObject>().rotateEnabled = false;
                UI_CaughtCombo.SetActive(false);
                UI_CaughtPrompt.SetActive(false);
                playerCaughtInputReady = false;
                rodSpawnerReady = true;
                Idle();
            }
        }

        if (rodSpawnerReady)
        {
            if(Input.GetButtonDown("X"))
            {
                //SPAWN ROD
                rodSpawner.SpawnRod();
                rodSpawnerReady = false;
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

        rodSpawnerReady = true;

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
        //AudioManager.instance.MusicAction();
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
    public void FishCaught()
    {
        ChangeState(FightingState);

        if (trophy != null)
        {
            Destroy(trophy);
        }
        trophy = Instantiate(caughtFish.mesh, caughtFishDisplay.transform.position, caughtFishDisplay.transform.rotation, caughtFishDisplay.transform);
        caughtFishDisplay.GetComponent<RotateObject>().rotateEnabled = true;

        // AudioManager.instance.MusicFishCaught();

        UI_CaughtPrompt.SetActive(true);
       
        GameObject.Find("CaughtData_Breed").GetComponent<TMP_Text>().text = caughtFish.maidenName;
        GameObject.Find("CaughtData_Size").GetComponent<TMP_Text>().text = caughtFish.sizeMultiplier.ToString();
        GameObject.Find("CaughtData_Level").GetComponent<TMP_Text>().text = caughtFish.foodScore.ToString();

        if (caughtFish.comboMeter > 1)
        {
            UI_CaughtCombo.SetActive(true);
            //COMBO DATA
            GameObject.Find("CaughtData_Combo").GetComponent<TMP_Text>().text = "x" + caughtFish.comboMeter.ToString();
        }
        else UI_CaughtCombo.SetActive(false);


        playerCaughtInputReady = true;

        //StartCoroutine(FightingCoroutine());
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

        FishCaught();
    }

    IEnumerator FightingCoroutine()
    {
        
        if (trophy != null)
        {
            Destroy(trophy);
        }
        trophy = Instantiate(caughtFish.mesh, caughtFishDisplay.transform.position, caughtFishDisplay.transform.rotation, caughtFishDisplay.transform);
        caughtFishDisplay.GetComponent<RotateObject>().rotateEnabled = true;

       // AudioManager.instance.MusicFishCaught();

        UI_CaughtPrompt.SetActive(true);
        //GameObject.Find("CaughtData").GetComponent<TMP_Text>().text = "You caught a x" + caughtFish.sizeMultiplier + "-sized " + caughtFish.maidenName + " fish! It Was Lvl: " + caughtFish.foodScore;
        GameObject.Find("CaughtData_Breed").GetComponent<TMP_Text>().text = caughtFish.maidenName;
        GameObject.Find("CaughtData_Size").GetComponent<TMP_Text>().text = caughtFish.sizeMultiplier.ToString();
        GameObject.Find("CaughtData_Level").GetComponent<TMP_Text>().text = caughtFish.foodScore.ToString();
       
        //GameObject.Find("CaughtData_Song").GetComponent<TMP_Text>().text = caughtFish.favoriteSong;
        //Debug.Log("You caught a x" + caughtFish.sizeMultiplier + "-sized " + caughtFish.maidenName + " fish! It Was Lvl: " + caughtFish.foodScore);

        float elapsedTime = 0f;

        while (elapsedTime < 5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        caughtFishDisplay.GetComponent<RotateObject>().rotateEnabled = false;
        UI_CaughtPrompt.SetActive(false);
        rodSpawnerReady = true;
        Idle();
    }
}
