using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core.Managers;
using Assets.Scripts.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

  public Button NewGameBtn;
  public Button ContinueGameBtn;
  public TextMeshProUGUI GamePercentage;


  void Start()
  {
    var hasContinueGame = DataManager.HasGameData();

    Debug.Log("hasContinueGame " + hasContinueGame);

    NewGameBtn.onClick.AddListener(StartNewGame);

    if (hasContinueGame)
    {
      ContinueGameBtn.onClick.AddListener(ContinueGame);
      GamePercentage.text = DataManager.GetGamePercent().ToString() + "%";
    }

    ContinueGameBtn.interactable = hasContinueGame;
    GamePercentage.gameObject.SetActive(hasContinueGame);


    var audios = FindObjectsOfType<AudioManager>();
    foreach (var audio in audios) DestroyImmediate(audio);
  }

  private void StartNewGame()
  {
    DataManager.SetNewGame();
    StartCoroutine(LoadLevel("ZoneA"));
  }
  private void ContinueGame()
  {
    var zone = DataManager.GetContinueGameZone();
    StartCoroutine(LoadLevel(zone));
  }

  string workingLevel = "";
  private IEnumerator LoadLevel(string level)
  {
    if (!string.IsNullOrEmpty(workingLevel)) yield break;

    workingLevel = level;

    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(level);
    asyncOperation.allowSceneActivation = false;
    Debug.Log("Pro :" + asyncOperation.progress);

    while (!asyncOperation.isDone)
    {
      if (asyncOperation.progress >= 0.9f)
      {
        asyncOperation.allowSceneActivation = true;
      }

      yield return null;
    }
  }
}
