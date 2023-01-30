using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_EnemyBase : MonoBehaviour
{
    [SerializeField]
    protected bool usePhysics;
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

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRef = GameObject.Find("Player");
        hostile = true;
        playerAttributes = playerRef.GetComponent<JS_PlayerAttributes>();
    }

    protected void Update()
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

        if(usePhysics)
        {
            rb.AddForce(dir * speed);
        }
        else
        {
            gameObject.transform.Translate(dir * speed * Time.deltaTime);
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

    IEnumerator EnemyTimeOut ()
    {
        hostile = false;
        yield return new WaitForSeconds(timeBetweenDamage);
        hostile = true;
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
