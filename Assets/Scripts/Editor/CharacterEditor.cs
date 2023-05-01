

using Core.Tags;
using Game.Characters;
using Unity.VisualScripting;
using UnityEditor;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
  private void OnEnable()
  {
    var script = target as Character;

    if (script.GetComponent<TagId>() == null)
    {
      var tag = script.AddComponent<TagId>();
      tag.id = TagId.GenerateShortUuid();
    }
  }
}
