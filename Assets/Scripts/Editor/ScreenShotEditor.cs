using System;
using Assets.Scripts.Data;
using Assets.Scripts.Map;
using Core.Managers;
using Game.Characters;
using Scripts.Map;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

[CustomEditor(typeof(ScreenShot))]
public class ScreenShotEditor : Editor
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    if (GUILayout.Button("CAPTURE"))
    {
      var script = target as ScreenShot;
      script.CaptureImage();
    }
  }
}
