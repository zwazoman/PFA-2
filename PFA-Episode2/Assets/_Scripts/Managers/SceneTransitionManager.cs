using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public async Task GoToScene(int buildIndex)
    {
        if (_canChangeScene)
        {
            _canChangeScene = false;

            AsyncOperation o = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
            o.allowSceneActivation = false;
            await FadeOut();
            o.allowSceneActivation = true;
        }
        
    }

    public async Task FadeIn(float? duration = null, Color? c = null)
    {
        if (c.HasValue) _image.color = c.Value;
        else _image.color = Color.black;

        if (!duration.HasValue) duration = _fadingDuration;

            _CanvasGroup.DOFade(0, duration.Value);
        await Task.Delay(Mathf.CeilToInt(duration.Value * 1000));
    }

    public async Task FadeOut(float? duration = null, Color? c = null)
    {
        if (c.HasValue) _image.color = c.Value;
        else _image.color = Color.black;

        if (!duration.HasValue) duration = _fadingDuration;

        _CanvasGroup.DOFade(1, duration.Value);
        await Task.Delay(Mathf.CeilToInt(duration.Value * 1000));
    }

    private void Start()
    {
        //_CanvasGroup.alpha = 1;
        //FadeIn();
    }
}
