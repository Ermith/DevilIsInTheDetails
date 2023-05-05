using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int MaxHealth = 100;
    public int Health { get; private set; }

    public int Damage = 10;

    public float AttackInterval = 1;

    void Awake()
    {
        Health = MaxHealth;
    }

    void Start()
    {
        InvokeRepeating("AttackHero", AttackInterval, AttackInterval);
    }

    void AttackHero()
    {
        var hero = GameObject.Find("Hero");

        hero.GetComponent<Hero>().HitBy(Damage, gameObject);
    }
}