using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class PlayerDataReader : MonoBehaviour
{
    [Header("Data")]
    public string dataRootPath;

    [Header("UI (0-4 used)")]
    public TMP_Text[] textSlots;

    private BoxCollider box;
    private float timer;
    private const float interval = 1f;

    private string currentMarkerKey = null;

    void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            DetectMarker();
            UpdateUI();
        }
    }

    // -------------------------------
    // 1. Detect marker (stable)
    // -------------------------------
    void DetectMarker()
    {
        currentMarkerKey = null;

        Vector3 center = transform.TransformPoint(box.center);
        Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, transform.lossyScale);

        Collider[] hits = Physics.OverlapBox(
            center,
            halfExtents,
            transform.rotation,
            ~0,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            DataMarker marker =
                hit.GetComponent<DataMarker>() ??
                hit.GetComponentInParent<DataMarker>();

            if (marker != null && marker.MarkerKey != "test_key")
            {
                currentMarkerKey = marker.MarkerKey;
                break; // 只取一个
            }
        }
    }

    // -------------------------------
    // 2. Update UI
    // -------------------------------
    void UpdateUI()
    {
        ClearUI();

        if (string.IsNullOrEmpty(currentMarkerKey))
            return;

        List<string> lines = ReadData(currentMarkerKey);

        for (int i = 0; i < lines.Count && i < textSlots.Length; i++)
        {
            textSlots[i].text = lines[i];
            textSlots[i].gameObject.SetActive(true);
        }
    }

    void ClearUI()
    {
        foreach (var t in textSlots)
        {
            if (t == null) continue;
            t.text = "";
            t.gameObject.SetActive(false);
        }
    }

    // -------------------------------
    // 3. Read CSV data
    // -------------------------------
    List<string> ReadData(string key)
    {
        List<string> result = new List<string>();

        string[] parameters =
        {
            "temp",
            "humidity",
            "lux",
            "pm2_5",
            "spl_a"
        };

        foreach (string param in parameters)
        {
            string path = Path.Combine(
                dataRootPath,
                param,
                param + "_" + key + ".csv"
            );

            if (!File.Exists(path))
                continue;

            if (TryReadValue(path, out float value))
            {
                result.Add(param + ": " + value.ToString("F2"));
            }
        }

        return result;
    }

    bool TryReadValue(string path, out float value)
    {
        value = 0f;

        try
        {
            using (FileStream fs = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fs))
            {
                string headerLine = reader.ReadLine();
                string dataLine = reader.ReadLine();

                if (string.IsNullOrEmpty(headerLine) || string.IsNullOrEmpty(dataLine))
                    return false;

                string[] headers = headerLine.Split(',');
                string[] data = dataLine.Split(',');

                for (int i = 0; i < headers.Length && i < data.Length; i++)
                {
                    if (headers[i].Trim().ToLower() == "value")
                    {
                        return float.TryParse(data[i], out value);
                    }
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

}
