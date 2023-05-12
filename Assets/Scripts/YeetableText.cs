
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class YeetableText : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public static YeetableText Yeet(string text, Color color, Vector3 source, Vector3 target, float duration, float spinSpeed = 0f, int fontSize = 4, Transform parent = null)
    {
        var yeetableText = Instantiate(GameDirector.GameDirectorInstance.YeetableTextPrefab, source, Quaternion.identity);
        var go = yeetableText.gameObject;
        go.transform.SetParent(parent ?? GameObject.FindWithTag("AboveGameCanvas").transform);
        yeetableText.Text.text = text;
        yeetableText.Text.fontSize = fontSize;

        var transparentColor = new Color(color.r, color.g, color.b, 0f);
        yeetableText.Text.color = transparentColor;

        var sequence = DOTween.Sequence();
        sequence.Append(yeetableText.Text.DOColor(color, duration * 0.1f).SetEase(Ease.OutCubic));
        sequence.Join(yeetableText.transform.DOMove(target, duration).SetEase(Ease.OutCubic));
        sequence.Insert(duration * 0.9f, yeetableText.Text.DOColor(transparentColor, duration * 0.1f).SetEase(Ease.OutCubic));
        sequence.OnComplete(() => Destroy(go));
        sequence.Play();

        if (spinSpeed > 0f)
        {
            yeetableText.transform.DORotate(Vector3.forward * 360f, spinSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        }

        return yeetableText;
    }

    public void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}