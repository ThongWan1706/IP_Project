using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Sources (Simple Approach)")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    // Linking this to the Master & BGM Slider in Inspector
    public void SetMusicVolume(float volume)
    {
        if (bgmSource != null)
            bgmSource.volume = volume; // volume rangeed 0 to 1
    }

    // Linking this to the SFX Slider
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;
    }
}