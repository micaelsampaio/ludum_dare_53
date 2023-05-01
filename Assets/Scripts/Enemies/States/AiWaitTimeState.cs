using System;
using Core.States;
using UnityEngine;

namespace Scripts.Enemies.States
{
  public class AiWaitTimeState : State
  {
    public float secondsToWait;
    public float waitSeconds;
    public void SetWaitTime(float s, Action onEndState)
    {
      secondsToWait = s;
      OnFinishState = onEndState;
    }
    public void SetWaitTime(float min, float max, Action onEndState)
    {
      secondsToWait = UnityEngine.Random.Range(min, max);
      OnFinishState = onEndState;
    }

    public override void InitState()
    {
      base.InitState();
      waitSeconds = 0;
    }
    public override void TickState()
    {
      base.TickState();
      waitSeconds += Time.deltaTime;

      if (waitSeconds > secondsToWait)
      {
        waitSeconds = 0;
        OnFinishState.Invoke();
      }
    }

    public override void EndState()
    {
      base.EndState();
      waitSeconds = 0;
      OnFinishState = null;
    }

  }
}
