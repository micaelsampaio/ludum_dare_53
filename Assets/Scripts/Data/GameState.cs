using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
  [Serializable]
  public class GameState
  {
    public PlayerState playerState;
    public int SoulsDelivered = 0;
    public string lastZone;

    [NonSerialized] public ZoneState zoneState;

    public static GameState Create()
    {
      var gameState = new GameState()
      {
        playerState = new PlayerState()
        {
          hp = 1
        }
      };
      return gameState;
    }

    public void Save() {
      DataManager.SetGameState(this);      
    }
  }
}
