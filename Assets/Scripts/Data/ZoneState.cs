using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Data
{
  [Serializable]
  public class ZoneState
  {
    public string name;

    [NonSerialized] public string CheckPointId;
    [NonSerialized] public GameSpot CheckPointSpot = GameSpot.None;
    [NonSerialized] public Vector3 CheckPointPosition;

    public List<string> SoulsCatched = new List<string>();
    public List<string> EnemiesKilled = new List<string>();

    public ZoneState(string name)
    {
      this.name = name;
    }

    public bool IsSoulCatched(string id)
    {
      return SoulsCatched.Contains(id);
    }

    public void MarkAsCatchedSoul(string id)
    {
      SoulsCatched.Add(id);
    }

    public void MarkAsDead(string id)
    {
      EnemiesKilled.Add(id);
    }

    public static ZoneState Create(string zone)
    {
      var zoneState = new ZoneState(zone);
      return zoneState;
    }
  }
}
