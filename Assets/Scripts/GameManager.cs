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

    public SceneState currentState = SceneState.Title;

    public bool isGameover = false;

    public void ChangeScene(string state)
    {
        SceneState converter = (SceneState)Enum.Parse(typeof(SceneState), state);

        switch (converter)
        {
            case SceneState.Title:
                currentState = SceneState.Title;
                UIManager.Instance.Open(currentState);
                SceneManager.LoadScene(state);
                UIManager.Instance.StartFadeOut();
                break;
            case SceneState.Lobby:
                currentState = SceneState.Lobby;
                StateManager.Instance.CurrentToStandard();
                UIManager.Instance.Open(currentState);
                StateManager.Instance.PlayerStateSet();
                SceneManager.LoadScene(state);
                UIManager.Instance.StartFadeOut();
                break;
            case SceneState.Dungeon:
                currentState = SceneState.Dungeon;
                UIManager.Instance.Open(currentState);
                StateManager.Instance.MonsterSetUp();
                SceneManager.LoadScene(state);
                UIManager.Instance.StartFadeOut();
                break;
            //case SceneState.BossRoom:
            //    currentState = SceneState.BossRoom;
            //    UIManager.Instance.Open(currentState);
            //    StateManager.Instance.MonsterSetUp();
            //    SceneManager.LoadScene(state);
            //    UIManager.Instance.StartFadeOut();
            //    break;
        }
    }
}
