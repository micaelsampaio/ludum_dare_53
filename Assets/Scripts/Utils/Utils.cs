using System;
using System.Collections;
using System.Collections.Generic;
using Core.Managers;
using Game.Characters;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{

  public const float LOOK_AT_IMMEDIATLY = 999999f;
  public const float GRAVITY = -50f;
  public const float MAX_GRAVITY = -50f;
  public const float MAX_GRAVITY_UP = 1000;

  public static LayerMask ENEMIES_LAYER = LayerMask.GetMask("enemy");
  public static LayerMask PLAYER_LAYER = LayerMask.GetMask("player");

  public static bool IsPlayer(Collider c)
  {
    return c.gameObject == GameManager.Instance.Player.gameObject;
  }

  public static bool IsPlayer(GameObject obj)
  {
    return obj == GameManager.Instance.Player.gameObject;
  }

  public static bool IsLayer(int layer, LayerMask layermask)
  {
    var result = layermask == (layermask | (1 << layer));
    return result;
  }

  public static Character GetCharacterFromObject(GameObject obj)
  {

    if (obj.TryGetComponent<Character>(out var character))
    {
      return character;
    }

    return null;
  }
  public static bool TryGetCharacter(GameObject obj, out Character character)
  {
    return obj.TryGetComponent<Character>(out character);
  }

  public static float ClampAngle(float lfAngle, float lfMin, float lfMax)
  {
    if (lfAngle < -360f) lfAngle += 360f;
    if (lfAngle > 360f) lfAngle -= 360f;
    return Mathf.Clamp(lfAngle, lfMin, lfMax);
  }

  public static Canvas FindParentCanvas(GameObject gameObject)
  {
    if (!gameObject.transform.parent) return null;

    if (gameObject.transform.parent.TryGetComponent<Canvas>(out var canvas))
    {
      return canvas;
    }

    return FindParentCanvas(gameObject.transform.parent.gameObject);
  }

  public static Vector3 getRandomPositionInRange(Transform transform, float range)
  {
    return transform.position + new Vector3(
      UnityEngine.Random.Range(-range, range),
      transform.position.y,
      UnityEngine.Random.Range(-range, range)
    );
  }
  public static Vector3 getRandomPositionInRange(Vector3 position, float range)
  {
    return position + new Vector3(
      UnityEngine.Random.Range(-range, range),
      position.y,
      UnityEngine.Random.Range(-range, range)
    );
  }

  public static Quaternion LookAt(Transform self, Transform target, float turnSpeed = LOOK_AT_IMMEDIATLY)
  {
    Vector3 targetDir = target.position - self.position;
    targetDir.y = 0;
    Vector3 newDir = targetDir * 20f;

    return Quaternion.LookRotation(Vector3.RotateTowards(self.forward, newDir, turnSpeed, 0f));
  }
  public static Quaternion LookAt(Transform self, Vector3 target, float turnSpeed = LOOK_AT_IMMEDIATLY)
  {
    Vector3 targetDir = target - self.position;
    targetDir.y = 0;
    Vector3 newDir = targetDir * 20f;

    return Quaternion.LookRotation(Vector3.RotateTowards(self.forward, newDir, turnSpeed, 0f));
  }
  public static Quaternion LookAt(Vector3 self, Vector3 target, float turnSpeed = LOOK_AT_IMMEDIATLY)
  {
    Vector3 targetDir = target - self;
    targetDir.y = 0;
    Vector3 newDir = targetDir * 20f;

    return Quaternion.LookRotation(Vector3.RotateTowards(self, newDir, turnSpeed, 0f));
  }

  public static Quaternion LookAtTransform(Transform self, Transform target)
  {
    Vector3 targetDir = target.position - self.position;
    Vector3 newDir = targetDir * 20f;

    return Quaternion.LookRotation(Vector3.RotateTowards(targetDir, newDir, 99999f, 0f));
  }
  public static Quaternion LookAtVector(Transform self, Vector3 target)
  {
    Vector3 targetDir = target - self.position;
    Vector3 newDir = targetDir * 20f;
    return Quaternion.LookRotation(newDir);
  }

  public static float GetDistance(Vector3 pos1, Vector3 pos2)
  {
    pos2.y = pos1.y;
    return Vector3.Distance(pos1, pos2);
  }

  public static Quaternion GetRandomRotation()
  {
    return Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
  }

  public static float GetDistance(GameObject pos1, GameObject pos2)
  {
    var pos = pos2.transform.position;
    pos.y = pos1.transform.position.y;
    return Vector3.Distance(pos1.transform.position, pos);
  }

  public static bool InternetNotAvailable() => Application.internetReachability == NetworkReachability.NotReachable;
  public static IEnumerator DelayAction(Action Cb, float time)
  {
    yield return new WaitForSeconds(time);
    Cb.Invoke();
  }

  public static IEnumerator WaitForEndAnimation(Animator animator)
  {
    while (true)
    {
      if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98) break;
      yield return null;
    }
    yield return null;
  }

  public static Transform RecursiveFindChild(Transform parent, string childName)
  {
    foreach (Transform child in parent)
    {
      if (child.name == childName)
      {
        return child;
      }
      else
      {
        Transform found = RecursiveFindChild(child, childName);
        if (found != null)
        {
          return found;
        }
      }
    }
    return null;
  }

  public static IEnumerator ScrollToTop(ScrollRect list)
  {
    yield return new WaitForEndOfFrame();
    list.gameObject.SetActive(true);
    list.verticalNormalizedPosition = 1f;
  }

  public static bool AnimatorHasParameter(Animator animator, string paramName)
  {
    foreach (AnimatorControllerParameter param in animator.parameters)
    {
      if (param.name == paramName)
        return true;
    }
    return false;
  }

  public static Vector3 GetRelativePosition(RectTransform p, Camera c, float DesireDistanceFromCamera = 5f)
  {
    return c.ScreenToWorldPoint(new Vector3(p.position.x, p.position.y, DesireDistanceFromCamera));
  }

  public static List<string> GetFpsList()
  {
    var fpsList = new List<string>();
    fpsList.Add("Default from system");
    fpsList.Add("24");
    fpsList.Add("30");
    fpsList.Add("60");
    fpsList.Add("120");
    fpsList.Add("Max (not recomended)");
    return fpsList;
  }

  public static int[] FPS = { -1, 24, 30, 60, 120, 900 };
  public static string[] LANGS = { "en", "pt" };
  public static string[] LANGS_DESCRIPTION = { "English", "Portguês" };

  public static int GetSelectedLang(string lang)
  {
    for (var i = 0; i < LANGS.Length; ++i)
    {
      if (LANGS[i] == lang) return i;
    }
    return 0;
  }
  public static string ParseLang(SystemLanguage lang)
  {
    return lang switch
    {
      SystemLanguage.English => "en",
      SystemLanguage.Portuguese => "pt",
      _ => "en"
    };
  }

  public static int GetFps(int selectedFps)
  {
    var fps = FPS[selectedFps];
    return fps;
  }

  public static int GetSelectedFps(int fps)
  {
    for (var i = 0; i < FPS.Length; ++i)
    {
      if (fps == FPS[i]) return i;
    }

    return 0;
  }
  public static float GetAudioValue(float value) => Mathf.Log10(value) * 20;

  public static GameObject FindPortalInScene(string currentPortal)
  {
    var portals = GameObject.FindObjectsOfType<ScenePortal>();

    foreach (var portal in portals)
    {
      if (portal.PortalId == currentPortal) return portal.gameObject;
    }
    return null;
  }

  private static Character[] GetTargets(Transform transform, Collider[] targetsCandidates)
  {
    List<Character> targets = new List<Character>();
    foreach (var target in targetsCandidates)
    {
      var newTarget = target.GetComponent<Character>();
      Debug.Log("TARGET ---> " + newTarget + " " + (newTarget != null ? newTarget.IsTargetable() : false));
      if (newTarget != null && newTarget.transform != transform && newTarget.IsTargetable() && !targets.Contains(newTarget))
      {
        targets.Add(newTarget);
      }
    }

    return targets.Count > 0 ? targets.ToArray() : new Character[] { };
  }

  public static Character[] GetTargetsInRange(Transform transform, int layer, float range)
  {
    var targetsCandidates = Physics.OverlapSphere(transform.position, range, layer);

    Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up * 3f, targetsCandidates.Length > 0 ? Color.green : Color.red, 10);
    Debug.DrawLine(transform.position + Vector3.up * 3f, transform.position + Vector3.up * 3f + transform.forward * 3f, targetsCandidates.Length > 0 ? Color.green : Color.red, 10);
    foreach (var target in targetsCandidates) Debug.DrawLine(transform.position + Vector3.up, target.transform.position, Color.red, 10f);

    if (targetsCandidates.Length > 0)
    {
      return GetTargets(transform, targetsCandidates);
    }

    return new Character[] { };
  }

  public static Character[] GetTargetsInRange(Transform transform, Vector3 position, int layer, float range)
  {
    var targetsCandidates = Physics.OverlapSphere(transform.position, range, layer);

    if (targetsCandidates.Length > 0)
    {
      return GetTargets(transform, targetsCandidates);
    }

    return new Character[] { };
  }

  public static Character GetFirstTargetInRange(Transform transform, int layer, float range)
  {
    var targetsCandidates = Physics.OverlapSphere(transform.position, range, layer);

    if (targetsCandidates.Length > 0)
    {
      foreach (var targetCandidate in targetsCandidates)
      {
        var newTarget = targetCandidate.GetComponent<Character>();
        if (newTarget != null && newTarget.transform != transform && newTarget.IsTargetable())
        {
          return newTarget;
        }
      }
    }

    return null;
  }

  public static Character GetTargetLookAt(Transform transform, int layer, float range, float angle = 60)
  {
    var targets = GetTargetsInRange(transform, layer, range);
    Character bestTarget = null;
    float closestDistanceSqr = Mathf.Infinity;
    Vector3 currentPosition = transform.position;

    foreach (var potentialTarget in targets)
    {
      Vector3 forward = transform.forward;
      Vector3 toOther = potentialTarget.transform.position - transform.position;
      var targetAngle = Vector3.Angle(transform.forward, toOther);
      if (angle > targetAngle)
      {
        Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
        float dSqrToTarget = directionToTarget.sqrMagnitude;
        if (dSqrToTarget < closestDistanceSqr)
        {
          closestDistanceSqr = dSqrToTarget;
          bestTarget = potentialTarget;
        }
      }
    }

    return bestTarget;
  }

  public static Character GetTargetLookAt(Transform transform, Transform transformToLook, List<Character> targets, float angle = 80)
  {

    Character bestTarget = null;
    float closestDistanceSqr = Mathf.Infinity;
    Vector3 currentPosition = transform.position;

    foreach (var potentialTarget in targets)
    {
      Vector3 toOther = potentialTarget.transform.position - transformToLook.position;
      var targetAngle = Vector3.Angle(transformToLook.forward, toOther);
      if (angle > targetAngle)
      {
        Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
        float dSqrToTarget = directionToTarget.sqrMagnitude;
        if (dSqrToTarget < closestDistanceSqr)
        {
          closestDistanceSqr = dSqrToTarget;
          bestTarget = potentialTarget;
        }
      }
    }

    return bestTarget;
  }
  public static Character GetBestTargetByDistance(Transform transform, List<Character> targets)
  {
    Character bestTarget = null;
    float closestDistanceSqr = Mathf.Infinity;
    Vector3 currentPosition = transform.position;

    foreach (var potentialTarget in targets)
    {
      Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
      float dSqrToTarget = directionToTarget.sqrMagnitude;
      if (dSqrToTarget < closestDistanceSqr)
      {
        closestDistanceSqr = dSqrToTarget;
        bestTarget = potentialTarget;
      }
    }

    return bestTarget;
  }
}