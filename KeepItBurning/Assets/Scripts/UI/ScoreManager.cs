using TMPro;
using UnityEngine;
using General;

namespace Score
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [SerializeField] public HappinessManager happinessManager;
        [SerializeField] public TimeManager timeManager;
        [SerializeField] public TextMeshProUGUI scoreText;

        [SerializeField] public float baseCorrectlyCookedFood = 100f;
        [SerializeField] public float baseIncorrectlyCookedFood = 25f;
        [SerializeField] public float baseTrash = 50f;

        public float Score;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            try
            {
                ServiceLocator.RegisterService<ScoreManager>(this);
            }
            catch (System.InvalidOperationException e)
            {
                Debug.LogError($"Failed to register ScoreManager: {e.Message}");
            }
        }

        private void OnDestroy()
        {
            try
            {
                ServiceLocator.UnregisterService<ScoreManager>(this);
            }
            catch (System.InvalidOperationException e)
            {
                Debug.LogWarning($"Failed to unregister ScoreManager: {e.Message}");
            }
        }

        private void Update()
        {
            timeManager.Timer();
        }

        public void AddScore(float AddScore)
        {
            Score += AddScore * timeManager.TimeMultiplier * Time.deltaTime;
            scoreText.text=Score.ToString("F0");
        }

        public void AddCorrectlyCookedFoodScore()
        {
            happinessManager.Increase();
            AddScore(baseCorrectlyCookedFood);
        }

        public void AddIncorrectlyCookedFoodScore()
        {
            happinessManager.Decrease();
            AddScore(baseIncorrectlyCookedFood);
        }

        public void AddTrashScore()
        {
            happinessManager.Increase();
            AddScore(baseTrash);
        }

    }

}

