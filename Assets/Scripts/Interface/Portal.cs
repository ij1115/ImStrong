using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IItem
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();
        if (info != null)
        {
            if(!UIManager.Instance.uis[2].GetComponent<DungeonUi>().OpenPortal)
            {
                UIManager.Instance.uis[2].GetComponent<DungeonUi>().OpenPortal = true;
                UIManager.Instance.uis[2].GetComponent<DungeonUi>().NextCheckWindow();
            }
        }
    }
}
