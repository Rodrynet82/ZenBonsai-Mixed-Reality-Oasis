using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Botón zen interactivo en la mesa.
    /// Al tocarlo con el dedo, re-centra suavemente la mesa frente al pecho del jugador.
    /// </summary>
    public class ZenRecenterButton : MonoBehaviour
    {
        private Vector3 initialScale;
        private float lastPressTime = -1f;

        private void Start()
        {
            initialScale = transform.localScale;
        }

        public void Press()
        {
            if (Time.time - lastPressTime < 1.0f) return;
            lastPressTime = Time.time;

            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.RecenterInFrontOfUser();
            }

            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlayTibetanBell(528f, 1.5f, 0.4f);
            }

            StartCoroutine(ButtonFeedback());
        }

        private System.Collections.IEnumerator ButtonFeedback()
        {
            float elapsed = 0f;
            float duration = 0.2f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float scale = 1f - 0.25f * Mathf.Sin((elapsed / duration) * Mathf.PI);
                transform.localScale = initialScale * scale;
                yield return null;
            }
            transform.localScale = initialScale;
        }

        private void OnTriggerEnter(Collider other)
        {
            Press();
        }
    }
}
