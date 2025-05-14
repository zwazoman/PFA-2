using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SpawnSetup
{
    public Spawner playerSpawner;

    public List<EnemySpawnerGroup> enemySpawnerGroups = new();
}
