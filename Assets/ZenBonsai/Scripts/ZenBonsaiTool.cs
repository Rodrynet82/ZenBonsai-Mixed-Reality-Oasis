using System.Collections.Generic;
using UnityEngine;

namespace ZenBonsai
{
    public enum BonsaiToolType
    {
        Shears,         // Tijeras tradicionales de poda (Satsuki Hasami)
        WateringCan,    // Regadera tradicional japonesa de cobre (Joro)
        Broom,          // Pincel / Escobilla de bambú y cerdas naturales (Hoki)
        WaterLadle      // Cazo de bambú tradicional de la fuente Tsukubai (Hishaku)
    }

    /// <summary>
    /// Herramienta interactiva de maestro bonsái (Tijeras, Regadera, Escobilla, Cazo Hishaku).
    /// Selección inteligente de la herramienta más cercana a los dedos reales (OVRSkeleton),
    /// agarre en las yemas de los dedos (nunca en la muñeca) y retorno automático limpio a su soporte.
    /// </summary>
    public class ZenBonsaiTool : MonoBehaviour
    {
        // Registro global de todas las herramientas para elegir SIEMPRE la más cercana a los dedos
        private static readonly List<ZenBonsaiTool> allTools = new List<ZenBonsaiTool>();

        // Control global de herramientas para que NUNCA se junten dos en la misma mano
        private static ZenBonsaiTool leftHandHeldTool = null;
        private static ZenBonsaiTool rightHandHeldTool = null;

        public static bool IsAnyToolHeld => (leftHandHeldTool != null || rightHandHeldTool != null);

        public static bool IsHandNearAnyTool(Vector3 worldPos, float radius = 0.22f)
        {
            for (int i = 0; i < allTools.Count; i++)
            {
                var t = allTools[i];
                if (t == null) continue;
                if (Vector3.Distance(t.transform.position, worldPos) < radius) return true;
            }
            return false;
        }

        [Header("Tipo de Herramienta")]
        public BonsaiToolType toolType;

        [Header("Referencias de la Herramienta")]
        public Transform handleAnchor;          // Punto donde los dedos agarran la herramienta
        public Transform actionPoint;           // Punto de acción (puntas de tijera, cuenco del cazo, cerdas pincel)
        public Transform movingPartA;           // Hoja A de tijera
        public Transform movingPartB;           // Hoja B de tijera

        [Header("Estado y Retorno")]
        public Transform traySlot;              // Ranura original en la bandeja o fuente
        public float returnSpeed = 14.0f;
        public float grabDistance = 0.16f;      // Radio de alcance cómodo, ganando siempre la más cercana

        private Transform holdingHand = null;
        private bool isHoldingLeft = false;
        private bool isHeld = false;
        private bool isReturning = false;

        private OVRPlugin.HandState leftHandState = new OVRPlugin.HandState();
        private OVRPlugin.HandState rightHandState = new OVRPlugin.HandState();

        private Transform leftHandAnchor = null;
        private Transform rightHandAnchor = null;
        private Transform centerEye = null;

        private Vector3 initialLocalScale = Vector3.one;
        private float lastActionTime = 0f;
        private float pourSoundTimer = 0f;
        private float releaseCooldownUntil = 0f;
        private float untrackedHoldTimer = 0f;

        private void OnEnable()
        {
            if (!allTools.Contains(this)) allTools.Add(this);
        }

        private void OnDisable()
        {
            allTools.Remove(this);
            if (leftHandHeldTool == this) leftHandHeldTool = null;
            if (rightHandHeldTool == this) rightHandHeldTool = null;
        }

        private void Start()
        {
            initialLocalScale = transform.localScale;
            if (handleAnchor == null) handleAnchor = transform;
            if (actionPoint == null) actionPoint = transform;
            FindHandAnchors();
        }

        private void FindHandAnchors()
        {
            if (centerEye == null)
            {
                GameObject eye = GameObject.Find("CenterEyeAnchor");
                if (eye != null) centerEye = eye.transform;
                else if (Camera.main != null) centerEye = Camera.main.transform;
            }
            if (leftHandAnchor == null)
            {
                GameObject l = GameObject.Find("LeftHandAnchor");
                if (l != null) leftHandAnchor = l.transform;
            }
            if (rightHandAnchor == null)
            {
                GameObject r = GameObject.Find("RightHandAnchor");
                if (r != null) rightHandAnchor = r.transform;
            }
        }

