using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class HealEffect : MonoBehaviour, IEffect
{
    public int HealAmount = 20;
    public void ExecuteEffect(EffectArgs args)
    {
        Vector2 dir = GetComponent<Item>().transform.up;
        Health health = null;
        var pos = args.ItemTile.Cell.InInventoryPos;

        if (dir == Vector2.right)
            health = GameDirector.EnemyInstance.GetComponent<Health>();

        if (dir == Vector2.left)
            health = GameDirector.HeroInstance.GetComponent<Health>();

        if (health != null)
        {
            args.Target = health.transform.position;
            args.Effect += () => health.HealBy(HealAmount, gameObject);
            return;
        }

        ItemTile itemTile = null;

        if (dir == Vector2.up)
            while (itemTile == null || itemTile == this)
            {
                pos.y += 1;
                var cell = GameDirector.InventoryInstance.GetCell(pos);
                if (cell == null) return;
                itemTile = cell.ItemTile;
            }

        if (dir == Vector2.down)
            while (itemTile == null || itemTile == this)
            {
                pos.y -= 1;
                var cell = GameDirector.InventoryInstance.GetCell(pos);
                if (cell == null) return;
                itemTile = cell.ItemTile;
            }

        if (itemTile == null)
            return;


        args.Target = itemTile.transform.position;
        args.Effect += () =>
        {
            itemTile.gameObject.AddComponent<HealEffect>().HealAmount = HealAmount / 2;
            itemTile.Item.ChangeFontColor(Color.red, outline: true);
        };
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
