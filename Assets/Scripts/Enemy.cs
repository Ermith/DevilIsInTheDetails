using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Health)), RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    public int Damage = 0;
    public float AttackInterval = 1;
    
    Health _health;
    public Health Health => _health ??= GetComponent<Health>();

    SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer => _spriteRenderer ??= GetComponent<SpriteRenderer>();

    private float _lastAttackTime;
    private float _nextAttackTime;

    public Health.DamageType NextDamageType;

    void Start()
    {
        Health.OnDeath += OnDeath;
        PrepareNextAttack();
    }

    void PrepareNextAttack()
    {
        _lastAttackTime = GameDirector.SimulationTime;
        _nextAttackTime = GameDirector.SimulationTime + AttackInterval;
        NextDamageType = (Health.DamageType)Random.Range(0, 3);
    }

    void Update()
    {
        float progress = (GameDirector.SimulationTime - _lastAttackTime) / (_nextAttackTime - _lastAttackTime);
        Health.Healthbar.SetAttack(progress, NextDamageType, 0f);
        if (GameDirector.SimulationTime >= _nextAttackTime)
        {
            AttackHero();
            PrepareNextAttack();
        }
    }

    void AttackHero()
    {
        var hero = GameDirector.HeroInstance;
        hero.GetComponent<Health>().HitBy(Damage, Health.DamageType.Slash, gameObject);
    }

    private void OnDeath(GameObject attacker)
    {
        GameDirector.GameDirectorInstance.EndFight();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(new Vector3(2f, 0f, 1f), 0.8f).SetEase(Ease.InBack));
        sequence.Join(transform.DOLocalMoveY(-1f, 0.8f).SetEase(Ease.InBack));
        sequence.Join(SpriteRenderer.DOColor(Color.red, 0.8f).SetEase(Ease.InBack));
        sequence.Join(Health.Healthbar.Background.DOColor(new Color(1f, 1f, 1f, 0f), 0.8f).SetEase(Ease.InBack));
        sequence.Join(Health.Healthbar.HealthBar.DOColor(new Color(1f, 1f, 1f, 0f), 0.8f).SetEase(Ease.InBack));
        sequence.Join(Health.Healthbar.DamageBar.DOColor(new Color(1f, 1f, 1f, 0f), 0.8f).SetEase(Ease.InBack));
        sequence.Join(Health.Healthbar.Text.DOColor(new Color(1f, 1f, 1f, 0f), 0.8f).SetEase(Ease.InBack));
        sequence.AppendInterval(0.2f);
        sequence.AppendCallback(() =>
        {
            Destroy(gameObject);
            GameDirector.GameDirectorInstance.StartFight();
        });
        sequence.Play();
    }
}