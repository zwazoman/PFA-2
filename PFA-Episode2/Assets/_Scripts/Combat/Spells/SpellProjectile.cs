using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    PooledObject _pool;

    [SerializeField] float rotationSpeed = 50f;
    
    [SerializeField] MeshFilter _filter;



    public void OnInstantiatedByPool()
    {
        TryGetComponent(out _pool);
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 1 * rotationSpeed, 0) * Time.deltaTime);
    }

    public async UniTask Launch(Entity caster, Entity target, Mesh spellMesh = null, float launchSpeed = .15f)
    {
        if(spellMesh != null)
            _filter.mesh = spellMesh;

        if(target != caster)
            await transform.DOMove(target.eatSocket.position, launchSpeed * (target.eatSocket.position - transform.position).magnitude);

        _pool.GoBackIntoPool();
    }

    public async UniTask Launch(Entity caster, WayPoint tile, Mesh spellMesh = null, float launchSpeed = .7f)
    {
        if (spellMesh != null)
            _filter.mesh = spellMesh;

        if (tile != caster.currentPoint)
            await transform.DOMove(tile.transform.position + Vector3.up * .5f, launchSpeed / (tile.transform.position + Vector3.up * .5f - transform.position).magnitude);

        _pool.GoBackIntoPool();
    }
}
