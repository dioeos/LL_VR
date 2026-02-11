using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "UI/Description Container")]
public class DescriptionContainer : ScriptableObject
{
  public Condition condition;
  public DataType dataType;
  public VideoClip previewVideo;

  [TextArea(6, 20)]
  public string text;
}
