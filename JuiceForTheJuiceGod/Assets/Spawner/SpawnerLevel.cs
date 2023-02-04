using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerLevel", menuName = "ScriptableObjects/SpawnerLevel", order = 1)]
public class SpawnerLevel : ScriptableObject
{
    public float timeToDeactivate;
    public float timeBetweenSpawns;
    public float[] enemySpawnRates;

    //Odds have to >= 100%
}
