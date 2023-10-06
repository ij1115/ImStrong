using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public State state;

    public UnitState unitState { get; private set; }

    private int hp;


    public void Awake()
    {
        state= new State();
    }
    public void StateUpdate()
    {
        StateManager.Instance.PlayerStateSet();
        state = StateManager.Instance.current;
        unitState = UnitState.NIdle;
    }

    public void SetUp()
    {
        hp = state.maxHp;
    }

    public void ChangeUnitState(UnitState uState)
    {
        unitState = uState;
    }
}
