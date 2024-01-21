using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public List<SoundList> sounds;

    private void Awake()
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            foreach (var s in sounds[i].Sounds)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.Clip;
                s.Source.volume = s.Volume;
                s.Source.pitch = s.Pitch;
                s.Source.loop = s.Loop;
            }
        }
    }

    private Sound FindSoundByName(string key)
    {
        Sound s = null;

        for (int i = 0; i < sounds.Count; i++)
        {
            s = sounds[i].Sounds.Find(sound => sound.Name == key);

            if (s == null)
                continue;

            return s;
        }

        return s;
    }

    public void Play(string name)
    {
        Sound s = FindSoundByName(name);

        if (s == null)
        {
            Debug.LogWarning("The sound '" + name + "' was not found");
            return;
        }

        s.Source.Play();
    }

    public void Stop(string name)
    {
        Sound s = FindSoundByName(name);

        if (s == null)
        {
            return;
        }

        s.Source.Stop();
    }

    public void Volume(string name, float volume)
    {
        Sound s = FindSoundByName(name);

        if (s == null)
        {
            Debug.LogWarning("O som " + name + " não foi encontrado");
            return;
        }

        s.Source.volume = volume;
    }

    //Normal
    public void FadeSound(string name, int fadeAway)
    {
        Sound s = FindSoundByName(name);
        StartCoroutine(FadeSoundEnum(name, fadeAway));
    }

    public IEnumerator FadeSoundEnum(string name, int fadeAway)
    {
        Sound s = FindSoundByName(name);

        if (s == null)
        {
            Debug.LogWarning("O som " + name + " n o foi encontrado");
            yield return null;
        }

        // Som surgindo aos poucos
        if (fadeAway == 1)
        {
            for (float i = s.Source.volume; i <= 0.1f; i += (0.2f * Time.deltaTime))
            {
                if (i > 0.095f)
                {
                    i = 0.1f;
                }

                s.Source.volume = i;
                yield return null;
            }
        }

        // Som sumindo aos poucos
        if (fadeAway == 2)
        {
            for (float i = s.Source.volume; i >= 0f; i -= (0.2f * Time.deltaTime))
            {
                if (i < 0.005f)
                {
                    i = 0;
                }

                s.Source.volume = i;
                yield return null;
            }
        }


        if (fadeAway == 3) //N o me lembro o que este faz
        {
            for (float i = s.Source.volume; i <= 0.25f; i += (0.2f * Time.deltaTime))
            {
                if (i > 0.22f)
                {
                    i = 0.25f;
                }

                s.Source.volume = i;
                yield return null;
            }
        }

        // Vers es mais r pidas
        // Som surgindo aos poucos (r pido)
        if (fadeAway == 4)
        {
            for (float i = s.Source.volume; i <= 0.1f; i += (0.6f * Time.deltaTime))
            {
                if (i > 0.095f)
                {
                    i = 0.1f;
                }

                s.Source.volume = i;
                yield return null;
            }
        }

        if (fadeAway == 5)
        {
            // Som sumindo aos poucos (r pido)
            for (float i = s.Source.volume; i >= 0f; i -= (0.6f * Time.deltaTime))
            {
                if (i < 0.005f)
                {
                    i = 0;
                }

                s.Source.volume = i;
                yield return null;
            }
        }
    }


    //=======

    public void FadeSoundAdvanced(string name, int fadeAway, float targetVolume)
    {
        StartCoroutine(FadeSoundAdvancedEnum(name, fadeAway, targetVolume));
    }

    public IEnumerator FadeSoundAdvancedEnum(string name, int fadeAway, float targetVolume)
    {
        Sound s = FindSoundByName(name);

        if (s == null)
        {
            Debug.LogWarning("O som " + name + " n o foi encontrado");
            yield return null;
        }

        // Som surgindo aos poucos
        if (fadeAway == 1)
        {
            for (float i = s.Source.volume; i <= 1.0f; i += 0.0004f)
            {
                if (i > targetVolume - 0.005f)
                {
                    i = targetVolume;
                }

                s.Source.volume = i;
                yield return null;
            }
        }

        // Som sumindo aos poucos
        if (fadeAway == 2)
        {
            for (float i = s.Source.volume; i >= 0f; i -= 0.0008f)
            {
                if (i < targetVolume + 0.005f)
                {
                    i = targetVolume;
                }

                s.Source.volume = i;
                yield return null;
            }
        }

        // Vers es mais r pidas
        // Som surgindo aos poucos (r pido)
        if (fadeAway == 3)
        {
            for (float i = s.Source.volume; i <= targetVolume; i += 0.001f)
            {
                if (i > targetVolume - 0.005f)
                {
                    i = targetVolume;
                }

                s.Source.volume = i;
                yield return null;
            }
        }

        if (fadeAway == 4)
        {
            // Som sumindo aos poucos (r pido)
            for (float i = s.Source.volume; i >= targetVolume; i -= 0.001f)
            {
                if (i < targetVolume + 0.005f)
                {
                    i = targetVolume;
                }

                s.Source.volume = i;
                yield return null;
            }
        }
    }
}
