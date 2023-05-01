using Core.Managers;
using UnityEngine;

namespace Assets.Scripts.Map
{
  public class PlatformCheckPoint : MonoBehaviour
  {
    public string id;
    public GameSpot spotType;

    public void OnHitPlayer()
    {
      var player = GameManager.Instance.Player;
      player.SetCheckPoint(id, spotType);
    }
  }
}
