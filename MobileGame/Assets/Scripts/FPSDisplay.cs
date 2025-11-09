using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    bool isOn = false;
    float deltaTime = 0.0f;
    GUIStyle style;
    Rect rect;

    void Start()
    {
        if (!isOn) return;
        int w = Screen.width, h = Screen.height;

        rect = new Rect(10, 10, w, h * 2 / 100);

        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h / 30;
        style.normal.textColor = Color.white;
        Application.targetFrameRate = 60;

    }

    void Update()
    {
        if (!isOn) return;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (!isOn) return;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS", fps);
        GUI.Label(rect, text, style);
    }
}
