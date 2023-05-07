using DG.Tweening;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public UDictionary<Enemy, int> EnemySpawnWeights;

    [SerializeField] private Transform _enemySpawn;

    private Enemy PickEnemy()
    {
        int totalWeight = 0;
        foreach (var enemy in EnemySpawnWeights)
        {
            totalWeight += enemy.Value;
        }

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var enemy in EnemySpawnWeights)
        {
            randomWeight -= enemy.Value;
            if (randomWeight <= 0)
            {
                return enemy.Key;
            }
        }

        return null;
    }

    public Enemy SpawnEnemy()
    {
        Enemy enemy = Instantiate(PickEnemy());
        
        enemy.transform.position = _enemySpawn.position + Vector3.right * Random.Range(3f, 5f);
        SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
        var color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;

        spriteRenderer.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
        enemy.transform.DOMoveX(_enemySpawn.position.x, 0.5f).SetEase(Ease.OutBack);

        enemy.GetComponent<Health>().SetupHealthbar();
        RectTransform rt = enemy.GetComponent<Health>().Healthbar.GetComponent<RectTransform>();
        rt.DOAnchorPos((Vector2)_enemySpawn.transform.position + Vector2.up * 2f, 0.5f).SetEase(Ease.OutBack);

        return enemy;
    }
}