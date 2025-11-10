using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AreaProp
{
    public InnerScriptableObjects propData;
    [Range(0f, 100f)]
    public float spawnChance = 50f;
}

[System.Serializable]
public class SpawnArea
{
    public string areaName = "Area";
    public Vector2 minimumPosition;
    public Vector2 maximumPosition;

    public List<AreaProp> areaProps = new List<AreaProp>();

    public float spawnDensityMultiplier = 1f;
}