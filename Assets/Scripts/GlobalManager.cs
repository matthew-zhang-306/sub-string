using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }

    public int previousBuildIndexIncrement;

    public AudioSource musicSource;
    public AudioSource windSource;
    public AudioClip[] songs;
    private int currentSong;

    private float maxVolume;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    
        DontDestroyOnLoad(gameObject);

        currentSong = -1;
        maxVolume = musicSource.volume;
    }


    public void PlaySong(int song) {
        if (currentSong != song) {
            musicSource.clip = songs[song];
            musicSource.Play();
        }

        currentSong = song;

        musicSource.DOKill();
        musicSource.DOFade(maxVolume, 1f);
    }

    public void StopSong() {
        musicSource.DOFade(0f, 2f);
    }

    public void PlayAmbience() {
        windSource.DOKill();
        windSource.DOFade(1f, 2f);
    }

    public void StopAmbience() {
        windSource.DOKill();
        windSource.DOFade(0f, 2f);
    }
}
