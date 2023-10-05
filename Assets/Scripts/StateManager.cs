using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class StateManager : MonoBehaviour
{
    private static StateManager instance;
    public StateInfo standardInfo { get; private set; }
    public StateInfo currentInfo { get; private set; }
    public StateInfo monsterInfo { get; private set; }

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

        standardInfo = new StateInfo(100,20,0,1f,1f);
        currentInfo = new StateInfo(standardInfo);
        monsterInfo = new StateInfo(currentInfo);
    }


    public void MobInfoSave()
    {
        monsterInfo = currentInfo;
    }
}