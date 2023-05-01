using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Characters;
using Characters;
using Core.Pooling;
using Scripts.Player;
using UnityEngine;

namespace Game.Characters
{
  public class Character : MonoBehaviour
  {
    [Header("[CHARACTER LOADING DATA]")]
    public CharacterData LoadingData;

    [Header("[CHARACTER]")]
    public CharacterType Type;
    public bool IsAlive;
    public int Hp;
    public float Speed;
    public int Damage = 1;
    public bool IsGrounded = true;

    [Header("[CORE COMPONENTS]")]
    public Animator Animator;
    public Rigidbody Rigidbody;
    public CharacterController CharacterController;
    public SoundController SoundController;

    public CharacterSocket[] CharacterSockets;
    protected LayerMask groundLayer;

    public event CharacterEvent OnDeath;

    private bool Loaded = false;

    public virtual void Awake()
    {
      LoadingData.Load(gameObject);
      groundLayer = ~(1 << gameObject.layer);
      IsAlive = Hp > 0;
    }

    public virtual void Start()
    {
      lastPos = transform.position;
      Loaded = true;
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

    protected void GroundedCheck()
    {
      IsGrounded = Physics.CheckSphere(transform.position, 0.2f, groundLayer, QueryTriggerInteraction.Ignore);
    }

    Vector3 lastPos;

    private void OnDrawGizmos()
    {
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(transform.position, 0.2f);

      Debug.DrawLine(transform.position + Vector3.up, lastPos + Vector3.up, Color.red, 10f);
      lastPos = transform.position;
    }
    public void OnDamage(int dmg)
    {
      Debug.Log("DAMAGE");

      Hp -= dmg;

      if (Hp <= 0)
      {
        Hp = 0;
        IsAlive = false;
        // TODO EVENTS
        OnDeath?.Invoke(this);
      }
    }

    public GameObject Spawn(string key, Transform target, Transform parent, bool active = true)
    {
      if (!Loaded) return null;

      var data = LoadingData.GetSpawnData(key);

      var clone = LoadingData.Spawn<GameObject>(key, target, parent);
      if (clone) clone.SetActive(true);
      if (!string.IsNullOrEmpty(data.spawnSound) && SoundController != null) SoundController.PlaySound(data.spawnSound);
      if (clone && !string.IsNullOrEmpty(data.spawnEffect))
      {
        var clone2 = LoadingData.Spawn<GameObject>(data.spawnEffect, clone.transform, parent.transform);
        if (clone2) clone2.SetActive(true);
      }
      if (active) clone.SetActive(true);
      return clone;
    }

    public Transform GetSocket(CharacterSocketType type)
    {
      for (var i = 0; i < CharacterSockets.Length; ++i)
      {
        if (CharacterSockets[i].type == type) return CharacterSockets[i].socket;
      }

      return transform;
    }

    public virtual bool IsTargetable()
    {
      return IsAlive;
    }
  }

  public delegate void CharacterEvent(Character character);
  public delegate void PlayerEvent(PlayerController player);

}