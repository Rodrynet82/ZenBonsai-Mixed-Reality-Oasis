using UnityEngine;
using UnityEngine.UI;

namespace ZenBonsai
{
    /// <summary>
    /// Pabellón espacial 3D (Tatefuda) para Meta Quest 3 con interacción táctil directa (Hands-First).
    /// Permite cambiar entre las 3 especies de bonsái con aviso neón independiente, abrir la ficha
    /// botánica de Información y Cuidados, y abrir el panel interactivo de Opciones del Santuario.
    /// </summary>
    public class ZenSpatialMenu : MonoBehaviour
    {
        public static ZenSpatialMenu Instance { get; private set; }

        [Header("Contenedor del Menú")]
        public GameObject menuPanel;

        [Header("Botones Táctiles Principales")]
        public ZenTouchButton btnSakura;
        public ZenTouchButton btnMatsu;
        public ZenTouchButton btnMomiji;
        public ZenTouchButton btnInfo;
        public ZenTouchButton btnOptions;

        // Campos heredados opcionales por compatibilidad
        [HideInInspector] public ZenTouchButton btnFreeZen;
        [HideInInspector] public ZenTouchButton btnChallenge;

        [Header("Indicadores de Texto")]
        public Text speciesInfoText;
        public Text modeInfoText;
        public Text treeStatusText;

        [Header("Ficha Botánica e Información (Info Panel)")]
        public GameObject infoPanelBox;
        public Text infoPanelTitle;
        public Text infoPanelBody;

        [Header("Panel de Opciones del Santuario (Options Panel)")]
        public GameObject optionsPanelBox;
        public Text optionsPanelTitle;
        public Text optionsPanelSubtitle;
        public ZenTouchButton btnOptLockTable;
        public ZenTouchButton btnOptRecenter;
        public ZenTouchButton btnOptLanguage;
        public ZenTouchButton btnOptCareSpeed;
        public ZenTouchButton btnOptLighting;
        public ZenTouchButton btnOptSound;
        public ZenTouchButton btnOptMusic;
        public ZenTouchButton btnOptEnvironment;
        public ZenTouchButton btnOptTreeFocus;
        public ZenTouchButton btnOptPetals;

        [Header("Entorno Virtual Zen (Opciones)")]
        public GameObject virtualSanctuaryBackdrop;
        public GameObject[] virtualEnvironmentBackdrops;
        public GameObject sakuraPetalsRain;
        private int environmentIndex = 0; // 0 = Passthrough MR, 1 = Jardín Monte Fuji, 2 = Pagoda y Lago Kioto, 3 = Templo Zen Bambú
        private bool isVirtualEnvironmentActive => environmentIndex > 0;
        private bool isPetalRainActive = true;

        private bool isInfoOpen = false;
        private bool isOptionsOpen = false;
        private int lightingPresetIndex = 0; // 0 = Día Zen, 1 = Atardecer Kyoto, 2 = Noche de Linternas
        private int soundVolumeIndex = 0;    // 0 = 100%, 1 = 40% Suave, 2 = Silencio
        private int languageIndex = 0;       // 0 = Español, 1 = English, 2 = 日本語 (Japanese)

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (btnSakura != null) btnSakura.onClick.AddListener(SelectSakura);
            if (btnMatsu != null) btnMatsu.onClick.AddListener(SelectMatsu);
            if (btnMomiji != null) btnMomiji.onClick.AddListener(SelectMomiji);
            if (btnInfo != null) btnInfo.onClick.AddListener(ToggleInfoPanel);
            if (btnOptions != null) btnOptions.onClick.AddListener(ToggleOptionsPanel);
            if (btnCloseInfo != null) btnCloseInfo.onClick.AddListener(CloseInfoPanel);
            if (btnCloseOptions != null) btnCloseOptions.onClick.AddListener(CloseOptionsPanel);

            if (btnOptLockTable != null) btnOptLockTable.onClick.AddListener(ToggleLockTable);
            if (btnOptRecenter != null) btnOptRecenter.onClick.AddListener(TriggerRecenterTable);
            if (btnOptLanguage != null) btnOptLanguage.onClick.AddListener(CycleLanguage);
            if (btnOptCareSpeed != null) btnOptCareSpeed.onClick.AddListener(CycleCareSpeed);
            if (btnOptLighting != null) btnOptLighting.onClick.AddListener(CycleLightingAmbiance);
            if (btnOptSound != null) btnOptSound.onClick.AddListener(CycleSoundVolume);
            if (btnOptMusic != null) btnOptMusic.onClick.AddListener(CycleMusicTrack);
            if (btnOptEnvironment != null) btnOptEnvironment.onClick.AddListener(ToggleEnvironment);
            if (btnOptTreeFocus != null) btnOptTreeFocus.onClick.AddListener(CycleTreeCareFocus);
            if (btnOptPetals != null) btnOptPetals.onClick.AddListener(TogglePetalRain);

            if (infoPanelBox != null) infoPanelBox.SetActive(isInfoOpen);
            if (optionsPanelBox != null) optionsPanelBox.SetActive(isOptionsOpen);
            ApplyEnvironmentVisibility();
            if (sakuraPetalsRain != null) sakuraPetalsRain.SetActive(isPetalRainActive);

            UpdateUI();
        }

