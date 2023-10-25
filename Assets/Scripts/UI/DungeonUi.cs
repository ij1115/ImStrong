using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUi : SceneUI
{
    //메뉴
    [SerializeField] private GameObject option;
    [SerializeField] private Text title;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button pauseButton;

    //조이스틱
    [SerializeField] private GameObject joystickButton;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private FloatingJoystick fJoystick;

    //스킬 버튼
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

    //캐릭터 인포
    [SerializeField] private GameObject charInfoBox;
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


    //클리어시
    public bool OpenPortal = false;

    [SerializeField] private GameObject nextCheckWindow;
    [SerializeField] private Button SelectWindow;
    [SerializeField] private Button CloseWindow;

    [SerializeField] private GameObject nextSelectWindow;
    [SerializeField] private Button nextSwordButton;
    [SerializeField] private Button nextAxeButton;
    [SerializeField] private Button nextSpearButton;
    [SerializeField] private TextMeshProUGUI nextSwordLev;
    [SerializeField] private TextMeshProUGUI nextAxeLev;
    [SerializeField] private TextMeshProUGUI nextSpearLev;

    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject infoAtk;
    [SerializeField] private GameObject infoMaxHp;
    [SerializeField] private GameObject infoAtkSp; 
    [SerializeField] private GameObject infoMovSp;
    [SerializeField] private GameObject infoLine;

    [SerializeField] private TextMeshProUGUI atk;
    [SerializeField] private TextMeshProUGUI maxHp;
    [SerializeField] private TextMeshProUGUI atkSp;
    [SerializeField] private TextMeshProUGUI movSp;

    [SerializeField] private Button lobbyButton;
    [SerializeField] private Button nextStageButton;


    //패배시
    [SerializeField] private GameObject dieBackGround;
    [SerializeField] private GameObject restartCheckWindow;
    [SerializeField] private Button yes;
    [SerializeField] private Button no;

    [SerializeField] private Button resetButton;
    [SerializeField] private Button retryButton;


    //체력바
    [SerializeField] private GameObject charHpBar;
    [SerializeField] private GameObject bossHpBar;

    public TextMeshProUGUI playerHp;
    public TextMeshProUGUI bossHp;
    public TextMeshProUGUI bossType;

    private Weapons weaponType;

    private bool infoOnOff = false;

    public GameObject gameOverUi;

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
        //gameObject.GetComponentInParent<UIManager>().GetComponent<Canvas>().worldCamera = Camera.main;
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

        OpenPortal = false;

        nextCheckWindow.SetActive(false);
        nextSelectWindow.SetActive(false);
        gameOverUi.SetActive(false);
        restartCheckWindow.SetActive(false);
        dieBackGround.SetActive(true);
        resetButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);

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

    public void PlayerDie()
    {
        if (infoOnOff)
            OnClickInfo();

        joystickButton.SetActive(false);
        charHpBar.SetActive(false);
        skillSet.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        charInfoButton.gameObject.SetActive(false);

        Time.timeScale = 0f;

        gameOverUi.SetActive(true);
    }
    public void OnClickOpenReStart()
    {
        restartCheckWindow.SetActive(true);
        dieBackGround.SetActive(false);
        resetButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
    }
    public void OnClickReStartNo()
    {
        restartCheckWindow.SetActive(false);
        dieBackGround.SetActive(true);
        resetButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
    }
    public void OnClickReStartYes()
    {
        GameManager.instance.isGameover = false;
        Time.timeScale = 1f;
        GameData.Instance.DataReset();
        UIManager.Instance.StartFadeIn("Lobby");
    }
    public void OnClickReTry()
    {
        GameManager.instance.isGameover = false;
        Time.timeScale = 1f;
        GameData.Instance.DungeonRetry();
        UIManager.Instance.StartFadeIn("Lobby");
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

    public void NextCheckWindow()
    {
        pauseButton.interactable = false;
        attack.interactable = false;
        fSkill.interactable = false;
        sSkill.interactable = false;
        evade.interactable = false;
        charInfoButton.interactable = false;
        Time.timeScale = 0f;
        nextCheckWindow.SetActive(true);
    }

    public void OnClickCloseCheck()
    {
        pauseButton.interactable = true;
        attack.interactable = true;
        fSkill.interactable = true;
        sSkill.interactable = true;
        evade.interactable = true;
        charInfoButton.interactable = true;
        nextCheckWindow.SetActive(false);
        Time.timeScale = 1f;
        OpenPortal = false;
    }
    public void OnClickWepSelectWindow()
    {
        pauseButton.interactable = true;
        attack.interactable = true;
        fSkill.interactable = true;
        sSkill.interactable = true;
        evade.interactable = true;
        charInfoButton.interactable = true;

        nextCheckWindow.SetActive(false);
        NextStageWepSelect();
    }
    public void NextStageWepSelect()
    {
        if (infoOnOff)
            OnClickInfo();

        joystickButton.SetActive(false);
        charHpBar.SetActive(false);
        skillSet.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        charInfoButton.gameObject.SetActive(false);

        backGround.SetActive(true);
        nextSwordLev.text = $"+{GameData.Instance.data.swordLev + 1}";
        nextAxeLev.text = $"+{GameData.Instance.data.axeLev + 1}";
        nextSpearLev.text = $"+{GameData.Instance.data.spearLev + 1}";

        nextSelectWindow.SetActive(true);
    }

    public void OnClickNextWepSword()
    {
        StateManager.Instance.SetCurrentWeapons(Weapons.Sword);
        weaponType = StateManager.Instance.GetCurrentWeapons();
        InfomationUpdate();
    }

    public void OnClickNextWepAxe()
    {
        StateManager.Instance.SetCurrentWeapons(Weapons.Axe);
        weaponType = StateManager.Instance.GetCurrentWeapons();
        InfomationUpdate();
    }

    public void OnClickNextWepSpear()
    {
        StateManager.Instance.SetCurrentWeapons(Weapons.Spear);
        weaponType = StateManager.Instance.GetCurrentWeapons();
        InfomationUpdate();
    }

    public void InfomationUpdate()
    {
        backGround.SetActive(false);
        infoAtk.SetActive(true);
        infoAtkSp.SetActive(true);
        infoLine.SetActive(true);
        infoMaxHp.SetActive(true);
        infoMovSp.SetActive(true);
        StateManager.Instance.PlayerStateSet();

        atk.text = $"{StateManager.Instance.current.atk}";
        atkSp.text = $"{StateManager.Instance.current.atkSp}";
        maxHp.text = $"{StateManager.Instance.current.maxHp}";
        movSp.text = $"{StateManager.Instance.current.movSp}";
    }

    public void OnClickLobby()
    {
        infoAtk.SetActive(false);
        infoAtkSp.SetActive(false);
        infoLine.SetActive(false);
        infoMaxHp.SetActive(false);
        infoMovSp.SetActive(false);
        nextSelectWindow.SetActive(false);

        OpenPortal = false;
        DungeonManager.instance.Reset();

        Time.timeScale = 1f;

        GameData.Instance.StageUp();
        GameData.Instance.DataSave();
        UIManager.Instance.StartFadeIn("Lobby");
    }

    public void OnClickNextStage()
    {
        infoAtk.SetActive(false);
        infoAtkSp.SetActive(false);
        infoLine.SetActive(false);
        infoMaxHp.SetActive(false);
        infoMovSp.SetActive(false);
        nextSelectWindow.SetActive(false);

        OpenPortal = false;

        DungeonManager.instance.Reset();

        Time.timeScale = 1f;

        GameData.Instance.StageUp();
        GameData.Instance.DataSave();
        GameData.Instance.DungeonInData();
        UIManager.Instance.StartFadeIn("Dungeon");
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

    public FloatingJoystick SetFJoyStick()
    {
        return fJoystick;
    }
}
