using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class JS_Player : MonoBehaviour
{
    public bool usePhysics;

    [Space]
    [Header("Input")]
    [SerializeField]
    private InputActionReference movementRefrence;
    [SerializeField]
    private InputActionReference smashRefrence;

    private JS_PlayerAttributes attributes;
    private Rigidbody rb;

    private GameObject hammer;
    //Input from the spacebar to tell player to smash
    private bool smashNow;
    //Locking bool so the smash damage is only applied once 
    private bool smashLock;

    private Transform enemySpawner;

    // Start is called before the first frame update
    void Start()
    {
        attributes = GetComponent<JS_PlayerAttributes>();
        hammer = gameObject.transform.Find("Hammer").gameObject;
        rb = gameObject.GetComponent<Rigidbody>();
        smashLock = false;
        enemySpawner = GameObject.Find("EnemySpawner").transform;
        attributes.invincibility = true;
    }

    // Update is called once per frame
    void Update()
    {
        Smashing();
        Movement();
    }

    void Smashing()
    {
        smashNow = smashRefrence.action.ReadValue<float>() > 0.9f;
        float hammerYInWorldSpace = hammer.transform.TransformPoint(Vector3.zero).y;

        if(!attributes.invincibility)
        {
            if(hammer.transform.localPosition.y >= -1f)
            {
                Debug.Log("Player invincibility enabled");
                attributes.invincibility = true;
            }
        }

        if (smashNow)
        {
            //Holding Space

            Vector3 currentGroundPos = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            if(hammerYInWorldSpace > 0.05f)
            {
                hammer.transform.Translate(Vector3.down * attributes.hammerFallSpeed);
            }
            else
            {
                hammer.transform.position = currentGroundPos;
                if(!smashLock)
                {
                    StartCoroutine(invincibilityTimer());
                    Combat();
                    smashLock = true;
                }
                AbsorbJuice();
            }
        }
        else
        {
            //Not Holding Space

            hammer.transform.position = Vector3.Lerp(hammer.transform.position, gameObject.transform.position, Time.deltaTime * attributes.hammerRecoverySpeed);
            if(smashLock)
            {
                //1.0f is the y axis reset range for another smash
                if (hammerYInWorldSpace > transform.position.y - 1.0f)
                {
                    smashLock = false;
                }
            }
        }
    }

    void Combat()
    {
        //Function called once when smash
        foreach(Transform child in enemySpawner)
        {
            if((child.position - hammer.transform.position).sqrMagnitude < attributes.damageRadiusSquared)
            {
                child.gameObject.GetComponent<JS_EnemyBase>().Death();
            }
        }
        Debug.Log("Dealing Damage");
    }

    void Movement()
    {
        if (smashNow)
        {
            if(usePhysics)
                rb.velocity = Vector3.zero;

            return;
        }

        Vector2 inputMovement = movementRefrence.action.ReadValue<Vector2>();
        Vector3 movementVector = new Vector3(inputMovement.x, 0, inputMovement.y);

        if(usePhysics)
        {
            rb.AddForce(movementVector * attributes.speed);
        }
        else
        {
            gameObject.transform.Translate(movementVector * attributes.speed * Time.deltaTime);
        }
    }

    IEnumerator invincibilityTimer()
    {
        yield return new WaitForSeconds(attributes.invincibilityTime);
        Debug.Log("Player invincibility disabled");
        attributes.invincibility = false;
    }

    void AbsorbJuice()
    {
        //foreach()
    }
}
