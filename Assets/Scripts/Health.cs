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

    public enum DamageType
    {
        Slash,
        Thrust,
        Strike,
        Poison
    }

    Healthbar _healthbar;
    private int _slashBlock;
    private int _thrustBlock;
    private int _strikeBlock;

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
    }

    public void SetupHealthbar()
    {
        Canvas canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        _healthbar = Instantiate(GameDirector.GameDirectorInstance.Healthbar, canvas.transform);
        _healthbar.Health = this;
        RectTransform rectTransform = _healthbar.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = transform.position + Vector3.up * 2f;
        OnHeal += _healthbar.OnHeal;
        OnHit += _healthbar.OnHit;
    }

    public void HitBy(int damage, DamageType type, GameObject attacker)
    {
        if (type == DamageType.Slash) damage -= _slashBlock;
        if (type == DamageType.Thrust) damage -= _thrustBlock;
        if (type == DamageType.Strike) damage -= _strikeBlock;

        _slashBlock = 0;
        _thrustBlock = 0;
        _strikeBlock = 0;

        if (damage < 0) damage = 0;

        HealthPoints -= damage;
        OnHit?.Invoke(damage, type, attacker);

        if (HealthPoints <= 0)
        {
            OnDeath?.Invoke(attacker);
            Destroy(gameObject);
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

    public void Poison(int damage, int count, float interval)
    {
        StartCoroutine(PoisonRoutine(damage, count, interval));
    }

    private IEnumerator PoisonRoutine(int damage, int count, float interval)
    {
        while (count > 0)
        {
            HitBy(damage, DamageType.Poison, null);
            count--;
            yield return new WaitForSeconds(interval);
        }
    }

    private void OnDestroy()
    {
        Destroy(_healthbar.gameObject);
    }
}
