using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    PooledObject _pool;

    [SerializeField] MeshFilter _filter;

    public void OnInstantiatedByPool()
    {
        TryGetComponent(out _pool);
    }

    public async UniTask Launch(Entity target, Mesh spellMesh = null)
    {
        if(spellMesh != null)
            _filter.mesh = spellMesh;

        await transform.DOMove(target.eatSocket.position, .7f);

        _pool.GoBackIntoPool();
    }
}
