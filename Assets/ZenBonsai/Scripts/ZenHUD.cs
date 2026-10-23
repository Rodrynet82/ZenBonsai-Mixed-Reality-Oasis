using UnityEngine;
using UnityEngine.UI;

namespace ZenBonsai
{
    /// <summary>
    /// HUD Zen minimalista en el espacio 3D (World Space) para mostrar
    /// la Armonía y la Hidratación del bonsái de forma limpia y estética.
    /// </summary>
    public class ZenHUD : MonoBehaviour
    {
        [Header("Referencias Visuales")]
        public Image harmonyFill;
        public Image hydrationFill;
        public Text statusText;

        private void Awake()
        {
            // El estado del bonsái ahora se muestra integrado en el panel Jardín Zen.
            // Destruir cualquier canvas flotante huérfano para eliminar al 100% la visión doble.
            Destroy(gameObject);
        }

        private void Update()
        {
            if (ZenBonsaiManager.Instance == null) return;

            float harmony = ZenBonsaiManager.Instance.harmony;
            float hydration = ZenBonsaiManager.Instance.hydration;

            if (harmonyFill != null)
            {
                harmonyFill.fillAmount = harmony / 100f;
            }

            if (hydrationFill != null)
            {
                hydrationFill.fillAmount = hydration / 100f;
            }

            if (statusText != null)
            {
                if (ZenBonsaiManager.Instance.currentState == BonsaiState.FullBloom)
                {
                    statusText.text = "🌸 FLORACIÓN PLENA 🌸\nMente en Paz";
                    statusText.color = new Color(1f, 0.6f, 0.8f);
                }
                else if (hydration < 25f)
                {
                    statusText.text = "💧 El Bonsái necesita agua";
                    statusText.color = new Color(0.4f, 0.7f, 1f);
                }
                else
                {
                    statusText.text = "🌿 Meditación Zen";
                    statusText.color = new Color(0.8f, 0.95f, 0.8f);
                }
            }
        }
    }
}
