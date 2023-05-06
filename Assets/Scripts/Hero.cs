using UnityEngine;

[RequireComponent(typeof(Health))]
public class Hero : MonoBehaviour
{
    Health _health;

    public Health Health => _health ??= GetComponent<Health>();

    private void Start()
    {
        Health.OnDeath += OnDeath;
    }

    private void OnDeath(GameObject attacker)
    {
        GameDirector.GameDirectorInstance.GameOver();
    }
}