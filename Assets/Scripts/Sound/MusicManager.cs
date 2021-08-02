using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicState
{
    playing,
    stop,
    pause
} 

public class MusicManager : MonoBehaviour
{

    public AudioClip[] music;

    private MusicState state = MusicState.stop;
    private int currentTrackIndex = 0;
    private AudioSource audioSource;

    private void Update()
    {
        // play next track
        if (!audioSource.isPlaying &&  state == MusicState.playing)
        {
            PlayNextTrack();
        }   
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // initial settings
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        PlayTrackByIndex(0);
    }

    public void PlayNextTrack()
    {
        currentTrackIndex++;
        if (currentTrackIndex > music.Length - 1)
        {
            currentTrackIndex = 0;
        }
        audioSource.clip = music[currentTrackIndex];
    }

    public void PlayTrackByName(string name)
    {
        AudioClip track = null;
        for (int i = 0; i < music.Length; i++)
        {
            if (music[i].name == name)
            {
                track = music[i];
                currentTrackIndex = i;
                break;
            }
        }

        if (track == null)
        {
            Debug.Log("Track with this name was not found");
            return;
        }
        else if (track != null)
        {
            Debug.Log("Playing music track: " + track.name);

            audioSource.clip = track;
            audioSource.Play();
            state = MusicState.playing;
        }
    }

    public void PlayTrackByIndex(int index)
    {
        audioSource.clip = music[index];
        audioSource.Play();
        state = MusicState.playing;
    }

    public void Pause()
    {
        audioSource.Pause();
        state = MusicState.pause;
    }

    public void Stop()
    {
        audioSource.Stop();
        state = MusicState.stop;
    }

    public string GetCurrentTrackName()
    {
        if (audioSource.clip == null) return null;
        return audioSource.clip.name;
    }
}