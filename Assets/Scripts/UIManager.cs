using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }

    public int targetWidth = 1600; // 원하는 해상도 너비
    public int targetHeight = 900; // 원하는 해상도 높이
    public bool fullScreen = true; // 전체 화면 여부

    public void SetResolution(int width, int height, bool fullScreen)
    {
        Screen.SetResolution(width, height, fullScreen);
    }

    public SceneState currentScene;

    //TitleUi
    [SerializeField] private GameObject titleLogo;
    [SerializeField] private GameObject pressText;

    //LobbyUi
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


    //DungeonUi
    [SerializeField] private GameObject option;
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

    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gameOverButton;

    [SerializeField] private GameObject charHpBar;
    [SerializeField] private GameObject bossHpBar;

    public TextMeshProUGUI playerHp;
    public TextMeshProUGUI bossHp;
    public TextMeshProUGUI bossType;

    private Weapons weaponType;

    private bool infoOnOff = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SetResolution(targetWidth, targetHeight, fullScreen);
    }

    public void SceneSynchronization()
    {
        switch(currentScene)
        {
            case SceneState.Title:
                TitleUiClose();
                break;
            case SceneState.Lobby:
                LobbyUiClose();
                break;
            case SceneState.Dungeon:
                DungeonUiClose();
                break;
            case SceneState.BossRoom:
                DungeonUiClose();
                break;
        }

        currentScene = GameManager.instance.currentState;

        switch (currentScene)
        {
            case SceneState.Title:
                TitleUiSet();
                break;
            case SceneState.Lobby:
                LobbyUiSet();
                break;
            case SceneState.Dungeon:
                DungeonUISet();
                break;
            case SceneState.BossRoom:
                DungeonUISet();
                break;
        }
    }


    //Title Ui
    public void TitleUiSet()
    {
        titleLogo.SetActive(true);
        pressText.SetActive(true);
    }

    public void TitleUiClose()
    {
        titleLogo.SetActive(false);
        pressText.SetActive(false);
    }


    //Lobby Ui
    
    public void LobbyUiSet()
    {
        lobbyOption.gameObject.SetActive(true);
        weaponSelect.SetActive(true);
        startButton.gameObject.SetActive(true);
    }
    public void LobbyUiClose()
    {
        lobbyOption.gameObject.SetActive(false);
        lobbyOptionMenu.SetActive(false);
        weaponSelect.SetActive(false);
        weaponSelectInfo.SetActive(false);
        startButton.gameObject.SetActive(false);
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

        switch (weaponType)
        {
            case Weapons.Sword:
                wepName.text = "Sword";
                wepCount.text = "+1";
                break;
            case Weapons.Axe:
                wepName.text = "Axe";
                wepCount.text = "+1";

                break;
            case Weapons.Spear:
                wepName.text = "Spear";
                wepCount.text = "+1";
                break;
        }
        atk.text = $"ATK : {StateManager.Instance.current.atk}";
        maxHp.text = $"Max HP : {StateManager.Instance.current.maxHp}";
        atkSp.text = $"ATK Speed : {StateManager.Instance.current.atkSp}";
        movSp.text = $"Move Speed : {StateManager.Instance.current.movSp}";
    }
    // Dungeon Ui

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
        Time.timeScale = 1f;
        GameManager.instance.ChangeScene("Lobby");
    }

    public void OnClickInfo()
    {
        switch(infoOnOff)
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
                swordInfo.SetActive(true);
                axeInfo.SetActive(false);
                spearInfo.SetActive(false);
                break;

            case Weapons.Axe:
                swordInfo.SetActive(false);
                axeInfo.SetActive(true);
                spearInfo.SetActive(false);
                break;

            case Weapons.Spear:
                swordInfo.SetActive(false);
                axeInfo.SetActive(false);
                spearInfo.SetActive(true);
                break;
        }

    }

    public void DungeonUISet()
    {
        pauseButton.gameObject.SetActive(true);
        joystickButton.SetActive(true);
        charInfoButton.gameObject.SetActive(true);
        skillSet.SetActive(true);

        weaponType = StateManager.Instance.GetCurrentWeapons();

        switch(weaponType)
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
    }

    public void DungeonUiClose()
    {
        pauseButton.gameObject.SetActive(false);
        joystickButton.SetActive(false);
        charInfoButton.gameObject.SetActive(false);
        skillSet.SetActive(false);

        foreach(var sword in swordButtonSet)
        {
            sword.SetActive(false);
        }
        foreach(var axe in axeButtonSet)
        { 
            axe.SetActive(false);
        }
        foreach( var spear in spearButtonSet)
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
