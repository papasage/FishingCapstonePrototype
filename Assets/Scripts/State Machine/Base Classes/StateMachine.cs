using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public IState CurrentState { get; private set; }
    public IState _previousState;

    bool _inTransition = false;

    public void ChangeState(IState newState)
    {
        if (CurrentState == newState || _inTransition)
        {
            return;
        }
            
        ChangeStateRoutine(newState);
    }

    public void RevertState()
    {
        if (_previousState != null)
        {
            ChangeState(CurrentState);
        }
    }

    void ChangeStateRoutine(IState newState)
    {
        _inTransition = true;
        
        if (CurrentState != null) 
        { 
            CurrentState.Exit(); 
        }

        if (_previousState != null) 
        {
            _previousState = CurrentState;
        }

        CurrentState = newState;

        if(CurrentState != null) 
        {
            CurrentState.Enter();
        }

        _inTransition = false;
    }

    public void Update()
    {
        if (CurrentState !=null && !_inTransition)
        {
            CurrentState.Tick();
        }
    }

    public void FixedUpdate()
    {
        if (CurrentState !=null && !_inTransition)
        {
            CurrentState.FixedTick();
        }
    }
}
