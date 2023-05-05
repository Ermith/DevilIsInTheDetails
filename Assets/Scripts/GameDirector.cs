using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public const string HeroName = "Hero";
    public const string EnemyName = "Enemy";
    public const string InventoryName = "Inventory";
    public const string WordManagerName = "WordManager";

    [SerializeField] private Enemy _enemy;
    [SerializeField] private Hero _hero;
    [SerializeField] private Transform _heroSpawn;
    [SerializeField] private Transform _enemySpawn;

    private bool _fighting = false;

    public void StartFight()
    {
        if (_fighting) return;
        _fighting = true;
    }

    public void EndFight()
    {
        if (!_fighting) return;
        _fighting = false;
    }

    private void Start()
    {
        
    }
}
