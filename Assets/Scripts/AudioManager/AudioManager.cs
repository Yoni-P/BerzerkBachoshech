using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Singleton class that can be used to play sounds in game
 */
public class AudioManager : MonoBehaviour
{
    public const String ELECTROCUTION_SOUND = "Electrocution";
    public const String ROBOT_EXPLOSION = "Robot Explosion";
    public const String HUMAN_SCREAM = "Human Scream";
    public const String LASER_SHOT = "Laser Shot";
    public const String THUNDERSTORM = "Thunderstorm";
    
    
    public static AudioManager instance;

    [SerializeField] private Sound[] _sounds;
    
    private Dictionary<string, AudioSource> audioSourcesDict;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        audioSourcesDict = new Dictionary<string, AudioSource>();
        foreach (Sound s in _sounds)
        {
            audioSourcesDict.Add(s.name, s.AudioSource);
        }
    }

    /**
     * Plays A sound based on a given name
     */
    public void PlaySound(string soundName)
    {
       if (audioSourcesDict.ContainsKey(soundName))
           audioSourcesDict[soundName].Play();
    }

    /**
     * Stops A sound based on a given name
     */
    public void StopSound(string soundName)
    {
        if (audioSourcesDict.ContainsKey(soundName))
            audioSourcesDict[soundName].Stop();
    }
}
