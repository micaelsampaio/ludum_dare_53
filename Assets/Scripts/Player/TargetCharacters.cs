using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Characters;
using Core.Managers;
using Game.Characters;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace Assets.Scripts.Player
{
  public class TargetCharacters : MonoBehaviour
  {
    [SerializeField] private Transform Icon;

    public Character Parent;
    public Character Target;
    public SpriteRenderer targetCircle;
    public CharacterHud Hud;

    public List<Character> TargetCandidates = new List<Character>();

    private void Start()
    {
      transform.SetParent(null);
      Icon.SetParent(null);
      Icon.gameObject.SetActive(false);
      Hud.gameObject.SetActive(false);

      var col = targetCircle.color;
      col.a = 0;
      targetCircle.color = col;

      GameManager.Instance.OnPlayerDeathEvent += (c) =>
      {
        var col = targetCircle.color;
        col.a = 0;
        targetCircle.color = col;
      };
    }

    private void Update()
    {
      var target = RequestTarget();
      SetTarget(target);
    }

    public void LateUpdate()
    {
      transform.position = Parent.transform.position;

      if (Target)
      {
        Icon.transform.position = Target.transform.position;
        Icon.transform.rotation = Utils.LookAt(Icon.transform, Parent.transform);
      }
    }

    public void SetTarget(Character newTarget)
    {
      if (newTarget == Target)
      {
        return;
      }

      if (Target) Target.OnDeath -= OnTargetDeath;

      if (newTarget == null)
      {
        Target = null;
        Icon.gameObject.SetActive(false);
        Hud.Hide();
        return;
      }

      if (!newTarget.IsAlive)
      {
        Target = null;
        newTarget.OnDeath -= OnTargetDeath;
        Icon.gameObject.SetActive(false);
        Hud.Hide();
        return;
      }

      Icon.gameObject.SetActive(true);
      Target = newTarget;
      Target.OnDeath += OnTargetDeath;
      Hud.Show(Target);
    }

    public Character RequestTarget()
    {
      var target = Utils.GetTargetLookAt(Parent.transform, GameManager.Instance.MainCamera.transform, TargetCandidates);

      foreach (var t in TargetCandidates)
      {
        Debug.DrawLine(Parent.transform.position + Vector3.up, t.transform.position + Vector3.up, t == target ? Color.green : Color.red);
      }

      SetTarget(target);

      return target;
    }

    public void OnDisable()
    {
      if (Target)
      {
        Target.OnDeath -= OnTargetDeath;
      }
    }


    public void OnTargetDeath(Character character)
    {
      character.OnDeath -= OnTargetDeath;

      Target = null;
      Icon.gameObject.SetActive(false);
      RemoveTarget(character);
      GetNewTarget();
    }

    private void GetNewTarget()
    {
      var target = RequestTarget();
      SetTarget(target);
    }

    public void AddTarget(Character character)
    {
      if (TargetCandidates.Contains(character) || !character.IsAlive || !character.gameObject.activeSelf) return;

      TargetCandidates.Add(character);

      StartCoroutine(TargetCircleVisibilityTaskfun(0.05f));
    }

    public void RemoveTarget(Character character)
    {
      TargetCandidates.Remove(character);

      Debug.Log("TargetCandidates.Count " + TargetCandidates.Count);

      if (TargetCandidates.Count == 0)
      {
        StartCoroutine(TargetCircleVisibilityTaskfun(0));
      }
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject == Parent.gameObject) return;

      if (Utils.TryGetCharacter(other.gameObject, out var character))
      {
        AddTarget(character);
        GetNewTarget();
      }
    }

    private void OnTriggerExit(Collider other)
    {
      if (other.gameObject == Parent.gameObject) return;

      if (Utils.TryGetCharacter(other.gameObject, out var character))
      {
        RemoveTarget(character);
        GetNewTarget();
      }
    }

    private IEnumerator TargetCircleVisibilityTaskfun(float alpha)
    {
      var time = 0f;
      var startAlpha = targetCircle.color;
      var endAlpha = targetCircle.color;
      endAlpha.a = alpha;

      do
      {
        time += Time.deltaTime * 4f;
        targetCircle.color = Color.Lerp(startAlpha, endAlpha, time);
        yield return null;
      } while (time < 1);
    }
  }
}
