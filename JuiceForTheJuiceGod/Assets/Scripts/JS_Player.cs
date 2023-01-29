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

    // Start is called before the first frame update
    void Start()
    {
        attributes = GetComponent<JS_PlayerAttributes>();
        hammer = gameObject.transform.Find("Hammer").gameObject;
        rb = gameObject.GetComponent<Rigidbody>();
        smashLock = false;
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

    void AbsorbJuice()
    {

    }
}
