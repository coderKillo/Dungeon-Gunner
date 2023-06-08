using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class DisplayMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Button("Display Test Text", ButtonSizes.Large)]
    private void DisplayTest()
    {
        DisplayText("Test", "This is a short text to test the UI, will disappear after 2 seconds.", 2f);
    }

    public void DisplayText(string title, string message, float duration, float alpha = 1f, float fadeIn = 0f)
    {
        titleText.text = title;
        messageText.text = message;

        GameManager.Instance.Player.EnablePlayer(false);

        var displaySequence = DOTween.Sequence();
        displaySequence.Append(canvasGroup.DOFade(alpha, fadeIn));
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
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageText), messageText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
    }
#endif
    #endregion
}
