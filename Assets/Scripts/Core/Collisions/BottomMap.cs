using Core.Managers;
using UnityEngine;

namespace Core.Collisions
{
  public class BottomMap : MonoBehaviour
  {
    Transform player;

    private void Start()
    {
      player = GameManager.Instance.Player.transform;
      Destroy(GetComponent<Renderer>());
    }

    private void LateUpdate()
    {
      var pos = player.position;
      pos.y = transform.position.y;
      transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
      if (Utils.IsPlayer(other.gameObject))
      {
        GameManager.Instance.OnPlayerFall();
      }
    }
  }
}
