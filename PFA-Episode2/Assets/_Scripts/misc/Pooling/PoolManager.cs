using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //singleton
    private static PoolManager instance;

    public static PoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Pool Manager");
                instance = go.AddComponent<PoolManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }


    public Pool ArrowPool;
    public Pool ProjectilePool;
    
    //VFXs
    public Pool vfx_GodrayPool;
}
