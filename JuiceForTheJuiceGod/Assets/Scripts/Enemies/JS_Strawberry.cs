using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Strawberry : JS_EnemyBase
{
    [SerializeField]
    private float sinWaveSpeed;
    [SerializeField]
    private float sinWaveForce;

    private float waveTimer;

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
        //gameObject.transform.Translate(dir * speed);

        gameObject.transform.Translate(dir + ((Mathf.Sin(waveTimer * sinWaveSpeed) * sinWaveForce) * perpDir) * speed);
        //rb.AddForce(dir + ((Mathf.Sin(waveTimer * sinWaveSpeed) * sinWaveForce) * perpDir) * speed);
    }
}
