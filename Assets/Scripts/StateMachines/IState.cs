using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T>
{
    void Tick();
    void OnStateEnter();
    void OnStateExit();
}