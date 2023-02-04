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

    void Start()
    {
        if (cameraTarget == null)
            Debug.LogWarning("Main Camera does not have a target to follow!");

    }

    void CameraUpdate()
    {
        if (followPlayer && cameraTarget != null)
        {
            gameObject.transform.position = Vector3.Lerp(this.transform.position, cameraTarget.transform.position + cameraOffset, cameraSpeed * Time.deltaTime);
            gameObject.transform.LookAt(cameraTarget.transform);
        }
    }

    private void FixedUpdate()
    {
            CameraUpdate();
    }
}
