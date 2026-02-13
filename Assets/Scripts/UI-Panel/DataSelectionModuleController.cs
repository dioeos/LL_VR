using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public enum Condition { A, B, C }
public enum DataType { AirQuality, Bluetooth, Motion, Health }
public enum Step { One, Two, Three, Four }

public class DataSelectionModuleController : MonoBehaviour
{
  [SerializeField]
  private GameObject dataModule;

  [Tooltip("The datatype text display")]
  [SerializeField]
  private TMP_Text dataTypeText;

  [Tooltip("The text area that will be changed depending on data type and " +
           "condition")]
  [SerializeField]
  private Text viewportText;

  [Tooltip("The preview button that users press to view effect")]
  [SerializeField]
  private Button previewButton;

  [SerializeField]
  private VideoPanelController videoPanel;

  // curent state variables
  private Condition currCondition;
  private DataType currDataType;
  private Step currStep;

  // toggle variables
  [SerializeField]
  private Toggle workspaceToggle;

  [SerializeField]
  private Toggle conferenceToggle;

  [SerializeField]
  private Toggle commonToggle;

  [SerializeField]
  private Toggle allToggle;

  [SerializeField]
  private Toggle noneToggle;

  // data container logic
  [Tooltip("The data containers with descriptions corresponding to a data " +
           "type and condition")]
  [SerializeField]
  private DescriptionContainer[] descContainers;

  private Dictionary<(Condition, DataType),
                     (string text, VideoClip previewVideo)> textMap;
  void Awake()
  {
    allToggle.onValueChanged.AddListener(OnAllToggleChanged);
    noneToggle.onValueChanged.AddListener(OnNoneToggleChanged);
    workspaceToggle.onValueChanged.AddListener(OnToggleDisableNone);
    conferenceToggle.onValueChanged.AddListener(OnToggleDisableNone);
    commonToggle.onValueChanged.AddListener(OnToggleDisableNone);
    previewButton.onClick.AddListener(OnPreviewPressed);
    previewButton.gameObject.SetActive(false);
    textMap = new();
    foreach (var entry in descContainers)
    {
      textMap[(entry.condition, entry.dataType)] =
          (entry.text, entry.previewVideo);
    }
  }

  void Start()
  {
    currCondition = Condition.A;
    currDataType = DataType.AirQuality;
    UpdateViewport();
  }

  void OnEnable()
  {
    ConditionBroadcaster.OnKeyAPressed += HandleA;
    ConditionBroadcaster.OnKeyBPressed += HandleB;
    ConditionBroadcaster.OnKeyCPressed += HandleC;

    UIController2.StepOneTriggered += HandleStepOne;
    UIController2.StepTwoTriggered += HandleStepTwo;
    UIController2.StepThreeTriggered += HandleStepThree;
    UIController2.StepFourTriggered += HandleStepFour;
  }

  void OnDisable()
  {
    ConditionBroadcaster.OnKeyAPressed -= HandleA;
    ConditionBroadcaster.OnKeyBPressed -= HandleB;
    ConditionBroadcaster.OnKeyCPressed -= HandleC;
    UIController2.StepOneTriggered -= HandleStepOne;
    UIController2.StepTwoTriggered -= HandleStepTwo;
    UIController2.StepThreeTriggered -= HandleStepThree;
    UIController2.StepFourTriggered -= HandleStepFour;
  }

  public void Show() { dataModule.SetActive(true); }
  public void Hide() { dataModule.SetActive(false); }

  // helper functions
  private void OnNoneToggleChanged(bool value)
  {
    if (value == false)
      return;
    allToggle.isOn = !value;
    workspaceToggle.isOn = !value;
    conferenceToggle.isOn = !value;
    commonToggle.isOn = !value;
  }
  private void OnAllToggleChanged(bool value)
  {
    if (value == true)
    {
      noneToggle.isOn = false;
    }
    workspaceToggle.isOn = value;
    conferenceToggle.isOn = value;
    commonToggle.isOn = value;
  }

  private void OnToggleDisableNone(bool value)
  {
    if (value == false)
      return;
    noneToggle.isOn = false;
  }

  private void OnPreviewPressed()
  {
    // call videopanel controller
    if (!videoPanel.IsVisible())
    {
      if (textMap.TryGetValue((currCondition, currDataType), out var val))
      {
        if (val.previewVideo != null)
        {
          Debug.Log("Showing clip");
          videoPanel.PlayClip(val.previewVideo);
        }
        else
        {
          Debug.Log("No clip");
        }
      }
    }
    else
    {
      videoPanel.Hide();
    }
  }

  public bool IsVisible() { return dataModule.activeSelf; }

  // keyboard trigger helper functions
  private void SetCondition(Condition newCondition)
  {
    currCondition = newCondition;
    UpdateViewport();
  }

  private void SetDataType(DataType newDataType)
  {
    currDataType = newDataType;
    UpdateViewport();
  }

  private void UpdateViewport()
  {
    if (!textMap.TryGetValue((currCondition, currDataType), out var descVal) ||
        string.IsNullOrEmpty(descVal.text))
    {
      viewportText.text = "No data available";
      return;
    }
    viewportText.text = descVal.text;
  }

  public DataType GetCurrentDataType() { return currDataType; }
  public AreaOptions GetCurrentAreaOptions()
  {
    return new AreaOptions(currCondition, workspaceToggle.isOn,
                           conferenceToggle.isOn, commonToggle.isOn,
                           allToggle.isOn, noneToggle.isOn);
  }

  private void HandleA()
  {
    SetCondition(Condition.A);
    if (previewButton.gameObject.activeSelf)
      previewButton.gameObject.SetActive(false);
  }
  private void HandleB()
  {
    SetCondition(Condition.B);

    if (previewButton.gameObject.activeSelf)
      previewButton.gameObject.SetActive(false);
  }
  private void HandleC()
  {
    SetCondition(Condition.C);
    previewButton.gameObject.SetActive(true);
  }

  private void HandleStepOne()
  {
    dataTypeText.text = "Indoor Air Quality";
    SetDataType(DataType.AirQuality);
  }
  private void HandleStepTwo()
  {
    // display motion
    dataTypeText.text = "Motion";
    SetDataType(DataType.Motion);
  }
  private void HandleStepThree()
  {
    // display bp
    dataTypeText.text = "Bluetooth Proximity";
    SetDataType(DataType.Bluetooth);
  }
  private void HandleStepFour()
  {
    // display health
    dataTypeText.text = "Health";
    SetDataType(DataType.Health);
  }
}
