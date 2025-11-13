using UnityEngine;

public class FartTimer : MonoBehaviour
{
    [Header("particle")]
    [SerializeField] private ParticleSystem fartParticle;

    [Header("Time")]
    [SerializeField] private float minDelay = 60f;

    [SerializeField] private float maxDelay = 120f;

    [SerializeField] private float activeDuration = 15f;

    private void Awake()
    {
        if (fartParticle == null)
        {
            fartParticle = GetComponent<ParticleSystem>();
        }

        if (fartParticle != null)
        {
            fartParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        else
        {
            Debug.LogWarning("[FartTimer]no particle.");
        }
    }

    private void OnEnable()
    {
        if (fartParticle == null)
        {
            Debug.LogWarning("[FartTimer] no particle.");
            return;
        }
        StartCoroutine(FartRoutine());
    }

    private System.Collections.IEnumerator FartRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            fartParticle.Play();
            yield return new WaitForSeconds(activeDuration);

            fartParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
