using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;

public class LabelBakeTool : MonoBehaviour
{
    [Header("TMP Text Targets")]
    public TMP_Text[] texts;

    [Header("Render Source")]
    public RenderTexture renderTexture;

    [Header("Preview Material (BaseMap = RenderTexture)")]
    public Material previewMaterial;

    [Header("Target")]
    public MeshRenderer targetRenderer;
    public int targetMaterialIndex = 0;

    [Header("Output")]
    public string outputFolder = "Assets/GeneratedLabels";

    // Internal storage for text inputs (kept in sync with texts)
    [HideInInspector]
    public string[] textInputs;

    public void SyncInputs()
    {
        int count = (texts != null) ? texts.Length : 0;

        if (textInputs == null || textInputs.Length != count)
        {
            string[] newInputs = new string[count];
            for (int i = 0; i < count; i++)
            {
                newInputs[i] = (textInputs != null && i < textInputs.Length)
                    ? textInputs[i]
                    : "";
            }
            textInputs = newInputs;
        }
    }

    public void ApplyTexts()
    {
        if (texts == null || texts.Length == 0)
        {
            Debug.LogWarning("No TMP_Text assigned.");
            return;
        }

        SyncInputs();

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null) continue;
            texts[i].text = textInputs[i];
#if UNITY_EDITOR
            EditorUtility.SetDirty(texts[i]);
#endif
        }

        Canvas.ForceUpdateCanvases();
    }

    public void Bake()
    {
        if (renderTexture == null || targetRenderer == null)
        {
            Debug.LogError("RenderTexture or TargetRenderer missing.");
            return;
        }

        RenderTexture.active = renderTexture;

        Texture2D tex = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.RGBA32,
            false
        );

        tex.ReadPixels(
            new Rect(0, 0, renderTexture.width, renderTexture.height),
            0,
            0
        );
        tex.Apply();

        RenderTexture.active = null;

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        string safeName = targetRenderer.gameObject.name.Replace(" ", "_");
        string fileName = "Label_" + safeName + "_" +
                          System.DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") +
                          ".png";

        string path = Path.Combine(outputFolder, fileName).Replace("\\", "/");

        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.Refresh();

        Texture2D savedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

        if (previewMaterial == null)
        {
            Debug.LogError("PreviewMaterial missing.");
            return;
        }

        Material newMat = new Material(previewMaterial);
        newMat.mainTexture = savedTex;

        Material[] mats = targetRenderer.materials;

        if (targetMaterialIndex < 0 || targetMaterialIndex >= mats.Length)
        {
            Debug.LogError("TargetMaterialIndex out of range.");
            return;
        }

        mats[targetMaterialIndex] = newMat;
        targetRenderer.materials = mats;

        Debug.Log("Label baked: " + path);
    }

    public void Clear()
    {
        if (targetRenderer == null || previewMaterial == null)
            return;

        Material[] mats = targetRenderer.materials;

        if (targetMaterialIndex < 0 || targetMaterialIndex >= mats.Length)
            return;

        mats[targetMaterialIndex] = previewMaterial;
        targetRenderer.materials = mats;

        Debug.Log("Label cleared.");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LabelBakeTool))]
public class LabelBakeToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LabelBakeTool tool = (LabelBakeTool)target;

        DrawDefaultInspector();

        tool.SyncInputs();

        GUILayout.Space(10);
        GUILayout.Label("Text Inputs", EditorStyles.boldLabel);

        for (int i = 0; i < tool.textInputs.Length; i++)
        {
            string label = (tool.texts != null && i < tool.texts.Length && tool.texts[i] != null)
                ? tool.texts[i].name
                : "Text " + i;

            tool.textInputs[i] = EditorGUILayout.TextField(label, tool.textInputs[i]);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Apply Texts"))
        {
            tool.ApplyTexts();
        }

        if (GUILayout.Button("Bake Label"))
        {
            tool.ApplyTexts();
            tool.Bake();
        }

        if (GUILayout.Button("Clear"))
        {
            tool.Clear();
        }
    }
}
#endif
