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
    public GameObject HitEffect;

    [SerializeField] private Transform collisionPosition;
    [SerializeField] private Renderer Mesh;

    private void Start()
    {
      if (!Character || !Character.gameObject.activeSelf)
      {
        gameObject.SetActive(false);
      }
    }

    public override void CreateState(Character character)
    {
      base.CreateState(character);
      ai = character as Ai;

      Destroy(Mesh);

      transform.SetParent(null);
      collisionPosition.transform.SetParent(null);
      character.OnDeath += (c) => gameObject.SetActive(false);
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
      var elasped = time / maxTime;

      //if (state == 0)
      //{
      //  if (time > 0.15)
      //  {
      //    state = 1;
      //    time = 0;
      //  }

      //  ai.transform.position = player.transform.position + player.transform.forward;

      //  return;
      //}

      var targetPosition = Vector3.Lerp(player.transform.position, collisionPosition.position, elasped);
      player.transform.position = targetPosition;
      ai.transform.position = player.transform.position + player.transform.forward;

      if (elasped > 1)
      {
        Done = true;
        HitEffect.transform.position = player.transform.position + Vector3.up * 0.1f;
        HitEffect.SetActive(true);

        GoToNextState(ai.stateMachine);
      }
    }

    public override void EndState()
    {
      base.EndState();
      player.GivePlayerControl();
      player.OnDamage(2);
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
