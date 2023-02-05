using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JS_CameraScript : MonoBehaviour
{
    [Header("Camera Settings")]
    public GameObject cameraTarget;
    public Vector3 cameraOffset;
    public float cameraSpeed;
    private bool lookAt = true;
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
            if(lookAt)
            gameObject.transform.LookAt(cameraTarget.transform);
        }
    }

    private void FixedUpdate()
    {
        CameraUpdate();
    }

    IEnumerator StopLookingAt(float time)
    {
        lookAt = false;
        yield return new WaitForSeconds(time);
        lookAt = true;
    }

    public void ScreenShake(Vector2 dir, float strength)
    {
        gameObject.transform.Translate(dir * strength, Space.World);
        StartCoroutine(StopLookingAt(0.5f));
    }
}
