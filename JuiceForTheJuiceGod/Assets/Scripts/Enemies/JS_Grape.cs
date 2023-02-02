using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Grape : JS_EnemyBase
{
    private float waveTimer;
    [SerializeField]
    private float sinWaveSpeed;
    [SerializeField]
    private float sideSpeed;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        waveTimer += Time.deltaTime;
    }

    protected override void Movement()
    {
        if (!hostile)
            return;

        if (devotionMode)
        {
            dir = playerRef.transform.position - transform.position;
            dir.y = 0;
            dir = dir.normalized;
        }
        else
        {
            KeepInBounds();
        }

        Vector2 a = Vector2.Perpendicular(new Vector2(dir.x, dir.z));
        Vector3 perpDir = new Vector3(a.x, 0, a.y);

        rb.AddForce(dir * speed);
        float sinScaler = Mathf.Sin(sinWaveSpeed * waveTimer);
        //Debug.Log(sinScaler);
        if(sinScaler > 0)
        {
            gameObject.transform.Translate(perpDir * sideSpeed * Time.deltaTime);
        }
        else
        {
            gameObject.transform.Translate(-1 * perpDir * sideSpeed * Time.deltaTime);
        }
    }
}
