using System.Collections.Generic;
using UnityEngine;

namespace ZenBonsai
{
    public enum BonsaiState
    {
        Meditation,     // Cuidando el árbol
        FullBloom,      // Floración plena (Victoria / Éxtasis zen)
        NeedsCare       // Muy marchito, requiere atención urgente
    }

    public enum GameMode
    {
        FreeZen,            // Meditación libre sin tiempo
        MasterChallenge     // Desafío Maestro Bonsái (3 minutos)
    }

    [System.Serializable]
    public class SpeciesCareData
    {
        public TreeType species;
        [Range(0f, 100f)] public float harmony = 60f;
        [Range(0f, 100f)] public float hydration = 86f;
        public float thirstMultiplier = 0.85f;
        public bool hasBloomed = false;
        public BonsaiState state = BonsaiState.Meditation;
        public bool notifiedNeedsWater = false;
    }

    /// <summary>
    /// Controlador principal del bucle de juego de Zen Bonsai.
    /// Gestiona de forma independiente la armonía, hidratación, poda, plagas y floración
    /// de las 3 especies (Sakura, Matsu y Momiji) a un ritmo lento y meditativo.
    /// </summary>
    public class ZenBonsaiManager : MonoBehaviour
    {
        public static ZenBonsaiManager Instance { get; private set; }

        [Header("Modo de Juego")]
        public GameMode currentGameMode = GameMode.FreeZen;
        public float challengeTimeRemaining = 180f; // 3 minutos
        public bool isChallengeCompleted = false;

        [Header("Estadísticas del Bonsái Activo")]
        [Range(0f, 100f)] public float harmony = 60f;
        [Range(0f, 100f)] public float hydration = 86f;
        public BonsaiState currentState = BonsaiState.Meditation;

        [Header("Cuidado Independiente por Especie")]
        public SpeciesCareData sakuraCare = new SpeciesCareData { species = TreeType.Sakura, harmony = 62f, hydration = 86f, thirstMultiplier = 0.88f };
        public SpeciesCareData matsuCare = new SpeciesCareData { species = TreeType.Matsu, harmony = 65f, hydration = 94f, thirstMultiplier = 0.68f };
        public SpeciesCareData momijiCare = new SpeciesCareData { species = TreeType.Momiji, harmony = 58f, hydration = 78f, thirstMultiplier = 1.00f };

        [Header("Tasas de cambio (Ritmo Zen Relajado)")]
        public float hydrationLossRate = 0.28f;         // Consumo de agua lento y pausado (~6 veces más relajado)
        public float backgroundHydrationFactor = 0.62f; // Consumo aún más suave para los árboles en reposo
        public float harmonyPassiveGainRate = 0.75f;    // Gana armonía pasiva si está sano e hidratado
        public float harmonyPenaltyPerWithered = 0.45f; // Penalización suave por rama seca

        [Header("Modo de Enfoque en Árboles (Opciones)")]
        [Tooltip("1 = Solo el árbol activo evoluciona; 2 = Sakura y Matsu; 3 = Tríada completa")]
        public int treeFocusMode = 3;

        public void CycleTreeFocusMode()
        {
            treeFocusMode++;
            if (treeFocusMode > 3) treeFocusMode = 1;
        }

        [Header("Referencias de Spawn")]
        public Transform trunkBase;
        public Transform[] leafNodes;
        public GameObject pestPrefab;

        [Header("Árbol Orgánico")]
        public OrganicBonsaiTree organicTree;

        [Header("Floración / Recompensas")]
        public GameObject sakuraBlossomsContainer; // Flores que brotan al llegar a 100%
        public float pestSpawnInterval = 45f;      // Aparición muy espaciada para mantener la calma

        private float pestSpawnTimer;
        private ZenPest currentActivePest;
        private List<PrunableBranch> branches = new List<PrunableBranch>();

        public SpeciesCareData GetCareData(TreeType type)
        {
            switch (type)
            {
                case TreeType.Sakura: return sakuraCare;
                case TreeType.Matsu: return matsuCare;
                case TreeType.Momiji: return momijiCare;
                default: return sakuraCare;
            }
        }

        public TreeType ActiveSpecies => organicTree != null ? organicTree.currentSpecies : TreeType.Sakura;

        public void ChangeSpecies(TreeType type)
        {
            // Si había una oruga caminando por el árbol anterior, limpiarla visualmente al cambiar de maceta
            if (currentActivePest != null)
            {
                Destroy(currentActivePest.gameObject);
                currentActivePest = null;
            }

            if (organicTree != null)
            {
                organicTree.ApplySpecies(type);
            }

            SyncActiveStatsFromSpecies(type);
        }

