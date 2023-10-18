using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : MonoBehaviour, IItem
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();
        if(info != null)
        {
            int healHp = Mathf.RoundToInt(info.hp * 0.4f);
            info.EffectCor(info.heal);
            info.RestoreHealth(healHp);
        }

        Destroy(gameObject);
    }
}
