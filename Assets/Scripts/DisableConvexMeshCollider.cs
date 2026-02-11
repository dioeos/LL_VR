using UnityEngine;

public class DisableConvexMeshCollider : MonoBehaviour
{
    [Tooltip("exe on start")]
    public bool runOnStart = true;

    private void Start()
    {
        if (runOnStart)
        {
            DisableConvex();
        }
    }

    [ContextMenu("Disable Convex On Children")]
    public void DisableConvex()
    {
        var colliders = GetComponentsInChildren<MeshCollider>(true);
        int count = 0;

        foreach (var col in colliders)
        {
            if (col.convex)
            {
                col.convex = false;
                count++;
            }
        }

        Debug.Log($"[DisableConvexMeshCollider] Disabled Convex on {count} MeshCollider(s).", this);
    }
}
