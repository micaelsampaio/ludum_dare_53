using UnityEngine;

namespace Core.Pooling
{
  [CreateAssetMenu(fileName = "PoolObjectScritable", menuName = "Game/PoolObject", order = 0)]
  public class PoolObjectScritable : ScriptableObject
  {
    public PoolObjects Pool;
    public string Key;
    public GameObject Prefab;
    public int amount = 10;
    public bool increment = false;

    public PoolObjects Register()
    {
      var poolInfo = new Pool()
      {
        key = Key,
        prefab = Prefab,
        amount = amount,
        increment = increment
      };

      Pool = PoolManager.Instance.RegisterPool(poolInfo);
      return Pool;
    }
  }
}