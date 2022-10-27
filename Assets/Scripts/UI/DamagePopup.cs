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

    static public DamagePopup Create(Vector3 location, int damage)
    {
        var damagePopup = (DamagePopup)PoolManager.Instance.ReuseComponent(GameResources.Instance.damagePopupPrefab, HelperUtilities.mainCamera.WorldToScreenPoint(location), Quaternion.identity);

        damagePopup.Setup(damage, location);

        return damagePopup;
    }

    void Setup(int damage, Vector3 location)
    {
        textMesh.text = damage.ToString();
        textMesh.alpha = 1;
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
