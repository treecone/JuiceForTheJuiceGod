using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Juice : MonoBehaviour
{
    public float juice;
    public float maxJuice;
    [SerializeField]
    private float maxScale;

    private Renderer juiceRenderer;
    private JUICE_TYPES juiceType;

    private void Start()
    {
        juiceRenderer = GetComponent<Renderer>();
        juice = maxJuice;
        //Randomize this a bit?
        gameObject.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
    }

    public void SetJuiceType(JUICE_TYPES type) { this.juiceType = type; }
    public JUICE_TYPES GetJuiceType() { return this.juiceType; }

    public float RetreveJuice(float amountToRetreve)
    {
        if(juice <= 0.1f)
        {
            Object.Destroy(gameObject);
        }

        if(amountToRetreve <= juice)
        {
            if(juiceRenderer != null)
                juiceRenderer.material.SetFloat("_Capacity", Mathf.Lerp(0, 1.1f, (juice / maxJuice)));
            float newScale = Mathf.Lerp(maxScale-1, maxScale, juice / maxJuice);
            gameObject.transform.localScale = new Vector3(newScale, newScale, newScale);
            juice -= amountToRetreve;

            //Debug.Log("Slurping: " + amountToRetreve);
            return amountToRetreve;
        }
        else
        {
            juice = 0;
            return juice;
        }
    }
}
