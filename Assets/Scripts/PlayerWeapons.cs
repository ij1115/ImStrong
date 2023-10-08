using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerWeapons : MonoBehaviour
{
    public Weapons type;

    public RuntimeAnimatorController swordAni;
    public RuntimeAnimatorController axeAni;
    public RuntimeAnimatorController spearAni;

    public List<GameObject> weaponList = new List<GameObject>();

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
