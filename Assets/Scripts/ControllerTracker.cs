using UnityEngine;
using UnityEngine.XR;

public class ControllerTracker : MonoBehaviour
{
    public XRNode hand = XRNode.RightHand;

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(hand);

        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
            transform.localPosition = pos;

        if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot))
            transform.localRotation = rot;
    }
}
