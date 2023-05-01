using System;
using System.Collections;
using UnityEngine;

public class AnimationUtils
{
  public static IEnumerator Shrink(Transform t, float scale, Action cb)
  {
    while (t.localScale.x > 0)
    {
      t.localScale -= Vector3.one * scale * Time.deltaTime;
      yield return null;
    }
    t.localScale = Vector3.zero;
    cb();
  }

  public static IEnumerator Grow(Transform t, float scale, Action cb)
  {
    while (t.localScale.x < 1)
    {
      t.localScale += Vector3.one * scale * Time.deltaTime;
      yield return null;
    }

    t.localScale = Vector3.one;
    cb();
  }
}