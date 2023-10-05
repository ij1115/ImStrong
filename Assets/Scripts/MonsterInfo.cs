using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo : MonoBehaviour
{
    public MonsterType type { get; private set; }
    public StateInfo state { get; private set; }

    private int hp;

    public void StateUpdate()
    {
        state = StateManager.Instance.monsterInfo;
    }

    public void SetType(MonsterType type)
    {
        this.type = type;
    }

    public void SetUp()
    {
        hp = state.maxHp;
    }
}
