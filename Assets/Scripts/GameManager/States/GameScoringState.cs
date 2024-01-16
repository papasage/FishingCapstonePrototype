using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScoringState : IState
{
    GameStateMachine _gameState;
    public delegate void OnStateScoring();
    public static OnStateScoring onStateScoring;

    public GameScoringState(GameStateMachine gameState)
    {
        _gameState = gameState;
    }
    public void Enter()
    {
        Debug.Log("GAME STATE ENTER: SCORING");
        onStateScoring();
    }
    public void Tick()
    {

    }
    public void FixedTick()
    {

    }
    public void Exit()
    {
        Debug.Log("GAME STATE EXIT: SCORING");
    }
}
