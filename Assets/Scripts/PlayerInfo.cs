using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public State state;

    private int hp;

    private bool dead = false;

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

    public void OnDamage(int damage)
    {
        int hitDamage = damage - Mathf.RoundToInt((float)state.def * 0.1f);

        hp -= damage;

        if(hp <=0 && !dead)
        {
            Die();
        }
    }

    public void Die()
    {
        dead = true;
    }
}
