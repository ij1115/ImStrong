using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAtkUpItem : MonoBehaviour, IItem
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();
        if (info != null)
        {
            info.EffectCor(info.atkUp);
            GameData.Instance.AxeLevUp();
            UIManager.Instance.uis[2].GetComponent<DungeonUi>().InfoWeaponsTextUpdate();
        }

        Destroy(gameObject);
    }
}