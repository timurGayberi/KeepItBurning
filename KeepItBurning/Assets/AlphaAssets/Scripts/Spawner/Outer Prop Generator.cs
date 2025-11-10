using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterPropGenerator : MonoBehaviour
{
    [Header("Outer Floor & Props")]
    public GameObject outerTilePrefab;
    public List<OuterScriptableObjects> outerPropPrefabs = new List<OuterScriptableObjects>();

    [Header("Grid Settings")]
    public int width = 50;
    public int height = 50;
    public int innerSize = 26;
    public int spacing = 3;
    public float propDistance = 3f;

    [Header("Raycast Settings")]
    public float heightOfCheck = 50f;
    public float rangeOfCheck = 100f;
    public LayerMask layerMask;

    [Header("Don't Spawn On Settings")]
    public LayerMask DontSpawnOn;
    public Vector3 checkBoxSize = new Vector3(1f, 2f, 1f);

    private List<Vector3> outerFloorPositions = new List<Vector3>();
    private List<GameObject> spawnedTiles = new List<GameObject>();

    void Start()
    {
        ResetSpawnCounters();
        StartCoroutine(GenerateOuterFloor());
    }

    public IEnumerator GenerateOuterFloor()
    {
        int centerX = width / 2;
        int centerZ = height / 2;
        int halfInner = innerSize / 2;

        outerFloorPositions.Clear();
        spawnedTiles.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (x > centerX - halfInner && x < centerX + halfInner &&
                    z > centerZ - halfInner && z < centerZ + halfInner)
                    continue;

                Vector3 checkPos = new Vector3(x * spacing, heightOfCheck, z * spacing);

                if (Physics.Raycast(checkPos, Vector3.down, out RaycastHit hit, rangeOfCheck, layerMask))
                {
                    Vector3 tilePos = hit.point;

                    if (outerTilePrefab != null)
                    {
                        GameObject tile = Instantiate(outerTilePrefab, tilePos, Quaternion.identity);
                        spawnedTiles.Add(tile);
                    }

                    outerFloorPositions.Add(tilePos);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        PlaceOuterProps();

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject tile in spawnedTiles)
        {
            if (tile != null) Destroy(tile);
        }
    }

    public void PlaceOuterProps()
    {
        List<Vector3> usedPosition = new List<Vector3>();

        foreach (Vector3 pos in outerFloorPositions)
        {
            if (Random.value < 0.5f && !IsNearUsed(pos, usedPosition))
            {
                OuterScriptableObjects selected = GetRandomAvailableProp(outerPropPrefabs);
                if (selected == null) continue;

                Vector3 propPos = new Vector3(pos.x, pos.y, pos.z);
                SpawnProp(selected, propPos);

                usedPosition.Add(pos);
            }
        }
    }

    private void SpawnProp(OuterScriptableObjects data, Vector3 pos)
    {
        if (data == null || data.prefab == null)
            return;

        if (data.spawnLimitType == SpawnLimitType.Limited &&
        data.currentSpawnedCount >= data.maxSpawnCount)
            return;

        CheckboxDebugger.DisplayBox(pos, checkBoxSize / 2f, Quaternion.identity, DontSpawnOn);

        if (Physics.CheckBox(pos, checkBoxSize / 2f, Quaternion.identity, DontSpawnOn))
            return;

        Instantiate(data.prefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0));

        if (data.spawnLimitType == SpawnLimitType.Limited)
            data.currentSpawnedCount++;
    }

    private OuterScriptableObjects GetRandomAvailableProp(List<OuterScriptableObjects> list)
    {
        List<OuterScriptableObjects> available = list.FindAll(p => p != null);

        if (available.Count == 0)
            return null;

        return available[Random.Range(0, available.Count)];
    }

    private void ResetSpawnCounters()
    {
        foreach (var outer in outerPropPrefabs)
        {
            if (outer != null)
                outer.currentSpawnedCount = 0;
        }
    }

    private bool IsNearUsed(Vector3 pos, List<Vector3> used)
    {
        foreach (var item in used)
        {
            if (Vector3.Distance(pos, item) < spacing * propDistance)
                return true;
        }
        return false;
    }
}