using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Managers;
using Game.Characters;
using UnityEngine;

namespace Assets.Scripts.Characters
{

  public class CharacterHud : MonoBehaviour
  {
    public List<GameObject> Hps;
    public Character target;
    public Transform cam;

    private void Awake()
    {
      cam = GameManager.Instance.MainCamera.transform;
      transform.SetParent(null);
    }

    private void OnEnable()
    {
      transform.rotation = Quaternion.Inverse(cam.transform.rotation);

      if (!target) Hide();
    }

    private void LateUpdate()
    {
      if (target)
      {
        transform.position = target.transform.position + Vector3.up * target.height + Vector3.up * 2f;
      }

      transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
    }

    public void Show(Character character)
    {
      if (target)
      {
        target.OnUpdateHp -= OnTargetUpdateHp;
        target.OnDeath -= OnTargetDied;
      }

      target = character;
      target.OnUpdateHp += OnTargetUpdateHp;
      target.OnDeath += OnTargetDied;

      UpdateHp(character.Hp, character.MaxHp);
      gameObject.SetActive(true);
    }

    public void Hide()
    {
      if (target)
      {
        target.OnUpdateHp -= OnTargetUpdateHp;
        target.OnDeath -= OnTargetDied;
      }
      target = null;
      gameObject.SetActive(false);
    }

    private void OnTargetDied(Character character)
    {
      Hide();
    }

    private void OnTargetUpdateHp(Character character)
    {
      UpdateHp(character.Hp, character.MaxHp);
    }

    public void UpdateHp(int hp, int maxHp)
    {
      var size = Hps.Count;
      var max = Math.Max(maxHp, size);

      for (int i = 0; i < max; ++i)
      {
        if (i >= size)
        {
          Hps.Add(Instantiate(Hps[0], Hps[0].transform.parent));
        }
        var _hp = i + 1;
        Hps[i].SetActive(maxHp >= _hp);
        Hps[i].transform.GetChild(0).gameObject.SetActive(hp >= _hp);
      }
    }
  }
}
