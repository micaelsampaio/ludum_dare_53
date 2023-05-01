using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Collisions;
using Core.Managers;
using Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Player
{
  public class GateKeepCollector : MonoBehaviour
  {
    public Animator Animator;
    [SerializeField] private OnTriggerEnterCallback colliderEvents;
    private int coroutineId;
    private PlayerController player;
    public GateKeeperSouls SoulsComponent;

    private void Start()
    {
      player = GameManager.Instance.Player;
      SetupCollisions();
    }

    private void SetupCollisions()
    {
      colliderEvents.OnTriggerEnterEvent += (collider) =>
      {
        if (Utils.IsPlayer(collider.gameObject))
        {
          StartCoroutine(GetSoulsTask(++coroutineId));
        }
      };

      colliderEvents.OnTriggerExitEvent += (collider) =>
      {
        if (Utils.IsPlayer(collider.gameObject))
        {
          ++coroutineId;
        }
      };
    }
    private IEnumerator GetSoulsTask(int id)
    {

      while (true)
      {

        if (id != coroutineId) yield break;

        if (player.PlayerSouls.RemoveSoulToDeliver() > 0)
        {
          Debug.Log("REMOVE SOUL");
          SoulsComponent.AddSoul();

          if (SoulsComponent.HasAllSouls())
          {
            Animator.SetTrigger("jump");
            yield break;
          }
          else
          {
            Animator.SetTrigger("talk" + UnityEngine.Random.Range(1, 4));
          }
        }

        yield return new WaitForSeconds(0.5f);

      }
    }


  }

}
