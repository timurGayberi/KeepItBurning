/*using PlayerScripts;
using Score;
using UnityEngine;

namespace GamePlay.Collectibles
{
    public class AlphaBlueMushroom : CollectibleBase
    {
        public float BlueMushroomScore = 676f;
        protected override void OnCollected(PlayerInventory inventory)
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(BlueMushroomScore);
                Debug.Log("Blue Mushroom Collected");
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