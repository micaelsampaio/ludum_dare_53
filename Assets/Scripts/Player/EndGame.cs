using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Player
{
  public class EndGame : MonoBehaviour
  {

    public PlayableDirector director;
    [SerializeField] private PlayableDirector Cutscene;


    public void Start()
    {
      Cutscene.stopped += (ctx) => SceneManager.LoadScene("Menu");
      Cutscene.Play();
    }
  }
}
