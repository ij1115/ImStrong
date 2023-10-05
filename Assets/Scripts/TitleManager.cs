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

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            GameManager.instance.ChangeScene("Lobby");
        }
    }
}
