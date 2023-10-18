using System;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfo : MonoBehaviour
{
    public State state;

    public Slider hpSlider;
    public MonsterType type { get; private set; }

    public int hp;

    public event Action onDeath;

    public bool dead { get; private set; }

    public SOItem[] normalDrop;
    public SOItem[] subBossDrop;
    public SOItem[] bossDrop;

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
                hpSlider = UIManager.Instance.uis[2].GetComponent<DungeonUi>().MonsterHpBarSet();
                hpSlider.maxValue = state.maxHp;
                hpSlider.minValue = 0;

                hp = state.maxHp;

                hpSlider.value = hp;
                UIManager.Instance.uis[2].GetComponent<DungeonUi>().bossHp.text = hp + " / " + state.maxHp;
                UIManager.Instance.uis[2].GetComponent<DungeonUi>().bossType.text = "SubBoss";
                break;

            case MonsterType.Boss:
                state = StateManager.Instance.BossState();
                hpSlider = UIManager.Instance.uis[2].GetComponent<DungeonUi>().MonsterHpBarSet();
                hpSlider.maxValue = state.maxHp;
                hpSlider.minValue = 0;

                hp = state.maxHp;

                hpSlider.value = hp;
                UIManager.Instance.uis[2].GetComponent<DungeonUi>().bossHp.text = hp + " / " + state.maxHp;
                UIManager.Instance.uis[2].GetComponent<DungeonUi>().bossType.text = "Boss";
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

        if(type == MonsterType.SubBoss || type == MonsterType.Boss) 
        { 
        hpSlider.value = hp;
        UIManager.Instance.uis[2].GetComponent<DungeonUi>().bossHp.text = hp + " / " + state.maxHp;
        }
        if (hp <= 0 && !dead)
        {
            hp = 0;
            if (type == MonsterType.SubBoss || type == MonsterType.Boss)
            {
                hpSlider.value = hp;
                UIManager.Instance.uis[2].GetComponent<DungeonUi>().bossHp.text = hp + " / " + state.maxHp;
            }
            Die();
        }
    }

    public void Die()
    {
        if(onDeath != null)
        {
            onDeath();
        }
        switch(type)
        {
            case MonsterType.Mob:
                foreach (var drop in normalDrop)
                {
                    drop.itemDrop(gameObject.transform.position);
                }
                break;
            case MonsterType.SubBoss:
                break;
            case MonsterType.Boss:
                break;
        }
        dead = true;
        
    }
}
