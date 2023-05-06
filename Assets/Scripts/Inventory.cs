using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int Width;
    public int Height;
    public Cell Cell;

    private List<List<Cell>> _grid;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = GameDirector.InventoryName;

        _grid = new List<List<Cell>>();
        for (int y = 0; y < Height; y++)
        {
            var row = new List<Cell>();
            for (int x = 0; x < Width; x++)
            {
                Cell cell = Instantiate(Cell);
                cell.InInventoryPos = new Vector2Int(x, y);
                Bounds bounds = cell.GetComponent<SpriteRenderer>().sprite.bounds;
                float width = bounds.size.x;
                float height = bounds.size.y;
                cell.Inventory = this;
                cell.transform.parent = transform;
                cell.transform.localPosition = (Vector2.right * width * x) + (Vector2.up * height * y);
                row.Add(cell);
            }
            _grid.Add(row);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return null;
        return _grid[y][x];
    }

    public Cell GetCell(Vector2Int pos)
    {
        return GetCell(pos.x, pos.y);
    }

    public bool CanPlace(Item item, Vector2Int pos, Item excludeItem = null)
    {
        for (int y = 0; y < item.Height; y++)
        {
            for (int x = 0; x < item.Width; x++)
            {
                if (item.Shape[x, y] != null)
                {
                    var cell = GetCell(pos.x + x, pos.y + y);
                    if (cell == null || cell.ItemTile != null && (excludeItem == null || cell.ItemTile.Item != excludeItem))
                        return false;
                }
            }
        }
        return true;
    }

    public void PlaceItem(Item item, Vector2Int pos)
    {
        for (int y = 0; y < item.Height; y++)
        {
            for (int x = 0; x < item.Width; x++)
            {
                if (item.Shape[x, y] != null)
                {
                    var cell = GetCell(pos.x + x, pos.y + y);
                    cell.ItemTile = item.Shape[x, y];
                }
            }
        }

        item.InInventory = true;
        item.transform.parent = transform;
        item.transform.localPosition = (Vector2)pos;
    }
}
