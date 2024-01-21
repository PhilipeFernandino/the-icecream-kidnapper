using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "ScriptableObjects/SoundList", order = 1)]
public class SoundList : ScriptableObject
{
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    public List<Sound> Sounds { get => sounds; set => sounds = value; }
}

[System.Serializable]
public class Sound
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private string clipName;
    [SerializeField][Range(0f, 1f)] private float volume = 1;
    [SerializeField][Range(.1f, 3f)] private float pitch = 1;
    [SerializeField] private bool loop;

    public AudioClip Clip { get => clip; set => clip = value; }
    public string Name { get => clipName; set => clipName = value; }
    public float Volume { get => volume; set => volume = value; }
    public float Pitch { get => pitch; set => pitch = value; }
    public bool Loop { get => loop; set => loop = value; }
    public AudioSource Source { get; set; }
}
