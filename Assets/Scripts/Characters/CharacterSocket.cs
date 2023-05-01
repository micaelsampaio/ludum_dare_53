using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Characters
{
  [Serializable]
  public class CharacterSocket
  {
    public CharacterSocketType type;
    public Transform socket;
  }
  [Serializable]
  public enum CharacterSocketType
  {
    NONE,
    TARGET,
    HEAD
  }
}
