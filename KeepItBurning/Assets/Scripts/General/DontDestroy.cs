using UnityEngine;

/// <summary>
/// Simple script to make a GameObject persist across scene loads.
/// Attach this to any GameObject you want to keep alive between scenes.
/// </summary>
public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
