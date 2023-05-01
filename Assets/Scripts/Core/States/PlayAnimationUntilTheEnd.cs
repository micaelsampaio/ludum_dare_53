using System;
using Core.States;
using Game.Characters;
using UnityEngine;

public class PlayAnimationUntilTheEnd : State
{
  [SerializeField] private Animator animator;
  [SerializeField] private string animationName;
  private float currentTime = 0;

  override public void InitState()
  {
    animator.Play(animationName);
    currentTime = 0;
  }

  override public void TickState()
  {
    currentTime += Time.deltaTime;

    if (currentTime > 0.2f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
    {
      OnFinishState?.Invoke();
    }
  }
}