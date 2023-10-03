using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
public static GameManager instance
    {
        get
        {
            if(singleton == null)
            {
                singleton = FindObjectOfType<GameManager>();
            }
            return singleton;
        }
    }

    private static GameManager singleton;

    public bool isGameover = false;
}
