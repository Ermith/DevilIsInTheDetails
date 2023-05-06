
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
    public UnityEngine.UI.Image ThrustBlock;
    public UnityEngine.UI.Image PoisonIndicator;
    public UnityEngine.UI.Image AttackIndicator;
    public UnityEngine.UI.Image AttackSlash;
    public UnityEngine.UI.Image AttackStrike;
    public UnityEngine.UI.Image AttackThrust;
    public UnityEngine.UI.Image AttackPoison;
    public TextMeshProUGUI Text;
    public Health Health;

    private int _healthTextNumber = -1;

    public void SetHealth(int health, int maxHealth, float tweenTime = 0.5f)
    {
        if (_healthTextNumber == -1)
            _healthTextNumber = health;

        float ratio = health * 1f / maxHealth;

        HealthBar.DOFillAmount(ratio, tweenTime);

        // delay on DamageBar tweening
        Sequence damageSequence = DOTween.Sequence();
        damageSequence.AppendInterval(tweenTime);
        damageSequence.Append(DamageBar.DOFillAmount(ratio, tweenTime / 2));
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
            tweenTime
        );

        float relDelta = (health - _healthTextNumber) * 1f / maxHealth;
        if (relDelta < 0)
            transform.DOShakePosition(tweenTime, relDelta * 1f);
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
            ThrustBlock.gameObject.SetActive(true);
            TextMeshProUGUI thrustText = ThrustBlock.GetComponentInChildren<TextMeshProUGUI>();
            thrustText.text = Health.ThrustBlock.ToString();
        }
        else
        {
            ThrustBlock.gameObject.SetActive(false);
        }
    }

    public void OnPoisonChange()
    {
        if (Health.PoisonCount > 0)
        {
            PoisonIndicator.gameObject.SetActive(true);
            TextMeshProUGUI poisonText = PoisonIndicator.GetComponentInChildren<TextMeshProUGUI>();
            poisonText.text = Health.PoisonCount.ToString();
        }
        else
        {
            PoisonIndicator.gameObject.SetActive(false);
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

    public void SetAttack(float progress, Health.DamageType type, float tweenTime = 0.1f)
    {
        if (progress < 0)
        {
            AttackIndicator.gameObject.SetActive(false);
            return;
        }
        AttackIndicator.gameObject.SetActive(true);
        if (tweenTime > 0)
            AttackIndicator.DOFillAmount(progress, tweenTime);
        else
            AttackIndicator.fillAmount = progress;

        AttackStrike.gameObject.SetActive(type == Health.DamageType.Strike);
        AttackSlash.gameObject.SetActive(type == Health.DamageType.Slash);
        AttackThrust.gameObject.SetActive(type == Health.DamageType.Thrust);
        AttackPoison.gameObject.SetActive(type == Health.DamageType.Poison);
    }

    void OnDestroy()
    {
        // kill all tweens
        DOTween.Kill(this);
    }
}