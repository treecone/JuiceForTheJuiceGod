using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_PlayerAttributes : MonoBehaviour
{
    [Header("Player Stats")]
    //Max is 100 for both
    [Range(0, 100), SerializeField]
    private float juiceFulness;
    public float JuiceFulness
    {
        get { return juiceFulness; }
        set { juiceFulness = Mathf.Clamp(value, 0, 100); }
    }
    [Range(0, 100), SerializeField]
    private int durability;
    public int Durability
    {
        get { return durability; }
        set { durability = Mathf.Clamp(value, 0, 100); }
    }

    public float speed;
    public float vision;

    [Space]
    [Header("Combat")]

    public int cupDamage;
    public float damageRadiusSquared;
    public float absorbtionRadiusSquared;
    //Juice absorbed per frame
    public float absorbtionSpeed;
    public float hammerRecoverySpeed;
    public float hammerFallSpeed;
    public float invincibilityTime;
    public bool invincibility;
    public float height;
    public float smashCost;

}
