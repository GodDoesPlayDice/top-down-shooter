using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsMeter : MonoBehaviour
{
	float deltaTime = 0.0f;

	private GUIStyle style;
	private Rect rect;

	private void Start()
    {
		style = new GUIStyle();

		int w = Screen.width, h = Screen.height;
		rect = new Rect(0, 0, w, h * 2 / 100);

		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 4 / 100;
		style.normal.textColor = new Color(0.5f, 0.5f, 0.0f, 1.0f);
	}

    void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps) {2:0.0}", msec, fps, Time.timeScale);
		GUI.Label(rect, text, style);
	}
}
