using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUi : SceneUI
{
    [SerializeField] private GameObject titleLogo;
    [SerializeField] private GameObject pressText;

    public override void Open()
    {
        titleLogo.SetActive(true);
        pressText.SetActive(true);

        base.Open();
    }

    public override void Close()
    {
        titleLogo.SetActive(false);
        pressText.SetActive(false);

        base.Close();
    }
}
