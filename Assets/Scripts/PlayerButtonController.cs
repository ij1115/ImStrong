using UnityEngine.UI;

public class PlayerButtonController : Button
{
    public bool ButtonPressed
    {
        get { return IsInteractable() && IsPressed(); }
    }
}
