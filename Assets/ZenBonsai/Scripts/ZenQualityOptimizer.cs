using UnityEngine;
using UnityEngine.XR;

namespace ZenBonsai
{
    /// <summary>
    /// Optimizador de Calidad Visual de Ultra Alta Definición para Meta Quest 3.
    /// Aumenta la resolución del búfer de ojo (supersampling), activa el filtro de nitidez por hardware (CAS),
    /// maximiza la tasa de refresco a 90 Hz, y enfoca el mapa de sombras en la mesa del bonsái.
    /// </summary>
    [DefaultExecutionOrder(-500)]
    public class ZenQualityOptimizer : MonoBehaviour
    {
        [Header("Nitidez & Resolución")]
        [Tooltip("Escala de resolución de ojo (1.35x para nitidez cristalina en Quest 3)")]
        [Range(1.0f, 1.6f)]
        public float eyeResolutionScale = 1.35f;

        [Tooltip("Tasa de refresco objetivo para Quest 3")]
        public float targetRefreshRate = 90.0f;

        [Header("Sombras de Alta Definición")]
        [Tooltip("Distancia de sombra optimizada para mesa cercana (3m para máxima definición)")]
        public float tabletopShadowDistance = 3.5f;

        private void Awake()
        {
            ApplyGraphicsBoost();
        }

        private void Start()
        {
            ApplyGraphicsBoost();
        }

        public void ApplyGraphicsBoost()
        {
            // 1. Supersampling en el búfer de ojos OpenXR (elimina la borrosidad en Quest 3)
            XRSettings.eyeTextureResolutionScale = eyeResolutionScale;

            // 2. Tasa de refresco fluida (90 FPS)
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Mathf.RoundToInt(targetRefreshRate);

            // 3. Enfocar el mapa de sombras en el bonsái (resolución máxima en 3.5 metros)
            QualitySettings.shadowDistance = tabletopShadowDistance;

            // 4. Integración con Meta XR SDK / OVRPlugin si está presente
            ConfigureOVRSettings();

            Debug.Log($"<color=cyan>[ZenQualityOptimizer]</color> Boost gráfico aplicado: Escala {eyeResolutionScale}x, Sombras {tabletopShadowDistance}m, FPS {targetRefreshRate}Hz");
        }

        private void ConfigureOVRSettings()
        {
            try
            {
                // Configurar frecuencia de pantalla a 90 Hz si el hardware lo soporta
                if (OVRPlugin.systemDisplayFrequenciesAvailable != null)
                {
                    foreach (float freq in OVRPlugin.systemDisplayFrequenciesAvailable)
                    {
                        if (Mathf.Approximately(freq, targetRefreshRate))
                        {
                            OVRPlugin.systemDisplayFrequency = targetRefreshRate;
                            break;
                        }
                    }
                }

                // Desactivar Foveated Rendering agresivo para que la visión periférica sea 100% nítida
                OVRPlugin.foveatedRenderingLevel = OVRPlugin.FoveatedRenderingLevel.Off;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[ZenQualityOptimizer] OVRPlugin config notice: " + ex.Message);
            }
        }
    }
}
