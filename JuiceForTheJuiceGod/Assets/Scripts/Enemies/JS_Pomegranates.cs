using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class JS_Pomegranates : JS_EnemyBase
{
    public bool childFruit;
    [SerializeField]
    private GameObject pomPrefab;
    [SerializeField]
    private int amountOfLilMansToSpawn;

    public bool fruitInvincibility;

    protected override void Start()
    {
        if (childFruit)
        {
            fruitInvincibility = true;
            StartCoroutine(OneSecondInvincibility());
        }

        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Death()
    {
        if (fruitInvincibility)
            return;

        if(childFruit)
        {
            //One of the smaller ones
            GameObject juice = Instantiate(juicePrefab) as GameObject;
            juice.transform.position = new Vector3(gameObject.transform.position.x, 0.05f, gameObject.transform.position.z);
            juice.GetComponent<SpriteRenderer>().color = juiceColor;
            juice.GetComponent<JS_Juice>().maxJuice = juiceToSpawn;
            juice.GetComponent<JS_Juice>().SetJuiceType(juiceType);
            juice.transform.SetParent(GameObject.Find("AllJuices").transform);


            var main = juice.GetComponent<ParticleSystem>().main;
            main.startColor = juiceColor;

            //Points
            canvasRef.GetComponent<JS_CanvasScript>().pointsTotal += pointsForDeath;
            canvasRef.GetComponent<JS_CanvasScript>().fruitsSmashed += 1;

            Destroy(gameObject);
        }
        else
        {

            GameObject enemySpawner = GameObject.Find("EnemySpawner");

            for(int i = 0; i < amountOfLilMansToSpawn; i++)
            {
                Vector3 newDir = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));

                //Parent fruit that splits 
                GameObject childFruit = Instantiate(pomPrefab) as GameObject;
                childFruit.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
                childFruit.GetComponent<JS_Pomegranates>().childFruit = true;
                childFruit.GetComponent<JS_Pomegranates>().fruitInvincibility = true;
                childFruit.GetComponent<JS_Pomegranates>().StartCoroutine(EnemyTimeOut());
                childFruit.GetComponent<JS_Pomegranates>().SetDirection(newDir);
                childFruit.transform.SetParent(enemySpawner.transform);
                childFruit.GetComponent<JS_EnemyBase>().SetSpawner(enemySpawner.GetComponent<JS_EnemySpawner>());
                childFruit.transform.localScale *= 0.5f;
            }

            Destroy(gameObject);
        }

    }

    IEnumerator OneSecondInvincibility()
    {
        fruitInvincibility = true;
        yield return new WaitForSeconds(1f);
        fruitInvincibility = false;
    }
}
