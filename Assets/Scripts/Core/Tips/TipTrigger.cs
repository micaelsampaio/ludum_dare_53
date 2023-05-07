using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.Managers;
using UnityEngine;

namespace Assets.Scripts.Core.Tips
{
  public class TipTrigger : MonoBehaviour
  {
    public Tip tip;

    private void Start()
    {
      if (TryGetComponent<Renderer>(out var renderer)) Destroy(renderer);

      var parent = TipManager.Instance.Canvas.transform;
      tip = Instantiate(tip, parent);
      tip.Close();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (Utils.IsPlayer(other))
      {
        TipManager.Instance.ShowTip(tip);
      }
    }
  }
}
