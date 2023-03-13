using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>();
    public GameObject[] enemiesCanSpawn; //we dont want to be spawning dumb stuff ay
    public RoomClass thisRoomClass;
    

    [Range(1f, 100f)]
    public float spawnChance;
    public int maxEnemyCanSpawn;

    private List<GameObject> instancedEnemies = new List<GameObject>();
    private bool hasActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemies()
    {
        int numOfEnemiesToSpawn = Random.Range(1, maxEnemyCanSpawn);
        float chance = Random.Range(0, 100f);
        if(spawnChance >= chance)
        {
            //we do the spawning
            spawnPoints = Shuffle(spawnPoints.ToArray());//suff the points so there is unpredictability
            for (int i = 0; i < numOfEnemiesToSpawn; i++)
            {
                Transform pointToSpawn = spawnPoints[i];
                GameObject eToSpawn = enemiesCanSpawn[Random.Range(0, enemiesCanSpawn.Length - 1)];
                GameObject eInstance = Instantiate(eToSpawn, pointToSpawn.position, pointToSpawn.rotation);
                instancedEnemies.Add(eInstance);
            }
        }
    }

    //activate the enmies
    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated == false && other.transform.CompareTag("Player"))
        {
            //active the dudes
            Debug.Log("activateed the enemies");

            hasActivated = true;
            foreach (GameObject g in instancedEnemies)
            {
                g.GetComponent<EnemyHealer>().Activate();
            }
        }
    }

    List<Transform> Shuffle(Transform[] points)
    {
        List<int> createIndexList = new List<int>();

        for (int i = 0; i < points.Length; i++)
        {
            createIndexList.Add(i);
        }


        int[] a = createIndexList.ToArray();
        // Loops through array
        for (int i = a.Length - 1; i > 0; i--)
        {
            // Randomize a number between 0 and i (so that the range decreases each time)
            int rnd = Random.Range(0, i);

            // Save the value of the current i, otherwise it'll overright when we swap the values
            int temp = a[i];

            // Swap the new and old values
            a[i] = a[rnd];
            a[rnd] = temp;
        }

        List<Transform> newPoints = new List<Transform>();

        for (int i = 0; i < points.Length; i++)
        {
            Transform target = points[a[i]]; //gets transform with index of "a" in transform
            newPoints.Add(target);
        }

        return newPoints;
    }
    
}
