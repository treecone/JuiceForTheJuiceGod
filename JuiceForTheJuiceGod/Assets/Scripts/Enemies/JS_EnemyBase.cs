using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_EnemyBase : MonoBehaviour
{
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

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRef = GameObject.Find("Player");
        hostile = true;
        playerAttributes = playerRef.GetComponent<JS_PlayerAttributes>();
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

        Vector3 dir = playerRef.transform.position - transform.position;
        dir.y = 0;
        dir = dir.normalized;

        gameObject.transform.Translate(dir * speed * Time.deltaTime);
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
                AkSoundEngine.PostEvent("EnemyDealtDamage", this.gameObject);

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
