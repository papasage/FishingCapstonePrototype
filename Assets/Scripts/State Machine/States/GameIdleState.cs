using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameIdleState : IState
{
    GameStateMachine _gameState;

    public delegate void OnStateIdle();
    public static OnStateIdle onStateIdle;

    public GameIdleState(GameStateMachine gameState)
    {
        _gameState = gameState;
    }
    public void Enter()
    {
        Debug.Log("GAME STATE ENTER: IDLE");
        onStateIdle();

    }
    public void Tick() 
    {
        
    }
    public void FixedTick()
    {

    }
    public void Exit()
    {
        Debug.Log("GAME STATE EXIT: IDLE");
    }
}
