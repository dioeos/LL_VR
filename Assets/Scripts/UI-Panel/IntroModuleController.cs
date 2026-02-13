using UnityEngine;
using UnityEngine.UI;

public class IntroModuleController : MonoBehaviour
{
  [SerializeField]
  private GameObject introModule;

  [SerializeField]
  private DataSelectionModuleController dataModule;

  // [SerializeField]
  // private Button continueButton;

  // [Tooltip("The continue button from the introduction module that triggers "
  // +
  //          "data selection module")]
  // [SerializeField]
  // private Button continueButton;

  void Awake()
  { /*  continueButton.onClick.AddListener(OnContinuePressed);  */
  }

  public void Show() { introModule.SetActive(true); }
  public void Hide() { introModule.SetActive(false); }

  public bool IsVisible() { return introModule.activeSelf; }

  // private void OnContinuePressed()
  // {
  //   Hide();
  //   dataModule.Show();
  // }
}
