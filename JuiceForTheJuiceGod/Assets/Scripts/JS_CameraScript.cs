using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Camera Settings")]
    public GameObject cameraTarget;
    public Vector3 cameraOffset;
    [Range(0, 1)]
    public float cameraSpeed;
    public bool followPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (cameraTarget == null)
            Debug.LogWarning("Main Camera does not have a target to follow!");
    }

    // Update is called once per frame
    void Update()
    {
        if(followPlayer && cameraTarget != null)
        {
            gameObject.transform.position = Vector3.Lerp(this.transform.position, cameraTarget.transform.position + cameraOffset, cameraSpeed * Time.deltaTime);
        }
    }
}
