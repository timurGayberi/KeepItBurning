using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(ParticleSystem))]
public class StarsManager : MonoBehaviour
{

    private Camera m_mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_mainCamera = Camera.main;
        transform.position = m_mainCamera.transform.position;
        transform.parent = m_mainCamera.transform;

        var starsRenderer = GetComponent<ParticleSystemRenderer>();
        starsRenderer.material.renderQueue = (int)RenderQueue.Background;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
