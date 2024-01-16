using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameReelingState : IState
{
    GameStateMachine _gameState;
    public delegate void OnStateReeling();
    public static OnStateReeling onStateReeling;

    public GameReelingState(GameStateMachine gameState)
    {
        _gameState = gameState;

    }
    public void Enter()
    {
        Debug.Log("GAME STATE ENTER: REELING");
        onStateReeling();
    }
    public void Tick()
    {

    }
    public void FixedTick()
    {

    }
    public void Exit()
    {
        Debug.Log("GAME STATE EXIT: REELING");
    }
}
