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

    public static float SimulationTime = 0f;

    private bool _fighting = false;

    public Healthbar Healthbar;

    [SerializeField] public Canvas PauseCanvas;

    [SerializeField] public Canvas GameOverCanvas;

    public bool IsGameOver = false;

    public bool IsPaused = false;

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

        if (IsGameOver && Input.GetKeyDown(KeyCode.Return))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
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
        PauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        if (IsPaused)
            Unpause();
        IsGameOver = true;
        GameOverCanvas.gameObject.SetActive(true);
    }

    public void Unpause()
    {
        if (IsGameOver)
            return;
        IsPaused = false;
        PauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
