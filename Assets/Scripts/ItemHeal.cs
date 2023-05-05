using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class ItemHeal : MonoBehaviour, IEffect
{
    public void ExecuteEffect()
    {
        GetComponent<Item>();
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
