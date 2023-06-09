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
        float karma = GameDirector.GameDirectorInstance.NormalizedKarma;
        Letter = GameDirector.WordManagerInstance.GetLetter(karma);
    }

    public void FixLetterRotation()
    {
        Text.transform.up = Vector3.up;
    }

    public void InitTooltip(string itemTooltip)
    {
        SimpleTooltip tooltip = gameObject.GetComponent<SimpleTooltip>() ?? gameObject.AddComponent<SimpleTooltip>();
        string tooltipText = itemTooltip;
        tooltipText += "\n";
        if (Letter == '#')
            tooltipText += "This tile is super duper cursed and cannot be used in any words. Oh no.";
        else if (Letter == '\0')
            tooltipText += "This tile is not cursed at all. It will be used as soon as it is placed.";
        else
            tooltipText += "This tile contains letter " + Letter + ". Form a 4+ letter English word to use it.";
        tooltip.infoLeft = tooltipText;
    }

    private Vector2Int? _inItemPos;
    
    public Vector2Int InItemPos()
    {
        if (_inItemPos != null)
            return _inItemPos.Value;
        
        var pos = transform.localPosition;
        var x = (int)Mathf.Round(pos.x);
        var y = (int)Mathf.Round(pos.y);
        if (x < 0 || y < 0)
            throw new System.Exception("Relative position of item tile is negative");
        if (Mathf.Abs(x - pos.x) > 0.01f || Mathf.Abs(y - pos.y) > 0.01f)
            throw new System.Exception("Relative position of item tile is not integer");
        _inItemPos = new Vector2Int(x, y);
        return _inItemPos.Value;
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

    public void UseAndDestroy()
    {
        var letterAnim = GetComponentInChildren<LetterAnimation>();
        var effectArgs = new EffectArgs();
        effectArgs.ItemTile = this;
        Item.gameObject.BroadcastMessage("ExecuteEffect", effectArgs, SendMessageOptions.DontRequireReceiver);
        float throwTime = 0.5f;

        letterAnim.PlayDisappearing(() =>
        {
            transform.DOJump(effectArgs.Target, 2, 1, throwTime).Join(
                Rotation(throwTime)).OnComplete(() =>
                {
                    effectArgs.Effect?.Invoke();
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

    private void OnDestroy()
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