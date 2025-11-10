using UnityEngine;

namespace Test
{
    public class GasLampFlicker : MonoBehaviour
    {
        public Light lampLight;

        [Header("Flicker Settings")] public float minIntensity = 2f;
        public float maxIntensity = 4f;
        public float flickerSpeed = 3f; // ne kadar hızlı titreşecek

        private float randomOffset; // her lambanın kendi “ritim farkı” olacak

        void Start()
        {
            if (lampLight == null)
                lampLight = GetComponent<Light>();

            // Her lambanın başlangıç fazını farklı yapıyoruz
            randomOffset = Random.Range(0f, 100f);
        }

        void Update()
        {
            // Flicker’ı perlin noise ile yapıyoruz (daha doğal sonuç)
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + randomOffset, 0.0f);
            lampLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
    }

}