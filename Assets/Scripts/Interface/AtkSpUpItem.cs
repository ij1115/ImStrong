using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkSpUpItem : MonoBehaviour, IItem
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();

        if (info != null)
        {
            info.EffectCor(info.atkSpUp);
            GameData.Instance.AttackSpeedUp();
            StateManager.Instance.PlayerStateSet();
        }

        Destroy(gameObject);
    }
}