using UnityEngine;

[CreateAssetMenu(fileName = "SpriteEffect_", menuName = "Scriptable Object/Effect/Sprite Effect")]
public class SpriteEffectSO : ScriptableObject
{
    public float frameRate = 24f;
    public Sprite[] spriteArray;
    public Vector3 offset = Vector3.zero;
    public Vector2 scale = Vector2.one;
}
