using System;
using System.Collections;
using UnityEngine;

namespace MyFirstMod;
public class FartBehavior : MonoBehaviour
{
    Rigidbody2D rb2d;
    BoxCollider2D bc2d;
    SpriteRenderer bulletSpriteRenderer;
    HealthManager hm;
    double xDeg, yDeg;

    public bool ignoreAllCollisions = false;

    public static HitInstance bulletDummyHitInstance = new HitInstance
    {
        DamageDealt = 4 + (PlayerData.instance.nailSmithUpgrades * 3),
        Multiplier = 1,
        IgnoreInvulnerable = false,
        CircleDirection = true,
        IsExtraDamage = false,
        Direction = 0,
        MoveAngle = 180,
        MoveDirection = false,
        MagnitudeMultiplier = 1,
        SpecialType = SpecialTypes.None,
    };

    public void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        bc2d = GetComponent<BoxCollider2D>();
        bulletSpriteRenderer = GetComponent<SpriteRenderer>();
        bc2d.enabled = !ignoreAllCollisions;
    }

    // Handles the colliders
    // https://github.com/TTacco/Hollow-Point/blob/master/HollowPoint/BulletBehaviour.cs#L171
    void OnTriggerEnter2D(Collider2D col)
    {
        hm = col.GetComponentInChildren<HealthManager>();
        bulletDummyHitInstance.Source = gameObject;

        if (col.gameObject.name.Contains("Idle") || hm != null)
        {
            Modding.Logger.Log("[Fart Knight] Enemy hit", MyFirstMod.GS.LogLevel);
            HeroController.instance.ResetAirMoves();
            HitTaker.Hit(col.gameObject, bulletDummyHitInstance);
            return;
        }
    }
}
