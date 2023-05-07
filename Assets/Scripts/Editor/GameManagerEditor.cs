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

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    // TODO

    if (GUILayout.Button("CREATE ID's"))
    {
      CreateIDs();
    }

    GUILayout.Space(50);

    if (GUILayout.Button("DELETE STORAGE"))
    {
      DataManager.SetNewGame();
      PlayerPrefs.Save();
      Debug.Log("DELETED GAME DATA");
    }

    GUILayout.Space(10);

    if (GUILayout.Button("Souls To Catch"))
    {
      GetSoulsToCatch();
    }
  }

  private void GetSoulsToCatch()
  {
    var souls = FindObjectsOfType<SoulObject>();
    var soulsC = 0;

    foreach (var soul in souls)
    {
      if (soul.ToCatch) soulsC++;
    }

    UnityEngine.Debug.Log("SOULS TO CATCH ---> " + soulsC);
  }

  private void CreateIDs()
  {
    var platforms = FindObjectsOfType<PlatformCheckPoint>();
    var id = EditorSceneManager.GetActiveScene().name.ToLower().Replace("zone", "") + "-";
    foreach (var script in platforms)
    {
      if (string.IsNullOrEmpty(script.id))
      {
        script.id = id + Guid.NewGuid().ToString();
        EditorUtility.SetDirty(script);
        EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
      }
    }

    var souls = FindObjectsOfType<SoulObject>();

    foreach (var script in souls)
    {
      if (string.IsNullOrEmpty(script.id))
      {
        script.id = id + Guid.NewGuid().ToString();
        EditorUtility.SetDirty(script);
        EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
      }
    }

    var characters = FindObjectsOfType<Character>();
    foreach (var script in characters)
    {
      if (string.IsNullOrEmpty(script.id))
      {
        script.id = id + Guid.NewGuid().ToString();
        EditorUtility.SetDirty(script);
        EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
      }
    }

    AssetDatabase.SaveAssets();
  }
}
