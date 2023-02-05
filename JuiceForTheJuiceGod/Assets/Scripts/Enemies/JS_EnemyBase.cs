using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_EnemyBase : MonoBehaviour
{
    [SerializeField]
    public bool devotionMode;

    public AK.Wwise.Event GlassCracks;
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
    public int pointsForDeath;

    [Space]
    [SerializeField]
    protected Color juiceColor;
    [SerializeField]
    protected JUICE_TYPES juiceType;

    [SerializeField]
    protected GameObject juicePrefab;
    protected JS_EnemySpawner spawner;
    [SerializeField]
    protected int juiceToSpawn;
    [SerializeField]
    protected GameObject hitParticleSystem;
    protected GameObject canvasRef;

    protected Vector3 dir;

    protected bool outOfBoundsLock;

    public void SetDirection(Vector3 dir)
    {
        this.dir = dir;
    }

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
        //devotionMode = false;
        dir = ((new Vector3(playerRef.transform.position.x, 0, playerRef.transform.position.z) - gameObject.transform.position) + new Vector3(Random.Range(-25.0f, 25.0f), 0, Random.Range(-25.0f, 25.0f))).normalized;
        rb.velocity = dir * speed;
        outOfBoundsLock = false;
        canvasRef = GameObject.Find("MainCanvas");

        if (playerRef.GetComponent<JS_Player>().lastStoredBiggestJuice == 2)
        {
            devotionMode = true;
        }
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

    protected void KeepInBounds()
    {
        Vector3 dirToSpawner = (spawner.gameObject.transform.position - transform.position);

        if (dirToSpawner.sqrMagnitude >= spawner.enemyDistanceAllowedSqr)
        {
            if (!outOfBoundsLock)
            {
                /*                dir *= -1;
                                dir += dirToSpawner;
                                dir.Normalize();*/

                Destroy(gameObject);
                outOfBoundsLock = true;
            }
        }

        if (outOfBoundsLock)
        {
            if (dirToSpawner.sqrMagnitude < spawner.enemyDistanceAllowedSqr)
            {
                outOfBoundsLock = false;
            }
        }
    }

    protected virtual void DealDamage()
    {
        if (!hostile || playerAttributes.invincibility)
        {
            return;
        }

        if (playerRef.transform.Find("Hammer").transform.TransformPoint(Vector3.zero).y < 1)
        {
            //If the player hammer is currently down
            if ((playerRef.transform.Find("Hammer").position - gameObject.transform.position).sqrMagnitude <= damageDistanceSquared)
            {
                GlassCracks.Post(gameObject);
                playerAttributes.Durability -= damage;
                Debug.Log(gameObject.name + gameObject.GetInstanceID() + " dealt damage!");

                //Particle System 
                GameObject hitParticle = Instantiate(hitParticleSystem);
                hitParticle.transform.position = playerRef.transform.GetChild(0).transform.position;
                //hitParticle.transform.SetParent(playerRef.transform);
                hitParticle.transform.LookAt(gameObject.transform.position);
                hitParticle.transform.Rotate(new Vector3(0, 180, 0));

                GameObject.Find("MainCamera").GetComponent<JS_CameraScript>().ScreenShake(hitParticle.transform.forward, 1.5f);
                Debug.DrawRay(gameObject.transform.position, hitParticle.transform.forward, Color.red);

                rb.AddForce((new Vector3(playerRef.transform.position.x, 0, playerRef.transform.position.z) - gameObject.transform.position) * -300);
                StartCoroutine(EnemyTimeOut());
            }
        }
    }

    protected IEnumerator EnemyTimeOut()
    {
        hostile = false;
        rb.velocity *= 0;
        yield return new WaitForSeconds(timeBetweenDamage);
        hostile = true;
    }

    public virtual void Death()
    {
        GameObject juice = Instantiate(juicePrefab) as GameObject;
        juice.transform.position = new Vector3(gameObject.transform.position.x, 0.05f, gameObject.transform.position.z);
        juice.GetComponent<SpriteRenderer>().color = juiceColor;
        juice.GetComponent<JS_Juice>().SetJuiceType(juiceType);
        juice.GetComponent<JS_Juice>().maxJuice = juiceToSpawn;
        juice.transform.SetParent(GameObject.Find("AllJuices").transform);

        //Points
        canvasRef.GetComponent<JS_CanvasScript>().pointsTotal += pointsForDeath;
        canvasRef.GetComponent<JS_CanvasScript>().fruitsSmashed += 1;


        var main = juice.GetComponent<ParticleSystem>().main;
        main.startColor = juiceColor;

        Destroy(gameObject);
    }
}