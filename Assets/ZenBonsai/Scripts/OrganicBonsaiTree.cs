using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZenBonsai
{
    [Serializable]
    public class SpeciesTreeModel
    {
        public TreeType treeType;
        public GameObject treeRoot;
        public Transform trunkBase;
        public List<PrunableBranch> prunableBranches = new List<PrunableBranch>();
        public Transform[] leafNodes;
        public GameObject blossomContainer;
    }

    /// <summary>
    /// Gestiona las 3 especies de bonsáis auténticos japoneses con geometrías y siluetas
    /// completamente diferenciadas y de alta fidelidad:
    /// - Matsu: Pino Negro de tronco nudoso, platos de agujas horizontales y madera muerta Jin.
    /// - Sakura: Cerezo en flor de ramas arqueadas, floración de pétalos rosados y brotes tiernos.
    /// - Momiji: Arce japonés en abanico (Hokidachi) con hojas palmadas en rojo otoñal bermellón.
    /// </summary>
    public class OrganicBonsaiTree : MonoBehaviour
    {
        public TreeType currentSpecies = TreeType.Sakura;

        [Header("Modelos de Especies Auténticas")]
        public SpeciesTreeModel sakuraModel = new SpeciesTreeModel();
        public SpeciesTreeModel matsuModel = new SpeciesTreeModel();
        public SpeciesTreeModel momijiModel = new SpeciesTreeModel();

        [Header("Materiales Compartidos")]
        public Material trunkMaterial;
        public Material potMaterial;
        public Material soilMaterial;
        public Material mossMaterial;
        public Material stoneMaterial;
        public Material foliageMaterial;
        public Material blossomMaterial;

        private void Awake()
        {
            AssignBranchOwners(sakuraModel, TreeType.Sakura);
            AssignBranchOwners(matsuModel, TreeType.Matsu);
            AssignBranchOwners(momijiModel, TreeType.Momiji);
        }

        private void Start()
        {
            ApplySpecies(currentSpecies);
        }

        private void AssignBranchOwners(SpeciesTreeModel model, TreeType species)
        {
            if (model == null || model.prunableBranches == null) return;
            for (int i = 0; i < model.prunableBranches.Count; i++)
            {
                if (model.prunableBranches[i] != null)
                {
                    model.prunableBranches[i].ownerSpecies = species;
                    model.prunableBranches[i].EnsureInitialized();
                }
            }
        }

        public SpeciesTreeModel GetModel(TreeType type)
        {
            switch (type)
            {
                case TreeType.Sakura: return sakuraModel;
                case TreeType.Matsu: return matsuModel;
                case TreeType.Momiji: return momijiModel;
                default: return sakuraModel;
            }
        }

        /// <summary>
        /// Cambia en tiempo real entre las 3 geometrías auténticas de bonsái.
        /// </summary>
        public void ApplySpecies(TreeType type)
        {
            currentSpecies = type;

            bool isSakura = type == TreeType.Sakura;
            bool isMatsu = type == TreeType.Matsu;
            bool isMomiji = type == TreeType.Momiji;

            if (sakuraModel.treeRoot != null) sakuraModel.treeRoot.SetActive(isSakura);
            if (matsuModel.treeRoot != null) matsuModel.treeRoot.SetActive(isMatsu);
            if (momijiModel.treeRoot != null) momijiModel.treeRoot.SetActive(isMomiji);

            SpeciesTreeModel activeModel = GetModel(type);

            // Registrar ramas y nodos en el Manager central del juego
            if (ZenBonsaiManager.Instance != null && activeModel != null)
            {
                ZenBonsaiManager.Instance.RegisterTreeData(
                    activeModel.prunableBranches,
                    activeModel.trunkBase,
                    activeModel.leafNodes,
                    activeModel.blossomContainer
                );
            }

            // Sonido de campana tibetana armónica al florecer o cambiar
            if (ZenSoundSynthesizer.Instance != null)
            {
                float freq = isSakura ? 528f : (isMatsu ? 432f : 480f);
                ZenSoundSynthesizer.Instance.PlayTibetanBell(freq, 2.0f, 0.45f);
            }

            Debug.Log($"<color=green>[Zen Bonsai]</color> Árbol auténtico activo: {type}");
        }
    }
}
