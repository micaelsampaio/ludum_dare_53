using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core.Managers
{
  // CREDITS
  //https://www.youtube.com/watch?v=WTknIOiei5c&list=PLDcPimbLEWH_w_l05-0Yt25qP2pjqv9Z4
  //https://www.youtube.com/watch?v=1CoejLmjIjE&list=PLksqc7Yl0JqdBRfes77e-zaMiaUyVH2sM&index=3
  public class AudioManager : MonoBehaviour
  {
    private void Start()
    {
      var components = FindObjectsOfType<AudioManager>().Length;
      if (components > 1) DestroyImmediate(gameObject);
      else
      {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
      }
    }
  }
}
