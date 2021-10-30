using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine<T> where T : class
{
    public IState<T> CurrentState;
    
    public void SetState(IState<T> state)
    {
        CurrentState?.OnStateExit();

        CurrentState = state;
        
        CurrentState?.OnStateEnter();
    }
}
