using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class LobbyUi : SceneUI
{
    [SerializeField] private Button lobbyOption;
    [SerializeField] private GameObject lobbyOptionMenu;
    [SerializeField] private Button lobbyOpClose;

    [SerializeField] private GameObject weaponSelect;
    [SerializeField] private Button swordSelectB;
    [SerializeField] private Button axeSelectB;
    [SerializeField] private Button spearSelectB;
    [SerializeField] private GameObject weaponSelectInfo;
    [SerializeField] private TextMeshProUGUI wepName;
    [SerializeField] private TextMeshProUGUI wepCount;
    [SerializeField] private TextMeshProUGUI atk;
    [SerializeField] private TextMeshProUGUI maxHp;
    [SerializeField] private TextMeshProUGUI atkSp;
    [SerializeField] private TextMeshProUGUI movSp;

    [SerializeField] private Button startButton;


    private Weapons weaponType;

    public override void Open()
    {
        lobbyOption.gameObject.SetActive(true);
        weaponSelect.SetActive(true);
        startButton.gameObject.SetActive(true);

        base.Open();
    }

    public override void Close()
    {
        lobbyOption.gameObject.SetActive(false);
        lobbyOptionMenu.SetActive(false);
        weaponSelect.SetActive(false);
        weaponSelectInfo.SetActive(false);
        startButton.gameObject.SetActive(false);

        base.Close();
    }
    
    public void OnClickLobbyOption()
    {
        lobbyOption.interactable = false;
        swordSelectB.interactable = false;
        axeSelectB.interactable = false;
        spearSelectB.interactable = false;
        startButton.interactable = false;

        lobbyOptionMenu.SetActive(true);
    }
    public void OnClickLobbyOptionClose()
    {
        lobbyOption.interactable = true;
        swordSelectB.interactable = true;
        axeSelectB.interactable = true;
        spearSelectB.interactable = true;
        startButton.interactable = true;

        lobbyOptionMenu.SetActive(false);
    }

    public void OnClickAxe()
    {
        StateManager.Instance.SetCurrentWeapons(Weapons.Axe);
        weaponType = StateManager.Instance.GetCurrentWeapons();
        LobbyManager.instance.WeaponsChange();
        WeaponSelectInfoOpen();
    }
    public void OnClickSpear()
    {
        StateManager.Instance.SetCurrentWeapons(Weapons.Spear);
       weaponType = StateManager.Instance.GetCurrentWeapons();
        LobbyManager.instance.WeaponsChange();
        WeaponSelectInfoOpen();
    }

    public void OnClickSword()
    {
        StateManager.Instance.SetCurrentWeapons(Weapons.Sword);
       weaponType = StateManager.Instance.GetCurrentWeapons();
        LobbyManager.instance.WeaponsChange();
        WeaponSelectInfoOpen();
    }

    public void OnClickStageButton()
    {
        GameManager.instance.ChangeScene("Dungeon");
    }

    public void WeaponSelectInfoOpen()
    {
        weaponSelectInfo.SetActive(true);
        StateManager.Instance.PlayerStateSet();

        switch (weaponType)
        { 
            case Weapons.Sword:
                wepName.text = "Sword";
                wepCount.text = $"+{GameData.Instance.data.swordLev + 1}";
                break;
            case Weapons.Axe:
                wepName.text = "Axe";
                wepCount.text = $"+{GameData.Instance.data.axeLev + 1}";

                break;
            case Weapons.Spear:
                wepName.text = "Spear";
                wepCount.text = $"+{GameData.Instance.data.spearLev + 1}";
                break;
        }

        atk.text = $"ATK : {StateManager.Instance.current.atk}";
        maxHp.text = $"Max HP : {StateManager.Instance.current.maxHp}";
        atkSp.text = $"ATK Speed : {StateManager.Instance.current.atkSp}";
        movSp.text = $"Move Speed : {StateManager.Instance.current.movSp}";
    }
}
