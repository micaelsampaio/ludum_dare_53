using System.Collections.Generic;
using Core.Managers;
using Core.Pooling;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Player
{
  public class GateKeeperSouls : MonoBehaviour
  {
    public int souls;
    public List<GateKeeperSoul> SoulsToUpdate;
    public Pool PoolSoulsData;
    public PoolObjects PoolSouls;

    public Transform Cage;
    public Transform CageParent;

    private void Start()
    {
      PoolSouls = PoolManager.Instance.RegisterPool(PoolSoulsData);
      SoulsToUpdate = new List<GateKeeperSoul>();

      souls = GameManager.Instance.GameState.SoulsDelivered;
    }

    public void AddSoul()
    {
      var clone = PoolSouls.Get();

      var velocity = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
      var soul = clone.transform;
      var soulParent = new GameObject("orbitParent").transform;
      soul.transform.SetParent(soulParent);
      soul.localScale = Vector3.one * 0.5f;
      soul.transform.localPosition = new Vector3(UnityEngine.Random.Range(0, 0.1f), UnityEngine.Random.Range(0, 0.1f), UnityEngine.Random.Range(0, 0.1f));

      soulParent.transform.SetParent(CageParent);
      soulParent.transform.localPosition = Vector3.zero;

      var soulToUpdate = new GateKeeperSoul()
      {
        soul = soulParent,
        velocity = velocity
      };

      SoulsToUpdate.Add(soulToUpdate);
      ++souls;
      GameManager.Instance.SetSoulsDelivered(souls);
      GameManager.Instance.GameUI.UpdateDeliveredSouls(souls);

      if (HasAllSouls())
      {
        GameManager.Instance.EndGame();
      }

      clone.SetActive(true);
    }

    public bool HasAllSouls() => souls >= GameManager.MAX_SOULS;

    private void Update()
    {
      var dt = Time.deltaTime;

      foreach (var soul in SoulsToUpdate) soul.Update(dt);
    }
  }

  public class GateKeeperSoul
  {
    public Transform soul;
    public Vector3 velocity;

    public void Update(float dt)
    {
      soul.Rotate(velocity * dt);
    }
  }
}
