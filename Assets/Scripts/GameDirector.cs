using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    [SerializeField]
    public float PosSentiment { get; set; }

    [SerializeField]
    public float NegSentiment { get; set; }

    public Enemy DummyEnemy;

    public float TimeMouseStopped = 0;

    public Vector2 LastMousePosition = Vector2.zero;

    public float TooltipDelay = 0.3f;

    public void StartFight()
    {
        if (_fighting) return;
        _fighting = true;

        EnemyInstance = Instantiate(_enemyPrefab);
        EnemyInstance.transform.position = _enemySpawn.position + Vector3.right * Random.Range(3f, 5f);
        SpriteRenderer spriteRenderer = EnemyInstance.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        spriteRenderer.DOColor(Color.white, 0.5f).SetEase(Ease.OutBack);
        EnemyInstance.transform.DOMoveX(_enemySpawn.position.x, 0.5f).SetEase(Ease.OutBack);

        EnemyInstance.GetComponent<Health>().SetupHealthbar();
        RectTransform rt = EnemyInstance.GetComponent<Health>().Healthbar.GetComponent<RectTransform>();
        rt.DOAnchorPos((Vector2)_enemySpawn.transform.position + Vector2.up * 2f, 0.5f).SetEase(Ease.OutBack);
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

        // Tooltip stuff, should be moved to its own class
        var mousePos = (Vector2)Input.mousePosition;
        if (mousePos == LastMousePosition)
        {
            TimeMouseStopped += Time.deltaTime;
        }
        else
        {
            if (TimeMouseStopped > TooltipDelay)
            {
                if (SimpleTooltip.ActiveTooltip != null)
                    SimpleTooltip.ActiveTooltip.HideTooltip();
            }
            TimeMouseStopped = 0;
        }

        if (TimeMouseStopped > TooltipDelay && TimeMouseStopped - Time.deltaTime < TooltipDelay)
        {
            if (SimpleTooltip.ActiveTooltip != null)
                SimpleTooltip.ActiveTooltip.ShowTooltipForReal();
        }

        LastMousePosition = mousePos;
    }

    public void EndFight()
    {
        if (!_fighting) return;
        _fighting = false;

        EnemyInstance = DummyEnemy;
    }

    // Happens before Start
    private void Awake()
    {
        Time.timeScale = 1f;

        GameDirectorInstance = this;
        WordManagerInstance = FindObjectOfType<WordManager>();
        InventoryInstance = FindObjectOfType<Inventory>();
        ItemManagerInstance = FindObjectOfType<ItemManager>();

        // HERO
        HeroInstance = Instantiate(_heroPrefab);
        HeroInstance.transform.position = _heroSpawn.position;

        // ENEMY
        EnemyInstance = DummyEnemy;
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
        {
            if (EnemyInstance != DummyEnemy)
                EnemyInstance.GetComponent<Health>().Die();
        }

        GUILayout.Label($"PosSentiment: {PosSentiment}");
        GUILayout.Label($"NegSentiment: {NegSentiment}");

        GUILayout.EndVertical();
    }
#endif

    public void Pause()
    {
        IsPaused = true;
        PauseCanvas.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        if (IsPaused)
            Unpause();
        IsGameOver = true;
        GameOverCanvas.gameObject.SetActive(true);

        Health heroHealth = HeroInstance.GetComponent<Health>();
        heroHealth.Healthbar.SetHealth(0, heroHealth.MaxHealth, 0f);
    }

    public void Unpause()
    {
        if (IsGameOver)
            return;
        IsPaused = false;
        PauseCanvas.gameObject.SetActive(false);
    }
}