        private void SyncActiveStatsFromSpecies(TreeType type)
        {
            SpeciesCareData data = GetCareData(type);
            harmony = data.harmony;
            hydration = data.hydration;
            currentState = data.state;

            if (sakuraBlossomsContainer != null)
            {
                sakuraBlossomsContainer.SetActive(data.hasBloomed);
            }
        }

        /// <summary>
        /// Actualiza dinámicamente las ramas podables, puntos de follaje y tronco al cambiar entre árboles reales.
        /// </summary>
        public void RegisterTreeData(List<PrunableBranch> newBranches, Transform newTrunkBase, Transform[] newLeafNodes, GameObject newBlossomContainer)
        {
            branches.Clear();
            if (newBranches != null) branches.AddRange(newBranches);
            trunkBase = newTrunkBase;
            leafNodes = newLeafNodes;
            sakuraBlossomsContainer = newBlossomContainer;

            SpeciesCareData data = GetCareData(ActiveSpecies);
            if (sakuraBlossomsContainer != null)
            {
                sakuraBlossomsContainer.SetActive(data.hasBloomed);
            }
        }

        /// <summary>
        /// Consulta si un árbol concreto (Sakura, Matsu o Momiji) necesita algún cuidado ahora mismo
        /// para encender su neón indicador alrededor del nombre en el cartel Jardín Zen.
        /// </summary>
        public bool DoesTreeNeedCare(TreeType type, out bool needsWater, out bool needsPrune, out bool hasPest, out float treeHydration, out float treeHarmony)
        {
            SpeciesCareData data = GetCareData(type);
            treeHydration = data.hydration;
            treeHarmony = data.harmony;

            needsWater = data.hydration < 36f;
            needsPrune = GetWitheredCountForSpecies(type) > 0;
            hasPest = (type == ActiveSpecies && currentActivePest != null);

            return needsWater || needsPrune || hasPest;
        }

        public int GetWitheredCountForSpecies(TreeType type)
        {
            if (organicTree == null) return 0;
            SpeciesTreeModel model = organicTree.GetModel(type);
            if (model == null || model.prunableBranches == null) return 0;

            int count = 0;
            for (int i = 0; i < model.prunableBranches.Count; i++)
            {
                PrunableBranch b = model.prunableBranches[i];
                if (b != null && b.isWithered) count++;
            }
            return count;
        }

        public void SetGameMode(GameMode mode)
        {
            currentGameMode = mode;
            if (mode == GameMode.MasterChallenge)
            {
                challengeTimeRemaining = 180f;
                isChallengeCompleted = false;
                sakuraCare.harmony = 55f; sakuraCare.hydration = 78f;
                matsuCare.harmony = 58f;  matsuCare.hydration = 82f;
                momijiCare.harmony = 52f; momijiCare.hydration = 72f;
                SyncActiveStatsFromSpecies(ActiveSpecies);
                if (ZenSoundSynthesizer.Instance != null)
                {
                    ZenSoundSynthesizer.Instance.PlayTibetanBell(528f, 2.5f, 0.8f);
                }
            }
            else
            {
                isChallengeCompleted = false;
                if (ZenSoundSynthesizer.Instance != null)
                {
                    ZenSoundSynthesizer.Instance.PlayTibetanBell(432f, 2.0f, 0.6f);
                }
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Destruir cualquier canvas flotante ZenHUD o bloque antiguo que pudiera quedar en la escena
            Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);
            foreach (var c in allCanvases)
            {
                if (c != null && (c.name.Contains("ZenHUD") || (c.name.Contains("StatusText") && c.transform.parent == transform)))
                {
                    Destroy(c.gameObject);
                }
            }
            var huds = FindObjectsOfType<ZenHUD>(true);
            foreach (var h in huds) if (h != null) Destroy(h.gameObject);

            // Limpiar cualquier objeto primitivo heredado de versiones anteriores (bloques grises ZenWaterBowl, BonsaiTrunk, BonsaiPot)
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child == null) continue;
                string cn = child.name;
                if (cn == "ZenWaterBowl" || cn == "BonsaiTrunk" || cn == "BonsaiPot" ||
                    cn.StartsWith("Branch_Foliage_") || cn == "SakuraBlossoms")
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private Transform centerEyeTransform;
        private Vector3 lastRecenterHeadPos = Vector3.zero;
        private bool hasInitialTrackingLocked = false;

