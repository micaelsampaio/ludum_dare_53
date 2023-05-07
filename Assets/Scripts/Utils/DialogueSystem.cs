using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{

  public TextMeshProUGUI Text;
  public Color TextColor;
  public Canvas Canvas;

  private void Awake()
  {
    Canvas.gameObject.SetActive(false);
  }

  public void StartDialogue(List<DialogueData> data, Action cb)
  {
    Canvas.gameObject.SetActive(true);
    StartCoroutine(StartDialogueLoop(data, cb));
  }

  private IEnumerator StartDialogueLoop(List<DialogueData> data, Action cb)
  {
    int i = 0;

    while (i < data.Count)
    {
      var dialogue = data[i++];

      yield return DialogueDelay(dialogue.character, dialogue.text);

      yield return new WaitForSeconds(dialogue.time);
    }

    cb();
    Canvas.gameObject.SetActive(false);
  }

  private IEnumerator DialogueDelay(string character, string text)
  {
    Text.color = TextColor;
    var str = new StringBuilder();

    for (var i = 0; i < text.Length; ++i)
    {
      str.Append(text[i]);
      Text.text = $"{character}: {str.ToString()}";
      yield return new WaitForSeconds(0.02f);
    }
    str.Clear();
  }
}