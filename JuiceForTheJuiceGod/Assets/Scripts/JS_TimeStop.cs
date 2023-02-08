using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_TimeStop : MonoBehaviour
{
    private float speed;
    private bool restoreTime;
    public AK.Wwise.Event Sqwart;
    public AK.Wwise.RTPC SlowMo;

    private void Start()
    {
        restoreTime = false;
        Time.timeScale = 1.0f;
    }

    public void Update()
    {
        SlowMo.SetGlobalValue(Time.timeScale * 100);
        if (restoreTime)
        {
            if (Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime * speed;
            }
            else
            {
                Time.timeScale = 1f;
                restoreTime = false;

                Vector3 currentGroundPos = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
                gameObject.transform.Find("Hammer").transform.position = currentGroundPos;
                Sqwart.Post(gameObject);
            }
        }
    }

    //What time to change to, tick at which time is restored, how long to delay the game before restoring time to normal
    public void StopTime(float ChangeTime, int RestoreSpeed, float Delay)
    {
        speed = RestoreSpeed;

        if (Delay > 0)
        {
            StopCoroutine(StartTimeAgain(Delay));
            StartCoroutine(StartTimeAgain(Delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = ChangeTime;
    }

    IEnumerator StartTimeAgain(float amt)
    {
        restoreTime = true;
        yield return new WaitForSecondsRealtime(amt);
    }
}