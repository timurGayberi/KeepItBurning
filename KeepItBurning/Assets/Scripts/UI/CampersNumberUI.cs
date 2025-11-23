using UnityEngine;
using TMPro;

namespace UI
{
    public class CampersNumberUI : MonoBehaviour
    {
        [SerializeField] private VisitorsManager visitorsManager;
        [SerializeField] private TextMeshProUGUI campersText;

        private int _lastVisitorCount = -1;

        private void Update()
        {
            if (visitorsManager == null || campersText == null) return;

            // OPTIMIZATION: Only update text when value changes to avoid GC
            if (_lastVisitorCount != visitorsManager.currentVisitors)
            {
                _lastVisitorCount = visitorsManager.currentVisitors;
                campersText.text = $"x{_lastVisitorCount}";
            }
        }
    }
}
