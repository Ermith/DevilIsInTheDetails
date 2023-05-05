using UnityEngine;

class Trash : MonoBehaviour
{
    // Path: Assets\Scripts\Trash.cs
    void OnTriggerEnter2D(Collider2D other)
    {
        // Path: Assets\Scripts\Trash.cs
        if (other.gameObject.CompareTag("Player"))
        {
            // Path: Assets\Scripts\Trash.cs
            Destroy(gameObject);
        }
    }
}