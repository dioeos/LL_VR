using System;
using UnityEngine;

public class OptionsStore : MonoBehaviour
{
  public UISavedOptions toggledOptions = new UISavedOptions();
  public Action<DataType, AreaOptions> OptionsChanged;

  public void Save(DataType dt, AreaOptions opts)
  {
    toggledOptions.Save(dt, opts);
    OptionsChanged?.Invoke(dt, opts);
  }

  public bool TryGet(DataType dt, out AreaOptions optsVal)
  {
    return toggledOptions.TryGet(dt, out optsVal);
  }
}
