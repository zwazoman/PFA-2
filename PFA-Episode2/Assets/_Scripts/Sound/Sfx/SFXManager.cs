using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AYellowpaper.SerializedCollections;
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
        DontDestroyOnLoad(gameObject);
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
    /// renvoie l'audioSource dans la pool et dï¿½sactive son gameObject
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
    /// rï¿½cupï¿½re une audioSource dans la pool et joue un des sons possibles
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
            audioSource.clip = audioClip; // assigne le clip random ï¿½ l'audiosource
        }
        catch(Exception e)
        {
            if (e is NoSoundsFound)
                Debug.LogWarning(e.Message, this);
            else
                Debug.LogException(e);
            
            return null;
        }

        return AudioSourceHandle(audioSource, choosenClip.Volume * volumefactor, choosenClip.Pitch * pitchfactor, true, false, choosenClip.MixerGroup);
    }

    public AudioSource PlaySFXClip(AudioClip audioClip, float volume = 1f, float pitch = 1f)
    {
        AudioSource audioSource = UseFromPool();

        audioSource.clip = audioClip; // assigne le clip random ï¿½ l'audiosource

        return AudioSourceHandle(audioSource, volume, pitch);
    }

    public AudioSource PlaySFXClip(String audioClipName, float volumeFactor = 1f, float pitchFactor = 1f)
    {
        AudioSource audioSource = UseFromPool();

        Clip choosenClip = null;

        foreach(String name in _soundsDict.Keys)
        {
            if (name == audioClipName)
                choosenClip = _soundsDict[name];
        }

        if (choosenClip == null)
        {
            Debug.LogWarning("incorrect sound Name");
            return null;
        }

        audioSource.clip = ChooseRandomClip(choosenClip);

        return AudioSourceHandle(audioSource, choosenClip.Volume * volumeFactor, choosenClip.Pitch * pitchFactor);
    }

    #endregion

    #region PlaySFXClipAtPosition

    /// <summary>
    /// chosit une liste dans une liste via un enum et choisit ensuite un audioclip au hasard dans ce dernier pour le jouer. le son sera placï¿½ a un endroit donnï¿½ et prendra en compte ou non les effets.
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
        catch(Exception e) when (e is not NoSoundsFound)
        {
            Debug.LogWarning(e.Message,this);
            return null;
        }catch(Exception e)
        {
            Debug.LogException(e);
            return null;
        }

        audioSource.gameObject.transform.position = position;

        return AudioSourceHandle(audioSource, choosenClip.Volume * volumeFactor, choosenClip.Pitch * pitchFactor, is2DSound, bypassesEffects, choosenClip.MixerGroup);

    }

    public AudioSource PlaySFXClipAtPosition(AudioClip choosenSound, Vector3 position, bool is2DSound = false, bool bypassesEffects = false, float volume = 1f, float pitch = 1f)
    {
        AudioSource audioSource = UseFromPool();

        audioSource.clip = choosenSound;

        audioSource.gameObject.transform.position = position;

        return AudioSourceHandle(audioSource, volume, pitch, is2DSound, bypassesEffects);
    }

    public AudioSource PlaySFXClipAtPosition(String audioClipName, Vector3 position, bool is2DSound = false, bool bypassesEffects = false, float volumeFactor = 1f, float pitchFactor = 1f)
    {
        AudioSource audioSource = UseFromPool();

        Clip choosenClip = null;

        foreach (String name in _soundsDict.Keys)
        {
            if (name == audioClipName)
                choosenClip = _soundsDict[name];
        }

        if (choosenClip == null)
        {
            Debug.LogWarning("incorrect sound Name");
            return null;
        }

        audioSource.clip = ChooseRandomClip(choosenClip);

        audioSource.gameObject.transform.position = position;

        return AudioSourceHandle(audioSource, choosenClip.Volume * volumeFactor, choosenClip.Pitch * pitchFactor, is2DSound, bypassesEffects);
    }

    #endregion

    /// <summary>
    /// gï¿½re le retour a la pool d'une audiosource s'arrï¿½tant apres la fin de son clip
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
    /// gï¿½re les rï¿½gales de l'audiosource et joue le son.
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <param name="is2D"></param>
    /// <param name="bypassesEffects"></param>
    /// <returns></returns>
    AudioSource AudioSourceHandle(AudioSource audioSource, float volume = 1, float pitch = 1, bool is2D = true, bool bypassesEffects = false, AudioMixerGroup choosenMixerGroup = null)
    {
        audioSource.volume = volume; // assigne le volume ï¿½ l'audiosource
        audioSource.pitch = pitch; // assigne le pitch ï¿½ l'audiosource
        if (!is2D) audioSource.spatialBlend = 1; // gï¿½re bien le blend rapport a l'audiolistener
        audioSource.bypassEffects = bypassesEffects; // gï¿½re le bypass ou non des effets
        if(choosenMixerGroup != null) audioSource.outputAudioMixerGroup = choosenMixerGroup; //selects the right audioMixer


        audioSource.Play(); // joue le son
<<<<<<< Updated upstream

        float clipLength = audioSource.clip.length; // détermine la longueur du son
=======
        
        float clipLength = audioSource.clip.length; // dï¿½termine la longueur du son
>>>>>>> Stashed changes

        StartCoroutine(HandleSoundEnd(audioSource, clipLength));

        return audioSource;
    }

    /// <summary>
    /// choisit un clip random dans le clip donnï¿½. trueRandom dï¿½fini si oui ou non, la liste devra-t-ï¿½tre parcourue entiï¿½rement avant de se rï¿½pï¿½ter
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

    #endif

    public class NoSoundsFound : Exception
    {
        public override string Message => "No Sounds where found in the list.";
    }
}

public enum RandomType
{
    TrueRandom,
    NoDoubleRandom,
    NoDuplicateRandom
}
