using UnityEngine;
using Core.States;
using Core.Managers;
using Game.Characters;
using Scripts.Enemies;
using Scripts.Player;

namespace Assets.Scripts.Enemies.States
{
  public class AiGateKeeperState : State
  {
    private Ai ai;
    private PlayerController player;
    private float time;
    private float maxTime;
    private int state;

    [SerializeField] private Transform collisionPosition;
    [SerializeField] private Renderer Mesh;

    public override void CreateState(Character character)
    {
      base.CreateState(character);
      ai = character as Ai;

      Destroy(Mesh);

      transform.SetParent(null);
      collisionPosition.transform.SetParent(null);
    }

    public override void InitState()
    {
      base.InitState();

      player.TakePlayerControl();
      ai.CanDealDamageOnCollision = false;
      player.transform.rotation = collisionPosition.rotation;
      time = 0;
      maxTime = 0.4f;
      state = 0;
      Done = false;
    }

    public override void TickState()
    {
      if (Done) return;
      base.TickState();

      time += Time.deltaTime;

      if (state == 0)
      {
        if (time > 0.15)
        {
          state = 1;
          time = 0;
        }

        ai.transform.position = player.transform.position + player.transform.forward;

        return;
      }

      var targetPosition = Vector3.Lerp(player.transform.position, collisionPosition.position, time / maxTime);
      player.transform.position = targetPosition;
      ai.transform.position = player.transform.position + player.transform.forward;

      if (time > maxTime)
      {
        player.Spawn("jump_dust", player.transform, player.transform);
        Done = true;
        GoToNextState(ai.stateMachine);
      }
    }

    public override void EndState()
    {
      base.EndState();
      player.GivePlayerControl();
      ai.CanDealDamageOnCollision = false;
    }
    private void OnTriggerEnter(Collider other)
    {
      if (Utils.IsPlayer(other))
      {
        player = GameManager.Instance.Player;
        ai.stateMachine.SetState(this);
      }
    }

    private void OnDrawGizmos()
    {
      Gizmos.color = Color.yellow;
      if (Character) Gizmos.DrawLine(transform.position, Character.transform.position);

      Gizmos.color = Color.blue;
      if (collisionPosition) Gizmos.DrawLine(transform.position, collisionPosition.transform.position);
    }
  }
}
