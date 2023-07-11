using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class DisplayMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image background;

    [Button("Display Test Text", ButtonSizes.Large)]
    private void DisplayTest()
    {
        DisplayText("Test", "This is a short text to test the UI, will disappear after 2 seconds.", 2f);
    }

    public void DisplayText(string title, string message, float duration, float alpha = 1f, float fadeIn = 0f)
    {
        titleText.text = title;
        messageText.text = message;

        var bgColor = background.color;
        bgColor.a = alpha;
        background.color = bgColor;

        GameManager.Instance.Player.EnablePlayer(false);

        var displaySequence = DOTween.Sequence();
        displaySequence.SetUpdate(true);
        displaySequence.Append(canvasGroup.DOFade(1f, fadeIn));
        displaySequence.AppendInterval(duration);
        displaySequence.AppendCallback(() =>
        {
            GameManager.Instance.Player.EnablePlayer(true);
        });
        displaySequence.Append(canvasGroup.DOFade(0f, 2f));
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
    }
#endif
    #endregion
}
