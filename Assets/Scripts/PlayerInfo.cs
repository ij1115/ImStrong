using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{  
    public StateInfo state { get; private set; }

    private int hp;

    public void StateUpdate()
    {
        state = StateManager.Instance.currentInfo;
    }

    public void SetUp()
    {
        hp = state.maxHp;
    }
}
