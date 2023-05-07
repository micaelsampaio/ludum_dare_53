using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.Managers;
using Core.Managers;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Core.Tips
{
  public class Tip : MonoBehaviour
  {
    public string id;
    public GameObject[] Tips;
    public int currentTip = 0;

    private void Start()
    {
      //GameManager.Instance.InputActions
    }

    public void Show()
    {
      this.gameObject.SetActive(true);
      UnityEngine.Debug.Log(" SHOOOW  " + Tips[currentTip]);
      foreach (var tip in Tips) tip.SetActive(false);
      Tips[currentTip].SetActive(true);
      Tips[currentTip].transform.GetChild(0).localScale = Vector3.zero;
      Tips[currentTip].transform.GetChild(0).DOScale(1, 0.5f);
    }

    public void Close()
    {
      UnityEngine.Debug.Log(" CLOSE  tips");
      currentTip = 0;
      this.gameObject.SetActive(false);
    }

    public void NextTip()
    {
      UnityEngine.Debug.Log(" NEXT TIP  click");
      if (++currentTip < Tips.Length)
      {
        Show();
      }
      else
      {
        TipManager.Instance.CloseTip();
      }
    }
  }
}
