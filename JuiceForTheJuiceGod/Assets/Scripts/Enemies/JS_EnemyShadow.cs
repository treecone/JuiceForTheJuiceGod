using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_EnemyShadow : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
    }
}
