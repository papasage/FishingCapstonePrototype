using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this is the interface for States. It is like a list of actions that a State can take.
public interface IState
{
    void Enter();
    void Tick();
    void FixedTick();
    void Exit();

}
