using System;
using UnityEngine;


public enum GroundType
{
    Concrete,
    Grass
}

public class PlayerFootstepsHandler : MonoBehaviour
{
    [SerializeField] Transform _stepSocket;
    [SerializeField] Terrain _terrain;

    public void WalkStep(int isRunning = 0)
    {
        GroundType groundType = CheckGroundType();

        Sounds sound = 0;

        switch (groundType)
        {
            case GroundType.Concrete:
                if (!Convert.ToBoolean(isRunning))
                    sound = Sounds.WalkConcrete;
                else
                    sound = Sounds.RunConcrete;
                    break;
            case GroundType.Grass:
                if (!Convert.ToBoolean(isRunning))
                    sound = Sounds.WalkGrass;
                else
                    sound = Sounds.RunGrass;
                break;
        }

        SFXManager.Instance.PlaySFXClipAtPosition(sound, _stepSocket.position, true);
    }

    GroundType CheckGroundType()
    {
        RaycastHit hit;
        Physics.Raycast(_stepSocket.position, Vector3.down, out hit);

        Terrain currentTerrain;
        hit.collider.gameObject.TryGetComponent<Terrain>(out currentTerrain);

        GroundType groundType = GroundType.Concrete;
        return groundType;
    }
}
