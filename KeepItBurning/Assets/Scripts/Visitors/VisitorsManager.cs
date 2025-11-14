using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisitorsManager : MonoBehaviour
{

    [SerializeField] private List<GameObject> visitorPrefabs = new List<GameObject>();
    [SerializeField] private int maxVisitors;
    [SerializeField] private float CountToSpawVisitors;
    [SerializeField] private float TimeToSpawVisitors;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [SerializeField] public Transform campfire;

    private List<GameObject> activeVisitors = new List<GameObject>();
    private List<Transform> availableSpawnPoints = new List<Transform>();
    private Dictionary<GameObject, Transform> visitorSpawnMap = new Dictionary<GameObject, Transform>();

    public int currentVisitors = 0;


    void Start()
    {
        availableSpawnPoints = new List<Transform>(spawnPoints);
        SpawnVisitor();
    }

    void Update()
    {
        CountToSpawVisitors += Time.deltaTime;
        if (CountToSpawVisitors > TimeToSpawVisitors)
        {
            CountToSpawVisitors = 0f;
            SpawnVisitor();
        }

    }

    public void SpawnVisitor()
    {
        if (activeVisitors.Count >= maxVisitors)
        {
            return;
        }

        if (availableSpawnPoints.Count == 0)
        {
            return;
        }
        
        GameObject chosenPrefab = visitorPrefabs[Random.Range(0, visitorPrefabs.Count)];
        int randomIndex = Random.Range(0, availableSpawnPoints.Count);
        Transform chosenPoint = availableSpawnPoints[randomIndex];

        availableSpawnPoints.RemoveAt(randomIndex);

        GameObject newVisitor = Instantiate(chosenPrefab, chosenPoint.position, Quaternion.identity);

        if (campfire != null)
        {
            newVisitor.transform.LookAt(campfire);
        }
        activeVisitors.Add(newVisitor);

        visitorSpawnMap[newVisitor] = chosenPoint;
        currentVisitors++;
        SoundManager.Play(SoundAction.NewVisitor);
    }

    public void RemoveVisitor(GameObject visitor)
    {
        if (!activeVisitors.Contains(visitor))
            return;

        activeVisitors.Remove(visitor);

        if (visitorSpawnMap.ContainsKey(visitor))
        {
            availableSpawnPoints.Add(visitorSpawnMap[visitor]);
            visitorSpawnMap.Remove(visitor);
        }

        Destroy(visitor);
        currentVisitors--;
        TimeToSpawVisitors-=0.75f;
    }
}
