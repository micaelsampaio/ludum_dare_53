using UnityEngine;

namespace Core.Pooling
{
  public class DestroyAfterTime : MonoBehaviour
  {
    public float Maxtime = 1f;
    private float CurrentTime;

    private void OnEnable()
    {
      CurrentTime = 0f;
    }

    private void Update()
    {
      CurrentTime += Time.deltaTime;

      if (CurrentTime > Maxtime)
      {
        GetComponent<PoolObject>().Enqueue();
      }
    }
  }
}