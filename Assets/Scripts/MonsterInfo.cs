using System;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfo : MonoBehaviour
{
    public State state;

    public Slider hpSlider;
    [SerializeField] private Transform canvasform;
    [SerializeField] private Transform monsform;
    public MonsterType type { get; private set; }

    public int hp;

    public event Action onDeath;

    public bool dead { get; private set; }

    public SOItem[] normalDrop;
    public SOItem[] subBossDrop;
    public SOItem[] bossDrop;

    private bool healDrop = false;
    private bool maxHpDrop = false;

    [SerializeField] private ParticleSystem[] particle;


    public void Awake()
    {
        state = new State();
        dead = false;
        healDrop = false;
        maxHpDrop = false;
    }

    public void Update()
    {
        if(type == MonsterType.Mob)
        {
            canvasform.LookAt(Camera.main.transform);
        }
    }
    public void StateUpdate()
    {
        switch(type)
        {
            case MonsterType.Mob:
                state = StateManager.Instance.MobState();
                for(int i =0; i< particle.Length; i++)
                {
                    var main = particle[i].main;
                    main.simulationSpeed = state.atkSp;
                }
                hpSlider.gameObject.SetActive(true);
                hpSlider.maxValue = state.maxHp;
                hpSlider.minValue = 0;
                hp = state.maxHp;
                hpSlider.value = hp;
                break;

            case MonsterType.SubBoss:
                state = StateManager.Instance.SubBossState();
                for (int i = 0; i < particle.Length; i++)
                {
                    var main = particle[i].main;
                    main.simulationSpeed = state.atkSp;
                }
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
                for (int i = 0; i < particle.Length; i++)
                {
                    var main = particle[i].main;
                    main.simulationSpeed = state.atkSp;
                }
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
            HealDrop(type);
            MaxHpDrop(type);
            hpSlider.value = hp;
            UIManager.Instance.uis[2].GetComponent<DungeonUi>().bossHp.text = hp + " / " + state.maxHp;
        }
        else if(type == MonsterType.Mob)
        {
            hpSlider.value = hp;
        }
        if (hp <= 0 && !dead)
        {
            hp = 0;
            if (type == MonsterType.SubBoss || type == MonsterType.Boss)
            {
                hpSlider.value = hp;
                UIManager.Instance.uis[2].GetComponent<DungeonUi>().bossHp.text = hp + " / " + state.maxHp;
            }
            else if (type == MonsterType.Mob)
            {
                hpSlider.value = hp;
                hpSlider.gameObject.SetActive(false);
            }
            Die();
        }
    }
    private void HealDrop(MonsterType t)
    {
        if((hp/state.maxHp) < 0.5f && !healDrop)
        {
            healDrop = true;
            switch (t)
            {
                case MonsterType.SubBoss:
                    for (int i = 0; i < 2; i++)
                    {
                        Vector3 dropPos = gameObject.transform.position;
                        dropPos.x += UnityEngine.Random.Range(-5f, 5f);
                        dropPos.z += UnityEngine.Random.Range(-5f, 5f);
                        subBossDrop[2].itemDrop(dropPos);
                    }
                    break;

                case MonsterType.Boss:
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector3 dropPos = gameObject.transform.position;
                            dropPos.x += UnityEngine.Random.Range(-5f, 5f);
                            dropPos.z += UnityEngine.Random.Range(-5f, 5f);
                            bossDrop[2].itemDrop(dropPos);
                        }
                    }
                    break;
            }

        }
      
    }

    private void MaxHpDrop(MonsterType t)
    {
        if ((hp / state.maxHp) < 0.5f && !maxHpDrop)
        {
            maxHpDrop = true;
            switch (t)
            {
                case MonsterType.SubBoss:
                    { 
                    Vector3 dropPos = gameObject.transform.position;
                    dropPos.x += UnityEngine.Random.Range(-5f, 5f);
                    dropPos.z += UnityEngine.Random.Range(-5f, 5f);

                    subBossDrop[1].itemDrop(dropPos);
                    }
                 break;

                case MonsterType.Boss:
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector3 dropPos = gameObject.transform.position;
                        dropPos.x += UnityEngine.Random.Range(-5f, 5f);
                        dropPos.z += UnityEngine.Random.Range(-5f, 5f);
                        bossDrop[1].itemDrop(dropPos);
                    }
                }
                 break;
            }
          
        }
    }
    public void Die()
    {
        if(onDeath != null)
        {
            onDeath();
        }
        switch (type)
        {
            case MonsterType.Mob:
                foreach (var drop in normalDrop)
                {
                    Vector3 dropPos = gameObject.transform.position;
                    dropPos.x += UnityEngine.Random.Range(-2f, 2f);
                    dropPos.z += UnityEngine.Random.Range(-2f, 2f);

                    drop.itemDrop(dropPos);
                }
                break;
            case MonsterType.SubBoss:
                for(int i=0; i<5; i++)
                {
                    Vector3 dropPos = gameObject.transform.position;
                    dropPos.x += UnityEngine.Random.Range(-2f, 2f);
                    dropPos.z += UnityEngine.Random.Range(-2f, 2f);

                    subBossDrop[0].itemDrop(dropPos);

                    if(i<2)
                    {
                        subBossDrop[1].itemDrop(dropPos);
                        subBossDrop[3].itemDrop(dropPos);
                        subBossDrop[4].itemDrop(dropPos);
                    }
                }
                break;
            case MonsterType.Boss:
                for (int i = 0; i < 10; i++)
                {
                    Vector3 dropPos = gameObject.transform.position;
                    dropPos.x += UnityEngine.Random.Range(-2f, 2f);
                    dropPos.z += UnityEngine.Random.Range(-2f, 2f);
                    subBossDrop[0].itemDrop(dropPos);

                    if (i < 3)
                    {
                        subBossDrop[1].itemDrop(dropPos);
                        subBossDrop[3].itemDrop(dropPos);
                        subBossDrop[4].itemDrop(dropPos);
                    }
                }
                break;
        }
        dead = true;
        
    }
}
