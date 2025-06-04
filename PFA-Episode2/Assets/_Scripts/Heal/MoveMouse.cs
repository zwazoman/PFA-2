using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class MoveMouse : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _speed;

    public static MoveMouse Instance;

    private void Awake() { Instance = this; }

    public async UniTask Moving()
    {
        _animator.PlayAnimationBool("Move");
        await transform.DOMove(_target.position, 1f / _speed).SetEase(Ease.InOutCubic).AsyncWaitForCompletion().AsUniTask();
        _animator.EndAnimationBool("Move");
    }
}
