using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    [HideInInspector]
    public bool CanMove = true;

    private bool _moving;

    private Vector2 _dragStartPosition;

    private Quaternion _dragStartRotation;

    [SerializeField]
    private GameObject _devilPrefab;

    [CanBeNull] private ItemTile _draggedTile;

    private Vector2 _dragOffset;

    [ItemCanBeNull] private ItemTile[,] _shape;

    public string TooltipText;

    public ItemTile[,] Shape
    {
        get
        {
            if (_shape == null)
            {
                InitShape();
            }

            return _shape;
        }
    }

    public int Width => Shape.GetLength(0);

    public int Height => Shape.GetLength(1);

    public int TileCount { get; private set; }

    public bool InInventory { get; set; }

    [CanBeNull] public static Item DraggedItem;

    public void Execute()
    {
    }

    public void Start()
    {
        GameDirector.ItemManagerInstance.LooseItems++;

        // call InitTooltips with a delay
        // this is needed because the item tiles are not initialized yet and I'm too lazy to do it properly
        StartCoroutine(InitTooltipsDelayed());
    }

    IEnumerator InitTooltipsDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        InitTooltips();
    }

    void InitTooltips()
    {
        string tooltipText = TooltipText;
        foreach (IEffect effect in GetComponentsInChildren<IEffect>())
        {
            tooltipText += "\n" + effect.TooltipText;
        }
        foreach (ItemTile tile in GetTiles())
        {
            tile.InitTooltip(tooltipText);
        }
    }

    void InitShape()
    {
        int width = 0;
        int height = 0;
        foreach (var tile in GetComponentsInChildren<ItemTile>())
        {
            var pos = tile.InItemPos();
            width = Mathf.Max(width, pos.x + 1);
            height = Mathf.Max(height, pos.y + 1);
        }

        TileCount = 0;
        _shape = new ItemTile[width, height];
        foreach (var tile in GetComponentsInChildren<ItemTile>())
        {
            var pos = tile.InItemPos();
            Shape[pos.x, pos.y] = tile;
            TileCount++;
        }
    }

    [Flags]
    public enum FontColoring
    {
        left = 1,
        right = 2,
        outline = 4
    }

    public void ChangeFontColor(Color color, bool left = false, bool right = false, bool outline = false)
    {
        foreach (var text in GetComponentsInChildren<TextMeshPro>())
        {
            Color rightColor = right ? color : text.colorGradient.topRight;
            Color leftColor = left ? color : text.colorGradient.topLeft;
            Color outlineColor = outline ? color : text.outlineColor;

            var gradient = new VertexGradient(leftColor, rightColor, leftColor, rightColor);
            text.colorGradient = gradient;
            text.outlineColor = outlineColor;
        }
    }
    public void TileDestroyed(ItemTile tile)
    {
        var pos = tile.InItemPos();
        Shape[pos.x, pos.y] = null;
        TileCount--;
        if (TileCount == 0)
        {
            Destroy(gameObject);
        }
    }

    public ItemTile GetTile(Vector2Int pos)
    {
        return Shape[pos.x, pos.y];
    }

    void ChangeColor(Color color)
    {
        foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Left Down
        if (Input.GetMouseButtonDown(0) && CanMove && !_moving && !GameDirector.GameDirectorInstance.IsPaused)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Collider2D collider = Physics2D.GetRayIntersection(ray, float.MaxValue, LayerMask.GetMask("Item")).collider;
            _moving = collider != null && collider.gameObject.transform.parent == transform;
            if (_moving)
            {
                _dragStartPosition = transform.position;
                _dragStartRotation = transform.rotation;
                _dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                DraggedItem = this;
                _draggedTile = collider.gameObject.GetComponentInParent<ItemTile>();
            }
        }

        // Left Up
        if (Input.GetMouseButtonUp(0) || GameDirector.GameDirectorInstance.IsPaused)
        {
            if (_moving)
            {
                if (!OnDropped())
                {
                    transform.position = _dragStartPosition;
                    transform.rotation = _dragStartRotation;
                    FixLetterRotation();
                }
                ChangeColor(new Color(1, 1, 1, 1));
                DraggedItem = null;
                _draggedTile = null;
            }

            _moving = false;
        }

        // Drag button with mouse
        if (_moving)
        {
            ChangeColor(DragOnTrash() ? new Color(1, 0.5f, 0.5f, 0.5f) : new Color(1, 1, 1, 1));
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = transform.position.z;
            transform.position = pos + (Vector3)_dragOffset;

            if (Input.GetMouseButtonDown(1))
            {
                GameDirector.GameDirectorInstance.DoesPlayerKnowTheyCanRotateStuff = true;
                Rotate(left: false);
            }
        }
    }

    public void Rotate(bool left)
    {
        // rotate around _dragOffset position
        transform.RotateAround(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, left ? 90 : -90);
        _dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        FixLetterRotation();
    }

    public void FixLetterRotation()
    {
        foreach (ItemTile tile in GetTiles())
        {
            tile.FixLetterRotation();
        }
    }

    void OnDestroy()
    {
        if (DraggedItem == this)
            DraggedItem = null;
        if (!InInventory)
            GameDirector.ItemManagerInstance.LooseItems--;
    }

    GameObject DragOnTrash()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Collider2D collider = Physics2D.GetRayIntersection(ray, distance: float.MaxValue, layerMask: LayerMask.GetMask("Trash")).collider;
        if (collider != null && collider.gameObject.CompareTag("Trash"))
            return collider.gameObject;
        return null;
    }

    public IEnumerable<ItemTile> GetTiles()
    {
        foreach (var tile in Shape)
        {
            if (tile != null)
                yield return tile;
        }
    }

    public (Cell?, Vector2Int?) GetPlaceData()
    {
        // ray from the _draggedTile
        Ray ray = new Ray(_draggedTile.transform.position + new Vector3(0, 0, 1), Vector3.back);
        var collider = Physics2D.GetRayIntersection(ray, distance: float.MaxValue, layerMask: LayerMask.GetMask("Inventory")).collider;
        if (collider != null)
        {
            Cell cell = collider.gameObject.GetComponent<Cell>();
            Inventory inventory = cell.transform.parent.GetComponent<Inventory>();
            var placePos = cell.InInventoryPos - _draggedTile.RotatedInItemPos();
            if (inventory.CanPlace(this, placePos, this))
                return (cell, placePos);
            return (cell, null);
        }

        return (null, null);
    }

    bool OnDropped()
    {
        if (!InInventory && DragOnTrash())
        {
            Destroy(gameObject);
            return true;
        }

        var (cell, placePos) = GetPlaceData();
        if (placePos != null)
        {
            cell.Inventory.PlaceItem(this, placePos.Value);

            return true;
        }

        // don't place on a cell if we don't fit there
        if (cell != null)
            return false;

        // if out of camera view
        if (!InCameraView())
        {
            return false;
        }

        // can't move out of inventory once inside
        if (InInventory)
            return false;

        return true;
    }

    public void PlayAppearing()
    {
        string clip = "DevilShake";
        var devilAnimation = Instantiate(_devilPrefab).GetComponent<Animation>();

        int r = UnityEngine.Random.Range(0, devilAnimation.GetClipCount());
        GameDirector.AudioManagerInstance.Play($"DevilLaugh{r + 1}");
        foreach (AnimationState c in devilAnimation)
            if (r-- == 0) { clip = c.name; break; }

        devilAnimation.transform.position = transform.position + Vector3.right * 2;
        devilAnimation.Play(clip);

        StartCoroutine(devilAnimation.OnComplete(clip, () =>
        {
            foreach (var anim in GetComponentsInChildren<LetterAnimation>())
                anim.PlayAppearing();

            Destroy(devilAnimation.gameObject);
        }));
    }

    public bool OverlapsAnyCell()
    {
        foreach (var tile in GetTiles())
        {
            Ray ray = new Ray(tile.transform.position + new Vector3(0, 0, 1), Vector3.back);
            var collider = Physics2D.GetRayIntersection(ray, distance: float.MaxValue, layerMask: LayerMask.GetMask("Inventory")).collider;
            if (collider != null)
                return true;
        }
        return false;
    }

    public bool InCameraView()
    {
        Camera camera = Camera.main;
        Vector3 viewPos = camera.WorldToViewportPoint(transform.position - transform.rotation * new Vector3(0.5f, 0.5f, 0));
        if (!(viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1))
            return false;
        viewPos = camera.WorldToViewportPoint(transform.position + transform.rotation * new Vector3(Width - 0.5f, Height - 0.5f, 0));
        if (!(viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1))
            return false;
        return true;
    }
}
