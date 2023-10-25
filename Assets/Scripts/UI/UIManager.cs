using UnityEngine;
using System.Collections;
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

    public SceneUI[] uis;

    public SceneState currentUi;
    public SceneState defaultUi;

    public GameObject fadeInOut;
    public Coroutine fadeCo;

    public float fadeTime = 0.5f;
    private float accumTime = 0f;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
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

    public void StartFadeIn(string scene)
    {
        fadeInOut.SetActive(true);

        if(fadeCo != null)
        {
            StopCoroutine(fadeCo);
            fadeCo = null;
        }
        fadeCo = StartCoroutine(FadeIn(scene));
    }

    public void StartFadeOut()
    {
        if(fadeCo != null)
        {
            StopCoroutine(fadeCo);
            fadeCo = null;
        }
        fadeCo = StartCoroutine(FadeOut());
    }
    public IEnumerator FadeIn(string scene)
    {
        accumTime = 0f;
        Color nextColor = fadeInOut.GetComponent<Image>().color;
        while (accumTime<fadeTime)
        {
            nextColor.a = Mathf.Lerp(0f, 1f, accumTime / fadeTime);
            fadeInOut.GetComponent<Image>().color = nextColor;
            yield return null;
            accumTime += Time.deltaTime;
        }
        nextColor.a = 1f;
        fadeInOut.GetComponent<Image>().color = nextColor;

        GameManager.instance.ChangeScene(scene);

        Camera cam = gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        cam.GetComponent<CameraScreen>().CameraViewSize();
    }
    public IEnumerator FadeOut()
    {
        accumTime = 0f;
        Color nextColor = fadeInOut.GetComponent<Image>().color;
        while (accumTime < fadeTime)
        {
            nextColor.a = Mathf.Lerp(1f, 0f, accumTime / fadeTime);
            fadeInOut.GetComponent<Image>().color = nextColor;
            yield return null;
            accumTime += Time.deltaTime;
        }
        nextColor.a = 0f;
        fadeInOut.GetComponent<Image>().color = nextColor;
        fadeInOut.SetActive(false);
    }
}
