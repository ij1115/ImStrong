using UnityEngine;
using SaveDataVC = SaveDataV2;

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
            GameData.Instance.DataLoad();
            GameManager.instance.ChangeScene("Lobby");
        }
    }
}
