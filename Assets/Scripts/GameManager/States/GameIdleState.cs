using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameIdleState : IState
{
    GameStateMachine _gameState;

    public delegate void OnStateIdle();
    public static OnStateIdle onStateIdle;

    FishingRodSpawner rodSpawner;
    bool castingReady = false;

    public GameIdleState(GameStateMachine gameState)
    {
        _gameState = gameState;
    }
    public void Enter()
    {
        Debug.Log("GAME STATE ENTER: IDLE");
        
        //throw the state flag
        onStateIdle();

        //Set Idle Ambience
        AudioManager.instance.AmbienceDock();
        //Set Idle Music
        AudioManager.instance.MusicPeaceful();

        //Find Rod Spawner
        rodSpawner = GameObject.Find("FishingRodSpawner").GetComponent<FishingRodSpawner>();

        //Tell UI Controller to go into Idle Mode
        UIController.instance.UIIdle();
    }
    public void Tick() 
    {
        Debug.Log("IDLETICK");

        if (castingReady == false)
        {
            if (Input.GetButtonDown("X"))
            {
                rodSpawner.SpawnRod();
                UIController.instance.UIIdleCastPrompt();
                castingReady = true;
            }
        }

        if (castingReady == true)
        {

        }

    }
    public void FixedTick()
    {

    }
    public void Exit()
    {
        Debug.Log("GAME STATE EXIT: IDLE");
    }
}
