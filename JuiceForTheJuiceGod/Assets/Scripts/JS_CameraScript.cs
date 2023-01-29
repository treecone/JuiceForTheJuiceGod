using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_CameraScript : MonoBehaviour
{
    [Header("Camera Settings")]
    public GameObject cameraTarget;
    public Vector3 cameraOffset;
    public float cameraSpeed;
    public bool followPlayer;

    private bool useFixedUpdate;

    // Start is called before the first frame update
    void Start()
    {
        if (cameraTarget == null)
            Debug.LogWarning("Main Camera does not have a target to follow!");

        useFixedUpdate = GameObject.Find("Player").GetComponent<JS_Player>().usePhysics;
    }

    void CameraUpdate()
    {
        if (followPlayer && cameraTarget != null)
        {
            gameObject.transform.position = Vector3.Lerp(this.transform.position, cameraTarget.transform.position + cameraOffset, cameraSpeed * Time.deltaTime);
            gameObject.transform.LookAt(cameraTarget.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!useFixedUpdate)
            CameraUpdate();
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate)
            CameraUpdate();
    }
}
