using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : MonoBehaviour, IEffect
{
    [SerializeField]
    private bool _orientedShield = false;

    public void ExecuteEffect(EffectArgs args)
    {
        var dir = GetComponent<Item>().transform.up;
        var health = GameDirector.HeroInstance.GetComponent<Health>();
        args.Target = health.transform.position;

        if (!_orientedShield)
        {
            args.Effect += () => health.AddBlock(5, 5, 10);
        }
        
        if (dir == Vector3.up)
        {
            args.Effect += () => health.AddBlock(20, 5, 5);
        }

        if (dir == Vector3.down)
        {
            args.Effect += () => health.AddBlock(5, 5, 20);
        }

        if (dir == Vector3.right)
        {
            args.Effect += () => health.AddBlock(5, 20, 5);
        }

        if (dir == Vector3.left)
        {
            args.Effect += () => health.AddBlock(-5, -5, -5);
        }
    }

    public string TooltipText
    {
        get
        {
            if (!_orientedShield)
                return "Adds 5 slash block, 5 piercing block, 10 bludgeoning block.";
            else
                return
                    "Adds block.\nPointing up: 20 slash block, 5 piercing block, 5 bludgeoning block.\nPointing down: 5 slash block, 5 piercing block, 20 bludgeoning block.\nPointing right: 5 slash block, 20 piercing block, 5 bludgeoning block.\nPointing left: -5 slash block, -5 piercing block, -5 bludgeoning block.";
        }
    }
}
