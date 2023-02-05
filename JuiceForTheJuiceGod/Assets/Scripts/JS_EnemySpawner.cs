using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    //Seconds
    public float spawnRate;
    public float xzSize;

    public List<SpawnerLevel> levelList;
    private int onLevel = 0;
    private float timer;
    private float spawnTimer; 

    public float enemyDistanceAllowedSqr;
    private float enemyDistanceAllowed;

    void Start()
    {
        //InvokeRepeating("SpawnRandomly", 2f, spawnRate);
        enemyDistanceAllowed = Mathf.Sqrt(enemyDistanceAllowedSqr);
        onLevel = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        Spawn();
    }

    void Spawn()
    {
        if(spawnTimer > levelList[onLevel].timeBetweenSpawns)
        {
            //Spawn enemy
            spawnTimer = 0;

            float rand = Random.Range(0, 100);
            int typeToSpawn = -1;

            //Pick right type of enemy
            float odds = 0;
            for(int i = 0; i < 6; i++)
            {
                odds += levelList[onLevel].enemySpawnRates[i];
                if (rand <= odds)
                {
                    typeToSpawn = i;
                    break;
                }
            }

            //Debug.Log(rand + "'" + odds + "'" + typeToSpawn);

            Vector3 spawnLocation = new Vector3(Random.Range(-xzSize, xzSize), 0, Random.Range(-xzSize, xzSize));
            GameObject enemy = Instantiate(enemyPrefabs[typeToSpawn], spawnLocation, Quaternion.identity) as GameObject;
            enemy.GetComponent<JS_EnemyBase>().SetSpawner(this);
            enemy.transform.SetParent(this.transform);

            //Update level
            if (timer > levelList[onLevel].timeToDeactivate && onLevel < levelList.Count)
            {
                onLevel++;
                Debug.Log("NEW LEVEL OF DIFFICULTY");
            }
        }
    }

    void SpawnRandomly()
    {
        Vector3 spawnLocation = new Vector3(Random.Range(-xzSize, xzSize), 0, Random.Range(-xzSize, xzSize));
        spawnLocation = spawnLocation.normalized * enemyDistanceAllowed;
        GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], spawnLocation, Quaternion.identity) as GameObject;
        enemy.GetComponent<JS_EnemyBase>().SetSpawner(this);
        enemy.transform.SetParent(this.transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(xzSize*2, 1, xzSize*2));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, enemyDistanceAllowed);
    }
}
