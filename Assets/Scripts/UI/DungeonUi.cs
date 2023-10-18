using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUi : SceneUI
{
    [SerializeField] private GameObject option;
    [SerializeField] private Text title;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private GameObject charInfoBox;

    public GameObject gameOverUi;

    [SerializeField] private GameObject joystickButton;
    [SerializeField] private FixedJoystick joystick;

    [SerializeField] private GameObject skillSet;
    [SerializeField] private GameObject[] swordButtonSet;
    [SerializeField] private GameObject[] axeButtonSet;
    [SerializeField] private GameObject[] spearButtonSet;

    public PlayerButtonController attack;
    public PlayerButtonController fSkill;
    public PlayerButtonController sSkill;
    public PlayerButtonController evade;

    public GameObject fSkillSlider;
    public GameObject sSkillSlider;

    [SerializeField] private Button charInfoButton;
    [SerializeField] private GameObject swordInfo;
    [SerializeField] private GameObject axeInfo;
    [SerializeField] private GameObject spearInfo;
    [SerializeField] private TextMeshProUGUI mSwLev;
    [SerializeField] private TextMeshProUGUI mSwVal;
    [SerializeField] private TextMeshProUGUI mSwSALev;
    [SerializeField] private TextMeshProUGUI mSwSpLev;

    [SerializeField] private TextMeshProUGUI mALev;
    [SerializeField] private TextMeshProUGUI mAVal;
    [SerializeField] private TextMeshProUGUI mASSwLev;
    [SerializeField] private TextMeshProUGUI mASSpLev;

    [SerializeField] private TextMeshProUGUI mSpLev;
    [SerializeField] private TextMeshProUGUI mSpVal;
    [SerializeField] private TextMeshProUGUI mSpSALev;
    [SerializeField] private TextMeshProUGUI mSpSSwLev;

    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gameOverButton;

    [SerializeField] private GameObject charHpBar;
    [SerializeField] private GameObject bossHpBar;

    public TextMeshProUGUI playerHp;
    public TextMeshProUGUI bossHp;
    public TextMeshProUGUI bossType;

    private Weapons weaponType;

    private bool infoOnOff = false;

    public override void Open()
    {
        pauseButton.gameObject.SetActive(true);
        joystickButton.SetActive(true);
        charInfoButton.gameObject.SetActive(true);
        skillSet.SetActive(true);

        weaponType = StateManager.Instance.GetCurrentWeapons();

        switch (weaponType)
        {
            case Weapons.Sword:
                foreach (var sword in swordButtonSet)
                {
                    sword.SetActive(true);
                }

                foreach (var axe in axeButtonSet)
                {
                    axe.SetActive(false);
                }

                foreach (var spear in spearButtonSet)
                {
                    spear.SetActive(false);
                }
                break;

            case Weapons.Axe:
                foreach (var sword in swordButtonSet)
                {
                    sword.SetActive(false);
                }

                foreach (var axe in axeButtonSet)
                {
                    axe.SetActive(true);
                }

                foreach (var spear in spearButtonSet)
                {
                    spear.SetActive(false);
                }
                break;

            case Weapons.Spear:
                foreach (var sword in swordButtonSet)
                {
                    sword.SetActive(false);
                }

                foreach (var axe in axeButtonSet)
                {
                    axe.SetActive(false);
                }

                foreach (var spear in spearButtonSet)
                {
                    spear.SetActive(true);
                }
                break;
        }
        base.Open();
    }

    public override void Close()
    {
        Time.timeScale = 1f;
        pauseButton.gameObject.SetActive(false);
        joystickButton.SetActive(false);
        charInfoButton.gameObject.SetActive(false);
        skillSet.SetActive(false);

        foreach (var sword in swordButtonSet)
        {
            sword.SetActive(false);
        }
        foreach (var axe in axeButtonSet)
        {
            axe.SetActive(false);
        }
        foreach (var spear in spearButtonSet)
        {
            spear.SetActive(false);
        }

        pauseButton.interactable = true;
        attack.interactable = true;
        fSkill.interactable = true;
        sSkill.interactable = true;
        evade.interactable = true;
        charInfoButton.interactable = true;

        infoOnOff = false;
        swordInfo.SetActive(false);
        axeInfo.SetActive(false);
        spearInfo.SetActive(false);
        charInfoBox.SetActive(false);

        gameOverUi.SetActive(false);

        option.SetActive(false);
        charHpBar.SetActive(false);
        bossHpBar.SetActive(false);
        base.Close();
    }

    public void OnClickPause()
    {
        pauseButton.interactable = false;
        attack.interactable = false;
        fSkill.interactable = false;
        sSkill.interactable = false;
        evade.interactable = false;
        charInfoButton.interactable = false;

        Time.timeScale = 0f;

        option.SetActive(true);
        title.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(true);
        homeButton.gameObject.SetActive(true);
    }
    public void OnClickPlay()
    {
        pauseButton.interactable = true;
        attack.interactable = true;
        fSkill.interactable = true;
        sSkill.interactable = true;
        evade.interactable = true;
        charInfoButton.interactable = true;
        option.SetActive(false);
        title.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        homeButton.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickHome()
    {
        pauseButton.interactable = true;
        attack.interactable = true;
        fSkill.interactable = true;
        sSkill.interactable = true;
        evade.interactable = true;
        charInfoButton.interactable = true;
        option.SetActive(false);
        title.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        homeButton.gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameManager.instance.ChangeScene("Lobby");
    }

    public void OnClickInfo()
    {
        switch (infoOnOff)
        {
            case true:
                infoOnOff = false;
                swordInfo.SetActive(false);
                axeInfo.SetActive(false);
                spearInfo.SetActive(false);
                charInfoBox.SetActive(false);
                break;

            case false:
                infoOnOff = true;
                charInfoBox.SetActive(true);
                InfoOpen();
                break;
        }
    }

    public void OnClickGameOver()
    {
        GameManager.instance.isGameover = false;
        GameManager.instance.ChangeScene("Lobby");
    }

    private void InfoOpen()
    {
        switch (weaponType)
        {
            case Weapons.Sword:
                InfoWeaponsTextUpdate();
                swordInfo.SetActive(true);
                axeInfo.SetActive(false);
                spearInfo.SetActive(false);
                break;

            case Weapons.Axe:
                InfoWeaponsTextUpdate();
                swordInfo.SetActive(false);
                axeInfo.SetActive(true);
                spearInfo.SetActive(false);
                break;

            case Weapons.Spear:
                InfoWeaponsTextUpdate();
                swordInfo.SetActive(false);
                axeInfo.SetActive(false);
                spearInfo.SetActive(true);
                break;
        }

    }

    public void InfoWeaponsTextUpdate()
    {
        mSwLev.text = $"+{GameData.Instance.data.swordLev + 1}";
        mSwVal.text = $"{StateManager.Instance.current.atk}";
        mSwSALev.text = $"+{GameData.Instance.data.axeLev + 1}";
        mSwSpLev.text = $"+{GameData.Instance.data.spearLev + 1}";

        mALev.text = $"+{GameData.Instance.data.axeLev + 1}";
        mAVal.text = $"{StateManager.Instance.current.atk}";
        mASSwLev.text = $"+{GameData.Instance.data.swordLev + 1}";
        mASSpLev.text = $"+{GameData.Instance.data.spearLev + 1}";

        mSpLev.text = $"+{GameData.Instance.data.spearLev + 1}";
        mSpVal.text = $"{StateManager.Instance.current.atk}";
        mSpSSwLev.text = $"+{GameData.Instance.data.swordLev + 1}";
        mSpSALev.text = $"+{GameData.Instance.data.axeLev + 1}";
    }
    public Slider PlayerHpBarSet()
    {
        charHpBar.SetActive(true);
        return charHpBar.GetComponentInChildren<Slider>();
    }

    public Slider MonsterHpBarSet()
    {
        bossHpBar.SetActive(true);
        return bossHpBar.GetComponentInChildren<Slider>();
    }

    public void BossDieUi()
    {
        bossHpBar.SetActive(false);
    }

    public FixedJoystick SetJoystick()
    {
        return joystick;
    }
}
