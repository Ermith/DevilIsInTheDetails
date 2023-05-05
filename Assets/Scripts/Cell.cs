using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour
{
    [HideInInspector]
    public Inventory Inventory;

    [CanBeNull] public ItemTile ItemTile;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2Int InInventoryPos()
    {
        return new Vector2Int((int)transform.localPosition.x, (int)transform.localPosition.y);
    }
}
