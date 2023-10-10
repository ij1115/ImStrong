using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType<LobbyManager>();
            }
            return singleton;
        }
    }

    private static LobbyManager singleton;

    public Button start;
    public Button exit;
    public Button Axe;
    public Button Spear;
    public Button Sword;
    public Button Select;

    public GameObject selectWindow;

    private void Awake()
    {
        StateManager.Instance.StandardSetUp();
    }

    public void OnClickOpenSelectWindow()
    {
        selectWindow.SetActive(true);
    }

    public void OnClickCloesSeletWindow()
    {
        selectWindow.SetActive(false);
    }

    public void OnClickAxe()
    {
        StateManager.Instance.SetCurrentWeapons(Weapons.Axe);
    }
    public void OnClickSpear()
    {
        StateManager.Instance.SetCurrentWeapons(Weapons.Spear);
    }

    public void OnClickSword()
    {
        StateManager.Instance.SetCurrentWeapons(Weapons.Sword);
    }

    public void OnClickStartTravel()
    {
        GameManager.instance.ChangeScene("Dungeon");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
