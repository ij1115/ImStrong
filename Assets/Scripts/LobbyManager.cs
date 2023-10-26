using TMPro;
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

    public GameObject player;
    public GameObject nameCanv;
    public TextMeshProUGUI playerName;

    public Camera currCamera;

    private void Awake()
    {
        StateManager.Instance.StandardSetUp();
        UIManager.Instance.ChangeCamera(currCamera);
        NameSet();
    }

    public void WeaponsChange()
    {
        player.GetComponent<PlayerWeapons>().RunTimeSwap();
    }

    public void NameSet()
    {
        if (playerName.text.Length < 1)
        {
            playerName.text = GameData.Instance.data.name;
        }
        nameCanv.SetActive(true);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
