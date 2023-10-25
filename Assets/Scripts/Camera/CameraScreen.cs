using UnityEngine;

public class CameraScreen : MonoBehaviour
{
    private void Awake()
    {
        CameraViewSize();
    }
    public void CameraViewSize()
    {
        Camera cam = Camera.main;

        Rect rect = cam.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float) 13f / 6f);
        float scalewidth = 1f / scaleheight;

        if(scaleheight<1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }

        cam.rect = rect;
    }

    private void OnPreCull() => GL.Clear(true, true, Color.black);
}
