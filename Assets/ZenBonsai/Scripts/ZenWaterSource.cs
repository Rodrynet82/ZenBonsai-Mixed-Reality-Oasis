using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Fuente de rocío / cuenco de agua interactivo.
    /// Al tocarlo con el dedo o palma, riega el bonsái con gotitas relajantes.
    /// </summary>
    public class ZenWaterSource : MonoBehaviour
    {
        [Header("Efectos")]
        public float waterAmount = 25f;
        public float cooldown = 0.5f;

        private float lastWaterTime = -1f;
        private Vector3 initialScale;

        private void Start()
        {
            initialScale = transform.localScale;
        }

        public void Water()
        {
            if (Time.time - lastWaterTime < cooldown) return;
            lastWaterTime = Time.time;

            // Suave sonido de gota al rozar el agua de la fuente Tsukubai (el riego real lo hace el cazo Hishaku por su cuenco)
            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlayWaterDrop();
            }

            // Efecto elástico de ondulación en la superficie del agua
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(PulseRoutine());
            }
        }

        private System.Collections.IEnumerator PulseRoutine()
        {
            float elapsed = 0f;
            float duration = 0.25f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float scale = 1f + 0.12f * Mathf.Sin((elapsed / duration) * Mathf.PI);
                transform.localScale = initialScale * scale;
                yield return null;
            }
            transform.localScale = initialScale;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<ZenBonsaiTool>() != null) return;
            Water();
        }
    }
}
