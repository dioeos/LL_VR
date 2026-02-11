using UnityEngine;

[ExecuteInEditMode]
public class ReplaceWithPrefab : MonoBehaviour
{
    [Header("Replace Settings")]
    public Transform target;               // 对齐目标（pivot）
    public GameObject prefab;              // 要实例化的 Prefab
    public Transform parentContainer;       // 新增：生成物的父物体（可选）
    public bool destroyTarget = false;

    [ContextMenu("Instantiate Prefab At Target (FORCE ALIGN)")]
    public void ReplaceNow()
    {
        if (target == null || prefab == null)
        {
            Debug.LogError("[ReplaceWithPrefab] Target or Prefab is null.", this);
            return;
        }

        // 1. 实例化
        GameObject instance = Instantiate(prefab);
        instance.hideFlags = HideFlags.None;

        Transform t = instance.transform;

        // 2. 先做纯 pivot 对齐（世界坐标）
        t.position = target.position;
        t.rotation = target.rotation;
        t.localScale = target.lossyScale;

        // 3. 强制几何中心对齐（兜底）
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length > 0)
        {
            Bounds worldBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                worldBounds.Encapsulate(renderers[i].bounds);

            Vector3 delta = target.position - worldBounds.center;
            t.position += delta;
        }

        // 4. 设置父物体（保持世界坐标不变）
        if (parentContainer != null)
        {
            t.SetParent(parentContainer, true); // true = 保持世界坐标
        }

        // 5. 可选：删除目标
        if (destroyTarget)
        {
            DestroyImmediate(target.gameObject);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(instance);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(instance.scene);
#endif

        Debug.Log("[ReplaceWithPrefab] FORCE aligned prefab and parented successfully.", instance);
    }
}
