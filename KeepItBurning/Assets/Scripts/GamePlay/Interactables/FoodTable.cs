using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using General;
using Player;

namespace GamePlay.Interactables
{
    public enum FoodType
    {
        Marshmallow,
        HotChocolate,
        Sausage
    }

    public enum TableStatus
    {
        Ready,      // Has food available
        Empty       // No food available, waiting to refill
    }

    public class FoodTable : MonoBehaviour, IInteractable
    {
        [Header("Food Configuration")]
        [Tooltip("What type of food does this table provide?")]
        [SerializeField]
        private FoodType foodType = FoodType.Marshmallow;

        [Tooltip("The Food Visual Prefab (non-interactable) shown on the table.")]
        public GameObject foodVisualPrefab;

        [Header("Table Display Settings")]
        [Tooltip("Maximum number of food items visible on the table at once.")]
        [SerializeField]
        private int maxFoodOnTable = 3;

        [Tooltip("Positions where food items appear on the table (relative to this GameObject).")]
        [SerializeField]
        private Transform[] foodSpawnPoints;

        [Tooltip("If no spawn points assigned, use random positions on table.")]
        [SerializeField]
        private float randomSpawnRadius = 0.5f;

        [Tooltip("Height offset for spawning visual items.")]
        [SerializeField]
        private float spawnHeight = 0.1f;

        [Header("Refill Settings")]
        [Tooltip("How long before the table adds one new food item.")]
        [SerializeField]
        private float refillTime = 10f;

        [Header("Interaction Settings")]
        [SerializeField]
        private float interactionDuration = 0.5f;

        [Tooltip("What cook state does this table provide food in?")]
        [SerializeField]
        private CollectibleBase.CookState cookState = CollectibleBase.CookState.Raw;

        private List<GameObject> spawnedFoodVisuals = new List<GameObject>();
        private int currentFoodCount = 0;
        private Coroutine refillCoroutine;

        private void Start()
        {
            if (foodVisualPrefab == null)
            {
                Debug.LogError($"[FOOD TABLE] Food Visual Prefab is NULL! Please assign it in the Inspector.");
                return;
            }

            // Spawn initial food on the table
            for (int i = 0; i < maxFoodOnTable; i++)
            {
                SpawnFoodVisual();
            }

            // Start refill coroutine to maintain max food
            refillCoroutine = StartCoroutine(RefillCoroutine());
        }

        public InteractionData GetInteractionData()
        {
            string foodName = GetFoodName();
            string prompt = $"Take {foodName} ({currentFoodCount}/{maxFoodOnTable})";
            float duration = interactionDuration;

            if (currentFoodCount <= 0)
            {
                prompt = $"No {foodName} available";
                duration = -1f;
            }

            return new InteractionData
            {
                promptText = prompt,
                actionDuration = duration
            };
        }

        public void Interact()
        {
            if (currentFoodCount <= 0)
            {
                return;
            }

            // Get player inventory
            PlayerInventory playerInventory = ServiceLocator.GetService<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("[FOOD TABLE] Could not find PlayerInventory service!");
                return;
            }

            // Check if player is already holding something
            if (playerInventory.IsHoldingFoodItem())
            {
                return;
            }

            // Give food to player with cook state
            int foodID = GetFoodCollectibleID();
            playerInventory.SetHeldFoodItem(foodID, cookState);

            // Remove one visual food item from the table
            RemoveFoodVisual();
        }

        public void StopInteraction()
        {
        }

        private IEnumerator RefillCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(refillTime);

                // Add a new food visual if below max
                if (currentFoodCount < maxFoodOnTable)
                {
                    SpawnFoodVisual();
                }
            }
        }

        private void SpawnFoodVisual()
        {
            if (foodVisualPrefab == null)
            {
                return;
            }

            Vector3 spawnPosition;
            Quaternion spawnRotation;

            // Use spawn points if assigned, otherwise random position
            if (foodSpawnPoints != null && foodSpawnPoints.Length > 0 && currentFoodCount < foodSpawnPoints.Length)
            {
                // Use position AND rotation from spawn point
                spawnPosition = foodSpawnPoints[currentFoodCount].position;
                spawnRotation = foodSpawnPoints[currentFoodCount].rotation;
            }
            else
            {
                // Random position on table
                var randomCircle = Random.insideUnitCircle * randomSpawnRadius;
                spawnPosition = new Vector3(
                    transform.position.x + randomCircle.x,
                    transform.position.y + spawnHeight,
                    transform.position.z + randomCircle.y
                );
                // Random rotation around Y axis
                spawnRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            }

            GameObject visual = Instantiate(foodVisualPrefab, spawnPosition, spawnRotation, transform);

            // Disable collider so it's not collectible (just visual)
            var collider = visual.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            spawnedFoodVisuals.Add(visual);
            currentFoodCount++;
        }

        private void RemoveFoodVisual()
        {
            if (spawnedFoodVisuals.Count > 0)
            {
                GameObject toRemove = spawnedFoodVisuals[spawnedFoodVisuals.Count - 1];
                spawnedFoodVisuals.RemoveAt(spawnedFoodVisuals.Count - 1);
                Destroy(toRemove);
                currentFoodCount--;
            }
        }

        private string GetFoodName()
        {
            switch (foodType)
            {
                case FoodType.Marshmallow:
                    return "Marshmallow";
                case FoodType.HotChocolate:
                    return "Hot Chocolate";
                case FoodType.Sausage:
                    return "Sausage";
                default:
                    return "Food";
            }
        }

        // Helper method to get the CollectibleID for this food type
        public int GetFoodCollectibleID()
        {
            switch (foodType)
            {
                case FoodType.Marshmallow:
                    return CollectibleIDs.MARSHMALLOW;
                case FoodType.HotChocolate:
                    return CollectibleIDs.HOT_CHOCOLATE;
                case FoodType.Sausage:
                    return CollectibleIDs.SAUSAGE;
                default:
                    return CollectibleIDs.DEFAULT_ITEM;
            }
        }
    }
}
