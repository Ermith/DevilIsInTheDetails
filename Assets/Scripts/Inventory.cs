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
        gameObject.name = GameDirector.InventoryName;

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
}
