using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeDamageEffect : MonoBehaviour, IEffect
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
            type = Health.DamageType.Strike;
        }

        if (dir == Vector2.right)
        {
            health = GameDirector.EnemyInstance.GetComponent<Health>();
            type = Health.DamageType.Slash;
        }

        if (dir == Vector2.left)
        {
            health = GameDirector.HeroInstance.GetComponent<Health>();
            type = Health.DamageType.Strike;
        }

        if (health == null)
            return;

        args.Target = health.transform.position;
        args.Effect += () => health.HitBy(Damage, type, gameObject);
    }
}
