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

    private void Awake()
    {
        StateManager.Instance.StandardSetUp();
    }

    public void OnClickStartTravel()
    {
        GameManager.instance.ChangeScene("Dungeon");
    }
}
