using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JS_PlayerAttributes : MonoBehaviour
{
    [Header("Player Stats")]
    //Max is 100 for both
    [Range(0, 100)]
    public float juicefulness;
    [Range(0, 100)]
    public int durability;

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
