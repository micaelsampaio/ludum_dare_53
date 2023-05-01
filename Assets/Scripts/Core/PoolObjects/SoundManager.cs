using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Core.Managers;

namespace Core.Pooling
{
  public class SoundCooldownManager
  {
    public static SoundCooldownManager Instance;

    public Dictionary<string, float> Sounds;

    private Coroutine DeleteSoundsRoutine;

    private SoundCooldownManager() { }

    public SoundCooldownManager(GameManager gm)
    {
      Sounds = new Dictionary<string, float>();
      gm.StartCoroutine(DeleteSoundsTimer());
      Instance = this;
    }

    public bool PlaySound(Sound sound)
    {
      if (sound == null || sound.cooldown <= 0) return true;

      var time = GameManager.time;

      if (Sounds.TryGetValue(sound.name, out var soundCooldown))
      {
        if (soundCooldown > time) return false;
        Sounds[sound.name] = time + sound.cooldown;
      }
      else
      {
        Sounds[sound.name] = time + sound.cooldown;
      }
      return true;
    }

    private IEnumerator DeleteSoundsTimer()
    {
      while (true)
      {
        yield return new WaitForSeconds(5);
        DeleteSounds();
      }
    }

    private void DeleteSounds()
    {
      var keys = GetCurrentSounds();
      var time = GameManager.time;

      foreach (var key in keys)
      {
        if (time > Sounds[key])
        {
          Sounds.Remove(key);
        }
      }
    }

    private string[] GetCurrentSounds()
    {
      var keys = Sounds.Keys;
      var temp_keys = new string[keys.Count];
      keys.CopyTo(temp_keys, 0);
      return temp_keys;
    }
  }
}