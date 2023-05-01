using System.Collections.Generic;
using UnityEngine;

namespace Core.Pooling
{
  public class PoolManager
  {
    public static PoolManager Instance;
    public Dictionary<string, PoolObjects> Pools;

    public static void Clear()
    {
      Instance = null;
    }
    public static PoolManager Create()
    {
      return new PoolManager();
    }
    public PoolManager()
    {
      Instance = this;
      Pools = new Dictionary<string, PoolObjects>();
    }

    public PoolObjects RegisterPool(Pool p)
    {
      if (Pools.TryGetValue(p.key, out PoolObjects currentPool))
      {
        currentPool.Add(p);
        return currentPool;
      }
      else
      {
        Pools[p.key] = new PoolObjects(p);
        return Pools[p.key];
      }
    }

    public PoolObjects GetPool(Pool p)
    {
      Pools.TryGetValue(p.key, out PoolObjects nPool);
      return nPool;
    }

    public PoolObjects GetPool(string key)
    {
      Pools.TryGetValue(key, out PoolObjects nPool);
      return nPool;
    }

  }
}