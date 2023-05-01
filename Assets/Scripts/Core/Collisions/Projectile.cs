using Core.Pooling;
using Game.Characters;
using UnityEngine;

namespace Assets.Scripts.Core.Collisions
{
  public class Projectile : MonoBehaviour
  {
    public Character Parent;
    public Vector3 targetPosition;
    public Transform target;
    public Transform targetPosTransform;
    public Character targetCharacter;
    public LayerMask hitMask;
    public float speed;
    public bool onlyHitTarget;
    public float maxTimeAlive = -1;

    public int damage;
    public string spawnOnHitKey;

    public void Setup(Character Parent, Transform target, LayerMask hitMask)
    {
      this.Parent = Parent;
      this.target = target;
      this.hitMask = hitMask;

      if (target)
      {
        targetCharacter = target.gameObject.GetComponent<Character>();
      }
      else
      {
        targetCharacter = null;
      }
    }

    public void SetTargetPosition(Transform target)
    {
      targetPosTransform = target;
    }

    public void Start()
    {
      var tempTarget = targetPosTransform != null ? targetPosTransform : target;

      if (tempTarget)
      {
        Debug.DrawLine(transform.position, tempTarget.position);
        transform.rotation = Utils.LookAt(transform, tempTarget);
      }

      if (maxTimeAlive > 0) Invoke(nameof(DisableProjectile), maxTimeAlive);
    }

    public void Update()
    {
      var tempTarget = targetPosTransform != null ? targetPosTransform : target;

      targetPosition = tempTarget && tempTarget.gameObject.activeSelf ? tempTarget.position : targetPosition;

      transform.SetPositionAndRotation(
        Vector3.MoveTowards(transform.position, targetPosition,
        Time.deltaTime * speed), Utils.LookAt(transform, tempTarget, Time.deltaTime * 10f)
      );
    }

    private void DisableProjectile()
    {
      GetComponent<PoolObject>().Enqueue();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.transform == target && targetCharacter)
      {
        targetCharacter.OnDamage(damage);
        DisableProjectile();

        if (!string.IsNullOrEmpty(spawnOnHitKey))
        {
          Parent.Spawn(spawnOnHitKey, target, Parent.transform);
        }
      }
    }

    private void OnDisable()
    {
      CancelInvoke();
    }

  }
}
