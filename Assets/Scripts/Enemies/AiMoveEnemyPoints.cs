


using Assets.Scripts.Enemies.States;
using Core.States;
using Game.Characters;
using Scripts.Enemies.States;
using UnityEngine;

namespace Scripts.Enemies
{
  public class AiMoveEnemyPoints : Ai
  {
    public AiMovePointsState MovePointsState;
    public AiWaitTimeState WaitState;
    public AiGateKeeperState GateKeeperState;
    public string HitEffectKey;

    public override void Awake()
    {
      base.Awake();

      CreateStates();

      OnDeath += OnCharacterDeath;
      // TODO
    }

    private void CreateStates()
    {

      var states = GetComponents<State>();

      if (GateKeeperState != null)
      {
        GateKeeperState.CreateState(this);
        GateKeeperState.NextState = MovePointsState;
      }

      stateMachine = new StateMachine(this, MovePointsState, states);
    }

    public override void Update()
    {
      base.Update();
      stateMachine.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (OnPlayerCollision(other.gameObject))
      {
        if (!string.IsNullOrEmpty(HitEffectKey)) Spawn(HitEffectKey, other.transform, transform);
      }
    }

    private void OnCharacterDeath(Character character)
    {
      gameObject.SetActive(false);
    }
  }
}
