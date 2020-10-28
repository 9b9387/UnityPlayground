using UnityEngine;

public class GPUInstancing : MonoBehaviour
{
    public GameObject prefab;
    public int instanceCount;
    // Start is called before the first frame update
    void Start()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();

        for (int i = 0; i < instanceCount; i++)
        {
            var obj = Instantiate(prefab);
            var renderer = obj.GetComponent<MeshRenderer>();

            float r = Random.Range(0.0f, 1.0f);
            float g = Random.Range(0.0f, 1.0f);
            float b = Random.Range(0.0f, 1.0f);
            props.SetColor("_Color", new Color(r, g, b));
            renderer.SetPropertyBlock(props);

            float x = Random.Range(-10f, 10f);
            float y = Random.Range(-10f, 10f);
            float z = Random.Range(-10f, 10f);
            obj.transform.position = new Vector3(x, y, z);

            float rotate = Random.Range(0f, 360f);
            obj.transform.rotation = Quaternion.Euler(new Vector3(0, rotate, 0));

            float s = Random.Range(0.3f, 1.5f);
            props.SetFloat("_Scale", s);
            renderer.SetPropertyBlock(props);
        }
    }
}
