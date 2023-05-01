using System;
using System.Collections;
using System.Collections.Generic;
using Core.Managers;
using Core.Pooling;
using Core.States;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

namespace Scripts.Player
{
  public class PlayerSouls : MonoBehaviour
  {
    public int souls;
    public int soulsToDeliver;

    private PlayerController player;
    [SerializeField] private float sizePerSoul = 0.1f;
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    [SerializeField] private float speed;
    [SerializeField] private float followSpeed;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float startMovingDistance;

    [SerializeField] private float material1;
    [SerializeField] private float material2;
    [SerializeField] private float material3;
    [SerializeField] private Renderer material4;
    [SerializeField] private Transform SizeObject;

    private STATE_NAME State;

    private Coroutine SetSizeTask;

    private PoolObjects SoulsToDeliverPool;
    public List<Transform> SoulsToDeliver;
    public float angle;
    public float radius = 1f;

    public event IntegerEvent OnUpdateSoulsToDeliver;

    private void Awake()
    {
      player = GameManager.Instance.Player;
      player.OnUpdateSouls += SetSouls;

      var targetPosition = player.transform.position + Vector3.up * 2f - player.transform.forward + player.transform.right;
      transform.position = targetPosition;
      transform.rotation = Utils.LookAt(transform, player.transform);

      SoulsToDeliverPool = PoolManager.Instance.GetPool("player_soul_to_deliver");
      SoulsToDeliver = new List<Transform>();

      var currentSoulsToDeliver = GameManager.Instance.GameState.playerState.soulsToDeliver;
      var currentSouls = GameManager.Instance.GameState.playerState.hp;
      CreateDEfaultSouls(currentSouls);
      CreateDEfaultSoulsToDeliver(currentSoulsToDeliver);

      StartIdle();
    }

    private void Start()
    {
      OnUpdateSoulsToDeliver?.Invoke(SoulsToDeliver.Count);
      Debug.Log("INvoke " + SoulsToDeliver.Count);
    }

    private void Update()
    {
      var targetPosition = player.transform.position + Vector3.up * 2f - player.transform.forward + player.transform.right;
      var dist = Vector3.Distance(transform.position, targetPosition);

      switch (State)
      {
        case STATE_NAME.IDLE:
          Idle(dist);
          break;
        case STATE_NAME.FOLLOW_TARGET:
          FollowTarget(dist, targetPosition);
          break;
      }

      UpdateSouls();
    }

    private void UpdateSouls()
    {

      angle += speed * Time.deltaTime;

      for (int i = 0; i < SoulsToDeliver.Count; ++i)
      {
        Transform transformToRotate = SoulsToDeliver[i];

        float x = Mathf.Sin(angle + ((2 * Mathf.PI) / SoulsToDeliver.Count) * i) * radius;
        float y = 0f;
        float z = Mathf.Cos(angle + ((2 * Mathf.PI) / SoulsToDeliver.Count) * i) * radius;

        transformToRotate.position = transform.position + new Vector3(x, y, z);
      }
    }

    private void StartIdle()
    {
      State = STATE_NAME.IDLE;
    }

    private void Idle(float dist)
    {
      if (dist > startMovingDistance)
      {
        StartFollowTarget();
      }

      transform.rotation = Utils.LookAt(transform, player.transform, Time.deltaTime * 10f);
    }

    private void StartFollowTarget()
    {
      followSpeed = 1;
      State = STATE_NAME.FOLLOW_TARGET;
    }

    private void FollowTarget(float dist, Vector3 targetPosition)
    {
      if (dist < minDistance)
      {
        StartIdle();
        return;
      }

      if (dist > maxDistance) followSpeed += Time.deltaTime * speed * 4;

      transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * followSpeed * Time.deltaTime);
      transform.rotation = Utils.LookAt(transform, targetPosition, Time.deltaTime * 20f);
    }


    private void SetSouls(int souls)
    {
      this.souls = souls;
      SetSize(souls);
    }

    private void SetSize(int nextSouls)
    {
      if (SetSizeTask != null) StopCoroutine(SetSizeTask);
      SetSizeTask = StartCoroutine(SetSizeTaskWorker(nextSouls));
    }

    private IEnumerator SetSizeTaskWorker(int nextSouls)
    {
      var size = SizeObject.localScale;
      var targetSize = Vector3.one * Mathf.Clamp(nextSouls * sizePerSoul, minSize, maxSize);
      var time = .0f;
      var maxtime = 0.25f;

      while (time < maxtime)
      {
        time += Time.deltaTime;
        SizeObject.localScale = Vector3.Lerp(size, targetSize, time / maxtime);
        yield return null;
      }

      UpdateMaterial(nextSouls);
      SizeObject.localScale = targetSize;
      SizeObject.localScale = targetSize;
    }

    private void UpdateMaterial(int souls)
    {
    }

    public void CreateDEfaultSoulsToDeliver(int souls)
    {
      for (var i = 0; i < souls; ++i)
      {
        var soul = SoulsToDeliverPool.Get();
        SoulsToDeliver.Add(soul.transform);
        soul.SetActive(true);
      }
    }

    public void CreateDEfaultSouls(int souls)
    {
      var targetSize = Vector3.one * Mathf.Clamp(souls * sizePerSoul, minSize, maxSize);
      SizeObject.localScale = targetSize;
    }

    public void AddSoulToDeliver(Vector3 source)
    {
      // TODO
      //++soulsToDeliver;
      var soul = SoulsToDeliverPool.Get();
      SoulsToDeliver.Add(soul.transform);
      soul.SetActive(true);

      GameManager.Instance.GameState.playerState.soulsToDeliver = SoulsToDeliver.Count;
      OnUpdateSoulsToDeliver?.Invoke(SoulsToDeliver.Count);
    }

    private IEnumerator SpawnSoulToDeliver()
    {
      yield return null;
    }

    private void OnDrawGizmos()
    {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, minDistance);
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, maxDistance);
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(transform.position, startMovingDistance);
    }

    public int RemoveSoulToDeliver()
    {

      if (SoulsToDeliver.Count > 0)
      {
        var index = SoulsToDeliver.Count - 1;
        var soul = SoulsToDeliver[index];
        SoulsToDeliver.RemoveAt(index);
        SoulsToDeliverPool.Enqueue(soul.gameObject);
        GameManager.Instance.GameState.playerState.soulsToDeliver = SoulsToDeliver.Count;
        OnUpdateSoulsToDeliver?.Invoke(SoulsToDeliver.Count);
        return 1;
      }
      return -1;
    }
  }
}
