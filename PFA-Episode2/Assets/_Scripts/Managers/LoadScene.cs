using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script qui fait changer de scène avec un écran de chargement
/// </summary>
public class LoadScene : MonoBehaviour
{
    [SerializeField] private GameObject _panelLoading;
    [SerializeField] private GameObject _logo;
    [SerializeField] private Animator _animator;
    private bool _loading;

    #region Singleton
    public static LoadScene Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    // Fonction pour passer sur l'écran de chargement et load la scene
    public IEnumerator SceneLoadingOn(string sceneName)
    {
        _panelLoading.SetActive(true);
        yield return new WaitForSeconds(1);
        _logo.SetActive(true);
        _animator.SetBool("Activate", true);
        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadSceneAsync(sceneName);
        _loading = true;
        StartCoroutine(SceneLoadingOff());
    }

    // Fonction pour enlever l'écran de chargement
    public IEnumerator SceneLoadingOff()
    {
        if (!_loading) yield return null;

        yield return new WaitForSeconds(4.9f);
        _logo.SetActive(false);
        _animator.SetBool("Activate", false);
        yield return new WaitForSeconds(1);
        _panelLoading.SetActive(false);
    }
}
