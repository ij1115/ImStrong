using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using SaveDataVC = SaveDataV3;

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

    private Coroutine start = null;

    private bool loading = false;

    private bool makeName = false;

    public Camera currCamera;

    // Update is called once per frame
    void Update()
    {
        if(start == null)
        {
            start = StartCoroutine(SetUp());
        }

        if (loading)
        {
            if (Input.anyKeyDown&&!makeName)
            {
                if (GameData.Instance.data.name == null||GameData.Instance.data.name.Length<1)
                {
                    makeName = true;
                    UIManager.Instance.uis[0].GetComponent<TitleUi>().MakeNameWindow();
                }
                else
                {
                    UIManager.Instance.StartFadeIn("Lobby");
                }
            }
        }
    }

    private IEnumerator SetUp()
    {
        UIManager.Instance.ChangeCamera(currCamera);

        Slider loadbar = GetComponent<Slider>();
        int loadingCount = 0;
        while (loadingCount < 3)
        {
            switch (loadingCount)
            {
                case 0:
                    loadbar = UIManager.Instance.uis[0].GetComponent<TitleUi>().LoadingBar();
                    loadbar.minValue = 0;
                    loadbar.maxValue = 3; 
                    break;
                case 1:
                    StateManager.Instance.StateManagerLoad();
                    break;
                case 2:
                    GameData.Instance.DataLoad();
                    break;
            }
            ++loadingCount;
            loadbar.value = loadingCount;
            yield return new WaitForSeconds(1f);
        }
        UIManager.Instance.uis[0].GetComponent<TitleUi>().LoadingSuccess();
        loading = true;
    }
}
