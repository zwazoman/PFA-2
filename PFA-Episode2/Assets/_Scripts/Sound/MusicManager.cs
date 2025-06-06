using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource _musicSource;
    [SerializeField] float _fadeDuration = .35f;

    [SerializeField] SerializedDictionary<string, MusicParameters> musicClipDict;

    private void Start()
    {
        SceneManager.activeSceneChanged += (Scene _, Scene _) => SceneTransitionManager.Instance.OnSceneChange += CheckMusic; ;
    }

    async void CheckMusic(string nextSceneName)
    {
        print(nextSceneName);
        foreach(string sceneName in musicClipDict.Keys)
            if(sceneName == nextSceneName)
                await SwapMusics(musicClipDict[sceneName]);
    }

    async UniTask SwapMusics(MusicParameters musicParameter)
    {
        AudioClip choosenMusic = musicParameter.musics.PickRandom();

        if (choosenMusic == _musicSource.clip)
            return;

        await _musicSource.DOFade(0, _fadeDuration);
        _musicSource.clip = choosenMusic;
        _musicSource.Play();
        await _musicSource.DOFade(musicParameter.targetVolume, _fadeDuration);
    }
}
