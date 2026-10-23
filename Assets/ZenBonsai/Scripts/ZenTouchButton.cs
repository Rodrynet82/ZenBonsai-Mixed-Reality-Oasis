using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZenBonsai
{
    /// <summary>
    /// Botón 3D físico para Meta Quest 3 con respuesta táctil, iluminación al activarse y sonido al tacto directo con el dedo índice.
    /// Compensa la altura entre el collider de la mano y la yema del dedo índice y garantiza que SOLO se pulse el botón más cercano.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ZenTouchButton : MonoBehaviour
    {
        private static readonly List<ZenTouchButton> allButtons = new List<ZenTouchButton>();
        public static float GlobalPressCooldownUntil = 0f;

        [Header("Acción")]
        public UnityEvent onClick;

        [Header("Sensación Física (Poking)")]
        [Tooltip("Parte visual que se hunde e ilumina al presionar")]
        public Transform visualPlate;
        public Vector3 pressDisplacement = new Vector3(0f, 0f, 0.012f); // Se hunde 1.2 cm hacia adentro
        public float returnSpeed = 12f;

        [Header("Marco de Neón (Aviso de Cuidado)")]
        public Renderer[] neonBorderRenderers;
        public UnityEngine.UI.Text buttonLabelText;

        [Header("Audio")]
        public bool playChimeSound = false;

        private Vector3 initialLocalPos;
        private Vector3 initialLocalScale;
        private Vector3 targetLocalPos;
        private Renderer plateRenderer;
        private Color defaultColor = new Color(0.5f, 0.5f, 0.5f);
        private bool isHighlighted = false;
        private bool isInitialized = false;
        private float flashTimer = 0f;

        private bool isNeonActive = false;
        private bool isNeonPulsing = false;
        private Color neonTargetColor = new Color(0.15f, 0.92f, 1.0f);

        private void OnEnable()
        {
            if (!allButtons.Contains(this)) allButtons.Add(this);
        }

        private void OnDisable()
        {
            allButtons.Remove(this);
        }

        private void EnsureInitialized()
        {
            if (isInitialized) return;
            isInitialized = true;

            if (visualPlate == null) visualPlate = transform;
            initialLocalPos = visualPlate.localPosition;
            initialLocalScale = visualPlate.localScale;
            targetLocalPos = initialLocalPos;

            plateRenderer = visualPlate.GetComponent<Renderer>();
            if (plateRenderer != null && plateRenderer.sharedMaterial != null)
            {
                defaultColor = plateRenderer.sharedMaterial.color;
            }
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void Start()
        {
            EnsureInitialized();

            // Asegurar que el collider sea Trigger
            Collider col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;

            // Asegurar un Rigidbody cinemático para colisiones de manos
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            ApplyVisualColor();
            UpdateNeonVisuals(0f);
        }

        private void Update()
        {
            if (visualPlate != null)
            {
                visualPlate.localPosition = Vector3.Lerp(visualPlate.localPosition, targetLocalPos, Time.deltaTime * returnSpeed);
                if (Vector3.Distance(visualPlate.localPosition, targetLocalPos) < 0.002f && targetLocalPos != initialLocalPos)
                {
                    targetLocalPos = initialLocalPos;
                }
            }

            if (flashTimer > 0f)
            {
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0f)
                {
                    ApplyVisualColor();
                }
            }

            if (isNeonActive && isNeonPulsing)
            {
                float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 3.6f);
                UpdateNeonVisuals(pulse);
            }

            // Pulsación por yema del dedo índice (o punta del mando) dentro del volumen frontal del botón
            CheckPokeInput();
        }

        [Header("Zona táctil (se rellena al construir la escena)")]
        public Vector2 touchHalfSize = new Vector2(0.05f, 0.03f);
        public bool isCircular = false;

        private bool wasPokedInside = false;
        private float prevLocalZ = -999f;
        private static Transform leftControllerAnchor;
        private static Transform rightControllerAnchor;

        /// <summary>
        /// Devuelve true si algún botón activo está a menos de 'radius' metros del punto (evita agarrar herramientas al pulsar).
        /// </summary>
        public static bool IsNearAnyButton(Vector3 worldPos, float radius)
        {
            float r2 = radius * radius;
            for (int i = 0; i < allButtons.Count; i++)
            {
                ZenTouchButton b = allButtons[i];
                if (b == null || !b.isActiveAndEnabled) continue;
                if ((b.transform.position - worldPos).sqrMagnitude < r2) return true;
            }
            return false;
        }

        private static void FindControllerAnchors()
        {
            if (leftControllerAnchor == null)
            {
                var l = GameObject.Find("LeftControllerAnchor");
                if (l == null) l = GameObject.Find("LeftHandAnchor");
                if (l != null) leftControllerAnchor = l.transform;
            }
            if (rightControllerAnchor == null)
            {
                var r = GameObject.Find("RightControllerAnchor");
                if (r == null) r = GameObject.Find("RightHandAnchor");
                if (r != null) rightControllerAnchor = r.transform;
            }
        }

        private bool IsPointInPressZone(Vector3 worldPoint, out bool isCrossing)
        {
            isCrossing = false;
            Vector3 p = transform.InverseTransformPoint(worldPoint);

            // Verificación en plano frontal X/Y (con margen cómodo para las yemas)
            if (isCircular)
            {
                float r2 = p.x * p.x + p.y * p.y;
                float maxR = touchHalfSize.x * 1.08f;
                if (r2 > maxR * maxR) return false;
            }
            else
            {
                if (Mathf.Abs(p.x) > touchHalfSize.x * 1.08f || Mathf.Abs(p.y) > touchHalfSize.y * 1.08f)
                    return false;
            }

            // Detección de cruce de trayectoria hacia la placa del botón (z = -0.008m)
            if (prevLocalZ > -900f && prevLocalZ < -0.006f && p.z >= -0.006f)
            {
                isCrossing = true;
            }
            prevLocalZ = p.z;

            // Zona de profundidad amplia y tolerante (entre 4.5 cm delante y 3.8 cm detrás de la cara frontal)
            if (p.z > 0.038f || p.z < -0.048f) return false;
            return true;
        }

        private void CheckPokeInput()
        {
            if (Time.timeSinceLevelLoad < 0.6f) return;

            bool inside = false;
            bool crossed = false;

            // 1. Yemas de los dedos índice, pulgar y corazón (Hand Tracking)
            IList<Vector3> tips = ZenFingerTips.GetIndexTips();
            for (int i = 0; i < tips.Count; i++)
            {
                if (IsPointInPressZone(tips[i], out bool cross))
                {
                    inside = true;
                    if (cross) crossed = true;
                    break;
                }
            }

            // 2. Punta de los mandos Touch (toque físico) y puntero + gatillo
            var activeCtrl = OVRInput.GetActiveController();
            bool usingTouch = (activeCtrl == OVRInput.Controller.Touch ||
                               activeCtrl == OVRInput.Controller.LTouch ||
                               activeCtrl == OVRInput.Controller.RTouch);
            if (usingTouch)
            {
                FindControllerAnchors();
                Transform[] ctrls = { leftControllerAnchor, rightControllerAnchor };
                OVRInput.Button[] triggers = { OVRInput.Button.PrimaryIndexTrigger, OVRInput.Button.SecondaryIndexTrigger };
                for (int c = 0; c < 2; c++)
                {
                    Transform ctrl = ctrls[c];
                    if (ctrl == null) continue;

                    if (IsPointInPressZone(ctrl.position + ctrl.forward * 0.05f, out bool ctrlCross))
                    {
                        inside = true;
                        if (ctrlCross) crossed = true;
                    }

                    if (OVRInput.GetDown(triggers[c]) && Time.time >= GlobalPressCooldownUntil)
                    {
                        Vector3 toButton = transform.position - ctrl.position;
                        float dist = toButton.magnitude;
                        if (dist < 1.2f && Vector3.Dot(ctrl.forward, toButton / Mathf.Max(dist, 0.001f)) > 0.990f)
                        {
                            Press();
                            return;
                        }
                    }
                }
            }

            // Registro instantáneo y fiable: pulsa al entrar en la zona o al cruzar la placa frontal
            if (inside || crossed)
            {
                if (!wasPokedInside)
                {
                    if (Time.time >= GlobalPressCooldownUntil)
                    {
                        wasPokedInside = true;
                        Press();
                    }
                    // Si estaba en cooldown durante este frame, NO marcamos wasPokedInside=true
                    // para procesar el toque inmediatamente al instante siguiente
                }
            }
            else
            {
                wasPokedInside = false;
                prevLocalZ = -999f;
            }
        }

        /// <summary>
        /// Enciende o apaga el marco de neón luminoso alrededor del botón cuando el árbol necesita agua, poda o limpieza.
        /// </summary>
        public void SetNeonAlert(bool active, Color color, bool pulse = true)
        {
            bool changed = (isNeonActive != active) || (isNeonPulsing != pulse) || (Vector4.Distance(neonTargetColor, color) > 0.02f);
            isNeonActive = active;
            neonTargetColor = color;
            isNeonPulsing = pulse;

            if (changed)
            {
                UpdateNeonVisuals(1f);
            }
        }

        public void SetLabelText(string text)
        {
            if (buttonLabelText != null && buttonLabelText.text != text)
            {
                buttonLabelText.text = text;
            }
        }

        private void UpdateNeonVisuals(float pulseFactor)
        {
            if (!Application.isPlaying) return;
            if (neonBorderRenderers == null || neonBorderRenderers.Length == 0) return;

            for (int i = 0; i < neonBorderRenderers.Length; i++)
            {
                Renderer r = neonBorderRenderers[i];
                if (r == null) continue;
                r.gameObject.SetActive(isNeonActive);
                if (!isNeonActive) continue;

                Material m = r.material;
                if (m == null) continue;

                float intensity = isNeonPulsing ? Mathf.Lerp(0.55f, 1.35f, pulseFactor) : 0.85f;
                Color baseCol = Color.Lerp(neonTargetColor, Color.white, 0.35f * pulseFactor);
                Color emisCol = neonTargetColor * (1.85f * intensity);

                m.color = baseCol;
                if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", baseCol);
                m.EnableKeyword("_EMISSION");
                if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", emisCol);
            }
        }

        [Tooltip("Método antiguo por colliders de falange/palma (impreciso). Desactivado: se usa la yema del índice.")]
        public bool useLegacyColliderTouch = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!useLegacyColliderTouch) return;

            // Ignorar durante los primeros 0.6 segundos tras cargar la escena
            if (Time.timeSinceLevelLoad < 0.6f) return;

            // Cooldown global: si ya se ha pulsado un botón hace menos de 0.45s, ignorar para evitar tocar dos botones a la vez
            if (Time.time < GlobalPressCooldownUntil) return;

            // Ignorar colisiones internas de la propia escena o herramientas
            if (other.GetComponentInParent<OrganicBonsaiTree>() != null) return;
            if (other.GetComponentInParent<ZenBonsaiTool>() != null) return;
            if (other.GetComponentInParent<ZenTouchButton>() != null) return;

            string n = other.name.ToLower();
            if (n.Contains("plate") || n.Contains("table") || n.Contains("soil") || n.Contains("pot") || n.Contains("bowl") || n.Contains("panel")) return;

            // Si son cápsulas individuales de los dedos bajos, ignorarlas (dejar solo índice o collider principal de mano)
            if (n.Contains("thumb") || n.Contains("middle") || n.Contains("ring") || n.Contains("pinky") || n.Contains("little") || n.Contains("wrist"))
            {
                return;
            }

            // Punto exacto de contacto relativo a este botón
            Vector3 contactWorld = other.ClosestPoint(transform.position);
            Vector3 localContact = transform.InverseTransformPoint(contactWorld);

            // Solo activar si el contacto penetra por el frontal del botón (evita tocar accidentalmente botones contiguos)
            if (localContact.z > 0.040f || localContact.z < -0.045f) return;
            if (Mathf.Abs(localContact.x) > 0.12f || Mathf.Abs(localContact.y) > 0.055f) return;

            Press();
        }

        /// <summary>
        /// Activa la pulsación del botón con sonido, destello luminoso, animación y bloqueo de rebote
        /// </summary>
        public void Press()
        {
            EnsureInitialized();
            if (Time.time < GlobalPressCooldownUntil) return;
            GlobalPressCooldownUntil = Time.time + 0.28f; // Cooldown ágil de 280 ms para evitar rebotes pero responder inmediatamente

            // Efecto visual de hundimiento y destello luminoso inmediato
            targetLocalPos = initialLocalPos + pressDisplacement;
            flashTimer = 0.22f;
            if (Application.isPlaying && plateRenderer != null && plateRenderer.material != null)
            {
                Color brightFlash = Color.Lerp(defaultColor, Color.white, 0.65f);
                plateRenderer.material.color = brightFlash;
                if (plateRenderer.material.HasProperty("_BaseColor"))
                    plateRenderer.material.SetColor("_BaseColor", brightFlash);
                plateRenderer.material.EnableKeyword("_EMISSION");
                if (plateRenderer.material.HasProperty("_EmissionColor"))
                    plateRenderer.material.SetColor("_EmissionColor", brightFlash * 0.75f);
            }

            // Sonido táctil zen
            if (ZenSoundSynthesizer.Instance != null)
            {
                if (playChimeSound)
                {
                    ZenSoundSynthesizer.Instance.PlayTibetanBell(580f, 1.2f, 0.6f);
                }
                else
                {
                    ZenSoundSynthesizer.Instance.PlayWoodClack(0.7f);
                }
            }

            onClick?.Invoke();
        }

        public void SetSelectedHighlight(bool selected)
        {
            EnsureInitialized();
            isHighlighted = selected;
            ApplyVisualColor();
        }

        private void ApplyVisualColor()
        {
            // En Edit Mode (o durante OnPreprocessBuild) NUNCA llamar a renderer.material
            // porque Unity lanza el error: "Instantiating material due to calling renderer.material during edit mode"
            if (!Application.isPlaying) return;
            if (plateRenderer == null || plateRenderer.material == null) return;

            Material mat = plateRenderer.material;
            if (isHighlighted)
            {
                // Botón activo: iluminado con brillo cálido e intenso y ligeramente elevado
                Color litColor = Color.Lerp(defaultColor, Color.white, 0.38f);
                Color emission = defaultColor * 0.65f + new Color(0.18f, 0.16f, 0.12f);
                mat.color = litColor;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", litColor);
                mat.EnableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emission);

                if (visualPlate != null)
                {
                    visualPlate.localScale = new Vector3(initialLocalScale.x * 1.06f, initialLocalScale.y * 1.06f, initialLocalScale.z * 1.22f);
                }
            }
            else
            {
                // Botón en reposo: tono mate más apagado para que destaque claramente el botón iluminado
                Color dimColor = defaultColor * 0.72f;
                dimColor.a = 1f;
                mat.color = dimColor;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", dimColor);
                if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", Color.black);

                if (visualPlate != null)
                {
                    visualPlate.localScale = initialLocalScale;
                }
            }
        }
    }

    /// <summary>
    /// Posición real de las yemas de los dedos (hand tracking de Quest).
    /// Se usa para pulsar botones con precisión de milímetros y para ubicar herramientas en las yemas.
    /// </summary>
    public static class ZenFingerTips
    {
        private static readonly List<Vector3> tips = new List<Vector3>(8);
        private static readonly Dictionary<OVRSkeleton, (Transform index, Transform thumb, Transform middle)> skeletonTips =
            new Dictionary<OVRSkeleton, (Transform index, Transform thumb, Transform middle)>();
        private static OVRSkeleton[] skeletons = new OVRSkeleton[0];
        private static float nextScanTime = 0f;
        private static int lastFrame = -1;
        private static Transform leftAnchor;
        private static Transform rightAnchor;

        private static void UpdateSkeletonsIfNeeded()
        {
            if (Time.unscaledTime >= nextScanTime)
            {
                nextScanTime = Time.unscaledTime + 1.2f;
                skeletons = UnityEngine.Object.FindObjectsByType<OVRSkeleton>(FindObjectsInactive.Exclude);
            }
        }

        public static bool TryGetPinchPoint(bool isLeft, out Vector3 pinchPoint)
        {
            pinchPoint = Vector3.zero;
            UpdateSkeletonsIfNeeded();

            for (int i = 0; i < skeletons.Length; i++)
            {
                OVRSkeleton sk = skeletons[i];
                if (sk == null || !sk.isActiveAndEnabled || !sk.IsInitialized || !sk.IsDataValid) continue;
                bool skIsLeft = (sk.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft);
                if (skIsLeft != isLeft) continue;

                if (!skeletonTips.TryGetValue(sk, out var boneGroup) || boneGroup.index == null || boneGroup.thumb == null)
                {
                    Transform idx = null, thm = null, mid = null;
                    var bones = sk.Bones;
                    if (bones != null)
                    {
                        for (int b = 0; b < bones.Count; b++)
                        {
                            if (bones[b].Id == OVRSkeleton.BoneId.Hand_IndexTip) idx = bones[b].Transform;
                            else if (bones[b].Id == OVRSkeleton.BoneId.Hand_ThumbTip) thm = bones[b].Transform;
                            else if (bones[b].Id == OVRSkeleton.BoneId.Hand_MiddleTip) mid = bones[b].Transform;
                        }
                    }
                    boneGroup = (idx, thm, mid);
                    skeletonTips[sk] = boneGroup;
                }

                if (boneGroup.thumb != null && boneGroup.index != null)
                {
                    // Comprobar que no devuelvan la posición de la muñeca
                    if ((boneGroup.index.position - sk.transform.position).sqrMagnitude > 0.05f * 0.05f)
                    {
                        pinchPoint = (boneGroup.thumb.position + boneGroup.index.position) * 0.5f;
                        return true;
                    }
                }
            }
            return false;
        }

        public static IList<Vector3> GetIndexTips()
        {
            if (lastFrame == Time.frameCount) return tips;
            lastFrame = Time.frameCount;
            tips.Clear();

            UpdateSkeletonsIfNeeded();

            for (int i = 0; i < skeletons.Length; i++)
            {
                OVRSkeleton sk = skeletons[i];
                if (sk == null || !sk.isActiveAndEnabled || !sk.IsInitialized || !sk.IsDataValid) continue;

                if (!skeletonTips.TryGetValue(sk, out var boneGroup) || boneGroup.index == null)
                {
                    Transform idx = null, thm = null, mid = null;
                    var bones = sk.Bones;
                    if (bones != null)
                    {
                        for (int b = 0; b < bones.Count; b++)
                        {
                            if (bones[b].Id == OVRSkeleton.BoneId.Hand_IndexTip) idx = bones[b].Transform;
                            else if (bones[b].Id == OVRSkeleton.BoneId.Hand_ThumbTip) thm = bones[b].Transform;
                            else if (bones[b].Id == OVRSkeleton.BoneId.Hand_MiddleTip) mid = bones[b].Transform;
                        }
                    }
                    boneGroup = (idx, thm, mid);
                    skeletonTips[sk] = boneGroup;
                }

                Vector3 wristPos = sk.transform.position;

                if (boneGroup.index != null && (boneGroup.index.position - wristPos).sqrMagnitude > 0.05f * 0.05f)
                {
                    tips.Add(boneGroup.index.position);
                }
                if (boneGroup.middle != null && (boneGroup.middle.position - wristPos).sqrMagnitude > 0.05f * 0.05f)
                {
                    tips.Add(boneGroup.middle.position);
                }
                if (boneGroup.thumb != null && (boneGroup.thumb.position - wristPos).sqrMagnitude > 0.05f * 0.05f)
                {
                    tips.Add(boneGroup.thumb.position);
                }
            }

            if (tips.Count == 0)
            {
                // Alternativa: estimación ergonómica desde los anclajes de mano
                if (leftAnchor == null) { var l = GameObject.Find("LeftHandAnchor"); if (l != null) leftAnchor = l.transform; }
                if (rightAnchor == null) { var r = GameObject.Find("RightHandAnchor"); if (r != null) rightAnchor = r.transform; }

                if (leftAnchor != null && IsHandTracked(OVRPlugin.Hand.HandLeft))
                {
                    tips.Add(leftAnchor.TransformPoint(new Vector3(0.012f, -0.025f, 0.135f)));
                }
                if (rightAnchor != null && IsHandTracked(OVRPlugin.Hand.HandRight))
                {
                    tips.Add(rightAnchor.TransformPoint(new Vector3(-0.012f, -0.025f, 0.135f)));
                }
            }

            return tips;
        }

        private static bool IsHandTracked(OVRPlugin.Hand hand)
        {
            OVRPlugin.HandState state = new OVRPlugin.HandState();
            if (OVRPlugin.GetHandState(OVRPlugin.Step.Render, hand, ref state))
            {
                return (state.Status & OVRPlugin.HandStatus.HandTracked) != 0;
            }
            return false;
        }
    }
}

