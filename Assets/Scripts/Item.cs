using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Item : MonoBehaviour
{
    [HideInInspector]
    public bool CanMove = true;

    [HideInInspector]
    public Vector2 Direction = Vector2.up;

    private bool _moving;

    private Vector2 _dragStartPosition;

    private Vector2 _dragOffset;

    [ItemCanBeNull] public ItemTile[,] Shape;

    public int Width => Shape.GetLength(0);

    public int Height => Shape.GetLength(1);

    public int TileCount { get; private set; }

    public bool InInventory { get; set; }

    [CanBeNull] public static Item DraggedItem;

    public void Execute()
    {
        gameObject.BroadcastMessage("ExecuteEffect");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Shape == null)
            InitShape();
    }

    void InitShape()
    {
        int width = 0;
        int height = 0;
        foreach (var tile in GetComponentsInChildren<ItemTile>())
        {
            var pos = tile.InItemPos();
            width = Mathf.Max(width, pos.x + 1);
            height = Mathf.Max(height, pos.y + 1);
        }

        TileCount = 0;
        Shape = new ItemTile[width, height];
        foreach (var tile in GetComponentsInChildren<ItemTile>())
        {
            var pos = tile.InItemPos();
            Shape[pos.x, pos.y] = tile;
            TileCount++;
        }
    }

    public void TileDestroyed(ItemTile tile)
    {
        var pos = tile.InItemPos();
        Shape[pos.x, pos.y] = null;
        TileCount--;
        if (TileCount == 0)
        {
            Destroy(gameObject);
        }
    }

    public ItemTile GetTile(Vector2Int pos)
    {
        return Shape[pos.x, pos.y];
    }

    void ChangeColor(Color color)
    {
        foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Left Down
        if (Input.GetMouseButtonDown(0) && CanMove && !_moving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Collider2D collider = Physics2D.GetRayIntersection(ray, float.MaxValue, LayerMask.GetMask("Item")).collider;
            _moving = collider != null && collider.gameObject.transform.parent == transform;
            if (_moving)
            {
                _dragStartPosition = transform.position;
                _dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                DraggedItem = this;
            }
        }

        // Left Up
        if (Input.GetMouseButtonUp(0))
        {
            if (_moving)
            {
                if (!OnDropped())
                {
                    transform.position = _dragStartPosition;
                }
                ChangeColor(new Color(1, 1, 1, 1));
                DraggedItem = null;
            }

            _moving = false;
        }

        // Drag button with mouse
        if (_moving)
        {
            ChangeColor(DragOnTrash() ? new Color(1, 0.5f, 0.5f, 0.5f) : new Color(1, 1, 1, 1));
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = transform.position.z;
            transform.position = pos + (Vector3)_dragOffset;
        }
    }
    
    void OnDestroy()
    {
        if (DraggedItem == this)
            DraggedItem = null;
    }

    GameObject DragOnTrash()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Collider2D collider = Physics2D.GetRayIntersection(ray, distance: float.MaxValue, layerMask: LayerMask.GetMask("Trash")).collider;
        if (collider != null && collider.gameObject.CompareTag("Trash"))
            return collider.gameObject;
        return null;
    }

    bool OnDropped()
    {
        // TODO return false on out of bounds
        
        if (!InInventory && DragOnTrash())
        {
            Destroy(gameObject);
            return true;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var collider = Physics2D.GetRayIntersection(ray, distance: float.MaxValue, layerMask: LayerMask.GetMask("Inventory")).collider;
        if (collider != null)
        {
            Cell cell = collider.gameObject.GetComponent<Cell>();
            Inventory inventory = cell.transform.parent.GetComponent<Inventory>();
            // TODO shift pos by where we are holding the thing
            if (inventory.CanPlace(this, cell.InInventoryPos, this))
            {
                inventory.PlaceItem(this, cell.InInventoryPos);
                return true;
            }

            // TODO check if we aren't overlapping with inventory grid maybe?
            return false;
        }

        return false;
    }
}
