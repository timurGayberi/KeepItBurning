/*using PlayerScripts;
using Score;
using UnityEngine;

namespace GamePlay.Collectibles
{
    public class AlphaYellowMushroom : CollectibleBase
    {
        public float YellowMushroomScore = 333f;
        protected override void OnCollected(PlayerInventory inventory)
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(YellowMushroomScore);
                Debug.Log("Yellow Mushroom Collected");
            }
            Destroy(gameObject);

            if (originSpawner != null && originPoint != null)
            {
                originSpawner.RestartSpawn(originPoint);
            }
        }
    }
}

*/