using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_Juice : MonoBehaviour
{
    public float juice;
    public float maxJuice;
    [SerializeField]
    private float maxScale;
    [SerializeField]
    private float minScale;

    [SerializeField]
    private Sprite[] allSprites; 

    private Renderer juiceRenderer;
    private JUICE_TYPES juiceType;

    private void Start()
    {
        juiceRenderer = GetComponent<Renderer>();
        juice = maxJuice;
        gameObject.GetComponent<SpriteRenderer>().sprite = allSprites[Random.Range(0, allSprites.Length-1)];
        //Randomize this a bit?
        gameObject.transform.localScale = new Vector3(Mathf.Lerp(minScale, maxScale, juice/100), Mathf.Lerp(minScale, maxScale, juice / 100), Mathf.Lerp(minScale, maxScale, juice / 100));
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
