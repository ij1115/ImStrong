using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Attack
{
    public int Damage
    {
        get; private set;
    }
    
    public Attack(int damage)
    {
        Damage = damage;
    }
}
