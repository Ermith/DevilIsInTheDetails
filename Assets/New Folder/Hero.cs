using UnityEngine;

class Hero : MonoBehaviour
{
    public int MaxHealth = 100;
    public int Health { get; private set; }

    private void Awake()
    {
        Health = MaxHealth;
    }
}