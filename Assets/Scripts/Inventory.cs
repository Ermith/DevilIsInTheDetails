using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class GridRow
{
    public List<Cell> Cells = new List<Cell>();

    public GridRow(int width)
    {
        for (int i = 0; i < width; i++)
        {
            Cells.Add(null);
        }
    }

    public GridRow(List<Cell> cells)
    {
        Cells = cells;
    }
}

public class Inventory : MonoBehaviour
{
    public int Width;
    public int Height;
    public Cell Cell;

    [SerializeField]
    private List<GridRow> _grid;

    // Start is called before the first frame update
    void Start()
    {
        _grid = new ();
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
            _grid.Add(new GridRow(row));
        }
    }

    // Update is called once per frame
    void Update()
    {
        Item item = Item.DraggedItem;
        if (item == null)
        {
            HighlightDrag(null, Vector2Int.zero);
        }
        else
        {
            var (_, pos) = item.GetPlaceData();
            if (pos == null)
            {
                HighlightDrag(null, Vector2Int.zero);
            }
            else
            {
                HighlightDrag(item, pos.Value);
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return null;
        return _grid[y].Cells[x];
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
                    Vector2Int tileRelPos = item.Shape[x, y].RotatedInItemPos();
                    var cell = GetCell(tileRelPos + pos);
                    if (cell == null || cell.ItemTile != null && (excludeItem == null || cell.ItemTile.Item != excludeItem))
                        return false;
                }
            }
        }
        return true;
    }

    public IEnumerable<Cell> GetCells()
    {
        foreach (var row in _grid)
        {
            foreach (var cell in row.Cells)
            {
                yield return cell;
            }
        }
    }

    public void HighlightDrag(Item item, Vector2Int pos)
    {
        foreach (Cell cell in GetCells())
        {
            cell.RefreshColor();
        }
        
        if(item == null)
            return;
        
        for (int y = 0; y < item.Height; y++)
        {
            for (int x = 0; x < item.Width; x++)
            {
                if (item.Shape[x, y] != null)
                {
                    Vector2Int tileRelPos = item.Shape[x, y].RotatedInItemPos();
                    var cell = GetCell(tileRelPos + pos);
                    if (cell != null)
                        cell.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.7f, 0.5f);
                }
            }
        }
    }

    public void RemoveItem(Item item)
    {
        if (!item.InInventory)
            throw new System.Exception("Item is not in inventory");
        foreach (ItemTile tile in item.GetTiles())
        {
            tile.Cell.ItemTile = null;
        }
    }

    public void PlaceItem(Item item, Vector2Int pos)
    {
        if (item.InInventory)
            RemoveItem(item);

        for (int y = 0; y < item.Height; y++)
        {
            for (int x = 0; x < item.Width; x++)
            {
                ItemTile tile = item.Shape[x, y];
                if (tile != null)
                {
                    Vector2Int tileRelPos = tile.RotatedInItemPos();
                    var cell = GetCell(tileRelPos + pos);
                    cell.ItemTile = tile;
                    tile.Cell = cell;
                }
            }
        }

        if (!item.InInventory)
            GameDirector.ItemManagerInstance.LooseItems--;
        item.InInventory = true;
        item.transform.parent = transform;
        item.transform.localPosition = (Vector2)pos;

        ItemPlaced();
    }

    private void ItemPlaced()
    {
        foreach (Cell cell in GetActivations())
        {
            cell.ItemTile.UseAndDestroy();
        }
    }

    public bool WordAt(Vector2Int pos, Vector2Int dir, [NotNullWhen(true)] out string word, [NotNullWhen(true)] out List<Cell> cells)
    {
        word = null;
        string currentWord = "";
        cells = null;
        List<Cell> currentCells = new();
        Cell cell = GetCell(pos);
        while (cell != null && cell.ItemTile != null && cell.ItemTile.Letter != '\0' && cell.ItemTile.Letter != '#')
        {
            currentWord += cell.ItemTile.Letter;
            currentCells.Add(cell);
            if (GameDirector.WordManagerInstance.IsWord(currentWord))
            {
                cells = currentCells.ToList();
                word = currentWord;
            }

            pos += dir;
            cell = GetCell(pos);
        }

        return word != null;
    }

    public List<Cell> GetActivations()
    {
        List<Cell> result = new();
        foreach (Cell cell in GetCells())
        {
            if (cell.ItemTile?.Letter == '\0')
            {
                result.Add(cell);
                Debug.Log($"Free tile at {cell.InInventoryPos}");
                continue;
            }
            foreach (Vector2Int dir in new[] { Vector2Int.down, Vector2Int.right })
            {
                if (WordAt(cell.InInventoryPos, dir, out string word, out List<Cell> cells))
                {
                    result.AddRange(cells);
                    Debug.Log($"Word completed: {word}");
                    (float pos, float neg) = GameDirector.WordManagerInstance.GetSentiment(word);

                    float beforeKarma = GameDirector.GameDirectorInstance.Karma;

                    GameDirector.GameDirectorInstance.PosSentiment += pos;
                    GameDirector.GameDirectorInstance.NegSentiment += neg;

                    float karmaDelta = GameDirector.GameDirectorInstance.Karma - beforeKarma;
                    if (karmaDelta != 0)
                    {
                        Vector3 startPos = cells[cells.Count / 2].transform.position;
                        var karmaRect = GameDirector.KarmaBarInstance.GetComponent<RectTransform>();
                        Vector3 endPos = karmaRect.anchoredPosition;

                        string text = word + "\n";
                        if (karmaDelta > 0)
                            text += $"+{(int)(karmaDelta * 1000)}";
                        else
                            text += $"{(int)(karmaDelta * 1000)}";

                        var color = karmaDelta > 0 ? Color.blue : Color.red;

                        YeetableText.Yeet(text, color, startPos,endPos, 1.5f);
                    }
                }
            }
        }

        // deduplicate
        result = result.Distinct().ToList();
        return result;
    }
}