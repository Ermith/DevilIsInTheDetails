using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cell : MonoBehaviour
{
    [HideInInspector]
    public Inventory Inventory;

    public Vector2Int InInventoryPos;
    
    [CanBeNull] private ItemTile _itemTile;

    [CanBeNull]
    public ItemTile ItemTile
    {
        get => _itemTile;
        set
        {
            _itemTile = value;
            RefreshColor();
        }
    }

    public void RefreshColor()
    {
        gameObject.GetComponent<SpriteRenderer>().color = ItemTile != null ? new Color(0.5f, 1f, 0.5f) : Color.white;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Random.value < 0.5f)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        if (Random.value < 0.5f)
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
