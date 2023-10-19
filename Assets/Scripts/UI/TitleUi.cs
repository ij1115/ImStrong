using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleUi : SceneUI
{
    [SerializeField] private GameObject titleLogo;
    [SerializeField] private GameObject pressText;
    [SerializeField] private GameObject makeName;
    [SerializeField] private Button makeNameButton;
    [SerializeField] private Slider loading;

    private int charLimit = 10;
    public override void Open()
    {
        titleLogo.SetActive(true);
        loading.gameObject.SetActive(true);
        pressText.SetActive(false);
        makeName.SetActive(false);

        base.Open();
    }

    public override void Close()
    {
        titleLogo.SetActive(false);
        loading.gameObject.SetActive(false);
        pressText.SetActive(false);
        makeName.SetActive(false);

        base.Close();
    }

    public void LoadingSuccess()
    {
        loading.gameObject.SetActive(false);
        pressText.SetActive(true);
    }

    public void MakeNameWindow()
    {
        titleLogo.SetActive(false);
        pressText.SetActive(false);
        makeName.SetActive(true);
        makeName.GetComponentInChildren<InputField>().onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    public Slider LoadingBar()
    {
        return loading;
    }

    public void OnInputFieldValueChanged(string text)
    {
        if(text.Length>charLimit)
        {
            makeName.GetComponentInChildren<InputField>().text = text.Substring(0, charLimit);
        }
    }

    public void OnClickNameMake()
    {
        if (makeName.GetComponentInChildren<InputField>().text.Length > 0)
        {
            GameData.Instance.data.name = makeName.GetComponentInChildren<InputField>().text;
            GameData.Instance.DataSave();
            UIManager.Instance.StartFadeIn("Lobby");
        }
    }
}
