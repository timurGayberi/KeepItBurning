using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Score
{
    public class HappinessUI : MonoBehaviour
    {
        [SerializeField] private HappinessManager happinessManager;
        [SerializeField] private GameObject happyMeter;
        [SerializeField] private GameObject neutralMeter;
        [SerializeField] private GameObject angryMeter;

        private void Update()
        {
            if (happinessManager == null) return;

            float happinessMultiplier = happinessManager.happiness / 50f;

            if (happyMeter != null) happyMeter.SetActive(false);
            if (neutralMeter != null) neutralMeter.SetActive(false);
            if (angryMeter != null) angryMeter.SetActive(false);

            if (happinessMultiplier > 1.5f)
            {
                if (happyMeter != null) happyMeter.SetActive(true);
            }
            else if (happinessMultiplier >= 0.5f && happinessMultiplier <= 1.5f)
            {
                if (neutralMeter != null) neutralMeter.SetActive(true);
            }
            else
            {
                if (angryMeter != null) angryMeter.SetActive(true);
            }
        }
    }
}
