using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamageEffect : MonoBehaviour, IEffect
{
    public int Damage = 20;
    public void ExecuteEffect(EffectArgs args)
    {
        Vector2 dir = GetComponent<Item>().transform.up;
        Health health = null;
        Health.DamageType type = Health.DamageType.Slash;

        if (dir == Vector2.up)
        {
            health = GameDirector.EnemyInstance.GetComponent<Health>();
            type = Health.DamageType.Slash;       
        }

        if (dir == Vector2.down)
        {
            health = GameDirector.EnemyInstance.GetComponent<Health>();
            type = Health.DamageType.Slash;
        }

        if (dir == Vector2.right)
        {
            health = GameDirector.EnemyInstance.GetComponent<Health>();
            type = Health.DamageType.Thrust;
        }

        if (dir == Vector2.left)
        {
            health = GameDirector.HeroInstance.GetComponent<Health>();
            type = Health.DamageType.Thrust;
        }

        if (health == null)
            return;

        args.Target = health.transform.position;
        args.Effect += () => health.HitBy(Damage, type, gameObject);
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
