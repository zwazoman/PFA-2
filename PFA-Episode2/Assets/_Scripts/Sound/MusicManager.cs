using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    #region Singleton
    private static MusicManager instance = null;
    public static MusicManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<GameObject>("SoundManager").GetComponent<MusicManager>());
            }
            return instance;
        }
    }
    #endregion

    [SerializeField] AudioSource _musicSource;
    [SerializeField] float _fadeDuration = .35f;

    [SerializeField] public SerializedDictionary<string, MusicParameters> musicClipDict;

    MusicParameters _currentParameter;

    private async void Start()
    {
        SceneManager.activeSceneChanged += (Scene _, Scene _) => SceneTransitionManager.Instance.OnSceneChange += ChangeMusic;

        ChangeMusic("MainMenu");
    }

    public async void ChangeMusic(string nextSceneName)
    {
        foreach(string sceneName in musicClipDict.Keys)
            if(sceneName == nextSceneName)
                await SwapClips(musicClipDict[sceneName]);
    }

    async UniTask SwapClips(MusicParameters musicParameter)
    {
        _currentParameter = musicParameter;
        AudioClip choosenMusic = musicParameter.musics.PickRandom();

        if (choosenMusic == _musicSource.clip)
            return;

        await _musicSource.DOFade(0, _fadeDuration);
        _musicSource.clip = choosenMusic;
        _musicSource.Play();
        await _musicSource.DOFade(musicParameter.targetVolume, _fadeDuration);
    }

    public async void ChangeVolume(float targetVolume, float changeDuration)
    {
        await _musicSource.DOFade(targetVolume, changeDuration);
    }

    public async void ResetVolume(float changeDuration)
    {
        await _musicSource.DOFade(_currentParameter.targetVolume, changeDuration);
    }
}
