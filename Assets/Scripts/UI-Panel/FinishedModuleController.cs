using UnityEngine;

public class FinishedModuleController : MonoBehaviour
{
  [SerializeField]
  private GameObject finishedModule;

  [SerializeField]
  private DataSelectionModuleController dataModule;

  public void Show()
  {
    Debug.Log("Showing finished");
    finishedModule.SetActive(true);
  }
  public void Hide() { finishedModule.SetActive(false); }

  public bool IsVisible() { return finishedModule.activeSelf; }
}
