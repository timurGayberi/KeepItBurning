using UnityEngine;
using TMPro;

namespace UI
{
    public class CampersNumberUI : MonoBehaviour
    {
        [SerializeField] private VisitorsManager visitorsManager;
        [SerializeField] private TextMeshProUGUI campersText;

        private void Update()
        {
            if (visitorsManager == null || campersText == null) return;

            campersText.text = $"x{visitorsManager.currentVisitors}";
        }
    }
}
