using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Managers;
using Assets.Scripts.Core.Tips;
using Assets.Scripts.Player;
using Core.Managers;
using Game.Characters;
using UnityEngine;

namespace Assets.Scripts.Tutorial
{

  public class Tutorial : MonoBehaviour
  {
    public List<TutorialStep> steps;
    public int currentStep = 0;
    [Header("CollectSouls")]
    public Tip CollectSoulTip;
    public Tip DeliverSoulTip;

    [Header("Enemy")]
    public Tip EnemyTip;
    public GameObject EnemyCollider;
    public GameObject EnemyCollider2;
    public ParticleSystem EnemyColliderEffect;
    public Character enemy;

    [Header("Portal")]
    public Tip PortalTip;
    public GameObject PortalCollider;
    public ParticleSystem PortalColliderEffect;

    private Coroutine TempCoroutine;

    private void Awake()
    {
      var gm = GameManager.Instance;
      var player = gm.Player;

      steps = new List<TutorialStep>();

      void OnUpdateSoulsToDeliver(int value)
      {
        if (value > 0)
          NextStep();
      }

      steps.Add(new TutorialStep()
      {
        Start = () =>
        {
          player.PlayerSouls.OnUpdateSoulsToDeliver += OnUpdateSoulsToDeliver;
          TipManager.Instance.ShowTip(CollectSoulTip);
        },
        End = () =>
        {
          player.PlayerSouls.OnUpdateSoulsToDeliver -= OnUpdateSoulsToDeliver;
        }
      });

      void OnSouldToDeliverUpdate(int value)
      {
        Debug.Log("Deliver SOULS " + value);
        NextStep();
      }

      steps.Add(new TutorialStep()
      {
        Start = () =>
        {
          TipManager.Instance.ShowTip(DeliverSoulTip);
          gm.OnSouldToDeliverUpdate += OnSouldToDeliverUpdate;
        },
        End = () =>
        {
          Debug.Log(" TUTORIAL 2 END");
          gm.OnSouldToDeliverUpdate -= OnSouldToDeliverUpdate;
          CloseTips();
        }
      });

      steps.Add(new TutorialStep()
      {
        Start = () =>
        {
          TipManager.Instance.ShowTip(EnemyTip);
          EnemyCollider.SetActive(false);
          EnemyCollider2.SetActive(false);
          EnemyColliderEffect.Stop();
          enemy.OnDeath += (c) => NextStep();
        }
      });

      steps.Add(new TutorialStep()
      {
        Start = () =>
        {
          TipManager.Instance.ShowTip(PortalTip);
          PortalCollider.SetActive(false);
          PortalColliderEffect.Stop();
        }
      });
    }

    private void CloseTips()
    {
      if (!TipManager.Instance.isActiveAndEnabled) return;
      TipManager.Instance.CloseTip();
    }

    private void Start()
    {
      PortalCollider.GetComponent<Renderer>().enabled = false;
      EnemyCollider.GetComponent<Renderer>().enabled = false;
      EnemyCollider2.GetComponent<Renderer>().enabled = false;

      var parentTips = TipManager.Instance.Canvas.transform;

      foreach (var tip in GameObject.FindObjectsOfType<Tip>())
      {
        tip.gameObject.SetActive(false);
        tip.transform.SetParent(parentTips);

        RectTransform rectTransform = tip.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
      }

      steps[currentStep].Start();
    }

    public void NextStep()
    {
      if (TempCoroutine != null) StopCoroutine(TempCoroutine);

      if (currentStep > 0)
      {
        steps[currentStep].End?.Invoke();
      }
      if (++currentStep < steps.Count)
      {
        steps[currentStep].Start?.Invoke();
      }
    }


  }
}
