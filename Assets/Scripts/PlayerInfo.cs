using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public State state;

    private int hp;


    public void Awake()
    {
        state= new State();

    }
    public void StateUpdate()
    {
        StateManager.Instance.PlayerStateSet();
        state = StateManager.Instance.current;
    }

    public void SetUp()
    {
        hp = state.maxHp;
    }
}
