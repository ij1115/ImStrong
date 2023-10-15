using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerButtonController : Button
{
    public bool ButtonPressed
    {
        get { return IsInteractable() && IsPressed(); }
    }
}
