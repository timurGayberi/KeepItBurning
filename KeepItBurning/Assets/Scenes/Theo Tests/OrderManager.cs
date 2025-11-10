using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [SerializeField] public Order orderPrefab; // Requests prefab 
    [SerializeField] public Transform ordersParent; // Where the orders will appear in the UI 
    [SerializeField] private int maxActiveOrders;
    [SerializeField] private float timeBetweenOrders;

    private List<Order> activeOrders = new List<Order>();
    public float spawnTimer;

    public void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= timeBetweenOrders)
        {
            spawnTimer = 0f;
            TrySpawnOrder();
        }

        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            Order order = activeOrders[i];
            if (order == null)
            {
                activeOrders.RemoveAt(i);
                continue;
            }

            if (!order.enabled) 
                activeOrders.RemoveAt(i);
        }
    }

    private void TrySpawnOrder()
    {
        if (activeOrders.Count >= maxActiveOrders)
            return;

        Order newOrder = Instantiate(orderPrefab, ordersParent);
        newOrder.Setup();

        activeOrders.Add(newOrder);

    }

    public void DeliverOrder()
    {

        if (activeOrders.Count == 0)
        {
            return;
        }

        Order order = activeOrders[0];
        order.SetDelivered();

        activeOrders.Remove(order);
        Destroy(order.gameObject);
    }
}
