using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons
{
    None = -1,
    Sword,
    Axe,
    Spear,
}

public class PlayerWeapons : MonoBehaviour
{
    public Weapons type = Weapons.None;

}
