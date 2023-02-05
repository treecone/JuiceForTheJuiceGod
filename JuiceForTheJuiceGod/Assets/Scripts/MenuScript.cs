using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public AK.Wwise.Event clickEvent;
    public AK.Wwise.Event clickStopEvent;
    public AK.Wwise.Event clickEventLow;

    private void Start()
    {
        //clickStopEvent.Post(gameObject);
    }

    public void PlayGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PlayAgain ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void QuitGame ()
    {
        Application.Quit();
    }

    public void StopSound()
    {
        clickStopEvent.Post(gameObject);
    }

    public void PlaySound ()
    {
        clickEvent.Post(gameObject);
    }

    public void PlaySoundLow()
    {
        clickEventLow.Post(gameObject);
    }
}
