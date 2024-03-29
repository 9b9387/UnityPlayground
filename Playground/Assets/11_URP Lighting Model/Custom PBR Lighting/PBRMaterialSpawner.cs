using UnityEngine;

public class PBRMaterialSpawner : MonoBehaviour
{
    public GameObject prefab;

    void Start()
    {
        for (int row = 0; row <= 10; row++)
        {
            for (int col = 0; col <= 10; col++)
            {
                var obj = Instantiate<GameObject>(prefab, new Vector3(col * 2f, 1f, row * 1.2f), Quaternion.identity);
                var renderer = obj.GetComponent<MeshRenderer>();
                var material = renderer.material;
                material.SetFloat("_Metallic", row / 10f);
                material.SetFloat("_Smoothness", col / 10f);
            }
        }
    }

    void Update()
    {
        
    }
}
