using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;


/// <summary>
/// Detects nearest XRSimpleInteractable under controller ray
/// When Trigger pressed, opens Chrome with specified URL
/// </summary>
public class DirectSimpleInteractableRay_OpenChrome : MonoBehaviour
{
    [Header("Ray source")]
    public XRNode hand = XRNode.RightHand;
    public Transform handTransformOverride;

    [Header("Ray settings")]
    public float rayLength = 12f;
    public LayerMask layerMask = ~0;
    public QueryTriggerInteraction triggerMode = QueryTriggerInteraction.Collide;

    [Header("Browser")]
    public string url = "https://www.google.com"; // default URL

    private bool lastTrigger = false;

    void Update()
    {
        Vector3 origin;
        Quaternion rot;

        // 1. Ray origin
        if (handTransformOverride != null)
        {
            origin = handTransformOverride.position;
            rot = handTransformOverride.rotation;
        }
        else
        {
            InputDevice dev = InputDevices.GetDeviceAtXRNode(hand);
            if (!dev.isValid) return;
            if (!dev.TryGetFeatureValue(CommonUsages.devicePosition, out origin)) return;
            if (!dev.TryGetFeatureValue(CommonUsages.deviceRotation, out rot)) return;
        }

        Vector3 dir = rot * Vector3.forward;
        Ray ray = new Ray(origin, dir);

        // 2. Raycast all
        RaycastHit[] hits = Physics.RaycastAll(ray, rayLength, layerMask, triggerMode);
        if (hits.Length == 0)
        {
            lastTrigger = false;
            return;
        }

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable target = null;
        RaycastHit targetHit = default;

        foreach (var h in hits)
        {
            var interactable = h.collider.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            if (interactable != null)
            {
                target = interactable;
                targetHit = h;
                break;
            }
        }

        if (target == null)
        {
            lastTrigger = false;
            return;
        }

        // 3. Debug line (scene view)
        //Debug.DrawLine(origin, targetHit.point, Color.green);

        // 4. Trigger press edge
        InputDevice device = InputDevices.GetDeviceAtXRNode(hand);
        if (!device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed))
            return;

        if (triggerPressed && !lastTrigger)
        {
            //Debug.Log($"[DirectSimple] Trigger on: {target.name}, opening Chrome...");

            try
            {
                // Try to open Chrome; fallback to default browser if not found
                Process.Start("chrome.exe", url);
            }
            catch (Exception)
            {
                Application.OpenURL(url);
            }
        }

        lastTrigger = triggerPressed;
    }
}
