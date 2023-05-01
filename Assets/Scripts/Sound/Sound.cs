
using System;
using UnityEngine;

[Serializable]
public class Sound
{
  public AudioClip audioClip;
  public string name;
  [Range(0, 1)]
  public float volume = 0.5f;
  public float cooldown;
}