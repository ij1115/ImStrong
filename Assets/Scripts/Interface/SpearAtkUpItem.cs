using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAtkUpItem : MonoBehaviour, IItem
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();
        if (info != null)
        {
            info.EffectCor(info.atkUp);
            GameData.Instance.SpearLevUp();
            UIManager.Instance.uis[2].GetComponent<DungeonUi>().InfoWeaponsTextUpdate();
        }

        Destroy(gameObject);
    }
}