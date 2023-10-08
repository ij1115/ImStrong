using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterInfo : MonoBehaviour
{
    public State state;
    public MonsterType type { get; private set; }

    private int hp;

    public event Action onDeath;

    public bool dead { get; private set; }

    public void Awake()
    {
        state = new State();
        dead = false;
    }

    public void StateUpdate()
    {
        switch(type)
        {
            case MonsterType.Mob:
                state = StateManager.Instance.MobState();
                hp = state.maxHp;
                break;

            case MonsterType.SubBoss:
                state = StateManager.Instance.SubBossState();
                hp = state.maxHp;
                break;

            case MonsterType.Boss:
                state = StateManager.Instance.BossState();
                hp = state.maxHp;
                break;
        }

    }

    public void SetType(MonsterType type)
    {
        this.type = type;
    }

    public void OnDamage(int damage)
    {
        int hitDamage = damage - Mathf.RoundToInt((float)state.def * 0.1f);
        
        hp -= hitDamage;

        if (hp <= 0 && !dead)
        {
            Die();
        }
    }

    public void Die()
    {
        if(onDeath != null)
        {
            onDeath();
        }

        dead = true;
    }
}
