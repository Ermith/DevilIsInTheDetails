using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
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

    public static bool IsPaused { get; set; } = false;

    public static float SimulationTime = 0f;

    private bool _fighting = false;

    [SerializeField]
    public ScreenDimmer ScreenDimmer;

    public void StartFight()
    {
        if (_fighting) return;
        _fighting = true;

        EnemyInstance = Instantiate(_enemyPrefab);
        EnemyInstance.transform.position = _enemySpawn.position;
    }

    void Update()
    {
        if (!IsPaused)
        {
            SimulationTime += Time.deltaTime;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
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
        ItemManagerInstance = FindObjectOfType<ItemManager>();

        // HERO
        HeroInstance = Instantiate(_heroPrefab);
        HeroInstance.transform.position = _heroSpawn.position;
    }

    private void Start()
    {
        //StartFight();
    }

#if DEBUG
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("StartFight"))
            StartFight();

        if (GUILayout.Button("EndFight"))
            EndFight();

        GUILayout.EndVertical();
    }
#endif

    public void Pause()
    {
        IsPaused = true;
        ScreenDimmer.gameObject.SetActive(true);
        // stretch it across camera view
        Camera camera = Camera.main;
        ScreenDimmer.gameObject.transform.localScale = new Vector3(
            camera.orthographicSize * camera.aspect * 2,
            camera.orthographicSize * 2,
            1);
        ScreenDimmer.gameObject.transform.position = new Vector3(0, 0, 0);
    }

    public void Unpause()
    {
        IsPaused = false;
        ScreenDimmer.gameObject.SetActive(false);
    }
}
