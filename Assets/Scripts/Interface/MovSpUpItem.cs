using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovSpUpItem : MonoBehaviour, IItem
{
    public void Use(GameObject target)
    {
        PlayerInfo info = target.GetComponent<PlayerInfo>();
        if (info != null)
        {
            info.EffectCor(info.movSpUp);
            GameData.Instance.MoveSpeedUp();
            StateManager.Instance.PlayerStateSet();
        }

        Destroy(gameObject);
    }
}