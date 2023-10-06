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
                SceneManager.LoadScene(state);
                break;
            case SceneState.Lobby:
                currentState = SceneState.Lobby;
                StateManager.Instance.CurrentToStandard();
                SceneManager.LoadScene(state);
                break;
            case SceneState.Dungeon:
                currentState = SceneState.Dungeon;
                StateManager.Instance.MonsterSetUp();
                SceneManager.LoadScene(state);
                break;
            case SceneState.BossRoom:
                currentState = SceneState.BossRoom;
                StateManager.Instance.MonsterSetUp();
                SceneManager.LoadScene(state);
                break;
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            StateManager.Instance.PlayerStateLog();
        }
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            StateManager.Instance.MobStateLog();
        }
        if(Input.GetKeyDown(KeyCode.Alpha9))
        { 
            StateManager.Instance.SubBossStateLog();
        }
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            StateManager.Instance.BossStateLog();
        }
    }

}
