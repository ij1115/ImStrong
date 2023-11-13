using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class _StateMachine : MonoBehaviour
{
    public BaseState currentState;
    private Dictionary<UnitState, BaseState> states = new Dictionary<UnitState, BaseState>();

    public void AddState(UnitState key, BaseState value)
    {
        if(!states.ContainsKey(key))
        {
            states.Add(key, value);
        }
    }

    public BaseState GetState(UnitState key)
    {
        if(states.TryGetValue(key, out BaseState value))
        {
            return value;
        }
        return null;
    }

    public void ChangeState(UnitState key)
    {
        currentState?.Close();

        if(states.TryGetValue(key, out BaseState value))
        {
            currentState = value;
        }

        currentState?.Open();
    }

    public void Update()
    {
        currentState?.OnUpdate();
    }

    public void FixedUpdate()
    {
        currentState?.OnFixedUpdate();
    }
}
