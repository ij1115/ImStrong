using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class LobbyUi : SceneUI
{
    [SerializeField] private Button lobbyOption;
    [SerializeField] private GameObject lobbyOptionMenu;
    [SerializeField] private Text title;
    [SerializeField] private Button lobbyOpClose;

    [SerializeField] private GameObject weaponSelect;
    [SerializeField] private Button swordSelectB;
    [SerializeField] private TextMeshProUGUI swordSelectBLev;
    [SerializeField] private Button axeSelectB;
    [SerializeField] private TextMeshProUGUI axeSelectBLev;
    [SerializeField] private Button spearSelectB;
    [SerializeField] private TextMeshProUGUI spearSelectBLev;
    [SerializeField] private GameObject weaponSelectInfo;
    [SerializeField] private Button weaponSelectInfoClose;
    [SerializeField] private TextMeshProUGUI wepName;
    [SerializeField] private TextMeshProUGUI wepCount;
    [SerializeField] private TextMeshProUGUI atk;
    [SerializeField] private TextMeshProUGUI maxHp;
    [SerializeField] private TextMeshProUGUI atkSp;
    [SerializeField] private TextMeshProUGUI movSp;
    [SerializeField] private TextMeshProUGUI stageText;


    [SerializeField] private Button startButton;

    private Weapons weaponType;

    public override void Open()
    {
        lobbyOption.gameObject.SetActive(true);
        weaponSelect.SetActive(true);
        weaponType = StateManager.Instance.GetCurrentWeapons();
        startButton.gameObject.SetActive(true);
        stageText.text = $"STAGE {GameData.Instance.data.stageLev}";
        swordSelectBLev.text = $"+{GameData.Instance.data.swordLev + 1}";
        axeSelectBLev.text = $"+{GameData.Instance.data.axeLev + 1}";
        spearSelectBLev.text = $"+{GameData.Instance.data.spearLev + 1}";

        switch(weaponType)
        {
            case Weapons.Sword:
                Color swColor = Color.red;
                swColor.a = 0.7f;
                axeSelectB.gameObject.GetComponent<Image>().color = Color.black;
                swordSelectB.gameObject.GetComponent<Image>().color = swColor;
                spearSelectB.gameObject.GetComponent<Image>().color = Color.black;
                break;
            case Weapons.Axe:
                Color axColor = Color.red;
                axColor.a = 0.7f;
                axeSelectB.gameObject.GetComponent<Image>().color = axColor;
                swordSelectB.gameObject.GetComponent<Image>().color = Color.black;
                spearSelectB.gameObject.GetComponent<Image>().color = Color.black;
                break;
            case Weapons.Spear:
                Color spColor = Color.red;
                spColor.a = 0.7f;
                axeSelectB.gameObject.GetComponent<Image>().color = Color.black;
                swordSelectB.gameObject.GetComponent<Image>().color = Color.black;
                spearSelectB.gameObject.GetComponent<Image>().color = spColor;
                break;
        }
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
        title.gameObject.SetActive(true);
        lobbyOpClose.gameObject.SetActive(true);
    }
    public void OnClickLobbyOptionClose()
    {
        lobbyOption.interactable = true;
        swordSelectB.interactable = true;
        axeSelectB.interactable = true;
        spearSelectB.interactable = true;
        startButton.interactable = true;

        lobbyOptionMenu.SetActive(false);
        title.gameObject.SetActive(false);
        lobbyOpClose.gameObject.SetActive(false);
    }

    public void OnClickAxe()
    {
        Color axColor = Color.red;
        axColor.a = 0.7f;
        axeSelectB.gameObject.GetComponent<Image>().color = axColor;
        swordSelectB.gameObject.GetComponent<Image>().color = Color.black;
        spearSelectB.gameObject.GetComponent<Image>().color = Color.black;
        StateManager.Instance.SetCurrentWeapons(Weapons.Axe);
        weaponType = StateManager.Instance.GetCurrentWeapons();
        LobbyManager.instance.WeaponsChange();
        WeaponSelectInfoOpen();
    }
    public void OnClickSpear()
    {
        Color spColor = Color.red;
        spColor.a = 0.7f;
        axeSelectB.gameObject.GetComponent<Image>().color = Color.black;
        swordSelectB.gameObject.GetComponent<Image>().color = Color.black;
        spearSelectB.gameObject.GetComponent<Image>().color = spColor;
        StateManager.Instance.SetCurrentWeapons(Weapons.Spear);
        weaponType = StateManager.Instance.GetCurrentWeapons();
        LobbyManager.instance.WeaponsChange();
        WeaponSelectInfoOpen();
    }

    public void OnClickSword()
    {
        Color swColor = Color.red;
        swColor.a = 0.7f;
        axeSelectB.gameObject.GetComponent<Image>().color = Color.black;
        swordSelectB.gameObject.GetComponent<Image>().color = swColor;
        spearSelectB.gameObject.GetComponent<Image>().color = Color.black;
        StateManager.Instance.SetCurrentWeapons(Weapons.Sword);
        weaponType = StateManager.Instance.GetCurrentWeapons();
        LobbyManager.instance.WeaponsChange();
        WeaponSelectInfoOpen();
    }

    public void OnClickInfoClose()
    {
        weaponSelectInfo.SetActive(false);
    }

    public void OnClickStageButton()
    {
        GameData.Instance.DungeonInData();
        UIManager.Instance.StartFadeIn("Dungeon");
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

        atk.text = $"{StateManager.Instance.current.atk}";
        maxHp.text = $"{StateManager.Instance.current.maxHp}";
        atkSp.text = $"{StateManager.Instance.current.atkSp}";
        movSp.text = $"{StateManager.Instance.current.movSp}";
    }
}
