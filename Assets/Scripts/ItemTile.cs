using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(TextMeshPro))]
public class ItemTile : MonoBehaviour
{
    public char Letter
    {
        get => _letter;
        set
        {
            _letter = value;
            Text.text = Letter.ToString();
        }
    }

    [CanBeNull] public Cell Cell;
    
    [SerializeField]
    private char _letter;

    private TextMeshPro _textB;
    private TextMeshPro Text => _textB ??= GetComponentInChildren<TextMeshPro>();

    void Start()
    {
        Letter = GameDirector.WordManagerInstance.GetLetter();
    }

    public void FixLetterRotation()
    {
        Text.transform.up = Vector3.up;
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

    public void UseAndDestroy(Vector3 target)
    {
        var letterAnim = GetComponentInChildren<LetterAnimation>();
        transform.parent = null;
        float throwTime = 0.5f;

        if (Item != null)
        {
            Item.TileDestroyed(this);
        }
        if (Cell != null)
        {
            Cell.ItemTile = null;
        }

        letterAnim.PlayDisappearing(() =>
        {
            transform.DOJump(target, 2, 1, throwTime).Join(
                Rotation(throwTime)).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        });
    }

    private Tween Rotation(float time, int iterations = 4)
    {
        Vector3 rotation = new Vector3(0, 0, 90);
        float iterationTime = time / iterations;
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < iterations; i++)
        {
            s.Append(transform.DORotate(rotation, iterationTime));
            rotation.z += 90;
        }

        return s.SetEase(Ease.InExpo);
    }

}