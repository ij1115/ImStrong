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
    private void Awake()
    {
        StateManager.Instance.StandardSetUp();
    }

    public void WeaponsChange()
    {
        player.GetComponent<PlayerWeapons>().RunTimeSwap();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
