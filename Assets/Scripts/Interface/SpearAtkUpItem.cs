using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAtkUpItem : MonoBehaviour
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();
        if (info != null)
        {
            GameData.Instance.SpearLevUp();
        }

        Destroy(gameObject);
    }
}