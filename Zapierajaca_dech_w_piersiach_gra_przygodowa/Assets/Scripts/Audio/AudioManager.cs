using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] footStep;
    private int _sceneID;
    private Sound currentfootStep;
    private AudioSource BG;
    private bool lockedPlay;

    void Awake()
    {
        if (footStep.Length > 0)
        {
            _sceneID = SceneManager.GetActiveScene().buildIndex;
            sounds[0] = footStep[_sceneID - 1];
        }

        foreach(Sound s in sounds)
        {
            AddNewAudioSource(s);
        }

        lockedPlay = false;
    }

    private void Start()
    {
        BG = Managers.Inventory.GetAudioBackground();
    }

    private void AddNewAudioSource(Sound s)
    {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;

        if (s.scapeSound)
        {
            s.source.spatialBlend = 1;
            s.source.spread = 0;
            s.source.reverbZoneMix = 1;
            s.source.rolloffMode = AudioRolloffMode.Linear;
            s.source.maxDistance = 30f;
        }
        else
        {
            s.source.spatialBlend = 0;
            s.source.spread = 0;
            s.source.reverbZoneMix = 1;
            s.source.rolloffMode = AudioRolloffMode.Linear;
            s.source.maxDistance = 30f;
        }
    }

    public void Play(string name)
    {
        if (!lockedPlay)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }
            s.source.Play();
        }
    }

    public void Play(string name, float volume, float pitch, bool loop, bool spaceSound)
    {
        if (!lockedPlay)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }

            if (!s.source.isPlaying)
            {
                s.source.volume = volume;
                s.source.pitch = pitch;
                s.source.loop = loop;

                if (s.scapeSound)
                {
                    s.source.spatialBlend = 1;
                    s.source.spread = 0;
                    s.source.reverbZoneMix = 1;
                    s.source.rolloffMode = AudioRolloffMode.Linear;
                    s.source.maxDistance = 30f;
                }
                else
                {
                    s.source.spatialBlend = 1;
                    s.source.spread = 0;
                    s.source.reverbZoneMix = 1;
                    s.source.rolloffMode = AudioRolloffMode.Linear;
                    s.source.maxDistance = 30f;
                }

                s.source.Play();
            }
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public bool isPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return false;
        }
        return s.source.isPlaying;
    }

    public void MuteAllSounds()
    {
        AudioSource[] allAudioSources;

        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource audioS in allAudioSources)
        {
                audioS.Stop();
        }
    }

    public void MuteSoundsWithoutThis()
    {
        AudioManager[] allAudioManagers;

        allAudioManagers = FindObjectsOfType(typeof(AudioManager)) as AudioManager[];
        foreach (AudioManager audioManager in allAudioManagers)
        {
            if (audioManager != this)
                audioManager.pauseAndLockAllLocalSounds();
        }
    }

    public void UnmuteAllManagers()
    {
        AudioManager[] allAudioManagers;

        allAudioManagers = FindObjectsOfType(typeof(AudioManager)) as AudioManager[];
        foreach (AudioManager audioManager in allAudioManagers)
        {
            audioManager.unlock();
        }
    }


    public void pauseAndLockAllLocalSounds()
    {
        lockedPlay = true;
        foreach(Sound s in sounds)
        {
            s.source.Stop();
        }
    }

    public void unlock()
    {
        lockedPlay = false;
    }
}
