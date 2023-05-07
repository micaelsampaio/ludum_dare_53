using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core.Tips;
using Core.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Core.Managers
{
  public class TipManager : MonoBehaviour
  {
    public static TipManager Instance;
    private Tip CurrentTip;
    public Canvas Canvas;

    private void Awake()
    {
      Instance = this;
      gameObject.SetActive(false);
    }

    public void ShowTip(Tip tip)
    {
      if (CurrentTip && tip) CurrentTip.Close();

      CurrentTip = tip;
      CurrentTip.Show();
      gameObject.SetActive(true);
      GameManager.Instance.InputActions.Player.CloseUi.started -= CloseTipKeyDown;
      GameManager.Instance.InputActions.Player.CloseUi.started += CloseTipKeyDown;
    }

    private void CloseTipKeyDown(InputAction.CallbackContext obj)
    {
      CurrentTip.NextTip();
      GameManager.Instance.InputActions.Player.CloseUi.started -= CloseTipKeyDown;
    }

    public void CloseTip()
    {
      gameObject.SetActive(false);
    }
    private void OnDisable()
    {
      GameManager.Instance.InputActions.Player.CloseUi.started -= CloseTipKeyDown;
    }

  }
}
