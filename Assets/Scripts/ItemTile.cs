using UnityEngine;


[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class ItemTile : MonoBehaviour
{
    public char Letter { get; private set; }

    void Start()
    {
        WordManager wordManager = GameObject.Find(GameDirector.WordManagerName).GetComponent<WordManager>();
        Letter = wordManager.GetLetter();
    }

    public Vector2Int InItemPos()
    {
        var pos = transform.localPosition;
        var x = (int)Mathf.Round(pos.x);
        var y = (int)Mathf.Round(pos.y);
        if (x < 0 || y < 0)
            throw new System.Exception("Relative position of item tile is negative");
        if (Mathf.Abs(x - pos.x) > 0.01f || Mathf.Abs(y - pos.y) > 0.01f)
            throw new System.Exception("Relative position of item tile is not integer");
        return new Vector2Int(x, y);
    }

    void OnDestroy()
    {
        Item item = GetComponentInParent<Item>();
        if (item != null)
        {
            item.TileDestroyed(this);
        }
    }
}