
using System;
using Core.Managers;
using UnityEngine;

namespace Assets.Scripts.Data
{
  public class DataManager
  {
    public static bool HasGameData()
    {
      return !string.IsNullOrEmpty(PlayerPrefs.GetString("gs"));
    }

    public static void SetNewGame()
    {
      PlayerPrefs.DeleteKey("gs");
      PlayerPrefs.DeleteKey("zoneA");
      PlayerPrefs.DeleteKey("zoneB");
      PlayerPrefs.DeleteKey("zoneC");
      PlayerPrefs.DeleteKey("zoneD");
      SaveAllData();
    }
    public static int GetGamePercent()
    {
      var gameStateRaw = PlayerPrefs.GetString("gs");
      var gameState = JsonUtility.FromJson<GameState>(gameStateRaw);
      Debug.Log(" ---> " + gameState.SoulsDelivered);
      return (int)((gameState.SoulsDelivered * 1.0f / GameManager.MAX_SOULS) * 100f);
    }
    public static void SetGameState(GameState state)
    {
      var json = JsonUtility.ToJson(state);
      PlayerPrefs.SetString("gs", json);


      if (state.zoneState != null)
      {
        var jsonZone = JsonUtility.ToJson(state.zoneState);
        PlayerPrefs.SetString($"z_{state.zoneState.name}", jsonZone);
        Debug.Log("ZONE STATE: " + jsonZone);
      }
      Debug.Log("GAME STATE: " + json);
    }

    public static GameState GetGameState(string zone)
    {
      var gameStateRaw = PlayerPrefs.GetString("gs");
      Debug.Log("GAME STATE: " + gameStateRaw);
      var zoneState = GetZone(zone);
      if (string.IsNullOrEmpty(gameStateRaw))
      {
        var gameState1 = GameState.Create();
        gameState1.zoneState = zoneState;
        return gameState1;
      }
      var gameState = JsonUtility.FromJson<GameState>(gameStateRaw);
      gameState.zoneState = zoneState;
      return gameState;
    }

    public static ZoneState GetZone(string zone)
    {
      var zoneStateRaw = PlayerPrefs.GetString($"z_{zone}");
      Debug.Log("ZONE STATE: " + zoneStateRaw);

      if (string.IsNullOrEmpty(zoneStateRaw))
      {
        return ZoneState.Create(zone);
      }

      return JsonUtility.FromJson<ZoneState>(zoneStateRaw);
    }

    public static void SaveAllData()
    {
      PlayerPrefs.Save();
    }

    public static string GetContinueGameZone()
    {
      var gameStateRaw = PlayerPrefs.GetString("gs");
      var gameState = JsonUtility.FromJson<GameState>(gameStateRaw);

      return string.IsNullOrEmpty(gameState.lastZone) ? "ZoneA" : gameState.lastZone;
    }
  }
}
