using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Player
{
  public class StartGameCutscene : MonoBehaviour
  {
    //[SerializeField] private PlayableDirector Director;

    public TextAsset FileDialogue;
    public List<DialogueData> Dialogue1;
    public DialogueSystem DialogueSystem;

    [Header("Characters")]
    public Animator playerBoat;
    public Animator gateKeeperBoat;
    public Transform boat;
    public Transform boatTrail;

    public CanvasGroup canvasGroup;

    public void Start()
    {
      Dialogue1 = DialogueParser.ParseDialogueFile(FileDialogue.text);
      // Director.stopped += (ctx) => SceneManager.LoadScene("ZoneTutorial");
      //Director.Play();
    }


    public void StartDialogue()
    {
      DialogueSystem.StartDialogue(Dialogue1, () => Debug.Log("DONE"));
    }

    private void Update()
    {

    }

    public void StartFade()
    {
      StartCoroutine(FadeOut(canvasGroup));
    }

    public void NextLevel()
    {
      SceneManager.LoadScene("ZoneTutorial");
    }

    public IEnumerator FadeOut(CanvasGroup canvas)
    {
      var time = 0f;
      var maxTime = 2f;
      do
      {
        time += Time.deltaTime;
        canvas.alpha = Mathf.Lerp(0f, 1f, time / maxTime);
        yield return null;
      } while (time < maxTime);
    }
  }

}
