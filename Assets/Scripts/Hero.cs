using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Hero : MonoBehaviour
{
    Health _health;

    public Health Health => _health ??= GetComponent<Health>();

    private void Start()
    {
        Health.OnDeath += OnDeath;
        Health.OnHit += OnHit;
    }

    private void OnHit(int damage, Health.DamageType type, GameObject attacker)
    {
        StartCoroutine(DelayedOuch(0.5f));
    }

 
    private IEnumerator DelayedOuch(float secs)
    {
        yield return new WaitForSeconds(secs);
        GameDirector.AudioManagerInstance.Play("Ouch");
    }

    private void OnDeath(GameObject attacker)
    {
        GameDirector.GameDirectorInstance.GameOver();
    }
}