using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon.asset", menuName = "Attack/WeaponAttack")]
public class PlayerWeapon : AttackDifinition
{
    public GameObject prefab;
    public float dotRange;

    public override void ExecuteAttack(GameObject attacker, GameObject defender)
    {
        if (defender == null)
            return;

        if (Vector3.Distance(attacker.transform.position, defender.transform.position) > range)
            return;

        var dir = defender.transform.position - attacker.transform.position;
        dir.Normalize();

        var dot = Vector3.Dot(dir, attacker.transform.position);
        if (dot < dotRange)
            return;

        var attackerState = attacker.GetComponent<State>();
        var definderState = defender.GetComponent<State>();
        var definition = CreateAttack(attackerState, definderState);

        var attackables = defender.GetComponents<IAttackerable>();
        foreach (var attackable in attackables)
        {
            attackable.OnAttack(attacker, definition);
        }
    }
}
