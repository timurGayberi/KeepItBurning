using UnityEngine;

namespace UI
{
    /// <summary>
    /// Makes the cooking canvas always face the camera (billboard effect)
    /// </summary>
    public class CookingCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        private void Update()
        {
            if (Camera.main != null && canvas != null && canvas.enabled)
            {
                transform.LookAt(Camera.main.transform);
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            }
        }
    }
}
