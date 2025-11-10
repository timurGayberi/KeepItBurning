using UnityEngine;

[CreateAssetMenu(fileName = "InnerScriptableObjects", menuName = "Scriptable Objects/InnerScriptableObjects")]
public class InnerScriptableObjects : ScriptableObject
{
    public GameObject prefab;
    public SpawnLimitType spawnLimitType;
    public int maxSpawnCount;
    [HideInInspector] public int currentSpawnedCount;
}
