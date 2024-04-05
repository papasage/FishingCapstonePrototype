using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBiteState : IState
{
    GameStateMachine _gameState;
    public delegate void OnStateBite();
    public static OnStateBite onStateBite;

    public GameBiteState(GameStateMachine gameState)
    {
        _gameState = gameState;
    }
    public void Enter()
    {
        Debug.Log("GAME STATE ENTER: BITE");
        onStateBite();
    }
    public void Tick()
    {

    }
    public void FixedTick()
    {

    }
    public void Exit()
    {
        Debug.Log("GAME STATE EXIT: BITE");
    }
}
