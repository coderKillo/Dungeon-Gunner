using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float upwardsSpeed;
    [SerializeField] private float disappearTime;
    [SerializeField] private float disappearSpeed;
    [SerializeField] private TextMeshProUGUI textMesh;

    private Vector3 worldPosition;
    private float disappearTimer = 0f;

    static public DamagePopup Create(Vector3 location, string text, Color color)
    {
        var damagePopup = (DamagePopup)PoolManager.Instance.ReuseComponent(GameResources.Instance.damagePopupPrefab, HelperUtilities.mainCamera.WorldToScreenPoint(location), Quaternion.identity);

        damagePopup.Setup(text, location, color);

        return damagePopup;
    }

    void Setup(string text, Vector3 location, Color color)
    {
        textMesh.text = text;
        textMesh.alpha = 1;
        textMesh.color = color;
        worldPosition = location;
        disappearTimer = disappearTime;

        gameObject.SetActive(true);
    }

    void Update()
    {
        worldPosition += new Vector3(0, upwardsSpeed, 0) * Time.deltaTime;
        textMesh.transform.position = HelperUtilities.mainCamera.WorldToScreenPoint(worldPosition);

        disappearTimer -= Time.deltaTime;
        if (disappearTimer <= 0)
        {
            textMesh.alpha -= disappearSpeed * Time.deltaTime;
            if (textMesh.alpha <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
