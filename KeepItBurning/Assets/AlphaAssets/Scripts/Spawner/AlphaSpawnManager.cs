using GamePlay.Collectibles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaSpawnManager : MonoBehaviour
{
    public LayerMask noSpawnLayer;
    public Vector3 checkBoxSize = new Vector3(1f, 1f, 1f);
    public List<SpawnPointData> spawnPoints = new List<SpawnPointData>();
    private Dictionary<SpawnPointData, Coroutine> activeRoutines = new Dictionary<SpawnPointData, Coroutine>();

    private void Start()
    {
        ResetCounts();

        foreach (var point in spawnPoints)
        {
            if (point != null && point.spawnTransform != null && point.propData != null)
                StartSpawnRoutine(point);
        }
    }

    private void ResetCounts()
    {
        foreach (var point in spawnPoints)
        {
            if (point != null && point.propData != null)
                point.propData.currentSpawnedCount = 0;
        }
    }

    private void StartSpawnRoutine(SpawnPointData point)
    {
        if (activeRoutines.ContainsKey(point) && activeRoutines[point] != null)
        {
            StopCoroutine(activeRoutines[point]);
        }

        Coroutine routine = StartCoroutine(SpawnRoutine(point));
        activeRoutines[point] = routine;
    }

    private IEnumerator SpawnRoutine(SpawnPointData point)
    {
        while (true)
        {
            yield return new WaitForSeconds(point.spawnInterval);

            if (point == null || point.spawnTransform == null || point.propData == null)
                yield break;

            if (point.propData.spawnLimitType == SpawnLimitType.Limited &&
                point.propData.currentSpawnedCount >= point.propData.maxSpawnCount)
                yield break;

            TrySpawn(point.propData, point.spawnTransform.position, point.spawnTransform.rotation, point);

            activeRoutines[point] = null;
            yield break;
        }
    }

    private void TrySpawn(InnerScriptableObjects data, Vector3 pos, Quaternion rot, SpawnPointData point)
    {
        if (Physics.CheckBox(pos, checkBoxSize / 2f, Quaternion.identity, noSpawnLayer))
            return;

        Quaternion finalRot = rot * Quaternion.Euler(-90f, 0f, 0f);

        GameObject spawned = Instantiate(data.prefab, pos, finalRot);

        var collectible = spawned.GetComponent<CollectibleBase>();
        if (collectible != null)
        {
            //collectible.originSpawner = this;
            //collectible.originPoint = point;
        }

        if (data.spawnLimitType == SpawnLimitType.Limited)
            data.currentSpawnedCount++;
    }

    public void RestartSpawn(SpawnPointData point)
    {
        StartSpawnRoutine(point);
    }
}

[System.Serializable]
public class SpawnPointData
{
    public Transform spawnTransform;
    public InnerScriptableObjects propData;
    public float spawnInterval;
}