using System;
using Core.Pooling;
using Game.Characters;
using UnityEngine;

namespace Assets.Scripts.Core.Collisions
{
  public class ArcProjectile : MonoBehaviour
  {
    public Character Parent;
    public Vector3 targetPosition;
    public Transform target;
    public float timeAlive;

    public int damage;
    public string spawnOnHitKey;

    public Action OnHitCb;
    public float currentTime;
    public Vector3 arcPos;

    private Vector3 startPosition;

    public void Setup(Character Parent, Transform target, Vector3 arcPos, Action OnHitCb)
    {
      this.Parent = Parent;
      this.target = target;
      this.arcPos = arcPos;
      this.OnHitCb = OnHitCb;
    }
    bool _enabled = false;
    public void OnEnable()
    {
      if (!_enabled)
      {
        _enabled = true;
        return;
      }

      startPosition = transform.position;
      currentTime = 0;
      //if (tempTarget)
      //{
      //  Debug.DrawLine(transform.position, tempTarget.position);
      //  transform.rotation = Utils.LookAt(transform, tempTarget);
      //}
      var size = 0.5f;
      for (var i = 0; i <= 100; i += 10)
      {
        var p = CalculateParabolicPath(startPosition, target.position, arcPos, i / 100);

        Debug.DrawLine(p + Vector3.right * size, p + Vector3.right * -size, Color.blue, 10f);
        Debug.DrawLine(p + Vector3.up * size, p + Vector3.up * -size, Color.blue, 10f);
      }

    }

    public void Update()
    {
      currentTime += Time.deltaTime;

      var elapsedTime = currentTime / timeAlive;
      var pos = CalculateParabolicPath(startPosition, target.position, arcPos, elapsedTime);

      transform.position = pos;

      if (elapsedTime > 1)
      {
        SpawnEffect(target);
        OnHitCb?.Invoke();
        DisableProjectile();
      }

    }

    private void DisableProjectile()
    {
      GetComponent<PoolObject>().Enqueue();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //  if (other.transform == target && targetCharacter)
    //  {
    //    targetCharacter.OnDamage(damage);
    //    SpawnEffect(targetCharacter.transform);
    //    DisableProjectile();

    //    if (!string.IsNullOrEmpty(spawnOnHitKey))
    //    {
    //      Parent.Spawn(spawnOnHitKey, target, Parent.transform);
    //    }
    //  }
    //}

    private void SpawnEffect(Transform targetCharacter)
    {
      if (!Parent) return;
      if (!string.IsNullOrEmpty(spawnOnHitKey)) Parent.Spawn(spawnOnHitKey, targetCharacter, Parent.transform);
    }

    private void OnDisable()
    {
      CancelInvoke();
    }

    public static Vector3 CalculateParabolicPath(Vector3 pointA, Vector3 pointB, Vector3 pointC, float t)
    {
      Vector3 p = (1 - t) * (1 - t) * pointA + 2 * (1 - t) * t * pointC + t * t * pointB;
      return p;
    }

  }
}
