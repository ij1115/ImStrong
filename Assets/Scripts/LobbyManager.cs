using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
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
