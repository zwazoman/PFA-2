using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingPotVisuals : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] Animator _animator;
    [SerializeField] RectTransform _imageTransform;
    [SerializeField] RawImage _UiImage;
    [SerializeField] Camera _renderCamera;
    [SerializeField] List<MeshFilter> _boneSockets;//1,2,3 -> ings, 4 -> dish

    [SerializeField] ParticleSystem _sauceParticleSystem;

    RenderTexture _rt;
    const float TweenDuration = .3f;
    async void Start()
    {
        //set texture dimensions
        await Awaitable.EndOfFrameAsync();
        Debug.Log("--");
        Debug.Log(_imageTransform.name, _imageTransform);
        Debug.Log(_imageTransform.rect.width);
        Debug.Log(_imageTransform.rect.height);
        
        _rt = new((int)_imageTransform.rect.width, (int)_imageTransform.rect.height, 1);
        _rt.antiAliasing = 2;
        _rt.Create();

        _renderCamera.targetTexture = _rt;
        _UiImage.texture = _rt; 
    }
    private void OnDestroy()
    {
        _rt.Release();
    }

    public void UpdateIngredientsList(List<Ingredient> ingredients)
    {
        byte c = 0;
        for(int i = 0; i < 3; i++)
        {
            if ( i< ingredients.Count && ingredients[i] != null )
            {
                c++;
                _boneSockets[i].transform.parent.DOScale(1, TweenDuration).SetEase(Ease.OutBack);
                _boneSockets[i].sharedMesh = ingredients[i].mesh;
            }
            else
            {
                _boneSockets[i].transform.parent.DOScale(0, TweenDuration).SetEase(Ease.OutBack);
                _boneSockets[i].sharedMesh = null;
            }
        }

        float alpha = (float)c / 3f;
        ParticleSystem.EmissionModule m = _sauceParticleSystem.emission;
        m.rateOverTime = alpha*10;
        _sauceParticleSystem.playbackSpeed = .5f + alpha * .5f;
        _animator.SetLayerWeight(1,alpha);
    }

    public void PlayCookedDishAnim(SpellData spell)
    {
        UpdateIngredientsList(new List<Ingredient>());
        _boneSockets[3].sharedMesh = GameManager.Instance.staticData.Visuals[spell.IngredientsCombination].mesh;

        _animator.SetTrigger("ApplyDish");
    }
}
