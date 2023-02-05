using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

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
    public AK.Wwise.Event SuckSound;
    public AK.Wwise.Event StopSuckSound;
    public AK.Wwise.RTPC CupFullness;
    public AK.Wwise.Event FirstHitSound;
    public AK.Wwise.Event HitGroundNoEnemy;

    [Space]
    [Header("Input")]
    [SerializeField]
    private InputActionReference movementRefrence;
    [SerializeField]
    private InputActionReference smashRefrence;
    [SerializeField]
    private float[] juiceStored;
    public int lastStoredBiggestJuice;

    private JS_PlayerAttributes attributes;
    private Rigidbody rb;

    [SerializeField]
    private Color[] juiceColors;

    private GameObject hammer;
    //Input from the spacebar to tell player to smash
    private bool holdingSpace;
    //Locking bool so the smash damage is only applied once 
    private bool smashLock;

    private Transform enemySpawner;
    private Transform allJuices;
    public List<GameObject> nearbyJuices;

    private bool absorbLock;
    private Vector3 ogCameraOffset;
    private GameObject mainCamera;
    private GameObject mainCanvas;

    [SerializeField]
    private Sprite[] playerSprites;
    private JS_EnemySpawner spawner;


    [Space]
    [Header("Attributes that change with fullness")]
    [SerializeField]
    private Vector2 heightDelta;
    [SerializeField]
    private Vector2 speedDelta;
    [SerializeField]
    private Vector2 smashCostDelta;
    [SerializeField]
    private Vector2 dragDelta;
    [SerializeField]
    private Vector2 viewDelta;
    [SerializeField]
    private Vector2 leakDelta;
    [SerializeField]
    private Vector2 particleLeakDelta;
    [SerializeField]
    private Vector2 hammerFallSpeedDelta;
    [SerializeField]
    private Vector2 hammerRecoverySpeedDelta;


    // Start is called before the first frame update
    void Start()
    {
        attributes = GetComponent<JS_PlayerAttributes>();
        hammer = gameObject.transform.Find("Hammer").gameObject;
        rb = gameObject.GetComponent<Rigidbody>();

        enemySpawner = GameObject.Find("EnemySpawner").transform;
        allJuices = GameObject.Find("AllJuices").transform;
        mainCanvas = GameObject.Find("MainCanvas");

        attributes.invincibility = true;
        smashLock = false;

        juiceStored = new float[6];
        nearbyJuices = new List<GameObject>();

        absorbLock = false;
        spawner = GameObject.Find("EnemySpawner").GetComponent<JS_EnemySpawner>();

        //Camera 
        mainCamera = GameObject.Find("MainCamera");
        ogCameraOffset = mainCamera.GetComponent<JS_CameraScript>().cameraOffset;
    }

    // Update is called once per frame
    void Update()
    {
        Smashing();
        Movement();
        UpdateAttributesWithFullness();
        Leaking();
        KeepInBounds();
        UpdateJuiceColors();

        gameObject.transform.position = new Vector3(transform.position.x, attributes.height, transform.position.z);
        mainCamera.GetComponent<JS_CameraScript>().cameraOffset = ogCameraOffset * attributes.vision;

        if(attributes.Durability <= 0)
        {
            Death();
        }
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
            if(hammerYInWorldSpace > 0.8f)
            {
                hammer.transform.Translate(Vector3.down * attributes.hammerFallSpeed * Time.deltaTime);
            }
            else
            {
                //hammer.transform.position = currentGroundPos;
                if(!smashLock)
                {
                    StartCoroutine(invincibilityTimer());
                    Combat();
                    gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = playerSprites[0];

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

                if (nearbyJuices.Count == 0)
                {
                    absorbLock = false;
                    StopSuckSound.Post(gameObject);
                }

                if (!attributes.invincibility && nearbyJuices.Count > 0)
                {
                    AbsorbJuice();
                }
            }
        }
        else
        {
            //Not Holding Space ---------------------------

            if(absorbLock)
            {
                StopSuckSound.Post(gameObject);
                absorbLock = false;
            }

            hammer.transform.position = Vector3.Lerp(hammer.transform.position, gameObject.transform.position, Time.deltaTime * attributes.hammerRecoverySpeed);
            if(smashLock)
            {
                //1.0f is the y axis reset range for another smash
                if (hammerYInWorldSpace > 0.2f)
                {
                    smashLock = false;
                    gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = playerSprites[1];
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

    void Death()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z), 0.1f);
        if(gameObject.transform.position.y <= 0.2f)
        {

        }
    }

    void Combat()
    {
        //Function called once when smash
        int amountOfEnemiesHit = 0;
        foreach(Transform child in enemySpawner)
        {
            if((child.position - hammer.transform.position).sqrMagnitude < attributes.damageRadiusSquared)
            {
                //Debug.Log("Dealing Damage to " + child.gameObject.name + child.gameObject.GetInstanceID());
                child.gameObject.GetComponent<JS_EnemyBase>().Death();
                amountOfEnemiesHit++;
            }
        }

        if(amountOfEnemiesHit == 0)
        {
            //Missed all enemies, lose juice
            Vector3 currentGroundPos = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            hammer.transform.position = currentGroundPos;
            Debug.Log("Missed enemies, leaking: " + attributes.smashCost);
            Leaking(attributes.smashCost);
            HitGroundNoEnemy.Post(gameObject);
        }
        else
        {
            FirstHitSound.Post(gameObject);
            gameObject.GetComponent<JS_TimeStop>().StopTime(0.01f, 10, 0.1f * amountOfEnemiesHit);
        }
    }

    void Movement()
    {
        if (holdingSpace)
        {
            rb.velocity = Vector3.zero;
        }

        Vector2 inputMovement = movementRefrence.action.ReadValue<Vector2>();
        Vector3 movementVector = new Vector3(inputMovement.x, 0, inputMovement.y);

        rb.AddForce(movementVector * attributes.speed);
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

        attributes.JuiceFulness += amountToYoinkTotal;

        //Change UI

    }

    void UpdateAttributesWithFullness()
    {
        attributes.height = Mathf.Lerp(heightDelta.x, heightDelta.y, attributes.JuiceFulness/100);
        attributes.speed = Mathf.Lerp(speedDelta.x, speedDelta.y, attributes.JuiceFulness / 100);
        attributes.smashCost = Mathf.Lerp(smashCostDelta.x, smashCostDelta.y, attributes.JuiceFulness / 100);
        rb.drag = Mathf.Lerp(dragDelta.x, dragDelta.y, attributes.JuiceFulness / 100);
        attributes.vision = Mathf.Lerp(viewDelta.x, viewDelta.y, attributes.JuiceFulness / 100);
        attributes.hammerFallSpeed = Mathf.Lerp(hammerFallSpeedDelta.x, hammerFallSpeedDelta.y, attributes.JuiceFulness / 100);
        attributes.hammerRecoverySpeed = Mathf.Lerp(hammerRecoverySpeedDelta.x, hammerRecoverySpeedDelta.y, attributes.JuiceFulness / 100);


        CupFullness.SetGlobalValue(attributes.JuiceFulness);
    }

    void Leaking(float amountToLeak = 0)
    {

        if(amountToLeak <= 0)
        {
            //Leak from durability
            amountToLeak = Mathf.Lerp(leakDelta.x, leakDelta.y, attributes.Durability / 100);
        }
        attributes.JuiceFulness -= amountToLeak;
        var emission = gameObject.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>().emission;
        emission.rateOverTime = Mathf.Lerp(particleLeakDelta.x, particleLeakDelta.y, attributes.Durability / 100);
    }

    void UpdateJuiceColors()
    {
        float amount = 0;
        int biggestJuice = 0;
        for(int i = 0; i < juiceStored.Length; i++)
        {
            if (juiceStored[i] > amount)
            {
                biggestJuice = i;
                amount = juiceStored[i];
            }
        }

        //New biggest juice stored
        if(lastStoredBiggestJuice != biggestJuice)
        {
            lastStoredBiggestJuice = biggestJuice;
            if (biggestJuice == 2)
            {
                //Enable Devotion mode
                foreach (Transform child in enemySpawner)
                {
                    child.GetComponent<JS_EnemyBase>().devotionMode = true;
                }
            }
            else
            {
                //Enable Devotion mode
                foreach (Transform child in enemySpawner)
                {
                    child.GetComponent<JS_EnemyBase>().devotionMode = false;
                }
            }
        }
        

        Color juiceColor = Color.Lerp(gameObject.transform.Find("Hammer").Find("PlayerSprite").GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color, juiceColors[biggestJuice], 0.1f);

        gameObject.transform.Find("Hammer").Find("PlayerSprite").GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = juiceColor;
        gameObject.transform.Find("Hammer").Find("LeakingParticle").GetComponent<ParticleSystem>().startColor = juiceColor;
        gameObject.transform.Find("Hammer").Find("PlayerSprite").GetChild(0).GetChild(0).transform.localPosition = new Vector3(0, Mathf.Lerp(-1.5f, 0, attributes.JuiceFulness/100), 0);
        mainCanvas.transform.Find("Mask").transform.GetChild(0).GetComponent<Image>().color = juiceColor;
        mainCanvas.transform.Find("Mask").transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(0, Mathf.Lerp(-200f, 5, attributes.JuiceFulness / 100), 0);

    }

    protected void KeepInBounds()
    {
        Vector3 dirToSpawner = (spawner.gameObject.transform.position - transform.position);

        if (dirToSpawner.sqrMagnitude >= spawner.enemyDistanceAllowedSqr)
        {
            rb.AddForce(dirToSpawner * attributes.speed * 15 * Time.deltaTime);
            Debug.Log("Out of bounds!");
        }
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
