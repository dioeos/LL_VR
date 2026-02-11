using UnityEngine;

// have this extend the DataTypeControllerBase.cs
public class MotionController : DataTypeControllerBase
{
  protected override void ApplyOptions(AreaOptions opts)
  {
    // for all the areas in base, enable motion lightscript
  }
}
