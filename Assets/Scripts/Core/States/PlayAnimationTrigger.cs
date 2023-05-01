using Core.States;
using Game.Characters;

public class PlayAnimationTrigger : State
{
  public string trigger;

  public override void CreateState(Character character)
  {
    base.CreateState(character);
  }

  public override void InitState()
  {
    Character.Animator.SetTrigger(this.trigger);
  }

  public override void EndState()
  {
    Character.Animator.ResetTrigger(this.trigger);
  }
}