        private void Start()
        {
            // Asegurar tamaño original 1:1 del bonsái
            transform.localScale = Vector3.one;

            pestSpawnTimer = Random.Range(12f, 18f);

            if (sakuraBlossomsContainer != null)
            {
                sakuraBlossomsContainer.SetActive(false);
            }

            // Suscribirse al botón de recentrado nativo de Meta Quest
            if (OVRManager.display != null)
            {
                OVRManager.display.RecenteredPose += RecenterInFrontOfUser;
            }

            // Posicionar inmediatamente y mantener seguimiento durante el arranque del visor Quest
            RecenterInFrontOfUser();
            StartCoroutine(DelayedRecenter());

            // Campana inicial relajante
            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlayTibetanBell(432f, 3.5f, 0.7f);
            }
        }

        private void OnDestroy()
        {
            if (OVRManager.display != null)
            {
                OVRManager.display.RecenteredPose -= RecenterInFrontOfUser;
            }
        }

        private Transform GetHeadTransform()
        {
            if (centerEyeTransform != null) return centerEyeTransform;

            GameObject eyeObj = GameObject.Find("CenterEyeAnchor");
            if (eyeObj != null)
            {
                centerEyeTransform = eyeObj.transform;
                return centerEyeTransform;
            }

            Camera cam = Camera.main;
            if (cam == null) cam = FindObjectOfType<Camera>();
            if (cam != null)
            {
                centerEyeTransform = cam.transform;
            }
            return centerEyeTransform;
        }

        private System.Collections.IEnumerator DelayedRecenter()
        {
            float elapsed = 0f;
            while (elapsed < 3.5f)
            {
                elapsed += Time.deltaTime;
                ZenBonsaiManipulator manip = GetComponent<ZenBonsaiManipulator>();
                if (manip != null && (manip.IsManipulating || manip.HasUserMovedTable))
                {
                    yield break;
                }

                Transform head = GetHeadTransform();
                if (head != null)
                {
                    Vector3 hp = head.position;
                    if (hp.y > 0.35f)
                    {
                        if (!hasInitialTrackingLocked || Vector3.Distance(hp, lastRecenterHeadPos) > 0.12f)
                        {
                            hasInitialTrackingLocked = true;
                            RecenterInFrontOfUser();
                        }
                    }
                }
                yield return null;
            }
        }

        [Header("Posicionamiento respecto al usuario")]
        public float forwardDistance = 0.50f; // 50 cm justo delante del jugador
        public float downwardOffset = 0.24f;  // 24 cm bajo el nivel de los ojos

        [ContextMenu("Recenter In Front Of User")]
        public void RecenterInFrontOfUser()
        {
            Transform head = GetHeadTransform();

            bool validHead = (head != null && head.position.y > 0.35f);
            Vector3 headPos = validHead ? head.position : new Vector3(0f, 1.20f, 0f);
            lastRecenterHeadPos = headPos;

            Vector3 forward = head != null ? head.forward : Vector3.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.01f) forward = Vector3.forward;
            forward.Normalize();

            float dist = Mathf.Clamp(forwardDistance, 0.44f, 0.60f);
            float drop = Mathf.Clamp(downwardOffset, 0.20f, 0.28f);
            float tableY = Mathf.Max(0.55f, headPos.y - drop);

            Vector3 newPos = new Vector3(headPos.x + forward.x * dist, tableY, headPos.z + forward.z * dist);
            Quaternion newRot = Quaternion.LookRotation(forward);

            transform.position = newPos;
            transform.rotation = newRot;
            transform.localScale = Vector3.one;

