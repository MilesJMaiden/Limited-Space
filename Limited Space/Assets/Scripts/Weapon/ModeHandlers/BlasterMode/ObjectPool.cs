using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    public GameObject objectPrefab;
    private Queue<GameObject> objects = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        // Pre-instantiate objects and add them to the pool
        int poolSize = 10; // Adjust
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newObj = Instantiate(objectPrefab, transform);
            newObj.SetActive(false);
            objects.Enqueue(newObj);
        }
    }

    public GameObject GetPooledObject(Vector3 position, Quaternion rotation)
    {
        if (objects.Count == 0)
        {
            GameObject newObj = Instantiate(objectPrefab, position, rotation, transform);
            return newObj;
        }

        GameObject obj = objects.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.SetParent(transform); // Set as child of the pool object
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform); // Reassign the parent
        objects.Enqueue(obj);
    }
}
