using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;

// emits events

public class UIController2 : MonoBehaviour
{

  public static event Action StepOneTriggered;
  public static event Action StepTwoTriggered;
  public static event Action StepThreeTriggered;
  public static event Action StepFourTriggered;

  // game objects to change
  [Tooltip("The intro module for the UI panel")]
  [SerializeField]
  private IntroModuleController introModule;

  [Tooltip("The data module for the UI panel")]
  [SerializeField]
  private DataSelectionModuleController dataModule;

  [Tooltip("The continue button users press to view other modules")]
  [SerializeField]
  private Button continueButton;

  [Tooltip("The header to change depending on step")]
  [SerializeField]
  private TMP_Text headerUI;

  private int[] stepOrder = { 0, 1, 2, 3, 4 };
  private int stepIndex = 0;

  [Tooltip(
      "The options store that stores the toggled options to datatype mapping")]
  [SerializeField]
  private OptionsStore optionsStore;

  void Awake() { continueButton.onClick.AddListener(OnContinuePressed); }

  void Start()
  {
    // show introduction module, disable the data selection module
    introModule.Show();
    dataModule.Hide();
  }

  private void UpdateCurrentOptions()
  {
    DataType dt = dataModule.GetCurrentDataType();
    AreaOptions opts = dataModule.GetCurrentAreaOptions();
    optionsStore.Save(dt, opts);
    Debug.Log(
        $"Saved {dt}: cond={opts.Condition}, " +
        $"work={opts.WorkspaceOn}, conf={opts.ConferenceRoomsOn}, " +
        $"common={opts.CommonAreasOn}, all={opts.AllOn}, none={opts.NoneOn}");
  }

  private void OnContinuePressed()
  {
    if (dataModule.IsVisible())
      UpdateCurrentOptions();
    var nextIndex = stepIndex += 1;

    if (nextIndex >= stepOrder.Length)
      return;
    EmitStep(stepIndex);
  }

  private void EmitStep(int stepVal)
  {
    switch (stepVal)
    {
      case 0:
        Debug.Log("Introduction");
        if (dataModule.IsVisible())
          dataModule.Hide();
        introModule.Show();
        break;
      case 1:
        Debug.Log("Step 1");
        if (introModule.IsVisible())
          introModule.Hide();
        headerUI.text = "Step 1 - Indoor Air Quality";
        StepOneTriggered?.Invoke();
        dataModule.Show();
        break;
      case 2:
        Debug.Log("Step 2");
        headerUI.text = "Step 2 - Indoor Air Quality";
        StepTwoTriggered?.Invoke();
        break;
      case 3:
        Debug.Log("Step 3");
        headerUI.text = "Step 3 - Bluetooth Proximity";
        StepThreeTriggered?.Invoke();
        break;
      case 4:
        Debug.Log("Step 4");
        headerUI.text = "Step 4 - Health";
        StepFourTriggered?.Invoke();
        break;
    }
  }
}
