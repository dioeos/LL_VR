using UnityEngine;

// have this extend the DataTypeControllerBase.cs
public class MotionController : DataTypeControllerBase
{
  private PresenceLightByCollider lightScript;
  protected override void ApplyOptions(AreaOptions opts)
  {
    bool workspaceEnable = opts.WorkspaceOn;
    bool conferenceEnable = opts.ConferenceRoomsOn;
    bool commonEnable = opts.CommonAreasOn;

    foreach (var area in workspaceAreas)
    {
      if (area == null)
        continue;

      var lightScript = area.GetComponent<PresenceLightByCollider>();
      if (lightScript != null)
        lightScript.enabled = workspaceEnable;
    }

    foreach (var area in conferenceAreas)
    {
      if (area == null)
        continue;

      var lightScript = area.GetComponent<PresenceLightByCollider>();
      if (lightScript != null)
        lightScript.enabled = conferenceEnable;
    }

    foreach (var area in commonAreas)
    {
      if (area == null)
        continue;

      var lightScript = area.GetComponent<PresenceLightByCollider>();
      if (lightScript != null)
        lightScript.enabled = commonEnable;
    }
  }
}
