using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo : MonoBehaviour
{
    public State state;
    public MonsterType type { get; private set; }

    private int hp;

    public void Awake()
    {
        state = new State();
    }
    public void StateUpdate()
    {
        switch(type)
        {
            case MonsterType.Mob:
                state = StateManager.Instance.MobState();
                break;

            case MonsterType.SubBoss:
                state = StateManager.Instance.SubBossState();
                break;

            case MonsterType.Boss:
                state = StateManager.Instance.BossState();
                break;
        }

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
