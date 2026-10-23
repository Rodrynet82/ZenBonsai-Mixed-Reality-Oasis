using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Manipulador espacial nativo y ultra-fluido de Meta Quest 3 para Zen Bonsai.
    /// Mantiene siempre el tamaño original 1:1 del bonsái y elimina cualquier salto de tracking óptico.
    /// - Modo 2 Manos (Ambos pellizcos activos):
    ///     * Rotación 360° suave (Giro).
    ///     * Desplazamiento 3D suave sin saltos (Mover, subir o bajar a la altura deseada).
    /// - Modo 1 Mano: Cuidado normal del bonsái sin mover la mesa.
    /// </summary>
    public class ZenBonsaiManipulator : MonoBehaviour
    {
        [Header("Configuración")]
        public Transform rootToMove;
        public bool allowScaleChange = false; // Mantiene el tamaño original 1:1 del bonsái
        public float minScale = 0.8f;
        public float maxScale = 1.2f;

        private Transform leftAnchor;
        private Transform rightAnchor;

        private OVRPlugin.HandState leftHandState = new OVRPlugin.HandState();
        private OVRPlugin.HandState rightHandState = new OVRPlugin.HandState();

        // Estado y suavizado
        [Header("Bloqueo de Mesa (Opciones)")]
        public bool isTableLocked = false;

        private bool isManipulating = false;
        public bool IsManipulating => isManipulating;
        public bool HasUserMovedTable { get; private set; } = false;

        private bool wasLeftPinching = false;
        private bool wasRightPinching = false;
        private int leftTrackedFrames = 0;
        private int rightTrackedFrames = 0;
        private float dualPinchHoldTime = 0f;
        private float unpinchTimer = 0f;
        private const float GracePeriod = 0.10f;

        private float initialHandDistance;
        private float initialAngle;
        private Vector3 initialMidpoint;
        private Vector3 prevMidpoint;
        private Vector3 initialRootPos;
        private Quaternion initialRootRot;
        private Vector3 initialRootScale = Vector3.one;

        private Vector3 targetPos;
        private Quaternion targetRot;
        private Vector3 targetScale = Vector3.one;

        private void Start()
        {
            if (rootToMove == null) rootToMove = transform;
            rootToMove.localScale = Vector3.one;
            targetPos = rootToMove.position;
            targetRot = rootToMove.rotation;
            targetScale = Vector3.one;
            FindAnchors();
        }

        private void FindAnchors()
        {
            if (leftAnchor == null)
            {
                GameObject lObj = GameObject.Find("LeftHandAnchor");
                if (lObj != null) leftAnchor = lObj.transform;
            }

            if (rightAnchor == null)
            {
                GameObject rObj = GameObject.Find("RightHandAnchor");
                if (rObj != null) rightAnchor = rObj.transform;
            }
        }

        private void Update()
        {
            if (leftAnchor == null || rightAnchor == null)
            {
                FindAnchors();
            }

            // Si la mesa está bloqueada en Opciones, o el jugador tiene/está cogiendo una herramienta o tocando un botón,
            // JAMÁS mover la mesa accidentalmente.
            if (isTableLocked || ZenBonsaiTool.IsAnyToolHeld || Time.time < ZenTouchButton.GlobalPressCooldownUntil + 0.5f)
            {
                isManipulating = false;
                dualPinchHoldTime = 0f;
                wasLeftPinching = false;
                wasRightPinching = false;
                ApplySmoothPose();
                return;
            }

            Vector3 lPos = leftAnchor != null ? leftAnchor.position : Vector3.zero;
            Vector3 rPos = rightAnchor != null ? rightAnchor.position : Vector3.zero;

            // Si cualquiera de las dos manos está cerca de las herramientas o del cazo de agua, no iniciar movimiento de mesa
            if (!isManipulating && (ZenBonsaiTool.IsHandNearAnyTool(lPos, 0.23f) || ZenBonsaiTool.IsHandNearAnyTool(rPos, 0.23f)))
            {
                dualPinchHoldTime = 0f;
                ApplySmoothPose();
                return;
            }

            bool lPinch = CheckNativePinch(true, wasLeftPinching, out bool lValidTrack);
            bool rPinch = CheckNativePinch(false, wasRightPinching, out bool rValidTrack);

            wasLeftPinching = lPinch;
            wasRightPinching = rPinch;

            bool bothValidPositions = lValidTrack && rValidTrack && lPos.sqrMagnitude > 0.01f && rPos.sqrMagnitude > 0.01f;
            bool bothPinching = lPinch && rPinch && bothValidPositions;

            if (bothPinching)
            {
                unpinchTimer = GracePeriod;

                if (!isManipulating)
                {
                    // Exigir 0.16s continuos de pellizco firme con AMBAS manos para descartar cierres involuntarios
                    dualPinchHoldTime += Time.deltaTime;
                    if (dualPinchHoldTime >= 0.16f)
                    {
                        StartManipulation(lPos, rPos);
                    }
                }
                else
                {
                    UpdateManipulation(lPos, rPos);
                }
            }
            else
            {
                dualPinchHoldTime = 0f;
                if (isManipulating)
                {
                    unpinchTimer -= Time.deltaTime;
                    if (unpinchTimer <= 0f || !bothValidPositions)
                    {
                        isManipulating = false;
                    }
                    else
                    {
                        UpdateManipulation(lPos, rPos);
                    }
                }
            }

            ApplySmoothPose();
        }

        private void ApplySmoothPose()
        {
            if (rootToMove != null)
            {
                if (!allowScaleChange) targetScale = Vector3.one;
                rootToMove.position = Vector3.Lerp(rootToMove.position, targetPos, Time.deltaTime * 18f);
                rootToMove.rotation = Quaternion.Slerp(rootToMove.rotation, targetRot, Time.deltaTime * 18f);
                rootToMove.localScale = Vector3.Lerp(rootToMove.localScale, targetScale, Time.deltaTime * 18f);
            }
        }

        private void StartManipulation(Vector3 lPos, Vector3 rPos)
        {
            isManipulating = true;

            initialHandDistance = Vector3.Distance(lPos, rPos);
            initialMidpoint = (lPos + rPos) * 0.5f;
            prevMidpoint = initialMidpoint;
            initialRootPos = targetPos;
            initialRootRot = targetRot;
            initialRootScale = Vector3.one;

            targetPos = initialRootPos;
            targetRot = initialRootRot;
            targetScale = Vector3.one;

            Vector3 dir = (rPos - lPos);
            dir.y = 0f;
            initialAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlayTibetanBell(580f, 0.25f, 0.25f);
            }
        }

        private void UpdateManipulation(Vector3 lPos, Vector3 rPos)
        {
            Vector3 currentMidpoint = (lPos + rPos) * 0.5f;

            Vector3 frameJump = currentMidpoint - prevMidpoint;
            if (frameJump.magnitude > 0.05f)
            {
                initialMidpoint += frameJump;
            }
            prevMidpoint = currentMidpoint;

            if (allowScaleChange && initialHandDistance > 0.06f)
            {
                float currentDist = Vector3.Distance(lPos, rPos);
                float ratio = currentDist / initialHandDistance;
                float clampedScale = Mathf.Clamp(initialRootScale.x * ratio, minScale, maxScale);
                targetScale = Vector3.one * clampedScale;
            }
            else
            {
                targetScale = Vector3.one;
            }

            Vector3 dir = (rPos - lPos);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.002f)
            {
                float currentAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
                float angleDelta = Mathf.DeltaAngle(initialAngle, currentAngle);
                targetRot = Quaternion.AngleAxis(-angleDelta, Vector3.up) * initialRootRot;
            }

            Vector3 deltaMid = currentMidpoint - initialMidpoint;
            if (deltaMid.sqrMagnitude > 0.0004f)
            {
                HasUserMovedTable = true;
            }

            Vector3 desiredPos = initialRootPos + deltaMid;
            desiredPos.y = Mathf.Clamp(desiredPos.y, 0.40f, 2.10f);
            targetPos = desiredPos;
        }

        public void RotateDegrees(float angle)
        {
            targetRot = Quaternion.Euler(0f, angle, 0f) * targetRot;
        }

        public void SetTargetPose(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            targetPos = pos;
            targetRot = rot;
            targetScale = scale;
            initialRootPos = pos;
            initialRootRot = rot;
            initialRootScale = scale;
            if (rootToMove != null)
            {
                rootToMove.position = pos;
                rootToMove.rotation = rot;
                rootToMove.localScale = scale;
            }
        }

        public void SetTargetPose(Vector3 pos, Quaternion rot)
        {
            SetTargetPose(pos, rot, Vector3.one);
        }

        private bool CheckNativePinch(bool isLeft, bool wasPinching, out bool hasValidTracking)
        {
            hasValidTracking = false;

            // EXCLUSIVAMENTE dedo ÍNDICE + PULGAR con umbral alto (0.82 para iniciar, 0.66 para mantener).
            // NUNCA comprobar el dedo corazón (Middle), ya que al tener la otra mano relajada o semi-cerrada
            // activaba falsamente el pellizco y movía la mesa al coger una herramienta.
            OVRPlugin.Hand hand = isLeft ? OVRPlugin.Hand.HandLeft : OVRPlugin.Hand.HandRight;
            if (OVRPlugin.GetHandState(OVRPlugin.Step.Render, hand, ref (isLeft ? ref leftHandState : ref rightHandState)))
            {
                var state = isLeft ? leftHandState : rightHandState;

                if ((state.Status & OVRPlugin.HandStatus.HandTracked) == 0)
                {
                    if (isLeft) leftTrackedFrames = 0; else rightTrackedFrames = 0;
                    return false;
                }

                if (isLeft)
                {
                    leftTrackedFrames++;
                    hasValidTracking = leftTrackedFrames >= 5;
                }
                else
                {
                    rightTrackedFrames++;
                    hasValidTracking = rightTrackedFrames >= 5;
                }

                if (!hasValidTracking) return false;

                float threshold = wasPinching ? 0.66f : 0.82f;
                float indexStrength = 0f;
                if (state.PinchStrength != null && state.PinchStrength.Length > (int)OVRPlugin.HandFinger.Index)
                {
                    indexStrength = state.PinchStrength[(int)OVRPlugin.HandFinger.Index];
                }

                return indexStrength > threshold;
            }

            var activeCtrl = OVRInput.GetActiveController();
            bool usingTouch = (activeCtrl == OVRInput.Controller.Touch ||
                               activeCtrl == OVRInput.Controller.LTouch ||
                               activeCtrl == OVRInput.Controller.RTouch);
            if (usingTouch)
            {
                hasValidTracking = true;
                if (isLeft)
                    return OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
                else
                    return OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
            }

            return false;
        }
    }
}
