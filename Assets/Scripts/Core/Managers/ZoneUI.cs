using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Core.Managers
{
  public class ZoneUI : MonoBehaviour
  {
    public GameObject ZoneNameView;
    public CanvasGroup ZoneNameViewGroup;
    public TextMeshProUGUI ZoneNameTxt;
    public static Dictionary<string, string> ZoneNames = new Dictionary<string, string>() {
        { "ZoneA", "Zone A"},
        { "ZoneB", "Zone B"},
        { "ZoneC", "Zone C"},
        { "ZoneD", "Zone D"},
        { "ZoneTutorial", "Tutorial"},
      };

    private void Start()
    {


      ZoneNameTxt.text = ZoneNames[SceneManager.GetActiveScene().name];

      StartCoroutine(ShowOff());
    }

    private IEnumerator ShowOff()
    {
      var group = ZoneNameViewGroup;
      var textMeshColor = ZoneNameTxt.color;

      yield return null;

      group.alpha = 0;
      textMeshColor.a = 0;
      StartCoroutine(FadeTextAlpha(ZoneNameTxt, 1, 0.5f));
      StartCoroutine(FadeCanvasGroup(group, 1, 0.5f));

      yield return new WaitForSeconds(3.5f);

      StartCoroutine(FadeTextAlpha(ZoneNameTxt, 0, 0.5f));
      StartCoroutine(FadeCanvasGroup(group, 0, 0.5f));

      yield return new WaitForSeconds(1f);

      Destroy(gameObject);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float alpha, float duration = 1f)
    {
      var time = 0f;
      var startAlpha = group.alpha;

      do
      {
        time += Time.deltaTime;
        group.alpha = Mathf.Lerp(startAlpha, alpha, time);
        yield return null;
      } while (time < duration);
    }

    private IEnumerator FadeTextAlpha(TextMeshProUGUI textMesh, float alpha, float fadeDuration = 0.5f)
    {
      float elapsedTime = 0f;
      Color startColor = textMesh.color;
      Color endColor = new Color(startColor.r, startColor.g, startColor.b, alpha);

      do
      {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / fadeDuration;
        textMesh.color = Color.Lerp(startColor, endColor, t);
        yield return null;
      } while (elapsedTime < fadeDuration);

      textMesh.color = endColor;
    }
  }
}
