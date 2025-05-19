using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Script qui fait bouger de l'UI comme les cartes de Balatro
/// </summary>
public class MoveCardUI : MonoBehaviour
{
    [SerializeField] private int maxRotaX = 11;
    [SerializeField] private int maxRotaY = 11;
    [SerializeField] private int maxRotaZ = 2;

    private void Start() { MoveCard(); }

    //fils de pute mateo
    private async UniTask MoveCard()
    {
        while(isActiveAndEnabled)
        {
            int x = Random.Range(0, maxRotaX);
            int y = Random.Range(0, maxRotaY);
            int z = Random.Range(0, maxRotaZ);
            float delay = Random.Range(1f, 2f);
            Sequence seq = DOTween.Sequence();
            if (gameObject == null) { return; }
            seq.Join(transform.DORotate(new Vector3(x, y, z), delay));
            await seq.AsyncWaitForCompletion().AsUniTask();
        }
    }

}
