using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAtkUpItem : MonoBehaviour
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();
        if (info != null)
        {
            GameData.Instance.AxeLevUp();
        }

        Destroy(gameObject);
    }
}