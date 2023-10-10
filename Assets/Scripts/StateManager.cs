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
    public float atkSp = 1.2f;
    public float movSp = 5f;

    public float mobHpSet = 1f;
    public float mobAtkSet = 0.5f;
    public float mobDefSet = 0.1f;
    public float mobAtkSpSet = 1f;
    public float mobMovSpSet = 0.5f;

    public float subBossHpSet = 2f;
    public float subBossAtkSet = 1.5f;
    public float subBossDefSet = 0.2f;
    public float subBossAtkSpSet = 1.25f;
    public float subBossMovSpSet = 0.7f;

    public float bossHpSet = 4f;
    public float bossAtkSet = 2f;
    public float bossDefSet = 0.4f;
    public float bossAtkSpSet = 1.5f;
    public float bossMovSpSet = 0.8f;

   
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

    private void Awake()
    {
        if(instance != null && instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        standard = new State();
        current = new State();
        monster = new State();
        currentWeapons = Weapons.Spear;
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
        current = standard;
    }
    
    public void PlayerStateSet()
    {
        // 인벤토리 추가시 수치 받아와서 더할 예정
        current.maxHp = standard.maxHp;
        current.atk = standard.atk;
        current.def = standard.def;
        current.atkSp = standard.atkSp;
        current.movSp = standard.movSp;
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

    // 디버깅용 코드
    public void PlayerStateLog()
    {
        Debug.Log($"PlayerInfo\n" +
            $"MaxHP : {current.maxHp}\n" +
            $"ATK : {current.atk}\n" +
            $"DEF : {current.def}\n" +
            $"AttackSpeed : {current.atkSp}\n" +
            $"MoveSpeed : {current.movSp}");
    }
    public void MobStateLog()
    {
        Debug.Log($"MonsterInfo\n" +
            $"MaxHP : {Mathf.RoundToInt((float)monster.maxHp * mobHpSet)}\n" +
            $"ATK : {Mathf.RoundToInt((float)monster.atk * mobAtkSet)}\n" +
            $"DEF : {Mathf.RoundToInt((float)monster.def * mobDefSet)}\n" +
            $"AttackSpeed : {monster.atkSp * mobAtkSpSet}\n" +
            $"MoveSpeed : {monster.movSp * mobMovSpSet}");
    }
    public void SubBossStateLog()
    {
        Debug.Log($"SubBossInfo\n" +
            $"MaxHP : {Mathf.RoundToInt((float)monster.maxHp * subBossHpSet)}\n" +
            $"ATK : {Mathf.RoundToInt((float)monster.atk * subBossAtkSet)}\n" +
            $"DEF : {Mathf.RoundToInt((float)monster.def * subBossDefSet)}\n" +
            $"AttackSpeed : {monster.atkSp * subBossAtkSpSet}\n" +
            $"MoveSpeed : {monster.movSp * subBossMovSpSet}");
    }
    public void BossStateLog()
    {
        Debug.Log($"BossInfo\n" +
            $"MaxHP : {Mathf.RoundToInt((float)monster.maxHp * bossHpSet)}\n" +
            $"ATK : {Mathf.RoundToInt((float)monster.atk * bossAtkSet)}\n" +
            $"DEF : {Mathf.RoundToInt((float)monster.def * bossDefSet)}\n" +
            $"AttackSpeed : {monster.atkSp * bossAtkSpSet}\n" +
            $"MoveSpeed : {monster.movSp * bossMovSpSet}");
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