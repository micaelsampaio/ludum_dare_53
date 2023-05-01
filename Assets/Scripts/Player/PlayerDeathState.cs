using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.States;
using Game.Characters;
using UnityEngine;

namespace Scripts.Player
{
  public class PlayerDeathState : State
  {
    private PlayerController player;

    public override void CreateState(Character character)
    {
      base.CreateState(character);
      player = character as PlayerController;
    }
    public override void InitState()
    {
      base.InitState();

      player.canMove = false;
      player.canDash = false;

      player.Animator.Play("death", 0, 0f);
    }
  }
}
