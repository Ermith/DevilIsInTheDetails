﻿using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int Damage = 10;
    public float AttackInterval = 1;

    void Start()
    {
        InvokeRepeating("AttackHero", AttackInterval, AttackInterval);
    }

    void AttackHero()
    {
        var hero = GameDirector.HeroInstance;
        hero.GetComponent<Health>().HitBy(Damage, Health.DamageType.Slash, gameObject);
    }

    private void OnDestroy()
    {
        GameDirector.GameDirectorInstance.EndFight();
        GameDirector.GameDirectorInstance.StartFight();
    }
}