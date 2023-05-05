using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    [HideInInspector]
    public Inventory Inventory;

    [HideInInspector]
    public bool CanMove = true;

    private bool _moving;

    private Vector2 _dragStartPosition;

    private Vector2 _dragOffset;

    public bool InInventory { get; private set; }

    [CanBeNull] public static Item DraggedItem;

    public void Execute()
    {
        gameObject.BroadcastMessage("ItemExecute");
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Left Down
        if (Input.GetMouseButtonDown(0) && CanMove && !_moving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Collider2D collider = Physics2D.GetRayIntersection(ray).collider;
            _moving = collider != null && collider.gameObject == gameObject;
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
                if (InInventory)
                {
                    transform.position = _dragStartPosition;
                }
                else
                {
                    OnDropped();
                }
                spriteRenderer.color = new Color(1, 1, 1, 1);
                DraggedItem = null;
            }

            _moving = false;
        }

        // Drag button with mouse
        if (_moving)
        {
            spriteRenderer.color = DragOnTrash() ? new Color(1, 0.5f, 0.5f, 0.5f) : new Color(1, 1, 1, 1);
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

    void OnDropped()
    {
        if (DragOnTrash())
        {
            Destroy(gameObject);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var collider = Physics2D.GetRayIntersection(ray, distance: float.MaxValue, layerMask: LayerMask.GetMask("Inventory")).collider;
        if (collider != null)
        {

        }
    }
}
