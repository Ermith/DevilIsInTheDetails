using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth = 100;
    public int HealthPoints { get; private set; }

    public delegate void OnHitHandler(int damage, DamageType type, GameObject attacker);
    public event OnHitHandler OnHit;

    public delegate void OnDeathHandler(GameObject attacker);
    public event OnDeathHandler OnDeath;

    public delegate void OnHealHandler(int heal, GameObject healer);
    public event OnHealHandler OnHeal;

    public delegate void OnHealthFullHandler(GameObject healer);
    public event OnDeathHandler OnHealthFull;

    public event Action OnBlockChange;

    public event Action OnPoisonChange;

    public enum DamageType
    {
        Slash,
        Thrust,
        Strike,
        Poison
    }
    
    public Healthbar Healthbar;
    private int _slashBlock;
    private int _thrustBlock;
    private int _strikeBlock;
    private int _poisonCount;

    public int SlashBlock => _slashBlock;
    public int ThrustBlock => _thrustBlock;
    public int StrikeBlock => _strikeBlock;

    public int PoisonCount
    {
        get => _poisonCount;
        private set
        {
            _poisonCount = value;
            OnPoisonChange?.Invoke();
        }
    }


    private void Awake()
    {
        HealthPoints = MaxHealth;
    }

    private void Start()
    {
        SetupHealthbar();
    }

    public void AddBlock(int slash, int thrust, int strike)
    {
        _slashBlock += slash;
        _thrustBlock += thrust;
        _strikeBlock += strike;

        OnBlockChange?.Invoke();
    }

    public void SetupHealthbar()
    {
        if (Healthbar != null)
            return;
        Canvas canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        Healthbar = Instantiate(GameDirector.GameDirectorInstance.Healthbar, canvas.transform);
        Healthbar.Health = this;
        RectTransform rectTransform = Healthbar.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = transform.position + Vector3.up * 2f;
        OnHeal += Healthbar.OnHeal;
        OnHit += Healthbar.OnHit;
        OnBlockChange += Healthbar.OnBlockChange;
        OnPoisonChange += Healthbar.OnPoisonChange;
    }

    public void HitBy(int damage, DamageType type, GameObject attacker)
    {
        if (type == DamageType.Slash) damage -= _slashBlock;
        if (type == DamageType.Thrust) damage -= _thrustBlock;
        if (type == DamageType.Strike) damage -= _strikeBlock;

        _slashBlock = 0;
        _thrustBlock = 0;
        _strikeBlock = 0;

        OnBlockChange?.Invoke();

        if (damage < 0) damage = 0;

        HealthPoints -= damage;
        OnHit?.Invoke(damage, type, attacker);

        if (HealthPoints <= 0)
        {
            OnDeath?.Invoke(attacker);
        }
    }

    public void HealBy(int health, GameObject healer)
    {
        HealthPoints += health;
        OnHeal?.Invoke(health, healer);

        if (HealthPoints >= MaxHealth)
        {
            HealthPoints = MaxHealth;
            OnHealthFull?.Invoke(healer);
        }
    }

    public void Poison(int count)
    {
        float interval = 1f;
        int damage = 5;
        PoisonCount += count;
        if (PoisonCount == count) // not started yet (probably) (I hope)
            StartCoroutine(PoisonRoutine(damage, interval));
    }

    private IEnumerator PoisonRoutine(int damage, float interval)
    {
        while (PoisonCount > 0)
        {
            yield return new WaitForSeconds(interval);
            HitBy(damage, DamageType.Poison, null);
            PoisonCount--;
        }
    }

    private void OnDestroy()
    {
        Destroy(Healthbar.gameObject);
    }
}
