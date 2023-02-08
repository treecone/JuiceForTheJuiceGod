using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JS_CanvasScript : MonoBehaviour
{

    [SerializeField]
    private Sprite[] cupFaces;
    private GameObject playerRef;
    [SerializeField]
    private Vector2 juiceBarInsideLimits;

    public AK.Wwise.RTPC HealthRtpc;

    public int pointsTotal;
    public int fruitsSmashed;
    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFaces();
        UpdateJuiceBar();
        UpdateScoreAndTime();
    }

    void UpdateScoreAndTime()
    {
        GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = "Points Total :" + pointsTotal + "<br>Fruits Smashed: " + fruitsSmashed;
        GameObject.Find("TimeText").GetComponent<TextMeshProUGUI>().text = "Time Elapsed: " + (int)Time.timeSinceLevelLoad;
    }

    void UpdateFaces()
    {
        //get closest 20 
        int playerDurability = playerRef.GetComponent<JS_PlayerAttributes>().Durability;

        if (playerDurability == 100) playerDurability = 99;
        Image cupFaceImage = gameObject.transform.Find("CupIcon").GetComponent<Image>();
        cupFaceImage.sprite = cupFaces[playerDurability / 20];
        HealthRtpc.SetGlobalValue(playerDurability);
    }

    void UpdateJuiceBar()
    {
        GameObject juiceBar = gameObject.transform.Find("JuiceBar").gameObject;
        juiceBar.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Lerp(juiceBarInsideLimits.x, juiceBarInsideLimits.y, playerRef.GetComponent<JS_PlayerAttributes>().JuiceFulness / 100), 0, 0);
    }
}