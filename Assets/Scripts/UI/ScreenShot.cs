
using System;
using System.IO;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
  public void CaptureImage()
  {
    long currentTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    string fileName = currentTimeSeconds + ".png";
    string screenshotFilePath = Path.Combine(Application.persistentDataPath, fileName);
    ScreenCapture.CaptureScreenshot(screenshotFilePath);
    Debug.Log("Screenshot saved to: " + screenshotFilePath);

  }
}