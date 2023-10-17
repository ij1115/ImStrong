using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAtkUpItem : MonoBehaviour, IItem
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();
        if (info != null)
        {
            GameData.Instance.SwordLevUp();
        }

        Destroy(gameObject);
    }
}