using System;
using System.Collections;
using System.Collections.Generic;
using Core.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveGameManager : MonoBehaviour
{
  private float nextSave = -1;
  private string GetInteractionText()
  {
    return "Save game";
  }

  private void OnTriggerEnter(Collider other)
  {
    if (Utils.IsPlayer(other.gameObject))
    {
      GameManager.Instance.InputActions.Player.Interact.started += OnSaveGame;
      GameManager.Instance.GameUI.AddInteraction("E", GetInteractionText());
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (Utils.IsPlayer(other.gameObject))
    {
      GameManager.Instance.InputActions.Player.Interact.started -= OnSaveGame;
      GameManager.Instance.GameUI.RemoveInteraction(GetInteractionText());
    }
  }

  private void OnSaveGame(InputAction.CallbackContext obj)
  {
    if (GameManager.time > nextSave)
    {
      GameManager.Instance.GameState.Save();
      nextSave = GameManager.time + 1;

      GameManager.Instance.InputActions.Player.Interact.started -= OnSaveGame;
      GameManager.Instance.GameUI.RemoveInteraction(GetInteractionText());
    }
  }
}
