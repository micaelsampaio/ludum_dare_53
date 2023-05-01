using UnityEngine;
namespace Core.Pooling
{
  public class PoolObject : MonoBehaviour, I_PoolObject
  {
    public PoolObjects Pool;

    public void Load(PoolObjects parent)
    {
      Pool = parent;
    }
    public void Enqueue() => Disable();
    public void Disable()
    {
      if (Pool != null)
      {
        Pool.Enqueue(gameObject);
      }
      else
      {
        GameObject.Destroy(gameObject);
        Debug.LogError("PoolObject -> " + gameObject + " has no parent pool");
      }
    }
  }

  public interface I_PoolObject
  {
    Transform transform { get; }
    void Load(PoolObjects parent);
    void Disable();
  }
}