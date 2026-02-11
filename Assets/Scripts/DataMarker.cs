using UnityEngine;

public class DataMarker : MonoBehaviour
{
    [SerializeField]
    private string markerKey = "test_key";

    public string MarkerKey
    {
        get { return markerKey; }
    }
}
