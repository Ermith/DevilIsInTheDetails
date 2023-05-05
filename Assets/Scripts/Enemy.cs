using UnityEngine;

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
        var hero = GameObject.Find("Hero");
        hero.GetComponent<Health>().HitBy(Damage, gameObject);
    }
}