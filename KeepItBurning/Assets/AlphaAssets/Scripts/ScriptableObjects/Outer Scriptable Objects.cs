using UnityEngine;

[CreateAssetMenu(fileName = "OuterScriptableObjects", menuName = "Scriptable Objects/OuterScriptableObjects", order = 0)]
public class OuterScriptableObjects : ScriptableObject
{
    public GameObject prefab;
    [Range(0, 100)] public float spawnChance;

    [Header("Spawn Limit Settings")]
    public SpawnLimitType spawnLimitType = SpawnLimitType.Infinite;
    public int maxSpawnCount = 10;

    [HideInInspector] public int currentSpawnedCount = 0;
}