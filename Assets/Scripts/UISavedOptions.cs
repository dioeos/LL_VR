using System.Collections.Generic;
using UnityEngine;

// public struct UISavedOptions
// {
//   public CurrCondition Condition { get; set; }
//   public CurrDataType DataType { get; set; }
//   public bool WorkspaceOn { get; set; }
//   public bool ConferenceRoomsOn { get; set; }
//   public bool CommonAreasOn { get; set; }
//   public bool AllOn { get; set; }
//
//   public UISavedOptions(CurrCondition condition, CurrDataType dataType,
//                         bool workspaceOn, bool conferenceRoomsOn,
//                         bool commonAreasOn, bool allOn)
//   {
//     Condition = condition;
//     DataType = dataType;
//     WorkspaceOn = workspaceOn;
//     ConferenceRoomsOn = conferenceRoomsOn;
//     CommonAreasOn = commonAreasOn;
//     AllOn = allOn;
//   }
// }

public struct AreaOptions
{
  public Condition Condition;
  public bool WorkspaceOn;
  public bool ConferenceRoomsOn;
  public bool CommonAreasOn;
  public bool AllOn;

  // None option is the default that will be set to true if nothing on
  public bool NoneOn;

  public AreaOptions(Condition condition, bool workspaceOn,
                     bool conferenceRoomsOn, bool commonAreasOn, bool allOn,
                     bool noneOn)
  {
    Condition = condition;
    WorkspaceOn = workspaceOn;
    ConferenceRoomsOn = conferenceRoomsOn;
    CommonAreasOn = commonAreasOn;
    AllOn = allOn;
    NoneOn = noneOn;
  }
}

public class UISavedOptions
{
  public Dictionary<DataType, AreaOptions> TogglesByTypeMap =
      new Dictionary<DataType, AreaOptions>();

  public void Save(DataType dataType, AreaOptions options)
  {
    TogglesByTypeMap[dataType] = options;
  }

  public bool TryGet(DataType dataType, out AreaOptions options)
  {
    return TogglesByTypeMap.TryGetValue(dataType, out options);
  }
}
