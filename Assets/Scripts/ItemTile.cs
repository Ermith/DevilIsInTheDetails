using UnityEngine;


[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class ItemTile : MonoBehaviour
{
    public char Letter { get; private set; }
}