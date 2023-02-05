using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class JS_EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;

    public List<SpawnerLevel> levelList;
    private int onLevel = 0;
    private float timer;
    private float spawnTimer; 

    public float enemyDistanceAllowedSqr;
    private float enemyDistanceAllowed;
    private GameObject playerRef;
    public float spawnDistance;

    void Start()
    {
        //InvokeRepeating("SpawnRandomly", 2f, spawnRate);
        enemyDistanceAllowed = Mathf.Sqrt(enemyDistanceAllowedSqr);
        playerRef = GameObject.Find("Player");
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

            Vector3 spawnLocation = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)).normalized;
            spawnLocation = new Vector3(playerRef.transform.position.x, 0, playerRef.transform.position.z) + spawnLocation * spawnDistance;
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

    /*    void SpawnRandomly()
        {
            Vector3 spawnLocation = new Vector3(Random.Range(-xzSize, xzSize), 0, Random.Range(-xzSize, xzSize));
            spawnLocation = spawnLocation.normalized * enemyDistanceAllowed;
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], spawnLocation, Quaternion.identity) as GameObject;
            enemy.GetComponent<JS_EnemyBase>().SetSpawner(this);
            enemy.transform.SetParent(this.transform);
        }*/

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, enemyDistanceAllowed);
    }
}