        private void Update()
        {
            if (ZenBonsaiManager.Instance == null) return;

            // 1. Actualizar el neón alrededor del medallón de cada árbol (nombre en una sola línea nítida)
            UpdateTreeButtonNeon(btnSakura, TreeType.Sakura, "SAKURA");
            UpdateTreeButtonNeon(btnMatsu, TreeType.Matsu, "MATSU");
            UpdateTreeButtonNeon(btnMomiji, TreeType.Momiji, "MOMIJI");

            // 2. Animar caída suave y balanceo orgánico de los pétalos de Sakura alrededor de la mesa
            AnimateSakuraPetals();

            // 3. Actualizar estado del bonsái activo en el pie del panel según el idioma elegido
            if (treeStatusText != null)
            {
                int harm = Mathf.RoundToInt(ZenBonsaiManager.Instance.harmony);
                int hydr = Mathf.RoundToInt(ZenBonsaiManager.Instance.hydration);
                TreeType activeSp = ZenBonsaiManager.Instance.ActiveSpecies;

                ZenBonsaiManager.Instance.DoesTreeNeedCare(activeSp, out bool needsWater, out bool needsPrune, out bool hasPest, out _, out _);

                if (ZenBonsaiManager.Instance.currentState == BonsaiState.FullBloom)
                {
                    treeStatusText.text = languageIndex == 0
                        ? $"🌸 ¡{activeSp} en Floración! Armonía {harm}% · Agua {hydr}%"
                        : (languageIndex == 1
                            ? $"🌸 {activeSp} in Full Bloom! Harmony {harm}% · Water {hydr}%"
                            : $"🌸 {activeSp} 満開 (Mankai)! 調和 {harm}% · 水分 {hydr}%");
                    treeStatusText.color = new Color(1.0f, 0.78f, 0.88f);
                }
                else if (hasPest)
                {
                    treeStatusText.text = languageIndex == 0
                        ? $"🧹 Oruga en {activeSp} · Usa la escobilla Hoki"
                        : (languageIndex == 1
                            ? $"🧹 Caterpillar on {activeSp} · Use the Hoki broom"
                            : $"🧹 {activeSp} に毛虫 · 箒 (Hoki) で払ってください");
                    treeStatusText.color = new Color(0.75f, 1.0f, 0.42f);
                }
                else if (needsWater)
                {
                    treeStatusText.text = languageIndex == 0
                        ? $"💧 {activeSp} pide agua con el cazo · Agua {hydr}%"
                        : (languageIndex == 1
                            ? $"💧 {activeSp} needs water with ladle · Water {hydr}%"
                            : $"💧 {activeSp} 水不足 · 柄杓 (Hishaku) で水やり {hydr}%");
                    treeStatusText.color = new Color(0.45f, 0.90f, 1.0f);
                }
                else if (needsPrune)
                {
                    treeStatusText.text = languageIndex == 0
                        ? $"✂️ {activeSp} tiene un brote seco · Agua {hydr}%"
                        : (languageIndex == 1
                            ? $"✂️ {activeSp} has a withered shoot · Water {hydr}%"
                            : $"✂️ {activeSp} 枯れ枝あり · 鋏 (Hasami) で剪定 {hydr}%");
                    treeStatusText.color = new Color(1.0f, 0.84f, 0.42f);
                }
                else
                {
                    treeStatusText.text = languageIndex == 0
                        ? $"🌿 {activeSp} · Armonía {harm}% · Hidratación {hydr}%"
                        : (languageIndex == 1
                            ? $"🌿 {activeSp} · Harmony {harm}% · Hydration {hydr}%"
                            : $"🌿 {activeSp} · 調和 (Harmony) {harm}% · 水分 (Water) {hydr}%");
                    treeStatusText.color = new Color(0.82f, 0.96f, 0.82f);
                }
            }
        }

        private void AnimateSakuraPetals()
        {
            if (sakuraPetalsRain != null && isPetalRainActive && sakuraPetalsRain.activeInHierarchy)
            {
                Transform rainTr = sakuraPetalsRain.transform;
                int count = rainTr.childCount;
                float t = Time.time;
                float dt = Time.deltaTime;

                for (int i = 0; i < count; i++)
                {
                    Transform petal = rainTr.GetChild(i);
                    if (petal == null) continue;

                    Vector3 pos = petal.localPosition;
                    float seed = i * 1.73f;

                    // Caída vertical suave con brisa lateral sinusoidal
                    float fallSpeed = 0.048f + (i % 5) * 0.008f;
                    pos.y -= fallSpeed * dt;
                    pos.x += Mathf.Sin(t * 1.4f + seed) * 0.032f * dt;
                    pos.z += Mathf.Cos(t * 1.1f + seed * 1.3f) * 0.026f * dt;

                    // Cuando llega a la superficie de la mesa, reaparece suavemente en la copa superior
                    if (pos.y < -0.045f)
                    {
                        float ang = (i * 22.5f + t * 12f) * Mathf.Deg2Rad;
                        float radius = 0.14f + (i % 6) * 0.048f;
                        pos.y = 0.42f + (i % 4) * 0.04f;
                        pos.x = Mathf.Cos(ang) * radius;
                        pos.z = Mathf.Sin(ang) * radius + 0.02f;
                    }

                    petal.localPosition = pos;
                    petal.Rotate(
                        (24f + (i % 3) * 8f) * dt,
                        (38f + (i % 4) * 10f) * dt,
                        (19f + (i % 2) * 12f) * dt,
                        Space.Self);
                }
            }

            // Animar elementos vivos del entorno cultural activo (cascadas, carpas Koi, farolillos flotantes)
            GameObject activeBackdrop = GetActiveEnvironmentObject();
            if (activeBackdrop != null && isVirtualEnvironmentActive && activeBackdrop.activeInHierarchy)
            {
                float dt = Time.deltaTime;
                float t = Time.time;

                Transform wfGroup = activeBackdrop.transform.Find("Sanctuary_WaterfallDrops");
                if (wfGroup != null)
                {
                    int c = wfGroup.childCount;
                    for (int i = 0; i < c; i++)
                    {
                        Transform drop = wfGroup.GetChild(i);
                        Vector3 p = drop.localPosition;
                        p.y -= (1.8f + (i % 3) * 0.35f) * dt;
                        if (p.y < -0.55f)
                        {
                            p.y = 3.2f;
                        }
                        drop.localPosition = p;
                    }
                }

                Transform koiGroup = activeBackdrop.transform.Find("Sanctuary_KoiSwimmers");
                if (koiGroup != null)
                {
                    int kc = koiGroup.childCount;
                    for (int k = 0; k < kc; k++)
                    {
                        Transform koi = koiGroup.GetChild(k);
                        float ang = t * (0.32f + k * 0.06f) + k * 2.1f;
                        float rX = 2.2f + (k % 2) * 1.1f;
                        float rZ = 1.6f + (k % 2) * 0.8f;
                        Vector3 center = new Vector3(1.2f, -0.78f, 6.8f);
                        Vector3 newPos = center + new Vector3(Mathf.Cos(ang) * rX, 0f, Mathf.Sin(ang) * rZ);
                        Vector3 vel = new Vector3(-Mathf.Sin(ang) * rX, 0f, Mathf.Cos(ang) * rZ);
                        koi.localPosition = newPos;
                        if (vel.sqrMagnitude > 0.001f)
                        {
                            koi.localRotation = Quaternion.LookRotation(vel.normalized, Vector3.up);
                        }
                    }
                }

                Transform floatingLanterns = activeBackdrop.transform.Find("Sanctuary_FloatingLanterns");
                if (floatingLanterns != null)
                {
                    int lc = floatingLanterns.childCount;
                    for (int l = 0; l < lc; l++)
                    {
                        Transform fl = floatingLanterns.GetChild(l);
                        Vector3 lp = fl.localPosition;
                        lp.y = -0.76f + Mathf.Sin(t * 1.6f + l * 1.3f) * 0.025f;
                        lp.x += Mathf.Sin(t * 0.45f + l * 0.9f) * 0.04f * dt;
                        fl.localPosition = lp;
                        fl.Rotate(0f, 8f * dt, 0f, Space.Self);
                    }
                }
            }
        }

