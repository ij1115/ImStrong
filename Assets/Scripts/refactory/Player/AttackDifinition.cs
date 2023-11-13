using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack.asset", menuName ="Attack/BaseAttack")]
public class AttackDifinition : ScriptableObject
{
    public float damage;
    public float range;

    public Attack CreateAttack(State attacker, State defender)
    {
        float damage = attacker.atk;
        
        if(defender != null)
        {
            damage -=  Mathf.RoundToInt((float)defender.def * 0.1f);
        }

        return new Attack((int)damage);
    }
    public virtual void ExecuteAttack(GameObject attacker, GameObject defender)
    {

    }
}
