using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
            StateManager.Instance.PlayerStateSet();
            info.RestoreHealth(0);
        }

        Destroy(gameObject);
    }
}
