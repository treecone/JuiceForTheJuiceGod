using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Pomegranates : JS_EnemyBase
{
    public bool childFruit;
    [SerializeField]
    private GameObject pomPrefab;

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
            juice.GetComponent<JS_Juice>().SetJuiceType(juiceType);
            juice.transform.SetParent(GameObject.Find("AllJuices").transform);

            FruitSquish.Post(gameObject);

            var main = juice.GetComponent<ParticleSystem>().main;
            main.startColor = juiceColor;

            Destroy(gameObject);
        }
        else
        {
            FruitSquish.Post(gameObject);

            GameObject enemySpawner = GameObject.Find("EnemySpawner");
            Vector2 perp = Vector2.Perpendicular(new Vector2(dir.x, dir.z));

            //Parent fruit that splits 
            GameObject childFruit1 = Instantiate(pomPrefab) as GameObject;
            childFruit1.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            childFruit1.GetComponent<JS_Pomegranates>().childFruit = true;
            childFruit1.GetComponent<JS_Pomegranates>().fruitInvincibility = true;
            childFruit1.GetComponent<JS_Pomegranates>().StartCoroutine(EnemyTimeOut());
            childFruit1.GetComponent<JS_Pomegranates>().SetDirection(new Vector3(perp.x, 0, perp.y));
            childFruit1.transform.SetParent(enemySpawner.transform);
            childFruit1.GetComponent<JS_EnemyBase>().SetSpawner(enemySpawner.GetComponent<JS_EnemySpawner>());
            childFruit1.transform.localScale *= 0.5f;

            GameObject childFruit2 = Instantiate(pomPrefab) as GameObject;
            childFruit2.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            childFruit2.GetComponent<JS_Pomegranates>().childFruit = true;
            childFruit2.GetComponent<JS_Pomegranates>().fruitInvincibility = true;
            childFruit2.GetComponent<JS_Pomegranates>().StartCoroutine(EnemyTimeOut());
            childFruit2.GetComponent<JS_Pomegranates>().SetDirection(new Vector3(perp.x, 0, perp.y) * -1);
            childFruit2.transform.SetParent(enemySpawner.transform);
            childFruit2.GetComponent<JS_EnemyBase>().SetSpawner(enemySpawner.GetComponent<JS_EnemySpawner>());
            childFruit2.transform.localScale *= 0.5f;

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
