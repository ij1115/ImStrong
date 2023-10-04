using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType<GameManager>();
            }
            return singleton;
        }
    }

    private static GameManager singleton;

    public enum SceneState
    {
        Title,
        Lobby,
        Dungeon,
        BossRoom,
    }

    public SceneState currentState = SceneState.Title;

    public bool isGameover = false;

    public void ChangeScene(string state)
    {
        SceneState converter = (SceneState)Enum.Parse(typeof(SceneState), state);

        switch (converter)
        {
            case SceneState.Title:
                currentState = SceneState.Title;
                SceneManager.LoadScene(state);
                break;
            case SceneState.Lobby:
                currentState = SceneState.Lobby;
                SceneManager.LoadScene(state);
                break;
            case SceneState.Dungeon:
                currentState = SceneState.Dungeon;
                SceneManager.LoadScene(state);
                break;
            case SceneState.BossRoom:
                currentState = SceneState.BossRoom;
                SceneManager.LoadScene(state);
                break;
        }
    }

}
