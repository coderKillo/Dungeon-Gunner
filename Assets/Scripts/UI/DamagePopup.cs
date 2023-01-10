using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float disappearTime;
    [SerializeField] private float disappearSpeed;
    [SerializeField] private TextMeshProUGUI textMesh;

    [MinMaxSlider(1, 6)]
    [SerializeField] private Vector2 speedRange;
    public float RandomSpeed { get { return Random.Range(speedRange.x, speedRange.y); } }

    [MinMaxSlider(-45, 45)]
    [SerializeField] private Vector2 directionRange;
    public float RandomDirection { get { return Random.Range(directionRange.x, directionRange.y); } }

    private Vector3 worldPosition;
    private float disappearTimer = 0f;
    private float speed = 0f;
    private Vector3 direction = Vector3.up;

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

        speed = RandomSpeed;
        direction = HelperUtilities.GetVectorFromAngle(RandomDirection + 90);

        gameObject.SetActive(true);
    }

    void Update()
    {
        worldPosition += direction.normalized * speed * Time.deltaTime;
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
