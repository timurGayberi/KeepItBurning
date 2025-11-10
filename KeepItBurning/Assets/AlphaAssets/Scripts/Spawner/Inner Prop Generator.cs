using System.Collections.Generic;
using UnityEngine;

public class InnerPropGenerator : MonoBehaviour
{
    [Header("Global Settings")]
    public float distanceBetweenCheck = 1f;
    public float heightOfCheck = 50f;
    public float rangeOfCheck = 100f;
    public LayerMask groundLayer;

    [Header("Spawn Areas")]
    public List<SpawnArea> spawnAreas = new List<SpawnArea>();

    [Header("No Spawn")]
    public LayerMask noSpawnLayer;
    public Vector3 checkBoxSize = new Vector3(1f, 2f, 1f);

    private void Start()
    {
        ResetCounts();
        InvokeRepeating(nameof(SpawnProps), 1f, 1f);
    }

    private void ResetCounts()
    {
        foreach (var area in spawnAreas)
        {
            foreach (var ap in area.areaProps)
            {
                if (ap.propData != null)
                    ap.propData.currentSpawnedCount = 0;
            }
        }
    }

    void SpawnProps()
    {
        foreach (var area in spawnAreas)
        {
            for (float x = area.minimumPosition.x; x < area.maximumPosition.x; x += distanceBetweenCheck)
            {
                for (float z = area.minimumPosition.y; z < area.maximumPosition.y; z += distanceBetweenCheck)
                {
                    Vector3 castPos = new Vector3(x, heightOfCheck, z);

                    if (!Physics.Raycast(castPos, Vector3.down, out RaycastHit hit, rangeOfCheck, groundLayer))
                        continue;

                    foreach (var ap in area.areaProps)
                    {
                        if (ap.propData == null || ap.propData.prefab == null) 
                            continue;

                        if (ap.propData.spawnLimitType == SpawnLimitType.Limited &&
                            ap.propData.currentSpawnedCount >= ap.propData.maxSpawnCount)
                            continue;

                        float finalChance = ap.spawnChance * area.spawnDensityMultiplier;

                        if (Random.Range(0f, 100f) <= finalChance)
                            SpawnProp(ap.propData, hit.point);
                    }
                }
            }
        }
    }

    private void SpawnProp(InnerScriptableObjects data, Vector3 pos)
    {
        if (Physics.CheckBox(pos, checkBoxSize / 2f, Quaternion.identity, noSpawnLayer))
            return;

        Instantiate(data.prefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0));

        if (data.spawnLimitType == SpawnLimitType.Limited)
            data.currentSpawnedCount++;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var area in spawnAreas)
        {
            Vector3 min = new Vector3(area.minimumPosition.x, 0, area.minimumPosition.y);
            Vector3 max = new Vector3(area.maximumPosition.x, 0, area.maximumPosition.y);
            Vector3 center = (min + max) / 2f;
            Vector3 size = max - min;

            Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
            Gizmos.DrawCube(center, size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, size);
        }
    }
}