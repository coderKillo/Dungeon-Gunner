using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Object/Sounds/Music Track", order = 0)]
public class MusicTrackSO : ScriptableObject
{
    public string musicName;
    public AudioClip musicClip;
    [Range(0, 1)] public float musicVolume;
}
