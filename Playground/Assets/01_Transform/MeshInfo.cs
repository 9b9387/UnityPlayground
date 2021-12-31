using System.Text;
using UnityEngine;

public class MeshInfo : MonoBehaviour
{
    public Mesh mesh;

    [ContextMenu("Print Mesh Info")]
    public void Info()
    {
        if(mesh == null)
        {
            Debug.LogWarning("Mesh is null");
            return;
        }
        
        var sb = new StringBuilder();
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            var v = mesh.vertices[i];
            sb.Append($"{i} {v}\n");
        }

        Debug.Log(sb.ToString());
    }
}
