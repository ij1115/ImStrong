using UnityEngine;

public class StateManager : MonoBehaviour
{
    private static StateManager instance;

    public State standard;
    public State current;
    public State monster;

    public Weapons currentWeapons;

    public int maxHp = 100;
    public int atk = 20;
    public int def = 0;
    public float atkSp = 1.20f;
    public float movSp = 5.00f;

    public float mobHpSet = 1f;
    public float mobAtkSet = 0.4f;
    public float mobDefSet = 0.1f;
    public float mobAtkSpSet = 0.9f;
    public float mobMovSpSet = 0.5f;

    public float subBossHpSet = 2f;
    public float subBossAtkSet = 1.3f;
    public float subBossDefSet = 0.2f;
    public float subBossAtkSpSet = 0.7f;
    public float subBossMovSpSet = 0.7f;

    public float bossHpSet = 4f;
    public float bossAtkSet = 1.8f;
    public float bossDefSet = 0.4f;
    public float bossAtkSpSet = 0.6f;
    public float bossMovSpSet = 0.8f;

    private ItemTable item;
   
    public static StateManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("StateManager");
                instance = go.AddComponent<StateManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public void StateManagerLoad()
    {
        if(instance != null && instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        item = DataManager.GetTable<ItemTable>();
        standard = new State();
        current = new State();
        monster = new State();
        currentWeapons = Weapons.Sword;
    }
    public void StandardSetUp()
    {
        if (standard != null)
        {
            standard.maxHp = maxHp;
            standard.atk = atk;
            standard.def = def;
            standard.atkSp = atkSp;
            standard.movSp = movSp;
        }
    }

    public void CurrentToStandard()
    {
        current.maxHp = standard.maxHp;
        current.atk = standard.atk;
        current.def = standard.def;
        current.atkSp = standard.atkSp;
        current.movSp = standard.movSp;
    }
    
    public void PlayerStateSet()
    {
        // 인벤토리 추가시 수치 받아와서 더할 예정
        current.maxHp = standard.maxHp + (GameData.Instance.data.maxHpUp * (int)item.GetValue(1).Item2);

        current.def = standard.def;

        switch(currentWeapons)
        {
            case Weapons.Sword:
                current.atk = standard.atk + (GameData.Instance.data.swordLev * (int)item.GetValue(4).Item2);
                current.atkSp = standard.atkSp + 0.3f + (GameData.Instance.data.atkSpUpLev * item.GetValue(2).Item2);
                break;

            case Weapons.Axe:
                current.atk = standard.atk + (GameData.Instance.data.axeLev * (int)item.GetValue(5).Item2);
                current.atkSp = standard.atkSp + (GameData.Instance.data.atkSpUpLev * item.GetValue(2).Item2);
                break;

            case Weapons.Spear:
                current.atk = standard.atk + (GameData.Instance.data.spearLev * (int)item.GetValue(6).Item2);
                current.atkSp = standard.atkSp + (GameData.Instance.data.atkSpUpLev * item.GetValue(2).Item2);
                break;
        }
        current.movSp = standard.movSp + (GameData.Instance.data.movSpUpLev * item.GetValue(3).Item2);
    }

    public Weapons GetCurrentWeapons()
    {
        return currentWeapons;
    }
    public void SetCurrentWeapons(Weapons type)
    {
        currentWeapons = type;
    }

    public void MonsterSetUp()
    {
        monster = current;
    }

    public State MobState()
    {
        var state = new State();
        state.maxHp = Mathf.RoundToInt((float)monster.maxHp * mobHpSet);
        state.atk = Mathf.RoundToInt((float)monster.atk * mobAtkSet);
        state.def = Mathf.RoundToInt((float)monster.def * mobDefSet);
        state.atkSp = monster.atkSp * mobAtkSpSet;
        state.movSp = monster.movSp * mobMovSpSet;

        return state;
    }

    public State SubBossState()
    {
        var state = new State();
        state.maxHp = Mathf.RoundToInt((float)monster.maxHp * subBossHpSet);
        state.atk = Mathf.RoundToInt((float)monster.atk * subBossAtkSet);
        state.def = Mathf.RoundToInt((float)monster.def * subBossDefSet);
        state.atkSp = monster.atkSp * subBossAtkSpSet;
        state.movSp = monster.movSp * subBossMovSpSet;

        return state;
    }

    public State BossState()
    {
        var state = new State();
        state.maxHp = Mathf.RoundToInt((float)monster.maxHp * bossHpSet);
        state.atk = Mathf.RoundToInt((float)monster.atk * bossAtkSet);
        state.def = Mathf.RoundToInt((float)monster.def * bossDefSet);
        state.atkSp = monster.atkSp * bossAtkSpSet;
        state.movSp = monster.movSp * bossMovSpSet;

        return state;
    }
}

public class State
{
    public int maxHp;
    public int atk;
    public int def;
    public float atkSp;
    public float movSp;
}