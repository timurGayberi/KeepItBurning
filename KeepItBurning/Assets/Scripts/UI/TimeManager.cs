using TMPro;
using UnityEngine;

namespace Score
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] public float timer = 0f;
        [SerializeField] public float IncreaseTime = 30f;
        [SerializeField] public float TimeMultiplier = 1f;

        [SerializeField] public TextMeshProUGUI timeText;

        private int _lastSecond = -1;

        public void Timer()
        {
            timer += Time.deltaTime;

            // OPTIMIZATION: Only update text when value changes to avoid GC
            int currentSecond = Mathf.FloorToInt(timer);
            if (currentSecond != _lastSecond)
            {
                _lastSecond = currentSecond;
                timeText.text = GetFormatedTime();
            }

            if (timer > IncreaseTime)
            {
                TimeMultiplier *= 1.1f;
                IncreaseTime += 30f;
            }
        }

        public string GetFormatedTime()
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}
