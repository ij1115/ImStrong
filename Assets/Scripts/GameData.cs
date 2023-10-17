using UnityEngine;
using SaveDataVC = SaveDataV2;

public class GameData : MonoBehaviour
{
    private static GameData instance;
    public static GameData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameData>();
            }
            return instance;
        }
    }

    public SaveDataVC data = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void DataLoad()
    {
        data = SaveLoadSystem.Load();
    }

    public void DataSave()
    {
        SaveLoadSystem.Save(data);
    }

    public void StageUp()
    {
        data.stageLev += 1;
    }

    public void SwordLevUp()
    {
       
        data.swordLev += 1;
    }

    public void AxeLevUp()
    {
        data.axeLev += 1;
    }

    public void AttackSpeedUp()
    {
        if (data.atkSpUpLev < 16)
        {
            data.atkSpUpLev += 1;
        }
    }

    public void MoveSpeedUp()
    {
        if(data.movSpUpLev <14)
        {
            data.movSpUpLev += 1;
        }
    }

    public void MaxHpUp()
    {
        data.maxHpUp += 1;
    }

    public void DataReset()
    {
        data.stageLev = 1;
        data.swordLev = 0;
        data.axeLev = 0;
        data.spearLev = 0;
        data.atkSpUpLev = 0;
        data.movSpUpLev= 0;
        data.maxHpUp = 0;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            data.stageLev += 1;
            DataSave();
        }
    }
}
