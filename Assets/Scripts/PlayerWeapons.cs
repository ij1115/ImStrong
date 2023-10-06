using UnityEngine;


public class PlayerWeapons : MonoBehaviour
{
    public Weapons type;

    public RuntimeAnimatorController swordAni;
    public RuntimeAnimatorController axeAni;
    public RuntimeAnimatorController spearAni;

    private PlayerMovement movement;

    private void Awake()
    {
        type = StateManager.Instance.GetCurrentWeapons();
        movement = GetComponent<PlayerMovement>();
    }

    public RuntimeAnimatorController GetAni()
    {
        switch(type)
        {
            case Weapons.Sword:
                return swordAni;

            case Weapons.Axe:
                return axeAni;
                 
            case Weapons.Spear: 
                return spearAni;
        }

        return null;
    }
}
