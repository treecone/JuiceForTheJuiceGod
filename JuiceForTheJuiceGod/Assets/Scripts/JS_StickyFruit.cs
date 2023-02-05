using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_StickyFruit : JS_EnemyBase
{
    private float timeCounter;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float rotateStrength;
    [SerializeField]
    private int healthRecovered;
    [SerializeField]
    private float rotationCutoffAngle;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        timeCounter += Time.deltaTime;
    }

    protected override void Movement()
    {
        if (!hostile)
            return;

        if (devotionMode)
        {
            dir = playerRef.transform.position - transform.position;
            dir.y = 0;
            dir = dir.normalized;
        }
        else
        {
            KeepInBounds();
        }


        rb.AddForce(dir * speed);
        //rb.velocity = dir;
        float theSin = Mathf.Sin(timeCounter * rotateSpeed);

        if(Mathf.Abs(theSin) > 0.2f)
        {
            dir = RotateVectorAroundAxis(dir, Vector3.up, theSin * rotateStrength);
        }
    }

    static Vector3 RotateVectorAroundAxis(Vector3 vector, Vector3 axis, float degrees)
    {
        return Quaternion.AngleAxis(degrees, axis) * vector;
    }

    public override void Death()
    {
        GameObject juice = Instantiate(juicePrefab) as GameObject;
        juice.transform.position = new Vector3(gameObject.transform.position.x, 0.05f, gameObject.transform.position.z);
        juice.GetComponent<SpriteRenderer>().color = juiceColor;
        juice.GetComponent<JS_Juice>().maxJuice = juiceToSpawn;
        juice.GetComponent<JS_Juice>().SetJuiceType(juiceType);
        juice.transform.SetParent(GameObject.Find("AllJuices").transform);

        //Healing here
        GameObject.Find("Player").GetComponent<JS_PlayerAttributes>().Durability += healthRecovered;
        Debug.Log("Player healed for: " + healthRecovered);


        //Points
        canvasRef.GetComponent<JS_CanvasScript>().pointsTotal += pointsForDeath;
        canvasRef.GetComponent<JS_CanvasScript>().fruitsSmashed += 1;

        var main = juice.GetComponent<ParticleSystem>().main;
        main.startColor = juiceColor;

        Destroy(gameObject);
    }
}
