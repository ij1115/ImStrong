using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHpUpItem : MonoBehaviour, IItem
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();
        if (info != null)
        {
            info.EffectCor(info.maxHpUp);
            GameData.Instance.MaxHpUp();
        }

        Destroy(gameObject);
    }
}
