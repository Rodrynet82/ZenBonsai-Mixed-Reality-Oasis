using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Rama u hoja podable del bonsái.
    /// Cuando se marchita (se pone amarillenta), el jugador debe podarla
    /// con los dedos (contacto directo o pellizco).
    /// </summary>
    public class PrunableBranch : MonoBehaviour
    {
        [Header("Configuración")]
        public TreeType ownerSpecies = TreeType.Sakura;
        public bool isWithered = false;
        public float timeToWither = 75f; // Ritmo zen relajado
        
        [Header("Colores")]
        public Color healthyColor = new Color(0.2f, 0.7f, 0.3f);
        public Color witheredColor = new Color(0.75f, 0.55f, 0.15f);

        [Header("Visuales")]
        public Renderer branchRenderer;
        
        private Vector3 initialScale;
        private float witherTimer;
        private bool isPruning = false;
        private bool isInitialized = false;

        public void EnsureInitialized()
        {
            if (isInitialized) return;
            isInitialized = true;

            initialScale = transform.localScale;
            if (branchRenderer == null)
            {
                branchRenderer = GetComponentInChildren<Renderer>();
            }

            SetHealthy(true);
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void Start()
        {
            EnsureInitialized();
        }

        /// <summary>
        /// Llamado por ZenBonsaiManager tanto para el árbol activo como para los que están en segundo plano,
        /// a un ritmo pausado y relajante.
        /// </summary>
        public void TickBranch(float deltaTime, bool allowNewWither)
        {
            EnsureInitialized();
            if (isPruning) return;

            if (!isWithered)
            {
                witherTimer -= deltaTime;
                if (witherTimer <= 0f)
                {
                    if (allowNewWither)
                    {
                        Wither();
                    }
                    else
                    {
                        // Si este árbol ya tiene una rama marchita esperando poda, aplazar suavemente para no saturar al jugador
                        witherTimer = Random.Range(25f, 45f);
                    }
                }
            }
        }

        public void SetHealthy(bool initial = false)
        {
            isWithered = false;
            witherTimer = initial ? Random.Range(45f, 105f) : Random.Range(65f, 130f);
            if (Application.isPlaying && branchRenderer != null && branchRenderer.material != null)
            {
                branchRenderer.material.color = healthyColor;
                if (branchRenderer.material.HasProperty("_BaseColor"))
                    branchRenderer.material.SetColor("_BaseColor", healthyColor);
            }
        }

        public void Wither()
        {
            isWithered = true;
            if (Application.isPlaying && branchRenderer != null && branchRenderer.material != null)
            {
                branchRenderer.material.color = witheredColor;
                if (branchRenderer.material.HasProperty("_BaseColor"))
                    branchRenderer.material.SetColor("_BaseColor", witheredColor);
            }

            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.OnBranchWithered(ownerSpecies);
            }
        }

        /// <summary>
        /// Llamado cuando la mano del jugador o las tijeras cortan la rama marchita.
        /// </summary>
        public void Prune()
        {
            if (isPruning || !isWithered) return;
            isPruning = true;

            // Sonido de tijera de poda
            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlaySnip();
            }

            // Efecto visual de hojas que caen
            if (ZenVisualEffects.Instance != null)
            {
                ZenVisualEffects.Instance.SpawnPruneLeaves(transform.position);
            }

            // Notificar al manager para el árbol correspondiente
            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.OnBranchPruned(ownerSpecies);
            }

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(PruneAndRegrowRoutine());
            }
            else
            {
                SetHealthy(false);
                transform.localScale = initialScale;
                isPruning = false;
            }
        }

        private System.Collections.IEnumerator PruneAndRegrowRoutine()
        {
            // Encoger rama cortada
            float elapsed = 0f;
            float duration = 0.2f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsed / duration);
                yield return null;
            }
            transform.localScale = Vector3.zero;

            // Esperar antes de rebrotar
            yield return new WaitForSeconds(2.5f);

            // Rebrotar sana y verde
            SetHealthy(false);
            elapsed = 0f;
            duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, elapsed / duration);
                yield return null;
            }
            transform.localScale = initialScale;
            isPruning = false;
        }

        private void OnDisable()
        {
            // Si el jugador cambia de árbol en mitad de la animación de rebrote, dejar la rama lista y sana
            if (isPruning)
            {
                SetHealthy(false);
                transform.localScale = initialScale;
                isPruning = false;
            }
        }

        // Detección por colisión física (la mano o las tijeras entran en contacto con la rama)
        private void OnTriggerEnter(Collider other)
        {
            // Si el usuario acaba de tocar un botón del panel Jardín Zen, ignorar roces accidentales
            if (Time.time < ZenTouchButton.GlobalPressCooldownUntil) return;

            if (isWithered && !isPruning)
            {
                Prune();
            }
        }
    }
}
