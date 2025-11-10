/*using PlayerScripts;
using Score;
using UnityEngine;

namespace GamePlay.Collectibles
{
    public class AlphaRedMushroom : CollectibleBase
    {
        public float RedMushroomScore = 1113f;
        protected override void OnCollected(PlayerInventory inventory)
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(RedMushroomScore);
                Debug.Log("Mushroom Collected");
            }
            Destroy(gameObject);

            if (originSpawner != null && originPoint != null)
            {
                riginSpawner.RestartSpawn(originPoint);
            }
        }
    }
}
*/
