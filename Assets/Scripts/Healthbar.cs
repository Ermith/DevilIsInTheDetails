
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public UnityEngine.UI.Image HealthBar;
    public UnityEngine.UI.Image DamageBar;
    public UnityEngine.UI.Image Background;
    public TextMeshProUGUI Text;
    public Health Health;

    private int _healthTextNumber = -1;

    public void SetHealth(int health, int maxHealth)
    {
        if (_healthTextNumber == -1)
            _healthTextNumber = health;

        float ratio = health * 1f / maxHealth;

        HealthBar.DOFillAmount(ratio, 0.5f);

        // delay on DamageBar tweening
        Sequence damageSequence = DOTween.Sequence();
        damageSequence.AppendInterval(0.5f);
        damageSequence.Append(DamageBar.DOFillAmount(ratio, 0.25f));

        // tween the text
        DOTween.To(
            () => _healthTextNumber,
            x =>
            {
                _healthTextNumber = x;
                Text.text = $"{x} / {maxHealth}";
            },
            health,
            0.5f
        );

        float relDelta = (health - _healthTextNumber) * 1f / maxHealth;
        if (relDelta < 0)
            transform.DOShakePosition(0.5f, relDelta * 1f);
    }

    public void OnHit(int amount, Health.DamageType type, GameObject whatever)
    {
        SetHealth(Health.HealthPoints, Health.MaxHealth);
    }

    public void OnHeal(int amount, GameObject whatever)
    {
        SetHealth(Health.HealthPoints, Health.MaxHealth);
    }
}