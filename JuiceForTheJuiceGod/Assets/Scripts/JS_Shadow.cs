using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Shadow : MonoBehaviour
{
    private Transform player;
    [SerializeField]
    private Vector3 baseScale;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(player.position.x, 0.05f, player.position.z);
        //TODO
        //gameObject.transform.localScale = baseScale * Mathf.Lerp(0.5f, 1, Mathf.Clamp(0, 1, (player.transform.GetChild(0).transform.position.y - transform.position.y)));
    }
}
