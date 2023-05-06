using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth = 100;
    public int HealthPoints { get; private set; }

    public delegate void OnHitHandler(int damage, GameObject attacker);
    public event OnHitHandler OnHit;

    public delegate void OnDeathHandler(GameObject attacker);
    public event OnDeathHandler OnDeath;

    public delegate void OnHealHandler(int heal, GameObject healer);
    public event OnHitHandler OnHeal;

    public delegate void OnHealthFullHandler(GameObject healer);
    public event OnDeathHandler OnHealthFull;
    
    Healthbar _healthbar;

    private void Awake()
    {
        HealthPoints = MaxHealth;
    }

    private void Start()
    {
        SetupHealthbar();
    }

    public void SetupHealthbar()
    {
        Canvas canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        _healthbar = Instantiate(GameDirector.GameDirectorInstance.Healthbar, canvas.transform);
        _healthbar.Health = this;
        RectTransform rectTransform = _healthbar.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = transform.position + Vector3.up * 2f;
        OnHeal += _healthbar.OnHealthChange;
        OnHit += _healthbar.OnHealthChange;
    }

    public void HitBy(int damage, GameObject attacker)
    {
        HealthPoints -= damage;
        OnHit?.Invoke(damage, attacker);

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

    private void OnDestroy()
    {
        Destroy(_healthbar.gameObject);
    }
}
