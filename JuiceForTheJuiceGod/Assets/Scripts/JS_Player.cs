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
    private bool smashNow;

    // Start is called before the first frame update
    void Start()
    {
        attributes = GetComponent<JS_PlayerAttributes>();
        hammer = gameObject.transform.Find("Hammer").gameObject;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Combat();
        Movement();
    }

    void Combat()
    {
        smashNow = smashRefrence.action.ReadValue<float>() > 0.9f;
        if(smashNow)
        {
            Vector3 currentGroundPos = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
            float hammerYInWorldSpace = hammer.transform.TransformPoint(Vector3.zero).y;
            if(hammerYInWorldSpace > 0)
            {
                hammer.transform.Translate(Vector3.down * attributes.hammerFallSpeed);
            }
            else
            {
                hammer.transform.position = currentGroundPos;
            }
        }
        else
        {
            hammer.transform.position = Vector3.Lerp(hammer.transform.position, gameObject.transform.position, Time.deltaTime * attributes.hammerRecoverySpeed);
        }
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
}
