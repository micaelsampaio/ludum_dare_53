using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

[Serializable]
public class DialogueData
{
  public string character;
  public string text;
  public float time;
}

public static class DialogueParser
{
  public static List<DialogueData> ParseDialogueFile(string fileContent)
  {
    var lines = fileContent.Split('\n');
    var dialogueLines = new List<DialogueData>();
    var currentLine = new DialogueData();

    for (var i = lines[0].Trim() == "---" ? 1 : 0; i < lines.Length; ++i)
    {
      var line = lines[i].Trim();
      UnityEngine.Debug.Log("line ." + line + ".");
      if (line.Contains(":"))
      {
        var key = line.Substring(0, line.Length - 1);
        var value = lines[++i].Trim();

        UnityEngine.Debug.Log("key " + key + " --> " + value);

        switch (key)
        {
          case "character":
            currentLine.character = value; break;
          case "text":
            currentLine.text = value; break;
          case "time":
            currentLine.time = float.Parse(value); break;
        }
      }

      if (line == "---")
      {
        dialogueLines.Add(currentLine);
        currentLine = new DialogueData();
      }
    }

    return dialogueLines;
  }
}
