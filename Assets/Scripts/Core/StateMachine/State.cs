
using System;
using UnityEngine;
using Game.Characters;

namespace Core.States
{
  public interface IState
  {
    STATE_NAME Name { get; }
    IState NextState { get; }
    Action OnFinishState { get; }
    bool Done { get; }
    void CreateState(Character character);
    void InitState();
    void TickState();
    void FixedTickState();
    void EndState();
    bool IsAvaliable();
    void GoToNextState(StateMachine stateMachine = null);
  }
  public class State : MonoBehaviour, IState
  {
    [Header("[STATE]")]
    public STATE_NAME _Name = STATE_NAME.NONE;
    public Character Character;
    public STATE_NAME Name { get => _Name; }
    public Action OnFinishState { get; set; } = null;
    public IState NextState { get; set; } = null;
    public bool Done { set; get; } = false;

    public virtual bool IsAvaliable() => true;
    public virtual void CreateState(Character character)
    {
      Character = character;
    }
    public virtual void InitState() { }
    public virtual void TickState() { }
    public virtual void FixedTickState() { }
    public virtual void EndState() { }

    public virtual void GoToNextState(StateMachine stateMachine = null)
    {
      if (stateMachine != null && NextState != null) stateMachine.SetState(NextState);
      if (OnFinishState != null) OnFinishState.Invoke();
    }
  }

  public enum STATE_NAME
  {
    NONE = 0,
    IDLE,
    IDLE_WAIT,
    ATTACK,
    GO_TO_POS,
    FOLLOW_TARGET,
    RUN_FROM_TARGET,
    DEATH,
    SPAWN,
    PUSH,
    DASH,
    JUMP,
    MOVE,
    BARKING,
    END,
    TURN,
    ETC = 8888,
    UNKNOWN = 9000
  }
}