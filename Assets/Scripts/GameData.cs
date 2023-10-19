using UnityEngine;
using SaveDataVC = SaveDataV3;

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
    public SaveDataVC tempData = new SaveDataVC();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void DungeonInData()
    {
        tempData.swordLev = data.swordLev;
        tempData.spearLev = data.spearLev;
        tempData.axeLev = data.axeLev;
        tempData.maxHpUp = data.maxHpUp;
        tempData.atkSpUpLev = data.atkSpUpLev;
        tempData.movSpUpLev = data.movSpUpLev;
    }

    public void DungeonRetry()
    {
        data.swordLev = tempData.swordLev;
        data.spearLev=tempData.spearLev;
        data.axeLev = tempData.axeLev;
        data.maxHpUp = tempData.maxHpUp;
        data.atkSpUpLev = tempData.atkSpUpLev;
        data.movSpUpLev = tempData.movSpUpLev;
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

    public void SpearLevUp()
    {
        data.spearLev += 1;
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
        SaveLoadSystem.Save(data);
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
