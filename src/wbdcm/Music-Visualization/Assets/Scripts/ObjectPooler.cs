using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public static ObjectPooler Instance;

    public void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Void that resets all the pools
    /// </summary>
    public void ResetPools()
    {
        foreach (Pool pool in pools)
        {
            foreach (GameObject current in poolDictionary[pool.tag])
            {
                current.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Method that is used instead of Instanciate. Spawns objects from pool
    /// </summary>
    /// <param name="tag">Tag of an object</param>
    /// <param name="position">Position where object will spawn</param>
    /// <returns></returns>
    public GameObject SpawnFromPool(string tag, Vector3 position)
    {
        if (poolDictionary == null)
        {
            EmergencyInitialization();
        }

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    /// <summary>
    /// Inicialization that is used in cases when spawn method is called before Start
    /// </summary>
    void EmergencyInitialization()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }
}
