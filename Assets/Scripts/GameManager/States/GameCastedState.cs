using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCastedState : IState
{
    GameStateMachine _gameState;
    public delegate void OnStateCasted();
    public static OnStateCasted onStateCasted;
    public GameCastedState(GameStateMachine gameState)
    {
        _gameState = gameState;
    }
    public void Enter()
    {
        Debug.Log("GAME STATE ENTER: CASTED");
        onStateCasted();
    }
    public void Tick()
    {

    }
    public void FixedTick()
    {

    }
    public void Exit()
    {
        Debug.Log("GAME STATE EXIT: CASTED");
    }
}
