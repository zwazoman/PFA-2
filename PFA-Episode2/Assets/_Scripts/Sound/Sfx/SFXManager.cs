using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    #region Singleton
    private static SFXManager instance = null;
    public static SFXManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Audio Manager");
                instance = go.AddComponent<SFXManager>();
            }
            return instance;
        }
    }
    #endregion

    [SerializeField, Tooltip("taille de l'audioPool")] 
    private int _poolSize;

    [SerializeField, Tooltip("prefab d'AudioSource")] 
    private AudioSource _SFXObject;
 
    [SerializeField, Tooltip("dictionnaire contenant le nom d'un son en clefs et ses variantes en valeurs")]
    private SerializedDictionary<string, Clip> _soundsDict;

    [SerializeField]
    private Sounds _soundTester;

    Queue<AudioSource> _audioPool = new Queue<AudioSource>();
    List<Clip> _clips = new List<Clip>();

    string _soundsEnumFilePath = "Assets/_Scripts/Sound/Sfx/SoundsEnum.cs";

    AudioMixerManager _audioMixerManager;

    //AudioListener _sceneListener;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach(Clip clip in _soundsDict.Values)
        {
            _clips.Add(clip);
        }

        for (int i = 0; i < _poolSize; i++)
        {
            AddNewSourceToPool();
        }

        _audioMixerManager = AudioMixerManager.Instance;
        //_sceneListener = FindAnyObjectByType<AudioListener>();
    }

    #region Pool

    /// <summary>
    /// instancie et ajoute une nouvelle audiosource a la pool.
    /// </summary>
    /// <returns></returns>
    AudioSource AddNewSourceToPool()
    {
        AudioSource source = Instantiate(_SFXObject);
        source.transform.parent = transform;
        source.gameObject.SetActive(false);
        _audioPool.Enqueue(source);
        return source;
    }

    /// <summary>
    /// sors de la pool/queue une audiosource et active son gameObject
    /// </summary>
    /// <returns></returns>
    AudioSource UseFromPool()
    {
        if (_audioPool.Count == 0)
                AddNewSourceToPool();

        AudioSource source = _audioPool.Dequeue();
        source.gameObject.SetActive(true);
        return source;
    }

    /// <summary>
    /// renvoie l'audioSource dans la pool et désactive son gameObject
    /// </summary>
    /// <param name="source"></param>
    void BackToPool(AudioSource source)
    {
        source.gameObject.SetActive(false);
        _audioPool.Enqueue(source);
    }

    #endregion

    #region PlaySFXClip

    /// <summary>
    /// récupère une audioSource dans la pool et joue un des sons possibles
    /// </summary>
    /// <param name="choosenSound"></param>
    /// <param name="volumefactor"></param>
    /// <param name="pitchfactor"></param>
    public AudioSource PlaySFXClip(Sounds choosenSound, float volumefactor = 1f, float pitchfactor = 1f)
    {
        AudioSource audioSource = UseFromPool();
        Clip choosenClip = _clips[(int)choosenSound];

        try
        {
            AudioClip audioClip = ChooseRandomClip(choosenClip);
            audioSource.clip = audioClip; // assigne le clip random à l'audiosource
        }
        catch(NoSoundsFound e)
        {
            Debug.LogException(e);
            return null;
        }

        return AudioSourceHandle(audioSource, choosenClip.Volume * volumefactor, choosenClip.Pitch * pitchfactor, true, false, choosenClip.MixerGroup);
    }

    /// <summary>
    /// récupère une audioSource dans la pool et joue le son donné
    /// </summary>
    /// <param name="choosenSound"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    public AudioSource PlaySFXClip(AudioClip audioClip, float volume = 1f, float pitch = 1f)
    {
        AudioSource audioSource = UseFromPool();

        audioSource.clip = audioClip; // assigne le clip random à l'audiosource

        return AudioSourceHandle(audioSource, volume, pitch);
    }

    #endregion

    #region PlaySFXClipAtPosition

    /// <summary>
    /// chosit une liste dans une liste via un enum et choisit ensuite un audioclip au hasard dans ce dernier pour le jouer. le son sera placé a un endroit donné et prendra en compte ou non les effets.
    /// </summary>
    /// <param name="choosenSound"></param>
    /// <param name="position"></param>
    /// <param name="bypassesEffects"></param>
    /// <param name="volumeFactor"></param>
    /// <param name="pitchFactor"></param>
    public AudioSource PlaySFXClipAtPosition(Sounds choosenSound, Vector3 position, bool is2DSound = false, bool bypassesEffects = false, float volumeFactor = 1f, float pitchFactor = 1f)
    {
        AudioSource audioSource = UseFromPool();

        Clip choosenClip = _clips[(int)choosenSound];

        try
        {
            AudioClip audioClip = ChooseRandomClip(choosenClip);
            audioSource.clip = audioClip;
        }
        catch(NoSoundsFound e)
        {
            Debug.LogException(e);
            return null;
        }

        audioSource.gameObject.transform.position = position;

        return AudioSourceHandle(audioSource, choosenClip.Volume * volumeFactor, choosenClip.Pitch * pitchFactor, is2DSound, bypassesEffects, choosenClip.MixerGroup);

    }

    /// <summary>
    ///joue un son. le son sera placé a un endroit donné et prendra en compte ou non les effets.
    /// </summary>
    /// <param name="choosenSound"></param>
    /// <param name="position"></param>
    /// <param name="bypassesEffects"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    public AudioSource PlaySFXClipAtPosition(AudioClip choosenSound, Vector3 position, bool is2DSound = false, bool bypassesEffects = false, float volume = 1f, float pitch = 1f)
    {
        AudioSource audioSource = UseFromPool();

        audioSource.clip = choosenSound;

        audioSource.gameObject.transform.position = position;

        return AudioSourceHandle(audioSource, volume, pitch, is2DSound, bypassesEffects);
    }

    #endregion

    /// <summary>
    /// gère le retour a la pool d'une audiosource s'arrêtant apres la fin de son clip
    /// </summary>
    /// <param name="source"></param>
    /// <param name="clipLength"></param>
    /// <returns></returns>
    IEnumerator HandleSoundEnd(AudioSource source, float clipLength)
    {
        yield return new WaitForSecondsRealtime(clipLength);
        BackToPool(source);
    }

    /// <summary>
    /// gère les régales de l'audiosource et joue le son.
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <param name="is2D"></param>
    /// <param name="bypassesEffects"></param>
    /// <returns></returns>
    AudioSource AudioSourceHandle(AudioSource audioSource, float volume = 1, float pitch = 1, bool is2D = true, bool bypassesEffects = false, AudioMixerGroup choosenMixerGroup = null)
    {
        audioSource.volume = volume; // assigne le volume à l'audiosource
        audioSource.pitch = pitch; // assigne le pitch à l'audiosource
        if (!is2D) audioSource.spatialBlend = 1; // gère bien le blend rapport a l'audiolistener
        audioSource.bypassEffects = bypassesEffects; // gère le bypass ou non des effets
        if(choosenMixerGroup != null) audioSource.outputAudioMixerGroup = choosenMixerGroup; //selects the right audioMixer


        audioSource.Play(); // joue le son

        float clipLength = audioSource.clip.length; // détermine la longueur du son

        StartCoroutine(HandleSoundEnd(audioSource, clipLength));

        return audioSource;
    }

    /// <summary>
    /// choisit un clip random dans le clip donné. trueRandom défini si oui ou non, la liste devra-t-être parcourue entièrement avant de se répéter
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="trueRandom"></param>
    /// <returns></returns>
    AudioClip ChooseRandomClip(Clip clip)
    {
        if (clip.ClipList.Count == 0)
        {
            if (clip.tempClipList.Count == 0) throw new NoSoundsFound();

            clip.ClipList.AddRange(clip.tempClipList);
            clip.tempClipList.Clear();
        }

        AudioClip choosenClip = clip.ClipList[UnityEngine.Random.Range(0, clip.ClipList.Count)];
        RandomType clipRandomType = clip.RandomType;

        switch (clipRandomType)
        {
            default:
                break;
            case RandomType.NoDoubleRandom:

                if (clip.tempClip != null)
                    clip.ClipList.Add(clip.tempClip);

                clip.ClipList.Remove(choosenClip);
                clip.tempClip = choosenClip;

                break;
            case RandomType.NoDuplicateRandom:

                choosenClip = clip.ClipList[UnityEngine.Random.Range(0, clip.ClipList.Count)];
                clip.ClipList.Remove(choosenClip);
                clip.tempClipList.Add(choosenClip);

                break;
        }

        return choosenClip;
    }

    #if UNITY_EDITOR
    public void GenerateSoundEnum()
    {
        ShowCow.ShowCowInCmd("BRAVO LES SONS",2);

        string enumText = "public enum Sounds{";
        foreach(string name in _soundsDict.Keys)
        {
            if(name.Contains(" "))
                Debug.LogError("No spaces allowed in sound naming");

            enumText += name + ",";
        }
        enumText += "}";
        File.WriteAllText(_soundsEnumFilePath, enumText);
    }

    public void TestSound()
    {
        //play sound in editor ?
    }

    #endif

    public class NoSoundsFound : Exception
    {
        public override string Message => "No Sounds where found in the list.";
    }

    public void aohfonvoi(AudioClip clip)
    {

    }
}

public enum RandomType
{
    TrueRandom,
    NoDoubleRandom,
    NoDuplicateRandom
}
