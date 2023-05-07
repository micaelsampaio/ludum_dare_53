using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.Collisions;
using Core.Collisions;
using Core.Managers;
using Core.Pooling;
using Scripts.Player;
using TMPro;
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

    public CanvasGroup DropSoulsGroup;
    public TextMeshProUGUI DropSoulsGroupTip;

    private void Start()
    {
      player = GameManager.Instance.Player;
      SetupCollisions();

      StartCoroutine(AnimateDropSoulsGroup());
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

          SoulsComponent.AddSoul();

          SpawnCatchEffect("soul_tocatch_arc", () =>
          { });

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

    private IEnumerator AnimateDropSoulsGroup()
    {
      yield return null;
      var minAlpha = 0.2f;
      var accumulator = 1f;
      var time = 0f;
      var startAlpha = 1f;
      var endAlpha = minAlpha;
      var startColor = new Color(DropSoulsGroupTip.color.r, DropSoulsGroupTip.color.g, DropSoulsGroupTip.color.g, startAlpha);
      var endColor = new Color(DropSoulsGroupTip.color.r, DropSoulsGroupTip.color.g, DropSoulsGroupTip.color.g, endAlpha);

      while (true)

      {
        time += Time.deltaTime;

        DropSoulsGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time);
        DropSoulsGroupTip.color = Color.Lerp(startColor, endColor, time);

        if (time > 1)
        {
          time = 0f;
          accumulator *= -1;

          if (accumulator == 1)
          {
            startAlpha = minAlpha;
            endAlpha = 1;
          }
          else
          {
            startAlpha = 1;
            endAlpha = minAlpha;
          }

          startColor = new Color(DropSoulsGroupTip.color.r, DropSoulsGroupTip.color.g, DropSoulsGroupTip.color.g, startAlpha);
          endColor = new Color(DropSoulsGroupTip.color.r, DropSoulsGroupTip.color.g, DropSoulsGroupTip.color.g, endAlpha);
        }

        yield return null;

      }
    }

    private void SpawnCatchEffect(string pool, Action Cb)
    {
      var source = player.PlayerSouls.transform;
      var target = SoulsComponent.CageParent;
      var clone = PoolManager.Instance.GetPool(pool).Get<ArcProjectile>();

      clone.transform.position = source.position;
      clone.transform.rotation = source.rotation;

      var midpoint = (source.position + target.position) / 2;
      var arcPos = midpoint + Vector3.up * 2;

      clone.Setup(player, target, arcPos, Cb);

      clone.gameObject.SetActive(true);
    }
  }
}
