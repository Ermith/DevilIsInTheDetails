using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Health)), RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    public int Damage = 0;
    public float AttackInterval = 1;
    public float AttackSlashWeight = 1f;
    public float AttackStrikeWeight = 1f;
    public float AttackThrustWeight = 1f;
    public float AttackPoisonWeight = 0f;
    public float BlockChance = 0.5f;
    public float BlockSlashWeight = 1f;
    public float BlockStrikeWeight = 1f;
    public float BlockThrustWeight = 1f;
    public int BlockAmount = 5;

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
        Health.OnHit += OnHit;
        PrepareNextAttack();
    }

    private void OnHit(int damage, Health.DamageType type, GameObject attacker)
    {
        GameDirector.AudioManagerInstance.Play("HammerHit");
    }

    void PrepareNextAttack()
    {
        _lastAttackTime = GameDirector.SimulationTime;
        _nextAttackTime = GameDirector.SimulationTime + AttackInterval;
        
        {
            float r = Random.value * (AttackSlashWeight + AttackStrikeWeight + AttackThrustWeight + AttackPoisonWeight);
            if (r < AttackSlashWeight)
            {
                NextDamageType = Health.DamageType.Slash;
            }
            else if (r < AttackSlashWeight + AttackStrikeWeight)
            {
                NextDamageType = Health.DamageType.Strike;
            }
            else if (r < AttackSlashWeight + AttackStrikeWeight + AttackThrustWeight)
            {
                NextDamageType = Health.DamageType.Thrust;
            }
            else
            {
                NextDamageType = Health.DamageType.Poison;
            }
        }

        if (Health.TotalBlock == 0 && Random.value < BlockChance)
        {
            float r = Random.value * (BlockSlashWeight + BlockStrikeWeight + BlockThrustWeight);
            Health.DamageType blockType;
            if (r < BlockSlashWeight)
            {
                blockType = Health.DamageType.Slash;
            }
            else if (r < BlockSlashWeight + BlockStrikeWeight)
            {
                blockType = Health.DamageType.Strike;
            }
            else
            {
                blockType = Health.DamageType.Thrust;
            }
            Health.AddBlock(BlockAmount, blockType);
        }
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
        GameDirector.AudioManagerInstance.Play("SwordHit");

        // animate slight movement to the left
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveX(transform.position.x + Damage / 10f, 0.1f).SetEase(Ease.OutCubic));
        sequence.Append(transform.DOMoveX(transform.position.x - Damage / 4f, 0.1f).SetEase(Ease.OutCubic));
        sequence.Append(transform.DOMoveX(transform.position.x, 0.1f).SetEase(Ease.InCubic));
    }

    private void OnDeath(GameObject attacker)
    {
        GameDirector.GameDirectorInstance.EndFight();

        var spriteHeight = SpriteRenderer.sprite.bounds.size.y;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(new Vector3(2f, 0f, 1f), 0.8f).SetEase(Ease.InBack));
        sequence.Join(transform.DOMoveY(transform.position.y -spriteHeight / 2, 0.8f).SetEase(Ease.InBack));
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