using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// Script qui fait bouger de l'UI comme les cartes de Balatro
/// </summary>
public class MoveCardUI : MonoBehaviour
{
    [SerializeField] private int maxRotaX = 11;
    [SerializeField] private int maxRotaY = 11;
    [SerializeField] private int maxRotaZ = 2;

    private void Start()
    {
        _ = MoveCardAsync(this.GetCancellationTokenOnDestroy()); //Sinon erreur
    }

    private async UniTask MoveCardAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int x = UnityEngine.Random.Range(0, maxRotaX);
                int y = UnityEngine.Random.Range(0, maxRotaY);
                int z = UnityEngine.Random.Range(0, maxRotaZ);
                float delay = UnityEngine.Random.Range(1f, 2f);

                Sequence seq = DOTween.Sequence();
                seq.Join(transform.DORotate(new Vector3(x, y, z), delay));

                await seq.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

}
