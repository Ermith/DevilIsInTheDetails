using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class EffectParams
{
    public bool Heal;
    public float HealAmount;

    public bool Damage;
    public float DamageAmount;
}

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    public EffectParams EffectParams;

    [HideInInspector]
    public Inventory Inventory;

    [HideInInspector]
    public bool CanMove = true;

    private bool _moving;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Left Down
        if(Input.GetMouseButtonDown(0) && CanMove)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Collider2D collider = Physics2D.GetRayIntersection(ray).collider;
            _moving = collider != null && collider.gameObject == gameObject;
        }

        // Left Up
        if (Input.GetMouseButtonUp(0))
        {
            _moving = false;
        }

        // Drag button with mouse
        if (_moving)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }
}
