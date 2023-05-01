
using System.Collections.Generic;
using Game.Characters;
using Scripts.Player;

namespace Core.States
{
  public class StateMachine
  {
    public IState CurrentState { get; private set; }
    public bool Lock = false;

    public StateMachine(State initialState)
    {
      SetState(initialState);
    }

    public StateMachine(Character character, State initialState, params State[] states)
    {
      CreateStates(character, states);
      SetState(initialState);
    }

    public void CreateStates(Character character, params State[] states)
    {
      foreach (var state in states)
      {
        state.CreateState(character);
      }
    }


    public void SetState(IState state)
    {
      if (Lock) return;

      CurrentState?.EndState();
      CurrentState = state;
      CurrentState.InitState();
    }

    public void Update()
    {
      CurrentState.TickState();
    }

    public void FixedUpdate()
    {
      CurrentState.FixedTickState();
    }
  }
}