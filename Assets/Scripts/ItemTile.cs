using JetBrains.Annotations;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class ItemTile : MonoBehaviour
{
    public char Letter { get; private set; }

    [CanBeNull] public Cell Cell;

    private TextMeshPro _text;

    void Start()
    {
        Letter = GameDirector.WordManagerInstance.GetLetter();
        _text = GetComponentInChildren<TextMeshPro>();
        _text.text = Letter.ToString();
    }

    public void FixLetterRotation()
    {
        _text.transform.up = Vector3.up;
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

    public Vector2Int RotatedInItemPos()
    {
        var pos = transform.position - transform.parent.position;
        var x = (int)Mathf.Round(pos.x);
        var y = (int)Mathf.Round(pos.y);
        if (Mathf.Abs(x - pos.x) > 0.01f || Mathf.Abs(y - pos.y) > 0.01f)
            throw new System.Exception("Relative position of item tile is not integer");
        return new Vector2Int(x, y);
    }

    public Item Item => GetComponentInParent<Item>();

    void OnDestroy()
    {
        if (Item != null)
        {
            Item.TileDestroyed(this);
        }
        if (Cell != null)
        {
            Cell.ItemTile = null;
        }
    }
}