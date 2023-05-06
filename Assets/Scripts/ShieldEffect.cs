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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
