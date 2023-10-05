using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public static TitleManager instance
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType<TitleManager>();
            }
            return singleton;
        }
    }

    private static TitleManager singleton;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            gameManager.ChangeScene("Lobby");
        }
    }
}
