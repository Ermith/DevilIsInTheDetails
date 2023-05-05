using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int MaxHealth = 100;
    public int Health { get; private set; }

    void Awake()
    {
        Health = MaxHealth;
    }
}