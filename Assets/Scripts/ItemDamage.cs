using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDamage : MonoBehaviour, IEffect
{
    public int Damage = 20;
    public void ExecuteEffect(EffectArgs args)
    {
        Vector2 dir = GetComponent<Item>().transform.up;
        Health health = null;

        if (dir == Vector2.right)
            health = GameDirector.EnemyInstance.GetComponent<Health>();
        if (dir == Vector2.left)
            health = GameDirector.HeroInstance.GetComponent<Health>();

        if (health == null)
            return;

        args.Target = health.transform.position;
        args.Effect += () => health.HitBy(Damage, gameObject);
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
