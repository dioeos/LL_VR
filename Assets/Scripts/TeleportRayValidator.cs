using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;

public class TeleportRayValidator : MonoBehaviour
{
    [Header("References")]
    public Transform playerRoot;
    public Component rayInteractor;

    // NEW: real tracked hand pose (assign in Inspector)
    [Header("Ray Direction Override")]
    public Transform handPoseTransform;

    [Header("Teleport Settings")]
    public string floorTag = "Floor";
    public float capsuleRadius = 0.3f;
    public float capsuleHeight = 1.7f;

    [Tooltip("When ray hits non-floor objects (furniture), cast downward from hit point to find Floor below.")]
    public float downwardSearchDistance = 5.0f;

    public bool debugDrawCapsule = false;

    private InputDevice rightHand;
    private bool lastTriggerPressed = false;

    // Cached reflection for: bool TryGetCurrent3DRaycastHit(out RaycastHit hit)
    private MethodInfo miTryGetHit;
    private object[] tryGetHitArgs;

    void Start()
    {
        Debug.Log("[TP-START] Script Start");

        rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        Debug.Log("[TP-START] Got RightHand device. valid = " + rightHand.isValid);

        if (rayInteractor == null)
        {
            Debug.Log("[TP-START] RayInteractor not assigned, searching...");
            rayInteractor = FindRayInteractorComponent(gameObject);
        }

        if (rayInteractor == null)
        {
            Debug.LogError("[TP-ERROR] RayInteractor not found.");
        }
        else
        {
            Debug.Log("[TP-START] RayInteractor found: " + rayInteractor.GetType().Name);
        }

        if (playerRoot == null)
        {
            Debug.LogError("[TP-ERROR] PlayerRoot not assigned.");
        }
        else
        {
            Debug.Log("[TP-START] PlayerRoot assigned: " + playerRoot.name);
        }

        if (handPoseTransform == null)
        {
            Debug.LogWarning("[TP-WARN] handPoseTransform not assigned. Ray will use its own transform.");
        }
        else
        {
            Debug.Log("[TP-START] handPoseTransform assigned: " + handPoseTransform.name);
        }

        CacheRayInteractorMethod();
    }

    void Update()
    {
        // NEW: force ray transform to follow real tracked hand pose
        if (handPoseTransform != null)
        {
            transform.position = handPoseTransform.position;
            transform.rotation = handPoseTransform.rotation;
        }

        Debug.Log("[TP-U0] Update tick");

        if (playerRoot == null)
        {
            Debug.Log("[TP-U1] playerRoot null -> return");
            return;
        }

        if (rayInteractor == null)
        {
            Debug.Log("[TP-U2] rayInteractor null -> return");
            return;
        }

        RaycastHit firstHit;
        if (!TryGetRayHit(out firstHit))
        {
            Debug.Log("[TP-U3] TryGetRayHit failed -> return");
            return;
        }

        if (firstHit.collider == null)
        {
            Debug.Log("[TP-U3B] firstHit.collider null -> return");
            return;
        }

        Debug.Log("[TP-U4] Ray first hit collider: " + firstHit.collider.name);
        Debug.Log("[TP-U5] First hit tag = " + firstHit.collider.tag);

        RaycastHit floorHit;
        if (!ResolveFloorHit(firstHit, out floorHit))
        {
            Debug.Log("[TP-U9] No Floor found for this aim -> return");
            return;
        }

        Debug.Log("[TP-U10] Using Floor collider: " + floorHit.collider.name + " | Tag = " + floorHit.collider.tag);

        Vector3 targetPosition = new Vector3(
            floorHit.point.x,
            playerRoot.position.y,
            floorHit.point.z
        );

        Debug.Log("[TP-U11] TargetPosition (XZ from Floor, keep Y) = " + targetPosition.ToString("F3"));

        bool canStand = CanStandAt(targetPosition);
        Debug.Log("[TP-U12] CanStandAt = " + canStand);

        bool triggerDown;
        if (!GetTriggerDown(out triggerDown))
        {
            Debug.Log("[TP-U13] Trigger read failed -> return");
            return;
        }

        Debug.Log("[TP-U14] TriggerDown = " + triggerDown);

        if (triggerDown && canStand)
        {
            Debug.Log("[TP-U15] TELEPORT EXECUTED");
            playerRoot.position = targetPosition;
        }
        else
        {
            Debug.Log("[TP-U16] Teleport condition not met");
        }
    }

