using Core.Managers;
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
        player.PlayerSouls.AddSoulToDeliver(transform.position);
        gameObject.SetActive(false);

        return;
      }


      if (player.CanAddSoul(1))
      {
        player.AddSoul(1);
        gameObject.SetActive(false);
      }
    }
    private void OnDrawGizmos()
    {
      Gizmos.color = ToCatch ? Color.magenta : Color.cyan;
      Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 3f);
    }
  }
}
