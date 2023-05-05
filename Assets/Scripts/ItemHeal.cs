using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class ItemHeal : MonoBehaviour, IEffect
{
    public void ExecuteEffect()
    {
        Vector2 dir = GetComponent<Item>().Direction;

        if (dir == Vector2.up)
        {

        } else if (dir == Vector2.left)
        {

        } else if (dir == Vector2.down)
        {
           
        } else if (dir == Vector2.right)
        {

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
