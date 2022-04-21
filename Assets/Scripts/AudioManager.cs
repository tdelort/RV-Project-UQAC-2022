using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe Sound pour simplifier la gestion du son dans l'éditeur Unity
[System.Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    [Range(0f,1f)]
    public float volume = 1f;
    [Range(1f,3f)]
    public float pitch = 1f;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    // Tous les types de sons accessibles
    public enum SoundType {
        WALK,
        THROW,
        MESSAGE,
        KLAXON,
        UI,
        MUSIC
    }

    // Les listes des différents sons et musiques
    public Sound[] walk;
    public Sound[] throw_ball;
    public Sound[] message;
    public Sound[] klaxon;
    public Sound[] ui;
    public Sound[] music;

    

    public static AudioManager instance;
    Vector3 offset;
    float camAngle = 60f;
    float dist = 12f;

    float sfxVolume = 1.0f;

    private void Awake() {
        // Vérification que BaseAudioManager est bien un singleton
        if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // Initialisation de tous les sons
        foreach (Sound s in walk)
            Initialize(s);
        foreach (Sound s in message)
            Initialize(s);
        foreach (Sound s in klaxon)
            Initialize(s);
        foreach (Sound s in ui)
            Initialize(s);
        foreach (Sound s in music)
            Initialize(s);
    }

    // Initialise le son s et ses caractéristiques
    private void Initialize(Sound s) {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;
    }

    //----------------------------------------------------------------
    //            LES FONCTIONS PUBLIQUES DE SON EN LOCAL
    //----------------------------------------------------------------

    // PlayMusic : joue localement la musique name
    public void PlayMusic(string name) {
        // StopMusics();

        Sound s = Array.Find(music, sound => sound.name == name);
        if(s == null) {
            Debug.LogWarning("Music " + name + " not found");
            return;
        }

        if (s.source != null)
        {
            s.source.Play();
        }
        Debug.Log("Music playing");
    }

    // PlaySound : joue localement le son name de type type
    public void PlaySound(SoundType type, string name) {
        _PlaySound(getTab(type),name);
    }

    // PlaySound : joue localement le son name de type type à la position position
    public void PlaySoundAt(SoundType type, string name, Vector3 position) {
        _PlaySoundAt(getTab(type),name,position);
    }

    // StopMusics : arrête localement toutes les musiques. Redondant avec StopSounds, mais intuitif quand on ne connaît pas le code.
    public void StopMusics() {
        foreach (Sound s in music) {
            if(s.source != null) { 
                if (s.source.isPlaying)
                    s.source.Stop();
            }
        }
    }

    // StopSound : arrête localement le son name de type type
    public void StopSound(SoundType type, string name) {
        foreach (Sound s in getTab(type)) {
            if (s.name == name) {
                if (s.source != null)
                {
                    if (s.source.isPlaying)
                        s.source.Stop();
                }
            }
        }
    }

    // StopSounds : arrête localement tous les sons de type type
    public void StopSounds(SoundType type) {
        foreach (Sound s in getTab(type)) {
            if (s.source != null)
            {
                if (s.source.isPlaying)
                    s.source.Stop();
            }
        }
    }

    // StopAllSounds : arrête localement tous les sons, même les musiques
    public void StopAllSounds() {
        foreach (SoundType type in (SoundType[]) Enum.GetValues(typeof(SoundType))) {
            StopSounds(type);
        }
    }

    // public IEnumerator FadeOutMusic(float fadeOutTime)
    // {
    //     float t = 1f;
    //     while (t > 0f)
    //     {
    //         t -= Time.deltaTime / fadeOutTime;
    //         foreach (Sound s in music)
    //         {
    //             s.source.volume = t * s.volume;
    //         }
    //         yield return new WaitForEndOfFrame();
    //     }
    //     StopMusics();
    //     foreach (Sound s in music)
    //     {
    //         s.source.volume = s.volume;
    //     }

    // }

    //----------------------------------------------------------------
    //                          AUTRES
    //----------------------------------------------------------------

    // Récupère le tableau associé au SoundType type
    Sound[] getTab(SoundType type) {
        switch (type)
        {
            case SoundType.WALK:
                return walk;
            case SoundType.THROW:
                return throw_ball;
            case SoundType.MESSAGE:
                return message;
            case SoundType.KLAXON:
                return klaxon;
            case SoundType.UI:
                return ui;
            case SoundType.MUSIC:
                return music;
            default:
                Debug.LogWarning("No array matches SounType : sending walk as default");
                return walk;
        }
    }

    // joue le son name du tableau sounds
    void _PlaySound(Sound[] sounds, string name) {
        Debug.Log("start _PlaySound");
        Sound s;
        
        if (name == string.Empty)
            s = sounds[UnityEngine.Random.Range(0,sounds.Length)];
        else 
            s = Array.Find(sounds, sound => sound.name == name);
        
        if(s == null) {
            Debug.LogWarning("Audio source " + name + " not found");
            return;
        }
        if(s.source != null) {
            s.source.Play();
        }
        Debug.Log("Sound " + s.name + " playing");
    }
    AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float radius, float volume)
    {
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip; // define the clip
        aSource.spatialBlend = 1.0f; // Full 3D
        aSource.minDistance = radius;
        aSource.maxDistance = 30f;
        aSource.volume = volume;
        // set other aSource properties here, if desired
        aSource.Play(); // start the sound
        Destroy(tempGO, clip.length); // destroy object after clip duration
        return aSource; // return the AudioSource reference
    }

    private void _PlaySoundAt(Sound[] sounds, string name, Vector3 position)
    {
        Debug.Log("start _PlaySoundAt");
        Sound s;
        if (name == string.Empty)
            s = sounds[UnityEngine.Random.Range(0, sounds.Length)];
        else
            s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Audio source " + name + " not found");
            return;
        }

        PlayClipAt(s.clip, position + offset, 7f, s.volume);
        Debug.Log("Sound " + s.name + " playing");
    }
}