            ZenBonsaiManipulator manip = GetComponent<ZenBonsaiManipulator>();
            if (manip != null)
            {
                manip.SetTargetPose(newPos, newRot, Vector3.one);
            }
        }

        private void Update()
        {
            ZenBonsaiManipulator manip = GetComponent<ZenBonsaiManipulator>();
            if (manip == null || (!manip.IsManipulating && !manip.HasUserMovedTable))
            {
                Transform head = GetHeadTransform();
                if (head != null && head.position.y > 0.50f)
                {
                    float verticalGap = head.position.y - transform.position.y;
                    Vector2 horizHead = new Vector2(head.position.x, head.position.z);
                    Vector2 horizTable = new Vector2(transform.position.x, transform.position.z);
                    float horizDist = Vector2.Distance(horizHead, horizTable);

                    if (verticalGap > 0.45f || verticalGap < 0.05f || horizDist < 0.22f || horizDist > 0.85f)
                    {
                        RecenterInFrontOfUser();
                    }
                }
            }

            if (currentGameMode == GameMode.MasterChallenge && !isChallengeCompleted)
            {
                challengeTimeRemaining -= Time.deltaTime;
                if (challengeTimeRemaining <= 0f)
                {
                    challengeTimeRemaining = 0f;
                    isChallengeCompleted = true;
                    float avgHarmony = (sakuraCare.harmony + matsuCare.harmony + momijiCare.harmony) / 3f;
                    if (ZenSoundSynthesizer.Instance != null)
                    {
                        if (avgHarmony >= 75f)
                        {
                            ZenSoundSynthesizer.Instance.PlayWindChime(0.8f);
                        }
                        else
                        {
                            ZenSoundSynthesizer.Instance.PlayTibetanBell(380f, 2.5f, 0.6f);
                        }
                    }
                }
            }

            // Actualizar de forma independiente según el modo de enfoque seleccionado en Opciones
            if (treeFocusMode == 1)
            {
                // Solo evoluciona el árbol activo; los otros dos quedan en reposo perfecto sin secarse
                UpdateSingleSpeciesCare(ActiveSpecies, GetCareData(ActiveSpecies));
            }
            else if (treeFocusMode == 2)
            {
                // Evolucionan Sakura y Matsu; Momiji queda en reposo
                UpdateSingleSpeciesCare(TreeType.Sakura, sakuraCare);
                UpdateSingleSpeciesCare(TreeType.Matsu, matsuCare);
            }
            else
            {
                // Tríada zen completa: los 3 árboles evolucionan a su ritmo pausado
                UpdateSingleSpeciesCare(TreeType.Sakura, sakuraCare);
                UpdateSingleSpeciesCare(TreeType.Matsu, matsuCare);
                UpdateSingleSpeciesCare(TreeType.Momiji, momijiCare);
            }

            // Sincronizar las variables públicas con el árbol que el jugador está contemplando ahora mismo
            SyncActiveStatsFromSpecies(ActiveSpecies);

            UpdatePests();
        }

        private void UpdateSingleSpeciesCare(TreeType species, SpeciesCareData data)
        {
            bool isActiveTree = (species == ActiveSpecies);
            float dt = Time.deltaTime;

            // 1. Consumo lento y meditativo de hidratación (cada árbol a su ritmo natural)
            float bgFactor = isActiveTree ? 1.0f : backgroundHydrationFactor;
            if (data.hydration > 0f)
            {
                data.hydration = Mathf.Max(0f, data.hydration - hydrationLossRate * data.thirstMultiplier * bgFactor * dt);
            }

            if (data.hydration < 35f && !data.notifiedNeedsWater)
            {
                data.notifiedNeedsWater = true;
                if (ZenSoundSynthesizer.Instance != null)
                {
                    ZenSoundSynthesizer.Instance.PlayWindChime(0.35f);
                }
            }
            else if (data.hydration >= 42f)
            {
                data.notifiedNeedsWater = false;
            }

            // 2. Evolución pausada de las ramas de este árbol (máximo 1 rama marchita en reposo para no agobiar)
            int witheredCount = 0;
            if (organicTree != null)
            {
                SpeciesTreeModel model = organicTree.GetModel(species);
                if (model != null && model.prunableBranches != null)
                {
                    for (int i = 0; i < model.prunableBranches.Count; i++)
                    {
                        if (model.prunableBranches[i] != null && model.prunableBranches[i].isWithered)
                        {
                            witheredCount++;
                        }
                    }

                    int maxAllowedWithered = isActiveTree ? 2 : 1;
                    float branchTimeScale = isActiveTree ? 1.0f : 0.55f;
                    for (int i = 0; i < model.prunableBranches.Count; i++)
                    {
                        PrunableBranch b = model.prunableBranches[i];
                        if (b != null)
                        {
                            b.TickBranch(dt * branchTimeScale, witheredCount < maxAllowedWithered);
                            if (b.isWithered) witheredCount = Mathf.Max(witheredCount, 1);
                        }
                    }
                }
            }

            // 3. Armonía individual de este árbol
            bool hasPestOnThisTree = isActiveTree && (currentActivePest != null);
            if (data.hydration >= 32f && witheredCount == 0 && !hasPestOnThisTree)
            {
                data.harmony = Mathf.Min(100f, data.harmony + harmonyPassiveGainRate * bgFactor * dt);
            }
            else
            {
                float penalty = witheredCount * harmonyPenaltyPerWithered;
                if (data.hydration < 22f) penalty += 0.55f;
                data.harmony = Mathf.Max(0f, data.harmony - penalty * bgFactor * dt);
            }

            // 4. Comprobar estado y floración individual
            if (data.harmony >= 95f && !data.hasBloomed)
            {
                data.hasBloomed = true;
                data.state = BonsaiState.FullBloom;
                if (isActiveTree && sakuraBlossomsContainer != null)
                {
                    sakuraBlossomsContainer.SetActive(true);
                }
                if (ZenSoundSynthesizer.Instance != null)
                {
                    ZenSoundSynthesizer.Instance.PlayTibetanBell(528f, 4.0f, 0.75f);
                    ZenSoundSynthesizer.Instance.PlayWindChime(0.65f);
                }
            }
            else if (data.harmony < 80f && data.hasBloomed)
            {
                data.hasBloomed = false;
                data.state = (data.hydration < 35f || witheredCount > 0) ? BonsaiState.NeedsCare : BonsaiState.Meditation;
                if (isActiveTree && sakuraBlossomsContainer != null)
                {
                    sakuraBlossomsContainer.SetActive(false);
                }
            }
            else if (!data.hasBloomed)
            {
                data.state = (data.hydration < 35f || witheredCount > 0 || hasPestOnThisTree) ? BonsaiState.NeedsCare : BonsaiState.Meditation;
            }
        }

        private void UpdatePests()
        {
            // Solo como máximo 1 oruga activa a la vez
            if (currentActivePest != null) return;

            pestSpawnTimer -= Time.deltaTime;
            if (pestSpawnTimer <= 0f)
            {
                pestSpawnTimer = Random.Range(24f, 36f);
                SpawnPest();
            }
        }

        private void SpawnPest()
        {
            if (leafNodes == null || leafNodes.Length == 0 || trunkBase == null) return;

            Transform targetLeaf = leafNodes[Random.Range(0, leafNodes.Length)];
            if (targetLeaf == null) return;

            GameObject bugObj = new GameObject("ZenCaterpillar");
            bugObj.transform.SetParent(trunkBase, false);

            SphereCollider col = bugObj.AddComponent<SphereCollider>();
            col.radius = 0.036f;
            col.isTrigger = true;

            Rigidbody bugRb = bugObj.AddComponent<Rigidbody>();
            bugRb.isKinematic = true;
            bugRb.useGravity = false;

            ZenPest pest = bugObj.AddComponent<ZenPest>();
            pest.crawlSpeed = 0.036f; // Avance pausado (~28 segundos subiendo pegada a la corteza del tronco y rama)
            currentActivePest = pest;

            pest.InitializeOnTrunk(ActiveSpecies, targetLeaf.localPosition);

            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlayWindChime(0.32f);
            }
        }

        public void WaterTree(float amount)
        {
            SpeciesCareData data = GetCareData(ActiveSpecies);
            data.hydration = Mathf.Clamp(data.hydration + amount, 0f, 100f);
            data.harmony = Mathf.Clamp(data.harmony + amount * 0.12f, 0f, 100f);
            SyncActiveStatsFromSpecies(ActiveSpecies);
        }

        public void OnBranchPruned(TreeType species = TreeType.Sakura)
        {
            SpeciesCareData data = GetCareData(species);
            data.harmony = Mathf.Clamp(data.harmony + 10f, 0f, 100f);
            SyncActiveStatsFromSpecies(ActiveSpecies);
        }

        public void OnBranchWithered(TreeType species = TreeType.Sakura)
        {
            // Suave campanilla zen cuando algún árbol (incluso en segundo plano) necesita poda
            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlayWindChime(0.35f);
            }
        }

        public void OnPestDefeated()
        {
            currentActivePest = null;
            SpeciesCareData data = GetCareData(ActiveSpecies);
            data.harmony = Mathf.Clamp(data.harmony + 10f, 0f, 100f);
            SyncActiveStatsFromSpecies(ActiveSpecies);
        }

        public void OnPestReachedGoal()
        {
            currentActivePest = null;
            SpeciesCareData data = GetCareData(ActiveSpecies);
            data.harmony = Mathf.Clamp(data.harmony - 5f, 0f, 100f);
            SyncActiveStatsFromSpecies(ActiveSpecies);
        }
    }
}
