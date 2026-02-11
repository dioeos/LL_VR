using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

/// <summary>
/// Moves a dot based on joystick input and changes its alpha.
/// Near the center it's transparent; at full push it's fully visible.
/// </summary>
public class XRJoystickDotAlpha : MonoBehaviour
{
    public XRNode inputSource = XRNode.LeftHand;   // controller to read
    public RectTransform dot;                      // the UI dot object
    public float maxRadius = 80f;                  // max movement in pixels
    public float moveSmooth = 8f;                  // smoothing speed
    public float deadZone = 0.05f;                 // ignore small noise
    public float minAlpha = 0.0f;                  // transparent at center
    public float maxAlpha = 1.0f;                  // visible at edge

    private Vector2 currentPos = Vector2.zero;
    private Image dotImage;

    void Start()
    {
        if (dot != null)
            dotImage = dot.GetComponent<Image>();
    }

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        if (!device.isValid || dot == null || dotImage == null) return;

        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
        {
            // Clamp stick magnitude to [0,1]
            if (axis.magnitude < deadZone)
                axis = Vector2.zero;

            Vector2 targetPos = axis.normalized * Mathf.Min(axis.magnitude, 1.0f) * maxRadius;
            currentPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * moveSmooth);
            dot.anchoredPosition = currentPos;

            // Fade alpha based on distance from center
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, axis.magnitude);
            Color c = dotImage.color;
            c.a = alpha;
            dotImage.color = c;
        }
    }
}
