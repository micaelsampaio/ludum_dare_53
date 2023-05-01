using UnityEngine;
namespace Core.Collisions
{

  public class OnTriggerEnterCallback : MonoBehaviour
  {
    public GameObject IgnoreGameObject;
    public event ColliderEvent OnTriggerEnterEvent;
    public event ColliderEvent OnTriggerExitEvent;

    public Collider[] Colliders;

    public float EnableCollidersAfterTime = -1;

    private void OnEnable()
    {
      if (EnableCollidersAfterTime > 0)
      {
        DisableColliders();
        Invoke("EnableColliders", EnableCollidersAfterTime);
      }
    }

    private void OnDisable()
    {
      CancelInvoke();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (IgnoreGameObject == other.gameObject) return;
      OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
      if (IgnoreGameObject == other.gameObject) return;
      OnTriggerExitEvent?.Invoke(other);
    }

    public void EnableColliders()
    {
      foreach (var collider in Colliders)
      {
        collider.enabled = true;
      }
    }

    public void DisableColliders()
    {
      foreach (var collider in Colliders)
      {
        collider.enabled = false;
      }
    }
  }
}

public delegate void ColliderEvent(Collider other);