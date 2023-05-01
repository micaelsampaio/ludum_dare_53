using Core.States;
using Game.Characters;
using UnityEngine;

namespace Scripts.Enemies.States
{
  public class AiMovePointsState : State
  {
    public Transform[] Points;
    public bool loop;
    public int currentPoint;

    public override void CreateState(Character character)
    {
      base.CreateState(character);

      Points[0].parent.gameObject.SetActive(false);
      Points[0].parent.SetParent(null);
    }

    public override void InitState()
    {
      base.InitState();

      SetNearPoint(Character.transform.position);
    }

    public override void TickState()
    {
      base.TickState();
      var targetPoint = Points[currentPoint % Points.Length].position;
      var targetPosition = Vector3.MoveTowards(transform.position, targetPoint, Character.Speed * Time.deltaTime);
      var dist = Vector3.Distance(targetPoint, targetPosition);

      transform.position = targetPosition;
      transform.rotation = Utils.LookAt(transform, targetPoint, Time.deltaTime * 10f);

      Debug.DrawLine(transform.position, targetPoint, Color.blue);

      if (dist < 0.1f)
      {
        ++currentPoint;
      }
    }

    public void SetNearPoint(Vector3 position)
    {
      var dist = float.MaxValue;
      for (var i = 0; i < Points.Length; ++i)
      {
        var tempDist = Vector3.Distance(Points[i].position, position);
        if (tempDist < dist)
        {
          dist = tempDist;
          currentPoint = i;
        }
      }
    }

    private void OnDrawGizmos()
    {
      if (Points.Length > 1)
      {
        Gizmos.color = Color.blue;
        for (var i = 0; i < Points.Length - 1; ++i)
        {
          Gizmos.DrawLine(Points[i].position + Vector3.up, Points[i + 1].position + Vector3.up);
        }
      }
    }

  }
}
