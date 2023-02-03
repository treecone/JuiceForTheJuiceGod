using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Durians : JS_EnemyBase
{
    [SerializeField]
    private float timeInJump;
    [SerializeField]
    private float timeBetweenJumps;

    private float fruitY;
    private float durianTimer = 0;
    private bool jumpNow;
    private float playerHeight;

    protected override void Start()
    {
        base.Start();
        fruitY = 0;
        jumpNow = false;
    }

    protected override void Update()
    {
        base.Update();

        if(devotionMode)
        {
            durianTimer += Time.deltaTime;
            gameObject.transform.position = new Vector3(transform.position.x, fruitY, transform.position.z);

            if (durianTimer >= timeBetweenJumps && hostile)
            {
                durianTimer = 0.1f;
                transform.Translate(Vector3.up * 0.1f);
                jumpNow = true;
                playerHeight = playerRef.transform.position.y;
            }

            if (jumpNow)
            {
                FruitJump();
            }
        }
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
    }

    protected override void DealDamage()
    {
        if (!hostile)
        {
            return;
        }

        if (playerAttributes.invincibility && playerRef.transform.Find("Hammer").TransformPoint(Vector3.zero).y < 3)
        {
            return;
        }

        if ((playerRef.transform.Find("Hammer").position - gameObject.transform.position).sqrMagnitude <= damageDistanceSquared)
        {
            playerAttributes.durability -= damage;
            Debug.Log(gameObject.name + gameObject.GetInstanceID() + " dealt damage!");

            //Particle System 
            GameObject hitParticle = Instantiate(hitParticleSystem);
            hitParticle.transform.position = playerRef.transform.GetChild(0).transform.position;
            //hitParticle.transform.SetParent(playerRef.transform);
            hitParticle.transform.LookAt(gameObject.transform.position);
            hitParticle.transform.Rotate(new Vector3(0, 180, 0));

            StartCoroutine(EnemyTimeOut());
        }
    }

    void FruitJump()
    {
        if(transform.position.y <= 0.05f)
        {
            jumpNow = false;
            gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }

        fruitY = Mathf.Abs(Mathf.Sin((durianTimer * Mathf.PI)/timeInJump)) * playerHeight;
    }
}
