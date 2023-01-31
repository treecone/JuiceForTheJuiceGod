using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public enum JUICE_TYPES
{
    Oranges = 0,
    Strawberries = 1,
    Grapes = 2,
    StickyFruit = 3,
    Durians = 4, 
    Pomegranates = 5,

};

public class JS_Player : MonoBehaviour
{
    public bool usePhysics;

    public AK.Wwise.Event SuckSound;
    public AK.Wwise.Event StopSuckSound;


    [Space]
    [Header("Input")]
    [SerializeField]
    private InputActionReference movementRefrence;
    [SerializeField]
    private InputActionReference smashRefrence;
    [SerializeField]
    private float[] juiceStored;

    private JS_PlayerAttributes attributes;
    private Rigidbody rb;

    private GameObject hammer;
    //Input from the spacebar to tell player to smash
    private bool holdingSpace;
    //Locking bool so the smash damage is only applied once 
    private bool smashLock;

    private Transform enemySpawner;
    private Transform allJuices;
    public List<GameObject> nearbyJuices;

    private bool absorbLock;

    // Start is called before the first frame update
    void Start()
    {
        attributes = GetComponent<JS_PlayerAttributes>();
        hammer = gameObject.transform.Find("Hammer").gameObject;
        rb = gameObject.GetComponent<Rigidbody>();

        enemySpawner = GameObject.Find("EnemySpawner").transform;
        allJuices = GameObject.Find("AllJuices").transform;

        attributes.invincibility = true;
        smashLock = false;

        juiceStored = new float[5];
        nearbyJuices = new List<GameObject>();

        absorbLock = false;
    }

    // Update is called once per frame
    void Update()
    {
        Smashing();
        Movement();
    }

    void Smashing()
    {
        holdingSpace = smashRefrence.action.ReadValue<float>() > 0.9f;
        float hammerYInWorldSpace = hammer.transform.TransformPoint(Vector3.zero).y;

        if(!attributes.invincibility)
        {
            if(hammer.transform.localPosition.y >= -1f)
            {
                Debug.Log("Player invincibility enabled");
                attributes.invincibility = true;
            }
        }

        if (holdingSpace)
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

                    //Get all nearby juices
                    nearbyJuices.Clear();
                    foreach(Transform child in allJuices)
                    {
                        //Debug.Log((child.position - hammer.transform.position).sqrMagnitude + "," + attributes.absorbtionRadiusSquared);
                        if((child.position - hammer.transform.position).sqrMagnitude <= attributes.absorbtionRadiusSquared)
                        {
                            nearbyJuices.Add(child.gameObject);
                            child.GetComponent<ParticleSystem>().Play();
                            var emission = child.GetComponent<ParticleSystem>().emission;
                            emission.rateOverTime = 5.0f;
                        }
                    }

                    smashLock = true;
                }

                if (!attributes.invincibility && nearbyJuices.Count > 0)
                {
                    AbsorbJuice();
                }
            }
        }
        else
        {
            //Not Holding Space
            if(!absorbLock)
            {
                StopSuckSound.Post(gameObject);
                absorbLock = false;
            }

            hammer.transform.position = Vector3.Lerp(hammer.transform.position, gameObject.transform.position, Time.deltaTime * attributes.hammerRecoverySpeed);
            if(smashLock)
            {
                //1.0f is the y axis reset range for another smash
                if (hammerYInWorldSpace > 1.0f)
                {
                    smashLock = false;
                }
            }

            if(nearbyJuices.Count > 0)
            {
                //Reset juice on ground
                foreach (GameObject j in nearbyJuices)
                {
                    var emission = j.GetComponent<ParticleSystem>().emission;
                    emission.rateOverTime = 0f;
                }
                nearbyJuices.Clear();
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
        if (holdingSpace)
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
        if(!absorbLock)
        {
            absorbLock = true;
            SuckSound.Post(gameObject);
        }

        float amountToYoinkTotal = 0;
        for(int i = 0; i < nearbyJuices.Count; i++)
        {
            float yoinked = nearbyJuices[i].GetComponent<JS_Juice>().RetreveJuice(attributes.absorbtionSpeed);

            if (yoinked == 0)
            {
                nearbyJuices.RemoveRange(i, 1);
                continue;
            }
            amountToYoinkTotal += yoinked;
            juiceStored[(int)nearbyJuices[i].GetComponent<JS_Juice>().GetJuiceType()] += yoinked;
        }

        attributes.juicefulness += amountToYoinkTotal;

        //Change UI

    }

    void OnDrawGizmos()
    {
        if(smashLock)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hammer.transform.position, attributes.damageRadiusSquared);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hammer.transform.position, Mathf.Sqrt(attributes.absorbtionRadiusSquared));
        }
    }
}
