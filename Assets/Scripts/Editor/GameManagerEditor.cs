using System;
using Assets.Scripts.Map;
using Core.Managers;
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
      PlayerPrefs.DeleteKey("gs");
      PlayerPrefs.DeleteKey("z_ZoneA");
      PlayerPrefs.DeleteKey("z_ZoneB");
      PlayerPrefs.DeleteKey("z_ZoneC");
      PlayerPrefs.Save();
    }
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


    AssetDatabase.SaveAssets();
  }
}
