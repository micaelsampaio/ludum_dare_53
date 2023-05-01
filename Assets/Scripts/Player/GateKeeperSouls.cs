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
      var soulTransform = GameManager.Instance.Player.PlayerSouls.transform;
      soul.transform.position = soulTransform.position;

      var soulToUpdate = new GateKeeperSoul()
      {
        soul = soul,
        velocity = velocity,
        startPosition = soul.position,
        child = soul.GetChild(0),
        parent = CageParent
      };

      SoulsToUpdate.Add(soulToUpdate);
      ++souls;
      GameManager.Instance.GameState.SoulsDelivered = souls;
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
    public Transform parent;
    public Vector3 startPosition;
    public Vector3 velocity;
    public Transform child;
    public int state = 0;
    public float time = 0;

    public void Update(float dt)
    {

      if (state == 0)
      {
        time += dt;

        soul.position = Vector3.Lerp(startPosition, parent.position, time / 0.5f);

        if (time > 1)
        {
          soul.position = parent.position;
          child.localPosition = soul.position;
          soul.SetParent(parent);
          state = 2;
        }
        return;
      }
      else
      {
        soul.Rotate(velocity * dt);
      }

      var cam = GameManager.Instance.MainCamera.transform;
      var localCameraPos = soul.InverseTransformPoint(cam.transform.position);
      child.LookAt(soul.position + localCameraPos, soul.parent.up);
    }
  }
}