        private void UpdateTreeButtonNeon(ZenTouchButton btn, TreeType species, string baseLabel)
        {
            if (btn == null || ZenBonsaiManager.Instance == null) return;

            bool needsCare = ZenBonsaiManager.Instance.DoesTreeNeedCare(
                species,
                out bool needsWater,
                out bool needsPrune,
                out bool hasPest,
                out float hydr,
                out float harm);

            if (needsCare)
            {
                if (needsWater && (needsPrune || hasPest))
                {
                    btn.SetNeonAlert(true, new Color(0.20f, 0.95f, 1.0f), true);
                    btn.SetLabelText(baseLabel + " 💧✂️");
                }
                else if (needsWater)
                {
                    btn.SetNeonAlert(true, new Color(0.12f, 0.88f, 1.0f), true);
                    btn.SetLabelText(baseLabel + " 💧");
                }
                else
                {
                    btn.SetNeonAlert(true, new Color(1.0f, 0.74f, 0.14f), true);
                    btn.SetLabelText(baseLabel + (hasPest ? " 🧹" : " ✂️"));
                }
            }
            else if (harm >= 95f)
            {
                btn.SetNeonAlert(true, new Color(0.45f, 1.0f, 0.65f), false);
                btn.SetLabelText(baseLabel + " ✨");
            }
            else
            {
                btn.SetNeonAlert(false, Color.black, false);
                btn.SetLabelText(baseLabel);
            }
        }

