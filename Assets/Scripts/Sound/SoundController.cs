using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
  public AudioSource audioSource;
  public List<Sound> Sounds;
  private Dictionary<string, int> SoundsIndex;

  private void Awake()
  {
    SoundsIndex = new Dictionary<string, int>();

    for (var i = 0; i < Sounds.Count; ++i)
    {
      var sound = Sounds[i];
      SoundsIndex[sound.name] = i;
    }

    //audioSource.outputAudioMixerGroup = AudioController.Instance.GameGroup;
  }

  public void RegisterSound(Sound sound)
  {
    if (SoundsIndex == null) SoundsIndex = new Dictionary<string, int>();

    var index = Sounds.Count;
    Sounds.Add(sound);
    SoundsIndex[sound.name] = index;
  }

  public Sound GetSound(string clipName)
  {
    if (SoundsIndex.TryGetValue(clipName, out var index))
    {
      return Sounds[index % Sounds.Count];
    }

    return null;
  }

  public void PlaySound(string clipName)
  {

    if (string.IsNullOrEmpty(clipName)) return;

    if (clipName.IndexOf(",") > 0)
    {
      PlayRandomSound(clipName);
      return;
    }

    var sound = GetSound(clipName);
    //var canPlaySound = SoundCooldownManager.Instance.PlaySound(sound);

    //if (!canPlaySound) return;


    if (sound != null)
    {
      audioSource.PlayOneShot(sound.audioClip, sound.volume > 0 ? sound.volume : 0.5f);
      return;
    }

    string sounds = "";
    Sounds.ForEach(s => sounds += s.name + ", ");
    Debug.LogWarning($"XXXXXXX   ----> SOUND NOT FOUND \"{clipName}\" - {sounds}");
  }

  public void PlaySound(Sound sound)
  {
    if (sound != null)
    {
      audioSource.PlayOneShot(sound.audioClip, sound.volume > 0 ? sound.volume : 0.5f);
    }
  }

  public void PlaySound(Sound[] sounds)
  {
    if (sounds != null)
    {
      var sound = sounds[UnityEngine.Random.Range(0, sounds.Length)];
      audioSource.PlayOneShot(sound.audioClip, sound.volume > 0 ? sound.volume : 0.5f);
    }
  }

  public void PlayRandomSound(string clipName)
  {
    var names = clipName.Split(',');
    var name = names[UnityEngine.Random.Range(0, names.Length)];
    var sound = GetSound(name);
    //var canPlaySound = SoundCooldownManager.Instance.PlaySound(GetSound(names[0]));

    //if (!canPlaySound) return;

    if (sound != null)
    {
      audioSource.PlayOneShot(sound.audioClip, sound.volume > 0 ? sound.volume : 0.5f);
    }
    else
    {
      Debug.LogWarning($"XXXXXXX   ----> RANDOM SOUND NOT FOUND \"{name}\"");
    }
  }
}
