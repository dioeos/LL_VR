using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Simple single-file collider generator.
/// Attach this script to any parent object.
/// In the Inspector (Edit mode), click "Generate Colliders" or "Clear Colliders".
/// </summary>
[ExecuteInEditMode]
public class AutoColliderGenerator : MonoBehaviour
{
    [Header("Options")]
    public bool includeInactive = false;
    public bool addConvexForDynamic = true;

#if UNITY_EDITOR
    [ContextMenu("Generate Colliders")]
    public void GenerateColliders()
    {
        int count = 0;
        var children = GetComponentsInChildren<Transform>(includeInactive);

        foreach (var child in children)
        {
            if (child == transform) continue;
            if (child.GetComponent<Collider>() != null) continue;

            MeshFilter mf = child.GetComponent<MeshFilter>();
            Renderer rend = child.GetComponent<Renderer>();

            if (mf != null && mf.sharedMesh != null)
            {
                MeshCollider mc = Undo.AddComponent<MeshCollider>(child.gameObject);
                mc.convex = addConvexForDynamic;
                count++;
            }
            else if (rend != null)
            {
                Undo.AddComponent<BoxCollider>(child.gameObject);
                count++;
            }
        }

        Debug.Log($"[AutoColliderGenerator] Added colliders to {count} objects under {name}.");
    }

    [ContextMenu("Clear Colliders")]
    public void ClearColliders()
    {
        int count = 0;
        var colliders = GetComponentsInChildren<Collider>(includeInactive);

        foreach (var col in colliders)
        {
            //  Trigger Collider
            if (col.isTrigger)
                continue;

            Undo.DestroyObjectImmediate(col);
            count++;
        }

        Debug.Log($"[AutoColliderGenerator] Removed {count} non-trigger colliders under {name}.");
    }


    // Custom inspector buttons
    [CustomEditor(typeof(AutoColliderGenerator))]
    public class AutoColliderGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AutoColliderGenerator script = (AutoColliderGenerator)target;

            GUILayout.Space(10);
            GUILayout.Label("Collider Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Generate Colliders"))
            {
                script.GenerateColliders();
            }

            if (GUILayout.Button("Clear Colliders"))
            {
                script.ClearColliders();
            }
        }
    }
#endif
}
