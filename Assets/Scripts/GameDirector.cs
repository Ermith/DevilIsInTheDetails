using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    //public const string HeroName = "Hero";
    //public const string EnemyName = "Enemy";
    //public const string InventoryName = "Inventory";
    //public const string WordManagerName = "WordManager";
    //public const string GameDirectorName = "GameDirector";

    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Hero _heroPrefab;
    [SerializeField] private Transform _heroSpawn;
    [SerializeField] private Transform _enemySpawn;

    public static Hero HeroInstance { get; private set; }
    public static Enemy EnemyInstance { get; private set; }
    public static WordManager WordManagerInstance { get; private set; }
    public static GameDirector GameDirectorInstance { get; private set; }
    public static Inventory InventoryInstance { get; private set; }

    public static ItemManager ItemManagerInstance { get; private set; }

    private bool _fighting = false;

    public void StartFight()
    {
        if (_fighting) return;
        _fighting = true;

        EnemyInstance = Instantiate(_enemyPrefab);
        EnemyInstance.transform.position = _enemySpawn.position;
    }

    public void EndFight()
    {
        if (!_fighting) return;
        _fighting = false;

        Destroy(EnemyInstance.gameObject);
        EnemyInstance = null;
    }

    // Happens before Start
    private void Awake()
    {
        GameDirectorInstance = this;
        WordManagerInstance = FindObjectOfType<WordManager>();
        InventoryInstance = FindObjectOfType<Inventory>();

        // HERO
        HeroInstance = Instantiate(_heroPrefab);
        HeroInstance.transform.position = _heroSpawn.position;
    }

    private void Start()
    {
        //StartFight();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("StartFight"))
            StartFight();

        if (GUILayout.Button("EndFight"))
            EndFight();

        GUILayout.EndVertical();
    }
}
