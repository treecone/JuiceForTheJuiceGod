using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_EnemyBase : MonoBehaviour
{
    [SerializeField]
    protected bool devotionMode;

    public AK.Wwise.Event FruitSquish;
    protected Rigidbody rb;
    protected GameObject playerRef;

    protected bool hostile;
    protected JS_PlayerAttributes playerAttributes;

    [Space]
    [Header("Stats")]
    public float speed;
    public int damage;
    public float damageDistanceSquared;
    public float timeBetweenDamage;

    [Space]
    [SerializeField]
    protected Color juiceColor;
    [SerializeField]
    protected JUICE_TYPES juiceType;

    [SerializeField]
    protected GameObject juicePrefab;
    protected JS_EnemySpawner spawner;

    [SerializeField]
    protected Vector3 dir;

    protected bool outOfBoundsLock;

    public void SetSpawner(JS_EnemySpawner spawner)
    {
        this.spawner = spawner;
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRef = GameObject.Find("Player");
        hostile = true;
        playerAttributes = playerRef.GetComponent<JS_PlayerAttributes>();
        devotionMode = false;
        dir = new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f)).normalized;
        rb.velocity = dir * speed;
        outOfBoundsLock = false;
    }

    protected virtual void Update()
    {
        Movement();
        DealDamage();
    }

    protected virtual void Movement()
    {
        if (!hostile)
            return;

        if(devotionMode)
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

    protected void KeepInBounds()
    {
        Vector3 dirToSpawner = (spawner.gameObject.transform.position - transform.position);

        if (dirToSpawner.sqrMagnitude >= spawner.enemyDistanceAllowedSqr)
        {
            if(!outOfBoundsLock)
            {
                dir *= -1;
                dir += dirToSpawner;
                dir.Normalize();
                outOfBoundsLock = true;
            }
        }

        if(outOfBoundsLock)
        {
            if (dirToSpawner.sqrMagnitude < spawner.enemyDistanceAllowedSqr)
            {
                outOfBoundsLock = false;
            }
        }
    }

    protected virtual void DealDamage()
    {
        if(!hostile || playerAttributes.invincibility)
        {
            return;
        }

        if(playerRef.transform.Find("Hammer").transform.TransformPoint(Vector3.zero).y < 1)
        {
            //If the player hammer is currently down
            if ((playerRef.transform.Find("Hammer").position - gameObject.transform.position).sqrMagnitude <= damageDistanceSquared)
            {
                playerAttributes.durability -= damage;
                Debug.Log(gameObject.name + gameObject.GetInstanceID() + " dealt damage!");
                StartCoroutine(EnemyTimeOut());
            }
        }
    }

    protected IEnumerator EnemyTimeOut ()
    {
        hostile = false;
        rb.velocity *= 0;
        yield return new WaitForSeconds(timeBetweenDamage);
        hostile = true;
    }

    public void Death()
    {
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
}
