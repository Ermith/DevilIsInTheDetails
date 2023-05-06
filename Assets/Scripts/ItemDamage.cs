using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDamage : MonoBehaviour, IEffect
{
    public int Damage = 20;
    public void ExecuteEffect()
    {
        Vector2 dir = GetComponent<Item>().Direction;
        Health health = null;

        if (dir == Vector2.right)
            health = GameDirector.EnemyInstance.GetComponent<Health>();
        if (dir == Vector2.left)
            health = GameDirector.HeroInstance.GetComponent<Health>();

        health?.HitBy(Damage, gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
