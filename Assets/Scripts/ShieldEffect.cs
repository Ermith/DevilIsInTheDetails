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


        if (!_orientedShield)
        {
            health.AddBlock(5, 5, 10);
        }
        
        if (dir == Vector3.up)
        {
            health.AddBlock(20, 5, 5);
        }

        if (dir == Vector3.down)
        {
            health.AddBlock(5, 5, 20);
        }

        if (dir == Vector3.right)
        {
            health.AddBlock(5, 20, 5);
        }

        if (dir == Vector3.left)
        {
            health.AddBlock(-5, -5, -5);
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
