using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private GUIStyle style = new GUIStyle();

    private void Start()
    {
        style.fontSize = 20;
        style.normal.textColor = Color.white;
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        int fps = Mathf.RoundToInt(1.0f / deltaTime);
        if (fps < 60f)
        {
            style.normal.textColor = Color.red;
        }
        else style.normal.textColor = Color.white;

        string text = $"FPS: {fps}";

        GUI.Label(new Rect(10, 10, 100, 20), text, style);
    }
}