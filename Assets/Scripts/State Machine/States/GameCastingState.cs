using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCastingState : IState
{
    GameStateMachine _gameState;

    public delegate void OnStateCasting();
    public static OnStateCasting onStateCasting;

    public GameCastingState(GameStateMachine gameState)
    {
        _gameState = gameState;
    }
    public void Enter()
    {
        Debug.Log("GAME STATE ENTER: CASTING");
        onStateCasting();

    }
    public void Tick()
    {

    }
    public void FixedTick()
    {

    }
    public void Exit()
    {
        Debug.Log("GAME STATE EXIT: CASTING");
    }
}
