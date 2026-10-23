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
        [Header("Panel de Opciones del Santuario (Options Panel)")]
        public GameObject optionsPanelBox;
        public ZenTouchButton btnOptLockTable;
        public ZenTouchButton btnOptRecenter;
        public ZenTouchButton btnOptLighting;
        public ZenTouchButton btnOptSound;
        public ZenTouchButton btnOptMusic;
        public ZenTouchButton btnOptEnvironment;
        public ZenTouchButton btnOptTreeFocus;
        public ZenTouchButton btnOptPetals;

        [Header("Entorno Virtual Zen (Opciones)")]
        public GameObject virtualSanctuaryBackdrop;
        public GameObject sakuraPetalsRain;
        private bool isVirtualEnvironmentActive = false;
        private bool isPetalRainActive = true;

        private bool isInfoOpen = false;
        private bool isOptionsOpen = false;
        private int lightingPresetIndex = 0; // 0 = Día Zen, 1 = Atardecer Kyoto, 2 = Noche de Linternas
        private int soundVolumeIndex = 0;    // 0 = 100%, 1 = 40% Suave, 2 = Silencio

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
            if (btnOptLighting != null) btnOptLighting.onClick.AddListener(CycleLightingAmbiance);
            if (btnOptSound != null) btnOptSound.onClick.AddListener(CycleSoundVolume);
            if (btnOptMusic != null) btnOptMusic.onClick.AddListener(CycleMusicTrack);
            if (btnOptEnvironment != null) btnOptEnvironment.onClick.AddListener(ToggleEnvironment);
            if (btnOptTreeFocus != null) btnOptTreeFocus.onClick.AddListener(CycleTreeCareFocus);
            if (btnOptPetals != null) btnOptPetals.onClick.AddListener(TogglePetalRain);

            if (infoPanelBox != null) infoPanelBox.SetActive(isInfoOpen);
            if (optionsPanelBox != null) optionsPanelBox.SetActive(isOptionsOpen);
            if (virtualSanctuaryBackdrop != null) virtualSanctuaryBackdrop.SetActive(isVirtualEnvironmentActive);
            if (sakuraPetalsRain != null) sakuraPetalsRain.SetActive(isPetalRainActive);

            UpdateUI();
        }

        private void Update()
        {
            if (ZenBonsaiManager.Instance == null) return;

            // 1. Actualizar el neón alrededor del nombre de cada árbol según sus necesidades individuales
            UpdateTreeButtonNeon(btnSakura, TreeType.Sakura, "🌸\nSakura");
            UpdateTreeButtonNeon(btnMatsu, TreeType.Matsu, "🌲\nMatsu");
            UpdateTreeButtonNeon(btnMomiji, TreeType.Momiji, "🍁\nMomiji");

            // 2. Animar caída suave y balanceo orgánico de los pétalos de Sakura alrededor de la mesa
            AnimateSakuraPetals();

            // 3. Actualizar estado del bonsái activo en el pie del panel
            if (treeStatusText != null)
            {
                int harm = Mathf.RoundToInt(ZenBonsaiManager.Instance.harmony);
                int hydr = Mathf.RoundToInt(ZenBonsaiManager.Instance.hydration);
                TreeType activeSp = ZenBonsaiManager.Instance.ActiveSpecies;

                ZenBonsaiManager.Instance.DoesTreeNeedCare(activeSp, out bool needsWater, out bool needsPrune, out bool hasPest, out _, out _);

                if (ZenBonsaiManager.Instance.currentState == BonsaiState.FullBloom)
                {
                    treeStatusText.text = $"🌸 ¡{activeSp} en Floración! Armonía {harm}% · Agua {hydr}%";
                    treeStatusText.color = new Color(1.0f, 0.78f, 0.88f);
                }
                else if (hasPest)
                {
                    treeStatusText.text = $"🧹 Oruga en {activeSp} · Usa la escobilla Hoki";
                    treeStatusText.color = new Color(0.75f, 1.0f, 0.42f);
                }
                else if (needsWater)
                {
                    treeStatusText.text = $"💧 {activeSp} pide agua con el cazo · Agua {hydr}%";
                    treeStatusText.color = new Color(0.45f, 0.90f, 1.0f);
                }
                else if (needsPrune)
                {
                    treeStatusText.text = $"✂️ {activeSp} tiene un brote seco · Agua {hydr}%";
                    treeStatusText.color = new Color(1.0f, 0.84f, 0.42f);
                }
                else
                {
                    treeStatusText.text = $"🌿 {activeSp} · Armonía {harm}% · Hidratación {hydr}%";
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

            // Animar cascada del Santuario Virtual si está activo
            if (virtualSanctuaryBackdrop != null && isVirtualEnvironmentActive && virtualSanctuaryBackdrop.activeInHierarchy)
            {
                Transform wfGroup = virtualSanctuaryBackdrop.transform.Find("Sanctuary_WaterfallDrops");
                if (wfGroup != null)
                {
                    float dt = Time.deltaTime;
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

        public void ToggleEnvironment()
        {
            isVirtualEnvironmentActive = !isVirtualEnvironmentActive;
            if (virtualSanctuaryBackdrop != null)
            {
                virtualSanctuaryBackdrop.SetActive(isVirtualEnvironmentActive);
            }

            // Cambiar entre Realidad Mixta Passthrough (habitación real) y Santuario Virtual 3D
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

            // Si existe la cúpula celeste 360 del Santuario Virtual, actualizar su tono atmosférico
            if (virtualSanctuaryBackdrop != null)
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

        public void UpdateUI()
        {
            TreeType sp = TreeType.Sakura;
            if (ZenBonsaiManager.Instance != null && ZenBonsaiManager.Instance.organicTree != null)
            {
                sp = ZenBonsaiManager.Instance.organicTree.currentSpecies;
            }

            if (btnSakura != null) btnSakura.SetSelectedHighlight(sp == TreeType.Sakura);
            if (btnMatsu != null) btnMatsu.SetSelectedHighlight(sp == TreeType.Matsu);
            if (btnMomiji != null) btnMomiji.SetSelectedHighlight(sp == TreeType.Momiji);

            if (btnInfo != null) btnInfo.SetSelectedHighlight(isInfoOpen);
            if (btnOptions != null) btnOptions.SetSelectedHighlight(isOptionsOpen);

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
                btnOptLockTable.SetLabelText(locked ? "🔒 Mesa:\nFija (Bloqueada)" : "🔓 Mesa:\nMóvil (2 Manos)");
                btnOptLockTable.SetSelectedHighlight(locked);
            }

            if (btnOptRecenter != null)
            {
                btnOptRecenter.SetLabelText("🎯 Centrar\nMesa Frente a Mí");
                btnOptRecenter.SetSelectedHighlight(false);
            }

            if (btnOptLighting != null)
            {
                string lightLabel = lightingPresetIndex == 0
                    ? "🌅 Ambiente:\nDía Zen"
                    : (lightingPresetIndex == 1 ? "🌇 Ambiente:\nAtardecer Kioto" : "🌙 Ambiente:\nNoche Zen");
                btnOptLighting.SetLabelText(lightLabel);
                btnOptLighting.SetSelectedHighlight(lightingPresetIndex != 0);
            }

            if (btnOptMusic != null)
            {
                int style = ZenSoundSynthesizer.Instance != null ? ZenSoundSynthesizer.Instance.CurrentMusicStyle : 0;
                string musicLabel = style == 0 ? "🎵 Música:\nShakuhachi" :
                                    (style == 1 ? "🔔 Música:\nCuencos Zen" :
                                    (style == 2 ? "🌊 Música:\nCascada y Viento" : "🔇 Música:\nSilencio"));
                btnOptMusic.SetLabelText(musicLabel);
                btnOptMusic.SetSelectedHighlight(style != 3);
            }

            if (btnOptEnvironment != null)
            {
                btnOptEnvironment.SetLabelText(isVirtualEnvironmentActive ? "⛩ Entorno:\nSantuario Fuji" : "🥽 Entorno:\nPassthrough MR");
                btnOptEnvironment.SetSelectedHighlight(isVirtualEnvironmentActive);
            }

            if (btnOptTreeFocus != null)
            {
                int mode = ZenBonsaiManager.Instance != null ? ZenBonsaiManager.Instance.treeFocusMode : 3;
                string modeLabel = mode == 1 ? "🌿 Cuidado:\n1 Árbol (Enfoque)" :
                                   (mode == 2 ? "🌿 Cuidado:\n2 Árboles" : "🌿 Cuidado:\n3 Árboles (Tríada)");
                btnOptTreeFocus.SetLabelText(modeLabel);
                btnOptTreeFocus.SetSelectedHighlight(mode != 3);
            }

            if (btnOptSound != null)
            {
                string soundLabel = soundVolumeIndex == 0
                    ? "🔊 Sonido:\n100%"
                    : (soundVolumeIndex == 1 ? "🔉 Sonido:\nSuave (40%)" : "🔇 Sonido:\nSilencio");
                btnOptSound.SetLabelText(soundLabel);
                btnOptSound.SetSelectedHighlight(soundVolumeIndex != 0);
            }

            if (btnOptPetals != null)
            {
                btnOptPetals.SetLabelText(isPetalRainActive ? "🌸 Pétalos:\nActivados" : "🌸 Pétalos:\nDesactivados");
                btnOptPetals.SetSelectedHighlight(isPetalRainActive);
            }
        }

        private void UpdateInfoPanelContent(TreeType sp)
        {
            if (infoPanelTitle == null || infoPanelBody == null) return;

            switch (sp)
            {
                case TreeType.Sakura:
                    infoPanelTitle.text = "🌸 Prunus serrulata · Sakura Moyogi";
                    infoPanelTitle.color = new Color(1.0f, 0.78f, 0.88f);
                    infoPanelBody.text =
                        "• Estilo: Moyogi (Vertical Informal) · Edad: ~45 años\n" +
                        "• Corteza: Castaño caoba satinado con lenticelas horizontales y raíces Nebari.\n" +
                        "• Simbolismo: Mono no aware (la serenidad efímera de la primavera japonesa).\n\n" +
                        "🛠 CUIDADO INDEPENDIENTE (AVISO NEÓN):\n" +
                        "💧 Riego: Usa el cazo de bambú (Hishaku) de la izquierda cuando brille el neón azul.\n" +
                        "✂️ Poda: Corta con calma los brotes secos con las tijeras Hasami si brilla el neón ámbar.\n" +
                        "🧹 Limpieza: Retira suavemente las orugas con la escobilla Hoki de la derecha.";
                    break;

                case TreeType.Matsu:
                    infoPanelTitle.text = "🌲 Pinus thunbergii · Kuromatsu (Pino Negro)";
                    infoPanelTitle.color = new Color(0.65f, 0.96f, 0.75f);
                    infoPanelBody.text =
                        "• Estilo: Ishitsuki / Moyogi · Edad: ~80 años\n" +
                        "• Corteza: Placas rugosas milenarias de pino negro sobre roca Suiseki.\n" +
                        "• Simbolismo: Longevidad, fortaleza serena y resistencia ante el paso del tiempo.\n\n" +
                        "🛠 CUIDADO INDEPENDIENTE (AVISO NEÓN):\n" +
                        "💧 Riego: Consumo lento; vierte agua fresca con el cazo Hishaku si se ilumina su neón.\n" +
                        "✂️ Poda: Pinzado de nubes de acículas con las tijeras para mantener su silueta.\n" +
                        "🧹 Limpieza: Mantén las almohadillas y el tronco libres de orugas con la escobilla.";
                    break;

                case TreeType.Momiji:
                    infoPanelTitle.text = "🍁 Acer palmatum · Yama-Momiji (Arce Japonés)";
                    infoPanelTitle.color = new Color(1.0f, 0.70f, 0.50f);
                    infoPanelBody.text =
                        "• Estilo: Sokan (Doble Tronco Padre-Hijo) · Edad: ~60 años\n" +
                        "• Corteza: Gris canela suave con estrías naturales y base musgosa.\n" +
                        "• Simbolismo: Momijigari (contemplación tranquila del otoño en Kioto).\n\n" +
                        "🛠 CUIDADO INDEPENDIENTE (AVISO NEÓN):\n" +
                        "💧 Riego: Hidrata sus hojas palmadas con el cazo de bambú cuando su neón avise.\n" +
                        "✂️ Poda: Selección pausada de ramillas marchitas con las tijeras de la derecha.\n" +
                        "🧹 Limpieza: Desliza la escobilla Hoki si aparece alguna oruga en el tronco.";
                    break;
            }
        }
    }
}

