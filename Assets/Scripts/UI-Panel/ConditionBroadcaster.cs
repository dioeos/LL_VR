using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ConditionBroadcaster : MonoBehaviour
{
  public static event Action OnKeyAPressed;
  public static event Action OnKeyBPressed;
  public static event Action OnKeyCPressed;

  void Update()
  {
    var kb = Keyboard.current;
    if (kb == null)
      return;

    bool isUpper = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;

    if (kb.aKey.wasPressedThisFrame)
    {
      if (isUpper)
      {
        OnKeyAPressed?.Invoke();
        Debug.Log("A (upper) pressed");
      }
      else
        OnKeyAPressed?.Invoke();
      Debug.Log("A (lower) pressed");
    }

    if (kb.bKey.wasPressedThisFrame)
    {
      if (isUpper)
      {
        OnKeyBPressed?.Invoke();
        Debug.Log("B (upper) pressed");
      }
      else
        OnKeyBPressed?.Invoke();
      Debug.Log("B (lower) pressed");
    }

    if (kb.cKey.wasPressedThisFrame)
    {
      if (isUpper)
      {
        OnKeyCPressed?.Invoke();
        Debug.Log("C (upper) pressed");
      }
      else
        OnKeyCPressed?.Invoke();
      Debug.Log("C (lower) pressed");
    }
  }
}
