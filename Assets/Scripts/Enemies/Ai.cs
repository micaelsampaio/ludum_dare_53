
using Core.Managers;
using Core.States;
using Game.Characters;
using UnityEngine;

namespace Scripts.Enemies
{
  public class Ai : Character
  {
    public StateMachine stateMachine;
    public float HitPlayerCooldown = -1;
    public bool CanDealDamageOnCollision = true;
    public bool MarkAsDead = false;

    public override void Awake()
    {
      if (MarkAsDead)
      {
        if (GameManager.Instance.GameState.zoneState.EnemiesKilled.Contains(id)){
          gameObject.SetActive(false);
          return;
        }
        OnDeath += (c) =>
        {
          GameManager.Instance.EnemiesKilled.Add(c.id);
        };
      }


      base.Awake();
    }


    public bool OnPlayerCollision(GameObject other)
    {
      if (!CanDealDamageOnCollision) return false;

      if (Utils.IsPlayer(other) && GameManager.time > HitPlayerCooldown)
      {
        HitPlayerCooldown = 0.15f;
        GameManager.Instance.Player.OnDamage(Damage);
        return true;
      }
      return false;
    }
  }
}
