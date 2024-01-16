using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLandingState : IState
{
    GameStateMachine _gameState;
    public delegate void OnStateLanding();
    public static OnStateLanding onStateLanding;

    public GameLandingState(GameStateMachine gameState)
    {
        _gameState = gameState;
    }
    public void Enter()
    {
        Debug.Log("GAME STATE ENTER: LANDING");
        onStateLanding();
    }
    public void Tick()
    {

    }
    public void FixedTick()
    {

    }
    public void Exit()
    {
        Debug.Log("GAME STATE EXIT: LANDING");
    }
}
