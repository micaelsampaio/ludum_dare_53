using System;
using Assets.Scripts.Core.Collisions;
using Core.Managers;
using Core.Pooling;
using UnityEngine;

namespace Scripts.Map
{
  public class SoulObject : MonoBehaviour
  {
    public string id;
    public bool ToCatch = false;

    private void Start()
    {
      if (ToCatch && GameManager.Instance.GameState.zoneState.IsSoulCatched(id))
      {
        gameObject.SetActive(false);
      }
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!Utils.IsPlayer(other)) return;

      var player = GameManager.Instance.Player;

      if (ToCatch)
      {
        GameManager.Instance.SoulsCatched.Add(id);
        gameObject.SetActive(false);
        SpawnCatchEffect("soul_tocatch_arc", () =>
        {
          player.PlayerSouls.AddSoulToDeliver(transform.position);
        });

        return;
      }

      if (player.CanAddSoul(1))
      {
        gameObject.SetActive(false);
        SpawnCatchEffect("soul_arc", () =>
        {
          player.AddSoul(1);
        });
      }
    }

    private void SpawnCatchEffect(string pool, Action Cb)
    {
      var player = GameManager.Instance.Player;
      var target = player.PlayerSouls.transform;
      var clone = PoolManager.Instance.GetPool(pool).Get<ArcProjectile>();
      clone.transform.position = transform.position;
      clone.transform.rotation = transform.rotation;
      var tempPos = transform.position;
      tempPos.y = target.position.y;
      var dir = (tempPos - target.position).normalized * 2f;
      var midpoint = (transform.position + target.position) / 2;
      var arcPos = midpoint + Vector3.up * 2 + dir;

      clone.Setup(player, player.PlayerSouls.transform, arcPos, Cb);
      clone.gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
      Gizmos.color = ToCatch ? Color.magenta : Color.cyan;
      Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 3f);
    }
  }
}
