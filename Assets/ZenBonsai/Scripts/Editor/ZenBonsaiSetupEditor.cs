#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ZenBonsai.Editor
{
    public class ZenBonsaiSetupEditor : EditorWindow
    {
        [MenuItem("Tools/Zen Bonsai/Montar Experiencia Bonsai en la Escena", false, 1)]
        public static void BuildZenBonsaiScene()
        {
            // 1. Contenedor Principal (posicionado ergonómicamente a la altura perfecta de trabajo sentado)
            var activeSceneRoots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            GameObject root = null;
            foreach (var rObj in activeSceneRoots)
            {
                if (rObj == null) continue;
                if (rObj.name.Contains("ZenHUD"))
                {
                    DestroyImmediate(rObj);
                    continue;
                }
                if (rObj.name == "ZenBonsai_Experience")
                {
                    if (root == null)
                    {
                        root = rObj;
                    }
                    else
                    {
                        // Eliminar contenedores raíz duplicados si existieran
                        DestroyImmediate(rObj);
                    }
                }
            }

            if (root == null)
            {
                root = new GameObject("ZenBonsai_Experience");
            }
            root.transform.position = new Vector3(0f, 0.96f, 0.50f);
            root.transform.rotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            // LIMPIEZA TOTAL DE HIJOS HEREDADOS: elimina los antiguos bloques grises (ZenWaterBowl),
            // troncos cilíndricos antiguos (BonsaiTrunk) y macetas primitivas (BonsaiPot) acumulados en la escena
            while (root.transform.childCount > 0)
            {
                DestroyImmediate(root.transform.GetChild(0).gameObject);
            }

            // Componentes de calidad gráfica y audio
            if (root.GetComponent<ZenQualityOptimizer>() == null) root.AddComponent<ZenQualityOptimizer>();
            if (root.GetComponent<ZenSoundSynthesizer>() == null) root.AddComponent<ZenSoundSynthesizer>();
            if (root.GetComponent<ZenVisualEffects>() == null) root.AddComponent<ZenVisualEffects>();

            ZenBonsaiManager manager = root.GetComponent<ZenBonsaiManager>();
            if (manager == null) manager = root.AddComponent<ZenBonsaiManager>();
            manager.forwardDistance = 0.50f;
            manager.downwardOffset = 0.24f;

            // ==========================================
            // PALETA DE MATERIALES PBR DE ALTA DEFINICIÓN CON TEXTURAS + MAPAS DE NORMALES 3D
            // ==========================================
            Material tableMat = CreateOrGetPBRMaterial("Zen_TableMat", new Color(0.08f, 0.045f, 0.038f), 0.94f, 0.02f); // Lacado Urushi caoba brillante reflectante
            Material tableApronMat = CreateOrGetPBRMaterial("Zen_TableApronMat", new Color(0.12f, 0.07f, 0.05f), 0.78f, 0.02f); // Palisandro Shitan tallado
            Material potMat = CreateOrGetSurfaceTexturedMaterial("Zen_PotMat", new Color(0.08f, 0.16f, 0.34f), new Color(0.03f, 0.07f, 0.18f), new Color(0.32f, 0.28f, 0.22f), 0.92f, 0.08f, 10); // Cerámica Tokoname esmaltada azul cobalto imperial con pátina de horno
            Material soilMat = CreateOrGetSurfaceTexturedMaterial("Zen_SoilMat", new Color(0.15f, 0.11f, 0.07f), new Color(0.07f, 0.05f, 0.03f), new Color(0.26f, 0.19f, 0.12f), 0.12f, 0.0f, 11); // Sustrato granular Akadama
            Material mossDeepMat = CreateOrGetSurfaceTexturedMaterial("Zen_MossDeepMat", new Color(0.14f, 0.38f, 0.11f), new Color(0.07f, 0.20f, 0.05f), new Color(0.28f, 0.54f, 0.16f), 0.16f, 0.0f, 11); // Musgo Kyoto verde profundo aterciopelado
            Material mossLightMat = CreateOrGetSurfaceTexturedMaterial("Zen_MossLightMat", new Color(0.24f, 0.56f, 0.15f), new Color(0.12f, 0.32f, 0.08f), new Color(0.42f, 0.72f, 0.22f), 0.20f, 0.0f, 11); // Musgo esmeralda con relieve 3D
            Material stoneMat = CreateOrGetSurfaceTexturedMaterial("Zen_StoneMat", new Color(0.26f, 0.28f, 0.30f), new Color(0.14f, 0.15f, 0.17f), new Color(0.42f, 0.44f, 0.46f), 0.52f, 0.08f, 11); // Roca mineral Suiseki tallada
            Material waterMat = CreateOrGetPBRMaterial("Zen_WaterMat", new Color(0.18f, 0.72f, 0.90f, 0.85f), 0.96f, 0.05f);
            Material buttonMat = CreateOrGetPBRMaterial("Zen_ButtonMat", new Color(0.20f, 0.26f, 0.32f), 0.74f, 0.05f);
            Material infoButtonMat = CreateOrGetPBRMaterial("Zen_InfoButtonMat", new Color(0.18f, 0.44f, 0.52f), 0.78f, 0.08f);
            Material goldMat = CreateOrGetPBRMaterial("Zen_GoldMat", new Color(0.92f, 0.76f, 0.28f), 0.88f, 0.70f);

            // Materiales botánicos de diseñador con textura procedural de corteza orgánica y venación de pétalos/hojas
            Material sakuraBarkMat = CreateOrGetBarkMaterial("Zen_SakuraBarkMat",
                new Color(0.28f, 0.18f, 0.14f), new Color(0.10f, 0.07f, 0.05f), new Color(0.44f, 0.32f, 0.25f), 0.38f, 1); // Castaño caoba envejecido con lenticelas
            Material sakuraPetalPinkMat = CreateOrGetSurfaceTexturedMaterial("Zen_SakuraPetalPinkMat",
                new Color(1.0f, 0.74f, 0.84f), new Color(0.84f, 0.28f, 0.48f), new Color(1.0f, 0.94f, 0.97f), 0.54f, 0.0f, 12); // Pétalo Sakura degradado con corazón carmesí y nervaduras
            sakuraPetalPinkMat.EnableKeyword("_EMISSION");
            if (sakuraPetalPinkMat.HasProperty("_EmissionColor")) sakuraPetalPinkMat.SetColor("_EmissionColor", new Color(0.24f, 0.10f, 0.15f)); // Luz filtrada translúcida

            Material sakuraPetalWhiteMat = CreateOrGetSurfaceTexturedMaterial("Zen_SakuraPetalWhiteMat",
                new Color(0.99f, 0.92f, 0.96f), new Color(0.88f, 0.42f, 0.60f), new Color(1.0f, 0.98f, 0.99f), 0.56f, 0.0f, 12); // Blanco nacarado Somei-Yoshino con cáliz rosado
            sakuraPetalWhiteMat.EnableKeyword("_EMISSION");
            if (sakuraPetalWhiteMat.HasProperty("_EmissionColor")) sakuraPetalWhiteMat.SetColor("_EmissionColor", new Color(0.18f, 0.13f, 0.16f));

            Material sakuraLeafShootMat = CreateOrGetSurfaceTexturedMaterial("Zen_SakuraLeafShootMat",
                new Color(0.28f, 0.56f, 0.22f), new Color(0.16f, 0.36f, 0.12f), new Color(0.48f, 0.72f, 0.30f), 0.36f, 0.0f, 13); // Brotes primaverales aserrados

            Material matsuBarkMat = CreateOrGetBarkMaterial("Zen_MatsuBarkMat",
                new Color(0.22f, 0.15f, 0.11f), new Color(0.08f, 0.05f, 0.04f), new Color(0.38f, 0.28f, 0.21f), 0.16f, 2); // Pino Kuromatsu con placas escamosas
            Material matsuNeedleDeepMat = CreateOrGetSurfaceTexturedMaterial("Zen_MatsuNeedleDeepMat",
                new Color(0.09f, 0.36f, 0.14f), new Color(0.04f, 0.20f, 0.07f), new Color(0.18f, 0.50f, 0.22f), 0.28f, 0.0f, 15); // Acícula pino imperial
            Material matsuNeedleTipMat = CreateOrGetSurfaceTexturedMaterial("Zen_MatsuNeedleTipMat",
                new Color(0.18f, 0.56f, 0.22f), new Color(0.09f, 0.36f, 0.12f), new Color(0.36f, 0.74f, 0.32f), 0.32f, 0.0f, 15); // Brotes aciculares Midori
            Material jinWoodMat = CreateOrGetBarkMaterial("Zen_JinWoodMat",
                new Color(0.92f, 0.89f, 0.81f), new Color(0.68f, 0.63f, 0.54f), new Color(0.98f, 0.96f, 0.90f), 0.14f, 3); // Madera muerta Jin/Shari blanqueada con veta fibrosa

            Material momijiBarkMat = CreateOrGetBarkMaterial("Zen_MomijiBarkMat",
                new Color(0.42f, 0.32f, 0.25f), new Color(0.22f, 0.16f, 0.12f), new Color(0.56f, 0.46f, 0.38f), 0.30f, 4); // Corteza gris-canela estriada de arce japonés
            Material momijiRubyMat = CreateOrGetSurfaceTexturedMaterial("Zen_MomijiRubyMat",
                new Color(0.76f, 0.10f, 0.11f), new Color(0.44f, 0.04f, 0.05f), new Color(0.94f, 0.24f, 0.16f), 0.40f, 0.0f, 13); // Rojo rubí profundo con venación
            Material momijiScarletMat = CreateOrGetSurfaceTexturedMaterial("Zen_MomijiScarletMat",
                new Color(0.94f, 0.18f, 0.10f), new Color(0.62f, 0.07f, 0.05f), new Color(1.0f, 0.44f, 0.14f), 0.42f, 0.0f, 13); // Escarlata encendido Deshojo
            Material momijiAmberMat = CreateOrGetSurfaceTexturedMaterial("Zen_MomijiAmberMat",
                new Color(0.98f, 0.54f, 0.09f), new Color(0.76f, 0.24f, 0.05f), new Color(1.0f, 0.78f, 0.22f), 0.44f, 0.0f, 13); // Ámbar dorado otoñal

            // Materiales de herramientas artesanas forjadas y bambú natural con relieve 3D
            Material copperMat = CreateOrGetPBRMaterial("Zen_CopperMat", new Color(0.85f, 0.49f, 0.31f), 0.84f, 0.88f);
            Material steelMat = CreateOrGetSurfaceTexturedMaterial("Zen_SteelMat",
                new Color(0.36f, 0.38f, 0.41f), new Color(0.18f, 0.19f, 0.22f), new Color(0.64f, 0.67f, 0.72f), 0.78f, 0.90f, 14); // Acero forjado japonés bruñido con grano metálico (como en la imagen)
            Material silverBladeMat = CreateOrGetSurfaceTexturedMaterial("Zen_SilverBladeMat",
                new Color(0.88f, 0.91f, 0.95f), new Color(0.66f, 0.70f, 0.75f), new Color(0.99f, 0.99f, 1.0f), 0.95f, 0.96f, 14); // Filo de acero templado pulido a espejo
            Material bambooMat = CreateOrGetSurfaceTexturedMaterial("Zen_BambooMat",
                new Color(0.80f, 0.70f, 0.45f), new Color(0.58f, 0.48f, 0.26f), new Color(0.90f, 0.82f, 0.58f), 0.56f, 0.0f, 15); // Bambú natural con fibras longitudinales
            Material bristleMat = CreateOrGetSurfaceTexturedMaterial("Zen_BristleMat",
                new Color(0.44f, 0.31f, 0.20f), new Color(0.24f, 0.16f, 0.09f), new Color(0.60f, 0.45f, 0.30f), 0.18f, 0.0f, 15); // Fibra de palma Shuro trenzada
            Material trayWoodMat = CreateOrGetPBRMaterial("Zen_TrayWoodMat", new Color(0.32f, 0.22f, 0.15f), 0.55f, 0.0f);

            // ==========================================
            // 2. MESA DE EXPOSICIÓN JAPONESA TALLADA (KAKIITA / SHITAN) CON ESTERILLA SUDARE
            // ==========================================
            BuildJapaneseDisplayTable(root.transform, tableMat, tableApronMat, goldMat, bambooMat);

            // Manipulador espacial a 2 manos (con tamaño original 1:1 bloqueado para no deformar el bonsái)
            ZenBonsaiManipulator manipulator = root.GetComponent<ZenBonsaiManipulator>();
            if (manipulator == null) manipulator = root.AddComponent<ZenBonsaiManipulator>();
            manipulator.rootToMove = root.transform;
            manipulator.allowScaleChange = false;

            // ==========================================
            // 3. MACETA CERÁMICA TOKONAME BISELADA Y MANTO DE MUSGO ESCULPIDO
            // ==========================================
            GameObject bonsaiRoot = new GameObject("BonsaiTree_Root");
            bonsaiRoot.transform.SetParent(root.transform, false);
            bonsaiRoot.transform.localPosition = Vector3.zero;

            OrganicBonsaiTree organicTree = bonsaiRoot.AddComponent<OrganicBonsaiTree>();
            manager.organicTree = organicTree;

            BuildBonsaiPot(bonsaiRoot.transform, potMat, soilMat, mossDeepMat, mossLightMat, stoneMat, goldMat);

            // ==========================================
            // 4. ESCULTURAS BOTÁNICAS 3D PROCEDURALES DE ALTA DEFINICIÓN CON TRONCOS REALISTAS Y SHARI
            // ==========================================
            organicTree.sakuraModel = BuildSakuraTree(bonsaiRoot.transform, sakuraBarkMat, jinWoodMat, mossLightMat, sakuraPetalPinkMat, sakuraPetalWhiteMat, sakuraLeafShootMat, goldMat);
            organicTree.matsuModel = BuildMatsuTree(bonsaiRoot.transform, matsuBarkMat, matsuNeedleDeepMat, matsuNeedleTipMat, jinWoodMat, stoneMat, mossLightMat);
            organicTree.momijiModel = BuildMomijiTree(bonsaiRoot.transform, momijiBarkMat, jinWoodMat, mossLightMat, momijiRubyMat, momijiScarletMat, momijiAmberMat);

            // ==========================================
            // 5. EXPOSITOR DE HERRAMIENTAS ORIENTADO HACIA EL JUGADOR + KUSAMONO DECORATIVO DETRÁS
            // ==========================================
            BuildToolTray(root.transform, trayWoodMat, tableMat, steelMat, silverBladeMat, copperMat, bambooMat, bristleMat, goldMat, potMat, mossLightMat, stoneMat, sakuraPetalPinkMat);

            // ==========================================
            // 6. FUENTE DE PIEDRA TSUKUBAI CON CAZO DE BAMBÚ INTERACTIVO DELANTE A LA IZQUIERDA (SIMÉTRICO A LAS HERRAMIENTAS)
            // ==========================================
            BuildTsukubaiWaterBasin(root.transform, stoneMat, waterMat, bambooMat, mossLightMat, goldMat);

            // ==========================================
            // 7. PABELLÓN JAPONÉS "JARDÍN ZEN" (TATEFUDA) CON TEJADO DE PAGODA, NEONES Y PANEL DE OPCIONES
            // ==========================================
            BuildZenPavilionSign(
                root.transform,
                tableMat, tableApronMat, trayWoodMat, goldMat, stoneMat,
                sakuraPetalPinkMat, matsuNeedleDeepMat, momijiRubyMat,
                buttonMat, infoButtonMat,
                organicTree);

            SetupCinematicBonsaiLighting();

            AssetDatabase.SaveAssets();
            Selection.activeGameObject = root;
            Undo.RegisterCreatedObjectUndo(root, "Setup Zen Bonsai Experience HD Masterpiece v22");
            Debug.Log("<color=green>[Zen Bonsai]</color> ¡Experiencia Bonsai HD Masterpiece v22 montada con éxito!");
        }

        private static void SetupCinematicBonsaiLighting()
        {
            // Configurar o crear la luz de sol dorada de ángulo rasante (Rim Light) que perfila la copa y corteza
            Light mainSun = null;
            Light fillSky = null;

            Light[] lights = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude);
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional)
                {
                    if (l.gameObject.name.Contains("Fill") || l.gameObject.name.Contains("Sky"))
                    {
                        fillSky = l;
                    }
                    else if (mainSun == null)
                    {
                        mainSun = l;
                    }
                }
            }

            if (mainSun == null)
            {
                GameObject sunObj = new GameObject("Zen_Sun_RimLight");
                mainSun = sunObj.AddComponent<Light>();
                mainSun.type = LightType.Directional;
            }

            mainSun.gameObject.name = "Zen_Sun_RimLight";
            mainSun.transform.rotation = Quaternion.Euler(28f, -138f, 0f);
            mainSun.color = new Color(1.0f, 0.93f, 0.82f); // Luz cálida dorada de hora mágica
            mainSun.intensity = 1.25f;
            mainSun.shadows = LightShadows.Soft;

            if (fillSky == null)
            {
                GameObject fillObj = new GameObject("Zen_Sky_FillLight");
                fillSky = fillObj.AddComponent<Light>();
                fillSky.type = LightType.Directional;
            }

            fillSky.gameObject.name = "Zen_Sky_FillLight";
            fillSky.transform.rotation = Quaternion.Euler(38f, 42f, 0f);
            fillSky.color = new Color(0.72f, 0.82f, 0.98f); // Relleno celeste suave y nítido
            fillSky.intensity = 0.42f;
            fillSky.shadows = LightShadows.None;

            // Configuración ambiental Trilight para iluminación envolvente suave
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.68f, 0.78f, 0.92f);
            RenderSettings.ambientEquatorColor = new Color(0.75f, 0.70f, 0.65f);
            RenderSettings.ambientGroundColor = new Color(0.24f, 0.20f, 0.18f);
        }

        private static void BuildZenPavilionSign(
            Transform parent,
            Material urushiMat, Material rosewoodMat, Material hinokiMat, Material goldMat, Material stoneMat,
            Material sakuraMat, Material matsuMat, Material momijiMat,
            Material buttonMat, Material infoButtonMat,
            OrganicBonsaiTree organicTree)
        {
            GameObject menuRoot = new GameObject("ZenSpatialMenu");
            menuRoot.transform.SetParent(parent, false);
            menuRoot.transform.localPosition = Vector3.zero;
            menuRoot.transform.localRotation = Quaternion.identity;

            ZenSpatialMenu spatialMenu = menuRoot.AddComponent<ZenSpatialMenu>();

            // =========================================================================
            // 1. CONSOLA FRONTAL INTEGRADA EN EL BORDE DELANTERO DE LA MESA (FRENTE AL JUGADOR)
            // No tapa el bonsái ni estorba al cuenco de agua: queda cómodamente al alcance de las manos
            // =========================================================================
            GameObject panelContainer = new GameObject("MenuPanelContainer");
            panelContainer.transform.SetParent(menuRoot.transform, false);
            panelContainer.transform.localPosition = new Vector3(0f, -0.048f, -0.258f);
            panelContainer.transform.localRotation = Quaternion.Euler(36f, 0f, 0f);
            spatialMenu.menuPanel = panelContainer;

            // Soportes de madera lacada que unen la consola al borde frontal de la mesa
            float[] bracketX = { -0.22f, 0.22f };
            for (int b = 0; b < bracketX.Length; b++)
            {
                GameObject bracket = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bracket.name = $"Console_TableBracket_{b}";
                bracket.transform.SetParent(panelContainer.transform, false);
                bracket.transform.localPosition = new Vector3(bracketX[b], 0.010f, 0.018f);
                bracket.transform.localScale = new Vector3(0.024f, 0.022f, 0.036f);
                bracket.GetComponent<Renderer>().sharedMaterial = urushiMat;
                var bc = bracket.GetComponent<Collider>(); if (bc != null) DestroyImmediate(bc);
            }

            // Tablero exterior lacado Urushi de perfil bajo en el frente de la mesa
            GameObject outerFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outerFrame.name = "Console_OuterUrushiFrame";
            outerFrame.transform.SetParent(panelContainer.transform, false);
            outerFrame.transform.localPosition = new Vector3(0f, 0f, 0.002f);
            outerFrame.transform.localScale = new Vector3(0.525f, 0.118f, 0.014f);
            outerFrame.GetComponent<Renderer>().sharedMaterial = urushiMat;
            var ofCol = outerFrame.GetComponent<Collider>(); if (ofCol != null) DestroyImmediate(ofCol);

            // Tablero interior de palisandro Shitan
            GameObject innerBoard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            innerBoard.name = "Console_InnerRosewoodBoard";
            innerBoard.transform.SetParent(panelContainer.transform, false);
            innerBoard.transform.localPosition = new Vector3(0f, 0f, -0.003f);
            innerBoard.transform.localScale = new Vector3(0.505f, 0.102f, 0.010f);
            innerBoard.GetComponent<Renderer>().sharedMaterial = rosewoodMat;
            var ibCol = innerBoard.GetComponent<Collider>(); if (ibCol != null) DestroyImmediate(ibCol);

            // Filetes dorados Maki-e superior e inferior en la consola frontal
            float[] trimY = { 0.048f, -0.048f };
            for (int g = 0; g < trimY.Length; g++)
            {
                GameObject gBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gBar.name = $"Console_GoldTrim_{g}";
                gBar.transform.SetParent(panelContainer.transform, false);
                gBar.transform.localPosition = new Vector3(0f, trimY[g], -0.009f);
                gBar.transform.localScale = new Vector3(0.495f, 0.003f, 0.004f);
                gBar.GetComponent<Renderer>().sharedMaterial = goldMat;
                var gc = gBar.GetComponent<Collider>(); if (gc != null) DestroyImmediate(gc);
            }

            // Separador vertical dorado entre los 3 botones de árboles y los 2 botones de paneles (Info / Opciones)
            GameObject vDivider = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vDivider.name = "Console_VerticalGoldDivider";
            vDivider.transform.SetParent(panelContainer.transform, false);
            vDivider.transform.localPosition = new Vector3(0.052f, -0.004f, -0.009f);
            vDivider.transform.localScale = new Vector3(0.0035f, 0.064f, 0.005f);
            vDivider.GetComponent<Renderer>().sharedMaterial = goldMat;
            var vdc = vDivider.GetComponent<Collider>(); if (vdc != null) DestroyImmediate(vdc);

            // =========================================================================
            // PLACA IMPERIAL SUPERIOR (HENGAKU / EMA) ENCIMA DE LOS BOTONES PARA ARMONÍA E HIDRATACIÓN
            // Queda elevada sobre la consola (y = +0.068m), totalmente despejada y con acabados en laca, Hinoki y oro
            // =========================================================================
            GameObject hengakuRoot = new GameObject("Hengaku_UpperStatusPlaque");
            hengakuRoot.transform.SetParent(panelContainer.transform, false);
            hengakuRoot.transform.localPosition = new Vector3(0f, 0.070f, -0.008f);

            // Soportes dorados que unen la placa Hengaku superior al marco de la consola
            float[] pillarX = { -0.185f, 0.185f };
            for (int p = 0; p < 2; p++)
            {
                GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pillar.name = $"Hengaku_GoldPillar_{p}";
                pillar.transform.SetParent(hengakuRoot.transform, false);
                pillar.transform.localPosition = new Vector3(pillarX[p], -0.022f, 0.002f);
                pillar.transform.localScale = new Vector3(0.007f, 0.014f, 0.007f);
                pillar.GetComponent<Renderer>().sharedMaterial = goldMat;
                var pc = pillar.GetComponent<Collider>(); if (pc != null) DestroyImmediate(pc);
            }

            // Marco exterior lacado Urushi de la placa superior
            GameObject hOuter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hOuter.name = "Hengaku_OuterUrushi";
            hOuter.transform.SetParent(hengakuRoot.transform, false);
            hOuter.transform.localPosition = Vector3.zero;
            hOuter.transform.localScale = new Vector3(0.495f, 0.036f, 0.012f);
            hOuter.GetComponent<Renderer>().sharedMaterial = urushiMat;
            var hoc = hOuter.GetComponent<Collider>(); if (hoc != null) DestroyImmediate(hoc);

            // Tablero interior de palisandro Shitan
            GameObject hInner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hInner.name = "Hengaku_InnerRosewood";
            hInner.transform.SetParent(hengakuRoot.transform, false);
            hInner.transform.localPosition = new Vector3(0f, 0f, -0.003f);
            hInner.transform.localScale = new Vector3(0.478f, 0.027f, 0.009f);
            hInner.GetComponent<Renderer>().sharedMaterial = rosewoodMat;
            var hic = hInner.GetComponent<Collider>(); if (hic != null) DestroyImmediate(hic);

            // Filetes dorados Maki-e superior e inferior de la placa Hengaku
            float[] hTrimY = { 0.015f, -0.015f };
            for (int ht = 0; ht < 2; ht++)
            {
                GameObject hBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                hBar.name = $"Hengaku_GoldTrim_{ht}";
                hBar.transform.SetParent(hengakuRoot.transform, false);
                hBar.transform.localPosition = new Vector3(0f, hTrimY[ht], -0.006f);
                hBar.transform.localScale = new Vector3(0.484f, 0.0028f, 0.004f);
                hBar.GetComponent<Renderer>().sharedMaterial = goldMat;
                var hbc = hBar.GetComponent<Collider>(); if (hbc != null) DestroyImmediate(hbc);
            }

            // Rodillos laterales de madera Hinoki y remates dorados Jiku en los extremos de la placa Hengaku
            float[] hEndX = { -0.252f, 0.252f };
            for (int e = 0; e < 2; e++)
            {
                GameObject hRod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                hRod.name = $"Hengaku_ScrollRod_{e}";
                hRod.transform.SetParent(hengakuRoot.transform, false);
                hRod.transform.localPosition = new Vector3(hEndX[e], 0f, -0.004f);
                hRod.transform.localScale = new Vector3(0.011f, 0.022f, 0.011f);
                hRod.GetComponent<Renderer>().sharedMaterial = hinokiMat;
                var hrc = hRod.GetComponent<Collider>(); if (hrc != null) DestroyImmediate(hrc);

                float[] capY = { 0.022f, -0.022f };
                for (int c = 0; c < 2; c++)
                {
                    GameObject jikuCap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    jikuCap.name = $"Hengaku_JikuFinial_{e}_{c}";
                    jikuCap.transform.SetParent(hengakuRoot.transform, false);
                    jikuCap.transform.localPosition = new Vector3(hEndX[e], capY[c], -0.004f);
                    jikuCap.transform.localScale = Vector3.one * 0.013f;
                    jikuCap.GetComponent<Renderer>().sharedMaterial = goldMat;
                    var jcc = jikuCap.GetComponent<Collider>(); if (jcc != null) DestroyImmediate(jcc);
                }
            }

            // Canvas de alta resolución en la placa superior Hengaku (delante de la placa: z = -0.010f)
            GameObject menuCanvasObj = new GameObject("Console_Canvas");
            menuCanvasObj.transform.SetParent(hengakuRoot.transform, false);
            menuCanvasObj.transform.localPosition = new Vector3(0f, 0f, -0.010f);
            menuCanvasObj.transform.localScale = Vector3.one * 0.00035f;

            Canvas mCanvas = menuCanvasObj.AddComponent<Canvas>();
            mCanvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler mScaler = menuCanvasObj.AddComponent<CanvasScaler>();
            mScaler.dynamicPixelsPerUnit = 5f;

            Font uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (uiFont == null) uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            // Estado en vivo del bonsái (Armonía e Hidratación) en la placa imperial ENCIMA de los botones
            GameObject mStatusObj = new GameObject("TreeStatusText");
            mStatusObj.transform.SetParent(menuCanvasObj.transform, false);
            mStatusObj.transform.localPosition = Vector3.zero;
            Text mStatus = mStatusObj.AddComponent<Text>();
            mStatus.font = uiFont;
            mStatus.alignment = TextAnchor.MiddleCenter;
            mStatus.fontSize = 31;
            mStatus.fontStyle = FontStyle.Bold;
            mStatus.horizontalOverflow = HorizontalWrapMode.Overflow;
            mStatus.verticalOverflow = VerticalWrapMode.Overflow;
            mStatus.text = "🌿 Sakura · Armonía 60% · Hidratación 86%";
            mStatus.color = new Color(0.94f, 0.99f, 0.90f);
            mStatusObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1320, 68);
            spatialMenu.treeStatusText = mStatus;

            // =========================================================================
            // FILA FRONTAL EN LA MESA (y = -0.006m, separada ampliamente de la placa superior):
            // 3 BOTONES DE ÁRBOLES (MEDALLONES KAMON) + BOTÓN INFO + BOTÓN OPCIONES
            // =========================================================================
            spatialMenu.btnSakura = Create3DRoundTouchButton(panelContainer.transform, "Btn_Sakura",
                "🌸", "桜", "Sakura", "Cerezo Flor",
                new Vector3(-0.188f, -0.006f, -0.014f), 0.076f, sakuraMat, new Color(1.0f, 0.68f, 0.82f), goldMat, urushiMat);

            spatialMenu.btnMatsu = Create3DRoundTouchButton(panelContainer.transform, "Btn_Matsu",
                "🌲", "松", "Matsu", "Pino Negro",
                new Vector3(-0.096f, -0.006f, -0.014f), 0.076f, matsuMat, new Color(0.60f, 0.96f, 0.72f), goldMat, urushiMat);

            spatialMenu.btnMomiji = Create3DRoundTouchButton(panelContainer.transform, "Btn_Momiji",
                "🍁", "楓", "Momiji", "Arce Rojo",
                new Vector3(-0.004f, -0.006f, -0.014f), 0.076f, momijiMat, new Color(1.0f, 0.70f, 0.45f), goldMat, urushiMat);

            spatialMenu.btnInfo = Create3DThematicRectButton(panelContainer.transform, "Btn_Info",
                "伝承", "📖 Cuidados", "Ficha Botánica",
                new Vector3(0.100f, -0.006f, -0.014f), new Vector3(0.088f, 0.054f, 0.018f), infoButtonMat, new Color(0.92f, 0.99f, 1.0f), goldMat, urushiMat);

            spatialMenu.btnOptions = Create3DThematicRectButton(panelContainer.transform, "Btn_Options",
                "神境", "⚙️ Opciones", "Ajustes Zen",
                new Vector3(0.198f, -0.006f, -0.014f), new Vector3(0.088f, 0.054f, 0.018f), buttonMat, new Color(1.0f, 0.90f, 0.58f), goldMat, urushiMat);

            // =========================================================================
            // 2. FICHA BOTÁNICA DESPLEGABLE (INFO PANEL) - KAKEMONO (PERGAMINO JAPONÉS)
            // =========================================================================
            GameObject infoCard = new GameObject("InfoPanel_BotanicalCard");
            infoCard.transform.SetParent(menuRoot.transform, false);
            infoCard.transform.localPosition = new Vector3(-0.24f, 0.17f, -0.14f);
            infoCard.transform.localRotation = Quaternion.Euler(12f, -14f, 0f);
            spatialMenu.infoPanelBox = infoCard;

            // Marco exterior Urushi / Seda oscura de montaje
            GameObject infoOuter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            infoOuter.name = "InfoCard_OuterFrame";
            infoOuter.transform.SetParent(infoCard.transform, false);
            infoOuter.transform.localPosition = Vector3.zero;
            infoOuter.transform.localScale = new Vector3(0.44f, 0.33f, 0.012f);
            infoOuter.GetComponent<Renderer>().sharedMaterial = urushiMat;
            var icCol = infoOuter.GetComponent<Collider>(); if (icCol != null) DestroyImmediate(icCol);

            // Tablero interior Washi / Seda palisandro
            GameObject infoInner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            infoInner.name = "InfoCard_InnerBoard";
            infoInner.transform.SetParent(infoCard.transform, false);
            infoInner.transform.localPosition = new Vector3(0f, 0f, -0.003f);
            infoInner.transform.localScale = new Vector3(0.418f, 0.308f, 0.009f);
            infoInner.GetComponent<Renderer>().sharedMaterial = rosewoodMat;
            var iiCol = infoInner.GetComponent<Collider>(); if (iiCol != null) DestroyImmediate(iiCol);

            // Barra superior de pergamino Kakemono (Ten) en madera Hinoki con remates dorados Jiku
            GameObject scrollRodTop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            scrollRodTop.name = "Kakemono_TopRod";
            scrollRodTop.transform.SetParent(infoCard.transform, false);
            scrollRodTop.transform.localPosition = new Vector3(0f, 0.168f, -0.007f);
            scrollRodTop.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            scrollRodTop.transform.localScale = new Vector3(0.012f, 0.225f, 0.012f);
            scrollRodTop.GetComponent<Renderer>().sharedMaterial = hinokiMat;
            var srtCol = scrollRodTop.GetComponent<Collider>(); if (srtCol != null) DestroyImmediate(srtCol);

            // Remates dorados (Jiku) en los extremos de la barra superior
            float[] jikuX = { -0.232f, 0.232f };
            for (int j = 0; j < 2; j++)
            {
                GameObject jiku = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                jiku.name = $"Kakemono_TopFinial_{j}";
                jiku.transform.SetParent(infoCard.transform, false);
                jiku.transform.localPosition = new Vector3(jikuX[j], 0.168f, -0.007f);
                jiku.transform.localScale = Vector3.one * 0.018f;
                jiku.GetComponent<Renderer>().sharedMaterial = goldMat;
                var jCol = jiku.GetComponent<Collider>(); if (jCol != null) DestroyImmediate(jCol);
            }

            // Barra inferior contrapeso de pergamino Kakemono (Chi)
            GameObject scrollRodBottom = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            scrollRodBottom.name = "Kakemono_BottomRod";
            scrollRodBottom.transform.SetParent(infoCard.transform, false);
            scrollRodBottom.transform.localPosition = new Vector3(0f, -0.168f, -0.007f);
            scrollRodBottom.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            scrollRodBottom.transform.localScale = new Vector3(0.014f, 0.225f, 0.014f);
            scrollRodBottom.GetComponent<Renderer>().sharedMaterial = hinokiMat;
            var srbCol = scrollRodBottom.GetComponent<Collider>(); if (srbCol != null) DestroyImmediate(srbCol);

            for (int j = 0; j < 2; j++)
            {
                GameObject jikuB = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                jikuB.name = $"Kakemono_BottomFinial_{j}";
                jikuB.transform.SetParent(infoCard.transform, false);
                jikuB.transform.localPosition = new Vector3(jikuX[j], -0.168f, -0.007f);
                jikuB.transform.localScale = Vector3.one * 0.020f;
                jikuB.GetComponent<Renderer>().sharedMaterial = goldMat;
                var jbCol = jikuB.GetComponent<Collider>(); if (jbCol != null) DestroyImmediate(jbCol);
            }

            // Filete dorado de separación bajo el título
            GameObject infoGoldRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            infoGoldRim.name = "InfoCard_GoldBorder";
            infoGoldRim.transform.SetParent(infoCard.transform, false);
            infoGoldRim.transform.localPosition = new Vector3(0f, 0.118f, -0.008f);
            infoGoldRim.transform.localScale = new Vector3(0.400f, 0.004f, 0.005f);
            infoGoldRim.GetComponent<Renderer>().sharedMaterial = goldMat;
            var igCol = infoGoldRim.GetComponent<Collider>(); if (igCol != null) DestroyImmediate(igCol);

            // Botón "✕ Cerrar" en la esquina superior derecha del pergamino
            spatialMenu.btnCloseInfo = Create3DTouchButton(infoCard.transform, "Btn_CloseInfo", "✕ Cerrar",
                new Vector3(0.168f, 0.138f, -0.014f), new Vector3(0.068f, 0.028f, 0.016f), buttonMat, new Color(1.0f, 0.85f, 0.85f), 10);

            // Canvas en alta resolución (dynamicPixelsPerUnit = 5f, escala 0.00035f para eliminar toda borrosidad en Quest 3)
            GameObject infoCanvasObj = new GameObject("InfoCard_Canvas");
            infoCanvasObj.transform.SetParent(infoCard.transform, false);
            infoCanvasObj.transform.localPosition = new Vector3(0f, 0f, -0.009f);
            infoCanvasObj.transform.localScale = Vector3.one * 0.00035f;
            Canvas iCanvas = infoCanvasObj.AddComponent<Canvas>();
            iCanvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler iScaler = infoCanvasObj.AddComponent<CanvasScaler>();
            iScaler.dynamicPixelsPerUnit = 5f;

            GameObject iTitleObj = new GameObject("InfoTitle");
            iTitleObj.transform.SetParent(infoCanvasObj.transform, false);
            iTitleObj.transform.localPosition = new Vector3(-80f, 395f, 0f);
            Text iTitle = iTitleObj.AddComponent<Text>();
            iTitle.font = uiFont;
            iTitle.alignment = TextAnchor.MiddleLeft;
            iTitle.fontSize = 32;
            iTitle.fontStyle = FontStyle.Bold;
            iTitle.color = new Color(1.0f, 0.78f, 0.88f);
            iTitleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(860, 60);
            spatialMenu.infoPanelTitle = iTitle;

            GameObject iBodyObj = new GameObject("InfoBody");
            iBodyObj.transform.SetParent(infoCanvasObj.transform, false);
            iBodyObj.transform.localPosition = new Vector3(0f, -32f, 0f);
            Text iBody = iBodyObj.AddComponent<Text>();
            iBody.font = uiFont;
            iBody.alignment = TextAnchor.UpperLeft;
            iBody.fontSize = 25;
            iBody.lineSpacing = 1.25f;
            iBody.color = new Color(0.96f, 0.95f, 0.92f);
            iBodyObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1120, 680);
            spatialMenu.infoPanelBody = iBody;

            infoCard.SetActive(false);

            // =========================================================================
            // 3. PERGAMINO DE OPCIONES DEL SANTUARIO (KAKEMONO JAPONÉS) FRENTE AL JUGADOR
            // Diseño unificado de pergamino colgante tradicional con barras Hinoki y remates Jiku dorados
            // =========================================================================
            GameObject optionsCard = new GameObject("OptionsPanel_Card");
            optionsCard.transform.SetParent(menuRoot.transform, false);
            optionsCard.transform.localPosition = new Vector3(0.0f, 0.18f, -0.16f);
            optionsCard.transform.localRotation = Quaternion.Euler(14f, 0f, 0f);
            spatialMenu.optionsPanelBox = optionsCard;

            // Marco exterior Urushi / Seda brocada de montaje
            GameObject optOuter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            optOuter.name = "OptionsCard_OuterFrame";
            optOuter.transform.SetParent(optionsCard.transform, false);
            optOuter.transform.localPosition = Vector3.zero;
            optOuter.transform.localScale = new Vector3(0.48f, 0.44f, 0.012f);
            optOuter.GetComponent<Renderer>().sharedMaterial = urushiMat;
            var ocCol = optOuter.GetComponent<Collider>(); if (ocCol != null) DestroyImmediate(ocCol);

            // Tablero interior Washi / Seda palisandro
            GameObject optInner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            optInner.name = "OptionsCard_InnerBoard";
            optInner.transform.SetParent(optionsCard.transform, false);
            optInner.transform.localPosition = new Vector3(0f, 0f, -0.003f);
            optInner.transform.localScale = new Vector3(0.456f, 0.416f, 0.009f);
            optInner.GetComponent<Renderer>().sharedMaterial = rosewoodMat;
            var oiCol = optInner.GetComponent<Collider>(); if (oiCol != null) DestroyImmediate(oiCol);

            // Barra superior del pergamino de Opciones (Kakemono Ten) en madera Hinoki con remates dorados Jiku
            GameObject optRodTop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            optRodTop.name = "OptionsKakemono_TopRod";
            optRodTop.transform.SetParent(optionsCard.transform, false);
            optRodTop.transform.localPosition = new Vector3(0f, 0.224f, -0.007f);
            optRodTop.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            optRodTop.transform.localScale = new Vector3(0.014f, 0.246f, 0.014f);
            optRodTop.GetComponent<Renderer>().sharedMaterial = hinokiMat;
            var ortCol = optRodTop.GetComponent<Collider>(); if (ortCol != null) DestroyImmediate(ortCol);

            float[] optJikuX = { -0.254f, 0.254f };
            for (int j = 0; j < 2; j++)
            {
                GameObject oJikuT = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                oJikuT.name = $"OptionsKakemono_TopFinial_{j}";
                oJikuT.transform.SetParent(optionsCard.transform, false);
                oJikuT.transform.localPosition = new Vector3(optJikuX[j], 0.224f, -0.007f);
                oJikuT.transform.localScale = Vector3.one * 0.020f;
                oJikuT.GetComponent<Renderer>().sharedMaterial = goldMat;
                var ojtCol = oJikuT.GetComponent<Collider>(); if (ojtCol != null) DestroyImmediate(ojtCol);
            }

            // Barra inferior contrapeso del pergamino de Opciones (Kakemono Chi)
            GameObject optRodBottom = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            optRodBottom.name = "OptionsKakemono_BottomRod";
            optRodBottom.transform.SetParent(optionsCard.transform, false);
            optRodBottom.transform.localPosition = new Vector3(0f, -0.224f, -0.007f);
            optRodBottom.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            optRodBottom.transform.localScale = new Vector3(0.016f, 0.246f, 0.016f);
            optRodBottom.GetComponent<Renderer>().sharedMaterial = hinokiMat;
            var orbCol = optRodBottom.GetComponent<Collider>(); if (orbCol != null) DestroyImmediate(orbCol);

            for (int j = 0; j < 2; j++)
            {
                GameObject oJikuB = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                oJikuB.name = $"OptionsKakemono_BottomFinial_{j}";
                oJikuB.transform.SetParent(optionsCard.transform, false);
                oJikuB.transform.localPosition = new Vector3(optJikuX[j], -0.224f, -0.007f);
                oJikuB.transform.localScale = Vector3.one * 0.022f;
                oJikuB.GetComponent<Renderer>().sharedMaterial = goldMat;
                var ojbCol = oJikuB.GetComponent<Collider>(); if (ojbCol != null) DestroyImmediate(ojbCol);
            }

            // Filete dorado Maki-e bajo la cabecera del pergamino
            GameObject optGoldRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            optGoldRim.name = "OptionsCard_GoldBorder";
            optGoldRim.transform.SetParent(optionsCard.transform, false);
            optGoldRim.transform.localPosition = new Vector3(0f, 0.146f, -0.008f);
            optGoldRim.transform.localScale = new Vector3(0.436f, 0.004f, 0.005f);
            optGoldRim.GetComponent<Renderer>().sharedMaterial = goldMat;
            var ogCol = optGoldRim.GetComponent<Collider>(); if (ogCol != null) DestroyImmediate(ogCol);

            // Botón "✕ Cerrar" en la esquina superior derecha del pergamino de opciones
            spatialMenu.btnCloseOptions = Create3DTouchButton(optionsCard.transform, "Btn_CloseOptions", "✕ Cerrar",
                new Vector3(0.184f, 0.178f, -0.014f), new Vector3(0.070f, 0.028f, 0.016f), buttonMat, new Color(1.0f, 0.85f, 0.85f), 10);

            // Canvas en alta resolución (dynamicPixelsPerUnit = 5f, escala 0.00035f)
            GameObject optCanvasObj = new GameObject("OptionsCard_Canvas");
            optCanvasObj.transform.SetParent(optionsCard.transform, false);
            optCanvasObj.transform.localPosition = new Vector3(0f, 0f, -0.009f);
            optCanvasObj.transform.localScale = Vector3.one * 0.00035f;
            Canvas oCanvas = optCanvasObj.AddComponent<Canvas>();
            oCanvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler oScaler = optCanvasObj.AddComponent<CanvasScaler>();
            oScaler.dynamicPixelsPerUnit = 5f;

            GameObject oTitleObj = new GameObject("OptionsTitle");
            oTitleObj.transform.SetParent(optCanvasObj.transform, false);
            oTitleObj.transform.localPosition = new Vector3(-75f, 518f, 0f);
            Text oTitle = oTitleObj.AddComponent<Text>();
            oTitle.font = uiFont;
            oTitle.alignment = TextAnchor.MiddleLeft;
            oTitle.fontSize = 29;
            oTitle.fontStyle = FontStyle.Bold;
            oTitle.text = "⚙️ PERGAMINO DE OPCIONES DEL SANTUARIO";
            oTitle.color = new Color(0.98f, 0.88f, 0.52f);
            oTitleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(920, 44);
            spatialMenu.optionsPanelTitle = oTitle;

            GameObject oSubtitleObj = new GameObject("OptionsSubtitle");
            oSubtitleObj.transform.SetParent(optCanvasObj.transform, false);
            oSubtitleObj.transform.localPosition = new Vector3(0f, 456f, 0f);
            Text oSubtitle = oSubtitleObj.AddComponent<Text>();
            oSubtitle.font = uiFont;
            oSubtitle.alignment = TextAnchor.MiddleCenter;
            oSubtitle.fontSize = 17;
            oSubtitle.fontStyle = FontStyle.Normal;
            oSubtitle.text = "Toca con el dedo índice para personalizar el idioma, música, entorno y ritmo de cuidado";
            oSubtitle.color = new Color(0.84f, 0.92f, 0.99f, 0.90f);
            oSubtitleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1200, 30);
            spatialMenu.optionsPanelSubtitle = oSubtitle;

            // Cuadrícula 2x5 de botones táctiles dentro del pergamino Kakemono (Columna Izq: x = -0.108m, Columna Der: x = +0.108m)
            Vector3 optBtnSize = new Vector3(0.194f, 0.046f, 0.018f);

            // Fila 1 (y = +0.104m): Mesa Fija vs Centrar Mesa
            spatialMenu.btnOptLockTable = Create3DTouchButton(optionsCard.transform, "Btn_OptLockTable", "🔓 Mesa:\nMóvil (2 Manos)",
                new Vector3(-0.108f, 0.104f, -0.014f), optBtnSize, buttonMat, Color.white, 10.5f);

            spatialMenu.btnOptRecenter = Create3DTouchButton(optionsCard.transform, "Btn_OptRecenter", "🎯 Centrar\nMesa Frente a Mí",
                new Vector3(0.108f, 0.104f, -0.014f), optBtnSize, infoButtonMat, new Color(0.92f, 0.99f, 1.0f), 10.5f);

            // Fila 2 (y = +0.046m): Idioma (ES / EN / JA) vs Velocidad / Ritmo de Cuidado
            spatialMenu.btnOptLanguage = Create3DTouchButton(optionsCard.transform, "Btn_OptLanguage", "🌐 Idioma:\nEspañol (ES)",
                new Vector3(-0.108f, 0.046f, -0.014f), optBtnSize, infoButtonMat, new Color(1.0f, 0.94f, 0.72f), 10.5f);

            spatialMenu.btnOptCareSpeed = Create3DTouchButton(optionsCard.transform, "Btn_OptCareSpeed", "⏳ Ritmo Cuidado:\nNormal (1.0x)",
                new Vector3(0.108f, 0.046f, -0.014f), optBtnSize, buttonMat, new Color(0.78f, 1.0f, 0.88f), 10.5f);

            // Fila 3 (y = -0.012m): Iluminación vs Entorno Virtual (Santuario Fuji / MR)
            spatialMenu.btnOptLighting = Create3DTouchButton(optionsCard.transform, "Btn_OptLighting", "🌅 Ambiente:\nDía Zen",
                new Vector3(-0.108f, -0.012f, -0.014f), optBtnSize, buttonMat, new Color(1.0f, 0.92f, 0.68f), 10.5f);

            spatialMenu.btnOptEnvironment = Create3DTouchButton(optionsCard.transform, "Btn_OptEnvironment", "🥽 Entorno:\nPassthrough MR",
                new Vector3(0.108f, -0.012f, -0.014f), optBtnSize, buttonMat, new Color(0.70f, 0.92f, 1.0f), 10.5f);

            // Fila 4 (y = -0.070m): Música Zen (7 pistas) vs Enfoque de Cuidado (1, 2 o 3 árboles)
            spatialMenu.btnOptMusic = Create3DTouchButton(optionsCard.transform, "Btn_OptMusic", "🎵 Música:\nFlauta Shakuhachi",
                new Vector3(-0.108f, -0.070f, -0.014f), optBtnSize, buttonMat, new Color(0.95f, 0.85f, 1.0f), 10.5f);

            spatialMenu.btnOptTreeFocus = Create3DTouchButton(optionsCard.transform, "Btn_OptTreeFocus", "🌿 Cuidado:\n3 Árboles (Tríada)",
                new Vector3(0.108f, -0.070f, -0.014f), optBtnSize, buttonMat, new Color(0.82f, 0.98f, 0.82f), 10.5f);

            // Fila 5 (y = -0.128m): Volumen de Sonido vs Lluvia de Pétalos de Sakura
            spatialMenu.btnOptSound = Create3DTouchButton(optionsCard.transform, "Btn_OptSound", "🔊 Sonido:\n100% (Normal)",
                new Vector3(-0.108f, -0.128f, -0.014f), optBtnSize, buttonMat, new Color(0.84f, 0.98f, 0.86f), 10.5f);

            spatialMenu.btnOptPetals = Create3DTouchButton(optionsCard.transform, "Btn_OptPetals", "🌸 Pétalos:\nActivados",
                new Vector3(0.108f, -0.128f, -0.014f), optBtnSize, buttonMat, new Color(1.0f, 0.82f, 0.90f), 10.5f);

            optionsCard.SetActive(false);

            // Construir los 3 Entornos Culturales Japoneses 360° de Alta Definición (PBR + Normales + 0 bloques primitivos)
            Material mountainMat = CreateOrGetSurfaceTexturedMaterial("Zen_FujiVolcanicMat",
                new Color(0.18f, 0.25f, 0.38f), new Color(0.10f, 0.14f, 0.24f), new Color(0.84f, 0.90f, 0.98f), 0.16f, 0.0f, 23);
            Material snowMat = CreateOrGetSurfaceTexturedMaterial("Zen_SnowGlacierMat",
                new Color(0.95f, 0.97f, 1.0f), new Color(0.76f, 0.84f, 0.94f), new Color(1.0f, 1.0f, 1.0f), 0.45f, 0.0f, 11);
            Material mistMat = CreateOrGetPBRMaterial("Zen_MistMat", new Color(0.88f, 0.91f, 0.97f), 0.10f, 0.0f);

            GameObject envFujiVeranda = BuildFujiVerandaSanctuary(parent, mountainMat, snowMat, mistMat, urushiMat, rosewoodMat, hinokiMat, stoneMat, goldMat, sakuraMat, matsuMat);
            GameObject envKyotoPagoda = BuildKyotoLakePagodaSanctuary(parent, mountainMat, snowMat, urushiMat, hinokiMat, stoneMat, goldMat, sakuraMat, momijiMat);
            GameObject envBambooTemple = BuildArashiyamaBambooTempleSanctuary(parent, mountainMat, urushiMat, hinokiMat, stoneMat, goldMat, matsuMat);

            spatialMenu.virtualSanctuaryBackdrop = envFujiVeranda;
            spatialMenu.virtualEnvironmentBackdrops = new GameObject[] { envFujiVeranda, envKyotoPagoda, envBambooTemple };
            spatialMenu.sakuraPetalsRain = BuildSakuraPetalsRain(parent, sakuraMat);

            // Activar por defecto el cerezo en flor e inicializar estado visual
            organicTree.ApplySpecies(TreeType.Sakura);
            spatialMenu.UpdateUI();
        }

        // =========================================================================
        // ENTORNO 1 (360° COMPLETO): JARDÍN IMPERIAL DEL MONTE FUJI Y SALÓN CHASHITSU / ENGAWA
        // Recreación fotorrealista 360°: Salón de té completo alrededor del jugador (suelo de tablones PBR,
        // tatamis con brocado, techo artesonado con farol Chōchin, Tokonoma con pergamino Sumi-e detrás del jugador,
        // estantería Chigaidana y lámpara Andon talladas a la izquierda) + Jardín 360° con estanque Koi, flores de loto,
        // cerezos Sakura y pinos Niwaki con follaje 3D, muros Kawara, Torii curvo, niebla volumétrica y Monte Fuji HD.
        // =========================================================================
        private static GameObject BuildFujiVerandaSanctuary(
            Transform parent,
            Material mountainMat, Material snowMat, Material mistMat,
            Material urushiMat, Material rosewoodMat, Material hinokiMat, Material stoneMat, Material goldMat,
            Material sakuraMat, Material pineNeedleMat)
        {
            GameObject sanctuaryRoot = new GameObject("ZenSanctuary_1_FujiVeranda");
            sanctuaryRoot.transform.SetParent(parent, false);
            sanctuaryRoot.transform.localPosition = Vector3.zero;

            // Materiales PBR de alta definición con mapas de normales para arquitectura y paisaje
            Material skyDomeMat = CreateOrGetPanoramicSkyDomeMaterial("Zen_SkyPanorama_FujiSunset",
                new Color(0.24f, 0.46f, 0.78f), new Color(0.58f, 0.72f, 0.90f), new Color(0.98f, 0.76f, 0.58f),
                new Color(1.0f, 0.92f, 0.80f), new Color(0.56f, 0.58f, 0.74f), new Color(0.22f, 0.30f, 0.46f), 101);

            Material sunMat = CreateOrGetPBRMaterial("Zen_SanctuarySunMat", new Color(1.0f, 0.93f, 0.76f), 0.9f, 0.0f);
            sunMat.EnableKeyword("_EMISSION");
            if (sunMat.HasProperty("_EmissionColor")) sunMat.SetColor("_EmissionColor", new Color(1.0f, 0.86f, 0.56f) * 1.85f);

            Material toriiRedMat = CreateOrGetSurfaceTexturedMaterial("Zen_ToriiVermilionPBRMat",
                new Color(0.80f, 0.16f, 0.10f), new Color(0.52f, 0.08f, 0.05f), new Color(0.95f, 0.32f, 0.20f), 0.68f, 0.06f, 17);
            Material pondMat = CreateOrGetSurfaceTexturedMaterial("Zen_SanctuaryPondPBRMat",
                new Color(0.09f, 0.32f, 0.42f), new Color(0.04f, 0.18f, 0.26f), new Color(0.32f, 0.62f, 0.72f), 0.96f, 0.22f, 24);
            Material bambooCulmMat = CreateOrGetSurfaceTexturedMaterial("Zen_BambooForestMat",
                new Color(0.24f, 0.48f, 0.18f), new Color(0.14f, 0.32f, 0.10f), new Color(0.40f, 0.66f, 0.28f), 0.52f, 0.0f, 15);
            Material mossGroundMat = CreateOrGetSurfaceTexturedMaterial("Zen_SanctuaryMossMat",
                new Color(0.18f, 0.42f, 0.14f), new Color(0.09f, 0.24f, 0.07f), new Color(0.32f, 0.58f, 0.20f), 0.20f, 0.0f, 11);
            Material kawaraTileMat = CreateOrGetSurfaceTexturedMaterial("Zen_KawaraTilePBRMat",
                new Color(0.17f, 0.19f, 0.23f), new Color(0.08f, 0.09f, 0.12f), new Color(0.38f, 0.42f, 0.48f), 0.72f, 0.22f, 19);
            Material shikkuiWallMat = CreateOrGetSurfaceTexturedMaterial("Zen_ShikkuiPlasterPBRMat",
                new Color(0.93f, 0.91f, 0.86f), new Color(0.76f, 0.73f, 0.68f), new Color(0.98f, 0.97f, 0.94f), 0.24f, 0.0f, 22);
            Material shojiPaperMat = CreateOrGetSurfaceTexturedMaterial("Zen_ShojiLatticePBRMat",
                new Color(0.96f, 0.92f, 0.82f), new Color(0.28f, 0.17f, 0.10f), new Color(1.0f, 0.97f, 0.90f), 0.22f, 0.0f, 20);
            shojiPaperMat.EnableKeyword("_EMISSION");
            if (shojiPaperMat.HasProperty("_EmissionColor")) shojiPaperMat.SetColor("_EmissionColor", new Color(0.36f, 0.30f, 0.22f));

            Material plankFloorMat = CreateOrGetSurfaceTexturedMaterial("Zen_HinokiPlankFloorMat",
                new Color(0.48f, 0.30f, 0.17f), new Color(0.26f, 0.15f, 0.08f), new Color(0.68f, 0.46f, 0.28f), 0.68f, 0.04f, 17);
            Material tatamiMat = CreateOrGetSurfaceTexturedMaterial("Zen_TatamiWeavePBRMat",
                new Color(0.58f, 0.56f, 0.32f), new Color(0.42f, 0.40f, 0.20f), new Color(0.74f, 0.70f, 0.44f), 0.28f, 0.0f, 18);
            Material templeStoneMat = CreateOrGetSurfaceTexturedMaterial("Zen_TempleStoneMasonryMat",
                new Color(0.42f, 0.43f, 0.45f), new Color(0.22f, 0.23f, 0.25f), new Color(0.62f, 0.64f, 0.66f), 0.25f, 0.05f, 21);
            Material sumieScrollMat = CreateOrGetSurfaceTexturedMaterial("Zen_TokonomaSumieScrollMat",
                new Color(0.92f, 0.89f, 0.82f), new Color(0.18f, 0.22f, 0.30f), new Color(0.98f, 0.96f, 0.90f), 0.25f, 0.0f, 25);
            Material realBarkMat = CreateOrGetBarkMaterial("Zen_SakuraBark_Mat",
                new Color(0.34f, 0.22f, 0.16f), new Color(0.16f, 0.10f, 0.07f), new Color(0.50f, 0.35f, 0.26f), 0.28f, 1);

            // 1. CÚPULA CELESTE PANORÁMICA 360° (GEODÉSICA CON NUBES VOLUMÉTRICAS Y CORDILLERAS PINTADAS)
            Mesh skySphereMesh = ZenBonsaiProceduralMeshes.CreatePanoramicSkySphereMesh("Mesh_PanoramicSkySphere_360", 42f, 64, 32);
            GameObject skyDome = new GameObject("Sanctuary_SkyDome");
            skyDome.transform.SetParent(sanctuaryRoot.transform, false);
            skyDome.AddComponent<MeshFilter>().sharedMesh = skySphereMesh;
            skyDome.AddComponent<MeshRenderer>().sharedMaterial = skyDomeMat;

            // 2. TERRENO CONTINUO 360° DE MUSGO Y COLINAS JAPONESAS (76m x 76m, CERO HUECOS EN NINGUNA DIRECCIÓN)
            Mesh terrain360Mesh = ZenBonsaiProceduralMeshes.CreateFull360SanctuaryTerrainMesh(
                "Mesh_FujiSanctuary_Terrain360", 76f, -0.82f, new Vector2(1.2f, 7.2f), new Vector2(6.4f, 4.8f), 0.42f, 101);
            GameObject mossGround = new GameObject("Sanctuary_MossLandscape360");
            mossGround.transform.SetParent(sanctuaryRoot.transform, false);
            mossGround.AddComponent<MeshFilter>().sharedMesh = terrain360Mesh;
            mossGround.AddComponent<MeshRenderer>().sharedMaterial = mossGroundMat;

            // 3. ANILLO MONTAÑOSO 360° ALREDEDOR DE TODO EL HORIZONTE + MONTE FUJI HD + BANCOS DE NIEBLA
            Mesh horizonRingMesh = ZenBonsaiProceduralMeshes.Create360MountainHorizonRingMesh(
                "Mesh_FujiSanctuary_HorizonRing360", 23f, 38f, 6.8f, 0.68f, 105);
            GameObject horizonRing = new GameObject("Sanctuary_360MountainRing");
            horizonRing.transform.SetParent(sanctuaryRoot.transform, false);
            horizonRing.AddComponent<MeshFilter>().sharedMesh = horizonRingMesh;
            horizonRing.AddComponent<MeshRenderer>().sharedMaterial = mountainMat;

            Mesh fujiRockMesh = ZenBonsaiProceduralMeshes.CreateDetailedMountFujiMesh("Mesh_HD_MountFuji_Volcano", 22f, 2.8f, 11.2f, out Mesh fujiSnowCapMesh);
            GameObject fujiBody = new GameObject("Distant_MountFuji_HD");
            fujiBody.transform.SetParent(sanctuaryRoot.transform, false);
            fujiBody.transform.localPosition = new Vector3(1.2f, -0.90f, 27.5f);
            fujiBody.AddComponent<MeshFilter>().sharedMesh = fujiRockMesh;
            fujiBody.AddComponent<MeshRenderer>().sharedMaterial = mountainMat;

            GameObject fujiSnow = new GameObject("Distant_MountFuji_SnowRidges");
            fujiSnow.transform.SetParent(fujiBody.transform, false);
            fujiSnow.AddComponent<MeshFilter>().sharedMesh = fujiSnowCapMesh;
            fujiSnow.AddComponent<MeshRenderer>().sharedMaterial = snowMat;

            Mesh mistBankMesh = ZenBonsaiProceduralMeshes.CreateVolumetricCloudBankMesh("Mesh_Fuji_MistCloudRing", 19.5f, 0.35f, 18, 111);
            GameObject mistRing = new GameObject("Sanctuary_MistCloudRing");
            mistRing.transform.SetParent(sanctuaryRoot.transform, false);
            mistRing.AddComponent<MeshFilter>().sharedMesh = mistBankMesh;
            mistRing.AddComponent<MeshRenderer>().sharedMaterial = mistMat;

            // 4. PABELLÓN DE TÉ JAPONÉS 360° COMPLETO (ENGAWA + SALÓN CHASHITSU + TOKONOMA DETRÁS DEL JUGADOR)
            BuildJapaneseTeaRoomVerandaFrame(
                sanctuaryRoot.transform, urushiMat, plankFloorMat, tatamiMat, shikkuiWallMat,
                shojiPaperMat, kawaraTileMat, templeStoneMat, sumieScrollMat, sunMat, goldMat, sakuraMat);

            // 5. MUROS DE JARDÍN IMPERIAL (TSUKIJI-BEI) CON MAMPOSTERÍA ISHIGAKI Y TEJADOS CURVOS KAWARA EN 360°
            Vector3[] wallPositions =
            {
                new Vector3(-5.2f, -0.82f, 6.2f),
                new Vector3(6.4f, -0.82f, 6.2f),
                new Vector3(-6.2f, -0.82f, -4.8f),
                new Vector3(6.2f, -0.82f, -4.8f)
            };
            float[] wallRotY = { 14f, -14f, 45f, -45f };
            Mesh wallRoofMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_WallKawaraRoof", 0.82f, 8.8f, 0.38f, 0.10f);
            Mesh wallStoneMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_WallStonePlinth", 0.46f, 8.5f, 0.52f, 0.0f);
            Mesh wallPlasterMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_WallPlasterBody", 0.32f, 8.4f, 1.15f, 0.0f);

            for (int w = 0; w < wallPositions.Length; w++)
            {
                GameObject gWall = new GameObject($"Sanctuary_GardenWall_{w}");
                gWall.transform.SetParent(sanctuaryRoot.transform, false);
                gWall.transform.localPosition = wallPositions[w];
                gWall.transform.localRotation = Quaternion.Euler(0f, wallRotY[w], 0f);

                GameObject wBase = new GameObject("Wall_IshigakiBase");
                wBase.transform.SetParent(gWall.transform, false);
                wBase.AddComponent<MeshFilter>().sharedMesh = wallStoneMesh;
                wBase.AddComponent<MeshRenderer>().sharedMaterial = templeStoneMat;

                GameObject wPlaster = new GameObject("Wall_ShikkuiBody");
                wPlaster.transform.SetParent(gWall.transform, false);
                wPlaster.transform.localPosition = new Vector3(0f, 0.48f, 0f);
                wPlaster.AddComponent<MeshFilter>().sharedMesh = wallPlasterMesh;
                wPlaster.AddComponent<MeshRenderer>().sharedMaterial = shikkuiWallMat;

                GameObject wRoof = new GameObject("Wall_KawaraRoof");
                wRoof.transform.SetParent(gWall.transform, false);
                wRoof.transform.localPosition = new Vector3(0f, 1.56f, 0f);
                wRoof.AddComponent<MeshFilter>().sharedMesh = wallRoofMesh;
                wRoof.AddComponent<MeshRenderer>().sharedMaterial = kawaraTileMat;
            }

            // 6. ESTANQUE ESPEJO KOI CON ONDAS, HOJAS Y FLORES DE LOTO 3D, SENDEROS TOBI-ISHI Y CARPAS KOI
            Mesh pondSurfaceMesh = ZenBonsaiProceduralMeshes.CreateOrganicMossMoundMesh("Mesh_FujiPond_WaterSheet", 13.5f, 10.2f, -0.78f, 0.004f);
            GameObject koiPond = new GameObject("Sanctuary_ReflectivePond");
            koiPond.transform.SetParent(sanctuaryRoot.transform, false);
            koiPond.transform.localPosition = new Vector3(1.2f, 0f, 7.2f);
            koiPond.AddComponent<MeshFilter>().sharedMesh = pondSurfaceMesh;
            koiPond.AddComponent<MeshRenderer>().sharedMaterial = pondMat;

            // Hojas y flores de loto flotando sobre el estanque
            Mesh lotusPadsMesh = ZenBonsaiProceduralMeshes.CreateSculptedLotusPadAndFlowerMesh("Mesh_Pond_LotusPads", 14, 303, out Mesh lotusFlowersMesh);
            GameObject lotusPadsObj = new GameObject("Pond_LotusPads");
            lotusPadsObj.transform.SetParent(sanctuaryRoot.transform, false);
            lotusPadsObj.transform.localPosition = new Vector3(1.2f, -0.775f, 6.8f);
            lotusPadsObj.AddComponent<MeshFilter>().sharedMesh = lotusPadsMesh;
            lotusPadsObj.AddComponent<MeshRenderer>().sharedMaterial = pineNeedleMat;

            GameObject lotusFlowersObj = new GameObject("Pond_LotusBlossoms");
            lotusFlowersObj.transform.SetParent(lotusPadsObj.transform, false);
            lotusFlowersObj.AddComponent<MeshFilter>().sharedMesh = lotusFlowersMesh;
            lotusFlowersObj.AddComponent<MeshRenderer>().sharedMaterial = sakuraMat;

            // Ondas concéntricas esculpidas en el agua
            Mesh rippleMesh = ZenBonsaiProceduralMeshes.CreateWaterRippleRingsMesh("Mesh_Pond_Ripples", 1.8f, 5);
            Vector3[] rippleCenters = { new Vector3(0.5f, -0.768f, 5.2f), new Vector3(2.2f, -0.768f, 7.4f), new Vector3(-0.4f, -0.768f, 8.1f) };
            for (int r = 0; r < rippleCenters.Length; r++)
            {
                GameObject rip = new GameObject($"Pond_Ripple_{r}");
                rip.transform.SetParent(sanctuaryRoot.transform, false);
                rip.transform.localPosition = rippleCenters[r];
                rip.AddComponent<MeshFilter>().sharedMesh = rippleMesh;
                rip.AddComponent<MeshRenderer>().sharedMaterial = snowMat;
            }

            // 7 Carpas Nishikigoi con aletas esculpidas nadando en el estanque
            Material koiOrangeMat = CreateOrGetPBRMaterial("Zen_KoiOrangeMat", new Color(0.96f, 0.34f, 0.08f), 0.88f, 0.0f);
            Material koiGoldMat = CreateOrGetPBRMaterial("Zen_KoiGoldMat", new Color(0.98f, 0.74f, 0.16f), 0.88f, 0.0f);
            Mesh koiMesh = ZenBonsaiProceduralMeshes.CreateSculptedKoiFishMesh("Mesh_Sanctuary_KoiFish");
            GameObject koiGroup = new GameObject("Sanctuary_KoiSwimmers");
            koiGroup.transform.SetParent(sanctuaryRoot.transform, false);
            for (int k = 0; k < 7; k++)
            {
                GameObject koi = new GameObject($"KoiCarp_{k}");
                koi.transform.SetParent(koiGroup.transform, false);
                koi.transform.localPosition = new Vector3(0.4f + k * 0.45f, -0.79f, 5.4f + (k % 3) * 0.9f);
                koi.AddComponent<MeshFilter>().sharedMesh = koiMesh;
                koi.AddComponent<MeshRenderer>().sharedMaterial = (k % 2 == 0) ? koiOrangeMat : koiGoldMat;
            }

            // Camino de piedras de paso (Tobi-ishi) y rocas ribereñas Suiseki alrededor de los 360°
            Vector3[] boulderPos =
            {
                new Vector3(-2.6f, -0.58f, 3.8f), new Vector3(-1.8f, -0.64f, 5.2f),
                new Vector3(3.2f, -0.48f, 5.6f),  new Vector3(4.2f, -0.35f, 6.8f),
                new Vector3(2.4f, -0.62f, 4.4f),  new Vector3(-2.4f, -0.52f, 8.4f),
                new Vector3(-4.8f, -0.55f, -2.5f), new Vector3(4.8f, -0.52f, -2.8f),
                new Vector3(-3.2f, -0.58f, -6.5f), new Vector3(3.5f, -0.55f, -6.8f),
                new Vector3(0.0f, -0.76f, 2.35f),  new Vector3(-0.35f, -0.76f, 3.05f), new Vector3(0.15f, -0.76f, 3.70f)
            };
            for (int i = 0; i < boulderPos.Length; i++)
            {
                bool isSteppingStone = (i >= 10);
                Vector3 rad = isSteppingStone
                    ? new Vector3(0.38f, 0.08f, 0.32f)
                    : new Vector3(0.95f + (i % 3) * 0.35f, 0.38f + (i % 2) * 0.26f, 0.82f + (i % 3) * 0.28f);
                GameObject rock = new GameObject($"Sanctuary_SculptedBoulder_{i}");
                rock.transform.SetParent(sanctuaryRoot.transform, false);
                rock.transform.localPosition = boulderPos[i];
                rock.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                    $"Mesh_FujiBoulder_{i}", rad, 1200 + i * 31);
                rock.AddComponent<MeshRenderer>().sharedMaterial = (!isSteppingStone && i % 2 == 0) ? mossGroundMat : templeStoneMat;
            }

            // 7. LINTERNAS DE PIEDRA TALLADAS KASUGA TŌRŌ EN EL JARDÍN FRONTAL Y PATIOS LATERALES/TRASERO (360°)
            Mesh lanternStoneMesh = ZenBonsaiProceduralMeshes.CreateSculptedStoneLanternMesh("Mesh_KasugaStoneLantern", out Mesh lanternLightMesh);
            Vector3[] lanternPositions =
            {
                new Vector3(-2.15f, -0.76f, 3.35f),
                new Vector3(2.85f, -0.68f, 6.45f),
                new Vector3(-4.2f, -0.76f, -3.6f),
                new Vector3(4.2f, -0.76f, -3.6f)
            };
            float[] lanternScales = { 1.05f, 0.90f, 0.95f, 0.95f };
            for (int l = 0; l < lanternPositions.Length; l++)
            {
                GameObject lantern = new GameObject($"Sanctuary_KasugaLantern_{l}");
                lantern.transform.SetParent(sanctuaryRoot.transform, false);
                lantern.transform.localPosition = lanternPositions[l];
                lantern.transform.localScale = Vector3.one * lanternScales[l];
                lantern.AddComponent<MeshFilter>().sharedMesh = lanternStoneMesh;
                lantern.AddComponent<MeshRenderer>().sharedMaterial = templeStoneMat;

                GameObject lGlow = new GameObject("Lantern_FireBoxGlow");
                lGlow.transform.SetParent(lantern.transform, false);
                lGlow.AddComponent<MeshFilter>().sharedMesh = lanternLightMesh;
                lGlow.AddComponent<MeshRenderer>().sharedMaterial = sunMat;
            }

            // 8. ÁRBOLES MONUMENTALES DE JARDÍN CON CORTEZA PBR Y FLORES/ACÍCULAS 3D EN 360°
            BuildGardenLandscapeTree(sanctuaryRoot.transform, "Sanctuary_RightNiwakiPine",
                new Vector3(3.8f, -0.65f, 6.2f), 2.9f, realBarkMat, pineNeedleMat, goldMat, TreeType.Matsu, 1410);
            BuildGardenLandscapeTree(sanctuaryRoot.transform, "Sanctuary_LeftSakuraTree",
                new Vector3(-3.6f, -0.72f, 5.8f), 3.1f, realBarkMat, sakuraMat, goldMat, TreeType.Sakura, 1420);
            BuildGardenLandscapeTree(sanctuaryRoot.transform, "Sanctuary_RearLeftSakuraTree",
                new Vector3(-4.5f, -0.75f, -5.8f), 3.0f, realBarkMat, sakuraMat, goldMat, TreeType.Sakura, 1430);
            BuildGardenLandscapeTree(sanctuaryRoot.transform, "Sanctuary_RearRightPineTree",
                new Vector3(4.6f, -0.75f, -5.6f), 2.8f, realBarkMat, pineNeedleMat, goldMat, TreeType.Matsu, 1440);

            // 9. BOSQUECILLOS DE BAMBÚ ARASHIYAMA ENVOLVIENDO LOS LATERALES Y EL PATIO TRASERO (360°)
            Mesh bambooClusterMesh = ZenBonsaiProceduralMeshes.CreateBambooGroveClusterMesh("Mesh_BambooGroveCluster", out Mesh bambooLeavesMesh);
            Vector3[] grovePositions =
            {
                new Vector3(-3.5f, -0.78f, 3.8f),  new Vector3(-4.2f, -0.78f, 6.5f),  new Vector3(-3.8f, -0.78f, 9.4f),
                new Vector3(4.5f, -0.78f, 4.4f),   new Vector3(5.0f, -0.78f, 7.6f),   new Vector3(4.4f, -0.78f, 10.6f),
                new Vector3(-5.4f, -0.78f, -1.2f), new Vector3(5.4f, -0.78f, -1.2f),
                new Vector3(-3.6f, -0.78f, -7.8f), new Vector3(0.0f, -0.78f, -8.5f),  new Vector3(3.6f, -0.78f, -7.8f)
            };
            for (int g = 0; g < grovePositions.Length; g++)
            {
                GameObject grove = new GameObject($"Sanctuary_BambooClump_{g}");
                grove.transform.SetParent(sanctuaryRoot.transform, false);
                grove.transform.localPosition = grovePositions[g];
                grove.transform.localRotation = Quaternion.Euler(0f, g * 47f, 0f);
                grove.AddComponent<MeshFilter>().sharedMesh = bambooClusterMesh;
                grove.AddComponent<MeshRenderer>().sharedMaterial = bambooCulmMat;

                GameObject leaves = new GameObject("Bamboo_CanopyLeaves");
                leaves.transform.SetParent(grove.transform, false);
                leaves.AddComponent<MeshFilter>().sharedMesh = bambooLeavesMesh;
                leaves.AddComponent<MeshRenderer>().sharedMaterial = pineNeedleMat;
            }

            // 10. PUERTA SAGRADA MYŌJIN TORII ESCULPIDA EN EL ESTANQUE FRONTAL
            Mesh toriiRedMesh = ZenBonsaiProceduralMeshes.CreateSculptedMyojinToriiMesh("Mesh_MyojinTorii_Main", out Mesh toriiDarkCapsMesh);
            GameObject toriiObj = new GameObject("Sanctuary_SculptedToriiGate");
            toriiObj.transform.SetParent(sanctuaryRoot.transform, false);
            toriiObj.transform.localPosition = new Vector3(1.35f, -0.78f, 10.8f);
            toriiObj.transform.localRotation = Quaternion.Euler(0f, -6f, 0f);
            toriiObj.AddComponent<MeshFilter>().sharedMesh = toriiRedMesh;
            toriiObj.AddComponent<MeshRenderer>().sharedMaterial = toriiRedMat;

            GameObject toriiCaps = new GameObject("Torii_ShimakiAndBases");
            toriiCaps.transform.SetParent(toriiObj.transform, false);
            toriiCaps.AddComponent<MeshFilter>().sharedMesh = toriiDarkCapsMesh;
            toriiCaps.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;

            sanctuaryRoot.SetActive(false);
            return sanctuaryRoot;
        }

        private static void BuildJapaneseTeaRoomVerandaFrame(
            Transform parent,
            Material urushiMat, Material plankFloorMat, Material tatamiMat, Material wallMat,
            Material shojiPaperMat, Material kawaraRoofMat, Material stonePlinthMat, Material sumieScrollMat,
            Material lampMat, Material goldMat, Material sakuraMat)
        {
            GameObject roomRoot = new GameObject("TeaRoom_EngawaPortal360");
            roomRoot.transform.SetParent(parent, false);
            roomRoot.transform.localPosition = Vector3.zero;

            // 1. SUELO ARTESANAL DE TABLONES DE CIPRÉS HINOKI (ENGAWA PERIMETRAL 360°) Y ESTERAS TATAMI CON BROCADO
            Mesh floorDeckMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_TeaRoom_HardwoodFloor", 6.8f, 5.6f, 0.04f, 0.0f);
            GameObject woodFloor = new GameObject("TeaRoom_HardwoodFloor");
            woodFloor.transform.SetParent(roomRoot.transform, false);
            woodFloor.transform.localPosition = new Vector3(0f, -0.76f, -0.65f);
            woodFloor.AddComponent<MeshFilter>().sharedMesh = floorDeckMesh;
            woodFloor.AddComponent<MeshRenderer>().sharedMaterial = plankFloorMat;

            Mesh tatamiMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_TeaRoom_TatamiMat", 1.85f, 0.92f, 0.018f, 0.0f);
            Vector3[] tatamiPos =
            {
                new Vector3(-0.95f, -0.725f, 0.05f),  new Vector3(0.95f, -0.725f, 0.05f),
                new Vector3(-0.95f, -0.725f, -0.90f), new Vector3(0.95f, -0.725f, -0.90f),
                new Vector3(-0.95f, -0.725f, -1.85f), new Vector3(0.95f, -0.725f, -1.85f)
            };
            for (int t = 0; t < tatamiPos.Length; t++)
            {
                GameObject tatami = new GameObject($"TeaRoom_WovenTatami_{t}");
                tatami.transform.SetParent(roomRoot.transform, false);
                tatami.transform.localPosition = tatamiPos[t];
                tatami.AddComponent<MeshFilter>().sharedMesh = tatamiMesh;
                tatami.AddComponent<MeshRenderer>().sharedMaterial = tatamiMat;
            }

            // 2. COLUMNAS TORNEADAS DE CEDRO HINOKI EN LOS 360° DEL SALÓN (CON BASAS DE PIEDRA SOSEKI Y MÉNSULAS KUMIMONO)
            Mesh pillarMesh = ZenBonsaiProceduralMeshes.CreateSculptedTurnedPillarMesh(
                "Mesh_TeaRoom_TurnedPillar", 0.095f, 3.15f, out Mesh plinthMesh, out Mesh corbelMesh);

            Vector3[] pillarCoords =
            {
                new Vector3(-2.45f, -0.74f, 1.55f),  new Vector3(2.45f, -0.74f, 1.55f),   // Frente (Ventanal al Monte Fuji)
                new Vector3(-3.15f, -0.74f, 1.55f),  new Vector3(3.15f, -0.74f, 1.55f),   // Esquinas delanteras Engawa
                new Vector3(-3.15f, -0.74f, -0.65f), new Vector3(3.15f, -0.74f, -0.65f),  // Laterales medios
                new Vector3(-3.15f, -0.74f, -2.85f), new Vector3(3.15f, -0.74f, -2.85f),  // Esquinas traseras
                new Vector3(-1.15f, -0.74f, -2.85f), new Vector3(1.15f, -0.74f, -2.85f)   // Pilares del Tokonoma trasero
            };
            for (int p = 0; p < pillarCoords.Length; p++)
            {
                GameObject pObj = new GameObject($"TeaRoom_TurnedPillar_{p}");
                pObj.transform.SetParent(roomRoot.transform, false);
                pObj.transform.localPosition = pillarCoords[p];
                pObj.AddComponent<MeshFilter>().sharedMesh = pillarMesh;
                pObj.AddComponent<MeshRenderer>().sharedMaterial = plankFloorMat;

                GameObject plinth = new GameObject("Pillar_StonePlinth");
                plinth.transform.SetParent(pObj.transform, false);
                plinth.AddComponent<MeshFilter>().sharedMesh = plinthMesh;
                plinth.AddComponent<MeshRenderer>().sharedMaterial = stonePlinthMat;

                GameObject corbel = new GameObject("Pillar_KumimonoCorbel");
                corbel.transform.SetParent(pObj.transform, false);
                corbel.AddComponent<MeshFilter>().sharedMesh = corbelMesh;
                corbel.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;
            }

            // 3. PANELES CORREDEROS SHOJI ESCULPIDOS EN 3D (ABIERTOS EN EL FRENTE Y ENMARCANDO LATERALES Y FONDO 360°)
            Mesh shojiFrameMesh = ZenBonsaiProceduralMeshes.CreateSculptedShojiScreenMesh(
                "Mesh_TeaRoom_ShojiScreen", 1.12f, 2.85f, 4, 7, out Mesh shojiPaperMesh);

            Vector3[] shojiPositions =
            {
                new Vector3(-2.82f, -0.60f, 1.55f),  new Vector3(2.82f, -0.60f, 1.55f),   // Frente abiertos hacia los lados
                new Vector3(-3.15f, -0.60f, 0.55f),  new Vector3(3.15f, -0.60f, 0.55f),   // Laterales delanteros
                new Vector3(-3.15f, -0.60f, -1.75f), new Vector3(3.15f, -0.60f, -1.75f),  // Laterales traseros
                new Vector3(-2.15f, -0.60f, -2.85f), new Vector3(2.15f, -0.60f, -2.85f)   // Traseros flanqueando el Tokonoma
            };
            float[] shojiRotY = { 0f, 0f, 90f, 90f, 90f, 90f, 0f, 0f };
            for (int s = 0; s < shojiPositions.Length; s++)
            {
                GameObject shoji = new GameObject($"TeaRoom_SculptedShoji_{s}");
                shoji.transform.SetParent(roomRoot.transform, false);
                shoji.transform.localPosition = shojiPositions[s];
                shoji.transform.localRotation = Quaternion.Euler(0f, shojiRotY[s], 0f);
                shoji.AddComponent<MeshFilter>().sharedMesh = shojiFrameMesh;
                shoji.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;

                GameObject paper = new GameObject("Shoji_WashiInsert");
                paper.transform.SetParent(shoji.transform, false);
                paper.AddComponent<MeshFilter>().sharedMesh = shojiPaperMesh;
                paper.AddComponent<MeshRenderer>().sharedMaterial = shojiPaperMat;
            }

            // 4. ALCOBA SAGRADA TOKONOMA DETRÁS DEL JUGADOR (z = -2.85m, PARA QUE AL GIRAR 180° VEA UN INTERIOR JAPONÉS REAL)
            GameObject tokonomaRoot = new GameObject("TeaRoom_RearTokonomaAlcove");
            tokonomaRoot.transform.SetParent(roomRoot.transform, false);
            tokonomaRoot.transform.localPosition = new Vector3(0f, -0.68f, -2.85f);

            Mesh backWallMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_Tokonoma_BackWall", 2.35f, 0.12f, 2.95f, 0.0f);
            GameObject tokoWall = new GameObject("Tokonoma_ShikkuiWall");
            tokoWall.transform.SetParent(tokonomaRoot.transform, false);
            tokoWall.AddComponent<MeshFilter>().sharedMesh = backWallMesh;
            tokoWall.AddComponent<MeshRenderer>().sharedMaterial = wallMat;

            // Pergamino colgante Kakemono pintado con tinta Sumi-e del Monte Fuji en el centro del Tokonoma
            Mesh scrollFrameMesh = ZenBonsaiProceduralMeshes.CreateSculptedShojiScreenMesh(
                "Mesh_Tokonoma_Kakemono", 0.78f, 1.65f, 1, 1, out Mesh scrollPaintingMesh);
            GameObject kakemono = new GameObject("Tokonoma_HangingSumieScroll");
            kakemono.transform.SetParent(tokonomaRoot.transform, false);
            kakemono.transform.localPosition = new Vector3(0f, 0.68f, 0.08f);
            kakemono.AddComponent<MeshFilter>().sharedMesh = scrollFrameMesh;
            kakemono.AddComponent<MeshRenderer>().sharedMaterial = goldMat;

            GameObject kPainting = new GameObject("Kakemono_SumieArtwork");
            kPainting.transform.SetParent(kakemono.transform, false);
            kPainting.AddComponent<MeshFilter>().sharedMesh = scrollPaintingMesh;
            kPainting.AddComponent<MeshRenderer>().sharedMaterial = sumieScrollMat;

            // 5. TECHO ARTESONADO JAPONÉS (GŌTENJŌ), TEJADO CURVO KAWARA Y FAROLILLOS COLGANTES CHŌCHIN
            Mesh pavilionRoofMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_TeaRoom_MainRoof", 7.8f, 6.8f, 1.35f, 0.34f);
            GameObject mainRoof = new GameObject("TeaRoom_OverhangingKawaraRoof");
            mainRoof.transform.SetParent(roomRoot.transform, false);
            mainRoof.transform.localPosition = new Vector3(0f, 2.40f, -0.65f);
            mainRoof.AddComponent<MeshFilter>().sharedMesh = pavilionRoofMesh;
            mainRoof.AddComponent<MeshRenderer>().sharedMaterial = kawaraRoofMat;

            Mesh chochinFrameMesh = ZenBonsaiProceduralMeshes.CreateSculptedHangingChochinLanternMesh("Mesh_HangingChochin", out Mesh chochinGlowMesh);
            GameObject hangingRoot = new GameObject("Sanctuary_HangingLanterns");
            hangingRoot.transform.SetParent( parent, false);
            Vector3[] chochinPos =
            {
                new Vector3(-1.85f, 2.38f, 1.35f),
                new Vector3(1.85f, 2.38f, 1.35f),
                new Vector3(-1.65f, 2.38f, -2.15f),
                new Vector3(1.65f, 2.38f, -2.15f)
            };
            for (int c = 0; c < chochinPos.Length; c++)
            {
                GameObject hl = new GameObject($"HangingChochin_{c}");
                hl.transform.SetParent(hangingRoot.transform, false);
                hl.transform.localPosition = chochinPos[c];
                hl.AddComponent<MeshFilter>().sharedMesh = chochinFrameMesh;
                hl.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;

                GameObject hGlow = new GameObject("Chochin_SilkGlow");
                hGlow.transform.SetParent(hl.transform, false);
                hGlow.AddComponent<MeshFilter>().sharedMesh = chochinGlowMesh;
                hGlow.AddComponent<MeshRenderer>().sharedMaterial = lampMat;
            }

            // 6. ESTANTERÍA ESCALONADA CHIGAIDANA Y LÁMPARA DE PAPEL ANDON TALLADAS A LA IZQUIERDA (COMO EN LA IMAGEN)
            Mesh chigaidanaMesh = ZenBonsaiProceduralMeshes.CreateSculptedChigaidanaCabinetMesh("Mesh_TeaRoom_Chigaidana", out Mesh chigaidanaBrassMesh);
            GameObject leftCabinet = new GameObject("TeaRoom_LeftChigaidanaShelf");
            leftCabinet.transform.SetParent(roomRoot.transform, false);
            leftCabinet.transform.localPosition = new Vector3(-2.35f, -0.72f, 0.42f);
            leftCabinet.AddComponent<MeshFilter>().sharedMesh = chigaidanaMesh;
            leftCabinet.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;

            GameObject cabBrass = new GameObject("Chigaidana_BrassAndVase");
            cabBrass.transform.SetParent(leftCabinet.transform, false);
            cabBrass.AddComponent<MeshFilter>().sharedMesh = chigaidanaBrassMesh;
            cabBrass.AddComponent<MeshRenderer>().sharedMaterial = goldMat;

            Mesh andonFrameMesh = ZenBonsaiProceduralMeshes.CreateSculptedAndonLampMesh("Mesh_TeaRoom_AndonLamp", out Mesh andonGlowMesh);
            Vector3[] andonPositions = { new Vector3(-2.32f, 0.14f, 0.65f), new Vector3(2.35f, -0.72f, -2.25f) };
            for (int a = 0; a < andonPositions.Length; a++)
            {
                GameObject andon = new GameObject($"TeaRoom_SculptedAndonLamp_{a}");
                andon.transform.SetParent(roomRoot.transform, false);
                andon.transform.localPosition = andonPositions[a];
                andon.AddComponent<MeshFilter>().sharedMesh = andonFrameMesh;
                andon.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;

                GameObject aGlow = new GameObject("Andon_WashiCore");
                aGlow.transform.SetParent(andon.transform, false);
                aGlow.AddComponent<MeshFilter>().sharedMesh = andonGlowMesh;
                aGlow.AddComponent<MeshRenderer>().sharedMaterial = lampMat;
            }
        }

        // =========================================================================
        // ENTORNO 2 (360° COMPLETO): PABELLÓN FLOTANTE DE KIOTO, LAGO ESPEJO 360° Y PAGODA DE 5 PISOS
        // El jugador está dentro de un pabellón imperial sobre el agua (Tsuri-dono) con columnas torneadas,
        // barandillas bermellón Kōran con remates de bronce Giboshi, techo curvo Kawara, farolillos colgantes
        // y pasarela sagrada trasera con puerta Karamon, rodeado en 360° por el lago, islas, cerezos y arces.
        // =========================================================================
        private static GameObject BuildKyotoLakePagodaSanctuary(
            Transform parent,
            Material mountainMat, Material snowMat,
            Material urushiMat, Material hinokiMat, Material stoneMat, Material goldMat,
            Material sakuraMat, Material momijiMat)
        {
            GameObject root = new GameObject("ZenSanctuary_2_KyotoLakePagoda");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = Vector3.zero;

            Material skyDomeMat = CreateOrGetPanoramicSkyDomeMaterial("Zen_SkyPanorama_KyotoTwilight",
                new Color(0.20f, 0.36f, 0.68f), new Color(0.56f, 0.58f, 0.82f), new Color(0.98f, 0.64f, 0.48f),
                new Color(1.0f, 0.86f, 0.72f), new Color(0.48f, 0.44f, 0.64f), new Color(0.20f, 0.24f, 0.38f), 202);

            Material sunMat = CreateOrGetPBRMaterial("Zen_SanctuarySunMat", new Color(1.0f, 0.90f, 0.70f), 0.9f, 0.0f);
            sunMat.EnableKeyword("_EMISSION");
            if (sunMat.HasProperty("_EmissionColor")) sunMat.SetColor("_EmissionColor", new Color(1.0f, 0.82f, 0.50f) * 1.75f);

            Material toriiRedMat = CreateOrGetSurfaceTexturedMaterial("Zen_ToriiVermilionPBRMat",
                new Color(0.80f, 0.16f, 0.10f), new Color(0.52f, 0.08f, 0.05f), new Color(0.95f, 0.32f, 0.20f), 0.68f, 0.06f, 17);
            Material lakeMat = CreateOrGetSurfaceTexturedMaterial("Zen_SanctuaryPondPBRMat",
                new Color(0.09f, 0.32f, 0.42f), new Color(0.04f, 0.18f, 0.26f), new Color(0.32f, 0.62f, 0.72f), 0.96f, 0.22f, 24);
            Material kawaraMat = CreateOrGetSurfaceTexturedMaterial("Zen_KawaraTilePBRMat",
                new Color(0.17f, 0.19f, 0.23f), new Color(0.08f, 0.09f, 0.12f), new Color(0.38f, 0.42f, 0.48f), 0.72f, 0.22f, 19);
            Material mossMat = CreateOrGetSurfaceTexturedMaterial("Zen_SanctuaryMossMat",
                new Color(0.18f, 0.42f, 0.14f), new Color(0.09f, 0.24f, 0.07f), new Color(0.32f, 0.58f, 0.20f), 0.20f, 0.0f, 11);
            Material plankFloorMat = CreateOrGetSurfaceTexturedMaterial("Zen_HinokiPlankFloorMat",
                new Color(0.48f, 0.30f, 0.17f), new Color(0.26f, 0.15f, 0.08f), new Color(0.68f, 0.46f, 0.28f), 0.68f, 0.04f, 17);
            Material shojiPaperMat = CreateOrGetSurfaceTexturedMaterial("Zen_ShojiLatticePBRMat",
                new Color(0.96f, 0.92f, 0.82f), new Color(0.28f, 0.17f, 0.10f), new Color(1.0f, 0.97f, 0.90f), 0.22f, 0.0f, 20);
            shojiPaperMat.EnableKeyword("_EMISSION");
            if (shojiPaperMat.HasProperty("_EmissionColor")) shojiPaperMat.SetColor("_EmissionColor", new Color(0.42f, 0.34f, 0.22f));
            Material templeStoneMat = CreateOrGetSurfaceTexturedMaterial("Zen_TempleStoneMasonryMat",
                new Color(0.42f, 0.43f, 0.45f), new Color(0.22f, 0.23f, 0.25f), new Color(0.62f, 0.64f, 0.66f), 0.25f, 0.05f, 21);
            Material realBarkMat = CreateOrGetBarkMaterial("Zen_SakuraBark_Mat",
                new Color(0.34f, 0.22f, 0.16f), new Color(0.16f, 0.10f, 0.07f), new Color(0.50f, 0.35f, 0.26f), 0.28f, 1);

            // 1. CÚPULA CELESTE PANORÁMICA 360° + TERRENO RIBEREÑO 360° + LAGO SAGRADO REFLECTANTE 360°
            Mesh skySphereMesh = ZenBonsaiProceduralMeshes.CreatePanoramicSkySphereMesh("Mesh_PanoramicSkySphere_360", 42f, 64, 32);
            GameObject skyDome = new GameObject("Sanctuary_SkyDome");
            skyDome.transform.SetParent(root.transform, false);
            skyDome.AddComponent<MeshFilter>().sharedMesh = skySphereMesh;
            skyDome.AddComponent<MeshRenderer>().sharedMaterial = skyDomeMat;

            Mesh kyotoTerrainMesh = ZenBonsaiProceduralMeshes.CreateFull360SanctuaryTerrainMesh(
                "Mesh_KyotoSanctuary_Terrain360", 76f, -0.78f, new Vector2(0f, 5.5f), new Vector2(18f, 16f), 0.38f, 202);
            GameObject kyotoShore = new GameObject("Kyoto_ShoreLandscape360");
            kyotoShore.transform.SetParent(root.transform, false);
            kyotoShore.AddComponent<MeshFilter>().sharedMesh = kyotoTerrainMesh;
            kyotoShore.AddComponent<MeshRenderer>().sharedMaterial = mossMat;

            Mesh lakeSheetMesh = ZenBonsaiProceduralMeshes.CreateOrganicMossMoundMesh("Mesh_Kyoto_LakeWaterSheet360", 54f, 54f, -0.82f, 0.005f);
            GameObject sacredLake = new GameObject("Kyoto_SacredMirrorLake360");
            sacredLake.transform.SetParent(root.transform, false);
            sacredLake.transform.localPosition = Vector3.zero;
            sacredLake.AddComponent<MeshFilter>().sharedMesh = lakeSheetMesh;
            sacredLake.AddComponent<MeshRenderer>().sharedMaterial = lakeMat;

            Mesh kyotoMountainsMesh = ZenBonsaiProceduralMeshes.Create360MountainHorizonRingMesh(
                "Mesh_KyotoSanctuary_HorizonRing360", 24f, 39f, 7.2f, 0.15f, 209);
            GameObject kyotoHorizon = new GameObject("Kyoto_360MountainRing");
            kyotoHorizon.transform.SetParent(root.transform, false);
            kyotoHorizon.AddComponent<MeshFilter>().sharedMesh = kyotoMountainsMesh;
            kyotoHorizon.AddComponent<MeshRenderer>().sharedMaterial = mountainMat;

            // 2. PABELLÓN FLOTANTE IMPERIAL 360° (TSURI-DONO) ALREDEDOR DEL JUGADOR (CON BARANDILLAS KŌRAN Y TECHO CURVO)
            GameObject pavilionRoot = new GameObject("Kyoto_OverwaterPavilion360");
            pavilionRoot.transform.SetParent(root.transform, false);

            Mesh deckMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_Kyoto_PavilionDeck", 5.6f, 5.2f, 0.06f, 0.0f);
            GameObject pavilionDeck = new GameObject("Kyoto_WaterPavilionDeck");
            pavilionDeck.transform.SetParent(pavilionRoot.transform, false);
            pavilionDeck.transform.localPosition = new Vector3(0f, -0.76f, -0.45f);
            pavilionDeck.AddComponent<MeshFilter>().sharedMesh = deckMesh;
            pavilionDeck.AddComponent<MeshRenderer>().sharedMaterial = plankFloorMat;

            // Columnas torneadas en bermellón y piedra sumergida + Techo imperial Irimoya sobre el jugador
            Mesh pillarMesh = ZenBonsaiProceduralMeshes.CreateSculptedTurnedPillarMesh(
                "Mesh_TeaRoom_TurnedPillar", 0.095f, 3.15f, out Mesh plinthMesh, out Mesh corbelMesh);
            Vector3[] pavPillars =
            {
                new Vector3(-2.45f, -0.74f, 1.85f),  new Vector3(2.45f, -0.74f, 1.85f),
                new Vector3(-2.45f, -0.74f, -2.75f), new Vector3(2.45f, -0.74f, -2.75f),
                new Vector3(-1.10f, -0.74f, -2.75f), new Vector3(1.10f, -0.74f, -2.75f)
            };
            for (int p = 0; p < pavPillars.Length; p++)
            {
                GameObject col = new GameObject($"Pavilion_VermilionPillar_{p}");
                col.transform.SetParent(pavilionRoot.transform, false);
                col.transform.localPosition = pavPillars[p];
                col.AddComponent<MeshFilter>().sharedMesh = pillarMesh;
                col.AddComponent<MeshRenderer>().sharedMaterial = toriiRedMat;

                GameObject pl = new GameObject("Pillar_Plinth");
                pl.transform.SetParent(col.transform, false);
                pl.AddComponent<MeshFilter>().sharedMesh = plinthMesh;
                pl.AddComponent<MeshRenderer>().sharedMaterial = templeStoneMat;

                GameObject cb = new GameObject("Pillar_Corbel");
                cb.transform.SetParent(col.transform, false);
                cb.AddComponent<MeshFilter>().sharedMesh = corbelMesh;
                cb.AddComponent<MeshRenderer>().sharedMaterial = goldMat;
            }

            // Barandillas tradicionales Kōran con remates de bronce Giboshi en los laterales y trasera del pabellón
            Mesh koranSideMesh = ZenBonsaiProceduralMeshes.CreateSculptedKoranBalustradeMesh(
                "Mesh_Kyoto_KoranSide", 4.5f, 0.56f, 5, out Mesh giboshiSideMesh);
            Vector3[] railPositions = { new Vector3(-2.55f, -0.72f, -0.45f), new Vector3(2.55f, -0.72f, -0.45f) };
            for (int r = 0; r < 2; r++)
            {
                GameObject rail = new GameObject($"Pavilion_KoranBalustrade_{r}");
                rail.transform.SetParent(pavilionRoot.transform, false);
                rail.transform.localPosition = railPositions[r];
                rail.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                rail.AddComponent<MeshFilter>().sharedMesh = koranSideMesh;
                rail.AddComponent<MeshRenderer>().sharedMaterial = toriiRedMat;

                GameObject gib = new GameObject("Koran_GoldenGiboshi");
                gib.transform.SetParent(rail.transform, false);
                gib.AddComponent<MeshFilter>().sharedMesh = giboshiSideMesh;
                gib.AddComponent<MeshRenderer>().sharedMaterial = goldMat;
            }

            // Santuario y Puerta Karamon iluminada detrás del jugador (z = -2.75m, vista trasera 180° espectacular)
            Mesh shojiFrameMesh = ZenBonsaiProceduralMeshes.CreateSculptedShojiScreenMesh(
                "Mesh_TeaRoom_ShojiScreen", 1.12f, 2.85f, 4, 7, out Mesh shojiPaperMesh);
            float[] rearShojiX = { -1.78f, 0.0f, 1.78f };
            for (int s = 0; s < rearShojiX.Length; s++)
            {
                GameObject shoji = new GameObject($"Pavilion_RearShoji_{s}");
                shoji.transform.SetParent(pavilionRoot.transform, false);
                shoji.transform.localPosition = new Vector3(rearShojiX[s], -0.68f, -2.75f);
                shoji.AddComponent<MeshFilter>().sharedMesh = shojiFrameMesh;
                shoji.AddComponent<MeshRenderer>().sharedMaterial = toriiRedMat;

                GameObject paper = new GameObject("Shoji_Glow");
                paper.transform.SetParent(shoji.transform, false);
                paper.AddComponent<MeshFilter>().sharedMesh = shojiPaperMesh;
                paper.AddComponent<MeshRenderer>().sharedMaterial = shojiPaperMat;
            }

            Mesh pavRoofMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_Kyoto_PavilionRoof", 6.8f, 6.4f, 1.45f, 0.38f);
            GameObject pavRoof = new GameObject("Pavilion_IrimoyaRoof");
            pavRoof.transform.SetParent(pavilionRoot.transform, false);
            pavRoof.transform.localPosition = new Vector3(0f, 2.40f, -0.45f);
            pavRoof.AddComponent<MeshFilter>().sharedMesh = pavRoofMesh;
            pavRoof.AddComponent<MeshRenderer>().sharedMaterial = kawaraMat;

            // Farolillos imperiales Chōchin colgando de las 4 esquinas del pabellón
            Mesh chochinFrameMesh = ZenBonsaiProceduralMeshes.CreateSculptedHangingChochinLanternMesh("Mesh_HangingChochin", out Mesh chochinGlowMesh);
            GameObject hangingRoot = new GameObject("Sanctuary_HangingLanterns");
            hangingRoot.transform.SetParent(root.transform, false);
            Vector3[] chochinPos =
            {
                new Vector3(-2.15f, 2.38f, 1.65f),  new Vector3(2.15f, 2.38f, 1.65f),
                new Vector3(-2.15f, 2.38f, -2.35f), new Vector3(2.15f, 2.38f, -2.35f)
            };
            for (int c = 0; c < chochinPos.Length; c++)
            {
                GameObject hl = new GameObject($"Kyoto_HangingChochin_{c}");
                hl.transform.SetParent(hangingRoot.transform, false);
                hl.transform.localPosition = chochinPos[c];
                hl.AddComponent<MeshFilter>().sharedMesh = chochinFrameMesh;
                hl.AddComponent<MeshRenderer>().sharedMaterial = goldMat;

                GameObject hGlow = new GameObject("Chochin_Glow");
                hGlow.transform.SetParent(hl.transform, false);
                hGlow.AddComponent<MeshFilter>().sharedMesh = chochinGlowMesh;
                hGlow.AddComponent<MeshRenderer>().sharedMaterial = sunMat;
            }

            // 3. ISLAS ROCOSAS ESCULPIDAS Y PAGODA DE 5 PISOS (GOJŪ-NO-TŌ) CON COLUMNAS, BALCONES Y AGUJA DE 9 ANILLOS
            GameObject pagodaIsland = new GameObject("Kyoto_PagodaRockyPromontory");
            pagodaIsland.transform.SetParent(root.transform, false);
            pagodaIsland.transform.localPosition = new Vector3(-4.2f, -0.52f, 12.8f);
            pagodaIsland.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                "Mesh_PagodaPromontory", new Vector3(4.2f, 1.15f, 3.6f), 2101);
            pagodaIsland.AddComponent<MeshRenderer>().sharedMaterial = mossMat;

            BuildFiveStoryJapanesePagoda(root.transform, new Vector3(-4.2f, 0.18f, 12.8f), kawaraMat, toriiRedMat, templeStoneMat, shojiPaperMat, goldMat);

            // 4. PUENTES ARQUEADOS DE TAMBOR (TAIKO-BASHI) EN EL FRENTE Y EN EL LADO TRASERO
            Mesh bridgeBeamMesh = ZenBonsaiProceduralMeshes.CreateArchedTaikoBridgeMesh("Mesh_Kyoto_TaikoBridge", 7.4f, 1.60f, 1.38f, out Mesh bridgeDeckMesh);
            Vector3[] bridgeCoords = { new Vector3(3.6f, -0.78f, 8.4f), new Vector3(-5.8f, -0.78f, -3.2f) };
            float[] bridgeRotY = { -24f, 62f };
            for (int b = 0; b < bridgeCoords.Length; b++)
            {
                GameObject bridgeObj = new GameObject($"Kyoto_ArchedTaikoBridge_{b}");
                bridgeObj.transform.SetParent(root.transform, false);
                bridgeObj.transform.localPosition = bridgeCoords[b];
                bridgeObj.transform.localRotation = Quaternion.Euler(0f, bridgeRotY[b], 0f);
                bridgeObj.AddComponent<MeshFilter>().sharedMesh = bridgeBeamMesh;
                bridgeObj.AddComponent<MeshRenderer>().sharedMaterial = toriiRedMat;

                GameObject bDeck = new GameObject("TaikoBridge_HinokiDeck");
                bDeck.transform.SetParent(bridgeObj.transform, false);
                bDeck.AddComponent<MeshFilter>().sharedMesh = bridgeDeckMesh;
                bDeck.AddComponent<MeshRenderer>().sharedMaterial = plankFloorMat;
            }

            // 5. GRAN TORII FLOTANTE DE MIYAJIMA SOBRE EL AGUA
            Mesh toriiRedMesh = ZenBonsaiProceduralMeshes.CreateSculptedMyojinToriiMesh("Mesh_MyojinTorii_Main", out Mesh toriiDarkCapsMesh);
            GameObject floatingTorii = new GameObject("Kyoto_FloatingToriiGate");
            floatingTorii.transform.SetParent(root.transform, false);
            floatingTorii.transform.localPosition = new Vector3(0.4f, -0.82f, 12.2f);
            floatingTorii.transform.localScale = Vector3.one * 1.22f;
            floatingTorii.AddComponent<MeshFilter>().sharedMesh = toriiRedMesh;
            floatingTorii.AddComponent<MeshRenderer>().sharedMaterial = toriiRedMat;

            GameObject floatingToriiCaps = new GameObject("FloatingTorii_Caps");
            floatingToriiCaps.transform.SetParent(floatingTorii.transform, false);
            floatingToriiCaps.AddComponent<MeshFilter>().sharedMesh = toriiDarkCapsMesh;
            floatingToriiCaps.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;

            // 6. 14 FAROLILLOS FLOTANTES ESCULPIDOS (TŌRŌ NAGASHI) Y FLORES DE LOTO EN 360° ALREDEDOR DEL PABELLÓN
            Mesh toroWoodMesh = ZenBonsaiProceduralMeshes.CreateSculptedToroNagashiMesh("Mesh_SculptedToroNagashi", out Mesh toroGlowMesh);
            GameObject floatingLanternsRoot = new GameObject("Sanctuary_FloatingLanterns");
            floatingLanternsRoot.transform.SetParent(root.transform, false);
            Vector3[] flPos =
            {
                new Vector3(-1.8f, -0.80f, 4.2f),  new Vector3(-0.4f, -0.80f, 5.5f),
                new Vector3(1.5f, -0.80f, 4.8f),   new Vector3(2.4f, -0.80f, 6.4f),
                new Vector3(-2.6f, -0.80f, 7.2f),  new Vector3(0.8f, -0.80f, 7.8f),
                new Vector3(-4.2f, -0.80f, 2.2f),  new Vector3(4.2f, -0.80f, 2.4f),
                new Vector3(-4.8f, -0.80f, -1.5f), new Vector3(4.8f, -0.80f, -1.8f),
                new Vector3(-3.2f, -0.80f, -5.4f), new Vector3(0.2f, -0.80f, -6.2f),
                new Vector3(3.4f, -0.80f, -5.2f),  new Vector3(-1.5f, -0.80f, 9.6f)
            };
            for (int f = 0; f < flPos.Length; f++)
            {
                GameObject fLantern = new GameObject($"ToroNagashi_{f}");
                fLantern.transform.SetParent(floatingLanternsRoot.transform, false);
                fLantern.transform.localPosition = flPos[f];
                fLantern.transform.localRotation = Quaternion.Euler(0f, f * 31f, 0f);
                fLantern.AddComponent<MeshFilter>().sharedMesh = toroWoodMesh;
                fLantern.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;

                GameObject flGlow = new GameObject("Toro_PaperGlow");
                flGlow.transform.SetParent(fLantern.transform, false);
                flGlow.AddComponent<MeshFilter>().sharedMesh = toroGlowMesh;
                flGlow.AddComponent<MeshRenderer>().sharedMaterial = sunMat;
            }

            // 7. CEREZOS SAKURA Y ARCES OTOÑALES MOMIJI CON FOLLAJE 3D EN LAS ISLAS Y ORILLAS 360°
            Vector3[] treePositions =
            {
                new Vector3(-4.6f, -0.65f, 6.4f),  new Vector3(5.2f, -0.65f, 10.8f),
                new Vector3(5.8f, -0.65f, 4.2f),   new Vector3(-6.4f, -0.65f, 1.2f),
                new Vector3(6.4f, -0.65f, -2.6f),  new Vector3(-5.2f, -0.65f, -6.8f),
                new Vector3(4.8f, -0.65f, -7.2f),  new Vector3(0.0f, -0.65f, -9.2f)
            };
            for (int t = 0; t < treePositions.Length; t++)
            {
                bool isSakura = (t % 2 == 0);
                BuildGardenLandscapeTree(
                    root.transform, $"Kyoto_ShoreTree_{t}", treePositions[t], 3.0f + (t % 3) * 0.25f,
                    realBarkMat, isSakura ? sakuraMat : momijiMat, goldMat,
                    isSakura ? TreeType.Sakura : TreeType.Momiji, 2200 + t * 17);
            }

            root.SetActive(false);
            return root;
        }

        // =========================================================================
        // ENTORNO 3 (360° COMPLETO): MONASTERIO ZEN KARESANSUI Y CATEDRAL DE BAMBÚ DE ARASHIYAMA 360°
        // El jugador está en el claustro Hōjō de un templo Zen (suelo de tablones, tatamis, altar budista con estatua
        // tallada de Buda/Jizō detrás del jugador, techo artesonado y faroles), rodeado en 360° por el mar de grava
        // rastrillada Karesansui, muros de claustro con puerta Sanmon y más de 32 bosquecillos gigantes de bambú en 360°.
        // =========================================================================
        private static GameObject BuildArashiyamaBambooTempleSanctuary(
            Transform parent,
            Material mountainMat,
            Material urushiMat, Material hinokiMat, Material stoneMat, Material goldMat, Material pineNeedleMat)
        {
            GameObject root = new GameObject("ZenSanctuary_3_ArashiyamaBambooTemple");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = Vector3.zero;

            Material skyDomeMat = CreateOrGetPanoramicSkyDomeMaterial("Zen_SkyPanorama_ArashiyamaZen",
                new Color(0.22f, 0.48f, 0.72f), new Color(0.54f, 0.74f, 0.86f), new Color(0.86f, 0.92f, 0.82f),
                new Color(0.98f, 0.98f, 0.92f), new Color(0.52f, 0.64f, 0.68f), new Color(0.18f, 0.32f, 0.28f), 303);

            Material sunMat = CreateOrGetPBRMaterial("Zen_SanctuarySunMat", new Color(1.0f, 0.92f, 0.74f), 0.9f, 0.0f);
            sunMat.EnableKeyword("_EMISSION");
            if (sunMat.HasProperty("_EmissionColor")) sunMat.SetColor("_EmissionColor", new Color(1.0f, 0.86f, 0.58f) * 1.65f);

            Material rakedSandMat = CreateOrGetSurfaceTexturedMaterial("Zen_RakedKaresansuiMat",
                new Color(0.88f, 0.86f, 0.82f), new Color(0.70f, 0.68f, 0.64f), new Color(0.96f, 0.95f, 0.92f), 0.14f, 0.0f, 16);
            Material bambooCulmMat = CreateOrGetSurfaceTexturedMaterial("Zen_BambooForestMat",
                new Color(0.24f, 0.48f, 0.18f), new Color(0.14f, 0.32f, 0.10f), new Color(0.40f, 0.66f, 0.28f), 0.52f, 0.0f, 15);
            Material mossMat = CreateOrGetSurfaceTexturedMaterial("Zen_SanctuaryMossMat",
                new Color(0.18f, 0.42f, 0.14f), new Color(0.09f, 0.24f, 0.07f), new Color(0.32f, 0.58f, 0.20f), 0.20f, 0.0f, 11);
            Material kawaraMat = CreateOrGetSurfaceTexturedMaterial("Zen_KawaraTilePBRMat",
                new Color(0.17f, 0.19f, 0.23f), new Color(0.08f, 0.09f, 0.12f), new Color(0.38f, 0.42f, 0.48f), 0.72f, 0.22f, 19);
            Material shikkuiMat = CreateOrGetSurfaceTexturedMaterial("Zen_ShikkuiPlasterPBRMat",
                new Color(0.93f, 0.91f, 0.86f), new Color(0.76f, 0.73f, 0.68f), new Color(0.98f, 0.97f, 0.94f), 0.24f, 0.0f, 22);
            Material plankFloorMat = CreateOrGetSurfaceTexturedMaterial("Zen_HinokiPlankFloorMat",
                new Color(0.48f, 0.30f, 0.17f), new Color(0.26f, 0.15f, 0.08f), new Color(0.68f, 0.46f, 0.28f), 0.68f, 0.04f, 17);
            Material tatamiMat = CreateOrGetSurfaceTexturedMaterial("Zen_TatamiWeavePBRMat",
                new Color(0.58f, 0.56f, 0.32f), new Color(0.42f, 0.40f, 0.20f), new Color(0.74f, 0.70f, 0.44f), 0.28f, 0.0f, 18);
            Material shojiPaperMat = CreateOrGetSurfaceTexturedMaterial("Zen_ShojiLatticePBRMat",
                new Color(0.96f, 0.92f, 0.82f), new Color(0.28f, 0.17f, 0.10f), new Color(1.0f, 0.97f, 0.90f), 0.22f, 0.0f, 20);
            shojiPaperMat.EnableKeyword("_EMISSION");
            if (shojiPaperMat.HasProperty("_EmissionColor")) shojiPaperMat.SetColor("_EmissionColor", new Color(0.36f, 0.32f, 0.24f));
            Material templeStoneMat = CreateOrGetSurfaceTexturedMaterial("Zen_TempleStoneMasonryMat",
                new Color(0.42f, 0.43f, 0.45f), new Color(0.22f, 0.23f, 0.25f), new Color(0.62f, 0.64f, 0.66f), 0.25f, 0.05f, 21);

            // 1. CÚPULA CELESTE 360° + TERRENO DE BOSQUE DE MUSGO 360° + ANILLO MONTAÑOSO 360°
            Mesh skySphereMesh = ZenBonsaiProceduralMeshes.CreatePanoramicSkySphereMesh("Mesh_PanoramicSkySphere_360", 42f, 64, 32);
            GameObject skyDome = new GameObject("Sanctuary_SkyDome");
            skyDome.transform.SetParent(root.transform, false);
            skyDome.AddComponent<MeshFilter>().sharedMesh = skySphereMesh;
            skyDome.AddComponent<MeshRenderer>().sharedMaterial = skyDomeMat;

            Mesh forestTerrainMesh = ZenBonsaiProceduralMeshes.CreateFull360SanctuaryTerrainMesh(
                "Mesh_Arashiyama_Terrain360", 76f, -0.82f, Vector2.zero, Vector2.zero, 0f, 303);
            GameObject forestGround = new GameObject("Temple_MossForestGround360");
            forestGround.transform.SetParent(root.transform, false);
            forestGround.AddComponent<MeshFilter>().sharedMesh = forestTerrainMesh;
            forestGround.AddComponent<MeshRenderer>().sharedMaterial = mossMat;

            Mesh horizonRingMesh = ZenBonsaiProceduralMeshes.Create360MountainHorizonRingMesh(
                "Mesh_Arashiyama_HorizonRing360", 24f, 38f, 6.5f, 0.10f, 309);
            GameObject horizonRing = new GameObject("Temple_360MountainRing");
            horizonRing.transform.SetParent(root.transform, false);
            horizonRing.AddComponent<MeshFilter>().sharedMesh = horizonRingMesh;
            horizonRing.AddComponent<MeshRenderer>().sharedMaterial = mountainMat;

            // 2. CLAUSTRO DE MEDITACIÓN ZEN 360° (HŌJŌ) CON ALTAR DE BUDA TALLADO EN PIEDRA DETRÁS DEL JUGADOR
            GameObject hojoRoot = new GameObject("Temple_HojoMeditationHall360");
            hojoRoot.transform.SetParent(root.transform, false);

            Mesh deckMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_Temple_EngawaDeck", 6.8f, 5.4f, 0.05f, 0.0f);
            GameObject templeDeck = new GameObject("Temple_EngawaDeck");
            templeDeck.transform.SetParent(hojoRoot.transform, false);
            templeDeck.transform.localPosition = new Vector3(0f, -0.76f, -0.60f);
            templeDeck.AddComponent<MeshFilter>().sharedMesh = deckMesh;
            templeDeck.AddComponent<MeshRenderer>().sharedMaterial = plankFloorMat;

            Mesh tatamiMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_TeaRoom_TatamiMat", 1.85f, 0.92f, 0.018f, 0.0f);
            Vector3[] tatamiPos =
            {
                new Vector3(-0.95f, -0.725f, 0.0f),   new Vector3(0.95f, -0.725f, 0.0f),
                new Vector3(-0.95f, -0.725f, -0.95f), new Vector3(0.95f, -0.725f, -0.95f),
                new Vector3(-0.95f, -0.725f, -1.90f), new Vector3(0.95f, -0.725f, -1.90f)
            };
            for (int t = 0; t < tatamiPos.Length; t++)
            {
                GameObject tat = new GameObject($"Hojo_Tatami_{t}");
                tat.transform.SetParent(hojoRoot.transform, false);
                tat.transform.localPosition = tatamiPos[t];
                tat.AddComponent<MeshFilter>().sharedMesh = tatamiMesh;
                tat.AddComponent<MeshRenderer>().sharedMaterial = tatamiMat;
            }

            // Columnas torneadas del monasterio + Techo curvo Kawara
            Mesh pillarMesh = ZenBonsaiProceduralMeshes.CreateSculptedTurnedPillarMesh(
                "Mesh_TeaRoom_TurnedPillar", 0.095f, 3.15f, out Mesh plinthMesh, out Mesh corbelMesh);
            Vector3[] hojoPillars =
            {
                new Vector3(-2.65f, -0.74f, 1.65f),  new Vector3(2.65f, -0.74f, 1.65f),
                new Vector3(-2.65f, -0.74f, -2.75f), new Vector3(2.65f, -0.74f, -2.75f),
                new Vector3(-1.15f, -0.74f, -2.75f), new Vector3(1.15f, -0.74f, -2.75f)
            };
            for (int p = 0; p < hojoPillars.Length; p++)
            {
                GameObject col = new GameObject($"Hojo_CedarPillar_{p}");
                col.transform.SetParent(hojoRoot.transform, false);
                col.transform.localPosition = hojoPillars[p];
                col.AddComponent<MeshFilter>().sharedMesh = pillarMesh;
                col.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;

                GameObject pl = new GameObject("Pillar_Plinth");
                pl.transform.SetParent(col.transform, false);
                pl.AddComponent<MeshFilter>().sharedMesh = plinthMesh;
                pl.AddComponent<MeshRenderer>().sharedMaterial = templeStoneMat;

                GameObject cb = new GameObject("Pillar_Corbel");
                cb.transform.SetParent(col.transform, false);
                cb.AddComponent<MeshFilter>().sharedMesh = corbelMesh;
                cb.AddComponent<MeshRenderer>().sharedMaterial = plankFloorMat;
            }

            Mesh hojoRoofMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_Hojo_MainRoof", 7.6f, 6.6f, 1.40f, 0.35f);
            GameObject hojoRoof = new GameObject("Hojo_KawaraRoof");
            hojoRoof.transform.SetParent(hojoRoot.transform, false);
            hojoRoof.transform.localPosition = new Vector3(0f, 2.40f, -0.55f);
            hojoRoof.AddComponent<MeshFilter>().sharedMesh = hojoRoofMesh;
            hojoRoof.AddComponent<MeshRenderer>().sharedMaterial = kawaraMat;

            // Altar Zen trasero (Butsudan) con estatua tallada de Buda en loto y paneles Shoji iluminados detrás del jugador
            Mesh shojiFrameMesh = ZenBonsaiProceduralMeshes.CreateSculptedShojiScreenMesh(
                "Mesh_TeaRoom_ShojiScreen", 1.12f, 2.85f, 4, 7, out Mesh shojiPaperMesh);
            float[] rearX = { -1.90f, 1.90f };
            for (int s = 0; s < rearX.Length; s++)
            {
                GameObject shoji = new GameObject($"Hojo_RearShoji_{s}");
                shoji.transform.SetParent(hojoRoot.transform, false);
                shoji.transform.localPosition = new Vector3(rearX[s], -0.68f, -2.75f);
                shoji.AddComponent<MeshFilter>().sharedMesh = shojiFrameMesh;
                shoji.AddComponent<MeshRenderer>().sharedMaterial = urushiMat;

                GameObject paper = new GameObject("Shoji_Glow");
                paper.transform.SetParent(shoji.transform, false);
                paper.AddComponent<MeshFilter>().sharedMesh = shojiPaperMesh;
                paper.AddComponent<MeshRenderer>().sharedMaterial = shojiPaperMat;
            }

            Mesh buddhaMesh = ZenBonsaiProceduralMeshes.CreateSculptedZenStatueMesh("Mesh_ZenBuddhaStatue", out Mesh lotusPedestalMesh);
            GameObject rearStatue = new GameObject("Hojo_RearZenBuddhaAltar");
            rearStatue.transform.SetParent(hojoRoot.transform, false);
            rearStatue.transform.localPosition = new Vector3(0f, -0.62f, -2.45f);
            rearStatue.AddComponent<MeshFilter>().sharedMesh = buddhaMesh;
            rearStatue.AddComponent<MeshRenderer>().sharedMaterial = templeStoneMat;

            GameObject statuePedestal = new GameObject("Buddha_LotusPedestal");
            statuePedestal.transform.SetParent(rearStatue.transform, false);
            statuePedestal.AddComponent<MeshFilter>().sharedMesh = lotusPedestalMesh;
            statuePedestal.AddComponent<MeshRenderer>().sharedMaterial = goldMat;

            // 3. MAR DE GRAVA BLANCA RASTRILLADA CON SURCOS CONCÉNTRICOS 3D (KARESANSUI) Y 7 ISLAS DE ROCA SUISEKI
            Mesh karesansuiMesh = ZenBonsaiProceduralMeshes.CreateRakedKaresansuiGardenMesh("Mesh_Ryoanji_KaresansuiSand", 18f, 14f);
            GameObject sandCourt = new GameObject("Temple_KaresansuiSandSea");
            sandCourt.transform.SetParent(root.transform, false);
            sandCourt.transform.localPosition = new Vector3(0f, -0.79f, 7.2f);
            sandCourt.AddComponent<MeshFilter>().sharedMesh = karesansuiMesh;
            sandCourt.AddComponent<MeshRenderer>().sharedMaterial = rakedSandMat;

            Vector3[] zenRockPos =
            {
                new Vector3(-3.2f, -0.62f, 4.5f),
                new Vector3(-1.4f, -0.66f, 6.2f),
                new Vector3(1.8f, -0.56f, 5.4f),
                new Vector3(3.4f, -0.64f, 7.8f),
                new Vector3(-2.5f, -0.58f, 9.2f),
                new Vector3(0.4f, -0.52f, 9.8f),
                new Vector3(2.6f, -0.60f, 10.6f)
            };
            for (int r = 0; r < zenRockPos.Length; r++)
            {
                GameObject mossBed = new GameObject($"Karesansui_MossRing_{r}");
                mossBed.transform.SetParent(root.transform, false);
                mossBed.transform.localPosition = zenRockPos[r] + new Vector3(0f, -0.10f, 0f);
                mossBed.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                    $"Mesh_KaresansuiMoss_{r}", new Vector3(1.25f, 0.16f, 1.05f), 3100 + r);
                mossBed.AddComponent<MeshRenderer>().sharedMaterial = mossMat;

                GameObject zRock = new GameObject($"Karesansui_SuisekiRock_{r}");
                zRock.transform.SetParent(root.transform, false);
                zRock.transform.localPosition = zenRockPos[r];
                zRock.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                    $"Mesh_KaresansuiRock_{r}", new Vector3(0.82f, 0.58f + (r % 3) * 0.18f, 0.68f), 3200 + r * 17);
                zRock.AddComponent<MeshRenderer>().sharedMaterial = templeStoneMat;
            }

            // 4. MUROS DEL CLAUSTRO (TSUKIJI-BEI) RODEANDO EL JARDÍN SECO CON TEJADOS CURVOS KAWARA (0 CUBOS PRIMITIVOS)
            Mesh wallRoofMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_TempleBackWallRoof", 16.4f, 1.15f, 0.52f, 0.14f);
            Mesh wallBodyMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_TempleWallBody", 15.8f, 0.36f, 1.45f, 0.0f);
            Mesh wallStoneBaseMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_TempleWallStoneBase", 16.0f, 0.48f, 0.48f, 0.0f);

            Vector3[] cloisterWalls =
            {
                new Vector3(0f, -0.80f, 14.2f),
                new Vector3(-8.8f, -0.80f, 6.8f),
                new Vector3(8.8f, -0.80f, 6.8f)
            };
            float[] cloisterRotY = { 0f, 90f, 90f };
            for (int cw = 0; cw < cloisterWalls.Length; cw++)
            {
                GameObject cWall = new GameObject($"Temple_CloisterWall_{cw}");
                cWall.transform.SetParent(root.transform, false);
                cWall.transform.localPosition = cloisterWalls[cw];
                cWall.transform.localRotation = Quaternion.Euler(0f, cloisterRotY[cw], 0f);

                GameObject cwStone = new GameObject("Cloister_IshigakiBase");
                cwStone.transform.SetParent(cWall.transform, false);
                cwStone.AddComponent<MeshFilter>().sharedMesh = wallStoneBaseMesh;
                cwStone.AddComponent<MeshRenderer>().sharedMaterial = templeStoneMat;

                GameObject cwPlaster = new GameObject("Cloister_ShikkuiWall");
                cwPlaster.transform.SetParent(cWall.transform, false);
                cwPlaster.transform.localPosition = new Vector3(0f, 0.42f, 0f);
                cwPlaster.AddComponent<MeshFilter>().sharedMesh = wallBodyMesh;
                cwPlaster.AddComponent<MeshRenderer>().sharedMaterial = shikkuiMat;

                GameObject cwRoof = new GameObject("Cloister_KawaraRoof");
                cwRoof.transform.SetParent(cWall.transform, false);
                cwRoof.transform.localPosition = new Vector3(0f, 1.82f, 0f);
                cwRoof.AddComponent<MeshFilter>().sharedMesh = wallRoofMesh;
                cwRoof.AddComponent<MeshRenderer>().sharedMaterial = kawaraMat;
            }

            // Linternas Kasuga talladas en los 4 puntos cardinales del claustro
            Mesh lanternStoneMesh = ZenBonsaiProceduralMeshes.CreateSculptedStoneLanternMesh("Mesh_KasugaStoneLantern", out Mesh lanternLightMesh);
            Vector3[] tLanternPos =
            {
                new Vector3(-4.2f, -0.76f, 4.2f),  new Vector3(4.2f, -0.76f, 4.2f),
                new Vector3(-4.2f, -0.76f, -3.8f), new Vector3(4.2f, -0.76f, -3.8f)
            };
            for (int l = 0; l < tLanternPos.Length; l++)
            {
                GameObject lantern = new GameObject($"Temple_StoneLantern_{l}");
                lantern.transform.SetParent(root.transform, false);
                lantern.transform.localPosition = tLanternPos[l];
                lantern.AddComponent<MeshFilter>().sharedMesh = lanternStoneMesh;
                lantern.AddComponent<MeshRenderer>().sharedMaterial = templeStoneMat;

                GameObject lGlow = new GameObject("Lantern_Glow");
                lGlow.transform.SetParent(lantern.transform, false);
                lGlow.AddComponent<MeshFilter>().sharedMesh = lanternLightMesh;
                lGlow.AddComponent<MeshRenderer>().sharedMaterial = sunMat;
            }

            // 5. CATEDRAL DE BAMBÚ SAGRADO DE ARASHIYAMA EN 360° COMPLETOS (32 AGRUPACIONES RODEANDO TODO EL TEMPLO)
            Mesh bambooClusterMesh = ZenBonsaiProceduralMeshes.CreateBambooGroveClusterMesh("Mesh_BambooGroveCluster", out Mesh bambooLeavesMesh);
            int totalBambooClusters = 32;
            for (int b = 0; b < totalBambooClusters; b++)
            {
                float ang = (b / (float)totalBambooClusters) * Mathf.PI * 2f;
                float dist = (b % 2 == 0) ? 14.5f : 17.2f;
                Vector3 bPos = new Vector3(Mathf.Sin(ang) * dist, -0.78f, Mathf.Cos(ang) * dist);
                // Despejar el patio central del jardín seco y el salón Hōjō
                if (Mathf.Abs(bPos.x) < 9.2f && bPos.z > -4.5f && bPos.z < 14.8f)
                {
                    bPos.x = (bPos.x >= 0f) ? 10.2f : -10.2f;
                }

                GameObject clump = new GameObject($"Arashiyama_SacredBamboo360_{b}");
                clump.transform.SetParent(root.transform, false);
                clump.transform.localPosition = bPos;
                clump.transform.localRotation = Quaternion.Euler(0f, b * 37f, 0f);
                clump.transform.localScale = Vector3.one * (1.22f + (b % 3) * 0.14f);
                clump.AddComponent<MeshFilter>().sharedMesh = bambooClusterMesh;
                clump.AddComponent<MeshRenderer>().sharedMaterial = bambooCulmMat;

                GameObject bLeaves = new GameObject("Bamboo_Canopy");
                bLeaves.transform.SetParent(clump.transform, false);
                bLeaves.AddComponent<MeshFilter>().sharedMesh = bambooLeavesMesh;
                bLeaves.AddComponent<MeshRenderer>().sharedMaterial = pineNeedleMat;
            }

            root.SetActive(false);
            return root;
        }

        private static void BuildFiveStoryJapanesePagoda(
            Transform parent, Vector3 localPos,
            Material kawaraRoofMat, Material vermilionMat, Material stoneBaseMat, Material shojiWindowMat, Material goldMat)
        {
            GameObject pagoda = new GameObject("Kyoto_FiveStoryPagoda_GojuNoTo");
            pagoda.transform.SetParent(parent, false);
            pagoda.transform.localPosition = localPos;
            pagoda.transform.localRotation = Quaternion.Euler(0f, 18f, 0f);

            // Podio de mampostería de piedra tallada (Kidan) sin bloques primitivos
            Mesh podiumMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh("Mesh_Pagoda_StoneKidan", 3.1f, 3.1f, 0.52f, 0.02f);
            GameObject stoneBase = new GameObject("Pagoda_StoneKidanBase");
            stoneBase.transform.SetParent(pagoda.transform, false);
            stoneBase.AddComponent<MeshFilter>().sharedMesh = podiumMesh;
            stoneBase.AddComponent<MeshRenderer>().sharedMaterial = stoneBaseMat;

            float currY = 0.50f;
            for (int tier = 0; tier < 5; tier++)
            {
                float scale = 1.0f - tier * 0.105f;
                float coreW = 1.88f * scale;
                float storyH = 0.82f * scale;
                float roofW = 3.25f * scale;
                float roofH = 0.44f * scale;

                // 4 Fachadas esculpidas con celosía Shoji iluminada y columnas esquineras de madera bermellón
                Mesh tierWallMesh = ZenBonsaiProceduralMeshes.CreateSculptedShojiScreenMesh(
                    $"Mesh_PagodaTierWall_{tier}", coreW, storyH, 3, 2, out Mesh tierGlowMesh);

                for (int side = 0; side < 4; side++)
                {
                    float ang = side * 90f;
                    Vector3 dir = Quaternion.Euler(0f, ang, 0f) * Vector3.forward;
                    GameObject facade = new GameObject($"Pagoda_Tier_{tier}_Facade_{side}");
                    facade.transform.SetParent(pagoda.transform, false);
                    facade.transform.localPosition = new Vector3(0f, currY, 0f) + dir * (coreW * 0.48f);
                    facade.transform.localRotation = Quaternion.Euler(0f, ang, 0f);
                    facade.AddComponent<MeshFilter>().sharedMesh = tierWallMesh;
                    facade.AddComponent<MeshRenderer>().sharedMaterial = vermilionMat;

                    GameObject fGlow = new GameObject("Facade_ShojiGlow");
                    fGlow.transform.SetParent(facade.transform, false);
                    fGlow.AddComponent<MeshFilter>().sharedMesh = tierGlowMesh;
                    fGlow.AddComponent<MeshRenderer>().sharedMaterial = shojiWindowMat;
                }

                // Tejado curvo japonés Irimoya con aleros levantados (Warabi-te) y tejas Kawara PBR
                Mesh tierRoofMesh = ZenBonsaiProceduralMeshes.CreateCurvedPagodaRoofMesh(
                    $"Mesh_PagodaTierRoof_{tier}", roofW, roofW, roofH, 0.18f * scale);
                GameObject roofObj = new GameObject($"Pagoda_CurvedRoof_{tier}");
                roofObj.transform.SetParent(pagoda.transform, false);
                roofObj.transform.localPosition = new Vector3(0f, currY + storyH, 0f);
                roofObj.AddComponent<MeshFilter>().sharedMesh = tierRoofMesh;
                roofObj.AddComponent<MeshRenderer>().sharedMaterial = kawaraRoofMat;

                currY += storyH + roofH * 0.74f;
            }

            // Aguja budista imperial (Sōrin) esculpida con los 9 anillos sagrados Kurin y joya Hōju
            Mesh sorinMesh = ZenBonsaiProceduralMeshes.CreateSculptedSorinSpireMesh("Mesh_Pagoda_SorinSpire", 1.55f);
            GameObject sorinSpire = new GameObject("Pagoda_GoldenSorinSpire");
            sorinSpire.transform.SetParent(pagoda.transform, false);
            sorinSpire.transform.localPosition = new Vector3(0f, currY + 0.12f, 0f);
            sorinSpire.AddComponent<MeshFilter>().sharedMesh = sorinMesh;
            sorinSpire.AddComponent<MeshRenderer>().sharedMaterial = goldMat;
        }

        private static void BuildGardenLandscapeTree(
            Transform parent, string name, Vector3 localPos, float scale,
            Material barkMat, Material foliageMat, Material goldMat, TreeType species, int seed)
        {
            GameObject tree = new GameObject(name);
            tree.transform.SetParent(parent, false);
            tree.transform.localPosition = localPos;
            tree.transform.localScale = Vector3.one * scale;

            BranchSpline[] branches = new BranchSpline[]
            {
                new BranchSpline(new Vector3(-0.02f, 0.24f, 0f), new Vector3(-0.18f, 0.34f, 0.04f), new Vector3(-0.34f, 0.38f, 0.06f), new Vector3(-0.46f, 0.42f, 0.08f), 0.040f, 0.012f),
                new BranchSpline(new Vector3(0.02f, 0.34f, 0f),  new Vector3(0.18f, 0.44f, -0.04f), new Vector3(0.32f, 0.48f, -0.06f), new Vector3(0.44f, 0.52f, -0.08f), 0.036f, 0.011f),
                new BranchSpline(new Vector3(0f, 0.44f, 0.02f),  new Vector3(-0.08f, 0.58f, 0.14f), new Vector3(-0.14f, 0.66f, 0.24f), new Vector3(-0.18f, 0.72f, 0.30f), 0.030f, 0.010f),
                new BranchSpline(new Vector3(0.01f, 0.48f, -0.02f), new Vector3(0.10f, 0.58f, -0.14f), new Vector3(0.18f, 0.66f, -0.24f), new Vector3(0.24f, 0.72f, -0.30f), 0.028f, 0.009f),
                new BranchSpline(new Vector3(-0.01f, 0.54f, -0.01f), new Vector3(-0.12f, 0.64f, -0.10f), new Vector3(-0.22f, 0.72f, -0.16f), new Vector3(-0.28f, 0.78f, -0.20f), 0.026f, 0.009f),
                new BranchSpline(new Vector3(0f, 0.58f, 0f), new Vector3(0.04f, 0.70f, 0f), new Vector3(0.02f, 0.80f, 0f), new Vector3(0f, 0.88f, 0f), 0.032f, 0.010f)
            };

            Mesh trunkMesh = ZenBonsaiProceduralMeshes.CreateMasterTreeWoodMesh(
                $"Mesh_{name}_Trunk",
                Vector3.zero, new Vector3(-0.08f, 0.26f, 0.03f), new Vector3(0.06f, 0.50f, -0.02f), new Vector3(0f, 0.74f, 0f),
                0.088f, 0.054f, 0.022f, branches, 8, 0.72f, 0.15f, 5.5f, seed);

            GameObject trunkObj = new GameObject("LandscapeTrunk");
            trunkObj.transform.SetParent(tree.transform, false);
            trunkObj.AddComponent<MeshFilter>().sharedMesh = trunkMesh;
            trunkObj.AddComponent<MeshRenderer>().sharedMaterial = barkMat;

            Vector3 padScale = new Vector3(0.44f, 0.16f, 0.38f);
            for (int i = 0; i < branches.Length; i++)
            {
                GameObject pad = new GameObject($"LandscapeCanopy_{i}");
                pad.transform.SetParent(tree.transform, false);
                pad.transform.localPosition = branches[i].p3 + new Vector3(0f, 0.02f, 0f);
                pad.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateFoliageCloudPadMesh(
                    $"Mesh_{name}_Pad_{i}", padScale, 14, 0.80f, 0.20f, seed + i * 19);
                pad.AddComponent<MeshRenderer>().sharedMaterial = foliageMat;

                // Añadir flores de Sakura 3D, hojas de Arce 3D o acículas de Pino 3D sobre cada copa del árbol de jardín
                if (species == TreeType.Sakura)
                {
                    Mesh corollas = ZenBonsaiProceduralMeshes.CreateSakuraBlossomCorollasMesh(
                        $"Mesh_{name}_Blossoms_{i}", padScale, 32, seed + 100 + i * 13, out Mesh stamens);
                    GameObject bObj = new GameObject("Canopy_3DBlossoms");
                    bObj.transform.SetParent(pad.transform, false);
                    bObj.AddComponent<MeshFilter>().sharedMesh = corollas;
                    bObj.AddComponent<MeshRenderer>().sharedMaterial = foliageMat;

                    GameObject sObj = new GameObject("Canopy_Stamens");
                    sObj.transform.SetParent(pad.transform, false);
                    sObj.AddComponent<MeshFilter>().sharedMesh = stamens;
                    sObj.AddComponent<MeshRenderer>().sharedMaterial = goldMat;
                }
                else if (species == TreeType.Momiji)
                {
                    Mesh mapleLeaves = ZenBonsaiProceduralMeshes.CreateMomijiStarLeavesMesh(
                        $"Mesh_{name}_Maple_{i}", padScale, 28, seed + 200 + i * 13);
                    GameObject mObj = new GameObject("Canopy_3DMapleLeaves");
                    mObj.transform.SetParent(pad.transform, false);
                    mObj.AddComponent<MeshFilter>().sharedMesh = mapleLeaves;
                    mObj.AddComponent<MeshRenderer>().sharedMaterial = foliageMat;
                }
                else
                {
                    Mesh pineNeedles = ZenBonsaiProceduralMeshes.CreateMatsuNeedleClustersMesh(
                        $"Mesh_{name}_Needles_{i}", padScale, 32, seed + 300 + i * 13);
                    GameObject nObj = new GameObject("Canopy_3DPineNeedles");
                    nObj.transform.SetParent(pad.transform, false);
                    nObj.AddComponent<MeshFilter>().sharedMesh = pineNeedles;
                    nObj.AddComponent<MeshRenderer>().sharedMaterial = foliageMat;
                }
            }
        }

        private static GameObject BuildSakuraPetalsRain(Transform parent, Material petalMat)
        {
            GameObject rainRoot = new GameObject("SakuraPetals_Rain");
            rainRoot.transform.SetParent(parent, false);
            rainRoot.transform.localPosition = Vector3.zero;

            // 28 pétalos curvados 3D reales (no esferas aplastadas) flotando y cayendo suavemente alrededor del bonsái
            Mesh singlePetalMesh = ZenBonsaiProceduralMeshes.CreateSingleFloatingSakuraPetalMesh("Mesh_SingleFloatingSakuraPetal");
            for (int p = 0; p < 28; p++)
            {
                GameObject petal = new GameObject($"FloatingPetal_{p}");
                petal.transform.SetParent(rainRoot.transform, false);
                float ang = p * (360f / 28f) * Mathf.Deg2Rad;
                float r = 0.15f + (p % 5) * 0.052f;
                float y = -0.02f + (p * 0.018f);
                petal.transform.localPosition = new Vector3(Mathf.Cos(ang) * r, y, Mathf.Sin(ang) * r + 0.02f);
                petal.transform.localRotation = Quaternion.Euler(Random.Range(10f, 65f), Random.Range(0f, 360f), Random.Range(10f, 55f));
                petal.transform.localScale = Vector3.one;
                petal.AddComponent<MeshFilter>().sharedMesh = singlePetalMesh;
                petal.AddComponent<MeshRenderer>().sharedMaterial = petalMat;
            }

            return rainRoot;
        }

        #region 2. Mesa Japonesa Lacada Urushi y Esterilla de Bambú Sudare

        private static void BuildJapaneseDisplayTable(Transform parent, Material tableMat, Material apronMat, Material goldMat, Material bambooMat)
        {
            GameObject tableRoot = new GameObject("ZenTable");
            tableRoot.transform.SetParent(parent, false);
            tableRoot.transform.localPosition = new Vector3(0f, -0.06f, 0f);

            // Tablero principal lacado Urushi
            GameObject topBoard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topBoard.name = "Table_TopBoard";
            topBoard.transform.SetParent(tableRoot.transform, false);
            topBoard.transform.localPosition = Vector3.zero;
            topBoard.transform.localScale = new Vector3(0.76f, 0.024f, 0.48f);
            topBoard.GetComponent<Renderer>().sharedMaterial = tableMat;

            // Filete dorado Maki-e perimetral empotrado en el tablero
            GameObject goldInlay = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goldInlay.name = "Table_GoldMakiE";
            goldInlay.transform.SetParent(tableRoot.transform, false);
            goldInlay.transform.localPosition = new Vector3(0f, 0.002f, 0f);
            goldInlay.transform.localScale = new Vector3(0.764f, 0.005f, 0.484f);
            goldInlay.GetComponent<Renderer>().sharedMaterial = goldMat;
            var gCol = goldInlay.GetComponent<Collider>(); if (gCol != null) DestroyImmediate(gCol);

            // Faldón tallado inferior (Makuita)
            GameObject apron = GameObject.CreatePrimitive(PrimitiveType.Cube);
            apron.name = "Table_CarvedApron";
            apron.transform.SetParent(tableRoot.transform, false);
            apron.transform.localPosition = new Vector3(0f, -0.022f, 0f);
            apron.transform.localScale = new Vector3(0.70f, 0.022f, 0.42f);
            apron.GetComponent<Renderer>().sharedMaterial = apronMat;
            var aCol = apron.GetComponent<Collider>(); if (aCol != null) DestroyImmediate(aCol);

            // 4 Patas japonesas talladas (Neko-ashi) con travesaños
            Vector3[] legPositions =
            {
                new Vector3(-0.33f, -0.048f, -0.19f),
                new Vector3(0.33f, -0.048f, -0.19f),
                new Vector3(-0.33f, -0.048f, 0.19f),
                new Vector3(0.33f, -0.048f, 0.19f)
            };
            for (int i = 0; i < legPositions.Length; i++)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leg.name = $"Table_Leg_{i}";
                leg.transform.SetParent(tableRoot.transform, false);
                leg.transform.localPosition = legPositions[i];
                leg.transform.localScale = new Vector3(0.036f, 0.055f, 0.036f);
                leg.GetComponent<Renderer>().sharedMaterial = tableMat;
                var lCol = leg.GetComponent<Collider>(); if (lCol != null) DestroyImmediate(lCol);

                GameObject footPad = GameObject.CreatePrimitive(PrimitiveType.Cube);
                footPad.transform.SetParent(leg.transform, false);
                footPad.transform.localPosition = new Vector3(0f, -0.48f, 0f);
                footPad.transform.localScale = new Vector3(1.25f, 0.18f, 1.25f);
                footPad.GetComponent<Renderer>().sharedMaterial = goldMat;
                var fpCol = footPad.GetComponent<Collider>(); if (fpCol != null) DestroyImmediate(fpCol);
            }

            // Esterilla tejida de varillas de bambú (Sudare / Kiita) bajo la maceta del bonsái
            GameObject sudareRoot = new GameObject("BambooMat_Sudare");
            sudareRoot.transform.SetParent(tableRoot.transform, false);
            sudareRoot.transform.localPosition = new Vector3(0f, 0.013f, 0f);

            int slatCount = 19;
            for (int s = 0; s < slatCount; s++)
            {
                float zOff = (s - (slatCount - 1) * 0.5f) * 0.014f;
                GameObject slat = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                slat.name = $"BambooSlat_{s}";
                slat.transform.SetParent(sudareRoot.transform, false);
                slat.transform.localPosition = new Vector3(0f, 0f, zOff);
                slat.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                slat.transform.localScale = new Vector3(0.005f, 0.19f, 0.011f);
                slat.GetComponent<Renderer>().sharedMaterial = bambooMat;
                var sc = slat.GetComponent<Collider>(); if (sc != null) DestroyImmediate(sc);
            }

            // Cintas de seda negra que unen las varillas de bambú
            float[] ribbonX = { -0.14f, 0.14f };
            for (int r = 0; r < ribbonX.Length; r++)
            {
                GameObject ribbon = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ribbon.transform.SetParent(sudareRoot.transform, false);
                ribbon.transform.localPosition = new Vector3(ribbonX[r], 0.001f, 0f);
                ribbon.transform.localScale = new Vector3(0.012f, 0.005f, 0.27f);
                ribbon.GetComponent<Renderer>().sharedMaterial = tableMat;
                var rc = ribbon.GetComponent<Collider>(); if (rc != null) DestroyImmediate(rc);
            }
        }

        private static void BuildTsukubaiWaterBasin(Transform parent, Material stoneMat, Material waterMat, Material bambooMat, Material mossMat, Material goldMat)
        {
            GameObject waterBowl = new GameObject("ZenWaterBowl_Tsukubai");
            waterBowl.transform.SetParent(parent, false);
            // Apoyado 100% firme al ras del tablero de la mesa (y = -0.050m) en el lado izquierdo despejado,
            // lejos de la consola frontal para coger el cazo Hishaku con total libertad y comodidad
            waterBowl.transform.localPosition = new Vector3(-0.27f, -0.050f, -0.045f);

            // Base circular de piedra/madera asentada directamente sobre el tablero de la mesa (cero hueco visual)
            GameObject stonePad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stonePad.name = "Tsukubai_TableBasePad";
            stonePad.transform.SetParent(waterBowl.transform, false);
            stonePad.transform.localPosition = new Vector3(0f, 0.004f, 0f);
            stonePad.transform.localScale = new Vector3(0.158f, 0.005f, 0.158f);
            stonePad.GetComponent<Renderer>().sharedMaterial = stoneMat;
            var spCol = stonePad.GetComponent<Collider>(); if (spCol != null) DestroyImmediate(spCol);

            // Cuerpo de piedra tallada Tsukubai
            Mesh basinMesh = ZenBonsaiProceduralMeshes.CreateTokonamePotMesh("Mesh_Tsukubai_StoneBasin", 0.135f, 0.135f, 0.060f);
            GameObject basinBody = new GameObject("Tsukubai_StoneBody");
            basinBody.transform.SetParent(waterBowl.transform, false);
            basinBody.transform.localPosition = new Vector3(0f, 0.002f, 0f);
            basinBody.AddComponent<MeshFilter>().sharedMesh = basinMesh;
            basinBody.AddComponent<MeshRenderer>().sharedMaterial = stoneMat;

            // Superficie de agua cristalina (solo emite onda suave al tocarla con la mano, sin lanzar chorros)
            GameObject waterSurface = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            waterSurface.name = "WaterSurface_Interactive";
            waterSurface.transform.SetParent(waterBowl.transform, false);
            waterSurface.transform.localPosition = new Vector3(0f, 0.048f, 0f);
            waterSurface.transform.localScale = new Vector3(0.110f, 0.006f, 0.110f);
            waterSurface.GetComponent<Renderer>().sharedMaterial = waterMat;

            var waterCol = waterSurface.GetComponent<Collider>();
            if (waterCol != null) waterCol.isTrigger = true;
            var waterRb = waterSurface.AddComponent<Rigidbody>();
            waterRb.isKinematic = true;
            waterRb.useGravity = false;
            waterSurface.AddComponent<ZenWaterSource>();

            // Caño tradicional de bambú japonés (Kakehi) en la parte trasera-izquierda de la fuente
            GameObject kakehiSpout = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            kakehiSpout.name = "Tsukubai_BambooSpout";
            kakehiSpout.transform.SetParent(waterBowl.transform, false);
            kakehiSpout.transform.localPosition = new Vector3(-0.052f, 0.076f, 0.045f);
            kakehiSpout.transform.localRotation = Quaternion.Euler(68f, 130f, 0f);
            kakehiSpout.transform.localScale = new Vector3(0.018f, 0.046f, 0.018f);
            kakehiSpout.GetComponent<Renderer>().sharedMaterial = bambooMat;
            var kc = kakehiSpout.GetComponent<Collider>(); if (kc != null) DestroyImmediate(kc);

            // =========================================================================
            // CAZO DE BAMBÚ ARTESANAL ESCULPIDO (HISHAKU) ORIENTADO HACIA EL JUGADOR
            // Cuenco hueco tallado con borde biselado, nudos de bambú y espiga de unión realista
            // =========================================================================
            GameObject ladleSlot = new GameObject("Slot_TsukubaiLadle");
            ladleSlot.transform.SetParent(waterBowl.transform, false);
            ladleSlot.transform.localPosition = new Vector3(0.008f, 0.068f, -0.012f);
            ladleSlot.transform.localRotation = Quaternion.Euler(-12f, -28f, 0f);

            GameObject ladleObj = new GameObject("Tool_BambooLadle_Hishaku");
            ladleObj.transform.SetParent(ladleSlot.transform, false);

            var ladleBox = ladleObj.AddComponent<BoxCollider>();
            ladleBox.size = new Vector3(0.11f, 0.08f, 0.19f);
            ladleBox.isTrigger = true;
            var ladleRb = ladleObj.AddComponent<Rigidbody>();
            ladleRb.isKinematic = true;

            ZenBonsaiTool ladleTool = ladleObj.AddComponent<ZenBonsaiTool>();
            ladleTool.toolType = BonsaiToolType.WaterLadle;
            ladleTool.traySlot = ladleSlot.transform;
            ladleTool.grabDistance = 0.125f;

            // Malla esculpida 3D del cazo Hishaku (mango de bambú con nudos + cuenco ahuecado de pared fina)
            Mesh hishakuBambooMesh = ZenBonsaiProceduralMeshes.CreateSculptedHishakuLadleMesh("Mesh_Tool_HishakuLadle", out Mesh hishakuWaterMesh);
            GameObject ladleCup = new GameObject("Hishaku_BambooCup");
            ladleCup.transform.SetParent(ladleObj.transform, false);
            ladleCup.transform.localPosition = Vector3.zero;
            ladleCup.AddComponent<MeshFilter>().sharedMesh = hishakuBambooMesh;
            ladleCup.AddComponent<MeshRenderer>().sharedMaterial = bambooMat;

            GameObject cupWater = new GameObject("Hishaku_WaterMeniscus");
            cupWater.transform.SetParent(ladleCup.transform, false);
            cupWater.transform.localPosition = Vector3.zero;
            cupWater.AddComponent<MeshFilter>().sharedMesh = hishakuWaterMesh;
            cupWater.AddComponent<MeshRenderer>().sharedMaterial = waterMat;

            GameObject ladleHandleAnchor = new GameObject("HandleAnchor");
            ladleHandleAnchor.transform.SetParent(ladleObj.transform, false);
            ladleHandleAnchor.transform.localPosition = new Vector3(0f, 0f, -0.045f);
            ladleTool.handleAnchor = ladleHandleAnchor.transform;

            // Punto exacto de vertido en el labio delantero del cuenco de bambú
            GameObject ladlePourPoint = new GameObject("LadlePourPoint");
            ladlePourPoint.transform.SetParent(ladleObj.transform, false);
            ladlePourPoint.transform.localPosition = new Vector3(0f, 0.012f, 0.076f);
            ladleTool.actionPoint = ladlePourPoint.transform;

            // Piedras de río y musgo alrededor de la fuente
            for (int i = 0; i < 4; i++)
            {
                GameObject pebble = new GameObject($"Tsukubai_Pebble_{i}");
                pebble.transform.SetParent(waterBowl.transform, false);
                float ang = (i * 92f + 18f) * Mathf.Deg2Rad;
                pebble.transform.localPosition = new Vector3(Mathf.Cos(ang) * 0.078f, 0.008f, Mathf.Sin(ang) * 0.078f);
                pebble.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh($"Mesh_Pebble_{i}", new Vector3(0.024f, 0.015f, 0.020f), 80 + i);
                pebble.AddComponent<MeshRenderer>().sharedMaterial = (i % 2 == 1) ? mossMat : stoneMat;
            }
        }

        #endregion

        #region 3. Construcción de la Maceta Tokoname y Suelo Orgánico Esculpido

        private static void BuildBonsaiPot(Transform parent, Material potMat, Material soilMat, Material mossDeepMat, Material mossLightMat, Material stoneMat, Material goldMat)
        {
            GameObject potRoot = new GameObject("BonsaiPot_Traditional");
            potRoot.transform.SetParent(parent, false);
            potRoot.transform.localPosition = new Vector3(0f, -0.042f, 0f);

            // Maceta cerámica japonesa Tokoname con paredes inclinadas y labio torneado
            Mesh potMesh = ZenBonsaiProceduralMeshes.CreateTokonamePotMesh("Mesh_Tokoname_BonsaiPot", 0.34f, 0.25f, 0.068f);
            GameObject potBody = new GameObject("Pot_Tokoname_Sculpted");
            potBody.transform.SetParent(potRoot.transform, false);
            potBody.transform.localPosition = Vector3.zero;
            potBody.AddComponent<MeshFilter>().sharedMesh = potMesh;
            potBody.AddComponent<MeshRenderer>().sharedMaterial = potMat;

            // 4 Patas cerámicas esculpidas (Unryu)
            Vector3[] feetOffsets =
            {
                new Vector3(-0.125f, -0.006f, -0.088f),
                new Vector3(0.125f, -0.006f, -0.088f),
                new Vector3(-0.125f, -0.006f, 0.088f),
                new Vector3(0.125f, -0.006f, 0.088f)
            };
            for (int i = 0; i < feetOffsets.Length; i++)
            {
                GameObject foot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                foot.name = $"Pot_Foot_{i}";
                foot.transform.SetParent(potRoot.transform, false);
                foot.transform.localPosition = feetOffsets[i];
                foot.transform.localScale = new Vector3(0.042f, 0.014f, 0.036f);
                foot.GetComponent<Renderer>().sharedMaterial = potMat;
                var fc = foot.GetComponent<Collider>(); if (fc != null) DestroyImmediate(fc);
            }

            // Manto continuo ondulado de musgo japonés (Koke) que asciende hacia el Nebari
            Mesh mossMoundMesh = ZenBonsaiProceduralMeshes.CreateOrganicMossMoundMesh("Mesh_Pot_MossMound", 0.305f, 0.220f, 0.056f, 0.026f);
            GameObject mossMound = new GameObject("Bonsai_MossTerrain");
            mossMound.transform.SetParent(potRoot.transform, false);
            mossMound.AddComponent<MeshFilter>().sharedMesh = mossMoundMesh;
            mossMound.AddComponent<MeshRenderer>().sharedMaterial = mossDeepMat;

            // Almohadillas de musgo esmeralda claro de relieve orgánico
            Vector3[] cushionPos =
            {
                new Vector3(-0.075f, 0.068f, -0.045f),
                new Vector3(0.068f, 0.067f, -0.038f),
                new Vector3(0.052f, 0.069f, 0.048f),
                new Vector3(-0.058f, 0.068f, 0.042f),
                new Vector3(0.010f, 0.072f, -0.058f)
            };
            for (int i = 0; i < cushionPos.Length; i++)
            {
                GameObject cushion = new GameObject($"Moss_VelvetCushion_{i}");
                cushion.transform.SetParent(potRoot.transform, false);
                cushion.transform.localPosition = cushionPos[i];
                cushion.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                    $"Mesh_MossCushion_{i}", new Vector3(0.046f, 0.012f, 0.040f), 200 + i * 7);
                cushion.AddComponent<MeshRenderer>().sharedMaterial = mossLightMat;
            }

            // Rocas minerales Suiseki talladas con facetas naturales
            Vector3[] stonePositions =
            {
                new Vector3(-0.095f, 0.068f, 0.048f),
                new Vector3(0.088f, 0.066f, -0.052f),
                new Vector3(0.032f, 0.068f, 0.065f)
            };
            Vector3[] stoneRadii =
            {
                new Vector3(0.028f, 0.020f, 0.022f),
                new Vector3(0.024f, 0.016f, 0.020f),
                new Vector3(0.018f, 0.012f, 0.016f)
            };
            for (int i = 0; i < stonePositions.Length; i++)
            {
                GameObject stone = new GameObject($"Suiseki_Rock_{i}");
                stone.transform.SetParent(potRoot.transform, false);
                stone.transform.localPosition = stonePositions[i];
                stone.transform.localRotation = Quaternion.Euler(0f, i * 67f + 25f, 0f);
                stone.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                    $"Mesh_SuisekiRock_{i}", stoneRadii[i], 300 + i * 19);
                stone.AddComponent<MeshRenderer>().sharedMaterial = stoneMat;
            }
        }

        #endregion

        #region 1. SAKURA (Cerezo Imperial Florido - Escultura Ancestral Moyogi, 8 Ramas y Lluvia de Pétalos)

        private static SpeciesTreeModel BuildSakuraTree(Transform parent, Material barkMat, Material shariMat, Material mossMat, Material petalPinkMat, Material petalWhiteMat, Material leafShootMat, Material goldMat)
        {
            SpeciesTreeModel model = new SpeciesTreeModel();
            model.treeType = TreeType.Sakura;

            GameObject treeObj = new GameObject("Tree_Sakura_Authentic");
            treeObj.transform.SetParent(parent, false);
            model.treeRoot = treeObj;

            GameObject trunk = new GameObject("SakuraTrunk_Sculpted");
            trunk.transform.SetParent(treeObj.transform, false);
            model.trunkBase = trunk.transform;

            // 8 Ramas maestras arquitectónicas formando una cúpula imperial plena (Chokkan / Moyogi)
            BranchSpline[] sakuraBranches = new BranchSpline[]
            {
                // Rama 0: Ichi-no-eda (Izquierda baja prominente y extendida)
                new BranchSpline(
                    new Vector3(-0.014f, 0.112f, 0.006f),
                    new Vector3(-0.055f, 0.122f, 0.014f),
                    new Vector3(-0.098f, 0.130f, 0.018f),
                    new Vector3(-0.138f, 0.142f, 0.022f),
                    0.019f, 0.0065f),

                // Rama 1: Ni-no-eda (Derecha-trasera elegante y equilibrada)
                new BranchSpline(
                    new Vector3(0.014f, 0.142f, -0.008f),
                    new Vector3(0.058f, 0.154f, -0.015f),
                    new Vector3(0.098f, 0.165f, -0.020f),
                    new Vector3(0.135f, 0.178f, -0.022f),
                    0.018f, 0.0060f),

                // Rama 2: Ura-eda (Trasera de gran profundidad espacial 3D)
                new BranchSpline(
                    new Vector3(0.004f, 0.168f, 0.012f),
                    new Vector3(0.002f, 0.182f, 0.048f),
                    new Vector3(-0.008f, 0.196f, 0.082f),
                    new Vector3(-0.012f, 0.212f, 0.110f),
                    0.016f, 0.0055f),

                // Rama 3: Mae-eda (Frontal-derecha enmarcando el tronco)
                new BranchSpline(
                    new Vector3(0.010f, 0.185f, -0.010f),
                    new Vector3(0.038f, 0.198f, -0.032f),
                    new Vector3(0.065f, 0.208f, -0.055f),
                    new Vector3(0.088f, 0.218f, -0.072f),
                    0.015f, 0.0050f),

                // Rama 4: Chu-eda (Izquierda media-alta extendida)
                new BranchSpline(
                    new Vector3(-0.008f, 0.198f, -0.004f),
                    new Vector3(-0.042f, 0.212f, -0.018f),
                    new Vector3(-0.072f, 0.224f, -0.028f),
                    new Vector3(-0.098f, 0.238f, -0.035f),
                    0.015f, 0.0050f),

                // Rama 5: Ko-eda (Trasera-izquierda superior de relleno)
                new BranchSpline(
                    new Vector3(-0.004f, 0.215f, 0.008f),
                    new Vector3(-0.024f, 0.228f, 0.034f),
                    new Vector3(-0.044f, 0.240f, 0.058f),
                    new Vector3(-0.058f, 0.252f, 0.076f),
                    0.014f, 0.0048f),

                // Rama 6: Mae-ko-eda (Frontal superior de cúpula)
                new BranchSpline(
                    new Vector3(0.006f, 0.230f, -0.005f),
                    new Vector3(0.018f, 0.244f, -0.024f),
                    new Vector3(0.028f, 0.256f, -0.042f),
                    new Vector3(0.035f, 0.268f, -0.054f),
                    0.013f, 0.0045f),

                // Rama 7: Jushin (Ápice imperial, corona superior abovedada)
                new BranchSpline(
                    new Vector3(0.006f, 0.245f, 0.003f),
                    new Vector3(0.008f, 0.262f, 0.002f),
                    new Vector3(0.010f, 0.278f, 0.001f),
                    new Vector3(0.012f, 0.294f, 0.000f),
                    0.015f, 0.0050f)
            };

            // Tronco maestro antiguo y nudoso con curvatura Moyogi y 10 raíces Nebari
            Mesh sakuraWoodMesh = ZenBonsaiProceduralMeshes.CreateMasterTreeWoodMesh(
                "Mesh_Sakura_WoodMasterpiece",
                new Vector3(0.000f, 0.026f, 0.000f),
                new Vector3(-0.045f, 0.108f, 0.016f),
                new Vector3(0.038f, 0.185f, -0.014f),
                new Vector3(0.008f, 0.255f, 0.004f),
                0.052f, 0.029f, 0.013f,
                sakuraBranches,
                10, 1.05f, 0.18f, 7.2f, 101);

            trunk.AddComponent<MeshFilter>().sharedMesh = sakuraWoodMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = barkMat;

            // 6 Almohadillas de musgo aterciopelado abrazando el amplio perímetro Nebari
            for (int m = 0; m < 6; m++)
            {
                float ang = (m * 60f + 15f) * Mathf.Deg2Rad;
                GameObject rootMoss = new GameObject($"Sakura_NebariMoss_{m}");
                rootMoss.transform.SetParent(trunk.transform, false);
                rootMoss.transform.localPosition = new Vector3(Mathf.Cos(ang) * 0.034f, 0.040f, Mathf.Sin(ang) * 0.034f);
                rootMoss.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                    $"Mesh_SakuraRootMoss_{m}", new Vector3(0.024f, 0.012f, 0.021f), 170 + m);
                rootMoss.AddComponent<MeshRenderer>().sharedMaterial = mossMat;
            }

            Vector3[] padScales =
            {
                new Vector3(0.145f, 0.052f, 0.125f),
                new Vector3(0.140f, 0.050f, 0.120f),
                new Vector3(0.130f, 0.048f, 0.115f),
                new Vector3(0.118f, 0.044f, 0.105f),
                new Vector3(0.128f, 0.046f, 0.112f),
                new Vector3(0.118f, 0.044f, 0.108f),
                new Vector3(0.122f, 0.045f, 0.110f),
                new Vector3(0.138f, 0.054f, 0.128f)
            };

            List<Transform> nodes = new List<Transform>();
            for (int i = 0; i < sakuraBranches.Length; i++)
            {
                Vector3 padPos = sakuraBranches[i].p3 + new Vector3(0f, 0.010f, 0f);
                Vector3 pScale = padScales[i];

                GameObject foliagePad = new GameObject($"Sakura_CanopyPad_{i}");
                foliagePad.transform.SetParent(trunk.transform, false);
                foliagePad.transform.localPosition = padPos;

                Mesh padMesh = ZenBonsaiProceduralMeshes.CreateFoliageCloudPadMesh(
                    $"Mesh_Sakura_CloudPad_{i}", pScale, 16, 0.74f, 0.16f, 400 + i * 23);

                foliagePad.AddComponent<MeshFilter>().sharedMesh = padMesh;
                MeshRenderer padRen = foliagePad.AddComponent<MeshRenderer>();
                padRen.sharedMaterial = (i % 3 == 0) ? leafShootMat : petalPinkMat;

                SphereCollider fCol = foliagePad.AddComponent<SphereCollider>();
                fCol.radius = pScale.x * 0.45f;
                fCol.isTrigger = true;
                Rigidbody fRb = foliagePad.AddComponent<Rigidbody>();
                fRb.isKinematic = true;

                PrunableBranch pb = foliagePad.AddComponent<PrunableBranch>();
                pb.ownerSpecies = TreeType.Sakura;
                pb.branchRenderer = padRen;
                pb.healthyColor = padRen.sharedMaterial.color;
                pb.witheredColor = new Color(0.78f, 0.58f, 0.18f);

                model.prunableBranches.Add(pb);
                nodes.Add(foliagePad.transform);

                // Gran densidad de corolas de 5 pétalos con muesca (68 a 78 por nube, más de 580 flores 3D en total)
                int corollaCount = (i == 7 || i == 0 || i == 1) ? 78 : 68;
                Mesh blossomMesh = ZenBonsaiProceduralMeshes.CreateSakuraBlossomCorollasMesh(
                    $"Mesh_Sakura_Corollas_{i}", pScale, corollaCount, 500 + i * 31, out Mesh stamenMesh);

                GameObject corollasObj = new GameObject($"Sakura_Blossoms5Petal_{i}");
                corollasObj.transform.SetParent(foliagePad.transform, false);
                corollasObj.AddComponent<MeshFilter>().sharedMesh = blossomMesh;
                corollasObj.AddComponent<MeshRenderer>().sharedMaterial = (i % 2 == 0) ? petalPinkMat : petalWhiteMat;

                GameObject stamensObj = new GameObject($"Sakura_GoldStamens_{i}");
                stamensObj.transform.SetParent(foliagePad.transform, false);
                stamensObj.AddComponent<MeshFilter>().sharedMesh = stamenMesh;
                stamensObj.AddComponent<MeshRenderer>().sharedMaterial = goldMat;
            }

            // 1. Pétalos enteros sobre el manto de musgo de la maceta
            Mesh fallenBlossomsMesh = ZenBonsaiProceduralMeshes.CreateSakuraBlossomCorollasMesh(
                "Mesh_Sakura_FallenPetals_Moss", new Vector3(0.25f, 0.008f, 0.18f), 14, 777, out Mesh _);
            GameObject fallenMossObj = new GameObject("Sakura_FallenBlossoms_OnMoss");
            fallenMossObj.transform.SetParent(treeObj.transform, false);
            fallenMossObj.transform.localPosition = new Vector3(0f, 0.038f, 0f);
            fallenMossObj.AddComponent<MeshFilter>().sharedMesh = fallenBlossomsMesh;
            fallenMossObj.AddComponent<MeshRenderer>().sharedMaterial = petalPinkMat;

            // 2. Pétalos sueltos esparcidos poéticamente sobre el lacado brillante de la mesa Urushi
            Mesh scatteredTablePetalsMesh = ZenBonsaiProceduralMeshes.CreateScatteredTablePetalsMesh(
                "Mesh_Sakura_TablePetals", 24, 888);
            GameObject tablePetalsObj = new GameObject("Sakura_FallenPetals_OnTable");
            tablePetalsObj.transform.SetParent(treeObj.transform, false);
            tablePetalsObj.transform.localPosition = new Vector3(0f, -0.046f, 0f);
            tablePetalsObj.AddComponent<MeshFilter>().sharedMesh = scatteredTablePetalsMesh;
            tablePetalsObj.AddComponent<MeshRenderer>().sharedMaterial = petalPinkMat;

            // Floración Plena (Recompensa Full Bloom)
            GameObject blossomRewardObj = new GameObject("Sakura_FullBloom_Reward");
            blossomRewardObj.transform.SetParent(treeObj.transform, false);
            for (int i = 0; i < sakuraBranches.Length; i++)
            {
                Mesh extraBloom = ZenBonsaiProceduralMeshes.CreateSakuraBlossomCorollasMesh(
                    $"Mesh_Sakura_FullBloom_{i}", padScales[i] * 1.15f, 26, 880 + i * 17, out Mesh _);
                GameObject bloomLayer = new GameObject($"BloomCrown_{i}");
                bloomLayer.transform.SetParent(blossomRewardObj.transform, false);
                bloomLayer.transform.localPosition = sakuraBranches[i].p3 + new Vector3(0f, 0.014f, 0f);
                bloomLayer.AddComponent<MeshFilter>().sharedMesh = extraBloom;
                bloomLayer.AddComponent<MeshRenderer>().sharedMaterial = petalPinkMat;
            }
            blossomRewardObj.SetActive(false);
            model.blossomContainer = blossomRewardObj;
            model.leafNodes = nodes.ToArray();
            return model;
        }

        #endregion

        #region 2. MATSU (Pino Negro Ancestral Kuromatsu - Corteza Escamosa, Vena Shari y Acículas Matsuba)

        private static SpeciesTreeModel BuildMatsuTree(Transform parent, Material barkMat, Material needleDeepMat, Material needleTipMat, Material jinMat, Material stoneMat, Material mossMat)
        {
            SpeciesTreeModel model = new SpeciesTreeModel();
            model.treeType = TreeType.Matsu;

            GameObject treeObj = new GameObject("Tree_Matsu_Authentic");
            treeObj.transform.SetParent(parent, false);
            model.treeRoot = treeObj;

            GameObject trunk = new GameObject("MatsuTrunk_Sculpted");
            trunk.transform.SetParent(treeObj.transform, false);
            model.trunkBase = trunk.transform;

            // Roca Suiseki principal abrazada por las raíces del pino (Estilo Ishitsuki)
            GameObject cliffRock = new GameObject("Matsu_Ishitsuki_Rock");
            cliffRock.transform.SetParent(trunk.transform, false);
            cliffRock.transform.localPosition = new Vector3(0.018f, 0.045f, -0.010f);
            cliffRock.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                "Mesh_Matsu_CliffRock", new Vector3(0.056f, 0.040f, 0.048f), 901);
            cliffRock.AddComponent<MeshRenderer>().sharedMaterial = stoneMat;

            BranchSpline[] matsuBranches = new BranchSpline[]
            {
                // 1. Gran rama caída derecha-frontal (Sashi-eda)
                new BranchSpline(
                    new Vector3(0.015f, 0.112f, -0.005f),
                    new Vector3(0.062f, 0.105f, -0.015f),
                    new Vector3(0.105f, 0.108f, -0.018f),
                    new Vector3(0.135f, 0.118f, -0.015f),
                    0.019f, 0.0065f),
                // 2. Rama izquierda media robusta
                new BranchSpline(
                    new Vector3(-0.018f, 0.145f, 0.008f),
                    new Vector3(-0.065f, 0.142f, 0.016f),
                    new Vector3(-0.102f, 0.148f, 0.020f),
                    new Vector3(-0.128f, 0.156f, 0.022f),
                    0.017f, 0.0060f),
                // 3. Rama trasera de contrapeso
                new BranchSpline(
                    new Vector3(0.000f, 0.175f, 0.012f),
                    new Vector3(0.005f, 0.180f, 0.048f),
                    new Vector3(0.008f, 0.185f, 0.080f),
                    new Vector3(0.010f, 0.194f, 0.105f),
                    0.015f, 0.0055f),
                // 4. Rama superior derecha
                new BranchSpline(
                    new Vector3(0.008f, 0.205f, -0.005f),
                    new Vector3(0.042f, 0.210f, -0.018f),
                    new Vector3(0.072f, 0.216f, -0.025f),
                    new Vector3(0.092f, 0.224f, -0.028f),
                    0.013f, 0.0050f),
                // 5. Corona triangular del pino (Ápice)
                new BranchSpline(
                    new Vector3(-0.005f, 0.225f, 0.005f),
                    new Vector3(-0.008f, 0.242f, 0.002f),
                    new Vector3(-0.004f, 0.256f, 0.000f),
                    new Vector3(0.000f, 0.268f, 0.000f),
                    0.014f, 0.0055f)
            };

            Mesh matsuWoodMesh = ZenBonsaiProceduralMeshes.CreateMasterTreeWoodMesh(
                "Mesh_Matsu_WoodMasterpiece",
                new Vector3(0.000f, 0.026f, 0.000f),
                new Vector3(-0.048f, 0.108f, 0.016f),
                new Vector3(0.042f, 0.172f, -0.014f),
                new Vector3(-0.006f, 0.245f, 0.005f),
                0.042f, 0.026f, 0.012f,
                matsuBranches,
                9, 0.92f, 0.20f, 9.5f, 202);

            trunk.AddComponent<MeshFilter>().sharedMesh = matsuWoodMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = barkMat;

            // Musgo esmeralda entre las raíces Nebari y la roca del pino negro
            for (int m = 0; m < 4; m++)
            {
                float ang = (m * 90f + 40f) * Mathf.Deg2Rad;
                GameObject rootMoss = new GameObject($"Matsu_NebariMoss_{m}");
                rootMoss.transform.SetParent(trunk.transform, false);
                rootMoss.transform.localPosition = new Vector3(Mathf.Cos(ang) * 0.028f, 0.042f, Mathf.Sin(ang) * 0.028f);
                rootMoss.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                    $"Mesh_MatsuRootMoss_{m}", new Vector3(0.024f, 0.012f, 0.020f), 270 + m);
                rootMoss.AddComponent<MeshRenderer>().sharedMaterial = mossMat;
            }

            // Platos horizontales esculpidos de acículas (Dan-zukuri)
            Vector3[] padScales =
            {
                new Vector3(0.145f, 0.036f, 0.115f),
                new Vector3(0.138f, 0.035f, 0.110f),
                new Vector3(0.118f, 0.032f, 0.098f),
                new Vector3(0.112f, 0.032f, 0.095f),
                new Vector3(0.125f, 0.038f, 0.108f)
            };

            List<Transform> nodes = new List<Transform>();
            for (int i = 0; i < matsuBranches.Length; i++)
            {
                Vector3 padPos = matsuBranches[i].p3 + new Vector3(0f, 0.006f, 0f);
                Vector3 pScale = padScales[i];

                GameObject needlePlate = new GameObject($"Matsu_NeedlePlate_{i}");
                needlePlate.transform.SetParent(trunk.transform, false);
                needlePlate.transform.localPosition = padPos;

                Mesh plateMesh = ZenBonsaiProceduralMeshes.CreateFoliageCloudPadMesh(
                    $"Mesh_Matsu_Plate_{i}", pScale, 16, 0.88f, 0.22f, 600 + i * 29);

                needlePlate.AddComponent<MeshFilter>().sharedMesh = plateMesh;
                MeshRenderer plateRen = needlePlate.AddComponent<MeshRenderer>();
                plateRen.sharedMaterial = needleDeepMat;

                SphereCollider fCol = needlePlate.AddComponent<SphereCollider>();
                fCol.radius = pScale.x * 0.45f;
                fCol.isTrigger = true;
                Rigidbody fRb = needlePlate.AddComponent<Rigidbody>();
                fRb.isKinematic = true;

                PrunableBranch pb = needlePlate.AddComponent<PrunableBranch>();
                pb.ownerSpecies = TreeType.Matsu;
                pb.branchRenderer = plateRen;
                pb.healthyColor = needleDeepMat.color;
                pb.witheredColor = new Color(0.68f, 0.44f, 0.15f);

                model.prunableBranches.Add(pb);
                nodes.Add(needlePlate.transform);

                // Fascículos radiantes de agujas de pino (Matsuba) sobre cada plato
                Mesh needlesMesh = ZenBonsaiProceduralMeshes.CreateMatsuNeedleClustersMesh(
                    $"Mesh_Matsu_Needles_{i}", pScale, 54, 700 + i * 19);

                GameObject tuftsObj = new GameObject($"Matsu_NeedleTufts_{i}");
                tuftsObj.transform.SetParent(needlePlate.transform, false);
                tuftsObj.AddComponent<MeshFilter>().sharedMesh = needlesMesh;
                tuftsObj.AddComponent<MeshRenderer>().sharedMaterial = needleTipMat;
            }

            model.leafNodes = nodes.ToArray();
            return model;
        }

        #endregion

        #region 3. MOMIJI (Arce Japonés Otoñal Doble Tronco Sokan - Hojas Estrelladas de 5 Puntas)

        private static SpeciesTreeModel BuildMomijiTree(Transform parent, Material barkMat, Material shariMat, Material mossMat, Material rubyMat, Material scarletMat, Material amberMat)
        {
            SpeciesTreeModel model = new SpeciesTreeModel();
            model.treeType = TreeType.Momiji;

            GameObject treeObj = new GameObject("Tree_Momiji_Authentic");
            treeObj.transform.SetParent(parent, false);
            model.treeRoot = treeObj;

            GameObject trunk = new GameObject("MomijiTrunk_Sculpted");
            trunk.transform.SetParent(treeObj.transform, false);
            model.trunkBase = trunk.transform;

            // Ramificación fina en abanico japonés con Doble Tronco (Estilo Sokan: tronco padre + tronco hijo desde la raíz)
            BranchSpline[] momijiBranches = new BranchSpline[]
            {
                // 1. Tronco Hijo Sokan emergiendo desde la base Nebari hacia la izquierda-frontal
                new BranchSpline(
                    new Vector3(-0.012f, 0.052f, -0.004f),
                    new Vector3(-0.055f, 0.095f, -0.012f),
                    new Vector3(-0.092f, 0.132f, -0.016f),
                    new Vector3(-0.118f, 0.162f, -0.018f),
                    0.018f, 0.0050f),
                // 2. Abanico derecho medio
                new BranchSpline(
                    new Vector3(0.010f, 0.132f, 0.000f),
                    new Vector3(0.048f, 0.154f, -0.008f),
                    new Vector3(0.085f, 0.178f, -0.012f),
                    new Vector3(0.115f, 0.198f, -0.015f),
                    0.014f, 0.0044f),
                // 3. Abanico izquierdo alto
                new BranchSpline(
                    new Vector3(-0.006f, 0.162f, 0.006f),
                    new Vector3(-0.042f, 0.182f, 0.018f),
                    new Vector3(-0.075f, 0.202f, 0.025f),
                    new Vector3(-0.098f, 0.218f, 0.028f),
                    0.012f, 0.0040f),
                // 4. Abanico trasero-derecho de profundidad
                new BranchSpline(
                    new Vector3(0.004f, 0.155f, 0.008f),
                    new Vector3(0.022f, 0.175f, 0.042f),
                    new Vector3(0.038f, 0.195f, 0.072f),
                    new Vector3(0.048f, 0.214f, 0.092f),
                    0.012f, 0.0040f),
                // 5. Corona central abovedada
                new BranchSpline(
                    new Vector3(0.002f, 0.175f, 0.000f),
                    new Vector3(0.006f, 0.202f, 0.002f),
                    new Vector3(-0.002f, 0.228f, -0.002f),
                    new Vector3(0.000f, 0.254f, 0.000f),
                    0.013f, 0.0042f)
            };

            Mesh momijiWoodMesh = ZenBonsaiProceduralMeshes.CreateMasterTreeWoodMesh(
                "Mesh_Momiji_WoodMasterpiece",
                new Vector3(0.000f, 0.026f, 0.000f),
                new Vector3(0.020f, 0.092f, 0.008f),
                new Vector3(-0.012f, 0.148f, -0.006f),
                new Vector3(0.004f, 0.205f, 0.000f),
                0.035f, 0.021f, 0.011f,
                momijiBranches,
                7, 0.78f, 0.11f, 5.5f, 303);

            trunk.AddComponent<MeshFilter>().sharedMesh = momijiWoodMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = barkMat;

            // Musgo esmeralda en la horquilla del doble tronco Sokan
            for (int m = 0; m < 4; m++)
            {
                float ang = (m * 90f + 35f) * Mathf.Deg2Rad;
                GameObject rootMoss = new GameObject($"Momiji_NebariMoss_{m}");
                rootMoss.transform.SetParent(trunk.transform, false);
                rootMoss.transform.localPosition = new Vector3(Mathf.Cos(ang) * 0.023f, 0.040f, Mathf.Sin(ang) * 0.023f);
                rootMoss.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                    $"Mesh_MomijiRootMoss_{m}", new Vector3(0.019f, 0.011f, 0.017f), 350 + m);
                rootMoss.AddComponent<MeshRenderer>().sharedMaterial = mossMat;
            }

            Vector3[] padScales =
            {
                new Vector3(0.125f, 0.042f, 0.105f),
                new Vector3(0.130f, 0.044f, 0.110f),
                new Vector3(0.118f, 0.040f, 0.102f),
                new Vector3(0.115f, 0.040f, 0.100f),
                new Vector3(0.145f, 0.052f, 0.135f)
            };

            List<Transform> nodes = new List<Transform>();
            for (int i = 0; i < momijiBranches.Length; i++)
            {
                Vector3 padPos = momijiBranches[i].p3 + new Vector3(0f, 0.008f, 0f);
                Vector3 pScale = padScales[i];

                GameObject mapleCanopy = new GameObject($"Momiji_Canopy_{i}");
                mapleCanopy.transform.SetParent(trunk.transform, false);
                mapleCanopy.transform.localPosition = padPos;

                Mesh canopyMesh = ZenBonsaiProceduralMeshes.CreateFoliageCloudPadMesh(
                    $"Mesh_Momiji_Canopy_{i}", pScale, 15, 0.75f, 0.18f, 800 + i * 37);

                mapleCanopy.AddComponent<MeshFilter>().sharedMesh = canopyMesh;
                MeshRenderer canRen = mapleCanopy.AddComponent<MeshRenderer>();
                canRen.sharedMaterial = (i % 2 == 0) ? scarletMat : rubyMat;

                SphereCollider fCol = mapleCanopy.AddComponent<SphereCollider>();
                fCol.radius = pScale.x * 0.45f;
                fCol.isTrigger = true;
                Rigidbody fRb = mapleCanopy.AddComponent<Rigidbody>();
                fRb.isKinematic = true;

                PrunableBranch pb = mapleCanopy.AddComponent<PrunableBranch>();
                pb.ownerSpecies = TreeType.Momiji;
                pb.branchRenderer = canRen;
                pb.healthyColor = canRen.sharedMaterial.color;
                pb.witheredColor = new Color(0.58f, 0.35f, 0.14f);

                model.prunableBranches.Add(pb);
                nodes.Add(mapleCanopy.transform);

                // Hojas palmadas de arce japonés en escarlata y ámbar dorado sobre cada copa
                Mesh starLeavesMesh = ZenBonsaiProceduralMeshes.CreateMomijiStarLeavesMesh(
                    $"Mesh_Momiji_StarLeaves_{i}", pScale, 44, 900 + i * 41);

                GameObject starLeavesObj = new GameObject($"Momiji_PalmateStarLeaves_{i}");
                starLeavesObj.transform.SetParent(mapleCanopy.transform, false);
                starLeavesObj.AddComponent<MeshFilter>().sharedMesh = starLeavesMesh;
                starLeavesObj.AddComponent<MeshRenderer>().sharedMaterial = (i % 2 == 0) ? amberMat : scarletMat;
            }

            // Hojas estrelladas de arce caídas sobre el musgo
            Mesh fallenMapleMesh = ZenBonsaiProceduralMeshes.CreateMomijiStarLeavesMesh(
                "Mesh_Momiji_FallenStarLeaves", new Vector3(0.22f, 0.008f, 0.16f), 12, 999);
            GameObject fallenMapleObj = new GameObject("Momiji_FallenLeaves_OnMoss");
            fallenMapleObj.transform.SetParent(treeObj.transform, false);
            fallenMapleObj.transform.localPosition = new Vector3(0f, 0.036f, 0f);
            fallenMapleObj.AddComponent<MeshFilter>().sharedMesh = fallenMapleMesh;
            fallenMapleObj.AddComponent<MeshRenderer>().sharedMaterial = amberMat;

            model.leafNodes = nodes.ToArray();
            return model;
        }

        #endregion

        #region 5. EXPOSITOR DE HERRAMIENTAS ORIENTADO AL JUGADOR + PLANTA DE ACENTO KUSAMONO DETRÁS

        private static void BuildToolTray(Transform parent, Material trayMat, Material darkWoodMat, Material steelMat, Material silverBladeMat, Material copperMat, Material bambooMat, Material bristleMat, Material goldMat, Material potMat, Material mossMat, Material stoneMat, Material blossomMat)
        {
            GameObject trayRoot = new GameObject("ZenToolTray");
            trayRoot.transform.SetParent(parent, false);
            // Misma orientación perfecta hacia el jugador (-14° X, +38° Y), pero adelantada hacia el jugador (Z = -0.075m)
            // y más compacta (solo 2 herramientas: Tijeras y Escobilla) para dejar totalmente visibles los adornos traseros
            trayRoot.transform.localPosition = new Vector3(0.39f, -0.015f, -0.075f);
            trayRoot.transform.localRotation = Quaternion.Euler(-14f, 38f, 0f);

            ZenToolTray trayComp = trayRoot.AddComponent<ZenToolTray>();

            // Bandeja artesana de ciprés Hinoki (26 cm de ancho) para las 2 herramientas (Tijeras y Escobilla)
            GameObject trayBoard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trayBoard.name = "Tray_Board";
            trayBoard.transform.SetParent(trayRoot.transform, false);
            trayBoard.transform.localPosition = Vector3.zero;
            trayBoard.transform.localScale = new Vector3(0.26f, 0.016f, 0.19f);
            trayBoard.GetComponent<Renderer>().sharedMaterial = trayMat;
            var tCol = trayBoard.GetComponent<Collider>(); if (tCol != null) DestroyImmediate(tCol);

            // Bordes laterales y separador central de madera oscura para delimitar visualmente las 2 herramientas
            float[] dividersX = { -0.125f, 0.000f, 0.125f };
            for (int d = 0; d < dividersX.Length; d++)
            {
                GameObject div = GameObject.CreatePrimitive(PrimitiveType.Cube);
                div.name = $"Tray_Divider_{d}";
                div.transform.SetParent(trayRoot.transform, false);
                div.transform.localPosition = new Vector3(dividersX[d], 0.011f, 0f);
                div.transform.localScale = new Vector3(0.008f, 0.010f, 0.18f);
                div.GetComponent<Renderer>().sharedMaterial = darkWoodMat;
                var dc = div.GetComponent<Collider>(); if (dc != null) DestroyImmediate(dc);
            }

            // =========================================================================
            // 1. IZQUIERDA (x = -0.064m): TIJERAS FORJADAS DE PODA (FIELES A LA IMAGEN DE PRESENTACIÓN)
            // Mangos contorneados en acero forjado, muelle helicoidal central, remache de latón y hojas biseladas puntiagudas
            // =========================================================================
            GameObject shearsSlot = new GameObject("Slot_Shears");
            shearsSlot.transform.SetParent(trayRoot.transform, false);
            shearsSlot.transform.localPosition = new Vector3(-0.064f, 0.024f, 0.0f);
            shearsSlot.transform.localRotation = Quaternion.identity;
            trayComp.shearsSlot = shearsSlot.transform;

            GameObject shearsObj = new GameObject("Tool_SatsukiShears");
            shearsObj.transform.SetParent(shearsSlot.transform, false);

            var shearsBox = shearsObj.AddComponent<BoxCollider>();
            shearsBox.size = new Vector3(0.08f, 0.05f, 0.15f);
            shearsBox.isTrigger = true;
            var shearsRb = shearsObj.AddComponent<Rigidbody>();
            shearsRb.isKinematic = true;

            ZenBonsaiTool shearsTool = shearsObj.AddComponent<ZenBonsaiTool>();
            shearsTool.toolType = BonsaiToolType.Shears;
            shearsTool.traySlot = shearsSlot.transform;
            shearsTool.grabDistance = 0.115f;
            trayComp.shears = shearsTool;

            // Cuerpo esculpido 3D: mangos ergonómicos de acero forjado + muelle helicoidal (Volute Spring) entre los mangos + perno central
            Mesh shearsHandlesMesh = ZenBonsaiProceduralMeshes.CreateSculptedShearsBodyMesh("Mesh_Tool_ShearsHandlesAndSpring", out Mesh shearsRivetMesh);
            GameObject shearsBodyObj = new GameObject("Shears_ForgedHandlesAndSpring");
            shearsBodyObj.transform.SetParent(shearsObj.transform, false);
            shearsBodyObj.AddComponent<MeshFilter>().sharedMesh = shearsHandlesMesh;
            shearsBodyObj.AddComponent<MeshRenderer>().sharedMaterial = steelMat;

            GameObject shearsRivetObj = new GameObject("Shears_CenterBrassPivot");
            shearsRivetObj.transform.SetParent(shearsObj.transform, false);
            shearsRivetObj.AddComponent<MeshFilter>().sharedMesh = shearsRivetMesh;
            shearsRivetObj.AddComponent<MeshRenderer>().sharedMaterial = goldMat;

            // Hojas de corte biseladas y puntiagudas en acero templado pulido (pivote en el centro z=0)
            Mesh bladeMeshL = ZenBonsaiProceduralMeshes.CreateSculptedShearBladeMesh("Mesh_Tool_ShearBladeL", true);
            Mesh bladeMeshR = ZenBonsaiProceduralMeshes.CreateSculptedShearBladeMesh("Mesh_Tool_ShearBladeR", false);

            GameObject bladeA = new GameObject("BladeA");
            bladeA.transform.SetParent(shearsObj.transform, false);
            bladeA.transform.localPosition = new Vector3(-0.004f, 0.0018f, 0.036f);
            bladeA.transform.localRotation = Quaternion.Euler(0f, 5f, 0f);
            bladeA.AddComponent<MeshFilter>().sharedMesh = bladeMeshL;
            bladeA.AddComponent<MeshRenderer>().sharedMaterial = silverBladeMat;

            GameObject bladeB = new GameObject("BladeB");
            bladeB.transform.SetParent(shearsObj.transform, false);
            bladeB.transform.localPosition = new Vector3(0.004f, -0.0018f, 0.036f);
            bladeB.transform.localRotation = Quaternion.Euler(0f, -5f, 0f);
            bladeB.AddComponent<MeshFilter>().sharedMesh = bladeMeshR;
            bladeB.AddComponent<MeshRenderer>().sharedMaterial = silverBladeMat;

            shearsTool.movingPartA = bladeA.transform;
            shearsTool.movingPartB = bladeB.transform;

            GameObject shearsHandleAnchor = new GameObject("HandleAnchor");
            shearsHandleAnchor.transform.SetParent(shearsObj.transform, false);
            shearsHandleAnchor.transform.localPosition = new Vector3(0f, 0f, -0.04f);
            shearsTool.handleAnchor = shearsHandleAnchor.transform;

            GameObject snipPoint = new GameObject("SnipPoint");
            snipPoint.transform.SetParent(shearsObj.transform, false);
            snipPoint.transform.localPosition = new Vector3(0f, 0f, 0.082f);
            shearsTool.actionPoint = snipPoint.transform;

            // =========================================================================
            // 2. DERECHA (x = +0.064m): ESCOBILLA ARTESANAL DE FIBRA DE PALMA Y BAMBÚ (SHURO HOKI)
            // Cabezal acampanado esculpido con hebras naturales, atado tradicional Maki y mango de bambú con nudos
            // =========================================================================
            GameObject broomSlot = new GameObject("Slot_BonsaiBroom");
            broomSlot.transform.SetParent(trayRoot.transform, false);
            broomSlot.transform.localPosition = new Vector3(0.064f, 0.026f, 0.0f);
            broomSlot.transform.localRotation = Quaternion.identity;
            trayComp.broomSlot = broomSlot.transform;

            GameObject broomObj = new GameObject("Tool_BonsaiBroom");
            broomObj.transform.SetParent(broomSlot.transform, false);

            var broomBox = broomObj.AddComponent<BoxCollider>();
            broomBox.size = new Vector3(0.09f, 0.07f, 0.16f);
            broomBox.isTrigger = true;
            var broomRb = broomObj.AddComponent<Rigidbody>();
            broomRb.isKinematic = true;

            ZenBonsaiTool broomTool = broomObj.AddComponent<ZenBonsaiTool>();
            broomTool.toolType = BonsaiToolType.Broom;
            broomTool.traySlot = broomSlot.transform;
            broomTool.grabDistance = 0.115f;
            trayComp.broom = broomTool;

            Mesh hokiStickMesh = ZenBonsaiProceduralMeshes.CreateSculptedHokiBroomMesh("Mesh_Tool_HokiBroom", out Mesh hokiBristlesMesh, out Mesh hokiCordMesh);
            GameObject bambooStick = new GameObject("Hoki_SegmentedBambooStick");
            bambooStick.transform.SetParent(broomObj.transform, false);
            bambooStick.AddComponent<MeshFilter>().sharedMesh = hokiStickMesh;
            bambooStick.AddComponent<MeshRenderer>().sharedMaterial = bambooMat;

            GameObject goldFerrule = new GameObject("Hoki_BraidedBinding");
            goldFerrule.transform.SetParent(broomObj.transform, false);
            goldFerrule.AddComponent<MeshFilter>().sharedMesh = hokiCordMesh;
            goldFerrule.AddComponent<MeshRenderer>().sharedMaterial = goldMat;

            GameObject bristles = new GameObject("Hoki_ShuroPalmBristles");
            bristles.transform.SetParent(broomObj.transform, false);
            bristles.AddComponent<MeshFilter>().sharedMesh = hokiBristlesMesh;
            bristles.AddComponent<MeshRenderer>().sharedMaterial = bristleMat;

            GameObject broomHandleAnchor = new GameObject("HandleAnchor");
            broomHandleAnchor.transform.SetParent(broomObj.transform, false);
            broomHandleAnchor.transform.localPosition = new Vector3(0f, 0f, -0.035f);
            broomTool.handleAnchor = broomHandleAnchor.transform;

            GameObject bristleTip = new GameObject("BristleTip");
            bristleTip.transform.SetParent(broomObj.transform, false);
            bristleTip.transform.localPosition = new Vector3(0f, 0f, 0.082f);
            broomTool.actionPoint = bristleTip.transform;

            // =========================================================================
            // 3. COMPOSICIÓN JAPONESA KUSAMONO Y SUISEKI DETRÁS DE LAS HERRAMIENTAS (DESPEJADOS)
            // =========================================================================
            GameObject kusamonoRoot = new GameObject("ZenAccent_KusamonoDisplay");
            kusamonoRoot.transform.SetParent(parent, false);
            kusamonoRoot.transform.localPosition = new Vector3(0.24f, -0.042f, 0.17f);
            kusamonoRoot.transform.localRotation = Quaternion.Euler(0f, 20f, 0f);

            Mesh miniPotMesh = ZenBonsaiProceduralMeshes.CreateTokonamePotMesh("Mesh_Kusamono_MiniPot", 0.115f, 0.095f, 0.032f);
            GameObject miniPot = new GameObject("Kusamono_CeramicDish");
            miniPot.transform.SetParent(kusamonoRoot.transform, false);
            miniPot.AddComponent<MeshFilter>().sharedMesh = miniPotMesh;
            miniPot.AddComponent<MeshRenderer>().sharedMaterial = potMat;

            GameObject miniMoss = new GameObject("Kusamono_MossMound");
            miniMoss.transform.SetParent(kusamonoRoot.transform, false);
            miniMoss.transform.localPosition = new Vector3(0f, 0.028f, 0f);
            miniMoss.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                "Mesh_Kusamono_Moss", new Vector3(0.044f, 0.018f, 0.036f), 512);
            miniMoss.AddComponent<MeshRenderer>().sharedMaterial = mossMat;

            GameObject miniRock = new GameObject("Kusamono_SuisekiStone");
            miniRock.transform.SetParent(kusamonoRoot.transform, false);
            miniRock.transform.localPosition = new Vector3(0.018f, 0.036f, -0.008f);
            miniRock.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                "Mesh_Kusamono_Rock", new Vector3(0.022f, 0.026f, 0.018f), 524);
            miniRock.AddComponent<MeshRenderer>().sharedMaterial = stoneMat;

            Mesh miniBlossoms = ZenBonsaiProceduralMeshes.CreateSakuraBlossomCorollasMesh(
                "Mesh_Kusamono_Blossoms", new Vector3(0.065f, 0.025f, 0.055f), 8, 555, out Mesh _);
            GameObject miniFlowers = new GameObject("Kusamono_AccentBlossoms");
            miniFlowers.transform.SetParent(kusamonoRoot.transform, false);
            miniFlowers.transform.localPosition = new Vector3(-0.012f, 0.044f, 0.005f);
            miniFlowers.AddComponent<MeshFilter>().sharedMesh = miniBlossoms;
            miniFlowers.AddComponent<MeshRenderer>().sharedMaterial = blossomMat;
        }

        #endregion

        #region Materiales, Texturas de Corteza Orgánica de Alta Definición y Botones con Neón

        private static Material CreateOrGetPBRMaterial(string matName, Color color, float smoothness = 0.5f, float metallic = 0.0f)
        {
            string path = $"Assets/ZenBonsai/Materials/{matName}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.color = color;
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", smoothness);
            mat.SetFloat("_Metallic", metallic);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        /// <summary>
        /// Genera un material PBR con textura Albedo 512x512 + Mapa de Normales 512x512 (_BumpMap)
        /// para cerámica esmaltada Tokoname (10), musgo/roca (11), pétalos Sakura con cáliz (12),
        /// hojas de arce con nervaduras (13), acero forjado bruñido (14), bambú natural (15) y grava Karesansui (16).
        /// </summary>
        private static Material CreateOrGetSurfaceTexturedMaterial(
            string matName, Color baseColor, Color darkAccent, Color lightAccent,
            float smoothness, float metallic, int style)
        {
            Material mat = CreateOrGetPBRMaterial(matName, Color.white, smoothness, metallic);

            string texPath = $"Assets/ZenBonsai/Materials/{matName}_v26_Tex.png";
            string normPath = $"Assets/ZenBonsai/Materials/{matName}_v26_Norm.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            Texture2D normTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normPath);

            if (tex == null || normTex == null)
            {
                int w = 512, h = 512;
                Texture2D genAlbedo = new Texture2D(w, h, TextureFormat.RGBA32, true);
                Texture2D genNorm = new Texture2D(w, h, TextureFormat.RGBA32, true);
                Color[] pixels = new Color[w * h];
                Color[] normPixels = new Color[w * h];
                float[,] heightMap = new float[w, h];

                for (int y = 0; y < h; y++)
                {
                    float v = y / (float)h;
                    for (int x = 0; x < w; x++)
                    {
                        float u = x / (float)w;
                        float rad = u * Mathf.PI * 2f;
                        float cx = Mathf.Cos(rad) * 1.3f + 2.5f;
                        float cy = Mathf.Sin(rad) * 1.3f + 2.5f;

                        float n1 = Mathf.PerlinNoise(cx * 4.5f + style * 5.1f, v * 6.5f + style * 2.3f);
                        float n2 = Mathf.PerlinNoise(cx * 14f + cy * 7f, v * 18f + style * 9.7f);
                        float n3 = Mathf.PerlinNoise(u * 36f + style, v * 36f + style);

                        Color c = baseColor;
                        float hVal = n1 * 0.5f + n2 * 0.3f + n3 * 0.2f;

                        if (style == 10) // Cerámica esmaltada Tokoname azul cobalto con pátina de horno en el labio
                        {
                            float drip = Mathf.Sin(rad * 6f + n1 * 3.5f) * 0.5f + 0.5f;
                            c = Color.Lerp(darkAccent, baseColor, Mathf.SmoothStep(0.15f, 0.85f, n1 * 0.65f + drip * 0.35f));
                            // Pátina bronce/tierra cocida en el borde superior e inferior de la maceta
                            float rimKiln = Mathf.Clamp01((v - 0.68f) / 0.28f) * (0.55f + 0.45f * n2);
                            c = Color.Lerp(c, lightAccent, rimKiln * 0.72f);
                            // Moteado mineral sutil del esmalte
                            if (n3 > 0.76f) c = Color.Lerp(c, lightAccent * 1.15f, (n3 - 0.76f) * 1.6f);
                            hVal = n2 * 0.35f + n3 * 0.25f;
                        }
                        else if (style == 11) // Musgo aterciopelado de Kioto / Sustrato / Roca mineral
                        {
                            float tuft = Mathf.SmoothStep(0.20f, 0.80f, n1 * 0.50f + n2 * 0.35f + n3 * 0.15f);
                            c = Color.Lerp(darkAccent, baseColor, tuft);
                            c = Color.Lerp(c, lightAccent, Mathf.Pow(n2 * n3, 1.1f) * 0.85f);
                            hVal = tuft * 0.65f + n3 * 0.35f;
                        }
                        else if (style == 12) // Pétalo de Sakura con cáliz carmesí suave y finas nervaduras
                        {
                            float centerDist = Vector2.Distance(new Vector2(u, v), new Vector2(0.5f, 0.5f)) * 1.8f;
                            float heartBlend = Mathf.Clamp01(1f - centerDist);
                            float vein = Mathf.Sin((u - 0.5f) * 48f + n1 * 2f) * 0.5f + 0.5f;
                            c = Color.Lerp(baseColor, lightAccent, Mathf.Clamp01(centerDist * 0.75f));
                            c = Color.Lerp(c, darkAccent, Mathf.Pow(heartBlend, 1.8f) * 0.68f);
                            hVal = vein * 0.35f + n2 * 0.25f;
                        }
                        else if (style == 13) // Hoja de arce / brote con nervaduras palmadas y gradiente otoñal
                        {
                            float veinPattern = Mathf.Abs(Mathf.Sin((u - 0.5f) * 32f + (v - 0.5f) * 24f));
                            c = Color.Lerp(darkAccent, baseColor, n1 * 0.7f + 0.3f);
                            c = Color.Lerp(c, lightAccent, (1f - veinPattern) * 0.45f + n2 * 0.30f);
                            hVal = (1f - veinPattern) * 0.55f + n2 * 0.35f;
                        }
                        else if (style == 14) // Acero japonés forjado y bruñido
                        {
                            float brush = Mathf.PerlinNoise(u * 8f, v * 85f);
                            float damascus = Mathf.Sin(u * 24f + v * 18f + n1 * 6f) * 0.5f + 0.5f;
                            c = Color.Lerp(darkAccent, baseColor, brush * 0.65f + damascus * 0.35f);
                            c = Color.Lerp(c, lightAccent, Mathf.Pow(brush, 2f) * 0.45f);
                            hVal = brush * 0.40f + damascus * 0.25f;
                        }
                        else if (style == 15) // Bambú natural / Fibra vegetal estriada
                        {
                            float fiber = Mathf.Sin(u * Mathf.PI * 64f + n1 * 3f) * 0.5f + 0.5f;
                            c = Color.Lerp(darkAccent, baseColor, fiber * 0.6f + n1 * 0.4f);
                            c = Color.Lerp(c, lightAccent, n2 * 0.35f);
                            hVal = fiber * 0.65f + n2 * 0.35f;
                        }
                        else if (style == 16) // Grava blanca rastrillada Karesansui con surcos concéntricos
                        {
                            float groove = Mathf.Sin(u * Mathf.PI * 28f) * 0.5f + 0.5f;
                            c = Color.Lerp(darkAccent, baseColor, groove * 0.75f + n3 * 0.25f);
                            c = Color.Lerp(c, lightAccent, Mathf.Pow(groove, 2f) * 0.4f);
                            hVal = groove * 0.80f + n3 * 0.20f;
                        }
                        else if (style == 17) // Tablones de madera noble japonesa (Engawa Hinoki / Keyaki) con vetas, nudos y juntas
                        {
                            float plankIdx = Mathf.Floor(u * 8f);
                            float plankFrac = Mathf.Repeat(u * 8f, 1f);
                            float seamX = Mathf.SmoothStep(0.0f, 0.045f, plankFrac) * Mathf.SmoothStep(1.0f, 0.955f, plankFrac);
                            float endJointV = Mathf.Repeat(v * 2f + plankIdx * 0.37f, 1f);
                            float seamY = Mathf.SmoothStep(0.0f, 0.018f, endJointV) * Mathf.SmoothStep(1.0f, 0.982f, endJointV);
                            float seam = seamX * seamY;

                            float grainU = (u * 26f) + plankIdx * 3.7f + Mathf.Sin(v * Mathf.PI * 4f + n1 * 3f) * 0.8f;
                            float ring = Mathf.Sin(grainU * Mathf.PI * 2f + n2 * 4.5f) * 0.5f + 0.5f;
                            float plankTone = Mathf.Sin(plankIdx * 2.3f) * 0.08f;

                            c = Color.Lerp(darkAccent, baseColor, ring * 0.65f + n1 * 0.35f + plankTone);
                            c = Color.Lerp(c, lightAccent, Mathf.Pow(ring, 2.2f) * 0.38f);
                            c = Color.Lerp(darkAccent * 0.42f, c, seam);
                            hVal = seam * (0.65f + ring * 0.25f + n3 * 0.10f);
                        }
                        else if (style == 18) // Estera Tatami tejida (Igusa) con ribete de brocado tradicional (Tatami-beri)
                        {
                            bool isBrocade = (u < 0.075f || u > 0.925f);
                            if (isBrocade)
                            {
                                float diamond = Mathf.Abs(Mathf.Sin(u * Mathf.PI * 80f) * Mathf.Cos(v * Mathf.PI * 48f));
                                Color brocadeBase = new Color(0.11f, 0.16f, 0.24f);
                                Color goldThread = new Color(0.86f, 0.68f, 0.26f);
                                c = Color.Lerp(brocadeBase, goldThread, Mathf.SmoothStep(0.62f, 0.88f, diamond) * 0.85f);
                                hVal = 0.72f + diamond * 0.20f;
                            }
                            else
                            {
                                float rushStraw = Mathf.Sin(v * Mathf.PI * 140f + n2 * 1.5f) * 0.5f + 0.5f;
                                float warpThread = Mathf.Sin(u * Mathf.PI * 36f) * 0.5f + 0.5f;
                                c = Color.Lerp(darkAccent, baseColor, rushStraw * 0.70f + n1 * 0.30f);
                                c = Color.Lerp(c, lightAccent, warpThread * 0.25f);
                                hVal = rushStraw * 0.65f + warpThread * 0.25f;
                            }
                        }
                        else if (style == 19) // Tejas cerámicas japonesas Ibushi-Gawara (canales Honbuki + escalones de teja)
                        {
                            float tileCol = Mathf.Sin(u * Mathf.PI * 24f) * 0.5f + 0.5f;
                            float tileRowFrac = Mathf.Repeat(v * 14f, 1f);
                            float rowOverlap = Mathf.SmoothStep(0.0f, 0.14f, tileRowFrac);
                            float glazeSheen = Mathf.SmoothStep(0.20f, 0.85f, tileCol) * rowOverlap;
                            c = Color.Lerp(darkAccent * 0.75f, baseColor, glazeSheen);
                            c = Color.Lerp(c, lightAccent, Mathf.Pow(tileCol, 3.0f) * rowOverlap * 0.55f);
                            hVal = tileCol * 0.72f * rowOverlap + tileRowFrac * 0.28f;
                        }
                        else if (style == 20) // Panel Shoji con cuadrícula de madera Kumiko y papel Washi translúcido
                        {
                            float gridU = Mathf.Repeat(u * 6f, 1f);
                            float gridV = Mathf.Repeat(v * 10f, 1f);
                            bool isOuterBorder = (u < 0.045f || u > 0.955f || v < 0.035f || v > 0.965f);
                            bool isKumikoBar = (gridU < 0.065f || gridU > 0.935f || gridV < 0.055f || gridV > 0.945f);
                            if (isOuterBorder || isKumikoBar)
                            {
                                float woodGrain = Mathf.Sin((u + v) * 90f + n1 * 4f) * 0.5f + 0.5f;
                                c = Color.Lerp(darkAccent * 0.75f, darkAccent, woodGrain);
                                hVal = 0.92f;
                            }
                            else
                            {
                                float washiFiber = Mathf.PerlinNoise(u * 42f, v * 42f) * 0.5f + Mathf.PerlinNoise(u * 85f, v * 18f) * 0.5f;
                                float warmGlow = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(u, v), new Vector2(0.5f, 0.5f)) * 0.95f);
                                c = Color.Lerp(baseColor, lightAccent, washiFiber * 0.45f + warmGlow * 0.45f);
                                hVal = 0.25f + washiFiber * 0.15f;
                            }
                        }
                        else if (style == 21) // Mampostería de piedra de templo japonés (Ishigaki) con juntas y musgo
                        {
                            float row = Mathf.Floor(v * 7f);
                            float rowV = Mathf.Repeat(v * 7f, 1f);
                            float colU = Mathf.Repeat(u * 5f + (row % 2f) * 0.5f, 1f);
                            float mortarH = Mathf.SmoothStep(0.0f, 0.08f, rowV) * Mathf.SmoothStep(1.0f, 0.92f, rowV);
                            float mortarV = Mathf.SmoothStep(0.0f, 0.06f, colU) * Mathf.SmoothStep(1.0f, 0.94f, colU);
                            float stoneMask = mortarH * mortarV;

                            float blockSeed = Mathf.Sin(row * 7.1f + Mathf.Floor(u * 5f + (row % 2f) * 0.5f) * 13.3f) * 0.08f;
                            c = Color.Lerp(darkAccent, baseColor, n1 * 0.55f + n2 * 0.45f + blockSeed);
                            c = Color.Lerp(c, lightAccent, Mathf.Pow(n3, 2f) * 0.35f);
                            // Musgo natural en las juntas húmedas e inferiores
                            Color mossTint = new Color(0.16f, 0.38f, 0.13f);
                            float mossCrevice = (1f - stoneMask) * Mathf.SmoothStep(0.35f, 0.75f, n1) + Mathf.Clamp01((0.28f - v) * 2.5f) * n2;
                            c = Color.Lerp(darkAccent * 0.45f, c, stoneMask);
                            c = Color.Lerp(c, mossTint, Mathf.Clamp01(mossCrevice * 0.75f));
                            hVal = stoneMask * (0.60f + n2 * 0.28f + n3 * 0.12f);
                        }
                        else if (style == 22) // Muro de estuco blanco japonés (Shikkui) con zócalo de madera de cedro (Koshikabe)
                        {
                            if (v < 0.28f)
                            {
                                float boardFrac = Mathf.Repeat(u * 12f, 1f);
                                float boardSeam = Mathf.SmoothStep(0.0f, 0.06f, boardFrac) * Mathf.SmoothStep(1.0f, 0.94f, boardFrac);
                                float woodRing = Mathf.Sin(u * 95f + n1 * 5f) * 0.5f + 0.5f;
                                Color cedarDark = new Color(0.22f, 0.14f, 0.09f);
                                Color cedarMid = new Color(0.36f, 0.23f, 0.15f);
                                c = Color.Lerp(cedarDark, cedarMid, woodRing * boardSeam);
                                hVal = 0.65f * boardSeam + woodRing * 0.20f;
                            }
                            else
                            {
                                float trowel = n1 * 0.55f + n2 * 0.30f + n3 * 0.15f;
                                c = Color.Lerp(darkAccent, baseColor, trowel);
                                c = Color.Lerp(c, lightAccent, Mathf.Pow(trowel, 2f) * 0.35f);
                                hVal = 0.40f + trowel * 0.25f;
                            }
                        }
                        else if (style == 23) // Laderas volcánicas del Monte Fuji con barrancos de basalto y escarcha alpina
                        {
                            float gully = Mathf.Sin(rad * 18f + n1 * 4f) * 0.5f + 0.5f;
                            float scree = n2 * 0.6f + n3 * 0.4f;
                            c = Color.Lerp(darkAccent, baseColor, gully * 0.65f + scree * 0.35f);
                            float highFrost = Mathf.Clamp01((v - 0.48f) / 0.42f) * Mathf.SmoothStep(0.42f, 0.78f, gully * 0.7f + n2 * 0.3f);
                            c = Color.Lerp(c, lightAccent, highFrost * 0.85f);
                            hVal = gully * 0.70f + scree * 0.30f;
                        }
                        else if (style == 24) // Agua de estanque y lago espejo con ondas entrelazadas y reflejos
                        {
                            float w1 = Mathf.Sin(u * Mathf.PI * 24f + v * Mathf.PI * 16f + n1 * 4f) * 0.5f + 0.5f;
                            float w2 = Mathf.Cos(u * Mathf.PI * 18f - v * Mathf.PI * 28f + n2 * 5f) * 0.5f + 0.5f;
                            float wave = (w1 + w2) * 0.5f;
                            c = Color.Lerp(darkAccent, baseColor, wave * 0.75f + n1 * 0.25f);
                            c = Color.Lerp(c, lightAccent, Mathf.Pow(wave, 3.2f) * 0.48f);
                            hVal = wave * 0.80f + n3 * 0.20f;
                        }
                        else if (style == 25) // Pergamino Kakemono pintado en tinta Sumi-e (Monte Fuji, Sol Rojo y Pino)
                        {
                            bool border = (u < 0.10f || u > 0.90f || v < 0.08f || v > 0.92f);
                            if (border)
                            {
                                float silkPat = Mathf.Abs(Mathf.Sin(u * Mathf.PI * 40f) * Mathf.Cos(v * Mathf.PI * 40f));
                                c = Color.Lerp(darkAccent, new Color(0.76f, 0.62f, 0.28f), silkPat * 0.45f);
                                hVal = 0.70f + silkPat * 0.20f;
                            }
                            else
                            {
                                c = Color.Lerp(baseColor, lightAccent, n1 * 0.35f + 0.5f);
                                // Sol carmesí japonés (Hinomaru) en la parte superior derecha del pergamino
                                float sunDist = Vector2.Distance(new Vector2(u, v), new Vector2(0.64f, 0.72f));
                                if (sunDist < 0.095f)
                                {
                                    c = Color.Lerp(new Color(0.82f, 0.16f, 0.12f), c, Mathf.SmoothStep(0.075f, 0.095f, sunDist));
                                }
                                // Silueta Sumi-e del Monte Fuji en el centro del pergamino
                                float fujiProfile = 0.34f + Mathf.Pow(Mathf.Clamp01(1f - Mathf.Abs(u - 0.50f) * 2.6f), 1.6f) * 0.30f;
                                if (Mathf.Abs(u - 0.50f) < 0.055f) fujiProfile = Mathf.Min(fujiProfile, 0.61f);
                                if (v < fujiProfile && v > 0.22f)
                                {
                                    float snowZone = Mathf.Clamp01((v - (fujiProfile - 0.09f)) / 0.09f);
                                    Color inkColor = Color.Lerp(new Color(0.14f, 0.16f, 0.20f), baseColor, snowZone * 0.85f + (0.34f - v) * 1.5f);
                                    c = Color.Lerp(c, inkColor, 0.85f);
                                }
                                hVal = 0.35f + n3 * 0.15f;
                            }
                        }

                        c.a = 1f;
                        pixels[y * w + x] = c;
                        heightMap[x, y] = hVal;
                    }
                }

                float bumpStrength = (style == 11 || style == 16 || style >= 17) ? 3.2f : 1.8f;
                for (int y = 0; y < h; y++)
                {
                    int yUp = (y + 1) % h;
                    int yDn = (y - 1 + h) % h;
                    for (int x = 0; x < w; x++)
                    {
                        int xR = (x + 1) % w;
                        int xL = (x - 1 + w) % w;
                        float dX = (heightMap[xR, y] - heightMap[xL, y]) * bumpStrength;
                        float dY = (heightMap[x, yUp] - heightMap[x, yDn]) * bumpStrength;
                        Vector3 n = new Vector3(-dX, -dY, 1.0f).normalized;
                        normPixels[y * w + x] = new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f, n.z * 0.5f + 0.5f, 1f);
                    }
                }

                genAlbedo.SetPixels(pixels);
                genAlbedo.Apply();
                System.IO.File.WriteAllBytes(texPath, genAlbedo.EncodeToPNG());
                Object.DestroyImmediate(genAlbedo);

                genNorm.SetPixels(normPixels);
                genNorm.Apply();
                System.IO.File.WriteAllBytes(normPath, genNorm.EncodeToPNG());
                Object.DestroyImmediate(genNorm);

                AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.ImportAsset(normPath, ImportAssetOptions.ForceSynchronousImport);

                tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                normTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normPath);
            }

            if (tex != null)
            {
                mat.mainTexture = tex;
                if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            }
            if (normTex != null)
            {
                mat.EnableKeyword("_NORMALMAP");
                if (mat.HasProperty("_BumpMap")) mat.SetTexture("_BumpMap", normTex);
                if (mat.HasProperty("_BumpScale")) mat.SetFloat("_BumpScale", 1.25f);
            }
            if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 0f);
            if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 1f);
            if (mat.HasProperty("_Cull")) mat.SetFloat("_Cull", 0f);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            EditorUtility.SetDirty(mat);
            return mat;
        }

        /// <summary>
        /// Genera una textura equirectangular panorámica 360° (1024x512) pintada con gradiente atmosférico Rayleigh/Mie,
        /// halo solar cálido, nubes cúmulos/cirros iluminadas por el sol y 3 cordilleras lejanas con niebla en 360°.
        /// </summary>
        private static Material CreateOrGetPanoramicSkyDomeMaterial(
            string matName, Color zenithColor, Color midSkyColor, Color horizonGlowColor,
            Color cloudLightColor, Color cloudShadowColor, Color distantMtnColor, int skySeed)
        {
            Material mat = CreateOrGetPBRMaterial(matName, Color.white, 0.0f, 0.0f);
            string texPath = $"Assets/ZenBonsai/Materials/{matName}_v26_Sky.png";
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

            if (tex == null)
            {
                int w = 1024, h = 512;
                Texture2D skyTex = new Texture2D(w, h, TextureFormat.RGBA32, true);
                skyTex.wrapModeU = TextureWrapMode.Repeat;
                skyTex.wrapModeV = TextureWrapMode.Clamp;
                Color[] pixels = new Color[w * h];

                for (int y = 0; y < h; y++)
                {
                    float v = y / (float)(h - 1);
                    for (int x = 0; x < w; x++)
                    {
                        float u = x / (float)w;
                        float rad = u * Mathf.PI * 2f;
                        float cx = Mathf.Cos(rad) * 2.2f + 5.0f + skySeed * 3.1f;
                        float cy = Mathf.Sin(rad) * 2.2f + 5.0f + skySeed * 1.7f;

                        // 1. Gradiente atmosférico de cúpula celeste
                        Color skyCol;
                        if (v >= 0.62f)
                        {
                            float t = Mathf.SmoothStep(0.62f, 0.98f, v);
                            skyCol = Color.Lerp(midSkyColor, zenithColor, t);
                        }
                        else if (v >= 0.44f)
                        {
                            float t = Mathf.SmoothStep(0.44f, 0.62f, v);
                            skyCol = Color.Lerp(horizonGlowColor, midSkyColor, t);
                        }
                        else
                        {
                            float t = Mathf.SmoothStep(0.18f, 0.44f, v);
                            skyCol = Color.Lerp(distantMtnColor * 0.72f, horizonGlowColor, t);
                        }

                        // 2. Halo solar dorado suave en el horizonte frontal-derecho
                        float dU = Mathf.Abs(Mathf.DeltaAngle(u * 360f, 195f)) / 180f;
                        float dV = Mathf.Abs(v - 0.53f) * 2.2f;
                        float sunDist = Mathf.Sqrt(dU * dU + dV * dV);
                        float sunGlow = Mathf.Pow(Mathf.Clamp01(1f - sunDist * 1.85f), 2.4f);
                        skyCol = Color.Lerp(skyCol, new Color(1.0f, 0.90f, 0.68f), sunGlow * 0.65f);

                        // 3. Nubes volumétricas pintadas en 360° (cúmulos y cirros iluminados al atardecer/día)
                        if (v > 0.45f && v < 0.86f)
                        {
                            float c1 = Mathf.PerlinNoise(cx * 2.4f, v * 7.5f + skySeed);
                            float c2 = Mathf.PerlinNoise(cx * 5.8f + cy * 2.5f, v * 16f + skySeed * 2.3f);
                            float c3 = Mathf.PerlinNoise(cx * 12f - cy * 4f, v * 32f);
                            float cloudDensity = c1 * 0.55f + c2 * 0.30f + c3 * 0.15f;
                            float bandMask = Mathf.Sin(Mathf.Clamp01((v - 0.45f) / 0.41f) * Mathf.PI);
                            float cloudAlpha = Mathf.SmoothStep(0.48f, 0.76f, cloudDensity) * bandMask;

                            if (cloudAlpha > 0.01f)
                            {
                                float cTop = Mathf.PerlinNoise(cx * 5.8f + cy * 2.5f, (v + 0.015f) * 16f + skySeed * 2.3f);
                                float rimLight = Mathf.Clamp01((cloudDensity - cTop) * 4.5f + 0.55f + sunGlow * 0.45f);
                                Color cloudCol = Color.Lerp(cloudShadowColor, cloudLightColor, rimLight);
                                skyCol = Color.Lerp(skyCol, cloudCol, cloudAlpha * 0.88f);
                            }
                        }

                        // 4. Tres cordilleras lejanas en 360° con perspectiva atmosférica y niebla en los valles
                        float m1 = 0.455f + Mathf.PerlinNoise(cx * 1.6f, cy * 1.6f) * 0.075f + Mathf.Sin(rad * 5f + skySeed) * 0.022f;
                        float m2 = 0.430f + Mathf.PerlinNoise(cx * 2.8f + 4f, cy * 2.8f + 4f) * 0.062f + Mathf.Cos(rad * 8f) * 0.018f;
                        float m3 = 0.405f + Mathf.PerlinNoise(cx * 4.5f + 9f, cy * 4.5f + 9f) * 0.048f;

                        if (v < m1)
                        {
                            float mist1 = Mathf.Clamp01((m1 - v) / 0.065f);
                            Color farRidge = Color.Lerp(horizonGlowColor * 0.92f, distantMtnColor, mist1 * 0.62f);
                            skyCol = Color.Lerp(skyCol, farRidge, Mathf.SmoothStep(m1, m1 - 0.008f, v));
                        }
                        if (v < m2)
                        {
                            float mist2 = Mathf.Clamp01((m2 - v) / 0.055f);
                            Color midRidge = Color.Lerp(horizonGlowColor * 0.82f, distantMtnColor * 0.78f, mist2 * 0.78f);
                            skyCol = Color.Lerp(skyCol, midRidge, Mathf.SmoothStep(m2, m2 - 0.006f, v));
                        }
                        if (v < m3)
                        {
                            Color nearRidge = distantMtnColor * 0.58f;
                            skyCol = Color.Lerp(skyCol, nearRidge, Mathf.SmoothStep(m3, m3 - 0.005f, v));
                        }

                        skyCol.a = 1f;
                        pixels[y * w + x] = skyCol;
                    }
                }

                skyTex.SetPixels(pixels);
                skyTex.Apply();
                System.IO.File.WriteAllBytes(texPath, skyTex.EncodeToPNG());
                Object.DestroyImmediate(skyTex);

                AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceSynchronousImport);
                tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            }

            if (tex != null)
            {
                tex.wrapModeU = TextureWrapMode.Repeat;
                tex.wrapModeV = TextureWrapMode.Clamp;
                mat.mainTexture = tex;
                if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
                mat.EnableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionMap")) mat.SetTexture("_EmissionMap", tex);
                if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", new Color(0.68f, 0.68f, 0.68f));
            }
            if (mat.HasProperty("_Cull")) mat.SetFloat("_Cull", 0f);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        /// <summary>
        /// Genera un material PBR de diseñador con textura Albedo 1024x1024 + Mapa de Normales 1024x1024 (_BumpMap)
        /// para que cada tronco único tenga relieve 3D hiperrealista de corteza natural, musgo en la base y nudos.
        /// </summary>
        private static Material CreateOrGetBarkMaterial(string matName, Color baseColor, Color furrowColor, Color highlightColor, float smoothness, int barkStyle)
        {
            Material mat = CreateOrGetPBRMaterial(matName, Color.white, smoothness, 0.0f);

            string texPath = $"Assets/ZenBonsai/Materials/{matName}_v22_Tex.png";
            string normPath = $"Assets/ZenBonsai/Materials/{matName}_v22_Norm.png";

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            Texture2D normTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normPath);

            if (tex == null || normTex == null)
            {
                int w = 1024, h = 1024;
                Texture2D genAlbedo = new Texture2D(w, h, TextureFormat.RGBA32, true);
                Texture2D genNorm = new Texture2D(w, h, TextureFormat.RGBA32, true);
                Color[] pixels = new Color[w * h];
                Color[] normPixels = new Color[w * h];
                float[,] heightMap = new float[w, h];

                Color mossBaseTint = new Color(0.18f, 0.42f, 0.14f);

                for (int y = 0; y < h; y++)
                {
                    float v = y / (float)h;
                    for (int x = 0; x < w; x++)
                    {
                        float u = x / (float)w;
                        // Hacer que la coordenada U sea periódica sin costura (seamless en 360 grados alrededor del tronco)
                        float rad = u * Mathf.PI * 2f;
                        float cx = Mathf.Cos(rad) * 1.4f + 3.5f;
                        float cy = Mathf.Sin(rad) * 1.4f + 3.5f;

                        float n1 = Mathf.PerlinNoise(cx * 3.8f + barkStyle * 7.3f, v * 8f + barkStyle * 3.1f);
                        float n2 = Mathf.PerlinNoise(cx * 9.5f + n1 * 1.8f, v * 22f + barkStyle * 11.7f);
                        float n3 = Mathf.PerlinNoise(cx * 22f + cy * 11f, v * 52f);

                        float ridge = Mathf.SmoothStep(0.14f, 0.82f, n1 * 0.60f + n2 * 0.40f);
                        Color c = Color.Lerp(furrowColor, baseColor, ridge);
                        c = Color.Lerp(c, highlightColor, Mathf.Pow(n2 * n3, 1.25f) * 0.85f);

                        float hVal = ridge * 0.70f + n3 * 0.30f;

                        // Estilo 1: Sakura (Castaño caoba satinado con lenticelas horizontales de cerezo)
                        if (barkStyle == 1)
                        {
                            float lenticel = Mathf.Sin(v * Mathf.PI * 68f + n1 * 5f);
                            if (lenticel > 0.91f && n2 > 0.42f)
                            {
                                c = Color.Lerp(c, furrowColor * 0.75f, 0.72f);
                                hVal -= 0.25f;
                            }
                        }
                        // Estilo 2: Matsu (Pino Negro Kuromatsu con placas escamosas profundas y fisuras oscuras)
                        else if (barkStyle == 2)
                        {
                            float plate = Mathf.Abs(Mathf.Sin(rad * 7f + n1 * 2.5f) * Mathf.Cos(v * Mathf.PI * 14f + n2 * 2.2f));
                            float plateStep = Mathf.SmoothStep(0.10f, 0.42f, plate);
                            c = Color.Lerp(furrowColor * 0.75f, c, plateStep);
                            hVal = plateStep * 0.85f + n3 * 0.15f;
                        }
                        // Estilo 4: Momiji (Arce japonés con estrías finas longitudinales)
                        else if (barkStyle == 4)
                        {
                            float stria = Mathf.Sin(rad * 24f + n1 * 4f) * 0.5f + 0.5f;
                            c = Color.Lerp(c, highlightColor, stria * 0.32f);
                            hVal = ridge * 0.55f + stria * 0.45f;
                        }

                        // Patina de musgo aterciopelado natural en las raíces bajas del tronco (v < 0.16)
                        if (v < 0.16f && barkStyle != 3)
                        {
                            float mossFactor = Mathf.Clamp01((0.16f - v) / 0.16f) * Mathf.SmoothStep(0.35f, 0.75f, n1);
                            c = Color.Lerp(c, mossBaseTint, mossFactor * 0.65f);
                        }

                        c.a = 1f;
                        pixels[y * w + x] = c;
                        heightMap[x, y] = hVal;
                    }
                }

                // Calcular mapa de normales 3D a partir del mapa de alturas
                float bumpStrength = 3.8f;
                for (int y = 0; y < h; y++)
                {
                    int yUp = (y + 1) % h;
                    int yDn = (y - 1 + h) % h;
                    for (int x = 0; x < w; x++)
                    {
                        int xR = (x + 1) % w;
                        int xL = (x - 1 + w) % w;

                        float dX = (heightMap[xR, y] - heightMap[xL, y]) * bumpStrength;
                        float dY = (heightMap[x, yUp] - heightMap[x, yDn]) * bumpStrength;

                        Vector3 n = new Vector3(-dX, -dY, 1.0f).normalized;
                        normPixels[y * w + x] = new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f, n.z * 0.5f + 0.5f, 1f);
                    }
                }

                genAlbedo.SetPixels(pixels);
                genAlbedo.Apply();
                System.IO.File.WriteAllBytes(texPath, genAlbedo.EncodeToPNG());
                Object.DestroyImmediate(genAlbedo);

                genNorm.SetPixels(normPixels);
                genNorm.Apply();
                System.IO.File.WriteAllBytes(normPath, genNorm.EncodeToPNG());
                Object.DestroyImmediate(genNorm);

                AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.ImportAsset(normPath, ImportAssetOptions.ForceSynchronousImport);

                tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                normTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normPath);
            }

            if (tex != null)
            {
                mat.mainTexture = tex;
                if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            }
            if (normTex != null)
            {
                mat.EnableKeyword("_NORMALMAP");
                if (mat.HasProperty("_BumpMap")) mat.SetTexture("_BumpMap", normTex);
                if (mat.HasProperty("_BumpScale")) mat.SetFloat("_BumpScale", 1.50f);
            }
            // Garantizar opacidad absoluta 100%, escritura en ZBuffer y renderizado sólido sin caras huecas
            if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 0f);
            if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 1f);
            if (mat.HasProperty("_Cull")) mat.SetFloat("_Cull", 0f);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static ZenTouchButton Create3DTouchButton(Transform parent, string name, string label, Vector3 localPos, Vector3 size, Material bgMat, Color textColor, float fontSize = 14)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            btnObj.transform.localPosition = localPos;

            // Collider 100% centrado con el botón visual (sin desfase vertical)
            BoxCollider box = btnObj.AddComponent<BoxCollider>();
            box.size = new Vector3(size.x * 0.95f, size.y * 0.95f, 0.034f);
            box.center = Vector3.zero;
            box.isTrigger = true;

            Rigidbody rb = btnObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            GameObject plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = "Plate";
            plate.transform.SetParent(btnObj.transform, false);
            plate.transform.localPosition = Vector3.zero;
            plate.transform.localScale = size;
            Material btnInstanceMat = CreateOrGetPBRMaterial($"Zen_Btn_{name}", bgMat.color, 0.72f, 0.05f);
            plate.GetComponent<Renderer>().sharedMaterial = btnInstanceMat;
            var plateCol = plate.GetComponent<Collider>();
            if (plateCol != null) DestroyImmediate(plateCol);

            // =========================================================================
            // MARCO DE NEÓN 3D ALREDEDOR DEL NOMBRE DEL BOTÓN (SE ILUMINA CUANDO EL ÁRBOL PIDE CUIDADO)
            // =========================================================================
            Material neonBaseMat = CreateOrGetPBRMaterial($"Zen_NeonBorder_{name}", new Color(0.15f, 0.90f, 1.0f), 0.92f, 0.0f);
            float tubeThick = 0.0055f;
            float zFront = -(size.z * 0.5f + 0.0015f);
            float hx = size.x * 0.5f + tubeThick * 0.45f;
            float hy = size.y * 0.5f + tubeThick * 0.45f;

            Vector3[] barPositions =
            {
                new Vector3(0f, hy, zFront),   // Superior
                new Vector3(0f, -hy, zFront),  // Inferior
                new Vector3(-hx, 0f, zFront),  // Izquierda
                new Vector3(hx, 0f, zFront)    // Derecha
            };
            Vector3[] barScales =
            {
                new Vector3(size.x + tubeThick * 1.8f, tubeThick, tubeThick),
                new Vector3(size.x + tubeThick * 1.8f, tubeThick, tubeThick),
                new Vector3(tubeThick, size.y + tubeThick * 1.8f, tubeThick),
                new Vector3(tubeThick, size.y + tubeThick * 1.8f, tubeThick)
            };

            Renderer[] neonRenderers = new Renderer[4];
            for (int n = 0; n < 4; n++)
            {
                GameObject neonBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                neonBar.name = $"NeonBorder_{n}";
                neonBar.transform.SetParent(btnObj.transform, false);
                neonBar.transform.localPosition = barPositions[n];
                neonBar.transform.localScale = barScales[n];
                Renderer nr = neonBar.GetComponent<Renderer>();
                nr.sharedMaterial = neonBaseMat;
                var nCol = neonBar.GetComponent<Collider>();
                if (nCol != null) DestroyImmediate(nCol);
                neonBar.SetActive(false); // Se enciende dinámicamente cuando el árbol necesita agua, poda o limpieza
                neonRenderers[n] = nr;
            }

            // Canvas en alta resolución (dynamicPixelsPerUnit = 5f, escala 0.00035f para eliminar toda borrosidad en Quest 3)
            GameObject textCanvasObj = new GameObject("LabelCanvas");
            textCanvasObj.transform.SetParent(btnObj.transform, false);
            textCanvasObj.transform.localPosition = new Vector3(0f, 0f, -(size.z * 0.5f + 0.002f));
            textCanvasObj.transform.localScale = new Vector3(0.00035f, 0.00035f, 0.00035f);

            Canvas canvas = textCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler scaler = textCanvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 5f;

            GameObject tChild = new GameObject("Text");
            tChild.transform.SetParent(textCanvasObj.transform, false);
            Text txt = tChild.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (txt.font == null) txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontStyle = FontStyle.Bold;
            txt.fontSize = Mathf.RoundToInt(fontSize * 2.85f);
            txt.lineSpacing = 1.15f;
            txt.text = label;
            txt.color = textColor;
            RectTransform rect = tChild.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size.x / 0.00035f, size.y / 0.00035f);

            ZenTouchButton touchBtn = btnObj.AddComponent<ZenTouchButton>();
            touchBtn.visualPlate = plate.transform;
            touchBtn.neonBorderRenderers = neonRenderers;
            touchBtn.buttonLabelText = txt;
            touchBtn.touchHalfSize = new Vector2(size.x * 0.52f, size.y * 0.52f);
            touchBtn.isCircular = false;
            touchBtn.pressDisplacement = new Vector3(0f, 0f, 0.010f);
            return touchBtn;
        }

        private static ZenTouchButton Create3DRoundTouchButton(
            Transform parent,
            string name,
            string symbolIcon,
            string kanjiBadge,
            string treeName,
            string subtitle,
            Vector3 localPos,
            float diameter,
            Material baseMat,
            Color accentColor,
            Material goldMat,
            Material urushiMat)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            btnObj.transform.localPosition = localPos;

            BoxCollider box = btnObj.AddComponent<BoxCollider>();
            box.size = new Vector3(diameter * 0.96f, diameter * 0.96f, 0.034f);
            box.center = Vector3.zero;
            box.isTrigger = true;

            Rigidbody rb = btnObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            // Marco circular exterior dorado (Fukurin / Medallón de laca y oro estilo Kamon)
            // Nota: Cylinder en Unity tiene altura 2 (-1 a +1), así que scale.y = 0.0035f ocupa de z = -0.0025f a +0.0045f
            GameObject outerRim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            outerRim.name = "MedallionRim";
            outerRim.transform.SetParent(btnObj.transform, false);
            outerRim.transform.localPosition = new Vector3(0f, 0f, 0.001f);
            outerRim.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            outerRim.transform.localScale = new Vector3(diameter * 1.08f, 0.0035f, diameter * 1.08f);
            Material rimInstanceMat = CreateOrGetPBRMaterial($"Zen_Rim_{name}", goldMat.color, 0.88f, 0.70f);
            outerRim.GetComponent<Renderer>().sharedMaterial = rimInstanceMat;
            var rimCol = outerRim.GetComponent<Collider>(); if (rimCol != null) DestroyImmediate(rimCol);

            // Plato interactivo circular (scale.y = 0.0045f -> cara frontal en z = -0.0045f)
            GameObject plate = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plate.name = "Plate";
            plate.transform.SetParent(btnObj.transform, false);
            plate.transform.localPosition = Vector3.zero;
            plate.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            plate.transform.localScale = new Vector3(diameter, 0.0045f, diameter);
            Material btnInstanceMat = CreateOrGetPBRMaterial($"Zen_Btn_{name}", baseMat.color, 0.72f, 0.06f);
            plate.GetComponent<Renderer>().sharedMaterial = btnInstanceMat;
            var plateCol = plate.GetComponent<Collider>(); if (plateCol != null) DestroyImmediate(plateCol);

            // Halo circular de neón (8 segmentos en arco que se iluminan cuando el árbol pide cuidado)
            Renderer[] neonRenderers = new Renderer[8];
            Material neonBaseMat = CreateOrGetPBRMaterial($"Zen_NeonRing_{name}", new Color(0.15f, 0.90f, 1.0f), 0.92f, 0.0f);
            float segRadius = diameter * 0.54f;
            float arcLength = (2f * Mathf.PI * segRadius / 8f) * 0.76f;
            for (int i = 0; i < 8; i++)
            {
                float angle = i * (360f / 8f) * Mathf.Deg2Rad;
                Vector3 segPos = new Vector3(Mathf.Cos(angle) * segRadius, Mathf.Sin(angle) * segRadius, -0.007f);
                GameObject neonSeg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                neonSeg.name = $"NeonSeg_{i}";
                neonSeg.transform.SetParent(btnObj.transform, false);
                neonSeg.transform.localPosition = segPos;
                neonSeg.transform.localRotation = Quaternion.Euler(0f, 0f, i * (360f / 8f) + 90f);
                neonSeg.transform.localScale = new Vector3(arcLength, 0.004f, 0.004f);
                Renderer nr = neonSeg.GetComponent<Renderer>();
                nr.sharedMaterial = neonBaseMat;
                var nCol = neonSeg.GetComponent<Collider>(); if (nCol != null) DestroyImmediate(nCol);
                neonSeg.SetActive(false);
                neonRenderers[i] = nr;
            }

            // Canvas en alta resolución situado claramente DELANTE de la cara frontal del cilindro (z = -0.011f frente a -0.0045f)
            // para que el icono, Kanji, nombre del bonsái y subtítulo sean 100% visibles en todo momento
            GameObject canvasObj = new GameObject("MedallionCanvas");
            canvasObj.transform.SetParent(btnObj.transform, false);
            canvasObj.transform.localPosition = new Vector3(0f, 0f, -0.011f);
            canvasObj.transform.localScale = Vector3.one * 0.00035f;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 5f;

            Font uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (uiFont == null) uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            // Símbolo e icono superior (🌸 桜 / 🌲 松 / 🍁 楓)
            GameObject iconObj = new GameObject("IconText");
            iconObj.transform.SetParent(canvasObj.transform, false);
            iconObj.transform.localPosition = new Vector3(0f, 34f, 0f);
            Text iconTxt = iconObj.AddComponent<Text>();
            iconTxt.font = uiFont;
            iconTxt.alignment = TextAnchor.MiddleCenter;
            iconTxt.fontStyle = FontStyle.Bold;
            iconTxt.fontSize = 27;
            iconTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
            iconTxt.verticalOverflow = VerticalWrapMode.Overflow;
            iconTxt.text = $"{symbolIcon} {kanjiBadge}";
            iconTxt.color = accentColor;
            iconObj.GetComponent<RectTransform>().sizeDelta = new Vector2(210, 38);

            // Nombre principal del árbol en letras latinas ("SAKURA", "MATSU", "MOMIJI")
            GameObject nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(canvasObj.transform, false);
            nameObj.transform.localPosition = new Vector3(0f, -2f, 0f);
            Text nameTxt = nameObj.AddComponent<Text>();
            nameTxt.font = uiFont;
            nameTxt.alignment = TextAnchor.MiddleCenter;
            nameTxt.fontStyle = FontStyle.Bold;
            nameTxt.fontSize = 22;
            nameTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
            nameTxt.verticalOverflow = VerticalWrapMode.Overflow;
            nameTxt.text = treeName.ToUpperInvariant();
            nameTxt.color = Color.white;
            nameObj.GetComponent<RectTransform>().sizeDelta = new Vector2(210, 32);

            // Subtítulo botánico ("Cerezo Flor", "Pino Negro", "Arce Rojo")
            GameObject subObj = new GameObject("SubText");
            subObj.transform.SetParent(canvasObj.transform, false);
            subObj.transform.localPosition = new Vector3(0f, -30f, 0f);
            Text subTxt = subObj.AddComponent<Text>();
            subTxt.font = uiFont;
            subTxt.alignment = TextAnchor.MiddleCenter;
            subTxt.fontStyle = FontStyle.Normal;
            subTxt.fontSize = 13;
            subTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
            subTxt.verticalOverflow = VerticalWrapMode.Overflow;
            subTxt.text = subtitle;
            subTxt.color = new Color(0.95f, 0.95f, 0.95f, 0.92f);
            subObj.GetComponent<RectTransform>().sizeDelta = new Vector2(210, 24);

            ZenTouchButton touchBtn = btnObj.AddComponent<ZenTouchButton>();
            touchBtn.visualPlate = plate.transform;
            touchBtn.neonBorderRenderers = neonRenderers;
            touchBtn.buttonLabelText = nameTxt;
            touchBtn.buttonSubLabelText = subTxt;
            touchBtn.touchHalfSize = new Vector2(diameter * 0.52f, diameter * 0.52f);
            touchBtn.isCircular = true;
            touchBtn.pressDisplacement = new Vector3(0f, 0f, 0.008f);
            return touchBtn;
        }

        private static ZenTouchButton Create3DThematicRectButton(
            Transform parent,
            string name,
            string kanjiBadge,
            string title,
            string subtitle,
            Vector3 localPos,
            Vector3 size,
            Material bgMat,
            Color textColor,
            Material goldMat,
            Material urushiMat)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            btnObj.transform.localPosition = localPos;

            BoxCollider box = btnObj.AddComponent<BoxCollider>();
            box.size = new Vector3(size.x * 0.95f, size.y * 0.95f, 0.034f);
            box.center = Vector3.zero;
            box.isTrigger = true;

            Rigidbody rb = btnObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            // Marco exterior Urushi negro lacado
            GameObject outerFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outerFrame.name = "ThematicOuterFrame";
            outerFrame.transform.SetParent(btnObj.transform, false);
            outerFrame.transform.localPosition = new Vector3(0f, 0f, 0.002f);
            outerFrame.transform.localScale = new Vector3(size.x * 1.05f, size.y * 1.05f, size.z * 0.9f);
            outerFrame.GetComponent<Renderer>().sharedMaterial = urushiMat;
            var ofCol = outerFrame.GetComponent<Collider>(); if (ofCol != null) DestroyImmediate(ofCol);

            // Remate decorativo dorado estilo Ema / Torii en el borde superior
            GameObject topBracket = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topBracket.name = "ThematicGoldBracket";
            topBracket.transform.SetParent(btnObj.transform, false);
            topBracket.transform.localPosition = new Vector3(0f, size.y * 0.525f, -0.004f);
            topBracket.transform.localScale = new Vector3(size.x * 1.06f, 0.0035f, 0.006f);
            Material bracketMat = CreateOrGetPBRMaterial($"Zen_Rim_{name}", goldMat.color, 0.88f, 0.70f);
            topBracket.GetComponent<Renderer>().sharedMaterial = bracketMat;
            var tbCol = topBracket.GetComponent<Collider>(); if (tbCol != null) DestroyImmediate(tbCol);

            // Plato interactivo interior que se hunde al presionar
            GameObject plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = "Plate";
            plate.transform.SetParent(btnObj.transform, false);
            plate.transform.localPosition = Vector3.zero;
            plate.transform.localScale = size;
            Material btnInstanceMat = CreateOrGetPBRMaterial($"Zen_Btn_{name}", bgMat.color, 0.72f, 0.05f);
            plate.GetComponent<Renderer>().sharedMaterial = btnInstanceMat;
            var plateCol = plate.GetComponent<Collider>(); if (plateCol != null) DestroyImmediate(plateCol);

            // Marco de neón
            Material neonBaseMat = CreateOrGetPBRMaterial($"Zen_NeonBorder_{name}", new Color(0.15f, 0.90f, 1.0f), 0.92f, 0.0f);
            float tubeThick = 0.0050f;
            float zFront = -(size.z * 0.5f + 0.0015f);
            float hx = size.x * 0.5f + tubeThick * 0.45f;
            float hy = size.y * 0.5f + tubeThick * 0.45f;

            Vector3[] barPositions =
            {
                new Vector3(0f, hy, zFront),
                new Vector3(0f, -hy, zFront),
                new Vector3(-hx, 0f, zFront),
                new Vector3(hx, 0f, zFront)
            };
            Vector3[] barScales =
            {
                new Vector3(size.x + tubeThick * 1.8f, tubeThick, tubeThick),
                new Vector3(size.x + tubeThick * 1.8f, tubeThick, tubeThick),
                new Vector3(tubeThick, size.y + tubeThick * 1.8f, tubeThick),
                new Vector3(tubeThick, size.y + tubeThick * 1.8f, tubeThick)
            };

            Renderer[] neonRenderers = new Renderer[4];
            for (int n = 0; n < 4; n++)
            {
                GameObject neonBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                neonBar.name = $"NeonBorder_{n}";
                neonBar.transform.SetParent(btnObj.transform, false);
                neonBar.transform.localPosition = barPositions[n];
                neonBar.transform.localScale = barScales[n];
                Renderer nr = neonBar.GetComponent<Renderer>();
                nr.sharedMaterial = neonBaseMat;
                var nCol = neonBar.GetComponent<Collider>(); if (nCol != null) DestroyImmediate(nCol);
                neonBar.SetActive(false);
                neonRenderers[n] = nr;
            }

            // Canvas en alta resolución
            GameObject textCanvasObj = new GameObject("ThematicCanvas");
            textCanvasObj.transform.SetParent(btnObj.transform, false);
            textCanvasObj.transform.localPosition = new Vector3(0f, 0f, -(size.z * 0.5f + 0.003f));
            textCanvasObj.transform.localScale = new Vector3(0.00035f, 0.00035f, 0.00035f);

            Canvas canvas = textCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler scaler = textCanvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 5f;

            Font uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (uiFont == null) uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            float cw = size.x / 0.00035f;

            // Sello Kanji superior (e.g. "【 伝承 】" o "【 神境 】")
            GameObject kanjiObj = new GameObject("KanjiBadge");
            kanjiObj.transform.SetParent(textCanvasObj.transform, false);
            kanjiObj.transform.localPosition = new Vector3(0f, 34f, 0f);
            Text kanjiTxt = kanjiObj.AddComponent<Text>();
            kanjiTxt.font = uiFont;
            kanjiTxt.alignment = TextAnchor.MiddleCenter;
            kanjiTxt.fontStyle = FontStyle.Bold;
            kanjiTxt.fontSize = 15;
            kanjiTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
            kanjiTxt.verticalOverflow = VerticalWrapMode.Overflow;
            kanjiTxt.text = $"【 {kanjiBadge} 】";
            kanjiTxt.color = new Color(0.98f, 0.85f, 0.40f);
            kanjiObj.GetComponent<RectTransform>().sizeDelta = new Vector2(cw, 22);

            // Título principal ("📖 Cuidados" o "⚙️ Opciones")
            GameObject titleObj = new GameObject("MainTitle");
            titleObj.transform.SetParent(textCanvasObj.transform, false);
            titleObj.transform.localPosition = new Vector3(0f, 4f, 0f);
            Text titleTxt = titleObj.AddComponent<Text>();
            titleTxt.font = uiFont;
            titleTxt.alignment = TextAnchor.MiddleCenter;
            titleTxt.fontStyle = FontStyle.Bold;
            titleTxt.fontSize = 22;
            titleTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
            titleTxt.verticalOverflow = VerticalWrapMode.Overflow;
            titleTxt.text = title;
            titleTxt.color = textColor;
            titleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(cw, 32);

            // Subtítulo ("Ficha Botánica" o "Ajustes Zen")
            GameObject subObj = new GameObject("SubTitle");
            subObj.transform.SetParent(textCanvasObj.transform, false);
            subObj.transform.localPosition = new Vector3(0f, -26f, 0f);
            Text subTxt = subObj.AddComponent<Text>();
            subTxt.font = uiFont;
            subTxt.alignment = TextAnchor.MiddleCenter;
            subTxt.fontStyle = FontStyle.Normal;
            subTxt.fontSize = 13;
            subTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
            subTxt.verticalOverflow = VerticalWrapMode.Overflow;
            subTxt.text = subtitle;
            subTxt.color = new Color(0.88f, 0.94f, 1.0f, 0.85f);
            subObj.GetComponent<RectTransform>().sizeDelta = new Vector2(cw, 20);

            ZenTouchButton touchBtn = btnObj.AddComponent<ZenTouchButton>();
            touchBtn.visualPlate = plate.transform;
            touchBtn.neonBorderRenderers = neonRenderers;
            touchBtn.buttonLabelText = titleTxt;
            touchBtn.buttonSubLabelText = subTxt;
            touchBtn.touchHalfSize = new Vector2(size.x * 0.52f, size.y * 0.52f);
            touchBtn.isCircular = false;
            touchBtn.pressDisplacement = new Vector3(0f, 0f, 0.010f);
            return touchBtn;
        }

        #endregion
    }

    public struct BranchSpline
    {
        public Vector3 p0, p1, p2, p3;
        public float rStart, rEnd;

        public BranchSpline(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float rs, float re)
        {
            p0 = a; p1 = b; p2 = c; p3 = d;
            rStart = rs; rEnd = re;
        }
    }

    public static class ZenBonsaiProceduralMeshes
    {
        private const string MeshFolder = "Assets/ZenBonsai/Meshes";

        public static Mesh CreateConicalMountainMesh(string assetName, float baseRadius, float topRadius, float height, int radialSegs)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int rings = 12;
            for (int r = 0; r <= rings; r++)
            {
                float t = r / (float)rings;
                // Silueta cóncava natural del volcán Monte Fuji
                float curveT = Mathf.Pow(1f - t, 1.45f);
                float radius = Mathf.Lerp(topRadius, baseRadius, curveT);
                float y = t * height;

                for (int s = 0; s <= radialSegs; s++)
                {
                    float u = s / (float)radialSegs;
                    float ang = u * Mathf.PI * 2f;
                    float ridge = 1f + Mathf.Sin(ang * 7f + t * 4f) * 0.04f * (1f - t * 0.5f);
                    verts.Add(new Vector3(Mathf.Cos(ang) * radius * ridge, y, Mathf.Sin(ang) * radius * ridge));
                    uvs.Add(new Vector2(u, t));
                }
            }

            int stride = radialSegs + 1;
            for (int r = 0; r < rings; r++)
            {
                for (int s = 0; s < radialSegs; s++)
                {
                    int a = r * stride + s;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;

                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                }
            }

            // Tapa superior del cráter
            int topCenter = verts.Count;
            verts.Add(new Vector3(0f, height, 0f));
            uvs.Add(new Vector2(0.5f, 1f));
            int lastRing = rings * stride;
            for (int s = 0; s < radialSegs; s++)
            {
                tris.Add(lastRing + s);
                tris.Add(topCenter);
                tris.Add(lastRing + s + 1);
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/ZenBonsai"))
                AssetDatabase.CreateFolder("Assets", "ZenBonsai");
            if (!AssetDatabase.IsValidFolder(MeshFolder))
                AssetDatabase.CreateFolder("Assets/ZenBonsai", "Meshes");
        }

        private static Mesh SaveOrReplaceMesh(Mesh mesh, string assetName)
        {
            EnsureFolder();
            mesh.name = assetName;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            string path = $"{MeshFolder}/{assetName}.asset";
            Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null)
            {
                existing.Clear();
                EditorUtility.CopySerialized(mesh, existing);
                EditorUtility.SetDirty(existing);
                return existing;
            }
            else
            {
                AssetDatabase.CreateAsset(mesh, path);
                return mesh;
            }
        }

        private static Vector3 EvalBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float u = 1f - t;
            return u * u * u * p0 + 3f * u * u * t * p1 + 3f * u * t * t * p2 + t * t * t * p3;
        }

        private static Vector3 EvalBezierTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float u = 1f - t;
            Vector3 d = 3f * u * u * (p1 - p0) + 6f * u * t * (p2 - p1) + 3f * t * t * (p3 - p2);
            return d.sqrMagnitude > 0.00001f ? d.normalized : Vector3.up;
        }

        public static Mesh CreateMasterTreeWoodMesh(
            string assetName,
            Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,
            float rBase, float rMid, float rTop,
            BranchSpline[] branches,
            int rootLobules,
            float rootFlareStrength,
            float barkRidgeDepth,
            float barkTwist,
            int seed)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Tronco único de alta definición (52 anillos verticales x 32 segmentos radiales) con venas cambiales y conicidad orgánica
            AppendSplineTube(
                verts, tris, uvs,
                p0, p1, p2, p3,
                rBase, rMid, rTop,
                52, 32,
                rootLobules, rootFlareStrength,
                barkRidgeDepth, barkTwist, seed, true);

            Random.State oldState = Random.state;
            Random.InitState(seed + 101);

            // Raíces superficiales Nebari vigorosas que se hunden orgánicamente en el manto de musgo
            for (int r = 0; r < rootLobules; r++)
            {
                float angle = (r / (float)rootLobules) * Mathf.PI * 2f + Random.Range(-0.18f, 0.18f);
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                Vector3 sideRoot = new Vector3(-dir.z, 0f, dir.x);
                Vector3 rp0 = p0 + new Vector3(0f, 0.022f, 0f) + dir * (rBase * 0.42f);
                Vector3 rp1 = rp0 + dir * 0.028f + new Vector3(0f, -0.005f, 0f);
                Vector3 rp2 = rp0 + dir * 0.056f + sideRoot * Random.Range(-0.012f, 0.012f) + new Vector3(0f, -0.014f, 0f);
                Vector3 rp3 = rp0 + dir * 0.086f + sideRoot * Random.Range(-0.016f, 0.016f) + new Vector3(0f, -0.022f, 0f);

                AppendSplineTube(
                    verts, tris, uvs,
                    rp0, rp1, rp2, rp3,
                    rBase * 0.44f, rBase * 0.26f, 0.0035f,
                    18, 14,
                    0, 0f, barkRidgeDepth * 0.7f, barkTwist * 0.5f, seed + r * 17, true);
            }

            if (branches != null)
            {
                for (int b = 0; b < branches.Length; b++)
                {
                    var br = branches[b];
                    float rMidBr = Mathf.Lerp(br.rStart, br.rEnd, 0.55f);
                    // Rama primaria con collar de inserción muscular (rStart * 1.12f)
                    AppendSplineTube(
                        verts, tris, uvs,
                        br.p0, br.p1, br.p2, br.p3,
                        br.rStart * 1.12f, rMidBr, br.rEnd,
                        28, 18,
                        0, 0f, barkRidgeDepth * 0.60f, barkTwist * 0.7f, seed + 300 + b * 29, true);

                    Vector3 tipDir = EvalBezierTangent(br.p0, br.p1, br.p2, br.p3, 0.90f);
                    Vector3 side = Vector3.Cross(tipDir, Vector3.up);
                    if (side.sqrMagnitude < 0.01f) side = Vector3.right;
                    side.Normalize();

                    // Ramificación secundaria y terciaria en abanico que sostiene cada nube de follaje (visibles entre las flores)
                    for (int s = -2; s <= 2; s++)
                    {
                        float tAttach = Mathf.Clamp(0.62f + Mathf.Abs(s) * 0.08f, 0.52f, 0.92f);
                        Vector3 twigStart = EvalBezier(br.p0, br.p1, br.p2, br.p3, tAttach);
                        Vector3 spreadDir = (tipDir + side * (s * 0.52f) + Vector3.up * (0.24f + (s % 2 == 0 ? 0.08f : 0f))).normalized;
                        Vector3 tp1 = twigStart + spreadDir * 0.018f;
                        Vector3 tp2 = twigStart + spreadDir * 0.034f + Vector3.up * 0.007f;
                        Vector3 tp3 = twigStart + spreadDir * 0.052f + Vector3.up * 0.014f;

                        AppendSplineTube(
                            verts, tris, uvs,
                            twigStart, tp1, tp2, tp3,
                            br.rEnd * 1.05f, br.rEnd * 0.68f, br.rEnd * 0.30f,
                            12, 10,
                            0, 0f, barkRidgeDepth * 0.35f, 1.2f, seed + 600 + b * 19 + s, true);

                        // Ramillas terciarias finas que asoman orgánicamente entre los pétalos (como en la imagen de presentación)
                        for (int tSub = -1; tSub <= 1; tSub += 2)
                        {
                            Vector3 subStart = EvalBezier(twigStart, tp1, tp2, tp3, 0.65f);
                            Vector3 subDir = (spreadDir + side * (tSub * 0.42f) + Vector3.up * 0.32f).normalized;
                            AppendSplineTube(
                                verts, tris, uvs,
                                subStart,
                                subStart + subDir * 0.010f,
                                subStart + subDir * 0.020f + Vector3.up * 0.004f,
                                subStart + subDir * 0.030f + Vector3.up * 0.008f,
                                br.rEnd * 0.48f, br.rEnd * 0.30f, 0.0014f,
                                7, 7,
                                0, 0f, barkRidgeDepth * 0.20f, 0.8f, seed + 900 + b * 31 + s * 7 + tSub, true);
                        }
                    }
                }
            }

            Random.state = oldState;

            Mesh mesh = new Mesh();
            if (verts.Count > 65000) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        private static void AppendSplineTube(
            List<Vector3> verts, List<int> tris, List<Vector2> uvs,
            Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,
            float rBase, float rMid, float rTop,
            int rings, int radialSegs,
            int rootLobules, float rootFlare,
            float barkRoughness, float barkTwist,
            int seed, bool capEnd)
        {
            int baseVertIndex = verts.Count;
            Vector3 prevTangent = EvalBezierTangent(p0, p1, p2, p3, 0f);
            Vector3 normal = Vector3.Cross(prevTangent, Vector3.forward);
            if (normal.sqrMagnitude < 0.001f) normal = Vector3.Cross(prevTangent, Vector3.right);
            normal.Normalize();

            for (int i = 0; i <= rings; i++)
            {
                float t = i / (float)rings;
                Vector3 center = EvalBezier(p0, p1, p2, p3, t);
                Vector3 tangent = EvalBezierTangent(p0, p1, p2, p3, t);

                Quaternion rot = Quaternion.FromToRotation(prevTangent, tangent);
                normal = (rot * normal).normalized;
                Vector3 binormal = Vector3.Cross(tangent, normal).normalized;
                prevTangent = tangent;

                float baseRadius = t < 0.5f
                    ? Mathf.Lerp(rBase, rMid, t * 2f)
                    : Mathf.Lerp(rMid, rTop, (t - 0.5f) * 2f);

                float nebariFactor = (rootLobules > 0 && t < 0.28f)
                    ? Mathf.Pow(1f - (t / 0.28f), 2.1f) * rootFlare
                    : 0f;

                for (int j = 0; j <= radialSegs; j++)
                {
                    float u = j / (float)radialSegs;
                    float angle = u * Mathf.PI * 2f;

                    float rMod = 1f;
                    if (nebariFactor > 0f)
                    {
                        float lobe = Mathf.Max(0f, Mathf.Cos(angle * rootLobules + seed * 0.5f));
                        rMod += nebariFactor * lobe;
                    }

                    // Venas musculares de cambium vivo (3-4 cordones orgánicos que giran suavemente por el tronco)
                    float cambiumMuscle = Mathf.Sin(angle * 3f + t * barkTwist * 0.65f + seed * 0.3f) * 0.14f;
                    float furrow1 = Mathf.Sin(angle * 9f + t * barkTwist + seed) * 0.45f;
                    float furrow2 = Mathf.Cos(angle * 17f - t * barkTwist * 1.3f + seed * 2f) * 0.40f;
                    float burlSwelling = Mathf.Pow(Mathf.Max(0f, Mathf.Sin(t * Mathf.PI * 4f + angle * 2f + seed)), 3f) * 0.32f;

                    rMod += cambiumMuscle + (furrow1 + furrow2 + burlSwelling) * barkRoughness;

                    float finalR = Mathf.Max(0.0015f, baseRadius * rMod);
                    Vector3 radialDir = normal * Mathf.Cos(angle) + binormal * Mathf.Sin(angle);
                    verts.Add(center + radialDir * finalR);
                    uvs.Add(new Vector2(u, t));
                }
            }

            int stride = radialSegs + 1;
            for (int i = 0; i < rings; i++)
            {
                for (int j = 0; j < radialSegs; j++)
                {
                    int a = baseVertIndex + i * stride + j;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;

                    // Orden horario visto desde fuera en el sistema izquierdo de Unity -> normales 100% hacia el exterior
                    tris.Add(a); tris.Add(c); tris.Add(b);
                    tris.Add(a); tris.Add(d); tris.Add(c);
                }
            }

            // Tapa inferior en la base del tubo para cerrar completamente la geometría
            Vector3 baseCenter = EvalBezier(p0, p1, p2, p3, 0f);
            int baseCapIdx = verts.Count;
            verts.Add(baseCenter);
            uvs.Add(new Vector2(0.5f, 0f));
            for (int j = 0; j < radialSegs; j++)
            {
                tris.Add(baseVertIndex + j);
                tris.Add(baseCapIdx);
                tris.Add(baseVertIndex + j + 1);
            }

            if (capEnd)
            {
                Vector3 tipCenter = EvalBezier(p0, p1, p2, p3, 1f);
                int tipIdx = verts.Count;
                verts.Add(tipCenter);
                uvs.Add(new Vector2(0.5f, 1f));
                int lastRingStart = baseVertIndex + rings * stride;
                for (int j = 0; j < radialSegs; j++)
                {
                    tris.Add(lastRingStart + j);
                    tris.Add(lastRingStart + j + 1);
                    tris.Add(tipIdx);
                }
            }
        }

        public static Mesh CreateFoliageCloudPadMesh(string assetName, Vector3 padSize, int puffCount, float bottomFlatness, float leafSpikiness, int seed)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float rx = padSize.x * 0.5f;
            float ry = padSize.y * 0.5f;
            float rz = padSize.z * 0.5f;

            // En lugar de una bola sólida opaca que tape las ramas, generamos ramilletes orgánicos aireados
            // de sub-brotes y hojas individuales que dejan entrever la madera del tronco y ramillas
            int bouquetCount = puffCount + 10;
            for (int p = 0; p < bouquetCount; p++)
            {
                float ang = (p / (float)bouquetCount) * Mathf.PI * 2f * 2.39996f + Random.Range(-0.18f, 0.18f);
                float ringDist = Mathf.Sqrt((p + 0.5f) / bouquetCount) * Random.Range(0.28f, 0.92f);
                float domeLift = Mathf.Cos(ringDist * Mathf.PI * 0.48f) * ry * 0.68f + Random.Range(-ry * 0.10f, ry * 0.22f);
                Vector3 offset = new Vector3(Mathf.Cos(ang) * rx * ringDist, domeLift, Mathf.Sin(ang) * rz * ringDist);
                float s = Random.Range(0.18f, 0.30f);
                Vector3 subRadii = new Vector3(rx * s, ry * (s + 0.12f), rz * s);

                AppendBotanicalPuff(verts, tris, uvs, offset, subRadii, bottomFlatness, leafSpikiness + 0.10f, seed + p * 13);
            }

            // Hojas/brotes individuales salientes en los bordes de la nube para dar silueta orgánica natural
            int outerLeafCount = 32;
            for (int l = 0; l < outerLeafCount; l++)
            {
                float ang = Random.Range(0f, Mathf.PI * 2f);
                float r = Random.Range(0.35f, 0.98f);
                Vector3 pos = new Vector3(Mathf.Cos(ang) * rx * r, Random.Range(-ry * 0.15f, ry * 0.78f), Mathf.Sin(ang) * rz * r);
                Vector3 outDir = new Vector3(pos.x / rx, 0.85f, pos.z / rz).normalized;
                Quaternion rot = Quaternion.LookRotation(outDir) * Quaternion.Euler(Random.Range(-25f, 25f), Random.Range(-25f, 25f), Random.Range(0f, 360f));
                AppendSingleCurvedPetal(verts, tris, uvs, pos, rot, Random.Range(0.012f, 0.018f));
            }

            Random.state = oldState;

            Mesh mesh = new Mesh();
            if (verts.Count > 65000) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        private static void AppendBotanicalPuff(
            List<Vector3> verts, List<int> tris, List<Vector2> uvs,
            Vector3 center, Vector3 radii, float bottomFlatness, float spikiness, int seed)
        {
            int latSegs = 8;
            int lonSegs = 12;
            int baseIdx = verts.Count;

            for (int lat = 0; lat <= latSegs; lat++)
            {
                float v = lat / (float)latSegs;
                float phi = (v - 0.5f) * Mathf.PI;
                float cosPhi = Mathf.Cos(phi);
                float sinPhi = Mathf.Sin(phi);

                for (int lon = 0; lon <= lonSegs; lon++)
                {
                    float u = lon / (float)lonSegs;
                    float theta = u * Mathf.PI * 2f;

                    Vector3 dir = new Vector3(cosPhi * Mathf.Cos(theta), sinPhi, cosPhi * Mathf.Sin(theta));
                    if (dir.y < 0f)
                    {
                        dir.y *= (1f - bottomFlatness);
                    }

                    float leafWave = Mathf.Abs(Mathf.Sin(theta * 5f + seed) * Mathf.Cos(phi * 4f + seed * 0.5f));
                    float scaleMod = 1f + leafWave * spikiness;

                    Vector3 pos = center + new Vector3(dir.x * radii.x, dir.y * radii.y, dir.z * radii.z) * scaleMod;
                    verts.Add(pos);
                    uvs.Add(new Vector2(u, v));
                }
            }

            int stride = lonSegs + 1;
            for (int lat = 0; lat < latSegs; lat++)
            {
                for (int lon = 0; lon < lonSegs; lon++)
                {
                    int a = baseIdx + lat * stride + lon;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;

                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                }
            }
        }

        public static Mesh CreateSakuraBlossomCorollasMesh(string assetName, Vector3 padSize, int flowerCount, int seed, out Mesh stamenMesh)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            List<Vector3> pVerts = new List<Vector3>();
            List<int> pTris = new List<int>();
            List<Vector2> pUvs = new List<Vector2>();

            List<Vector3> sVerts = new List<Vector3>();
            List<int> sTris = new List<int>();
            List<Vector2> sUvs = new List<Vector2>();

            float rx = padSize.x * 0.48f;
            float ry = padSize.y * 0.52f;
            float rz = padSize.z * 0.48f;

            for (int f = 0; f < flowerCount; f++)
            {
                float u = Random.Range(0f, Mathf.PI * 2f);
                float r = Mathf.Sqrt(Random.Range(0.05f, 0.98f));
                float x = Mathf.Cos(u) * r * rx;
                float z = Mathf.Sin(u) * r * rz;
                float domeY = Mathf.Sqrt(Mathf.Max(0.05f, 1f - r * r * 0.75f)) * ry + Random.Range(0.002f, 0.009f);

                Vector3 flowerCenter = new Vector3(x, domeY, z);
                Vector3 normal = new Vector3(x / rx, 1.35f, z / rz).normalized;
                Quaternion rot = Quaternion.LookRotation(normal) * Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
                float flowerScale = Random.Range(0.011f, 0.016f);

                AppendFivePetalSakuraFlower(pVerts, pTris, pUvs, sVerts, sTris, sUvs, flowerCenter, rot, flowerScale);
            }

            Random.state = oldState;

            Mesh petalMesh = new Mesh();
            petalMesh.SetVertices(pVerts);
            petalMesh.SetTriangles(pTris, 0);
            petalMesh.SetUVs(0, pUvs);

            Mesh stMesh = new Mesh();
            stMesh.SetVertices(sVerts);
            stMesh.SetTriangles(sTris, 0);
            stMesh.SetUVs(0, sUvs);

            stamenMesh = SaveOrReplaceMesh(stMesh, assetName + "_Stamens");
            return SaveOrReplaceMesh(petalMesh, assetName);
        }

        private static void AppendFivePetalSakuraFlower(
            List<Vector3> pVerts, List<int> pTris, List<Vector2> pUvs,
            List<Vector3> sVerts, List<int> sTris, List<Vector2> sUvs,
            Vector3 center, Quaternion rot, float radius)
        {
            for (int p = 0; p < 5; p++)
            {
                Quaternion petalRot = rot * Quaternion.Euler(0f, 0f, p * 72f);

                int idx = pVerts.Count;
                Vector3 v0 = center;
                Vector3 v1 = center + petalRot * new Vector3(-radius * 0.34f, radius * 0.55f, radius * 0.14f);
                Vector3 v2 = center + petalRot * new Vector3(-radius * 0.20f, radius * 0.98f, radius * 0.22f);
                Vector3 v3 = center + petalRot * new Vector3(0f, radius * 0.84f, radius * 0.18f);
                Vector3 v4 = center + petalRot * new Vector3(radius * 0.20f, radius * 0.98f, radius * 0.22f);
                Vector3 v5 = center + petalRot * new Vector3(radius * 0.34f, radius * 0.55f, radius * 0.14f);

                pVerts.Add(v0); pVerts.Add(v1); pVerts.Add(v2);
                pVerts.Add(v3); pVerts.Add(v4); pVerts.Add(v5);

                // UVs radiales desde el cáliz central (0.5, 0.5) hacia los bordes del pétalo para mostrar el corazón carmesí (Sakura-ben)
                pUvs.Add(new Vector2(0.50f, 0.50f));
                pUvs.Add(new Vector2(0.18f, 0.72f));
                pUvs.Add(new Vector2(0.30f, 0.96f));
                pUvs.Add(new Vector2(0.50f, 0.88f));
                pUvs.Add(new Vector2(0.70f, 0.96f));
                pUvs.Add(new Vector2(0.82f, 0.72f));

                pTris.Add(idx + 0); pTris.Add(idx + 1); pTris.Add(idx + 2);
                pTris.Add(idx + 0); pTris.Add(idx + 2); pTris.Add(idx + 3);
                pTris.Add(idx + 0); pTris.Add(idx + 3); pTris.Add(idx + 4);
                pTris.Add(idx + 0); pTris.Add(idx + 4); pTris.Add(idx + 5);

                pTris.Add(idx + 0); pTris.Add(idx + 2); pTris.Add(idx + 1);
                pTris.Add(idx + 0); pTris.Add(idx + 3); pTris.Add(idx + 2);
                pTris.Add(idx + 0); pTris.Add(idx + 4); pTris.Add(idx + 3);
                pTris.Add(idx + 0); pTris.Add(idx + 5); pTris.Add(idx + 4);
            }

            int sIdx = sVerts.Count;
            float sr = radius * 0.22f;
            Vector3 sc = center + rot * new Vector3(0f, 0f, sr * 0.6f);
            sVerts.Add(sc + rot * new Vector3(0f, 0f, sr));
            sVerts.Add(sc + rot * new Vector3(sr, 0f, 0f));
            sVerts.Add(sc + rot * new Vector3(0f, sr, 0f));
            sVerts.Add(sc + rot * new Vector3(-sr, 0f, 0f));
            sVerts.Add(sc + rot * new Vector3(0f, -sr, 0f));
            for (int k = 0; k < 5; k++) sUvs.Add(Vector2.one * 0.5f);

            sTris.Add(sIdx + 0); sTris.Add(sIdx + 1); sTris.Add(sIdx + 2);
            sTris.Add(sIdx + 0); sTris.Add(sIdx + 2); sTris.Add(sIdx + 3);
            sTris.Add(sIdx + 0); sTris.Add(sIdx + 3); sTris.Add(sIdx + 4);
            sTris.Add(sIdx + 0); sTris.Add(sIdx + 4); sTris.Add(sIdx + 1);
        }

        public static Mesh CreateScatteredTablePetalsMesh(string assetName, int count, int seed)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            for (int i = 0; i < count; i++)
            {
                // Distribución en anillo elíptico alrededor de la maceta rectangular (ancho 0.34m, fondo 0.25m)
                float ang = Random.Range(0f, Mathf.PI * 2f);
                float dist = Random.Range(0.19f, 0.34f);
                float x = Mathf.Cos(ang) * (dist * 1.15f);
                float z = Mathf.Sin(ang) * (dist * 0.85f);
                // Evitar el centro justo donde está la maceta
                if (Mathf.Abs(x) < 0.17f && Mathf.Abs(z) < 0.13f)
                {
                    x += Mathf.Sign(x != 0 ? x : 1f) * 0.06f;
                }

                Vector3 pos = new Vector3(x, Random.Range(0.0005f, 0.0020f), z);
                float yaw = Random.Range(0f, 360f);
                float pitch = Random.Range(-5f, 5f);
                Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
                float scale = Random.Range(0.009f, 0.014f);

                AppendSingleCurvedPetal(verts, tris, uvs, pos, rot, scale);
            }

            Random.state = oldState;

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        private static void AppendSingleCurvedPetal(
            List<Vector3> verts, List<int> tris, List<Vector2> uvs,
            Vector3 center, Quaternion rot, float size)
        {
            int idx = verts.Count;
            float w = size * 0.55f;
            float l = size;

            Vector3 v0 = center;
            Vector3 v1 = center + rot * new Vector3(-w * 0.50f, 0.0006f, l * 0.35f);
            Vector3 v2 = center + rot * new Vector3(-w * 0.35f, 0.0014f, l * 0.88f);
            Vector3 v3 = center + rot * new Vector3(0f, 0.0018f, l * 0.76f);
            Vector3 v4 = center + rot * new Vector3(w * 0.35f, 0.0014f, l * 0.88f);
            Vector3 v5 = center + rot * new Vector3(w * 0.50f, 0.0006f, l * 0.35f);

            verts.Add(v0); verts.Add(v1); verts.Add(v2);
            verts.Add(v3); verts.Add(v4); verts.Add(v5);

            for (int k = 0; k < 6; k++) uvs.Add(new Vector2(0.5f, 0.5f));

            // Doble cara para que sea visible desde cualquier ángulo en VR
            tris.Add(idx + 0); tris.Add(idx + 1); tris.Add(idx + 2);
            tris.Add(idx + 0); tris.Add(idx + 2); tris.Add(idx + 3);
            tris.Add(idx + 0); tris.Add(idx + 3); tris.Add(idx + 4);
            tris.Add(idx + 0); tris.Add(idx + 4); tris.Add(idx + 5);

            tris.Add(idx + 0); tris.Add(idx + 2); tris.Add(idx + 1);
            tris.Add(idx + 0); tris.Add(idx + 3); tris.Add(idx + 2);
            tris.Add(idx + 0); tris.Add(idx + 4); tris.Add(idx + 3);
            tris.Add(idx + 0); tris.Add(idx + 5); tris.Add(idx + 4);
        }

        public static Mesh CreateMatsuNeedleClustersMesh(string assetName, Vector3 padSize, int tuftCount, int seed)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float rx = padSize.x * 0.48f;
            float ry = padSize.y * 0.50f;
            float rz = padSize.z * 0.48f;

            for (int t = 0; t < tuftCount; t++)
            {
                float ang = Random.Range(0f, Mathf.PI * 2f);
                float r = Mathf.Sqrt(Random.Range(0.02f, 0.96f));
                float x = Mathf.Cos(ang) * r * rx;
                float z = Mathf.Sin(ang) * r * rz;
                float y = Mathf.Sqrt(Mathf.Max(0.05f, 1f - r * r * 0.8f)) * ry;

                Vector3 tuftOrigin = new Vector3(x, y, z);
                Vector3 upDir = new Vector3(x * 0.4f, 1.0f, z * 0.4f).normalized;

                for (int n = 0; n < 5; n++)
                {
                    float nAng = (n * 72f + Random.Range(-12f, 12f)) * Mathf.Deg2Rad;
                    Vector3 needleDir = (upDir * 0.78f + new Vector3(Mathf.Cos(nAng) * 0.62f, 0f, Mathf.Sin(nAng) * 0.62f)).normalized;
                    float len = Random.Range(0.015f, 0.022f);
                    float w = 0.0022f;

                    Vector3 side = Vector3.Cross(needleDir, Vector3.up);
                    if (side.sqrMagnitude < 0.001f) side = Vector3.right;
                    side.Normalize();

                    int idx = verts.Count;
                    verts.Add(tuftOrigin - side * w);
                    verts.Add(tuftOrigin + side * w);
                    verts.Add(tuftOrigin + needleDir * len);

                    uvs.Add(new Vector2(0f, 0f));
                    uvs.Add(new Vector2(1f, 0f));
                    uvs.Add(new Vector2(0.5f, 1f));

                    tris.Add(idx + 0); tris.Add(idx + 2); tris.Add(idx + 1);
                    tris.Add(idx + 0); tris.Add(idx + 1); tris.Add(idx + 2);
                }
            }

            Random.state = oldState;

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateMomijiStarLeavesMesh(string assetName, Vector3 padSize, int leafCount, int seed)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float rx = padSize.x * 0.50f;
            float ry = padSize.y * 0.52f;
            float rz = padSize.z * 0.50f;

            float[] lobeAngles = { -72f, -36f, 0f, 36f, 72f };
            float[] lobeLengths = { 0.65f, 0.88f, 1.0f, 0.88f, 0.65f };

            for (int l = 0; l < leafCount; l++)
            {
                float u = Random.Range(0f, Mathf.PI * 2f);
                float r = Mathf.Sqrt(Random.Range(0.05f, 0.98f));
                float x = Mathf.Cos(u) * r * rx;
                float z = Mathf.Sin(u) * r * rz;
                float y = Mathf.Sqrt(Mathf.Max(0.05f, 1f - r * r * 0.78f)) * ry + Random.Range(-0.003f, 0.008f);

                Vector3 leafCenter = new Vector3(x, y, z);
                Vector3 normal = new Vector3(x / rx, 1.25f, z / rz).normalized;
                Quaternion rot = Quaternion.LookRotation(normal) * Quaternion.Euler(Random.Range(-25f, 25f), Random.Range(-25f, 25f), Random.Range(0f, 360f));
                float leafSize = Random.Range(0.013f, 0.019f);

                for (int lb = 0; lb < 5; lb++)
                {
                    Quaternion lobeRot = rot * Quaternion.Euler(0f, 0f, lobeAngles[lb]);
                    float len = leafSize * lobeLengths[lb];
                    float w = leafSize * 0.16f;

                    int idx = verts.Count;
                    verts.Add(leafCenter);
                    verts.Add(leafCenter + lobeRot * new Vector3(-w, len * 0.45f, 0.001f));
                    verts.Add(leafCenter + lobeRot * new Vector3(0f, len, -0.001f));
                    verts.Add(leafCenter + lobeRot * new Vector3(w, len * 0.45f, 0.001f));

                    for (int k = 0; k < 4; k++) uvs.Add(Vector2.one * 0.5f);

                    tris.Add(idx + 0); tris.Add(idx + 1); tris.Add(idx + 2);
                    tris.Add(idx + 0); tris.Add(idx + 2); tris.Add(idx + 3);
                    tris.Add(idx + 0); tris.Add(idx + 2); tris.Add(idx + 1);
                    tris.Add(idx + 0); tris.Add(idx + 3); tris.Add(idx + 2);
                }
            }

            Random.state = oldState;

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateTokonamePotMesh(string assetName, float width, float depth, float height)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float[] yProfile = { 0.0f, 0.006f, height * 0.82f, height * 0.85f, height, height, height * 0.72f };
            float[] scaleProfile = { 0.84f, 0.88f, 0.98f, 1.06f, 1.06f, 0.90f, 0.86f };

            int ringSegs = 48;
            int profileCount = yProfile.Length;

            for (int p = 0; p < profileCount; p++)
            {
                float y = yProfile[p];
                float s = scaleProfile[p];
                float hx = (width * 0.5f) * s;
                float hz = (depth * 0.5f) * s;

                for (int i = 0; i <= ringSegs; i++)
                {
                    float u = i / (float)ringSegs;
                    float ang = u * Mathf.PI * 2f;
                    float cosA = Mathf.Cos(ang);
                    float sinA = Mathf.Sin(ang);
                    float sx = Mathf.Sign(cosA) * Mathf.Pow(Mathf.Abs(cosA), 0.55f) * hx;
                    float sz = Mathf.Sign(sinA) * Mathf.Pow(Mathf.Abs(sinA), 0.55f) * hz;

                    verts.Add(new Vector3(sx, y, sz));
                    uvs.Add(new Vector2(u, p / (float)(profileCount - 1)));
                }
            }

            int stride = ringSegs + 1;
            for (int p = 0; p < profileCount - 1; p++)
            {
                for (int i = 0; i < ringSegs; i++)
                {
                    int a = p * stride + i;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;

                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                }
            }

            int bottomCenterIdx = verts.Count;
            verts.Add(new Vector3(0f, 0f, 0f));
            uvs.Add(new Vector2(0.5f, 0.5f));
            for (int i = 0; i < ringSegs; i++)
            {
                tris.Add(bottomCenterIdx);
                tris.Add(i + 1);
                tris.Add(i);
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateOrganicMossMoundMesh(string assetName, float width, float depth, float baseHeight, float moundLift)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int gridX = 24;
            int gridZ = 20;
            float hx = width * 0.5f;
            float hz = depth * 0.5f;

            for (int iz = 0; iz <= gridZ; iz++)
            {
                float vz = iz / (float)gridZ;
                float nz = (vz - 0.5f) * 2f;

                for (int ix = 0; ix <= gridX; ix++)
                {
                    float vx = ix / (float)gridX;
                    float nx = (vx - 0.5f) * 2f;

                    float r2 = Mathf.Clamp01(nx * nx * 0.85f + nz * nz * 0.85f);
                    float dome = Mathf.Cos(r2 * Mathf.PI * 0.5f) * moundLift;

                    float hill1 = Mathf.Sin(nx * 5.5f + 0.4f) * Mathf.Cos(nz * 4.8f - 0.3f) * 0.0045f;
                    float hill2 = Mathf.Sin(nx * 11f - nz * 9f) * 0.0020f;

                    float y = baseHeight + dome + (1f - r2 * 0.7f) * (hill1 + hill2);
                    verts.Add(new Vector3(nx * hx, y, nz * hz));
                    uvs.Add(new Vector2(vx, vz));
                }
            }

            int stride = gridX + 1;
            for (int iz = 0; iz < gridZ; iz++)
            {
                for (int ix = 0; ix < gridX; ix++)
                {
                    int a = iz * stride + ix;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;

                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateSuisekiStoneMesh(string assetName, Vector3 radii, int seed)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            AppendBotanicalPuff(verts, tris, uvs, Vector3.zero, radii, 0.65f, 0.24f, seed);

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        // =========================================================================
        // MALLAS ESCULPIDAS DE HERRAMIENTAS DE MAESTRO BONSÁI (TIJERAS FORJADAS, ESCOBILLA HOKI Y CAZO HISHAKU)
        // =========================================================================

        public static Mesh CreateSculptedShearsBodyMesh(string assetName, out Mesh rivetMesh)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Dos brazos/mangos ergonómicos curvados de acero forjado (como las tijeras de la imagen de presentación)
            for (int side = -1; side <= 1; side += 2)
            {
                Vector3 p0 = new Vector3(side * 0.005f, 0f, 0.002f);
                Vector3 p1 = new Vector3(side * 0.015f, 0f, -0.022f);
                Vector3 p2 = new Vector3(side * 0.024f, 0f, -0.052f);
                Vector3 p3 = new Vector3(side * 0.017f, 0f, -0.078f);

                AppendSplineTube(verts, tris, uvs, p0, p1, p2, p3, 0.0052f, 0.0046f, 0.0038f, 16, 12, 0, 0f, 0.02f, 0f, 10 + side, true);

                // Arco interior del ojal del mango (Satsuki / Okubo loop)
                Vector3 lp0 = p3;
                Vector3 lp1 = new Vector3(side * 0.007f, 0f, -0.082f);
                Vector3 lp2 = new Vector3(side * 0.006f, 0f, -0.062f);
                Vector3 lp3 = new Vector3(side * 0.014f, 0f, -0.036f);
                AppendSplineTube(verts, tris, uvs, lp0, lp1, lp2, lp3, 0.0038f, 0.0035f, 0.0038f, 12, 10, 0, 0f, 0.01f, 0f, 20 + side, true);
            }

            // Muelle helicoidal metálico (Volute Spring) entre los dos mangos (idéntico al de la imagen de referencia)
            Vector3 sp0 = new Vector3(-0.012f, 0f, -0.034f);
            Vector3 sp1 = new Vector3(-0.004f, 0.002f, -0.030f);
            Vector3 sp2 = new Vector3(0.004f, -0.002f, -0.030f);
            Vector3 sp3 = new Vector3(0.012f, 0f, -0.034f);
            AppendSplineTube(verts, tris, uvs, sp0, sp1, sp2, sp3, 0.0032f, 0.0045f, 0.0032f, 14, 12, 0, 0f, 0.18f, 18f, 77, true);

            // Remache / Perno central de latón dorado con arandela
            List<Vector3> rVerts = new List<Vector3>();
            List<int> rTris = new List<int>();
            List<Vector2> rUvs = new List<Vector2>();
            AppendSplineTube(
                rVerts, rTris, rUvs,
                new Vector3(0f, -0.0055f, 0f),
                new Vector3(0f, -0.0020f, 0f),
                new Vector3(0f, 0.0020f, 0f),
                new Vector3(0f, 0.0055f, 0f),
                0.0065f, 0.0068f, 0.0055f, 6, 14, 0, 0f, 0f, 0f, 1, true);

            Mesh rMesh = new Mesh();
            rMesh.SetVertices(rVerts);
            rMesh.SetTriangles(rTris, 0);
            rMesh.SetUVs(0, rUvs);
            rivetMesh = SaveOrReplaceMesh(rMesh, assetName + "_Rivet");

            Mesh bodyMesh = new Mesh();
            bodyMesh.SetVertices(verts);
            bodyMesh.SetTriangles(tris, 0);
            bodyMesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(bodyMesh, assetName);
        }

        public static Mesh CreateSculptedShearBladeMesh(string assetName, bool isLeftBlade)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float s = isLeftBlade ? 1f : -1f;
            // Hoja de acero biselada con lomo grueso, filo cortante agudo y punta afilada (centrada en su eje local)
            int segs = 12;
            float len = 0.082f;
            float zOffset = -0.036f;

            for (int i = 0; i <= segs; i++)
            {
                float t = i / (float)segs;
                float z = zOffset + t * len;
                float width = Mathf.Lerp(0.0115f, 0.0006f, Mathf.Pow(t, 1.25f));
                float thick = Mathf.Lerp(0.0026f, 0.0005f, t);
                float curveX = Mathf.Sin(t * Mathf.PI) * 0.0022f * s;

                Vector3 sharpEdge = new Vector3(curveX - s * width * 0.35f, 0f, z);
                Vector3 spineTop = new Vector3(curveX + s * width * 0.65f, thick, z);
                Vector3 spineBot = new Vector3(curveX + s * width * 0.65f, -thick, z);
                Vector3 backOuter = new Vector3(curveX + s * width * 0.85f, 0f, z);

                verts.Add(sharpEdge);
                verts.Add(spineTop);
                verts.Add(backOuter);
                verts.Add(spineBot);

                uvs.Add(new Vector2(0f, t));
                uvs.Add(new Vector2(0.4f, t));
                uvs.Add(new Vector2(0.8f, t));
                uvs.Add(new Vector2(1f, t));
            }

            for (int i = 0; i < segs; i++)
            {
                int b0 = i * 4;
                int b1 = (i + 1) * 4;
                for (int k = 0; k < 4; k++)
                {
                    int kNext = (k + 1) % 4;
                    tris.Add(b0 + k); tris.Add(b1 + k); tris.Add(b1 + kNext);
                    tris.Add(b0 + k); tris.Add(b1 + kNext); tris.Add(b0 + kNext);
                    tris.Add(b0 + k); tris.Add(b1 + kNext); tris.Add(b1 + k);
                    tris.Add(b0 + k); tris.Add(b0 + kNext); tris.Add(b1 + kNext);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateSculptedHokiBroomMesh(string assetName, out Mesh bristlesMesh, out Mesh cordMesh)
        {
            // 1. Mango de bambú con nudos naturales
            List<Vector3> sVerts = new List<Vector3>();
            List<int> sTris = new List<int>();
            List<Vector2> sUvs = new List<Vector2>();
            AppendSplineTube(
                sVerts, sTris, sUvs,
                new Vector3(0f, 0f, -0.072f),
                new Vector3(0f, 0f, -0.042f),
                new Vector3(0f, 0f, -0.015f),
                new Vector3(0f, 0f, 0.010f),
                0.0075f, 0.0082f, 0.0095f, 16, 14, 0, 0f, 0.06f, 0f, 31, true);

            // 2. Virola y atado trenzado tradicional Maki
            List<Vector3> cVerts = new List<Vector3>();
            List<int> cTris = new List<int>();
            List<Vector2> cUvs = new List<Vector2>();
            AppendSplineTube(
                cVerts, cTris, cUvs,
                new Vector3(0f, 0f, 0.006f),
                new Vector3(0f, 0f, 0.014f),
                new Vector3(0f, 0f, 0.022f),
                new Vector3(0f, 0f, 0.030f),
                0.0105f, 0.0135f, 0.0165f, 10, 14, 0, 0f, 0.12f, 12f, 41, true);

            // 3. Cabezal acampanado de fibras de palma Shuro con haces de cerdas esculpidos
            List<Vector3> bVerts = new List<Vector3>();
            List<int> bTris = new List<int>();
            List<Vector2> bUvs = new List<Vector2>();
            for (int f = -4; f <= 4; f++)
            {
                float spreadX = f * 0.0052f;
                Vector3 bp0 = new Vector3(spreadX * 0.35f, 0f, 0.024f);
                Vector3 bp1 = new Vector3(spreadX * 0.65f, 0f, 0.044f);
                Vector3 bp2 = new Vector3(spreadX * 0.90f, 0f, 0.065f);
                Vector3 bp3 = new Vector3(spreadX * 1.15f, 0f, 0.084f);
                AppendSplineTube(bVerts, bTris, bUvs, bp0, bp1, bp2, bp3, 0.0065f, 0.0072f, 0.0042f, 10, 10, 0, 0f, 0.15f, 4f, 50 + f, true);
            }

            Mesh bMesh = new Mesh();
            bMesh.SetVertices(bVerts); bMesh.SetTriangles(bTris, 0); bMesh.SetUVs(0, bUvs);
            bristlesMesh = SaveOrReplaceMesh(bMesh, assetName + "_Bristles");

            Mesh cMesh = new Mesh();
            cMesh.SetVertices(cVerts); cMesh.SetTriangles(cTris, 0); cMesh.SetUVs(0, cUvs);
            cordMesh = SaveOrReplaceMesh(cMesh, assetName + "_Binding");

            Mesh sMesh = new Mesh();
            sMesh.SetVertices(sVerts); sMesh.SetTriangles(sTris, 0); sMesh.SetUVs(0, sUvs);
            return SaveOrReplaceMesh(sMesh, assetName);
        }

        public static Mesh CreateSculptedHishakuLadleMesh(string assetName, out Mesh waterMeniscusMesh)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // 1. Mango largo de bambú ahusado con nudos naturales atravesando el cuenco
            AppendSplineTube(
                verts, tris, uvs,
                new Vector3(0f, -0.002f, -0.095f),
                new Vector3(0f, 0.000f, -0.045f),
                new Vector3(0f, 0.002f, 0.005f),
                new Vector3(0f, 0.004f, 0.042f),
                0.0052f, 0.0058f, 0.0048f, 18, 12, 0, 0f, 0.05f, 0f, 88, true);

            // 2. Cuenco de bambú ahuecado (pared exterior, labio biselado superior y cavidad interior real)
            Vector3 cupCenter = new Vector3(0f, 0.004f, 0.058f);
            float outerR = 0.022f;
            float innerR = 0.0185f;
            float hBot = -0.018f;
            float hTop = 0.020f;
            float hInnerBot = -0.014f;

            float[] rProf = { 0.001f, outerR * 0.96f, outerR, outerR * 1.04f, outerR, innerR, innerR * 0.95f, 0.001f };
            float[] yProf = { hBot, hBot, -0.010f, -0.006f, hTop, hTop, hInnerBot, hInnerBot };

            int segs = 28;
            int baseIdx = verts.Count;
            for (int p = 0; p < rProf.Length; p++)
            {
                for (int s = 0; s <= segs; s++)
                {
                    float u = s / (float)segs;
                    float ang = u * Mathf.PI * 2f;
                    verts.Add(cupCenter + new Vector3(Mathf.Cos(ang) * rProf[p], yProf[p], Mathf.Sin(ang) * rProf[p]));
                    uvs.Add(new Vector2(u, p / (float)(rProf.Length - 1)));
                }
            }

            int stride = segs + 1;
            for (int p = 0; p < rProf.Length - 1; p++)
            {
                for (int s = 0; s < segs; s++)
                {
                    int a = baseIdx + p * stride + s;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                    tris.Add(a); tris.Add(c); tris.Add(b);
                    tris.Add(a); tris.Add(d); tris.Add(c);
                }
            }

            // 3. Disco de agua cristalina dentro de la cavidad del cazo
            List<Vector3> wVerts = new List<Vector3>();
            List<int> wTris = new List<int>();
            List<Vector2> wUvs = new List<Vector2>();
            Vector3 wCenter = cupCenter + new Vector3(0f, hTop * 0.65f, 0f);
            wVerts.Add(wCenter);
            wUvs.Add(new Vector2(0.5f, 0.5f));
            for (int s = 0; s <= segs; s++)
            {
                float u = s / (float)segs;
                float ang = u * Mathf.PI * 2f;
                wVerts.Add(wCenter + new Vector3(Mathf.Cos(ang) * (innerR * 0.97f), 0f, Mathf.Sin(ang) * (innerR * 0.97f)));
                wUvs.Add(new Vector2(Mathf.Cos(ang) * 0.5f + 0.5f, Mathf.Sin(ang) * 0.5f + 0.5f));
            }
            for (int s = 0; s < segs; s++)
            {
                wTris.Add(0); wTris.Add(s + 2); wTris.Add(s + 1);
                wTris.Add(0); wTris.Add(s + 1); wTris.Add(s + 2);
            }

            Mesh wMesh = new Mesh();
            wMesh.SetVertices(wVerts); wMesh.SetTriangles(wTris, 0); wMesh.SetUVs(0, wUvs);
            waterMeniscusMesh = SaveOrReplaceMesh(wMesh, assetName + "_Water");

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateSingleFloatingSakuraPetalMesh(string assetName)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            AppendSingleCurvedPetal(verts, tris, uvs, Vector3.zero, Quaternion.identity, 0.015f);
            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        // =========================================================================
        // MALLAS ARQUITECTÓNICAS Y PAISAJÍSTICAS DE LOS 3 SANTUARIOS CULTURALES JAPONESES
        // =========================================================================

        public static Mesh CreateCurvedPagodaRoofMesh(string assetName, float width, float depth, float height, float eaveLift)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int gridX = 16;
            int gridZ = 16;
            float hx = width * 0.5f;
            float hz = depth * 0.5f;

            for (int iz = 0; iz <= gridZ; iz++)
            {
                float vz = iz / (float)gridZ;
                float nz = (vz - 0.5f) * 2f;
                for (int ix = 0; ix <= gridX; ix++)
                {
                    float vx = ix / (float)gridX;
                    float nx = (vx - 0.5f) * 2f;

                    float distEdge = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(nz));
                    float cornerDist = Mathf.Sqrt(nx * nx + nz * nz);
                    // Curva cóncava tradicional Irimoya con aleros levantados en las esquinas (Warabi-te)
                    float slope = Mathf.Pow(1f - distEdge, 1.55f) * height;
                    float cornerCurl = Mathf.Pow(Mathf.Clamp01((cornerDist - 0.85f) / 0.56f), 2.0f) * eaveLift;
                    float tileRib = Mathf.Sin(nx * Mathf.PI * 18f) * 0.012f * distEdge;

                    verts.Add(new Vector3(nx * hx, slope + cornerCurl + tileRib, nz * hz));
                    uvs.Add(new Vector2(vx, vz));
                }
            }

            int stride = gridX + 1;
            for (int iz = 0; iz < gridZ; iz++)
            {
                for (int ix = 0; ix < gridX; ix++)
                {
                    int a = iz * stride + ix;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                    tris.Add(a); tris.Add(c); tris.Add(b);
                    tris.Add(a); tris.Add(d); tris.Add(c);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateSculptedMyojinToriiMesh(string assetName, out Mesh darkCapsMesh)
        {
            List<Vector3> rVerts = new List<Vector3>();
            List<int> rTris = new List<int>();
            List<Vector2> rUvs = new List<Vector2>();

            List<Vector3> dVerts = new List<Vector3>();
            List<int> dTris = new List<int>();
            List<Vector2> dUvs = new List<Vector2>();

            // 1. Dos columnas principales ligeramente inclinadas hacia adentro (Hashira con Korobi auténtico)
            for (int side = -1; side <= 1; side += 2)
            {
                Vector3 p0 = new Vector3(side * 1.48f, 0.25f, 0f);
                Vector3 p1 = new Vector3(side * 1.44f, 1.35f, 0f);
                Vector3 p2 = new Vector3(side * 1.40f, 2.45f, 0f);
                Vector3 p3 = new Vector3(side * 1.36f, 3.42f, 0f);
                AppendSplineTube(rVerts, rTris, rUvs, p0, p1, p2, p3, 0.145f, 0.132f, 0.120f, 16, 18, 0, 0f, 0.01f, 0f, 10 + side, true);

                // Pedestal negro de piedra (Kamebara / Nemaki)
                AppendSplineTube(
                    dVerts, dTris, dUvs,
                    new Vector3(side * 1.48f, 0.0f, 0f),
                    new Vector3(side * 1.48f, 0.12f, 0f),
                    new Vector3(side * 1.48f, 0.24f, 0f),
                    new Vector3(side * 1.48f, 0.36f, 0f),
                    0.22f, 0.20f, 0.17f, 6, 18, 0, 0f, 0.02f, 0f, 20 + side, true);
            }

            // 2. Travesaño inferior Nuki y tablilla central Gakuzuka
            AppendSplineTube(
                rVerts, rTris, rUvs,
                new Vector3(-1.88f, 2.72f, 0f),
                new Vector3(-0.62f, 2.72f, 0f),
                new Vector3(0.62f, 2.72f, 0f),
                new Vector3(1.88f, 2.72f, 0f),
                0.085f, 0.088f, 0.085f, 14, 12, 0, 0f, 0.01f, 0f, 33, true);

            AppendSplineTube(
                rVerts, rTris, rUvs,
                new Vector3(0f, 2.72f, 0f),
                new Vector3(0f, 2.95f, 0f),
                new Vector3(0f, 3.18f, 0f),
                new Vector3(0f, 3.40f, 0f),
                0.095f, 0.095f, 0.095f, 6, 12, 0, 0f, 0.01f, 0f, 34, true);

            // 3. Dintel superior curvo Kasagi (con puntas arqueadas hacia el cielo Sorimashi, como en la imagen)
            Vector3 kp0 = new Vector3(-2.32f, 3.66f, 0f);
            Vector3 kp1 = new Vector3(-0.85f, 3.32f, 0f);
            Vector3 kp2 = new Vector3(0.85f, 3.32f, 0f);
            Vector3 kp3 = new Vector3(2.32f, 3.66f, 0f);
            AppendSplineTube(rVerts, rTris, rUvs, kp0, kp1, kp2, kp3, 0.115f, 0.125f, 0.115f, 24, 16, 0, 0f, 0.01f, 0f, 35, true);

            // Cubierta negra superior del Kasagi (Shimaki)
            AppendSplineTube(
                dVerts, dTris, dUvs,
                kp0 + new Vector3(-0.04f, 0.08f, 0f),
                kp1 + new Vector3(0f, 0.08f, 0f),
                kp2 + new Vector3(0f, 0.08f, 0f),
                kp3 + new Vector3(0.04f, 0.08f, 0f),
                0.122f, 0.132f, 0.122f, 24, 16, 0, 0f, 0.01f, 0f, 36, true);

            Mesh dMesh = new Mesh();
            dMesh.SetVertices(dVerts); dMesh.SetTriangles(dTris, 0); dMesh.SetUVs(0, dUvs);
            darkCapsMesh = SaveOrReplaceMesh(dMesh, assetName + "_DarkCaps");

            Mesh rMesh = new Mesh();
            rMesh.SetVertices(rVerts); rMesh.SetTriangles(rTris, 0); rMesh.SetUVs(0, rUvs);
            return SaveOrReplaceMesh(rMesh, assetName);
        }

        public static Mesh CreateSculptedStoneLanternMesh(string assetName, out Mesh fireBoxGlowMesh)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Perfil completo torneado hexagonal de una linterna de piedra Kasuga Tōrō (base Kiso, poste Sao, plataforma Chūdai, tejado Kasa y joya Hōju)
            float[] yP = { 0.0f, 0.16f, 0.22f, 0.28f, 0.92f, 0.98f, 1.08f, 1.10f, 1.38f, 1.40f, 1.48f, 1.66f, 1.78f };
            float[] rP = { 0.34f, 0.32f, 0.22f, 0.13f, 0.12f, 0.20f, 0.32f, 0.20f, 0.20f, 0.44f, 0.36f, 0.08f, 0.01f };

            int segs = 6; // Hexagonal auténtico japonés
            for (int p = 0; p < yP.Length; p++)
            {
                for (int s = 0; s <= segs; s++)
                {
                    float u = s / (float)segs;
                    float ang = u * Mathf.PI * 2f;
                    verts.Add(new Vector3(Mathf.Cos(ang) * rP[p], yP[p], Mathf.Sin(ang) * rP[p]));
                    uvs.Add(new Vector2(u, p / (float)(yP.Length - 1)));
                }
            }

            int stride = segs + 1;
            for (int p = 0; p < yP.Length - 1; p++)
            {
                if (p == 7) continue; // Dejar hueco de la cámara de luz (Hibukuro) para el núcleo luminoso
                for (int s = 0; s < segs; s++)
                {
                    int a = p * stride + s;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                    tris.Add(a); tris.Add(c); tris.Add(b);
                    tris.Add(a); tris.Add(d); tris.Add(c);
                }
            }

            // Pilares de esquina de la cámara de fuego (Hibukuro)
            for (int s = 0; s < segs; s++)
            {
                float ang = (s / (float)segs) * Mathf.PI * 2f;
                Vector3 dir = new Vector3(Mathf.Cos(ang) * 0.19f, 0f, Mathf.Sin(ang) * 0.19f);
                AppendSplineTube(
                    verts, tris, uvs,
                    dir + new Vector3(0f, 1.08f, 0f),
                    dir + new Vector3(0f, 1.18f, 0f),
                    dir + new Vector3(0f, 1.28f, 0f),
                    dir + new Vector3(0f, 1.40f, 0f),
                    0.022f, 0.022f, 0.022f, 4, 6, 0, 0f, 0f, 0f, s, false);
            }

            // Núcleo luminoso cálido dentro de la linterna
            List<Vector3> gVerts = new List<Vector3>();
            List<int> gTris = new List<int>();
            List<Vector2> gUvs = new List<Vector2>();
            AppendSplineTube(
                gVerts, gTris, gUvs,
                new Vector3(0f, 1.10f, 0f),
                new Vector3(0f, 1.19f, 0f),
                new Vector3(0f, 1.29f, 0f),
                new Vector3(0f, 1.38f, 0f),
                0.15f, 0.16f, 0.15f, 4, 6, 0, 0f, 0f, 0f, 1, true);

            Mesh gMesh = new Mesh();
            gMesh.SetVertices(gVerts); gMesh.SetTriangles(gTris, 0); gMesh.SetUVs(0, gUvs);
            fireBoxGlowMesh = SaveOrReplaceMesh(gMesh, assetName + "_Glow");

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateArchedTaikoBridgeMesh(string assetName, float length, float width, float archHeight, out Mesh deckMesh)
        {
            List<Vector3> rVerts = new List<Vector3>();
            List<int> rTris = new List<int>();
            List<Vector2> rUvs = new List<Vector2>();

            List<Vector3> dVerts = new List<Vector3>();
            List<int> dTris = new List<int>();
            List<Vector2> dUvs = new List<Vector2>();

            float halfL = length * 0.5f;
            float halfW = width * 0.5f;

            // Vigas arqueadas y barandillas bermellón a ambos lados
            for (int side = -1; side <= 1; side += 2)
            {
                float x = side * halfW;
                Vector3 bp0 = new Vector3(x, 0f, -halfL);
                Vector3 bp1 = new Vector3(x, archHeight * 1.25f, -halfL * 0.35f);
                Vector3 bp2 = new Vector3(x, archHeight * 1.25f, halfL * 0.35f);
                Vector3 bp3 = new Vector3(x, 0f, halfL);

                AppendSplineTube(rVerts, rTris, rUvs, bp0, bp1, bp2, bp3, 0.08f, 0.09f, 0.08f, 20, 10, 0, 0f, 0f, 0f, 10 + side, true);
                AppendSplineTube(
                    rVerts, rTris, rUvs,
                    bp0 + Vector3.up * 0.65f,
                    bp1 + Vector3.up * 0.65f,
                    bp2 + Vector3.up * 0.65f,
                    bp3 + Vector3.up * 0.65f,
                    0.055f, 0.055f, 0.055f, 20, 10, 0, 0f, 0f, 0f, 20 + side, true);

                for (int post = 0; post <= 6; post++)
                {
                    float t = post / 6f;
                    Vector3 basePt = EvalBezier(bp0, bp1, bp2, bp3, t);
                    AppendSplineTube(
                        rVerts, rTris, rUvs,
                        basePt, basePt + Vector3.up * 0.22f, basePt + Vector3.up * 0.45f, basePt + Vector3.up * 0.72f,
                        0.05f, 0.05f, 0.06f, 6, 8, 0, 0f, 0f, 0f, 30 + post, true);
                }
            }

            // Tablones de madera Hinoki del piso curvo del puente
            int plankCount = 22;
            for (int p = 0; p < plankCount; p++)
            {
                float t = p / (float)(plankCount - 1);
                float z = Mathf.Lerp(-halfL, halfL, t);
                float y = Mathf.Sin(t * Mathf.PI) * archHeight;
                AppendSplineTube(
                    dVerts, dTris, dUvs,
                    new Vector3(-halfW * 0.92f, y, z),
                    new Vector3(-halfW * 0.30f, y, z),
                    new Vector3(halfW * 0.30f, y, z),
                    new Vector3(halfW * 0.92f, y, z),
                    0.065f, 0.065f, 0.065f, 4, 8, 0, 0f, 0f, 0f, p, true);
            }

            Mesh dM = new Mesh();
            dM.SetVertices(dVerts); dM.SetTriangles(dTris, 0); dM.SetUVs(0, dUvs);
            deckMesh = SaveOrReplaceMesh(dM, assetName + "_Deck");

            Mesh rM = new Mesh();
            rM.SetVertices(rVerts); rM.SetTriangles(rTris, 0); rM.SetUVs(0, rUvs);
            return SaveOrReplaceMesh(rM, assetName);
        }

        public static Mesh CreateBambooGroveClusterMesh(string assetName, out Mesh bambooFoliageMesh)
        {
            Random.State oldState = Random.state;
            Random.InitState(707);

            List<Vector3> cVerts = new List<Vector3>();
            List<int> cTris = new List<int>();
            List<Vector2> cUvs = new List<Vector2>();

            List<Vector3> lVerts = new List<Vector3>();
            List<int> lTris = new List<int>();
            List<Vector2> lUvs = new List<Vector2>();

            // 7 Tallos de bambú altos con nudos segmentados y penachos de hojas plumosas en la parte alta
            for (int s = 0; s < 7; s++)
            {
                float ang = (s / 7f) * Mathf.PI * 2f + Random.Range(-0.2f, 0.2f);
                float dist = (s == 0) ? 0f : Random.Range(0.35f, 0.85f);
                Vector3 basePos = new Vector3(Mathf.Cos(ang) * dist, 0f, Mathf.Sin(ang) * dist);
                Vector3 lean = new Vector3(Random.Range(-0.35f, 0.35f), 0f, Random.Range(-0.35f, 0.35f));
                float h = Random.Range(5.2f, 6.8f);

                Vector3 p0 = basePos;
                Vector3 p1 = basePos + lean * 0.25f + Vector3.up * (h * 0.33f);
                Vector3 p2 = basePos + lean * 0.65f + Vector3.up * (h * 0.66f);
                Vector3 p3 = basePos + lean + Vector3.up * h;

                AppendSplineTube(cVerts, cTris, cUvs, p0, p1, p2, p3, 0.058f, 0.050f, 0.036f, 24, 10, 0, 0f, 0.08f, 0f, 100 + s, true);

                // Ramilletes de hojas de bambú a lo largo de la mitad superior de cada caña
                for (int b = 0; b < 4; b++)
                {
                    float tb = 0.48f + b * 0.16f;
                    Vector3 nodePt = EvalBezier(p0, p1, p2, p3, tb);
                    AppendBotanicalPuff(lVerts, lTris, lUvs, nodePt + new Vector3(0f, 0.12f, 0f), new Vector3(0.68f, 0.26f, 0.68f), 0.55f, 0.34f, 400 + s * 13 + b);
                }
            }

            Random.state = oldState;

            Mesh lMesh = new Mesh();
            lMesh.SetVertices(lVerts); lMesh.SetTriangles(lTris, 0); lMesh.SetUVs(0, lUvs);
            bambooFoliageMesh = SaveOrReplaceMesh(lMesh, assetName + "_Foliage");

            Mesh cMesh = new Mesh();
            cMesh.SetVertices(cVerts); cMesh.SetTriangles(cTris, 0); cMesh.SetUVs(0, cUvs);
            return SaveOrReplaceMesh(cMesh, assetName);
        }

        public static Mesh CreateDetailedMountFujiMesh(string assetName, float baseRadius, float topRadius, float height, out Mesh snowRidgesMesh)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int rings = 28;
            int radialSegs = 64;
            for (int r = 0; r <= rings; r++)
            {
                float t = r / (float)rings;
                float curveT = Mathf.Pow(1f - t, 1.48f);
                float radius = Mathf.Lerp(topRadius, baseRadius, curveT);
                float y = t * height;

                for (int s = 0; s <= radialSegs; s++)
                {
                    float u = s / (float)radialSegs;
                    float ang = u * Mathf.PI * 2f;
                    // Barrancos volcánicos radiales naturales del Monte Fuji
                    float gully = Mathf.Sin(ang * 9f + t * 2.5f) * 0.035f + Mathf.Cos(ang * 19f - t * 4f) * 0.020f;
                    float rMod = 1f + gully * (1f - t * 0.35f);
                    verts.Add(new Vector3(Mathf.Cos(ang) * radius * rMod, y, Mathf.Sin(ang) * radius * rMod));
                    uvs.Add(new Vector2(u, t));
                }
            }

            int stride = radialSegs + 1;
            for (int r = 0; r < rings; r++)
            {
                for (int s = 0; s < radialSegs; s++)
                {
                    int a = r * stride + s;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                }
            }

            // Cumbre nevada con lenguas de nieve que descienden por las laderas volcánicas (como en la imagen de presentación)
            List<Vector3> sVerts = new List<Vector3>();
            List<int> sTris = new List<int>();
            List<Vector2> sUvs = new List<Vector2>();

            int snowRings = 16;
            for (int r = 0; r <= snowRings; r++)
            {
                float st = r / (float)snowRings;
                for (int s = 0; s <= radialSegs; s++)
                {
                    float u = s / (float)radialSegs;
                    float ang = u * Mathf.PI * 2f;
                    // Lenguas irregulares de nieve entre t = 0.54 y t = 1.0
                    float snowLineBottom = 0.60f + Mathf.Sin(ang * 7f) * 0.06f + Mathf.Cos(ang * 13f) * 0.045f;
                    float t = Mathf.Lerp(snowLineBottom, 1.0f, st);
                    float curveT = Mathf.Pow(1f - t, 1.48f);
                    float radius = Mathf.Lerp(topRadius, baseRadius, curveT) + 0.06f;
                    float y = t * height + 0.03f;
                    float gully = Mathf.Sin(ang * 9f + t * 2.5f) * 0.035f + Mathf.Cos(ang * 19f - t * 4f) * 0.020f;
                    float rMod = 1f + gully * (1f - t * 0.35f);

                    sVerts.Add(new Vector3(Mathf.Cos(ang) * radius * rMod, y, Mathf.Sin(ang) * radius * rMod));
                    sUvs.Add(new Vector2(u, st));
                }
            }

            for (int r = 0; r < snowRings; r++)
            {
                for (int s = 0; s < radialSegs; s++)
                {
                    int a = r * stride + s;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    sTris.Add(a); sTris.Add(b); sTris.Add(c);
                    sTris.Add(a); sTris.Add(c); sTris.Add(d);
                }
            }

            Mesh sMesh = new Mesh();
            sMesh.SetVertices(sVerts); sMesh.SetTriangles(sTris, 0); sMesh.SetUVs(0, sUvs);
            snowRidgesMesh = SaveOrReplaceMesh(sMesh, assetName + "_SnowRidges");

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateWaterRippleRingsMesh(string assetName, float maxRadius, int ringCount)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int segs = 36;
            for (int r = 0; r < ringCount; r++)
            {
                float rad = (r + 1) * (maxRadius / ringCount);
                float thick = 0.028f * (1f - r / (float)ringCount * 0.5f);
                int baseIdx = verts.Count;

                for (int s = 0; s <= segs; s++)
                {
                    float u = s / (float)segs;
                    float ang = u * Mathf.PI * 2f;
                    Vector3 dir = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang));
                    verts.Add(dir * (rad - thick));
                    verts.Add(dir * (rad + thick));
                    uvs.Add(new Vector2(u, 0f));
                    uvs.Add(new Vector2(u, 1f));
                }

                for (int s = 0; s < segs; s++)
                {
                    int a = baseIdx + s * 2;
                    int b = a + 1;
                    int c = a + 3;
                    int d = a + 2;
                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateSculptedKoiFishMesh(string assetName)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // 1. Cuerpo fusiforme sinuoso de carpa Nishikigoi
            AppendSplineTube(
                verts, tris, uvs,
                new Vector3(0f, 0f, -0.22f),
                new Vector3(0.016f, 0.005f, -0.06f),
                new Vector3(-0.012f, 0.006f, 0.09f),
                new Vector3(0f, 0f, 0.22f),
                0.007f, 0.052f, 0.020f, 14, 12, 0, 0f, 0f, 0f, 1, true);

            // 2. Aleta caudal bifurcada en abanico (cola elegante)
            AppendSingleCurvedPetal(verts, tris, uvs, new Vector3(-0.012f, 0.004f, -0.24f), Quaternion.Euler(85f, 165f, 15f), 0.085f);
            AppendSingleCurvedPetal(verts, tris, uvs, new Vector3(0.012f, 0.004f, -0.24f), Quaternion.Euler(85f, 195f, -15f), 0.085f);

            // 3. Aletas pectorales abiertas a ambos lados y aleta dorsal
            AppendSingleCurvedPetal(verts, tris, uvs, new Vector3(-0.042f, -0.006f, 0.07f), Quaternion.Euler(12f, -115f, -10f), 0.070f);
            AppendSingleCurvedPetal(verts, tris, uvs, new Vector3(0.042f, -0.006f, 0.07f), Quaternion.Euler(12f, 115f, 10f), 0.070f);
            AppendSingleCurvedPetal(verts, tris, uvs, new Vector3(0f, 0.042f, -0.02f), Quaternion.Euler(-35f, 180f, 90f), 0.075f);

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateRakedKaresansuiGardenMesh(string assetName, float width, float depth)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int gridX = 72;
            int gridZ = 60;

            Vector2[] rockIslands =
            {
                new Vector2(-3.2f, -2.7f),
                new Vector2(-1.4f, -1.0f),
                new Vector2(1.8f, -1.8f),
                new Vector2(3.4f, 0.6f),
                new Vector2(-2.5f, 2.0f),
                new Vector2(0.4f, 2.6f),
                new Vector2(2.6f, 3.4f)
            };

            for (int iz = 0; iz <= gridZ; iz++)
            {
                float vz = iz / (float)gridZ;
                float z = (vz - 0.5f) * depth;
                for (int ix = 0; ix <= gridX; ix++)
                {
                    float vx = ix / (float)gridX;
                    float x = (vx - 0.5f) * width;

                    // Surcos longitudinales Samon + anillos concéntricos alrededor de cada isla de roca Suiseki
                    float wave = Mathf.Sin(x * 11.5f) * 0.022f;
                    for (int r = 0; r < rockIslands.Length; r++)
                    {
                        float d = Vector2.Distance(new Vector2(x, z), rockIslands[r]);
                        if (d < 2.25f)
                        {
                            float ringBlend = Mathf.SmoothStep(2.25f, 0.85f, d);
                            float ringWave = Mathf.Cos(d * 14.0f) * 0.028f;
                            wave = Mathf.Lerp(wave, ringWave, ringBlend);
                        }
                    }

                    verts.Add(new Vector3(x, wave, z));
                    uvs.Add(new Vector2(vx * 10f, vz * 10f));
                }
            }

            int stride = gridX + 1;
            for (int iz = 0; iz < gridZ; iz++)
            {
                for (int ix = 0; ix < gridX; ix++)
                {
                    int a = iz * stride + ix;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreatePanoramicSkySphereMesh(string assetName, float radius, int lonSegs = 64, int latSegs = 32)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            for (int lat = 0; lat <= latSegs; lat++)
            {
                float v = lat / (float)latSegs;
                float phi = (v - 0.5f) * Mathf.PI;
                float cosPhi = Mathf.Cos(phi);
                float sinPhi = Mathf.Sin(phi);

                for (int lon = 0; lon <= lonSegs; lon++)
                {
                    float u = lon / (float)lonSegs;
                    float theta = u * Mathf.PI * 2f;
                    float x = cosPhi * Mathf.Sin(theta) * radius;
                    float y = sinPhi * radius * 0.72f;
                    float z = cosPhi * Mathf.Cos(theta) * radius;
                    verts.Add(new Vector3(x, y, z));
                    uvs.Add(new Vector2(u, v));
                }
            }

            int stride = lonSegs + 1;
            for (int lat = 0; lat < latSegs; lat++)
            {
                for (int lon = 0; lon < lonSegs; lon++)
                {
                    int a = lat * stride + lon;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    // Normales mirando hacia el interior de la cúpula celeste
                    tris.Add(a); tris.Add(c); tris.Add(b);
                    tris.Add(a); tris.Add(d); tris.Add(c);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateFull360SanctuaryTerrainMesh(
            string assetName, float size, float baseHeight,
            Vector2 pondCenter, Vector2 pondRadii, float pondDepth, int seed)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int grid = 64;
            for (int iz = 0; iz <= grid; iz++)
            {
                float vz = iz / (float)grid;
                float z = (vz - 0.5f) * size;
                for (int ix = 0; ix <= grid; ix++)
                {
                    float vx = ix / (float)grid;
                    float x = (vx - 0.5f) * size;

                    float distFromOrigin = Mathf.Sqrt(x * x + z * z);
                    float n1 = Mathf.PerlinNoise(vx * 6.5f + seed * 1.7f, vz * 6.5f + seed * 2.3f);
                    float n2 = Mathf.PerlinNoise(vx * 16f + seed * 4.1f, vz * 16f + seed * 3.7f);

                    // Colinas suaves de musgo (Koke-yama) alrededor de los 360°, manteniendo plana la base del salón central
                    float courtyardFlatten = Mathf.SmoothStep(3.6f, 8.5f, distFromOrigin);
                    float outerHills = Mathf.SmoothStep(11f, 32f, distFromOrigin) * (1.2f + n1 * 2.4f);
                    float y = baseHeight + ( (n1 - 0.35f) * 0.42f + (n2 - 0.5f) * 0.16f ) * courtyardFlatten + outerHills;

                    // Cuenca excavada natural para el estanque o lago
                    if (pondRadii.x > 0.1f && pondRadii.y > 0.1f)
                    {
                        float dx = (x - pondCenter.x) / pondRadii.x;
                        float dz = (z - pondCenter.y) / pondRadii.y;
                        float pDist = Mathf.Sqrt(dx * dx + dz * dz);
                        if (pDist < 1.25f)
                        {
                            float basin = Mathf.SmoothStep(1.25f, 0.65f, pDist);
                            y = Mathf.Lerp(y, baseHeight - pondDepth, basin);
                        }
                    }

                    verts.Add(new Vector3(x, y, z));
                    uvs.Add(new Vector2(vx * 18f, vz * 18f));
                }
            }

            int stride = grid + 1;
            for (int iz = 0; iz < grid; iz++)
            {
                for (int ix = 0; ix < grid; ix++)
                {
                    int a = iz * stride + ix;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh Create360MountainHorizonRingMesh(
            string assetName, float innerRadius, float outerRadius, float maxPeakHeight, float frontValleyDip, int seed)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int radialSegs = 96;
            int rings = 14;

            for (int r = 0; r <= rings; r++)
            {
                float t = r / (float)rings;
                float rad = Mathf.Lerp(innerRadius, outerRadius, t);
                // Perfil de montaña: sube desde la base interior hasta la cresta (t=0.68) y baja por detrás
                float profile = Mathf.Sin(Mathf.Pow(t, 0.78f) * Mathf.PI);

                for (int s = 0; s <= radialSegs; s++)
                {
                    float u = s / (float)radialSegs;
                    float ang = u * Mathf.PI * 2f;
                    float cx = Mathf.Cos(ang) * 2.5f + seed * 3.1f;
                    float cz = Mathf.Sin(ang) * 2.5f + seed * 1.9f;

                    float n1 = Mathf.PerlinNoise(cx * 1.8f + t * 1.5f, cz * 1.8f);
                    float n2 = 1f - Mathf.Abs(Mathf.Sin(ang * 7f + n1 * 3.5f));
                    float n3 = Mathf.PerlinNoise(cx * 5.2f, cz * 5.2f + t * 3f);

                    // En el frente exacto (+Z, ang cercano a PI/2) bajamos ligeramente las colinas si queremos despejar la vista al Monte Fuji
                    float frontDist = Mathf.Abs(Mathf.DeltaAngle(ang * Mathf.Rad2Deg, 90f));
                    float valleyMask = Mathf.Lerp(1f - frontValleyDip, 1.0f, Mathf.SmoothStep(8f, 38f, frontDist));

                    float h = (0.32f + n1 * 0.42f + n2 * 0.35f + n3 * 0.15f) * maxPeakHeight * profile * valleyMask;
                    float rJitter = rad + (n1 - 0.5f) * 2.2f * profile;

                    verts.Add(new Vector3(Mathf.Cos(ang) * rJitter, -1.1f + h, Mathf.Sin(ang) * rJitter));
                    uvs.Add(new Vector2(u * 12f, t * 2f));
                }
            }

            int stride = radialSegs + 1;
            for (int r = 0; r < rings; r++)
            {
                for (int s = 0; s < radialSegs; s++)
                {
                    int a = r * stride + s;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                    tris.Add(a); tris.Add(c); tris.Add(b);
                    tris.Add(a); tris.Add(d); tris.Add(c);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateVolumetricCloudBankMesh(string assetName, float ringRadius, float baseHeight, int clusterCount, int seed)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            for (int i = 0; i < clusterCount; i++)
            {
                float ang = (i / (float)clusterCount) * Mathf.PI * 2f + Random.Range(-0.12f, 0.12f);
                float dist = ringRadius + Random.Range(-3.2f, 3.2f);
                Vector3 center = new Vector3(Mathf.Cos(ang) * dist, baseHeight + Random.Range(-0.4f, 0.9f), Mathf.Sin(ang) * dist);
                Vector3 radii = new Vector3(Random.Range(3.2f, 5.4f), Random.Range(0.42f, 0.85f), Random.Range(2.4f, 4.2f));
                AppendBotanicalPuff(verts, tris, uvs, center, radii, 0.55f, 0.18f, seed + i * 17);
            }

            Random.state = oldState;
            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateSculptedTurnedPillarMesh(
            string assetName, float radius, float height, out Mesh stonePlinthMesh, out Mesh bracketCorbelMesh)
        {
            List<Vector3> pVerts = new List<Vector3>();
            List<int> pTris = new List<int>();
            List<Vector2> pUvs = new List<Vector2>();

            // Columna cilíndrica torneada de cedro Hinoki con éntasis japonés tradicional
            AppendSplineTube(
                pVerts, pTris, pUvs,
                new Vector3(0f, 0.14f, 0f),
                new Vector3(0f, height * 0.36f, 0f),
                new Vector3(0f, height * 0.70f, 0f),
                new Vector3(0f, height - 0.12f, 0f),
                radius * 1.04f, radius, radius * 0.94f, 16, 18, 0, 0f, 0.008f, 0f, 11, true);

            // Basa de piedra tallada acampanada (Soseki / Tsuka-ishi)
            List<Vector3> sVerts = new List<Vector3>();
            List<int> sTris = new List<int>();
            List<Vector2> sUvs = new List<Vector2>();
            AppendSplineTube(
                sVerts, sTris, sUvs,
                new Vector3(0f, 0.0f, 0f),
                new Vector3(0f, 0.06f, 0f),
                new Vector3(0f, 0.11f, 0f),
                new Vector3(0f, 0.16f, 0f),
                radius * 1.55f, radius * 1.42f, radius * 1.12f, 6, 12, 0, 0f, 0.02f, 0f, 22, true);

            // Capitel de ménsulas escalonadas de carpintería tradicional (Kumimono / Masugumi)
            List<Vector3> bVerts = new List<Vector3>();
            List<int> bTris = new List<int>();
            List<Vector2> bUvs = new List<Vector2>();
            AppendSplineTube(
                bVerts, bTris, bUvs,
                new Vector3(-radius * 2.4f, height - 0.08f, 0f),
                new Vector3(-radius * 0.8f, height - 0.04f, 0f),
                new Vector3(radius * 0.8f, height - 0.04f, 0f),
                new Vector3(radius * 2.4f, height - 0.08f, 0f),
                radius * 0.65f, radius * 0.85f, radius * 0.65f, 8, 8, 0, 0f, 0f, 0f, 33, true);
            AppendSplineTube(
                bVerts, bTris, bUvs,
                new Vector3(0f, height - 0.08f, -radius * 2.4f),
                new Vector3(0f, height - 0.04f, -radius * 0.8f),
                new Vector3(0f, height - 0.04f, radius * 0.8f),
                new Vector3(0f, height - 0.08f, radius * 2.4f),
                radius * 0.65f, radius * 0.85f, radius * 0.65f, 8, 8, 0, 0f, 0f, 0f, 34, true);

            Mesh sMesh = new Mesh();
            sMesh.SetVertices(sVerts); sMesh.SetTriangles(sTris, 0); sMesh.SetUVs(0, sUvs);
            stonePlinthMesh = SaveOrReplaceMesh(sMesh, assetName + "_Plinth");

            Mesh bMesh = new Mesh();
            bMesh.SetVertices(bVerts); bMesh.SetTriangles(bTris, 0); bMesh.SetUVs(0, bUvs);
            bracketCorbelMesh = SaveOrReplaceMesh(bMesh, assetName + "_Corbel");

            Mesh pMesh = new Mesh();
            pMesh.SetVertices(pVerts); pMesh.SetTriangles(pTris, 0); pMesh.SetUVs(0, pUvs);
            return SaveOrReplaceMesh(pMesh, assetName);
        }

        public static Mesh CreateSculptedShojiScreenMesh(
            string assetName, float width, float height, int cols, int rows, out Mesh paperInsertMesh)
        {
            List<Vector3> wVerts = new List<Vector3>();
            List<int> wTris = new List<int>();
            List<Vector2> wUvs = new List<Vector2>();

            float hx = width * 0.5f;
            float frameT = 0.032f;

            // Montantes verticales exteriores (Tsukeko)
            for (int side = -1; side <= 1; side += 2)
            {
                float x = side * (hx - frameT * 0.5f);
                AppendSplineTube(
                    wVerts, wTris, wUvs,
                    new Vector3(x, 0f, 0f), new Vector3(x, height * 0.33f, 0f),
                    new Vector3(x, height * 0.66f, 0f), new Vector3(x, height, 0f),
                    frameT * 0.65f, frameT * 0.65f, frameT * 0.65f, 6, 8, 0, 0f, 0f, 0f, 10 + side, true);
            }

            // Travesaños horizontales principales (inferiores, zócalo Koshita y dintel superior)
            float[] mainRungs = { 0.02f, height * 0.22f, height - 0.02f };
            for (int r = 0; r < mainRungs.Length; r++)
            {
                float y = mainRungs[r];
                AppendSplineTube(
                    wVerts, wTris, wUvs,
                    new Vector3(-hx, y, 0f), new Vector3(-hx * 0.33f, y, 0f),
                    new Vector3(hx * 0.33f, y, 0f), new Vector3(hx, y, 0f),
                    frameT * 0.62f, frameT * 0.62f, frameT * 0.62f, 6, 8, 0, 0f, 0f, 0f, 20 + r, true);
            }

            // Celosía fina Kumiko vertical y horizontal
            for (int c = 1; c < cols; c++)
            {
                float x = Mathf.Lerp(-hx, hx, c / (float)cols);
                AppendSplineTube(
                    wVerts, wTris, wUvs,
                    new Vector3(x, height * 0.22f, 0f), new Vector3(x, height * 0.48f, 0f),
                    new Vector3(x, height * 0.74f, 0f), new Vector3(x, height - 0.02f, 0f),
                    0.008f, 0.008f, 0.008f, 4, 6, 0, 0f, 0f, 0f, 30 + c, false);
            }
            for (int r = 1; r < rows; r++)
            {
                float y = Mathf.Lerp(height * 0.22f, height - 0.02f, r / (float)rows);
                AppendSplineTube(
                    wVerts, wTris, wUvs,
                    new Vector3(-hx, y, 0f), new Vector3(-hx * 0.33f, y, 0f),
                    new Vector3(hx * 0.33f, y, 0f), new Vector3(hx, y, 0f),
                    0.007f, 0.007f, 0.007f, 4, 6, 0, 0f, 0f, 0f, 50 + r, false);
            }

            // Lámina interior de papel Washi translúcido
            List<Vector3> pVerts = new List<Vector3>
            {
                new Vector3(-hx + frameT * 0.5f, 0.02f, 0f),
                new Vector3(hx - frameT * 0.5f, 0.02f, 0f),
                new Vector3(hx - frameT * 0.5f, height - 0.02f, 0f),
                new Vector3(-hx + frameT * 0.5f, height - 0.02f, 0f)
            };
            List<Vector2> pUvs = new List<Vector2>
            {
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f)
            };
            List<int> pTris = new List<int> { 0, 2, 1, 0, 3, 2, 0, 1, 2, 0, 2, 3 };

            Mesh pM = new Mesh();
            pM.SetVertices(pVerts); pM.SetTriangles(pTris, 0); pM.SetUVs(0, pUvs);
            paperInsertMesh = SaveOrReplaceMesh(pM, assetName + "_Paper");

            Mesh wM = new Mesh();
            wM.SetVertices(wVerts); wM.SetTriangles(wTris, 0); wM.SetUVs(0, wUvs);
            return SaveOrReplaceMesh(wM, assetName);
        }

        public static Mesh CreateSculptedChigaidanaCabinetMesh(string assetName, out Mesh brassHardwareMesh)
        {
            List<Vector3> wVerts = new List<Vector3>();
            List<int> wTris = new List<int>();
            List<Vector2> wUvs = new List<Vector2>();

            List<Vector3> gVerts = new List<Vector3>();
            List<int> gTris = new List<int>();
            List<Vector2> gUvs = new List<Vector2>();

            // 6 Columnas torneadas de palisandro de la estantería escalonada japonesa Chigaidana
            Vector3[] postPos =
            {
                new Vector3(-0.18f, 0f, -0.58f), new Vector3(0.18f, 0f, -0.58f),
                new Vector3(-0.18f, 0f, 0.00f),  new Vector3(0.18f, 0f, 0.00f),
                new Vector3(-0.18f, 0f, 0.58f),  new Vector3(0.18f, 0f, 0.58f)
            };
            float[] postHeights = { 0.86f, 0.86f, 0.86f, 0.86f, 0.62f, 0.62f };
            for (int i = 0; i < postPos.Length; i++)
            {
                Vector3 p = postPos[i];
                float h = postHeights[i];
                AppendSplineTube(wVerts, wTris, wUvs, p, p + Vector3.up * (h * 0.35f), p + Vector3.up * (h * 0.7f), p + Vector3.up * h,
                    0.022f, 0.019f, 0.018f, 8, 8, 0, 0f, 0f, 0f, 10 + i, true);
            }

            // Baldas escalonadas asimétricas (Chigai-dana) con bordes moldurados
            float[] shelfY = { 0.08f, 0.36f, 0.60f, 0.84f };
            float[] shelfZMin = { -0.62f, -0.62f, -0.12f, -0.62f };
            float[] shelfZMax = { 0.62f, 0.18f, 0.62f, 0.12f };
            for (int s = 0; s < shelfY.Length; s++)
            {
                for (int r = -2; r <= 2; r++)
                {
                    float x = r * 0.075f;
                    AppendSplineTube(
                        wVerts, wTris, wUvs,
                        new Vector3(x, shelfY[s], shelfZMin[s]),
                        new Vector3(x, shelfY[s], Mathf.Lerp(shelfZMin[s], shelfZMax[s], 0.33f)),
                        new Vector3(x, shelfY[s], Mathf.Lerp(shelfZMin[s], shelfZMax[s], 0.66f)),
                        new Vector3(x, shelfY[s], shelfZMax[s]),
                        0.042f, 0.042f, 0.042f, 6, 6, 0, 0f, 0f, 0f, 30 + s * 5 + r, true);
                }
            }

            // Herrajes circulares de latón (Hikite) y jarrón cerámico decorativo sobre la balda
            AppendSplineTube(
                gVerts, gTris, gUvs,
                new Vector3(0.02f, 0.38f, 0.34f),
                new Vector3(0.02f, 0.45f, 0.34f),
                new Vector3(0.02f, 0.52f, 0.34f),
                new Vector3(0.02f, 0.57f, 0.34f),
                0.045f, 0.072f, 0.032f, 10, 14, 0, 0f, 0f, 0f, 77, true);

            Mesh gM = new Mesh();
            gM.SetVertices(gVerts); gM.SetTriangles(gTris, 0); gM.SetUVs(0, gUvs);
            brassHardwareMesh = SaveOrReplaceMesh(gM, assetName + "_Brass");

            Mesh wM = new Mesh();
            wM.SetVertices(wVerts); wM.SetTriangles(wTris, 0); wM.SetUVs(0, wUvs);
            return SaveOrReplaceMesh(wM, assetName);
        }

        public static Mesh CreateSculptedAndonLampMesh(string assetName, out Mesh glowingWashiMesh)
        {
            List<Vector3> fVerts = new List<Vector3>();
            List<int> fTris = new List<int>();
            List<Vector2> fUvs = new List<Vector2>();

            float r = 0.135f;
            float h = 0.44f;

            // 4 Columnillas de esquina de madera lacada + anillos de celosía Kumiko
            for (int c = 0; c < 4; c++)
            {
                float ang = (c * 90f + 45f) * Mathf.Deg2Rad;
                Vector3 corner = new Vector3(Mathf.Cos(ang) * r, 0f, Mathf.Sin(ang) * r);
                AppendSplineTube(
                    fVerts, fTris, fUvs,
                    corner, corner + Vector3.up * (h * 0.33f), corner + Vector3.up * (h * 0.66f), corner + Vector3.up * h,
                    0.012f, 0.011f, 0.011f, 6, 6, 0, 0f, 0f, 0f, 10 + c, true);
            }

            float[] ringHeights = { 0.04f, 0.16f, 0.28f, 0.40f };
            for (int rh = 0; rh < ringHeights.Length; rh++)
            {
                float y = ringHeights[rh];
                for (int c = 0; c < 4; c++)
                {
                    float a0 = (c * 90f + 45f) * Mathf.Deg2Rad;
                    float a1 = ((c + 1) * 90f + 45f) * Mathf.Deg2Rad;
                    Vector3 p0 = new Vector3(Mathf.Cos(a0) * r, y, Mathf.Sin(a0) * r);
                    Vector3 p3 = new Vector3(Mathf.Cos(a1) * r, y, Mathf.Sin(a1) * r);
                    AppendSplineTube(fVerts, fTris, fUvs, p0, Vector3.Lerp(p0, p3, 0.33f), Vector3.Lerp(p0, p3, 0.66f), p3,
                        0.009f, 0.009f, 0.009f, 4, 6, 0, 0f, 0f, 0f, 30 + rh * 4 + c, false);
                }
            }

            // Cilindro interior luminoso de papel Washi cálido
            List<Vector3> gVerts = new List<Vector3>();
            List<int> gTris = new List<int>();
            List<Vector2> gUvs = new List<Vector2>();
            AppendSplineTube(
                gVerts, gTris, gUvs,
                new Vector3(0f, 0.05f, 0f), new Vector3(0f, 0.16f, 0f),
                new Vector3(0f, 0.28f, 0f), new Vector3(0f, 0.39f, 0f),
                0.105f, 0.112f, 0.105f, 8, 16, 0, 0f, 0f, 0f, 99, true);

            Mesh gM = new Mesh();
            gM.SetVertices(gVerts); gM.SetTriangles(gTris, 0); gM.SetUVs(0, gUvs);
            glowingWashiMesh = SaveOrReplaceMesh(gM, assetName + "_Glow");

            Mesh fM = new Mesh();
            fM.SetVertices(fVerts); fM.SetTriangles(fTris, 0); fM.SetUVs(0, fUvs);
            return SaveOrReplaceMesh(fM, assetName);
        }

        public static Mesh CreateSculptedHangingChochinLanternMesh(string assetName, out Mesh silkGlowMesh)
        {
            List<Vector3> fVerts = new List<Vector3>();
            List<int> fTris = new List<int>();
            List<Vector2> fUvs = new List<Vector2>();

            // Cadena superior, casquillos lacados (Jūbako) y borla colgante inferior
            AppendSplineTube(
                fVerts, fTris, fUvs,
                new Vector3(0f, 0.0f, 0f), new Vector3(0f, -0.12f, 0f),
                new Vector3(0f, -0.24f, 0f), new Vector3(0f, -0.34f, 0f),
                0.010f, 0.010f, 0.012f, 6, 8, 0, 0f, 0f, 0f, 1, true);
            AppendSplineTube(
                fVerts, fTris, fUvs,
                new Vector3(0f, -0.32f, 0f), new Vector3(0f, -0.35f, 0f),
                new Vector3(0f, -0.38f, 0f), new Vector3(0f, -0.40f, 0f),
                0.11f, 0.14f, 0.13f, 4, 14, 0, 0f, 0f, 0f, 2, true);
            AppendSplineTube(
                fVerts, fTris, fUvs,
                new Vector3(0f, -0.78f, 0f), new Vector3(0f, -0.81f, 0f),
                new Vector3(0f, -0.88f, 0f), new Vector3(0f, -0.98f, 0f),
                0.12f, 0.09f, 0.015f, 6, 14, 0, 0f, 0f, 0f, 3, true);

            // Globo ovalado acanalado de seda luminosa (Chōchin)
            List<Vector3> gVerts = new List<Vector3>();
            List<int> gTris = new List<int>();
            List<Vector2> gUvs = new List<Vector2>();
            AppendSplineTube(
                gVerts, gTris, gUvs,
                new Vector3(0f, -0.38f, 0f), new Vector3(0f, -0.50f, 0f),
                new Vector3(0f, -0.66f, 0f), new Vector3(0f, -0.80f, 0f),
                0.13f, 0.22f, 0.12f, 14, 18, 0, 0f, 0.03f, 0f, 4, true);

            Mesh gM = new Mesh();
            gM.SetVertices(gVerts); gM.SetTriangles(gTris, 0); gM.SetUVs(0, gUvs);
            silkGlowMesh = SaveOrReplaceMesh(gM, assetName + "_Glow");

            Mesh fM = new Mesh();
            fM.SetVertices(fVerts); fM.SetTriangles(fTris, 0); fM.SetUVs(0, fUvs);
            return SaveOrReplaceMesh(fM, assetName);
        }

        public static Mesh CreateSculptedToroNagashiMesh(string assetName, out Mesh paperGlowMesh)
        {
            List<Vector3> wVerts = new List<Vector3>();
            List<int> wTris = new List<int>();
            List<Vector2> wUvs = new List<Vector2>();

            // Base flotante de madera de cedro con proa y popa curvadas + 4 finos montantes y tejadillo
            AppendSplineTube(
                wVerts, wTris, wUvs,
                new Vector3(0f, 0.02f, -0.19f), new Vector3(0f, 0.0f, -0.06f),
                new Vector3(0f, 0.0f, 0.06f), new Vector3(0f, 0.02f, 0.19f),
                0.12f, 0.16f, 0.12f, 8, 8, 0, 0f, 0f, 0f, 1, true);

            for (int c = 0; c < 4; c++)
            {
                float ang = (c * 90f + 45f) * Mathf.Deg2Rad;
                Vector3 pt = new Vector3(Mathf.Cos(ang) * 0.105f, 0.01f, Mathf.Sin(ang) * 0.105f);
                AppendSplineTube(wVerts, wTris, wUvs, pt, pt + Vector3.up * 0.10f, pt + Vector3.up * 0.20f, pt + Vector3.up * 0.30f,
                    0.009f, 0.008f, 0.008f, 4, 6, 0, 0f, 0f, 0f, 10 + c, true);
            }

            List<Vector3> gVerts = new List<Vector3>();
            List<int> gTris = new List<int>();
            List<Vector2> gUvs = new List<Vector2>();
            AppendSplineTube(
                gVerts, gTris, gUvs,
                new Vector3(0f, 0.03f, 0f), new Vector3(0f, 0.11f, 0f),
                new Vector3(0f, 0.20f, 0f), new Vector3(0f, 0.28f, 0f),
                0.082f, 0.092f, 0.082f, 8, 12, 0, 0f, 0f, 0f, 5, true);

            Mesh gM = new Mesh();
            gM.SetVertices(gVerts); gM.SetTriangles(gTris, 0); gM.SetUVs(0, gUvs);
            paperGlowMesh = SaveOrReplaceMesh(gM, assetName + "_Glow");

            Mesh wM = new Mesh();
            wM.SetVertices(wVerts); wM.SetTriangles(wTris, 0); wM.SetUVs(0, wUvs);
            return SaveOrReplaceMesh(wM, assetName);
        }

        public static Mesh CreateSculptedSorinSpireMesh(string assetName, float height)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Aguja budista imperial (Sōrin) con base Roban, cuenco Fukubachi, 9 anillos sagrados Kurin y joya Hōju
            int steps = 36;
            int segs = 16;
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                float y = t * height;
                float r = Mathf.Lerp(0.14f, 0.025f, t);

                if (t < 0.14f) r = 0.24f * (1f - t * 2f); // Roban & Fukubachi
                else if (t >= 0.18f && t <= 0.74f) // 9 Anillos Kurin
                {
                    float ringWave = Mathf.Sin(((t - 0.18f) / 0.56f) * Mathf.PI * 18f);
                    r += Mathf.Max(0f, ringWave) * 0.085f * (1f - t * 0.35f);
                }
                else if (t > 0.86f) // Joya sagrada Hōju en la cúspide
                {
                    r = Mathf.Sin(((1f - t) / 0.14f) * Mathf.PI) * 0.095f + 0.005f;
                }

                for (int s = 0; s <= segs; s++)
                {
                    float u = s / (float)segs;
                    float ang = u * Mathf.PI * 2f;
                    verts.Add(new Vector3(Mathf.Cos(ang) * r, y, Mathf.Sin(ang) * r));
                    uvs.Add(new Vector2(u, t));
                }
            }

            int stride = segs + 1;
            for (int i = 0; i < steps; i++)
            {
                for (int s = 0; s < segs; s++)
                {
                    int a = i * stride + s;
                    int b = a + stride;
                    int c = b + 1;
                    int d = a + 1;
                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(a); tris.Add(c); tris.Add(d);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(verts); mesh.SetTriangles(tris, 0); mesh.SetUVs(0, uvs);
            return SaveOrReplaceMesh(mesh, assetName);
        }

        public static Mesh CreateSculptedKoranBalustradeMesh(
            string assetName, float length, float height, int postCount, out Mesh giboshiFinialsMesh)
        {
            List<Vector3> rVerts = new List<Vector3>();
            List<int> rTris = new List<int>();
            List<Vector2> rUvs = new List<Vector2>();

            List<Vector3> gVerts = new List<Vector3>();
            List<int> gTris = new List<int>();
            List<Vector2> gUvs = new List<Vector2>();

            float hx = length * 0.5f;

            // Barandillas horizontales (Hōtate superior con extremos curvados Warabi-te, Hirageta central y Jifuku inferior)
            float[] railY = { 0.12f, height * 0.56f, height };
            float[] railR = { 0.032f, 0.026f, 0.040f };
            for (int i = 0; i < railY.Length; i++)
            {
                float lift = (i == 2) ? 0.07f : 0f;
                AppendSplineTube(
                    rVerts, rTris, rUvs,
                    new Vector3(-hx - 0.10f, railY[i] + lift, 0f),
                    new Vector3(-hx * 0.45f, railY[i], 0f),
                    new Vector3(hx * 0.45f, railY[i], 0f),
                    new Vector3(hx + 0.10f, railY[i] + lift, 0f),
                    railR[i], railR[i], railR[i], 14, 10, 0, 0f, 0f, 0f, 10 + i, true);
            }

            // Postes verticales con remates de bronce en forma de bulbo de loto (Giboshi)
            for (int p = 0; p < postCount; p++)
            {
                float x = Mathf.Lerp(-hx, hx, p / (float)(postCount - 1));
                AppendSplineTube(
                    rVerts, rTris, rUvs,
                    new Vector3(x, 0f, 0f), new Vector3(x, height * 0.35f, 0f),
                    new Vector3(x, height * 0.70f, 0f), new Vector3(x, height + 0.04f, 0f),
                    0.042f, 0.038f, 0.040f, 6, 10, 0, 0f, 0f, 0f, 30 + p, true);

                AppendSplineTube(
                    gVerts, gTris, gUvs,
                    new Vector3(x, height + 0.03f, 0f),
                    new Vector3(x, height + 0.09f, 0f),
                    new Vector3(x, height + 0.15f, 0f),
                    new Vector3(x, height + 0.20f, 0f),
                    0.048f, 0.064f, 0.010f, 8, 12, 0, 0f, 0f, 0f, 60 + p, true);
            }

            Mesh gM = new Mesh();
            gM.SetVertices(gVerts); gM.SetTriangles(gTris, 0); gM.SetUVs(0, gUvs);
            giboshiFinialsMesh = SaveOrReplaceMesh(gM, assetName + "_Giboshi");

            Mesh rM = new Mesh();
            rM.SetVertices(rVerts); rM.SetTriangles(rTris, 0); rM.SetUVs(0, rUvs);
            return SaveOrReplaceMesh(rM, assetName);
        }

        public static Mesh CreateSculptedLotusPadAndFlowerMesh(
            string assetName, int padCount, int seed, out Mesh lotusFlowerMesh)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            List<Vector3> pVerts = new List<Vector3>();
            List<int> pTris = new List<int>();
            List<Vector2> pUvs = new List<Vector2>();

            List<Vector3> fVerts = new List<Vector3>();
            List<int> fTris = new List<int>();
            List<Vector2> fUvs = new List<Vector2>();
            List<Vector3> sVerts = new List<Vector3>();
            List<int> sTris = new List<int>();
            List<Vector2> sUvs = new List<Vector2>();

            for (int i = 0; i < padCount; i++)
            {
                float ang = (i / (float)padCount) * Mathf.PI * 2f + Random.Range(-0.35f, 0.35f);
                float dist = Random.Range(0.45f, 2.35f);
                Vector3 center = new Vector3(Mathf.Cos(ang) * dist, 0.008f + (i % 3) * 0.003f, Mathf.Sin(ang) * dist);
                float rad = Random.Range(0.22f, 0.36f);

                // Hoja circular de loto con muesca radial y borde ligeramente ondulado
                int baseIdx = pVerts.Count;
                pVerts.Add(center);
                pUvs.Add(new Vector2(0.5f, 0.5f));
                int segs = 20;
                for (int s = 0; s <= segs; s++)
                {
                    float u = s / (float)segs;
                    float a = u * Mathf.PI * 1.92f + i * 1.1f;
                    float rMod = rad * (1f + Mathf.Sin(u * Mathf.PI * 12f) * 0.05f);
                    pVerts.Add(center + new Vector3(Mathf.Cos(a) * rMod, Mathf.Sin(u * Mathf.PI * 6f) * 0.008f, Mathf.Sin(a) * rMod));
                    pUvs.Add(new Vector2(Mathf.Cos(a) * 0.5f + 0.5f, Mathf.Sin(a) * 0.5f + 0.5f));
                }
                for (int s = 0; s < segs; s++)
                {
                    pTris.Add(baseIdx); pTris.Add(baseIdx + s + 2); pTris.Add(baseIdx + s + 1);
                    pTris.Add(baseIdx); pTris.Add(baseIdx + s + 1); pTris.Add(baseIdx + s + 2);
                }

                // Flor de loto abierta de doble corona sobre la mitad de las hojas
                if (i % 2 == 0)
                {
                    Vector3 fPos = center + new Vector3(0.05f, 0.025f, 0.04f);
                    AppendFivePetalSakuraFlower(fVerts, fTris, fUvs, sVerts, sTris, sUvs, fPos, Quaternion.identity, 0.14f);
                    AppendFivePetalSakuraFlower(fVerts, fTris, fUvs, sVerts, sTris, sUvs, fPos + new Vector3(0f, 0.018f, 0f), Quaternion.Euler(0f, 36f, 0f), 0.10f);
                }
            }

            Random.state = oldState;

            Mesh fM = new Mesh();
            fM.SetVertices(fVerts); fM.SetTriangles(fTris, 0); fM.SetUVs(0, fUvs);
            lotusFlowerMesh = SaveOrReplaceMesh(fM, assetName + "_Flowers");

            Mesh pM = new Mesh();
            pM.SetVertices(pVerts); pM.SetTriangles(pTris, 0); pM.SetUVs(0, pUvs);
            return SaveOrReplaceMesh(pM, assetName);
        }

        public static Mesh CreateSculptedZenStatueMesh(string assetName, out Mesh lotusPedestalMesh)
        {
            List<Vector3> sVerts = new List<Vector3>();
            List<int> sTris = new List<int>();
            List<Vector2> sUvs = new List<Vector2>();

            // Estatua tallada en piedra de Buda / Jizō meditando en postura de loto (Zazen) + nimbo sagrado Kōhai detrás
            AppendSplineTube(
                sVerts, sTris, sUvs,
                new Vector3(0f, 0.16f, 0f), new Vector3(0f, 0.28f, 0f),
                new Vector3(0f, 0.44f, 0f), new Vector3(0f, 0.56f, 0f),
                0.22f, 0.16f, 0.08f, 12, 16, 0, 0f, 0.015f, 0f, 10, true);
            // Cabeza serena y Ushnisha
            AppendSplineTube(
                sVerts, sTris, sUvs,
                new Vector3(0f, 0.54f, 0f), new Vector3(0f, 0.62f, 0f),
                new Vector3(0f, 0.70f, 0f), new Vector3(0f, 0.76f, 0f),
                0.075f, 0.095f, 0.045f, 10, 14, 0, 0f, 0.008f, 0f, 11, true);
            // Nimbo / Halo flamígero tallado en piedra (Kōhai) detrás de la estatua
            AppendSingleCurvedPetal(sVerts, sTris, sUvs, new Vector3(0f, 0.18f, 0.10f), Quaternion.Euler(-82f, 0f, 0f), 0.68f);

            // Pedestal de loto tallado (Rengeza)
            List<Vector3> pVerts = new List<Vector3>();
            List<int> pTris = new List<int>();
            List<Vector2> pUvs = new List<Vector2>();
            AppendSplineTube(
                pVerts, pTris, pUvs,
                new Vector3(0f, 0.0f, 0f), new Vector3(0f, 0.06f, 0f),
                new Vector3(0f, 0.12f, 0f), new Vector3(0f, 0.18f, 0f),
                0.30f, 0.24f, 0.28f, 8, 16, 0, 0f, 0.025f, 0f, 20, true);

            Mesh pM = new Mesh();
            pM.SetVertices(pVerts); pM.SetTriangles(pTris, 0); pM.SetUVs(0, pUvs);
            lotusPedestalMesh = SaveOrReplaceMesh(pM, assetName + "_Pedestal");

            Mesh sM = new Mesh();
            sM.SetVertices(sVerts); sM.SetTriangles(sTris, 0); sM.SetUVs(0, sUvs);
            return SaveOrReplaceMesh(sM, assetName);
        }
    }
}
#endif
