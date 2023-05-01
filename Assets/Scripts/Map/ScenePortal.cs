using System;
using System.Collections;
using System.Collections.Generic;
using Core.Collisions;
using Core.Managers;
using Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
  public string NextScene;
  public string PortalId;
  public Animator Animator;

  public OnTriggerEnterCallback triggerEvents;

  private void Start()
  {
    triggerEvents.transform.SetParent(null);
    triggerEvents.OnTriggerEnterEvent += (other) =>
    {
      if (Utils.IsPlayer(other.gameObject))
        Animator.SetBool("open", true);
    };
    triggerEvents.OnTriggerExitEvent += (other) =>
    {
      if (Utils.IsPlayer(other.gameObject))
        Animator.SetBool("open", false);
    };
  }

  private void OnTriggerEnter(Collider other)
  {
    if (Utils.IsPlayer(other.gameObject))
    {
      GameManager.Instance.InputActions.Player.Jump.started += OnEnterPortal;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (Utils.IsPlayer(other.gameObject))
    {
      GameManager.Instance.InputActions.Player.Jump.started -= OnEnterPortal;
    }
  }

  private void OnEnterPortal(InputAction.CallbackContext obj)
  {
    GameManager.Instance.Save();
    GameManager.Instance.InputActions.Player.Jump.started -= OnEnterPortal;
    GameManager.currentPortal = PortalId;
    SceneManager.LoadScene(NextScene);
  }
}
