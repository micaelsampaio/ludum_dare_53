using UnityEngine;

public class NavMeshRender : MonoBehaviour
{
  public Renderer Render;
  private void Awake()
  {
    Render.enabled = false;
  }
}