    bool ResolveFloorHit(RaycastHit firstHit, out RaycastHit floorHit)
    {
        floorHit = default(RaycastHit);

        if (firstHit.collider != null && firstHit.collider.CompareTag(floorTag))
        {
            floorHit = firstHit;
            Debug.Log("[TP-F0] First hit IS Floor: " + floorHit.collider.name);
            return true;
        }

        Debug.Log("[TP-F1] First hit is NOT Floor, searching downward from hit point...");

        Vector3 origin = firstHit.point + Vector3.up * 0.05f;
        Ray downRay = new Ray(origin, Vector3.down);

        RaycastHit[] hits = Physics.RaycastAll(
            downRay,
            downwardSearchDistance,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore
        );

        Debug.Log("[TP-F2] Downward RaycastAll hit count = " + hits.Length);

        bool found = false;
        float bestY = float.NegativeInfinity;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit h = hits[i];
            if (h.collider == null)
                continue;

            Debug.Log("[TP-F3] Down hit " + i + " : " + h.collider.name + " | Tag = " + h.collider.tag +
                      " | Point = " + h.point.ToString("F3"));

            if (h.collider.CompareTag(floorTag))
            {
                if (!found || h.point.y > bestY)
                {
                    bestY = h.point.y;
                    floorHit = h;
                    found = true;
                }
            }
        }

        if (found)
        {
            Debug.Log("[TP-F4] Found Floor below: " + floorHit.collider.name + " | Point = " + floorHit.point.ToString("F3"));
        }
        else
        {
            Debug.Log("[TP-F5] No Floor collider found below.");
        }

        return found;
    }

    bool CanStandAt(Vector3 position)
    {
        Debug.Log("[TP-C0] Checking stand position");

        Vector3 bottom = position + Vector3.up * capsuleRadius;
        Vector3 top = bottom + Vector3.up * (capsuleHeight - 2f * capsuleRadius);

        Debug.Log("[TP-C1] Capsule bottom = " + bottom.ToString("F3"));
        Debug.Log("[TP-C2] Capsule top = " + top.ToString("F3"));

        bool blocked = Physics.CheckCapsule(
            bottom,
            top,
            capsuleRadius,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore
        );

        Debug.Log("[TP-C3] CheckCapsule blocked = " + blocked);

        if (debugDrawCapsule)
        {
            DrawCapsuleDebug(bottom, top, capsuleRadius, blocked ? Color.red : Color.green);
        }

        return !blocked;
    }

    bool GetTriggerDown(out bool down)
    {
        down = false;

        if (!rightHand.isValid)
        {
            Debug.Log("[TP-I0] RightHand invalid, reacquiring");
            rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            return false;
        }

        bool pressed;
        if (rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out pressed))
        {
            Debug.Log("[TP-I1] Trigger pressed = " + pressed);
            down = pressed && !lastTriggerPressed;
            lastTriggerPressed = pressed;
            return true;
        }

        Debug.Log("[TP-I2] TryGetFeatureValue failed");
        return false;
    }

    void CacheRayInteractorMethod()
    {
        miTryGetHit = null;
        tryGetHitArgs = null;

        if (rayInteractor == null)
            return;

        Type t = rayInteractor.GetType();
        miTryGetHit = t.GetMethod(
            "TryGetCurrent3DRaycastHit",
            BindingFlags.Instance | BindingFlags.Public,
            null,
            new Type[] { typeof(RaycastHit).MakeByRefType() },
            null
        );

        if (miTryGetHit == null)
        {
            Debug.LogError("[TP-ERROR] RayInteractor has no TryGetCurrent3DRaycastHit(out RaycastHit)");
            rayInteractor = null;
            return;
        }

        Debug.Log("[TP-START] Cached TryGetCurrent3DRaycastHit");
        tryGetHitArgs = new object[] { default(RaycastHit) };
    }

    bool TryGetRayHit(out RaycastHit hit)
    {
        hit = default(RaycastHit);

        if (rayInteractor == null || miTryGetHit == null || tryGetHitArgs == null)
        {
            Debug.Log("[TP-R0] Ray reflection not ready");
            return false;
        }

        tryGetHitArgs[0] = default(RaycastHit);
        bool ok = (bool)miTryGetHit.Invoke(rayInteractor, tryGetHitArgs);

        if (!ok)
        {
            Debug.Log("[TP-R1] RayInteractor reports no hit");
            return false;
        }

        hit = (RaycastHit)tryGetHitArgs[0];
        return true;
    }

    Component FindRayInteractorComponent(GameObject go)
    {
        Component[] comps = go.GetComponents<Component>();
        for (int i = 0; i < comps.Length; i++)
        {
            Component c = comps[i];
            if (c == null) continue;

            MethodInfo mi = c.GetType().GetMethod(
                "TryGetCurrent3DRaycastHit",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new Type[] { typeof(RaycastHit).MakeByRefType() },
                null
            );

            if (mi != null)
                return c;
        }
        return null;
    }

    void DrawCapsuleDebug(Vector3 bottom, Vector3 top, float radius, Color color)
    {
        Debug.DrawLine(bottom, top, color);
        Debug.DrawLine(bottom + Vector3.right * radius, top + Vector3.right * radius, color);
        Debug.DrawLine(bottom - Vector3.right * radius, top - Vector3.right * radius, color);
        Debug.DrawLine(bottom + Vector3.forward * radius, top + Vector3.forward * radius, color);
        Debug.DrawLine(bottom - Vector3.forward * radius, top - Vector3.forward * radius, color);
    }
}
