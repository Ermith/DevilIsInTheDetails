using UnityEngine;

[RequireComponent(typeof(Health))]
class Hero : MonoBehaviour
{
    private void Start()
    {
        gameObject.name = GameDirector.HeroName;
    }
}