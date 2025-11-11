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

        public void Timer()
        {
            timer += Time.deltaTime;
            timeText.text = GetFormatedTime();

            if (timer > IncreaseTime)
            {
                TimeMultiplier *= 1.1f;
                IncreaseTime += 30f;
            }
        }

        public string GetFormatedTime()

        {

            int hours = Mathf.FloorToInt(timer / 3600),

                minutes = Mathf.FloorToInt((timer % 3600) / 60),

                seconds = Mathf.FloorToInt((timer % 3600) % 60);


            if (hours < 1)
            {
                return $"{minutes:00}:{seconds:00}";
            }
            else
                return $"{hours:00}:{minutes:00}:{seconds:00}";
        }
    }
}
