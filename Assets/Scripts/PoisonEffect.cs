using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffect : MonoBehaviour, IEffect
{
    public int PoisonDamage = 5;
    public int Ticks = 3;
    public float Interval = 1f;

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
            args.Effect += () => health.Poison(PoisonDamage, Ticks, Interval);
            return;
        }

        ItemTile itemTile = null;

        if (dir == Vector2.up)
            while (itemTile != null && itemTile != this)
            {
                pos.y += 1;
                var cell = GameDirector.InventoryInstance.GetCell(pos);
                if (cell == null) return;
                itemTile = cell.ItemTile;
            }

        if (dir == Vector2.down)
            while (itemTile != null && itemTile != this)
            {
                pos.y -= 1;
                var cell = GameDirector.InventoryInstance.GetCell(pos);
                if (cell == null) return;
                itemTile = cell.ItemTile;
            }

        if (itemTile == null || Ticks <= 1)
            return;

        var poison = itemTile.gameObject.AddComponent<PoisonEffect>();

        args.Effect += () =>
        {
            poison.PoisonDamage = PoisonDamage;
            poison.Ticks = Ticks - 1;
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
