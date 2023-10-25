using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] RectTransform safeAreaPanel;

    private void Awake()
    {
        UpdateSafeArea();
    }

    private void UpdateSafeArea()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 min = safeArea.position;
        Vector2 max = safeArea.position + safeArea.size;

        safeAreaPanel.anchorMin = new Vector2(min.x/ Screen.width, min.y / Screen.height);
        safeAreaPanel.anchorMax = new Vector2(max.x/ Screen.width, max.y/ Screen.height);
    }
}
