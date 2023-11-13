using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerWeapons : MonoBehaviour
{
    public Weapons type;

    public PlayerWeapon[] weapons = new PlayerWeapon[3];
    public PlayerWeapon CurrentWeapon { get; private set; }
    public Transform lHandDummy;
    public Transform rHandDummy;
    private GameObject rHandWeapon;
    private GameObject lHandWeapon;

    public RuntimeAnimatorController swordAni;
    public RuntimeAnimatorController axeAni;
    public RuntimeAnimatorController spearAni;

    public List<GameObject> weaponList = new List<GameObject>();


    public void RightHandEquipWeapon(PlayerWeapon newWeapon, Transform parent)
    {
        if (newWeapon == null)
            return;

        RightHandUnEquipWeapon();

        rHandWeapon = Instantiate(newWeapon.prefab, parent);
        CurrentWeapon = newWeapon;
    }

    public void LeftHandEquipWeapon(PlayerWeapon newWeapon, Transform parent)
    {
        if(newWeapon == null) 
            return;

        LeftHandUnEquipWeapon();

        lHandWeapon = Instantiate(newWeapon.prefab,parent);
        CurrentWeapon = newWeapon;
    }

    public void RightHandUnEquipWeapon()
    {
        if(CurrentWeapon != null)
        {      
            Destroy(rHandWeapon);
            CurrentWeapon = null;
        }
    }

    public void LeftHandUnEquipWeapon()
    {
        if(CurrentWeapon != null)
        {
            Destroy(lHandWeapon);
            CurrentWeapon = null;
        }
    }


    private void Awake()
    {
        type = StateManager.Instance.GetCurrentWeapons();
        foreach(Transform obj in gameObject.GetComponentsInChildren<Transform>())
        {
            if (obj == null)
                return;

            if (obj.CompareTag("weapons"))
            {

                obj.GetComponent<MeshCollider>().enabled = false;
                weaponList.Add(obj.gameObject);
                obj.gameObject.SetActive(false);
            }
        }
        switch (type)
        {
            case Weapons.Sword:
                {
                    foreach (var weapon in weaponList)
                    {
                        if (weapon.gameObject.name == "WepSword")
                        {
                            weapon.SetActive(true);
                        }
                    }
                }
                break;
            case Weapons.Spear:
                {
                    foreach (var weapon in weaponList)
                    {
                        if (weapon.gameObject.name == "WepSpear")
                        {
                            weapon.SetActive(true);
                        }
                    }
                }
                break;
            case Weapons.Axe:
                {
                    foreach (var weapon in weaponList)
                    {
                        if (weapon.gameObject.name == "LWepAxe" || weapon.gameObject.name == "RWepAxe")
                        {
                            weapon.SetActive(true);
                        }
                    }
                }
                break;
        }
    }

    public void RunTimeSwap()
    {
        foreach(var weapon in weaponList)
        {
            weapon.SetActive(false);
        }

        type = StateManager.Instance.GetCurrentWeapons();

        switch(type)
        {
            case Weapons.None:
                foreach (var weapon in weaponList)
                {
                    weapon.SetActive(false);
                }
                break;

            case Weapons.Sword:
                {
                    foreach (var weapon in weaponList)
                    {
                        if(weapon.gameObject.name == "WepSword")
                        {
                            weapon.SetActive(true);
                        }
                    }
                }
                break;
            case Weapons.Spear:
                {
                    foreach (var weapon in weaponList)
                    {
                        if (weapon.gameObject.name == "WepSpear")
                        {
                            weapon.SetActive(true);
                        }
                    }
                }
                break;
            case Weapons.Axe:
                {
                    foreach (var weapon in weaponList)
                    {
                        if (weapon.gameObject.name == "LWepAxe" || weapon.gameObject.name == "RWepAxe")
                        {
                            weapon.SetActive(true);
                        }
                    }
                }
                break;
        }
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
