using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class ItemHeal : MonoBehaviour, IEffect
{
    public int HealAmount = 20;
    public void ExecuteEffect()
    {
        Vector2 dir = GetComponent<Item>().Direction;
        Health health = null;

        if (dir == Vector2.up)
        {

        } else if (dir == Vector2.left)
        {
            health = GameDirector.HeroInstance.GetComponent<Health>();
        } else if (dir == Vector2.down)
        {
           
        } else if (dir == Vector2.right)
        {
            health = GameDirector.EnemyInstance.GetComponent<Health>();
        }

        health?.HealBy(HealAmount, gameObject);
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
