﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using Core.Pooling;
using Game.Characters;
using Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Core.Managers
{
  public class GameManager : MonoBehaviour
  {
    public static GameManager Instance;
    public static float time;
    public static string currentPortal;
    //public static int MAX_SOULS = 15;
    public static int MAX_SOULS = 10;

    public Camera MainCamera;

    public PlayerController Player;
    public PlayerInputActions InputActions;
    public GameUI GameUI;

    public GameState GameState;

    public event CharacterEvent OnPlayerDeathEvent;
    public event VoidEvent OnEndGameEvent;
    public Transform PlayerStartPosition;


    private void Awake()
    {
      Instance = this;

      PoolManager.Create();

      GameUI = Instantiate(GameUI);

      LoadGameState();
      InitCamera();
      InitInput();
      SetupPlayerPosition();
      BindPlayerEvents();

      Player.OnDeath += (c) => OnPlayerDeath();
      PlayerStartPosition.gameObject.SetActive(false);
    }

    private void Update()
    {
      time += Time.deltaTime;
    }

    private void InitCamera()
    {
      MainCamera = MainCamera != null ? MainCamera : Camera.main;
    }

    private void InitInput()
    {
      InputActions = new PlayerInputActions();
      InputActions.Player.Enable();
    }

    private void SetupPlayerPosition()
    {
      if (!string.IsNullOrEmpty(currentPortal))
      {
        var portal = Utils.FindPortalInScene(currentPortal);
        if (portal != null) Player.SetPosition(portal.transform.position + portal.transform.forward * 3f);
      }
      else
      {
        Player.transform.SetPositionAndRotation(PlayerStartPosition.position, PlayerStartPosition.rotation);
      }
    }

    private void BindPlayerEvents()
    {
      Player.OnUpdateSouls += hp => GameState.playerState.hp = hp;
    }

    private void LoadGameState()
    {
      var zone = SceneManager.GetActiveScene().name;
      GameState = DataManager.GetGameState(zone);
      GameState.lastZone = zone;
    }

    public void Save()
    {
      GameState.Save();
      DataManager.SaveAllData();
    }

    public void OnPlayerDeath()
    {
      StartCoroutine(PlayerDeathTask());
    }

    public void OnPlayerFall()
    {
      if (!string.IsNullOrEmpty(GameState.zoneState.CheckPointId))
      {
        Debug.Log("CHECKPOINT");

        Debug.DrawLine(Player.transform.position, GameState.zoneState.CheckPointPosition, Color.green);
        Debug.DrawLine(Vector3.up * 10f, GameState.zoneState.CheckPointPosition, Color.white);
        Player.SetPosition(GameState.zoneState.CheckPointPosition + Vector3.up * 0.05f);
        OnPlayerDeathEvent?.Invoke(Player);
        return;
      }

      Debug.Log("PLAYER POS");

      Player.SetPosition(PlayerStartPosition.position);
      Player.transform.rotation = PlayerStartPosition.rotation;
      Player.ResetPlayer();

      OnPlayerDeathEvent?.Invoke(Player);
    }

    private void OnDestroy()
    {
      InputActions.Disable();
      InputActions.Dispose();
    }

    private IEnumerator PlayerDeathTask()
    {
      yield return new WaitForSeconds(3);
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private bool _gameEnded = false;
    public void EndGame()
    {
      if (_gameEnded) return;
      _gameEnded = true;

      OnEndGameEvent?.Invoke();
      StartCoroutine(EndGameTask());
    }

    private IEnumerator EndGameTask()
    {
      yield return new WaitForSeconds(1.5f);
      SceneManager.LoadScene("EndGame");
    }
  }
}
