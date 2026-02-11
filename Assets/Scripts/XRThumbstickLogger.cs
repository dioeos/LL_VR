using UnityEngine;
using UnityEngine.XR;

public class XRThumbstickLogger : MonoBehaviour
{
    // Select LeftHand or RightHand in the Inspector
    public XRNode hand = XRNode.LeftHand;

    void Update()
    {
        // Get the input device for the specified hand
        InputDevice device = InputDevices.GetDeviceAtXRNode(hand);

        // Read the thumbstick 2D axis
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
        {
            if (axis.magnitude > 0.05f) // ignore small noise
            {
                Debug.Log($"[{hand}] Thumbstick Axis -> X: {axis.x:F2}, Y: {axis.y:F2}");
            }
        }

        // Read the thumbstick press (click)
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool pressed) && pressed)
        {
            Debug.Log($"[{hand}] Thumbstick Pressed");
        }
    }
}
