using System;
using UnityEngine;

namespace ZenBonsai
{
    public enum TreeType
    {
        Sakura, // Cerezo en flor
        Matsu,  // Pino negro ancestral
        Momiji  // Arce rojo japonés
    }

    [Serializable]
    public class BonsaiSpeciesData
    {
        public TreeType treeType;
        public string displayName;
        public string description;
        public Color trunkColor;
        public Color healthyFoliageColor;
        public Color witheredColor;
        public Color blossomColor;

        public static BonsaiSpeciesData GetPreset(TreeType type)
        {
            switch (type)
            {
                case TreeType.Sakura:
                    return new BonsaiSpeciesData
                    {
                        treeType = TreeType.Sakura,
                        displayName = "Sakura (Cerezo Japonés)",
                        description = "Símbolo de renovación y belleza efímera. Florece en delicados tonos rosados.",
                        trunkColor = new Color(0.28f, 0.18f, 0.12f),
                        healthyFoliageColor = new Color(0.25f, 0.72f, 0.35f),
                        witheredColor = new Color(0.85f, 0.65f, 0.18f),
                        blossomColor = new Color(1.0f, 0.65f, 0.82f)
                    };

                case TreeType.Matsu:
                    return new BonsaiSpeciesData
                    {
                        treeType = TreeType.Matsu,
                        displayName = "Matsu (Pino Negro Zen)",
                        description = "Símbolo de longevidad y fortaleza. Agujas perennes verde bosque y porte majestuoso.",
                        trunkColor = new Color(0.20f, 0.14f, 0.10f),
                        healthyFoliageColor = new Color(0.12f, 0.45f, 0.18f),
                        witheredColor = new Color(0.75f, 0.45f, 0.12f),
                        blossomColor = new Color(0.95f, 0.85f, 0.40f)
                    };

                case TreeType.Momiji:
                    return new BonsaiSpeciesData
                    {
                        treeType = TreeType.Momiji,
                        displayName = "Momiji (Arce Rojo Otoñal)",
                        description = "Símbolo de contemplación y serenidad. Follaje bermellón y dorado de otoño.",
                        trunkColor = new Color(0.32f, 0.22f, 0.16f),
                        healthyFoliageColor = new Color(0.85f, 0.20f, 0.15f),
                        witheredColor = new Color(0.55f, 0.35f, 0.20f),
                        blossomColor = new Color(1.0f, 0.80f, 0.30f)
                    };

                default:
                    return GetPreset(TreeType.Sakura);
            }
        }
    }
}
