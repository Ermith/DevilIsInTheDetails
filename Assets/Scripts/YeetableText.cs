
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class YeetableText : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public static void Yeet(string text, Color color, Vector3 source, Vector3 target, float duration, float spinSpeed = 0f, int fontSize = 4)
    {
        var yeetableText = Instantiate(GameDirector.GameDirectorInstance.YeetableTextPrefab, source, Quaternion.identity);
        var go = yeetableText.gameObject;
        go.transform.SetParent(GameObject.FindWithTag("Canvas").transform);
        yeetableText.Text.text = text;
        yeetableText.Text.color = color;
        yeetableText.Text.fontSize = fontSize;

        yeetableText.transform.DOMove(target, duration).SetEase(Ease.OutCubic).OnComplete(() => Destroy(go));

        if (spinSpeed > 0f)
        {
            yeetableText.transform.DORotate(Vector3.forward * 360f, spinSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        }
    }
}