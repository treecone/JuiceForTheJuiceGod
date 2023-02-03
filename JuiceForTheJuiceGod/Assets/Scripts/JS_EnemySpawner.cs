using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    //Seconds
    public float spawnRate;
    public float xzSize;

    public float enemyDistanceAllowedSqr;
    private float enemyDistanceAllowed;

    void Start()
    {
        InvokeRepeating("SpawnRandomly", 2f, spawnRate);
        enemyDistanceAllowed = Mathf.Sqrt(enemyDistanceAllowedSqr);
    }

    void Update()
    {
        
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
