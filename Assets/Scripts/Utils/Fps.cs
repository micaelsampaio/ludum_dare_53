using UnityEngine;
using System.Collections;

public class Fps : MonoBehaviour
{
  public float updateInterval = 0.1f; //How often should the number update
  float accum = 0.0f;
  int frames = 0;
  float timeleft;
  float fps;

  GUIStyle textStyle = new GUIStyle();

  // Use this for initialization
  void Start()
  {
    timeleft = updateInterval;
    textStyle.fontStyle = FontStyle.Bold;
    textStyle.fontSize = 50;
    textStyle.normal.textColor = Color.black;
  }

  // Update is called once per frame
  void Update()
  {
    timeleft -= Time.deltaTime;
    accum += Time.timeScale / Time.deltaTime;
    ++frames;

    if (timeleft <= 0.0)
    {
      fps = (accum / frames);
      timeleft = updateInterval;
      accum = 0.0f;
      frames = 0;
    }
  }

  void OnGUI()
  {
    GUI.Label(new Rect(Screen.width - 120, 120, 100, 25), fps.ToString("F2") + "FPS", textStyle);
  }
}