using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    //public const string HeroName = "Hero";
    //public const string EnemyName = "Enemy";
    //public const string InventoryName = "Inventory";
    //public const string WordManagerName = "WordManager";
    //public const string GameDirectorName = "GameDirector";
    
    [SerializeField] private Hero _heroPrefab;
    [SerializeField] private Transform _heroSpawn;

    public static Hero HeroInstance { get; private set; }
    public static Enemy EnemyInstance { get; private set; }
    public static WordManager WordManagerInstance { get; private set; }
    public static GameDirector GameDirectorInstance { get; private set; }
    public static Inventory InventoryInstance { get; private set; }
    public static ItemManager ItemManagerInstance { get; private set; }
    public static EnemyManager EnemyManagerInstance { get; private set; }
    public static KarmaBar KarmaBarInstance { get; private set; }
    public static AudioManager AudioManagerInstance { get; private set; }

    public static float SimulationTime = 0f;

    private bool _fighting = false;

    public Healthbar Healthbar;

    [SerializeField] public Canvas PauseCanvas;

    [SerializeField] public Canvas GameOverCanvas;

    public bool IsGameOver = false;

    public bool IsPaused = false;

    private float _posSentiment;
    private float _negSentiment;

    [SerializeField]
    public float PosSentiment
    {
        get => _posSentiment;
        set
        {
            _posSentiment = value;
            KarmaBarInstance.OnKarmaChange();
        }
    }

    [SerializeField]
    public float NegSentiment
    {
        get => _negSentiment;
        set
        {
            _negSentiment = value;
            KarmaBarInstance.OnKarmaChange();
        }
    }

    public Enemy DummyEnemy;

    public float TimeMouseStopped = 0;

    public Vector2 LastMousePosition = Vector2.zero;

    public float TooltipDelay = 0.3f;

    public float KarmaFactor = 0.2f;

    public float Karma => Math.Clamp((PosSentiment - NegSentiment) * KarmaFactor, -1f, 1f);

    public YeetableText YeetableTextPrefab;

    public int EnemiesDefeated = 0;

    public List<(string, float)> WordsMatched = new();

    public TextMeshProUGUI PauseStats;
    public TextMeshProUGUI GameOverStats;

    public void StartFight()
    {
        if (_fighting) return;
        _fighting = true;
        EnemyInstance = EnemyManagerInstance.SpawnEnemy();
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
            SceneManager.LoadScene("Game");
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

    public void UpdateStatsText()
    {
        string text = $"Enemies Defeated: {EnemiesDefeated}\n";
        text += $"Words Matched: {WordsMatched.Count}\n";
        text += $"Karma: {Karma}\n";

        PauseStats.text = text;
        GameOverStats.text = text;
    }

    public void EndFight()
    {
        if (!_fighting) return;
        _fighting = false;

        EnemiesDefeated++;
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
        EnemyManagerInstance = FindObjectOfType<EnemyManager>();
        KarmaBarInstance = FindObjectOfType<KarmaBar>();
        AudioManagerInstance = FindObjectOfType<AudioManager>();

        // HERO
        HeroInstance = Instantiate(_heroPrefab);
        HeroInstance.transform.position = _heroSpawn.position;

        // ENEMY
        EnemyInstance = DummyEnemy;
    }

    private void Start()
    {
        StartFight();
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
        GUILayout.Label($"Karma: {Karma}");

        GUILayout.EndVertical();
    }
#endif

    public void Pause()
    {
        IsPaused = true;
        PauseCanvas.gameObject.SetActive(true);
        UpdateStatsText();
    }

    public void GameOver()
    {
        if (IsPaused)
            Unpause();
        UpdateStatsText();
        IsGameOver = true;
        GameOverCanvas.gameObject.SetActive(true);
    }

    public void Unpause()
    {
        if (IsGameOver)
            return;
        IsPaused = false;
        PauseCanvas.gameObject.SetActive(false);
    }
}
