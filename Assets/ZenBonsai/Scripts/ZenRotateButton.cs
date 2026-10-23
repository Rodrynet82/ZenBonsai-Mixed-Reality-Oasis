using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Botón táctil colocado en los bordes de la mesa.
    /// Al tocarlo con el dedo índice, rota el bonsái 45 grados para poder inspeccionarlo
    /// y cuidarlo desde cualquier ángulo ("darle la vuelta").
    /// </summary>
    public class ZenRotateButton : MonoBehaviour
    {
        public float rotateAngle = 45f;
        public ZenBonsaiManipulator manipulator;
        public DraggableZenTable tableTarget;

        private Vector3 initialScale;
        private float lastPressTime = -1f;

        private void Start()
        {
            initialScale = transform.localScale;
            if (manipulator == null)
            {
                manipulator = FindObjectOfType<ZenBonsaiManipulator>();
            }
        }

        public void Press()
        {
            if (Time.time - lastPressTime < 0.35f) return;
            lastPressTime = Time.time;

            if (manipulator != null)
            {
                manipulator.RotateDegrees(rotateAngle);
            }
            else if (tableTarget != null)
            {
                tableTarget.RotateTableDegrees(rotateAngle);
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
