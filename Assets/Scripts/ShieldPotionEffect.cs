using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPotionEffect : MonoBehaviour, IEffect
{
    public int Slash;
    public int Thrust;
    public int Strike;

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
            args.Effect += () => health.AddBlock(Slash, Thrust, Strike);
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
            var effect = itemTile.gameObject.AddComponent<ShieldPotionEffect>();
            effect.Slash = Slash;
            effect.Thrust = Thrust;
            effect.Strike = Strike;

            itemTile.Item.ChangeFontColor(Color.blue, left:true);
        };
    }
}
