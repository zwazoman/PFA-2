using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    #region Singleton
    private static SceneTransitionManager instance;

    public static SceneTransitionManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Scene Transition Manager");
                instance = go.AddComponent<SceneTransitionManager>();
            }
            return instance;
        }
    }
    #endregion

    /// <summary>
    /// structure à respecter dans les build settings
    /// </summary>
    public enum Scene
    {
        TitleScreen,
        WorldMap,
        //HealingStation,
        Kitchen,
        IngredientChest,
        Fight0,
        //Fight1,
        //Fight2
    }

    [SerializeField] CanvasGroup _CanvasGroup;
    [SerializeField] Image _image;
    [SerializeField] float _fadingDuration;

    bool _canChangeScene = true;

    private void Awake()
    {
        instance = this;
    }

    public void LoadScence(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToSceneByID(int scene) => TGoToSceneByID(scene);

    public async UniTask TGoToSceneByID(int scene)
    {
        if (_canChangeScene)
        {
            _canChangeScene = false;

            AsyncOperation o = SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Single);
            o.allowSceneActivation = false;
            await FadeOut();

            Debug.Log("Incremental Garbage collector enabled : " + GarbageCollector.isIncremental);
            Debug.Log("Collected garbage for 100ms. some garbage still remains : " +GarbageCollector.CollectIncremental(100).ToString());

            o.allowSceneActivation = true;
        }
    }

    public async UniTask GoToScene(string sceneName)
    {
        if (_canChangeScene)
        {
            _canChangeScene = false;

            AsyncOperation o = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            o.allowSceneActivation = false;
            await FadeOut();
            o.allowSceneActivation = true;
        }
    }

    async UniTask FadeIn(float? duration = null, Color? c = null)
    {
        //if (c.HasValue) _image.color = c.Value;
        //else _image.color = Color.black;

        if (!duration.HasValue) duration = _fadingDuration;

        _CanvasGroup.gameObject.SetActive(true);
        await _CanvasGroup.DOFade(0, duration.Value).AsyncWaitForCompletion().AsUniTask();
        _CanvasGroup.gameObject.SetActive(false);

    }

    async UniTask FadeOut(float? duration = null, Color? c = null)
    {
        //if (c.HasValue) _image.color = c.Value;
        //else _image.color = Color.black;

        if (!duration.HasValue) duration = _fadingDuration;

        _CanvasGroup.gameObject.SetActive(true);
        await _CanvasGroup.DOFade(1, duration.Value).AsyncWaitForCompletion().AsUniTask() ;
    }

    private async void Start()
    {
        _CanvasGroup.alpha = 1;
        await FadeIn();
    }
}
