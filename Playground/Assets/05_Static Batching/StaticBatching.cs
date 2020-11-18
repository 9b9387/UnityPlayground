using UnityEngine;

public class StaticBatching : MonoBehaviour
{
    public Mesh mesh;

    void Start()
    {
        // 对所有子节点进行网格合并，所有的子节点会进行一次合批处理
        StaticBatchingUtility.Combine(gameObject);
    }

    /// <summary>
    /// 运行时，对material更改会打断合批
    /// </summary>
    [ContextMenu("ChangeMaterial")]
    public void ChangeMaterial()
    {
        var mrs = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < mrs.Length; i++)
        {
            mrs[i].material = new Material(Shader.Find("Standard"));
        }
    }

    /// <summary>
    /// 运行时，对mesh更改会打断合批
    /// </summary>
    [ContextMenu("ChangeMesh")]
    public void ChangeMesh()
    {
        var mrs = GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < mrs.Length; i++)
        {
            mrs[i].mesh = mesh;
        }
    }
}