        public void SelectSakura()
        {
            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.ChangeSpecies(TreeType.Sakura);
            }
            UpdateUI();
        }

        public void SelectMatsu()
        {
            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.ChangeSpecies(TreeType.Matsu);
            }
            UpdateUI();
        }

        public void SelectMomiji()
        {
            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.ChangeSpecies(TreeType.Momiji);
            }
            UpdateUI();
        }

        [Header("Botones de Cierre Rápido")]
        public ZenTouchButton btnCloseInfo;
        public ZenTouchButton btnCloseOptions;

        public void ToggleInfoPanel()
        {
            isInfoOpen = !isInfoOpen;
            if (isInfoOpen) isOptionsOpen = false;

            if (infoPanelBox != null) infoPanelBox.SetActive(isInfoOpen);
            if (optionsPanelBox != null) optionsPanelBox.SetActive(isOptionsOpen);
            UpdateUI();
        }

        public void CloseInfoPanel()
        {
            isInfoOpen = false;
            if (infoPanelBox != null) infoPanelBox.SetActive(false);
            UpdateUI();
        }

        public void ToggleOptionsPanel()
        {
            isOptionsOpen = !isOptionsOpen;
            if (isOptionsOpen) isInfoOpen = false;

            if (optionsPanelBox != null) optionsPanelBox.SetActive(isOptionsOpen);
            if (infoPanelBox != null) infoPanelBox.SetActive(isInfoOpen);
            UpdateUI();
        }

        public void CloseOptionsPanel()
        {
            isOptionsOpen = false;
            if (optionsPanelBox != null) optionsPanelBox.SetActive(false);
            UpdateUI();
        }

        public void ToggleLockTable()
        {
            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManipulator manip = ZenBonsaiManager.Instance.GetComponent<ZenBonsaiManipulator>();
                if (manip != null)
                {
                    manip.isTableLocked = !manip.isTableLocked;
                }
            }
            UpdateOptionsButtonsUI();
        }

        public void TriggerRecenterTable()
        {
            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.RecenterInFrontOfUser();
            }
            UpdateOptionsButtonsUI();
        }

        public void CycleLightingAmbiance()
        {
            lightingPresetIndex = (lightingPresetIndex + 1) % 3;
            ApplyLightingPreset(lightingPresetIndex);
            UpdateOptionsButtonsUI();
        }

        public void CycleMusicTrack()
        {
            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.CycleMusicStyle();
            }
            UpdateOptionsButtonsUI();
        }

        private GameObject GetActiveEnvironmentObject()
        {
            if (environmentIndex <= 0) return null;
            int idx = environmentIndex - 1;
            if (virtualEnvironmentBackdrops != null && idx >= 0 && idx < virtualEnvironmentBackdrops.Length && virtualEnvironmentBackdrops[idx] != null)
            {
                return virtualEnvironmentBackdrops[idx];
            }
            return virtualSanctuaryBackdrop;
        }

        private void ApplyEnvironmentVisibility()
        {
            if (virtualEnvironmentBackdrops != null && virtualEnvironmentBackdrops.Length > 0)
            {
                for (int i = 0; i < virtualEnvironmentBackdrops.Length; i++)
                {
                    if (virtualEnvironmentBackdrops[i] != null)
                    {
                        virtualEnvironmentBackdrops[i].SetActive(environmentIndex == (i + 1));
                    }
                }
            }
            else if (virtualSanctuaryBackdrop != null)
            {
                virtualSanctuaryBackdrop.SetActive(environmentIndex > 0);
            }
        }

        public void ToggleEnvironment()
        {
            int envCount = (virtualEnvironmentBackdrops != null && virtualEnvironmentBackdrops.Length > 0)
                ? (virtualEnvironmentBackdrops.Length + 1)
                : 2;
            environmentIndex = (environmentIndex + 1) % envCount;

            ApplyEnvironmentVisibility();

            // Cambiar entre Realidad Mixta Passthrough (habitación real) y Santuarios Culturales 3D
            var ptLayers = FindObjectsOfType<OVRPassthroughLayer>();
            foreach (var pt in ptLayers)
            {
                if (pt != null) pt.enabled = !isVirtualEnvironmentActive;
            }

            ApplyLightingPreset(lightingPresetIndex);
            UpdateOptionsButtonsUI();
        }

        public void CycleTreeCareFocus()
        {
            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.CycleTreeFocusMode();
            }
            UpdateOptionsButtonsUI();
        }

        public void TogglePetalRain()
        {
            isPetalRainActive = !isPetalRainActive;
            if (sakuraPetalsRain != null)
            {
                sakuraPetalsRain.SetActive(isPetalRainActive);
            }
            UpdateOptionsButtonsUI();
        }

        public void CycleSoundVolume()
        {
            soundVolumeIndex = (soundVolumeIndex + 1) % 3;
            switch (soundVolumeIndex)
            {
                case 0: AudioListener.volume = 1.0f; break;
                case 1: AudioListener.volume = 0.40f; break;
                case 2: AudioListener.volume = 0.0f; break;
            }
            UpdateOptionsButtonsUI();
        }

        private void ApplyLightingPreset(int preset)
        {
            Light dirLight = null;
            Light[] lights = FindObjectsOfType<Light>();
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] != null && lights[i].type == LightType.Directional)
                {
                    dirLight = lights[i];
                    break;
                }
            }

            Color skyBgColor;
            if (preset == 0)
            {
                // Día Zen Sereno (Cielo azul celeste de primavera en Kioto)
                if (dirLight != null)
                {
                    dirLight.color = new Color(1.0f, 0.96f, 0.88f);
                    dirLight.intensity = 1.25f;
                }
                RenderSettings.ambientSkyColor = new Color(0.62f, 0.74f, 0.86f);
                RenderSettings.ambientEquatorColor = new Color(0.54f, 0.52f, 0.48f);
                RenderSettings.ambientGroundColor = new Color(0.28f, 0.26f, 0.22f);
                skyBgColor = new Color(0.52f, 0.72f, 0.92f, 1.0f);
            }
            else if (preset == 1)
            {
                // Atardecer Dorado en Kioto
                if (dirLight != null)
                {
                    dirLight.color = new Color(1.0f, 0.68f, 0.42f);
                    dirLight.intensity = 1.05f;
                }
                RenderSettings.ambientSkyColor = new Color(0.72f, 0.48f, 0.52f);
                RenderSettings.ambientEquatorColor = new Color(0.62f, 0.40f, 0.32f);
                RenderSettings.ambientGroundColor = new Color(0.26f, 0.18f, 0.16f);
                skyBgColor = new Color(0.84f, 0.52f, 0.44f, 1.0f);
            }
            else
            {
                // Noche de Linternas Zen
                if (dirLight != null)
                {
                    dirLight.color = new Color(0.55f, 0.68f, 0.95f);
                    dirLight.intensity = 0.62f;
                }
                RenderSettings.ambientSkyColor = new Color(0.20f, 0.28f, 0.44f);
                RenderSettings.ambientEquatorColor = new Color(0.18f, 0.22f, 0.32f);
                RenderSettings.ambientGroundColor = new Color(0.12f, 0.14f, 0.18f);
                skyBgColor = new Color(0.11f, 0.16f, 0.28f, 1.0f);
            }

            // Actualizar color de fondo de cámara (transparente para Passthrough MR, cielo sereno para Santuario Virtual)
            Camera[] cams = FindObjectsOfType<Camera>();
            for (int c = 0; c < cams.Length; c++)
            {
                if (cams[c] == null) continue;
                cams[c].clearFlags = CameraClearFlags.SolidColor;
                cams[c].backgroundColor = isVirtualEnvironmentActive ? skyBgColor : new Color(0f, 0f, 0f, 0f);
            }

            // Si existen las cúpulas celestes 360 de los Santuarios Virtuales, actualizar su tono atmosférico
            if (virtualEnvironmentBackdrops != null && virtualEnvironmentBackdrops.Length > 0)
            {
                for (int i = 0; i < virtualEnvironmentBackdrops.Length; i++)
                {
                    if (virtualEnvironmentBackdrops[i] == null) continue;
                    Transform skyDome = virtualEnvironmentBackdrops[i].transform.Find("Sanctuary_SkyDome");
                    if (skyDome != null)
                    {
                        Renderer r = skyDome.GetComponent<Renderer>();
                        if (r != null && r.material != null)
                        {
                            r.material.color = skyBgColor;
                            if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", skyBgColor);
                            if (r.material.HasProperty("_EmissionColor"))
                            {
                                r.material.EnableKeyword("_EMISSION");
                                r.material.SetColor("_EmissionColor", skyBgColor * 0.65f);
                            }
                        }
                    }
                }
            }
            else if (virtualSanctuaryBackdrop != null)
            {
                Transform skyDome = virtualSanctuaryBackdrop.transform.Find("Sanctuary_SkyDome");
                if (skyDome != null)
                {
                    Renderer r = skyDome.GetComponent<Renderer>();
                    if (r != null && r.material != null)
                    {
                        r.material.color = skyBgColor;
                        if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", skyBgColor);
                        if (r.material.HasProperty("_EmissionColor"))
                        {
                            r.material.EnableKeyword("_EMISSION");
                            r.material.SetColor("_EmissionColor", skyBgColor * 0.65f);
                        }
                    }
                }
            }
        }

        public void CycleLanguage()
        {
            languageIndex = (languageIndex + 1) % 3;
            UpdateUI();
        }

        public void CycleCareSpeed()
        {
            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.CycleCareSpeed();
            }
            UpdateUI();
        }

        public void UpdateUI()
        {
            TreeType sp = TreeType.Sakura;
            if (ZenBonsaiManager.Instance != null && ZenBonsaiManager.Instance.organicTree != null)
            {
                sp = ZenBonsaiManager.Instance.organicTree.currentSpecies;
            }

            // Actualizar subtítulos de los medallones y botones principales según idioma
            if (btnSakura != null)
            {
                btnSakura.SetSubLabelText(languageIndex == 0 ? "Cerezo Flor" : (languageIndex == 1 ? "Cherry Blossom" : "染井吉野 · Yoshino"));
                btnSakura.SetSelectedHighlight(sp == TreeType.Sakura);
            }
            if (btnMatsu != null)
            {
                btnMatsu.SetSubLabelText(languageIndex == 0 ? "Pino Negro" : (languageIndex == 1 ? "Black Pine" : "黒松 · Kuromatsu"));
                btnMatsu.SetSelectedHighlight(sp == TreeType.Matsu);
            }
            if (btnMomiji != null)
            {
                btnMomiji.SetSubLabelText(languageIndex == 0 ? "Arce Rojo" : (languageIndex == 1 ? "Japanese Maple" : "山紅葉 · Kaede"));
                btnMomiji.SetSelectedHighlight(sp == TreeType.Momiji);
            }

            if (btnInfo != null)
            {
                btnInfo.SetLabelText(languageIndex == 0 ? "📖 Cuidados" : (languageIndex == 1 ? "📖 Care Guide" : "📖 育成指南"));
                btnInfo.SetSubLabelText(languageIndex == 0 ? "Ficha Botánica" : (languageIndex == 1 ? "Botanical Scroll" : "盆栽の心得 · Lore"));
                btnInfo.SetSelectedHighlight(isInfoOpen);
            }
            if (btnOptions != null)
            {
                btnOptions.SetLabelText(languageIndex == 0 ? "⚙️ Opciones" : (languageIndex == 1 ? "⚙️ Settings" : "⚙️ 環境設定"));
                btnOptions.SetSubLabelText(languageIndex == 0 ? "Ajustes Zen" : (languageIndex == 1 ? "Zen Sanctuary" : "禅の調整 · Zen"));
                btnOptions.SetSelectedHighlight(isOptionsOpen);
            }

            string closeTxt = languageIndex == 0 ? "✕ Cerrar" : (languageIndex == 1 ? "✕ Close" : "✕ 閉じる");
            if (btnCloseInfo != null) btnCloseInfo.SetLabelText(closeTxt);
            if (btnCloseOptions != null) btnCloseOptions.SetLabelText(closeTxt);

            if (optionsPanelTitle != null)
            {
                optionsPanelTitle.text = languageIndex == 0
                    ? "⚙️ PERGAMINO DE OPCIONES DEL SANTUARIO"
                    : (languageIndex == 1
                        ? "⚙️ ZEN SANCTUARY SETTINGS SCROLL"
                        : "⚙️ 禅の環境設定 · SANCTUARY SCROLL");
            }
            if (optionsPanelSubtitle != null)
            {
                optionsPanelSubtitle.text = languageIndex == 0
                    ? "Toca con el dedo índice para personalizar el idioma, música, entorno y ritmo de cuidado"
                    : (languageIndex == 1
                        ? "Touch with your index finger to customize language, music, environment & care pace"
                        : "人差し指で触れて、言語・音楽・庭園環境・盆栽の育成速度を自由に調整できます");
            }

            UpdateInfoPanelContent(sp);
            UpdateOptionsButtonsUI();
        }

        private void UpdateOptionsButtonsUI()
        {
            if (btnOptLockTable != null)
            {
                bool locked = false;
                if (ZenBonsaiManager.Instance != null)
                {
                    ZenBonsaiManipulator manip = ZenBonsaiManager.Instance.GetComponent<ZenBonsaiManipulator>();
                    if (manip != null) locked = manip.isTableLocked;
                }
                string lockTxt;
                if (languageIndex == 0)
                    lockTxt = locked ? "🔒 Mesa:\nFija (Bloqueada)" : "🔓 Mesa:\nMóvil (2 Manos)";
                else if (languageIndex == 1)
                    lockTxt = locked ? "🔒 Table:\nLocked in Place" : "🔓 Table:\nMovable (2 Hands)";
                else
                    lockTxt = locked ? "🔒 盆栽卓:\n固定 (Locked)" : "🔓 盆栽卓:\n両手移動 (Move)";

                btnOptLockTable.SetLabelText(lockTxt);
                btnOptLockTable.SetSelectedHighlight(locked);
            }

            if (btnOptRecenter != null)
            {
                string recTxt = languageIndex == 0
                    ? "🎯 Centrar\nMesa Frente a Mí"
                    : (languageIndex == 1 ? "🎯 Recenter\nTable in Front" : "🎯 正面に配置\n(Recenter Table)");
                btnOptRecenter.SetLabelText(recTxt);
                btnOptRecenter.SetSelectedHighlight(false);
            }

            if (btnOptLanguage != null)
            {
                string langTxt = languageIndex == 0
                    ? "🌐 Idioma:\nEspañol (ES)"
                    : (languageIndex == 1 ? "🌐 Language:\nEnglish (EN)" : "🌐 言語 (Lang):\n日本語 (JA)");
                btnOptLanguage.SetLabelText(langTxt);
                btnOptLanguage.SetSelectedHighlight(languageIndex != 0);
            }

            if (btnOptCareSpeed != null)
            {
                int spd = ZenBonsaiManager.Instance != null ? ZenBonsaiManager.Instance.careSpeedIndex : 0;
                string spdTxt;
                if (languageIndex == 0)
                {
                    spdTxt = spd == 0 ? "⏳ Ritmo Cuidado:\nNormal (1.0x)" :
                            (spd == 1 ? "⚡ Ritmo Cuidado:\nRápido (2.2x)" :
                            (spd == 2 ? "🔥 Ritmo Cuidado:\nMaestro (3.8x)" : "🍃 Ritmo Cuidado:\nLento Zen (0.5x)"));
                }
                else if (languageIndex == 1)
                {
                    spdTxt = spd == 0 ? "⏳ Care Pace:\nNormal (1.0x)" :
                            (spd == 1 ? "⚡ Care Pace:\nFast (2.2x)" :
                            (spd == 2 ? "🔥 Care Pace:\nMaster (3.8x)" : "🍃 Care Pace:\nSlow Zen (0.5x)"));
                }
                else
                {
                    spdTxt = spd == 0 ? "⏳ 育成速度:\n標準 (Normal 1x)" :
                            (spd == 1 ? "⚡ 育成速度:\n速い (Fast 2.2x)" :
                            (spd == 2 ? "🔥 育成速度:\n達人 (Master 3.8x)" : "🍃 育成速度:\nゆったり (Slow)"));
                }
                btnOptCareSpeed.SetLabelText(spdTxt);
                btnOptCareSpeed.SetSelectedHighlight(spd != 0);
            }

            if (btnOptLighting != null)
            {
                string lightLabel;
                if (languageIndex == 0)
                {
                    lightLabel = lightingPresetIndex == 0 ? "🌅 Ambiente:\nDía Zen" :
                                (lightingPresetIndex == 1 ? "🌇 Ambiente:\nAtardecer Kioto" : "🌙 Ambiente:\nNoche Zen");
                }
                else if (languageIndex == 1)
                {
                    lightLabel = lightingPresetIndex == 0 ? "🌅 Ambiance:\nZen Daylight" :
                                (lightingPresetIndex == 1 ? "🌇 Ambiance:\nKyoto Sunset" : "🌙 Ambiance:\nLantern Night");
                }
                else
                {
                    lightLabel = lightingPresetIndex == 0 ? "🌅 照明 (Light):\n昼の禅 (Day)" :
                                (lightingPresetIndex == 1 ? "🌇 照明 (Light):\n京都の夕暮れ" : "🌙 照明 (Light):\n灯籠の夜 (Night)");
                }
                btnOptLighting.SetLabelText(lightLabel);
                btnOptLighting.SetSelectedHighlight(lightingPresetIndex != 0);
            }

            if (btnOptMusic != null)
            {
                int style = ZenSoundSynthesizer.Instance != null ? ZenSoundSynthesizer.Instance.CurrentMusicStyle : 0;
                string musicLabel;
                if (languageIndex == 0)
                {
                    switch (style)
                    {
                        case 0: musicLabel = "🎵 Música:\nFlauta Shakuhachi"; break;
                        case 1: musicLabel = "🎶 Música:\nArpa Koto Imperial"; break;
                        case 2: musicLabel = "🔔 Música:\nCuencos Tibetanos"; break;
                        case 3: musicLabel = "🌧 Música:\nLluvia y Bambú"; break;
                        case 4: musicLabel = "🌊 Música:\nCascada y Brisa"; break;
                        case 5: musicLabel = "🎐 Música:\nCampanas Fūrin"; break;
                        default: musicLabel = "🔇 Música:\nSilencio"; break;
                    }
                }
                else if (languageIndex == 1)
                {
                    switch (style)
                    {
                        case 0: musicLabel = "🎵 Music:\nShakuhachi Flute"; break;
                        case 1: musicLabel = "🎶 Music:\nImperial Koto Harp"; break;
                        case 2: musicLabel = "🔔 Music:\nSinging Bowls"; break;
                        case 3: musicLabel = "🌧 Music:\nKyoto Rain & Bamboo"; break;
                        case 4: musicLabel = "🌊 Music:\nWaterfall & Breeze"; break;
                        case 5: musicLabel = "🎐 Music:\nFūrin Wind Chimes"; break;
                        default: musicLabel = "🔇 Music:\nSilence"; break;
                    }
                }
                else
                {
                    switch (style)
                    {
                        case 0: musicLabel = "🎵 音楽:\n尺八 (Shakuhachi)"; break;
                        case 1: musicLabel = "🎶 音楽:\n箏曲 (Imperial Koto)"; break;
                        case 2: musicLabel = "🔔 音楽:\nおりん (Zen Bowls)"; break;
                        case 3: musicLabel = "🌧 音楽:\n京都の雨と鹿威し"; break;
                        case 4: musicLabel = "🌊 音楽:\n富士の滝と風"; break;
                        case 5: musicLabel = "🎐 音楽:\n風鈴と梵鐘 (Fūrin)"; break;
                        default: musicLabel = "🔇 音楽:\n静寂 (Silence)"; break;
                    }
                }
                btnOptMusic.SetLabelText(musicLabel);
                btnOptMusic.SetSelectedHighlight(style != 6);
            }

            if (btnOptEnvironment != null)
            {
                string envTxt;
                if (languageIndex == 0)
                {
                    switch (environmentIndex)
                    {
                        case 0: envTxt = "🥽 Entorno:\nPassthrough MR"; break;
                        case 1: envTxt = "🗻 Entorno:\nJardín Monte Fuji"; break;
                        case 2: envTxt = "🏮 Entorno:\nPagoda de Kioto"; break;
                        default: envTxt = "🎋 Entorno:\nTemplo Bambú Zen"; break;
                    }
                }
                else if (languageIndex == 1)
                {
                    switch (environmentIndex)
                    {
                        case 0: envTxt = "🥽 Environment:\nPassthrough MR"; break;
                        case 1: envTxt = "🗻 Environment:\nMt. Fuji Veranda"; break;
                        case 2: envTxt = "🏮 Environment:\nKyoto Lake Pagoda"; break;
                        default: envTxt = "🎋 Environment:\nBamboo Zen Temple"; break;
                    }
                }
                else
                {
                    switch (environmentIndex)
                    {
                        case 0: envTxt = "🥽 背景環境:\n複合現実 (MR)"; break;
                        case 1: envTxt = "🗻 背景環境:\n富士と縁側庭園"; break;
                        case 2: envTxt = "🏮 背景環境:\n京都・五重塔の湖"; break;
                        default: envTxt = "🎋 背景環境:\n嵐山・枯山水禅寺"; break;
                    }
                }

                btnOptEnvironment.SetLabelText(envTxt);
                btnOptEnvironment.SetSelectedHighlight(isVirtualEnvironmentActive);
            }

            if (btnOptTreeFocus != null)
            {
                int mode = ZenBonsaiManager.Instance != null ? ZenBonsaiManager.Instance.treeFocusMode : 3;
                string modeLabel;
                if (languageIndex == 0)
                {
                    modeLabel = mode == 1 ? "🌿 Cuidado:\n1 Árbol (Enfoque)" :
                               (mode == 2 ? "🌿 Cuidado:\n2 Árboles" : "🌿 Cuidado:\n3 Árboles (Tríada)");
                }
                else if (languageIndex == 1)
                {
                    modeLabel = mode == 1 ? "🌿 Active Care:\n1 Tree (Focus)" :
                               (mode == 2 ? "🌿 Active Care:\n2 Trees" : "🌿 Active Care:\n3 Trees (Triad)");
                }
                else
                {
                    modeLabel = mode == 1 ? "🌿 育成対象:\n一鉢集中 (1 Tree)" :
                               (mode == 2 ? "🌿 育成対象:\n二鉢 (2 Trees)" : "🌿 育成対象:\n三鉢すべて (All 3)");
                }
                btnOptTreeFocus.SetLabelText(modeLabel);
                btnOptTreeFocus.SetSelectedHighlight(mode != 3);
            }

            if (btnOptSound != null)
            {
                string soundLabel;
                if (languageIndex == 0)
                {
                    soundLabel = soundVolumeIndex == 0 ? "🔊 Sonido:\n100% (Normal)" :
                                (soundVolumeIndex == 1 ? "🔉 Sonido:\nSuave (40%)" : "🔇 Sonido:\nSilencio");
                }
                else if (languageIndex == 1)
                {
                    soundLabel = soundVolumeIndex == 0 ? "🔊 Sound:\n100% (Full)" :
                                (soundVolumeIndex == 1 ? "🔉 Sound:\nSoft (40%)" : "🔇 Sound:\nMuted");
                }
                else
                {
                    soundLabel = soundVolumeIndex == 0 ? "🔊 音量:\n100% (最大)" :
                                (soundVolumeIndex == 1 ? "🔉 音量:\n控えめ (40%)" : "🔇 音量:\n消音 (Mute)");
                }
                btnOptSound.SetLabelText(soundLabel);
                btnOptSound.SetSelectedHighlight(soundVolumeIndex != 0);
            }

            if (btnOptPetals != null)
            {
                string petTxt;
                if (languageIndex == 0)
                    petTxt = isPetalRainActive ? "🌸 Pétalos:\nActivados" : "🌸 Pétalos:\nDesactivados";
                else if (languageIndex == 1)
                    petTxt = isPetalRainActive ? "🌸 Sakura Petals:\nEnabled" : "🌸 Sakura Petals:\nDisabled";
                else
                    petTxt = isPetalRainActive ? "🌸 桜吹雪:\nあり (On)" : "🌸 桜吹雪:\nなし (Off)";

                btnOptPetals.SetLabelText(petTxt);
                btnOptPetals.SetSelectedHighlight(isPetalRainActive);
            }
        }

        private void UpdateInfoPanelContent(TreeType sp)
        {
            if (infoPanelTitle == null || infoPanelBody == null) return;

            switch (sp)
            {
                case TreeType.Sakura:
                    infoPanelTitle.color = new Color(1.0f, 0.78f, 0.88f);
                    if (languageIndex == 0)
                    {
                        infoPanelTitle.text = "🌸 Prunus serrulata · Sakura Moyogi (Cerezo en Flor)";
                        infoPanelBody.text =
                            "• Estilo: Moyogi (Vertical Informal) · Edad: ~45 años\n" +
                            "• Corteza: Castaño caoba satinado con lenticelas horizontales y raíces Nebari.\n" +
                            "• Simbolismo: Mono no aware (la serenidad efímera de la primavera japonesa).\n\n" +
                            "🛠 GUÍA DE CUIDADO (AVISO NEÓN CIRCULAR):\n" +
                            "💧 Riego: Toma el cazo de bambú (Hishaku) de la izquierda e inclínalo cuando brille el neón azul.\n" +
                            "✂️ Poda: Corta los brotes secos con las tijeras Hasami si se ilumina el neón ámbar.\n" +
                            "🧹 Limpieza: Desliza la escobilla Hoki de la derecha para retirar orugas del tronco.";
                    }
                    else if (languageIndex == 1)
                    {
                        infoPanelTitle.text = "🌸 Prunus serrulata · Sakura Moyogi (Cherry Blossom)";
                        infoPanelBody.text =
                            "• Style: Moyogi (Informal Upright) · Age: ~45 years\n" +
                            "• Bark: Satin mahogany chestnut with horizontal lenticels and Nebari surface roots.\n" +
                            "• Symbolism: Mono no aware (the serene, fleeting beauty of Japanese spring).\n\n" +
                            "🛠 CARE GUIDE (CIRCULAR NEON HALO):\n" +
                            "💧 Watering: Pinch the bamboo ladle (Hishaku) on the left and tilt gently when blue neon glows.\n" +
                            "✂️ Pruning: Snip withered shoots with the Hasami shears when amber neon glows.\n" +
                            "🧹 Sweeping: Brush away caterpillars gently using the Hoki broom on the right tray.";
                    }
                    else
                    {
                        infoPanelTitle.text = "🌸 染井吉野 (Prunus serrulata) · 桜 模様木 (Sakura Moyogi)";
                        infoPanelBody.text =
                            "• 樹形 (Style): 模様木 (Moyogi) · 樹齢 (Age): 約45年\n" +
                            "• 樹皮 (Bark): 艶やかな樺色の樹皮と力強い根張り (Nebari)。\n" +
                            "• 象徴 (Spirit): 「もののあわれ」— 春の儚くも優美な静寂。\n\n" +
                            "🛠 盆栽のお手入れ指南 (光輪の合図):\n" +
                            "💧 水やり (Water): 青く光る時、左の竹柄杓 (Hishaku) を傾けて水を注ぎます。\n" +
                            "✂️ 剪定 (Prune): 琥珀色に光る時、剪定鋏 (Hasami) で枯れ枝を整えます。\n" +
                            "🧹 掃除 (Sweep): 棕櫚箒 (Hoki) で幹の害虫を優しく払い落とします。";
                    }
                    break;

                case TreeType.Matsu:
                    infoPanelTitle.color = new Color(0.65f, 0.96f, 0.75f);
                    if (languageIndex == 0)
                    {
                        infoPanelTitle.text = "🌲 Pinus thunbergii · Kuromatsu (Pino Negro Japonés)";
                        infoPanelBody.text =
                            "• Estilo: Ishitsuki / Moyogi · Edad: ~80 años\n" +
                            "• Corteza: Placas rugosas milenarias de pino negro abrazando roca Suiseki.\n" +
                            "• Simbolismo: Longevidad, fortaleza serena y resistencia ante el paso del tiempo.\n\n" +
                            "🛠 GUÍA DE CUIDADO (AVISO NEÓN CIRCULAR):\n" +
                            "💧 Riego: Consumo pausado; vierte agua fresca con el cazo Hishaku si brilla su neón.\n" +
                            "✂️ Poda: Pinzado de nubes de acículas con las tijeras para mantener su silueta.\n" +
                            "🧹 Limpieza: Mantén las almohadillas de musgo y la corteza libres de orugas.";
                    }
                    else if (languageIndex == 1)
                    {
                        infoPanelTitle.text = "🌲 Pinus thunbergii · Kuromatsu (Japanese Black Pine)";
                        infoPanelBody.text =
                            "• Style: Ishitsuki / Moyogi · Age: ~80 years\n" +
                            "• Bark: Ancient rugged black pine plates embracing a natural Suiseki stone.\n" +
                            "• Symbolism: Longevity, quiet strength, and timeless endurance.\n\n" +
                            "🛠 CARE GUIDE (CIRCULAR NEON HALO):\n" +
                            "💧 Watering: Slow water consumption; pour fresh water with the Hishaku ladle when alerted.\n" +
                            "✂️ Pruning: Trim dry needle clouds with the shears to preserve its layered silhouette.\n" +
                            "🧹 Sweeping: Keep the moss pads and rugged bark free of caterpillars with the broom.";
                    }
                    else
                    {
                        infoPanelTitle.text = "🌲 黒松 (Pinus thunbergii) ·Kuromatsu (Japanese Black Pine)";
                        infoPanelBody.text =
                            "• 樹形 (Style): 石付き模様木 (Ishitsuki) · 樹齢 (Age): 約80年\n" +
                            "• 樹皮 (Bark): 歳月を重ねた亀甲肌の幹と水石 (Suiseki) の調和。\n" +
                            "• 象徴 (Spirit): 不老長寿と揺るぎない静寂の風格。\n\n" +
                            "🛠 盆栽のお手入れ指南 (光輪の合図):\n" +
                            "💧 水やり (Water): 乾燥しすぎた時、柄杓 (Hishaku) で静かに潤します。\n" +
                            "✂️ 剪定 (Prune): 鋏 (Hasami) で古葉や枯れ枝を剪定し、棚を整えます。\n" +
                            "🧹 掃除 (Sweep): 箒 (Hoki) で苔と幹を清らかに保ちます。";
                    }
                    break;

                case TreeType.Momiji:
                    infoPanelTitle.color = new Color(1.0f, 0.70f, 0.50f);
                    if (languageIndex == 0)
                    {
                        infoPanelTitle.text = "🍁 Acer palmatum · Yama-Momiji (Arce Rojo de Kioto)";
                        infoPanelBody.text =
                            "• Estilo: Sokan (Doble Tronco Padre-Hijo) · Edad: ~60 años\n" +
                            "• Corteza: Gris canela suave con estrías naturales y base de musgo aterciopelado.\n" +
                            "• Simbolismo: Momijigari (contemplación serena del otoño carmesí en Kioto).\n\n" +
                            "🛠 GUÍA DE CUIDADO (AVISO NEÓN CIRCULAR):\n" +
                            "💧 Riego: Hidrata a menudo sus hojas palmadas con el cazo de bambú cuando avise.\n" +
                            "✂️ Poda: Selección cuidadosa de ramillas secas con las tijeras de la bandeja.\n" +
                            "🧹 Limpieza: Pasa la escobilla Hoki si aparece alguna oruga entre las ramas.";
                    }
                    else if (languageIndex == 1)
                    {
                        infoPanelTitle.text = "🍁 Acer palmatum · Yama-Momiji (Kyoto Red Maple)";
                        infoPanelBody.text =
                            "• Style: Sokan (Twin Trunk Father-Child) · Age: ~60 years\n" +
                            "• Bark: Smooth cinnamon-grey bark with natural striations and velvet moss base.\n" +
                            "• Symbolism: Momijigari (peaceful contemplation of crimson autumn in Kyoto).\n\n" +
                            "🛠 CARE GUIDE (CIRCULAR NEON HALO):\n" +
                            "💧 Watering: Hydrate its delicate palmate leaves frequently with the bamboo ladle.\n" +
                            "✂️ Pruning: Carefully trim dry twigs using the Hasami shears on the right tray.\n" +
                            "🧹 Sweeping: Brush gently with the Hoki broom if a caterpillar climbs the trunk.";
                    }
                    else
                    {
                        infoPanelTitle.text = "🍁 山紅葉 (Acer palmatum) · 双幹 (Yama-Momiji Sokan)";
                        infoPanelBody.text =
                            "• 樹形 (Style): 双幹 (Sokan · 親子双幹) · 樹齢 (Age): 約60年\n" +
                            "• 樹皮 (Bark): 滑らかな灰褐色の幹肌と深い京苔のコントラスト。\n" +
                            "• 象徴 (Spirit): 「紅葉狩り」— 京都の錦秋を映す雅な彩り。\n\n" +
                            "🛠 盆栽のお手入れ指南 (光輪の合図):\n" +
                            "💧 水やり (Water): 水を好むため、柄杓 (Hishaku) でこまめに水を与えます。\n" +
                            "✂️ 剪定 (Prune): 鋏 (Hasami) で枯れた小枝を丁寧に切り戻します。\n" +
                            "🧹 掃除 (Sweep): 箒 (Hoki) で葉や幹についた害虫を取り除きます。";
                    }
                    break;
            }
        }
    }
}

