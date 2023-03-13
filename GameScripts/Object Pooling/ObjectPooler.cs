using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    //example of object spawn : ObjectPooler.Instance.SpawnFromPool("something", this.transform.position, quaternion.identity)

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool setToParent;
    }
    public List<Pool> pools;

    #region Singleton;
    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, this.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("pool with tag" + tag + "doesnt exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        if(parent == null)
        {
            objectToSpawn.transform.SetParent(this.transform);
        }
        else
        {
            objectToSpawn.transform.SetParent(parent);
        }

        IPooledObject ipooledObject = objectToSpawn.GetComponent<IPooledObject>();
        if(ipooledObject != null)
        {
            ipooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

}
