using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private GameManager gameManager;

    public Button start;

    private void Awake()
    {
        gameManager = GameManager.instance;
    }
    public void OnClickStartTravel()
    {
        gameManager.ChangeScene("Dungeon");
    }
}
