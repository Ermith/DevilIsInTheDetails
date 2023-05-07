using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffect : MonoBehaviour, IEffect
{
    public int Ticks = 3;

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
            args.Effect += () => health.Poison(Ticks);
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

        else if (dir == Vector2.down)
            while (itemTile == null || itemTile == this)
            {
                pos.y -= 1;
                var cell = GameDirector.InventoryInstance.GetCell(pos);
                if (cell == null) return;
                itemTile = cell.ItemTile;
            }

        if (itemTile == null || Ticks <= 1)
            return;

        args.Target = itemTile.transform.position;
        args.Effect += () =>
        {
            var poison = itemTile.gameObject.AddComponent<PoisonEffect>();
            poison.Ticks = Ticks - 1;
            itemTile.Item.ChangeFontColor(Color.green, right:true);
        };
    }
    public string TooltipText => $"Poisons the target for {Ticks} turns.\nPointing left poisons the hero.\nPointing right poisons the enemy.\nPointing up or down transfers this effect to the next item in that direction.";
}
