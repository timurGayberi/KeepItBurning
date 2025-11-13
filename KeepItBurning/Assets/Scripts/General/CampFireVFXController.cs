using UnityEngine;
using UnityEngine.VFX;
using GamePlay.Interactables;

namespace General
{
    public class CampfireVFXController : MonoBehaviour
    {
        [Header("References")]
        public VisualEffect fireVFX;
        public Light campfireLight;

        [Header("Main Control")]
        [Range(0f, 1f)] public float MasterControl = 1.0f;

        [Header("Max Values")]
        [Range(0f, 1.15f)] public float maxFireSize = 1.15f;
        [Range(0f, 2f)] public float maxFireSpeed = 2.0f;
        [Range(0f, 55f)] public float maxLightIntensity = 55f;

        [Header("Flicker Settings")]
        public float flickerSpeed = 3f;        // flicker hızı
        public float flickerStrength = 0.2f;   // flicker’ın ne kadar değişken olacağı (0.2 = %20 oynama)

        [Space]
        [SerializeField] private float currentFireSize;
        [SerializeField] private float currentFireSpeed;
        [SerializeField] private float currentLightIntensity;

        private float randomOffset; // her kamp ateşi için farklı flicker fazı
        
        
        private float _targetMasterControl = 1.0f;
        [SerializeField] private float controlLerpSpeed = 2f;

        private void Start()
        {
            // Her kamp ateşi farklı bir "titreşim fazı" ile başlasın
            randomOffset = Random.Range(0f, 100f);
            
        }
        
        public void SetFuelNormalized(float normalizedFuel)
        {
            // Set the target control value. This will be smoothed in Update.
            _targetMasterControl = Mathf.Clamp01(normalizedFuel);
            
            // Ensure the fire is active if fuel is > 0
            if (normalizedFuel > 0f)
            {
                gameObject.SetActive(true);
            }
        }
        
        public void ShutDownFire()
        {
            // Ensure MasterControl is set to zero instantly for a clean shutdown
            MasterControl = 0f;
            _targetMasterControl = 0f;
            
            // Apply zero values
            if (fireVFX != null)
            {
                fireVFX.SetFloat("Fire size", 0f);
                fireVFX.SetFloat("Fire speed", 0f);
                // Optionally stop the VFX particle system
                // fireVFX.Stop(); 
            }
            if (campfireLight != null)
                campfireLight.intensity = 0f;
            
            // Disable the entire GameObject if desired (depends on your hierarchy)
            // gameObject.SetActive(false); 
        }

        private void Update()
        {
            
            MasterControl = Mathf.Lerp(MasterControl, _targetMasterControl, Time.deltaTime * controlLerpSpeed);
            
            if (MasterControl <= 0.01f)
            {
                return;
            }
            
            // MasterControl oranında değerleri hesapla
            currentFireSize = maxFireSize * MasterControl;
            currentFireSpeed = maxFireSpeed * MasterControl;

            // 🔥 Flicker hesabı (Perlin Noise ile doğal titreşim)
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed + randomOffset, 0f);
            float flicker = Mathf.Lerp(1f - flickerStrength, 1f + flickerStrength, noise);

            // Flicker’ı MasterControl ile birleştir
            currentLightIntensity = maxLightIntensity * MasterControl * flicker;

            // VFX Graph parametreleri güncelle
            if (fireVFX != null)
            {
                fireVFX.SetFloat("Fire size", currentFireSize);
                fireVFX.SetFloat("Fire speed", currentFireSpeed);
            }

            // Işık yoğunluğunu güncelle
            if (campfireLight != null)
                campfireLight.intensity = currentLightIntensity;
        }
    }
}
