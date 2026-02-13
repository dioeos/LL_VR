using UnityEngine;

public abstract class DataTypeControllerBase : MonoBehaviour
{

  // variables only accessible through inheritors
  [SerializeField]
  protected OptionsStore optionsStore;

  [SerializeField]
  protected DataType datatype;

  protected GameObject[] workspaceAreas;
  protected GameObject[] conferenceAreas;
  protected GameObject[] commonAreas;

  private string workspaceTag = "WorkspaceAreas";
  private string conferenceTag = "ConferenceAreas";
  private string commonTag = "CommonAreas";

  protected void Awake()
  {
    // fetch all areas and store into variables via tags
    workspaceAreas = GameObject.FindGameObjectsWithTag(workspaceTag);
    conferenceAreas = GameObject.FindGameObjectsWithTag(conferenceTag);
    commonAreas = GameObject.FindGameObjectsWithTag(commonTag);
  }

  protected void OnEnable()
  {
    // add listener
    optionsStore.OptionsChanged += OnOptionsChanged;
  }

  protected void OnOptionsChanged(DataType dt, AreaOptions opts)
  {
    if (dt != datatype)
      return;
    ApplyOptions(opts);
  }

  protected abstract void ApplyOptions(AreaOptions opts);
}
