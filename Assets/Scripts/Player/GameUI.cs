using System;
using System.Collections;
using System.Collections.Generic;
using Core.Managers;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{

  [SerializeField] private TextMeshProUGUI soulsText;
  [SerializeField] private TextMeshProUGUI soulsToDeliverText;
  [SerializeField] private TextMeshProUGUI soulsToDeliveredText;

  [SerializeField] private GameObject Interaction;
  [SerializeField] private TextMeshProUGUI InteractionKeyTxt;
  [SerializeField] private TextMeshProUGUI InteractionTxt;

  private void Start()
  {
    var gameState = GameManager.Instance.GameState;
    var playerState = gameState.playerState;
    soulsText.text = playerState.hp.ToString();
    soulsToDeliverText.text = playerState.soulsToDeliver.ToString();
    soulsToDeliveredText.text = gameState.SoulsDelivered.ToString() + " / " + Mathf.Max(gameState.SoulsDelivered, GameManager.MAX_SOULS).ToString();

    GameManager.Instance.Player.OnUpdateHp += c => UpdateSouls(c.Hp);
    GameManager.Instance.Player.OnUpdateSouls += UpdateSouls;
    GameManager.Instance.Player.PlayerSouls.OnUpdateSoulsToDeliver += UpdateSoulsToDeliver;

    Interaction.SetActive(false);
  }

  private void UpdateSoulsToDeliver(int index)
  {
    soulsToDeliverText.text = index.ToString();
  }

  private void UpdateSouls(int index)
  {
    soulsText.text = index.ToString();
  }

  public void UpdateDeliveredSouls(int index)
  {
    soulsToDeliveredText.text = index.ToString() + " / " + Mathf.Max(index, GameManager.MAX_SOULS).ToString();

  }

  public void AddInteraction(string key, string text)
  {
    InteractionKeyTxt.text = $"[{key}]";
    InteractionTxt.text = text;
    Interaction.SetActive(true);
  }

  public void RemoveInteraction(string text)
  {
    if (InteractionTxt.text == text)
    {
      Interaction.SetActive(false);
    }
  }


}
