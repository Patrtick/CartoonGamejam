using System.Collections.Generic;
using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour
{
    [Header("Музыка боя")]
    [SerializeField] private AudioClip[] battleTracks;

    [Header("Музыка конца игры")]
    [SerializeField] private AudioClip[] endGameTracks;

    [SerializeField] private AudioSource audioSource;

    private List<int> shuffledIndexes = new();

    private int currentIndex;

    private bool isPlayingEndGameMusic;

    private void Start()
    {
        PlayBattleMusic();
    }

    private void Update()
    {
        if (!Application.isFocused)
            return;

        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    public void PlayBattleMusic()
    {
        isPlayingEndGameMusic = false;

        GenerateShuffle(battleTracks);

        PlayNextTrack();
    }

    public void PlayEndGameMusic()
    {
        isPlayingEndGameMusic = true;

        GenerateShuffle(endGameTracks);

        PlayNextTrack();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    private void GenerateShuffle(AudioClip[] tracks)
    {
        shuffledIndexes.Clear();

        for (int i = 0; i < tracks.Length; i++)
        {
            shuffledIndexes.Add(i);
        }

        for (int i = 0; i < shuffledIndexes.Count; i++)
        {
            int randomIndex =
                Random.Range(i, shuffledIndexes.Count);

            (shuffledIndexes[i], shuffledIndexes[randomIndex]) = (shuffledIndexes[randomIndex], shuffledIndexes[i]);
        }

        currentIndex = 0;
    }

    private void PlayNextTrack()
    {
        AudioClip[] currentTracks =
            isPlayingEndGameMusic
            ? endGameTracks
            : battleTracks;

        if (
            currentTracks == null ||
            currentTracks.Length == 0
        )
        {
            return;
        }

        if (currentIndex >= shuffledIndexes.Count)
        {
            GenerateShuffle(currentTracks);
        }

        int trackIndex =
            shuffledIndexes[currentIndex];

        audioSource.clip =
            currentTracks[trackIndex];

        audioSource.Play();

        currentIndex++;
    }
}