using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Tablero / Bandeja de madera zen (Tool Tray) donde descansan las herramientas de bonsái.
    /// Contiene los anclajes y ranuras magnéticas para Tijeras, Regadera y Pincel.
    /// </summary>
    public class ZenToolTray : MonoBehaviour
    {
        [Header("Ranuras de Herramientas")]
        public Transform shearsSlot;
        public Transform wateringCanSlot;
        public Transform broomSlot;

        [Header("Herramientas Asignadas")]
        public ZenBonsaiTool shears;
        public ZenBonsaiTool wateringCan;
        public ZenBonsaiTool broom;
    }
}
