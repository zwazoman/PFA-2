using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource _musicSource;
    [SerializeField] float _fadeDuration = .35f;

    [SerializeField] SerializedDictionary<string, MusicParameters> musicClipDict;

    private void Start()
    {
        _musicSource = GetComponent<AudioSource>();
        SceneTransitionManager.Instance.OnSceneChange += CheckMusic;
    }

    async void CheckMusic(string nextSceneName)
    {
        foreach(string sceneName in musicClipDict.Keys)
            if(sceneName == nextSceneName)
                await SwapMusics(musicClipDict[sceneName]);
    }

    async UniTask SwapMusics(MusicParameters musicParameter)
    {

        await _musicSource.DOFade(0, _fadeDuration);
        _musicSource.clip = musicParameter.musics.PickRandom();
        await _musicSource.DOFade(musicParameter.targetVolume, _fadeDuration);
    }
}
