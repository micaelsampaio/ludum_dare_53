using Assets.Scripts.Characters;
using Assets.Scripts.Core.Collisions;
using Core.Managers;
using Core.States;
using Game.Characters;
using Scripts.Player;
using UnityEngine;

public class PlayerThrowSoul : State
{
  public PlayerController player;
  public string currentProjectile;
  public Character target;
  public float NextShotCooldown = 2f;
  public float NextShotCooldownTimer = -1;

  public override void CreateState(Character character)
  {
    base.CreateState(character);
    player = character as PlayerController;

    player.Input.Player.Attack.performed += (ctx) =>
    {
      if (ctx.performed)
      {
        ValidateStartState();
      }
    };
  }

  public override void InitState()
  {
    base.InitState();

    var clone = player.Spawn(currentProjectile, transform, transform);
    var projectile = clone.GetComponent<Projectile>();
    projectile.Setup(Character, target.transform, Utils.ENEMIES_LAYER);
    projectile.SetTargetPosition(target.GetSocket(CharacterSocketType.TARGET));
    projectile.transform.position = player.PlayerSouls.transform.position;
    projectile.gameObject.SetActive(true);

    player.StateMachine.SetState(player.IdleState);

    NextShotCooldownTimer = GameManager.time + NextShotCooldown;
  }

  public override void EndState()
  {
    base.EndState();

    target = null;
  }

  private void ValidateStartState()
  {
    if (!player.hasControl || player.GetStateName() != STATE_NAME.IDLE || NextShotCooldownTimer > GameManager.time) return;
    var target = Utils.GetTargetLookAt(transform, Utils.ENEMIES_LAYER, 6f);
    if (target == null) return;
    this.target = target;
    player.StateMachine.SetState(this);
  }
}
