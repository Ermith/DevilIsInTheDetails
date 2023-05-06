
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public UnityEngine.UI.Image HealthBar;
    public UnityEngine.UI.Image DamageBar;
    public UnityEngine.UI.Image Background;
    public UnityEngine.UI.Image SlashBlock;
    public UnityEngine.UI.Image StrikeBlock;
    public UnityEngine.UI.Image PierceBlock;
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
        damageSequence.Play();

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

    public void OnBlockChange()
    {
        if (Health.StrikeBlock > 0)
        {
            StrikeBlock.gameObject.SetActive(true);
            TextMeshProUGUI strikeText = StrikeBlock.GetComponentInChildren<TextMeshProUGUI>();
            strikeText.text = Health.StrikeBlock.ToString();
        }
        else
        {
            StrikeBlock.gameObject.SetActive(false);
        }

        if (Health.SlashBlock > 0)
        {
            SlashBlock.gameObject.SetActive(true);
            TextMeshProUGUI slashText = SlashBlock.GetComponentInChildren<TextMeshProUGUI>();
            slashText.text = Health.SlashBlock.ToString();
        }
        else
        {
            SlashBlock.gameObject.SetActive(false);
        }

        if (Health.ThrustBlock > 0)
        {
            PierceBlock.gameObject.SetActive(true);
            TextMeshProUGUI pierceText = PierceBlock.GetComponentInChildren<TextMeshProUGUI>();
            pierceText.text = Health.ThrustBlock.ToString();
        }
        else
        {
            PierceBlock.gameObject.SetActive(false);
        }
    }

    public void OnHit(int amount, Health.DamageType type, GameObject whatever)
    {
        SetHealth(Health.HealthPoints, Health.MaxHealth);
    }

    public void OnHeal(int amount, GameObject whatever)
    {
        SetHealth(Health.HealthPoints, Health.MaxHealth);
    }

    void OnDestroy()
    {
        // kill all tweens
        DOTween.Kill(this);
    }
}