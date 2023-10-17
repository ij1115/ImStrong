using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public State state;

    public Slider hpSlider;

    public int hp { get; private set; }

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
        hpSlider = UIManager.Instance.uis[2].GetComponent<DungeonUi>().PlayerHpBarSet();
        hpSlider.maxValue = state.maxHp;
        hpSlider.minValue = 0;

        hp = state.maxHp;
        UIManager.Instance.uis[2].GetComponent<DungeonUi>().playerHp.text = hp + " / " + state.maxHp;
        hpSlider.value = hp;
    }

    public void OnDamage(int damage)
    {
        int hitDamage = damage - Mathf.RoundToInt((float)state.def * 0.1f);

        hp -= hitDamage;

        hpSlider.value = hp;
        UIManager.Instance.uis[2].GetComponent<DungeonUi>().playerHp.text = hp + " / " + state.maxHp;
      
        if (hp <=0 && !dead)
        {
            hp = 0;
            hpSlider.value = hp;
            UIManager.Instance.uis[2].GetComponent<DungeonUi>().playerHp.text = hp + " / " + state.maxHp;
            Die();
        }
    }

    public void RestoreHealth(int health)
    {
        if (dead)
        {
            return;
        }

        if(state.maxHp<hp+health)
        {
            health = state.maxHp - hp;
        }

        hp += health;
    }

    public void Die()
    {
        dead = true;
        UIManager.Instance.uis[2].GetComponent<DungeonUi>().gameOverUi.SetActive(true);
        GameManager.instance.isGameover = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dead)
            return;

        var item = other.GetComponent<IItem>();
        if(item != null)
        {
            item.Use(gameObject);
        }
    }
}
