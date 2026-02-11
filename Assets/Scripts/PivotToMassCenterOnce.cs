using UnityEngine;

[ExecuteInEditMode]
public class PivotToMassCenterOnce : MonoBehaviour
{
    [ContextMenu("Recenter Pivots To Mass Center")]
    public void RecenterAll()
    {
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>(true);
        int count = 0;

        foreach (MeshFilter mf in filters)
        {
            if (RecenterOne(mf))
            {
                count++;
            }
        }

        Debug.Log("Recentered pivots for " + count + " MeshFilters.");
    }

    private bool RecenterOne(MeshFilter mf)
    {
        if (mf == null) return false;
        Mesh original = mf.sharedMesh;
        if (original == null) return false;


        Mesh mesh = Object.Instantiate(original);
        mesh.name = original.name + "_pivotCentered";
        mf.sharedMesh = mesh;

        Vector3[] verts = mesh.vertices;
        if (verts == null || verts.Length == 0) return false;

        Vector3 centroid = Vector3.zero;
        for (int i = 0; i < verts.Length; i++)
        {
            centroid += verts[i];
        }
        centroid /= verts.Length;


        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] -= centroid;
        }
        mesh.vertices = verts;
        mesh.RecalculateBounds();

        Transform t = mf.transform;
        Vector3 worldOffset = t.TransformVector(centroid);
        t.position += worldOffset;

        return true;
    }
}
