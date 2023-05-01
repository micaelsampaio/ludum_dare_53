using UnityEngine;

namespace Core.Pooling
{
  public class DisableAfterTimeNoPool : MonoBehaviour
  {
    [Header("Dont use this with pool object")]
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
        gameObject.SetActive(false);
      }
    }
  }
}