using DG.Tweening;
using UnityEngine;

/// <summary>
/// Script qui fait bouger de l'UI comme les cartes de Balatro
/// </summary>
public class MoveCardUI : MonoBehaviour
{
    private void Start() { MoveCard(); }

    private async void MoveCard()
    {
        int x = Random.Range(0, 11);
        int y = Random.Range(0, 11);
        int z = Random.Range(0, 2);
        float delay = Random.Range(1f, 2f);
        Sequence seq = DOTween.Sequence();
        if (gameObject == null) { return; }
        seq.Join(transform.DORotate(new Vector3(x, y, z), delay));
        await seq.AsyncWaitForCompletion();
        MoveCard();
    }

}
