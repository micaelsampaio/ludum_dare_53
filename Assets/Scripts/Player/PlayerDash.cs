using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using Core.Collisions;
using Core.Managers;
using Core.States;
using Game.Characters;
using Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Player
{
  public class PlayerDash : State
  {
    private bool isActive;
    public Vector3 HitSize;
    public Vector3 PosOffset;
    public Vector3 Direction;
    public float DashTime;
    public float DashSpeed;
    public float DashSpeedAir;
    public float time;
    public PlayerController player;
    private bool inAir;
    private Vector3 velocity;
    private float acceleration;

    public override void CreateState(Character character)
    {
      base.CreateState(character);
      player = character as PlayerController;

      player.Input.Player.Dash.performed += (ctx) =>
      {
        if (ctx.performed && player.hasControl && player.canDash)
        {
          player.StateMachine.SetState(this);
        }
      };
    }

    public override void InitState()
    {
      base.InitState();

      isActive = true;
      time = 0;

      inAir = !player.IsGrounded || player.isJumping;
      acceleration = 1;
      player.canMove = false;
      player.verticalVelocity = 0;
      Direction = player.transform.forward.normalized;
      player.canDash = false;

      Character.Spawn("dash_dust", player.transform, player.transform);

      if (inAir)
      {
        velocity = Direction * DashSpeedAir;
      }
      else
      {
        velocity = Direction * DashSpeed;
      }
    }

    public override void EndState()
    {
      base.EndState();
      player.canMove = true;
      player.verticalVelocity = 0f;
      isActive = false;
    }

    public override void TickState()
    {
      base.TickState();
      time += Time.deltaTime;
      var percent = time / DashTime;

      if (percent > 0.8)
      {
        acceleration = Mathf.Clamp(acceleration - Time.deltaTime * (inAir ? DashSpeed : DashSpeedAir), 0.1f, 1000f);
      }

      var speed = acceleration * velocity;
      Character.Rigidbody.velocity =  speed;

      if (time >= DashTime)
      {
        GoToNextState(player.StateMachine);
      }
    }


    private void OnDrawGizmos()
    {
      if (isActive)
      {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + transform.forward * PosOffset.z + transform.up * PosOffset.y, HitSize);
      }
    }
  }
}
