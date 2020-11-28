using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBatching : MonoBehaviour
{
    public GameObject prefab;
    public List<GameObject> gameObjects;
    // Start is called before the first frame update
    void Start()
    {
        gameObjects = new List<GameObject>();
        for (int i = 0; i < 1000; i++)
        {
            var x = Random.Range(-10.0f, 10.0f);
            var y = Random.Range(-5.0f, 5.0f);
            var z = Random.Range(-10.0f, 10.0f);
            var pos = new Vector3(x, y, z);
            Debug.Log(y);
            var angle = Random.Range(0f, 360f);
            var obj = GameObject.Instantiate(prefab, pos, Quaternion.Euler(0, angle, z));
            gameObjects.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            var obj = gameObjects[i];
            {
                var pos = obj.transform.position;
                pos = new Vector3(pos.x, pos.y + (1 * Time.deltaTime), pos.z);
                obj.transform.position = pos;
            }

            if (obj.transform.position.y > 5)
            {
                var pos = new Vector3(obj.transform.position.x, -5f, obj.transform.position.z);
                obj.transform.position = pos;
            }
        }
    }
}
