using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }

    public int targetWidth = 2340; // 원하는 해상도 너비
    public int targetHeight = 1080; // 원하는 해상도 높이
    public bool fullScreen = true; // 전체 화면 여부

    public void SetResolution(int width, int height, bool fullScreen)
    {
        Screen.SetResolution(width, height, fullScreen);
    }

    public SceneUI[] uis;

    public SceneState currentUi;
    public SceneState defaultUi;
   
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SetResolution(targetWidth, targetHeight, fullScreen);
        Open(defaultUi);
    }

    public SceneUI Open(SceneState scene)
    {
        if (uis[(int)currentUi].gameObject.activeSelf)
        {
            uis[(int)currentUi].Close();
        }
        currentUi = scene;

        uis[(int)currentUi].Open();
        return uis[(int)currentUi];
    }
}