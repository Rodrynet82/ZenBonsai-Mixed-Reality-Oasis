using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Sistema de manipulación de la mesa de bonsái para manos (Hand Tracking) y mandos.
    /// Permite agarrar las asas con el gesto de pellizco (Pinch) o agarre (Grip),
    /// mover la mesa en 3D y girarla libremente ("darle la vuelta").
    /// </summary>
    public class DraggableZenTable : MonoBehaviour
    {
        [Header("Referencias")]
        public Transform rootToMove;
        public float grabRadius = 0.20f; // Distancia para poder agarrar el asa

        private Transform activeHand = null;
        private Vector3 initialHandPos;
        private Quaternion initialHandRot;
        private Vector3 initialRootPos;
        private Quaternion initialRootRot;
        private bool isHolding = false;

        private void Start()
        {
            if (rootToMove == null)
            {
                rootToMove = transform.root;
            }
        }

        private void Update()
        {
            CheckHandGrabs();

            if (isHolding && activeHand != null && rootToMove != null)
            {
                // Mover y rotar la mesa siguiendo el movimiento y orientación de la mano/muñeca
                Vector3 handDeltaPos = activeHand.position - initialHandPos;
                Quaternion handDeltaRot = activeHand.rotation * Quaternion.Inverse(initialHandRot);

                rootToMove.position = initialRootPos + handDeltaPos;
                rootToMove.rotation = handDeltaRot * initialRootRot;

                // Verificar si se soltó el agarre
                if (!IsPinchingOrGripping(activeHand))
                {
                    Release();
                }
            }
        }

        private void CheckHandGrabs()
        {
            if (isHolding) return;

            // Buscar manos o mandos cercanos al asa
            Collider[] colliders = Physics.OverlapSphere(transform.position, grabRadius);
            foreach (var col in colliders)
            {
                if (IsHandCollider(col))
                {
                    Transform hand = col.transform;
                    if (IsPinchingOrGripping(hand))
                    {
                        Grab(hand);
                        break;
                    }
                }
            }
        }

        private void Grab(Transform hand)
        {
            isHolding = true;
            activeHand = hand;

            initialHandPos = hand.position;
            initialHandRot = hand.rotation;
            initialRootPos = rootToMove.position;
            initialRootRot = rootToMove.rotation;

            // Sonido de agarre suave
            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlayTibetanBell(600f, 0.4f, 0.25f);
            }
        }

        private void Release()
        {
            isHolding = false;
            activeHand = null;
        }

        private bool IsPinchingOrGripping(Transform hand)
        {
            // Detecta agarre por OVRInput (manos o mandos)
            bool leftGrip = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
            bool rightGrip = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);

            if (leftGrip || rightGrip) return true;

            // OVRHand pinch check si está disponible en la jerarquía
            OVRHand ovrHand = hand.GetComponentInParent<OVRHand>();
            if (ovrHand != null)
            {
                return ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index) ||
                       ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Middle);
            }

            // Si está dentro del trigger físico y tocando
            return true;
        }

        private bool IsHandCollider(Collider col)
        {
            string n = col.name.ToLower();
            string rootN = col.transform.root.name.ToLower();
            return n.Contains("hand") || n.Contains("finger") || n.Contains("controller") || n.Contains("wrist") ||
                   rootN.Contains("hand") || rootN.Contains("controller");
        }

        private void OnTriggerStay(Collider other)
        {
            if (!isHolding && IsHandCollider(other))
            {
                if (IsPinchingOrGripping(other.transform))
                {
                    Grab(other.transform);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isHolding && activeHand == other.transform)
            {
                Release();
            }
        }

        /// <summary>
        /// Botón táctil para girar la mesa rápidamente 45 grados.
        /// </summary>
        public void RotateTableDegrees(float degrees)
        {
            if (rootToMove != null)
            {
                rootToMove.Rotate(Vector3.up, degrees, Space.World);
                if (ZenSoundSynthesizer.Instance != null)
                {
                    ZenSoundSynthesizer.Instance.PlayTibetanBell(528f, 0.8f, 0.35f);
                }
            }
        }
    }
}
