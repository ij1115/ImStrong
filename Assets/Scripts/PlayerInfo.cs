using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public State state;

    public Slider hpSlider;

    private int hp;

    public bool dead { get; private set; }

    public void Awake()
    {
        state= new State();
        dead = false;
    }
    public void StateUpdate()
    {
        StateManager.Instance.PlayerStateSet();
        state = StateManager.Instance.current;
    }

    public void SetUp()
    {
        hpSlider = UIManager.Instance.PlayerHpBarSet();
        hpSlider.maxValue = state.maxHp;
        hpSlider.minValue = 0;

        hp = state.maxHp;
        UIManager.Instance.playerHp.text = hp + " / " + state.maxHp;
        hpSlider.value = hp;
    }

    public void OnDamage(int damage)
    {
        int hitDamage = damage - Mathf.RoundToInt((float)state.def * 0.1f);

        hp -= hitDamage;

        hpSlider.value = hp;
        UIManager.Instance.playerHp.text = hp + " / " + state.maxHp;
      
        if (hp <=0 && !dead)
        {
            hp = 0;
            hpSlider.value = hp;
            UIManager.Instance.playerHp.text = hp + " / " + state.maxHp;
            Die();
        }
    }

    public void Die()
    {
        dead = true;
        UIManager.Instance.gameOverUi.SetActive(true);
        GameManager.instance.isGameover = true;
    }
}
