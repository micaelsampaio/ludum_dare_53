using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Characters;
using Characters;
using Core.Managers;
using Core.Pooling;
using Scripts.Player;
using UnityEngine;

namespace Game.Characters
{
  public class Character : MonoBehaviour
  {
    [Header("[CHARACTER LOADING DATA]")]
    public string id;
    public CharacterData LoadingData;

    [Header("[CHARACTER]")]
    public CharacterType Type;
    public bool IsAlive;
    public int Hp;
    public int MaxHp;
    public float Speed;
    public int Damage = 1;
    public bool IsGrounded = true;
    public float radius;
    public float height;

    [Header("[CORE COMPONENTS]")]
    public Animator Animator;
    public Rigidbody Rigidbody;
    public SoundController SoundController;

    public CharacterSocket[] CharacterSockets;
    protected LayerMask groundLayer;

    public event CharacterEvent OnDeath;
    public event CharacterEvent OnUpdateHp;

    private bool Loaded = false;

    public virtual void Awake()
    {
      LoadingData.Load(gameObject);
      groundLayer = ~(1 << gameObject.layer);      
      Hp = MaxHp;
      IsAlive = Hp > 0;
    }

    public virtual void Start()
    {
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
      IsGrounded = Physics.CheckSphere(transform.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore);
    }

    public void OnDamage(int dmg)
    {
      Debug.Log("DAMAGE");

      Hp -= dmg;

      OnUpdateHp?.Invoke(this);

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
      if (!string.IsNullOrEmpty(data.spawnSound) && SoundController == null) GameManager.Instance.SoundController.PlaySound(LoadingData.GetSound(data.spawnSound));
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