using UnityEngine;
using UnityEngine.Video;

public class VideoPanelController : MonoBehaviour
{
  [SerializeField]
  private GameObject videoPanelRoot;

  [SerializeField]
  private VideoPlayer videoPlayer;
  // reference to video
  void Start() { Hide(); }

  public void Show() { videoPanelRoot.SetActive(true); }

  public void Hide()
  {
    Debug.Log("Hiding video panel");
    videoPanelRoot.SetActive(false);
    videoPlayer.Stop();
  }

  public void PlayClip(VideoClip clip)
  {
    if (clip == null)
    {
      Debug.Log("Clip is null");
      return;
    }
    videoPlayer.clip = clip;
    Show();
    videoPlayer.Play();
  }

  public bool IsVisible() { return videoPanelRoot.activeSelf; }
}
