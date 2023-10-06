using UnityEngine;


public class StateManager : MonoBehaviour
{
    private static StateManager instance;

    public int maxHp = 100;
    public int atk = 20;
    public int def = 0;
    public float atkSp = 1f;
    public float movSp = 1f;

    public State standard;
    public State current;
    public State monster;

    public float mobHpSet = 1f;
    public float mobAtkSet = 0.5f;
    public float mobDefSet = 0.1f;
    public float mobAtkSpSet = 1f;
    public float mobMovSpSet = 0.5f;

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

    }

    public void MobInfoSave()
    {
        monster = current;
    }

    public State MobState()
    {
        var state = new State();
        state.maxHp = Mathf.RoundToInt((float)current.maxHp * mobHpSet);
        state.atk = Mathf.RoundToInt((float)current.atk * mobAtkSet);
        state.def = Mathf.RoundToInt((float)current.def * mobDefSet);
        state.atkSp = current.atkSp * mobAtkSpSet;
        state.movSp = current.movSp * mobMovSpSet;

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