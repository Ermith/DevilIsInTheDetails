using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int Width;
    public int Height;
    public Cell Cell;

    private List<List<Cell>> _list;

    // Start is called before the first frame update
    void Start()
    {
        _list = new List<List<Cell>>();
        for (int y = 0; y < Height; y++)
        {
            var list = new List<Cell>();
            for (int x = 0; x < Width; x++)
            {
                Cell cell = Instantiate(Cell);
                Bounds bounds = cell.GetComponent<SpriteRenderer>().sprite.bounds;
                float width = bounds.size.x;
                float height = bounds.size.y;
                cell.Inventory = this;
                cell.transform.parent = transform;
                cell.transform.position = (Vector2.right * width * (x - Width / 2)) + (Vector2.down * height * (y - Height / 2));
                list.Add(cell);
            }
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
        return _list[y][x];
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
        item.transform.position = (Vector2.right * 0.5f * (item.Width - 1)) + (Vector2.down * 0.5f * (item.Height - 1));
    }
}
