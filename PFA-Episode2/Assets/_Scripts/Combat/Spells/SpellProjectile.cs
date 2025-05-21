using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [HideInInspector] public Entity target;

    [HideInInspector] public Mesh mesh;

    [SerializeField] MeshFilter _filter;

    private void Start()
    {
        _filter.mesh = mesh;
    }

    public async UniTask FlyToward(Entity target)
    {
        await transform.DOMove(target.transform.position, .7f);
    }
}