        private void Update()
        {
            if (isHeld)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, initialLocalScale, Time.deltaTime * 16f);
                UpdateHeldState();
            }
            else
            {
                UpdateReturnToTray();
                if (Time.time > releaseCooldownUntil && !isReturning)
                {
                    CheckHandGrabsAndHover();
                }
                else
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, initialLocalScale, Time.deltaTime * 16f);
                }
            }
        }

        private void UpdateHeldState()
        {
            if (holdingHand == null)
            {
                Release();
                return;
            }

            // Comprobar si el usuario abrió el pellizco (índice + pulgar), soltó el mando o perdió tracking
            if (!IsHandPinchingOrGripping(isHoldingLeft, true))
            {
                Release();
                return;
            }

            var activeCtrl = OVRInput.GetActiveController();
            bool usingTouch = (activeCtrl == OVRInput.Controller.Touch ||
                               activeCtrl == OVRInput.Controller.LTouch ||
                               activeCtrl == OVRInput.Controller.RTouch);

            Vector3 targetGripPos;
            Quaternion targetGripRot;

            if (usingTouch)
            {
                // Agarre ergonómico en mandos Meta Quest Touch (alineado con la empuñadura física)
                targetGripPos = holdingHand.position + holdingHand.forward * 0.045f - holdingHand.up * 0.02f;
                targetGripRot = holdingHand.rotation * Quaternion.Euler(22f, 0f, 0f);
            }
            else
            {
                // Agarre en las yemas de los dedos (pinch entre índice y pulgar)
                Vector3 fingerOffset = isHoldingLeft
                    ? new Vector3(-0.015f, -0.015f, 0.125f)
                    : new Vector3(0.015f, -0.015f, 0.125f);
                targetGripPos = holdingHand.TransformPoint(fingerOffset);
                targetGripRot = holdingHand.rotation * Quaternion.Euler(14f, isHoldingLeft ? 8f : -8f, 0f);
            }

            // CRÍTICO: Posicionar de modo que el HANDLE_ANCHOR encaje exactamente en la mano/dedos
            // (evita que la herramienta quede retrasada pegada a la muñeca)
            Vector3 handleOffset = handleAnchor != null ? (transform.position - handleAnchor.position) : Vector3.zero;
            Vector3 finalTargetPos = targetGripPos + handleOffset;

            transform.position = Vector3.Lerp(transform.position, finalTargetPos, Time.deltaTime * 32f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetGripRot, Time.deltaTime * 28f);

            // Comportamiento funcional según herramienta
            switch (toolType)
            {
                case BonsaiToolType.Shears:
                    HandleShearsBehavior();
                    break;

                case BonsaiToolType.WateringCan:
                case BonsaiToolType.WaterLadle:
                    HandleWateringCanBehavior();
                    break;

                case BonsaiToolType.Broom:
                    HandleBroomBehavior();
                    break;
            }
        }

        private void UpdateReturnToTray()
        {
            if (traySlot != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, traySlot.position, Time.deltaTime * returnSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, traySlot.rotation, Time.deltaTime * 18f);

                if (Vector3.Distance(transform.position, traySlot.position) < 0.015f)
                {
                    transform.position = traySlot.position;
                    transform.rotation = traySlot.rotation;
                    isReturning = false;
                }
            }
        }

        #region Detección de Agarre Inteligente en los Dedos (Nunca en la Muñeca)

        /// <summary>
        /// Calcula el punto real de pellizco en las yemas de los dedos o mandos Touch.
        /// </summary>
        private Vector3 GetTrueHandPoint(Transform handAnchor, bool isLeft)
        {
            if (handAnchor == null) return Vector3.zero;
            var activeCtrl = OVRInput.GetActiveController();
            bool usingTouch = (activeCtrl == OVRInput.Controller.Touch ||
                               activeCtrl == OVRInput.Controller.LTouch ||
                               activeCtrl == OVRInput.Controller.RTouch);
            if (usingTouch)
            {
                return handAnchor.position + handAnchor.forward * 0.045f - handAnchor.up * 0.02f;
            }

            Vector3 fingerOffset = isLeft
                ? new Vector3(-0.015f, -0.015f, 0.120f)
                : new Vector3(0.015f, -0.015f, 0.120f);
            return handAnchor.TransformPoint(fingerOffset);
        }

        private float GetDistanceToPoint(Vector3 point)
        {
            float centerDist = Vector3.Distance(transform.position, point);
            float handleDist = handleAnchor != null ? Vector3.Distance(handleAnchor.position, point) : centerDist;
            float bestAnchorDist = Mathf.Min(centerDist, handleDist);

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                float surfaceDist = Vector3.Distance(col.ClosestPoint(point), point);
                return Mathf.Lerp(surfaceDist, bestAnchorDist, 0.45f);
            }
            return bestAnchorDist;
        }

        /// <summary>
        /// Comprueba si ESTA herramienta es estrictamente la más cercana a los dedos entre todas las disponibles.
        /// </summary>
        private bool IsClosestAvailableTool(Vector3 handPoint, out float myDist)
        {
            myDist = GetDistanceToPoint(handPoint);
            if (myDist > grabDistance) return false;

            // 1. Si los dedos están justo encima de un botón (< 6 cm), no coger herramientas (evita agarres al pulsar)
            if (ZenTouchButton.IsNearAnyButton(handPoint, 0.06f)) return false;

            // 2. Si se ha tocado o se está tocando un botón recientemente, bloquear agarres de herramientas
            if (Time.time < ZenTouchButton.GlobalPressCooldownUntil + 0.45f) return false;

            for (int i = 0; i < allTools.Count; i++)
            {
                ZenBonsaiTool other = allTools[i];
                if (other == null || other == this || other.isHeld || other.isReturning) continue;

                float otherDist = other.GetDistanceToPoint(handPoint);
                if (otherDist < myDist - 0.002f)
                {
                    return false; // Otra herramienta está más cerca de los dedos
                }
            }
            return true;
        }

        private void CheckHandGrabsAndHover()
        {
            if (leftHandAnchor == null || rightHandAnchor == null)
            {
                FindHandAnchors();
            }

            bool isHoveredByAnyHand = false;

            // Mano Derecha: solo si la mano derecha está libre
            if (rightHandAnchor != null && rightHandHeldTool == null)
            {
                Vector3 handPointR = GetTrueHandPoint(rightHandAnchor, false);
                if (IsClosestAvailableTool(handPointR, out float distR))
                {
                    isHoveredByAnyHand = true;
                    if (IsHandPinchingOrGripping(false, false))
                    {
                        Grab(rightHandAnchor, false);
                        return;
                    }
                }
            }

            // Mano Izquierda: solo si la mano izquierda está libre
            if (leftHandAnchor != null && leftHandHeldTool == null)
            {
                Vector3 handPointL = GetTrueHandPoint(leftHandAnchor, true);
                if (IsClosestAvailableTool(handPointL, out float distL))
                {
                    isHoveredByAnyHand = true;
                    if (IsHandPinchingOrGripping(true, false))
                    {
                        Grab(leftHandAnchor, true);
                        return;
                    }
                }
            }

            // Realce visual sutil (1.16x) cuando los dedos apuntan a esta herramienta
            Vector3 targetScale = isHoveredByAnyHand ? initialLocalScale * 1.16f : initialLocalScale;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 14f);
        }

        public void Grab(Transform hand, bool isLeft)
        {
            // Liberar cualquier herramienta previa en esa mano por seguridad absoluta
            if (isLeft && leftHandHeldTool != null && leftHandHeldTool != this)
            {
                leftHandHeldTool.Release();
            }
            if (!isLeft && rightHandHeldTool != null && rightHandHeldTool != this)
            {
                rightHandHeldTool.Release();
            }

            isHeld = true;
            isReturning = false;
            holdingHand = hand;
            isHoldingLeft = isLeft;
            untrackedHoldTimer = 0f;
            transform.localScale = initialLocalScale;

            if (isLeft)
            {
                leftHandHeldTool = this;
            }
            else
            {
                rightHandHeldTool = this;
            }

            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlayWoodClack(0.55f);
            }
        }

        public void Release()
        {
            if (isHoldingLeft && leftHandHeldTool == this) leftHandHeldTool = null;
            if (!isHoldingLeft && rightHandHeldTool == this) rightHandHeldTool = null;

            isHeld = false;
            holdingHand = null;
            untrackedHoldTimer = 0f;
            isReturning = true; // Vuela directamente de regreso a su soporte sin quedarse pegada
            transform.localScale = initialLocalScale;
            releaseCooldownUntil = Time.time + 0.45f;
        }

        private bool IsHandPinchingOrGripping(bool isLeft, bool isCurrentlyHeld)
        {
            var activeCtrl = OVRInput.GetActiveController();
            bool usingTouchControllers = (activeCtrl == OVRInput.Controller.Touch ||
                                          activeCtrl == OVRInput.Controller.LTouch ||
                                          activeCtrl == OVRInput.Controller.RTouch);

            // 1. Mandos VR físicos (Meta Quest Touch Controllers)
            if (usingTouchControllers)
            {
                untrackedHoldTimer = 0f;
                if (isLeft)
                {
                    return OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
                }
                else
                {
                    return OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
                }
            }

            // 2. Hand Tracking nativo de Quest (EXCLUSIVAMENTE pellizco deliberado de dedo ÍNDICE + PULGAR)
            OVRPlugin.Hand hand = isLeft ? OVRPlugin.Hand.HandLeft : OVRPlugin.Hand.HandRight;
            if (OVRPlugin.GetHandState(OVRPlugin.Step.Render, hand, ref (isLeft ? ref leftHandState : ref rightHandState)))
            {
                var state = isLeft ? leftHandState : rightHandState;
                if ((state.Status & OVRPlugin.HandStatus.HandTracked) != 0)
                {
                    untrackedHoldTimer = 0f;

                    float indexStrength = 0f;
                    if (state.PinchStrength != null && state.PinchStrength.Length > (int)OVRPlugin.HandFinger.Index)
                    {
                        indexStrength = state.PinchStrength[(int)OVRPlugin.HandFinger.Index];
                    }

                    // Umbral exigente: > 0.72f para agarrar con clara intención (evita que manos semi-cerradas o apuntando cojan cosas)
                    // Soltar inmediatamente cuando el índice se abre (< 0.42f)
                    float threshold = isCurrentlyHeld ? 0.40f : 0.62f;
                    return (indexStrength > threshold);
                }
            }

            if (isCurrentlyHeld)
            {
                untrackedHoldTimer += Time.deltaTime;
                return untrackedHoldTimer < 0.12f;
            }
            return false;
        }

        #endregion

        #region Comportamiento Funcional de Herramientas

        private void HandleShearsBehavior()
        {
            Vector3 cutPoint = actionPoint != null ? actionPoint.position : transform.position;

            Collider[] hits = Physics.OverlapSphere(cutPoint, 0.085f);
            foreach (var hit in hits)
            {
                PrunableBranch branch = hit.GetComponentInParent<PrunableBranch>();
                if (branch != null && branch.isWithered)
                {
                    if (Time.time - lastActionTime > 0.4f)
                    {
                        lastActionTime = Time.time;
                        StartCoroutine(AnimateShearsCut());
                        branch.Prune();
                        break;
                    }
                }
            }
        }

        private System.Collections.IEnumerator AnimateShearsCut()
        {
            float elapsed = 0f;
            float duration = 0.15f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float angle = Mathf.Sin((elapsed / duration) * Mathf.PI) * 18f;
                if (movingPartA != null) movingPartA.localRotation = Quaternion.Euler(0f, angle, 0f);
                if (movingPartB != null) movingPartB.localRotation = Quaternion.Euler(0f, -angle, 0f);
                yield return null;
            }
            if (movingPartA != null) movingPartA.localRotation = Quaternion.identity;
            if (movingPartB != null) movingPartB.localRotation = Quaternion.identity;
        }

        private bool IsLadleCupOverPot()
        {
            if (ZenBonsaiManager.Instance == null || ZenBonsaiManager.Instance.trunkBase == null) return false;

            Vector3 cupPos = actionPoint != null ? actionPoint.position : transform.position;
            Vector3 potPos = ZenBonsaiManager.Instance.trunkBase.position;

            float horizDist = Vector2.Distance(new Vector2(cupPos.x, cupPos.z), new Vector2(potPos.x, potPos.z));
            float heightAbovePot = cupPos.y - potPos.y;

            return horizDist < 0.20f && heightAbovePot > 0.01f && heightAbovePot < 0.42f;
        }

        private void HandleWateringCanBehavior()
        {
            // Solo verter agua cuando:
            // 1) El cuenco del cazo (actionPoint) está situado sobre la maceta del bonsái
            // 2) Y el cazo está INCLINADO hacia abajo o girado (vaciando agua deliberadamente)
            if (!IsLadleCupOverPot()) return;

            bool isTilted = (transform.forward.y < -0.15f || Vector3.Dot(transform.up, Vector3.down) > 0.12f);
            if (!isTilted) return;

            Vector3 cupSpoutPos = actionPoint != null ? actionPoint.position : transform.position;
            Vector3 targetSoilPos = ZenBonsaiManager.Instance.trunkBase.position + Vector3.down * 0.02f;

            if (ZenVisualEffects.Instance != null)
            {
                ZenVisualEffects.Instance.SpawnWaterStream(cupSpoutPos, targetSoilPos);
            }

            pourSoundTimer -= Time.deltaTime;
            if (pourSoundTimer <= 0f)
            {
                pourSoundTimer = 0.55f;
                if (ZenSoundSynthesizer.Instance != null)
                {
                    ZenSoundSynthesizer.Instance.PlayWateringStream(0.48f);
                }
            }

            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.WaterTree(Time.deltaTime * 24f);
            }
        }

        private void HandleBroomBehavior()
        {
            Vector3 sweepPos = actionPoint != null ? actionPoint.position : transform.position;

            Collider[] hits = Physics.OverlapSphere(sweepPos, 0.09f);
            foreach (var hit in hits)
            {
                ZenPest pest = hit.GetComponentInParent<ZenPest>();
                if (pest != null)
                {
                    pest.Dispel();
                    if (ZenSoundSynthesizer.Instance != null)
                    {
                        ZenSoundSynthesizer.Instance.PlayBroomSweep(0.7f);
                    }
                    if (ZenVisualEffects.Instance != null)
                    {
                        ZenVisualEffects.Instance.SpawnBroomDust(sweepPos);
                    }
                    break;
                }
            }
        }

        #endregion
    }
}
