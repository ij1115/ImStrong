using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackerable
{
    void OnAttack(GameObject attacker, Attack attack);
}
