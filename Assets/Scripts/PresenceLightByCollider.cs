using UnityEngine;

public class PresenceLightByCollider : MonoBehaviour
{
    [Header("Detection")]
    public LayerMask playerLayer;

    [Header("Light Control")]
    public Transform lightSet;
    public float turnOffDelay = 10f;

    [Header("Override")]
    public bool alwaysOn = false;

    [Header("Debug")]
    public bool enableDebug = true;
    public float checkInterval = 0.2f;

    BoxCollider boxCollider;

    float noOccupantTimer = 0f;
    bool isLightOn = false;
    bool hasPlayerCached = false;

    float checkTimer = 0f;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError($"[PresenceLight] No BoxCollider on {name}");
        }
    }

    void Update()
    {
        if (alwaysOn)
        {
            if (!isLightOn)
            {
                SetLight(true);
                isLightOn = true;

                if (enableDebug)
                    Debug.Log($"[LightZone:{name}] ALWAYS ON");
            }
            return;
        }

        checkTimer += Time.deltaTime;
        if (checkTimer < checkInterval)
            return;

        checkTimer = 0f;

        if (boxCollider == null)
            return;

        Bounds b = boxCollider.bounds;

        Collider[] hits = Physics.OverlapBox(
            b.center,
            b.extents,
            Quaternion.identity,
            playerLayer
        );

        bool hasPlayerNow = hits.Length > 0;

        if (enableDebug && hasPlayerNow != hasPlayerCached)
        {
            Debug.Log(
                hasPlayerNow
                    ? $"[LightZone:{name}] PLAYER ENTER"
                    : $"[LightZone:{name}] PLAYER EXIT"
            );
        }

        hasPlayerCached = hasPlayerNow;

        if (hasPlayerNow)
        {
            noOccupantTimer = 0f;

            if (!isLightOn)
            {
                SetLight(true);
                isLightOn = true;

                if (enableDebug)
                    Debug.Log($"[LightZone:{name}] LIGHT ON");
            }
        }
        else
        {
            noOccupantTimer += checkInterval;

            if (isLightOn && noOccupantTimer >= turnOffDelay)
            {
                SetLight(false);
                isLightOn = false;

                if (enableDebug)
                    Debug.Log($"[LightZone:{name}] LIGHT OFF (timeout)");
            }
        }
    }

    void SetLight(bool active)
    {
        foreach (Transform child in lightSet)
        {
            child.gameObject.SetActive(active);
        }
    }

    void OnDrawGizmos()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();

        if (boxCollider == null)
            return;

        if (alwaysOn)
            Gizmos.color = Color.green;
        else if (hasPlayerCached)
            Gizmos.color = Color.yellow;
        else
            Gizmos.color = Color.cyan;

        Bounds b = boxCollider.bounds;
        Gizmos.DrawWireCube(b.center, b.size);
    }
}
