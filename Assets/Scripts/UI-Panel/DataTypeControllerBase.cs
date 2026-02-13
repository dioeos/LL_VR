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

  private string workspaceTag = "Workspaces";
  private string conferenceTag = "Conference";
  private string commonTag = "Common";

  protected void Awake()
  {
    // fetch all areas and store into variables via tags
    workspaceAreas = GameObject.FindGameObjectsWithTag(workspaceTag);
    Debug.Log("Workspace Area: " + workspaceAreas.Length);
    conferenceAreas = GameObject.FindGameObjectsWithTag(conferenceTag);
    Debug.Log("Conference Area: " + conferenceAreas.Length);
    commonAreas = GameObject.FindGameObjectsWithTag(commonTag);
    Debug.Log("Common Area: " + commonAreas.Length);
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
