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
            // PALETA DE MATERIALES PBR DE ALTA DEFINICIÓN CON TEXTURAS DE CORTEZA REALISTA
            // ==========================================
            Material tableMat = CreateOrGetPBRMaterial("Zen_TableMat", new Color(0.08f, 0.055f, 0.045f), 0.88f, 0.05f); // Lacado Urushi negro-caoba
            Material tableApronMat = CreateOrGetPBRMaterial("Zen_TableApronMat", new Color(0.14f, 0.09f, 0.07f), 0.72f, 0.02f); // Palisandro Shitan tallado
            Material potMat = CreateOrGetPBRMaterial("Zen_PotMat", new Color(0.11f, 0.17f, 0.21f), 0.90f, 0.10f);   // Cerámica Tokoname esmaltada azul-acero
            Material soilMat = CreateOrGetPBRMaterial("Zen_SoilMat", new Color(0.14f, 0.10f, 0.07f), 0.10f, 0.0f);   // Sustrato Akadema / Kurotsuchi
            Material mossDeepMat = CreateOrGetPBRMaterial("Zen_MossDeepMat", new Color(0.13f, 0.42f, 0.11f), 0.14f, 0.0f); // Musgo Kyoto verde profundo
            Material mossLightMat = CreateOrGetPBRMaterial("Zen_MossLightMat", new Color(0.28f, 0.62f, 0.16f), 0.18f, 0.0f); // Musgo aterciopelado esmeralda
            Material stoneMat = CreateOrGetPBRMaterial("Zen_StoneMat", new Color(0.22f, 0.24f, 0.27f), 0.55f, 0.08f); // Roca Suiseki mineral
            Material waterMat = CreateOrGetPBRMaterial("Zen_WaterMat", new Color(0.18f, 0.72f, 0.90f, 0.85f), 0.96f, 0.05f);
            Material buttonMat = CreateOrGetPBRMaterial("Zen_ButtonMat", new Color(0.20f, 0.26f, 0.32f), 0.74f, 0.05f);
            Material infoButtonMat = CreateOrGetPBRMaterial("Zen_InfoButtonMat", new Color(0.18f, 0.44f, 0.52f), 0.78f, 0.08f);
            Material goldMat = CreateOrGetPBRMaterial("Zen_GoldMat", new Color(0.90f, 0.74f, 0.26f), 0.85f, 0.65f);

            // Materiales botánicos de diseñador con textura procedural de corteza orgánica para cada especie
            Material sakuraBarkMat = CreateOrGetBarkMaterial("Zen_SakuraBarkMat",
                new Color(0.32f, 0.17f, 0.13f), new Color(0.16f, 0.08f, 0.06f), new Color(0.52f, 0.34f, 0.25f), 0.34f, 1); // Cerezo con lenticelas horizontales
            Material sakuraPetalPinkMat = CreateOrGetPBRMaterial("Zen_SakuraPetalPinkMat", new Color(1.0f, 0.72f, 0.83f), 0.45f, 0.0f); // Pétalo rosa Sakura
            Material sakuraPetalWhiteMat = CreateOrGetPBRMaterial("Zen_SakuraPetalWhiteMat", new Color(0.99f, 0.94f, 0.96f), 0.48f, 0.0f); // Blanco nacarado Somei-Yoshino
            Material sakuraLeafShootMat = CreateOrGetPBRMaterial("Zen_SakuraLeafShootMat", new Color(0.24f, 0.58f, 0.26f), 0.28f, 0.0f); // Follaje primaveral

            Material matsuBarkMat = CreateOrGetBarkMaterial("Zen_MatsuBarkMat",
                new Color(0.22f, 0.15f, 0.11f), new Color(0.08f, 0.05f, 0.04f), new Color(0.38f, 0.28f, 0.21f), 0.16f, 2); // Pino Kuromatsu con placas escamosas
            Material matsuNeedleDeepMat = CreateOrGetPBRMaterial("Zen_MatsuNeedleDeepMat", new Color(0.08f, 0.36f, 0.14f), 0.25f, 0.0f); // Acícula pino imperial
            Material matsuNeedleTipMat = CreateOrGetPBRMaterial("Zen_MatsuNeedleTipMat", new Color(0.16f, 0.54f, 0.20f), 0.30f, 0.0f); // Brotes aciculares Midori
            Material jinWoodMat = CreateOrGetBarkMaterial("Zen_JinWoodMat",
                new Color(0.92f, 0.89f, 0.81f), new Color(0.68f, 0.63f, 0.54f), new Color(0.98f, 0.96f, 0.90f), 0.14f, 3); // Madera muerta Jin/Shari blanqueada con veta fibrosa

            Material momijiBarkMat = CreateOrGetBarkMaterial("Zen_MomijiBarkMat",
                new Color(0.42f, 0.32f, 0.25f), new Color(0.22f, 0.16f, 0.12f), new Color(0.56f, 0.46f, 0.38f), 0.30f, 4); // Corteza gris-canela estriada de arce japonés
            Material momijiRubyMat = CreateOrGetPBRMaterial("Zen_MomijiRubyMat", new Color(0.75f, 0.11f, 0.11f), 0.38f, 0.0f); // Rojo rubí profundo
            Material momijiScarletMat = CreateOrGetPBRMaterial("Zen_MomijiScarletMat", new Color(0.92f, 0.18f, 0.11f), 0.40f, 0.0f); // Escarlata encendido Deshojo
            Material momijiAmberMat = CreateOrGetPBRMaterial("Zen_MomijiAmberMat", new Color(0.98f, 0.52f, 0.08f), 0.42f, 0.0f); // Ámbar dorado otoñal

            // Materiales de herramientas artesanas
            Material copperMat = CreateOrGetPBRMaterial("Zen_CopperMat", new Color(0.85f, 0.49f, 0.31f), 0.84f, 0.88f);
            Material steelMat = CreateOrGetPBRMaterial("Zen_SteelMat", new Color(0.16f, 0.17f, 0.19f), 0.72f, 0.82f);
            Material bambooMat = CreateOrGetPBRMaterial("Zen_BambooMat", new Color(0.80f, 0.70f, 0.46f), 0.52f, 0.0f);
            Material bristleMat = CreateOrGetPBRMaterial("Zen_BristleMat", new Color(0.46f, 0.35f, 0.24f), 0.16f, 0.0f);
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
            BuildToolTray(root.transform, trayWoodMat, tableMat, steelMat, copperMat, bambooMat, bristleMat, goldMat, potMat, mossLightMat, stoneMat, sakuraPetalPinkMat);

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

            AssetDatabase.SaveAssets();
            Selection.activeGameObject = root;
            Undo.RegisterCreatedObjectUndo(root, "Setup Zen Bonsai Experience HD Masterpiece v21");
            Debug.Log("<color=green>[Zen Bonsai]</color> ¡Experiencia Bonsai HD Masterpiece v21 montada con éxito!");
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
            vDivider.transform.localPosition = new Vector3(0.052f, 0.006f, -0.009f);
            vDivider.transform.localScale = new Vector3(0.0035f, 0.064f, 0.005f);
            vDivider.GetComponent<Renderer>().sharedMaterial = goldMat;
            var vdc = vDivider.GetComponent<Collider>(); if (vdc != null) DestroyImmediate(vdc);

            // Canvas de estado integrado en la consola frontal
            GameObject menuCanvasObj = new GameObject("Console_Canvas");
            menuCanvasObj.transform.SetParent(panelContainer.transform, false);
            menuCanvasObj.transform.localPosition = new Vector3(0f, 0f, -0.010f);
            menuCanvasObj.transform.localScale = Vector3.one * 0.001f;

            Canvas mCanvas = menuCanvasObj.AddComponent<Canvas>();
            mCanvas.renderMode = RenderMode.WorldSpace;
            menuCanvasObj.AddComponent<CanvasScaler>();

            Font uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (uiFont == null) uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            // Estado en vivo del bonsái en la franja inferior de la consola de mesa
            GameObject mStatusObj = new GameObject("TreeStatusText");
            mStatusObj.transform.SetParent(menuCanvasObj.transform, false);
            mStatusObj.transform.localPosition = new Vector3(0f, -35f, 0f);
            Text mStatus = mStatusObj.AddComponent<Text>();
            mStatus.font = uiFont;
            mStatus.alignment = TextAnchor.MiddleCenter;
            mStatus.fontSize = 12;
            mStatus.fontStyle = FontStyle.Bold;
            mStatus.text = "🌿 Sakura · Armonía 60% · Hidratación 86%";
            mStatus.color = new Color(0.86f, 0.98f, 0.86f);
            mStatusObj.GetComponent<RectTransform>().sizeDelta = new Vector2(480, 22);
            spatialMenu.treeStatusText = mStatus;

            // =========================================================================
            // FILA FRONTAL EN LA MESA: 3 BOTONES DE ÁRBOLES + BOTÓN INFO + BOTÓN OPCIONES
            // =========================================================================
            spatialMenu.btnSakura = Create3DTouchButton(panelContainer.transform, "Btn_Sakura", "🌸\nSakura",
                new Vector3(-0.194f, 0.010f, -0.014f), new Vector3(0.086f, 0.052f, 0.018f), sakuraMat, new Color(0.18f, 0.06f, 0.10f), 12);

            spatialMenu.btnMatsu = Create3DTouchButton(panelContainer.transform, "Btn_Matsu", "🌲\nMatsu",
                new Vector3(-0.098f, 0.010f, -0.014f), new Vector3(0.086f, 0.052f, 0.018f), matsuMat, Color.white, 12);

            spatialMenu.btnMomiji = Create3DTouchButton(panelContainer.transform, "Btn_Momiji", "🍁\nMomiji",
                new Vector3(-0.002f, 0.010f, -0.014f), new Vector3(0.086f, 0.052f, 0.018f), momijiMat, Color.white, 12);

            spatialMenu.btnInfo = Create3DTouchButton(panelContainer.transform, "Btn_Info", "📖 Info y\nCuidados",
                new Vector3(0.104f, 0.010f, -0.014f), new Vector3(0.088f, 0.052f, 0.018f), infoButtonMat, new Color(0.92f, 0.99f, 1.0f), 11);

            spatialMenu.btnOptions = Create3DTouchButton(panelContainer.transform, "Btn_Options", "⚙️ Menú\nOpciones",
                new Vector3(0.200f, 0.010f, -0.014f), new Vector3(0.088f, 0.052f, 0.018f), buttonMat, new Color(1.0f, 0.90f, 0.58f), 11);

            // =========================================================================
            // 2. FICHA BOTÁNICA DESPLEGABLE (INFO PANEL) FRENTE AL JUGADOR AL PULSAR "INFO"
            // Se abre frente al jugador (ligeramente a la izquierda para ver el árbol a la vez, sin girar el cuello)
            // =========================================================================
            GameObject infoCard = new GameObject("InfoPanel_BotanicalCard");
            infoCard.transform.SetParent(menuRoot.transform, false);
            infoCard.transform.localPosition = new Vector3(-0.22f, 0.16f, -0.14f);
            infoCard.transform.localRotation = Quaternion.Euler(12f, -14f, 0f);
            spatialMenu.infoPanelBox = infoCard;

            GameObject infoOuter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            infoOuter.name = "InfoCard_OuterFrame";
            infoOuter.transform.SetParent(infoCard.transform, false);
            infoOuter.transform.localPosition = Vector3.zero;
            infoOuter.transform.localScale = new Vector3(0.42f, 0.29f, 0.012f);
            infoOuter.GetComponent<Renderer>().sharedMaterial = urushiMat;
            var icCol = infoOuter.GetComponent<Collider>(); if (icCol != null) DestroyImmediate(icCol);

            GameObject infoInner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            infoInner.name = "InfoCard_InnerBoard";
            infoInner.transform.SetParent(infoCard.transform, false);
            infoInner.transform.localPosition = new Vector3(0f, 0f, -0.003f);
            infoInner.transform.localScale = new Vector3(0.398f, 0.268f, 0.009f);
            infoInner.GetComponent<Renderer>().sharedMaterial = rosewoodMat;
            var iiCol = infoInner.GetComponent<Collider>(); if (iiCol != null) DestroyImmediate(iiCol);

            GameObject infoGoldRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            infoGoldRim.name = "InfoCard_GoldBorder";
            infoGoldRim.transform.SetParent(infoCard.transform, false);
            infoGoldRim.transform.localPosition = new Vector3(0f, 0.130f, -0.008f);
            infoGoldRim.transform.localScale = new Vector3(0.385f, 0.006f, 0.005f);
            infoGoldRim.GetComponent<Renderer>().sharedMaterial = goldMat;
            var igCol = infoGoldRim.GetComponent<Collider>(); if (igCol != null) DestroyImmediate(igCol);

            GameObject infoCanvasObj = new GameObject("InfoCard_Canvas");
            infoCanvasObj.transform.SetParent(infoCard.transform, false);
            infoCanvasObj.transform.localPosition = new Vector3(0f, 0f, -0.009f);
            infoCanvasObj.transform.localScale = Vector3.one * 0.001f;
            Canvas iCanvas = infoCanvasObj.AddComponent<Canvas>();
            iCanvas.renderMode = RenderMode.WorldSpace;

            GameObject iTitleObj = new GameObject("InfoTitle");
            iTitleObj.transform.SetParent(infoCanvasObj.transform, false);
            iTitleObj.transform.localPosition = new Vector3(0f, 96f, 0f);
            Text iTitle = iTitleObj.AddComponent<Text>();
            iTitle.font = uiFont;
            iTitle.alignment = TextAnchor.MiddleCenter;
            iTitle.fontSize = 15;
            iTitle.fontStyle = FontStyle.Bold;
            iTitle.color = new Color(1.0f, 0.78f, 0.88f);
            iTitleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(365, 28);
            spatialMenu.infoPanelTitle = iTitle;

            GameObject iBodyObj = new GameObject("InfoBody");
            iBodyObj.transform.SetParent(infoCanvasObj.transform, false);
            iBodyObj.transform.localPosition = new Vector3(0f, -12f, 0f);
            Text iBody = iBodyObj.AddComponent<Text>();
            iBody.font = uiFont;
            iBody.alignment = TextAnchor.MiddleLeft;
            iBody.fontSize = 11;
            iBody.lineSpacing = 1.14f;
            iBody.color = new Color(0.95f, 0.93f, 0.88f);
            iBodyObj.GetComponent<RectTransform>().sizeDelta = new Vector2(360, 185);
            spatialMenu.infoPanelBody = iBody;

            infoCard.SetActive(false);

            // =========================================================================
            // 3. PANEL DE OPCIONES DEL SANTUARIO FRENTE AL JUGADOR AL PULSAR "OPCIONES"
            // Posicionado ergonómicamente delante del jugador sentado (sin tener que girarse a la izquierda)
            // =========================================================================
            GameObject optionsCard = new GameObject("OptionsPanel_Card");
            optionsCard.transform.SetParent(menuRoot.transform, false);
            optionsCard.transform.localPosition = new Vector3(0.0f, 0.17f, -0.16f);
            optionsCard.transform.localRotation = Quaternion.Euler(14f, 0f, 0f);
            spatialMenu.optionsPanelBox = optionsCard;

            GameObject optOuter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            optOuter.name = "OptionsCard_OuterFrame";
            optOuter.transform.SetParent(optionsCard.transform, false);
            optOuter.transform.localPosition = Vector3.zero;
            optOuter.transform.localScale = new Vector3(0.45f, 0.39f, 0.012f);
            optOuter.GetComponent<Renderer>().sharedMaterial = urushiMat;
            var ocCol = optOuter.GetComponent<Collider>(); if (ocCol != null) DestroyImmediate(ocCol);

            GameObject optInner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            optInner.name = "OptionsCard_InnerBoard";
            optInner.transform.SetParent(optionsCard.transform, false);
            optInner.transform.localPosition = new Vector3(0f, 0f, -0.003f);
            optInner.transform.localScale = new Vector3(0.428f, 0.368f, 0.009f);
            optInner.GetComponent<Renderer>().sharedMaterial = rosewoodMat;
            var oiCol = optInner.GetComponent<Collider>(); if (oiCol != null) DestroyImmediate(oiCol);

            GameObject optGoldRim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            optGoldRim.name = "OptionsCard_GoldBorder";
            optGoldRim.transform.SetParent(optionsCard.transform, false);
            optGoldRim.transform.localPosition = new Vector3(0f, 0.176f, -0.008f);
            optGoldRim.transform.localScale = new Vector3(0.415f, 0.006f, 0.005f);
            optGoldRim.GetComponent<Renderer>().sharedMaterial = goldMat;
            var ogCol = optGoldRim.GetComponent<Collider>(); if (ogCol != null) DestroyImmediate(ogCol);

            GameObject optCanvasObj = new GameObject("OptionsCard_Canvas");
            optCanvasObj.transform.SetParent(optionsCard.transform, false);
            optCanvasObj.transform.localPosition = new Vector3(0f, 0f, -0.009f);
            optCanvasObj.transform.localScale = Vector3.one * 0.001f;
            Canvas oCanvas = optCanvasObj.AddComponent<Canvas>();
            oCanvas.renderMode = RenderMode.WorldSpace;

            GameObject oTitleObj = new GameObject("OptionsTitle");
            oTitleObj.transform.SetParent(optCanvasObj.transform, false);
            oTitleObj.transform.localPosition = new Vector3(0f, 156f, 0f);
            Text oTitle = oTitleObj.AddComponent<Text>();
            oTitle.font = uiFont;
            oTitle.alignment = TextAnchor.MiddleCenter;
            oTitle.fontSize = 14;
            oTitle.fontStyle = FontStyle.Bold;
            oTitle.text = "⚙️ OPCIONES DEL SANTUARIO ZEN";
            oTitle.color = new Color(0.98f, 0.88f, 0.52f);
            oTitleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(390, 26);

            // Cuadrícula 2x4 de botones con detección exacta de la yema del índice (Columna Izq: x = -0.106m, Columna Der: x = +0.106m)
            // Fila 1 (y = +0.096m): Mesa Fija vs Centrar Mesa
            spatialMenu.btnOptLockTable = Create3DTouchButton(optionsCard.transform, "Btn_OptLockTable", "🔓 Mesa:\nMóvil (2 Manos)",
                new Vector3(-0.106f, 0.096f, -0.014f), new Vector3(0.184f, 0.050f, 0.018f), buttonMat, Color.white, 11);

            spatialMenu.btnOptRecenter = Create3DTouchButton(optionsCard.transform, "Btn_OptRecenter", "🎯 Centrar\nMesa Frente a Mí",
                new Vector3(0.106f, 0.096f, -0.014f), new Vector3(0.184f, 0.050f, 0.018f), infoButtonMat, new Color(0.92f, 0.99f, 1.0f), 11);

            // Fila 2 (y = +0.032m): Iluminación vs Entorno Virtual (Santuario Fuji / MR)
            spatialMenu.btnOptLighting = Create3DTouchButton(optionsCard.transform, "Btn_OptLighting", "🌅 Ambiente:\nDía Zen",
                new Vector3(-0.106f, 0.032f, -0.014f), new Vector3(0.184f, 0.050f, 0.018f), buttonMat, new Color(1.0f, 0.92f, 0.68f), 11);

            spatialMenu.btnOptEnvironment = Create3DTouchButton(optionsCard.transform, "Btn_OptEnvironment", "🥽 Entorno:\nPassthrough MR",
                new Vector3(0.106f, 0.032f, -0.014f), new Vector3(0.184f, 0.050f, 0.018f), buttonMat, new Color(0.70f, 0.92f, 1.0f), 11);

            // Fila 3 (y = -0.032m): Música Zen vs Enfoque de Cuidado
            spatialMenu.btnOptMusic = Create3DTouchButton(optionsCard.transform, "Btn_OptMusic", "🎵 Música:\nShakuhachi",
                new Vector3(-0.106f, -0.032f, -0.014f), new Vector3(0.184f, 0.050f, 0.018f), buttonMat, new Color(0.95f, 0.85f, 1.0f), 11);

            spatialMenu.btnOptTreeFocus = Create3DTouchButton(optionsCard.transform, "Btn_OptTreeFocus", "🌿 Cuidado:\n3 Árboles (Tríada)",
                new Vector3(0.106f, -0.032f, -0.014f), new Vector3(0.184f, 0.050f, 0.018f), buttonMat, new Color(0.82f, 0.98f, 0.82f), 11);

            // Fila 4 (y = -0.096m): Volumen de Sonido vs Pétalos de Sakura
            spatialMenu.btnOptSound = Create3DTouchButton(optionsCard.transform, "Btn_OptSound", "🔊 Sonido:\n100%",
                new Vector3(-0.106f, -0.096f, -0.014f), new Vector3(0.184f, 0.050f, 0.018f), buttonMat, new Color(0.84f, 0.98f, 0.86f), 11);

            spatialMenu.btnOptPetals = Create3DTouchButton(optionsCard.transform, "Btn_OptPetals", "🌸 Pétalos:\nActivados",
                new Vector3(0.106f, -0.096f, -0.014f), new Vector3(0.184f, 0.050f, 0.018f), buttonMat, new Color(1.0f, 0.82f, 0.90f), 11);

            optionsCard.SetActive(false);

            // Construir el Santuario Virtual 360° con Monte Fuji Cónico, Torii, Lago, Bambú y Cascada
            Material mountainMat = CreateOrGetPBRMaterial("Zen_MountainMat", new Color(0.22f, 0.30f, 0.44f), 0.15f, 0.0f);
            Material snowMat = CreateOrGetPBRMaterial("Zen_SnowMat", new Color(0.96f, 0.98f, 1.0f), 0.40f, 0.0f);
            Material mistMat = CreateOrGetPBRMaterial("Zen_MistMat", new Color(0.78f, 0.84f, 0.94f), 0.12f, 0.0f);
            spatialMenu.virtualSanctuaryBackdrop = BuildVirtualZenSanctuary(parent, mountainMat, snowMat, mistMat, urushiMat, hinokiMat, stoneMat, goldMat);
            spatialMenu.sakuraPetalsRain = BuildSakuraPetalsRain(parent, sakuraMat);

            // Activar por defecto el cerezo en flor e inicializar estado visual
            organicTree.ApplySpecies(TreeType.Sakura);
            spatialMenu.UpdateUI();
        }

        private static GameObject BuildVirtualZenSanctuary(
            Transform parent,
            Material mountainMat, Material snowMat, Material mistMat,
            Material urushiMat, Material hinokiMat, Material stoneMat, Material goldMat)
        {
            GameObject sanctuaryRoot = new GameObject("ZenSanctuary_VirtualBackdrop");
            sanctuaryRoot.transform.SetParent(parent, false);
            sanctuaryRoot.transform.localPosition = Vector3.zero;

            // Materiales exclusivos del Santuario Japonés 360°
            Material skyDomeMat = CreateOrGetPBRMaterial("Zen_SanctuarySkyDomeMat", new Color(0.52f, 0.72f, 0.92f), 0.0f, 0.0f);
            skyDomeMat.EnableKeyword("_EMISSION");
            if (skyDomeMat.HasProperty("_EmissionColor")) skyDomeMat.SetColor("_EmissionColor", new Color(0.36f, 0.50f, 0.66f));
            if (skyDomeMat.HasProperty("_Cull")) skyDomeMat.SetFloat("_Cull", 0f); // Renderizar interior de la cúpula 360°

            Material sunMat = CreateOrGetPBRMaterial("Zen_SanctuarySunMat", new Color(1.0f, 0.92f, 0.74f), 0.9f, 0.0f);
            sunMat.EnableKeyword("_EMISSION");
            if (sunMat.HasProperty("_EmissionColor")) sunMat.SetColor("_EmissionColor", new Color(1.0f, 0.85f, 0.55f) * 1.6f);

            Material toriiRedMat = CreateOrGetPBRMaterial("Zen_ToriiVermilionMat", new Color(0.82f, 0.16f, 0.12f), 0.65f, 0.05f);
            Material pondMat = CreateOrGetPBRMaterial("Zen_SanctuaryPondMat", new Color(0.14f, 0.46f, 0.58f), 0.94f, 0.10f);
            Material sandGardenMat = CreateOrGetPBRMaterial("Zen_KaresansuiSandMat", new Color(0.84f, 0.82f, 0.78f), 0.18f, 0.0f);
            Material bambooGreenMat = CreateOrGetPBRMaterial("Zen_BambooForestMat", new Color(0.22f, 0.52f, 0.20f), 0.42f, 0.0f);
            Material mossGroundMat = CreateOrGetPBRMaterial("Zen_SanctuaryMossMat", new Color(0.18f, 0.40f, 0.16f), 0.20f, 0.0f);

            // 1. CÚPULA CELESTE ENVOLVENTE 360° (EVITA EL FONDO NEGRO EN VR)
            GameObject skyDome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            skyDome.name = "Sanctuary_SkyDome";
            skyDome.transform.SetParent(sanctuaryRoot.transform, false);
            skyDome.transform.localPosition = Vector3.zero;
            skyDome.transform.localScale = new Vector3(68f, 48f, 68f);
            skyDome.GetComponent<Renderer>().sharedMaterial = skyDomeMat;
            var sdc = skyDome.GetComponent<Collider>(); if (sdc != null) DestroyImmediate(sdc);

            // Sol naciente / Disco solar dorado sobre el horizonte
            GameObject sunOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sunOrb.name = "Sanctuary_SunOrb";
            sunOrb.transform.SetParent(sanctuaryRoot.transform, false);
            sunOrb.transform.localPosition = new Vector3(-5.5f, 9.2f, 25f);
            sunOrb.transform.localScale = new Vector3(4.2f, 4.2f, 0.5f);
            sunOrb.GetComponent<Renderer>().sharedMaterial = sunMat;
            var soc = sunOrb.GetComponent<Collider>(); if (soc != null) DestroyImmediate(soc);

            // 2. TERRAZA DE MADERA ENGAWA Y JARDÍN SECO KARESANSUI BAJO EL JUGADOR
            GameObject engawaDeck = GameObject.CreatePrimitive(PrimitiveType.Cube);
            engawaDeck.name = "Sanctuary_EngawaWoodenFloor";
            engawaDeck.transform.SetParent(sanctuaryRoot.transform, false);
            engawaDeck.transform.localPosition = new Vector3(0f, -0.76f, -0.4f);
            engawaDeck.transform.localScale = new Vector3(4.2f, 0.08f, 3.4f);
            engawaDeck.GetComponent<Renderer>().sharedMaterial = hinokiMat;
            var edc = engawaDeck.GetComponent<Collider>(); if (edc != null) DestroyImmediate(edc);

            // Jardín zen de grava blanca rastrillada alrededor del pabellón
            GameObject sandFloor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            sandFloor.name = "Sanctuary_KaresansuiGarden";
            sandFloor.transform.SetParent(sanctuaryRoot.transform, false);
            sandFloor.transform.localPosition = new Vector3(0f, -0.84f, 6.0f);
            sandFloor.transform.localScale = new Vector3(32f, 0.04f, 32f);
            sandFloor.GetComponent<Renderer>().sharedMaterial = sandGardenMat;
            var sfc = sandFloor.GetComponent<Collider>(); if (sfc != null) DestroyImmediate(sfc);

            // Estanque reflectante turquesa frente a la terraza Engawa
            GameObject koiPond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            koiPond.name = "Sanctuary_ReflectivePond";
            koiPond.transform.SetParent(sanctuaryRoot.transform, false);
            koiPond.transform.localPosition = new Vector3(0f, -0.80f, 8.5f);
            koiPond.transform.localScale = new Vector3(14f, 0.03f, 9.5f);
            koiPond.GetComponent<Renderer>().sharedMaterial = pondMat;
            var kpc = koiPond.GetComponent<Collider>(); if (kpc != null) DestroyImmediate(kpc);

            // Islas de musgo y roca en el estanque
            Vector3[] islandPos = { new Vector3(-4.2f, -0.65f, 7.2f), new Vector3(4.5f, -0.60f, 8.0f), new Vector3(-2.0f, -0.70f, 11.2f) };
            Vector3[] islandScale = { new Vector3(2.8f, 0.45f, 2.2f), new Vector3(3.2f, 0.55f, 2.5f), new Vector3(2.4f, 0.38f, 1.9f) };
            for (int i = 0; i < islandPos.Length; i++)
            {
                GameObject island = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                island.name = $"Sanctuary_MossIsland_{i}";
                island.transform.SetParent(sanctuaryRoot.transform, false);
                island.transform.localPosition = islandPos[i];
                island.transform.localScale = islandScale[i];
                island.GetComponent<Renderer>().sharedMaterial = mossGroundMat;
                var ic = island.GetComponent<Collider>(); if (ic != null) DestroyImmediate(ic);
            }

            // 3. PUERTA SAGRADA TORII EN BERMELLÓN ENMARCANDO EL MONTE FUJI
            GameObject toriiRoot = new GameObject("Sanctuary_ToriiGate");
            toriiRoot.transform.SetParent(sanctuaryRoot.transform, false);
            toriiRoot.transform.localPosition = new Vector3(0f, -0.75f, 9.2f);

            float[] toriiPillarX = { -1.45f, 1.45f };
            for (int p = 0; p < 2; p++)
            {
                GameObject tPillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tPillar.transform.SetParent(toriiRoot.transform, false);
                tPillar.transform.localPosition = new Vector3(toriiPillarX[p], 1.65f, 0f);
                tPillar.transform.localScale = new Vector3(0.22f, 1.65f, 0.22f);
                tPillar.GetComponent<Renderer>().sharedMaterial = toriiRedMat;
                var tpc = tPillar.GetComponent<Collider>(); if (tpc != null) DestroyImmediate(tpc);

                GameObject tBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tBase.transform.SetParent(toriiRoot.transform, false);
                tBase.transform.localPosition = new Vector3(toriiPillarX[p], 0.12f, 0f);
                tBase.transform.localScale = new Vector3(0.34f, 0.14f, 0.34f);
                tBase.GetComponent<Renderer>().sharedMaterial = urushiMat;
                var tbc = tBase.GetComponent<Collider>(); if (tbc != null) DestroyImmediate(tbc);
            }

            // Dintel superior Kasagi y travesaño Nuki del Torii
            GameObject kasagi = GameObject.CreatePrimitive(PrimitiveType.Cube);
            kasagi.transform.SetParent(toriiRoot.transform, false);
            kasagi.transform.localPosition = new Vector3(0f, 3.35f, 0f);
            kasagi.transform.localScale = new Vector3(4.1f, 0.20f, 0.28f);
            kasagi.GetComponent<Renderer>().sharedMaterial = toriiRedMat;
            var kgc = kasagi.GetComponent<Collider>(); if (kgc != null) DestroyImmediate(kgc);

            GameObject shimakiCap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shimakiCap.transform.SetParent(toriiRoot.transform, false);
            shimakiCap.transform.localPosition = new Vector3(0f, 3.48f, 0f);
            shimakiCap.transform.localScale = new Vector3(4.25f, 0.08f, 0.32f);
            shimakiCap.GetComponent<Renderer>().sharedMaterial = urushiMat;
            var skc = shimakiCap.GetComponent<Collider>(); if (skc != null) DestroyImmediate(skc);

            GameObject nuki = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nuki.transform.SetParent(toriiRoot.transform, false);
            nuki.transform.localPosition = new Vector3(0f, 2.75f, 0f);
            nuki.transform.localScale = new Vector3(3.55f, 0.14f, 0.18f);
            nuki.GetComponent<Renderer>().sharedMaterial = toriiRedMat;
            var nkc = nuki.GetComponent<Collider>(); if (nkc != null) DestroyImmediate(nkc);

            // 4. LINTERNAS DE PIEDRA JAPONESAS (ISHIDORO) CON LUZ CÁLIDA A AMBOS LADOS
            float[] lanternX = { -2.2f, 2.2f };
            for (int l = 0; l < 2; l++)
            {
                GameObject lantern = new GameObject($"Sanctuary_StoneLantern_{l}");
                lantern.transform.SetParent(sanctuaryRoot.transform, false);
                lantern.transform.localPosition = new Vector3(lanternX[l], -0.75f, 3.2f);

                GameObject lPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                lPost.transform.SetParent(lantern.transform, false);
                lPost.transform.localPosition = new Vector3(0f, 0.55f, 0f);
                lPost.transform.localScale = new Vector3(0.18f, 0.55f, 0.18f);
                lPost.GetComponent<Renderer>().sharedMaterial = stoneMat;
                var lpc = lPost.GetComponent<Collider>(); if (lpc != null) DestroyImmediate(lpc);

                GameObject lLightBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lLightBox.transform.SetParent(lantern.transform, false);
                lLightBox.transform.localPosition = new Vector3(0f, 1.22f, 0f);
                lLightBox.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);
                lLightBox.GetComponent<Renderer>().sharedMaterial = sunMat;
                var llc = lLightBox.GetComponent<Collider>(); if (llc != null) DestroyImmediate(llc);

                GameObject lRoof = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                lRoof.transform.SetParent(lantern.transform, false);
                lRoof.transform.localPosition = new Vector3(0f, 1.44f, 0f);
                lRoof.transform.localScale = new Vector3(0.58f, 0.08f, 0.58f);
                lRoof.GetComponent<Renderer>().sharedMaterial = stoneMat;
                var lrc = lRoof.GetComponent<Collider>(); if (lrc != null) DestroyImmediate(lrc);
            }

            // 5. BOSQUECILLO DE BAMBÚ ARASHIYAMA FLANQUEANDO EL JARDÍN
            for (int b = 0; b < 14; b++)
            {
                float side = (b % 2 == 0) ? -1f : 1f;
                float bx = side * (3.2f + (b % 5) * 0.65f);
                float bz = 1.5f + (b / 2) * 1.15f;
                GameObject stalk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                stalk.name = $"Sanctuary_BambooStalk_{b}";
                stalk.transform.SetParent(sanctuaryRoot.transform, false);
                stalk.transform.localPosition = new Vector3(bx, 1.8f, bz);
                stalk.transform.localRotation = Quaternion.Euler(Random.Range(-2.5f, 2.5f), 0f, Random.Range(-3f, 3f));
                stalk.transform.localScale = new Vector3(0.085f, 2.8f, 0.085f);
                stalk.GetComponent<Renderer>().sharedMaterial = bambooGreenMat;
                var sc = stalk.GetComponent<Collider>(); if (sc != null) DestroyImmediate(sc);
            }

            // 6. MONTE FUJI CÓNICO ESCULPIDO CON CUMBRE NEVADA EN EL HORIZONTE (NO UN CILINDRO PLANO)
            Mesh fujiConeMesh = ZenBonsaiProceduralMeshes.CreateConicalMountainMesh("Mesh_Sanctuary_FujiCone", 18f, 2.8f, 9.5f, 36);
            GameObject fujiBody = new GameObject("Distant_MountFuji");
            fujiBody.transform.SetParent(sanctuaryRoot.transform, false);
            fujiBody.transform.localPosition = new Vector3(0f, -0.8f, 26f);
            fujiBody.AddComponent<MeshFilter>().sharedMesh = fujiConeMesh;
            fujiBody.AddComponent<MeshRenderer>().sharedMaterial = mountainMat;

            Mesh fujiSnowMesh = ZenBonsaiProceduralMeshes.CreateConicalMountainMesh("Mesh_Sanctuary_FujiSnowCap", 6.4f, 2.75f, 3.6f, 36);
            GameObject fujiSnow = new GameObject("Distant_MountFuji_SnowPeak");
            fujiSnow.transform.SetParent(sanctuaryRoot.transform, false);
            fujiSnow.transform.localPosition = new Vector3(0f, 5.15f, 26f);
            fujiSnow.AddComponent<MeshFilter>().sharedMesh = fujiSnowMesh;
            fujiSnow.AddComponent<MeshRenderer>().sharedMaterial = snowMat;

            // Colinas laterales suaves
            Mesh hillMesh = ZenBonsaiProceduralMeshes.CreateConicalMountainMesh("Mesh_Sanctuary_SideHill", 12f, 2.2f, 5.2f, 24);
            float[] hillX = { -15f, 15f };
            for (int h = 0; h < 2; h++)
            {
                GameObject hill = new GameObject($"Sanctuary_SideMountain_{h}");
                hill.transform.SetParent(sanctuaryRoot.transform, false);
                hill.transform.localPosition = new Vector3(hillX[h], -0.8f, 22f);
                hill.AddComponent<MeshFilter>().sharedMesh = hillMesh;
                hill.AddComponent<MeshRenderer>().sharedMaterial = mountainMat;
            }

            // 7. CASCADA ZEN ANIMADA SOBRE ROCAS EN EL ESTANQUE DERECHO
            GameObject waterfallRock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            waterfallRock.name = "Sanctuary_WaterfallCliff";
            waterfallRock.transform.SetParent(sanctuaryRoot.transform, false);
            waterfallRock.transform.localPosition = new Vector3(5.2f, 1.1f, 8.8f);
            waterfallRock.transform.localScale = new Vector3(2.8f, 4.2f, 2.2f);
            waterfallRock.GetComponent<Renderer>().sharedMaterial = stoneMat;
            var wrc = waterfallRock.GetComponent<Collider>(); if (wrc != null) DestroyImmediate(wrc);

            GameObject wfDropsRoot = new GameObject("Sanctuary_WaterfallDrops");
            wfDropsRoot.transform.SetParent(sanctuaryRoot.transform, false);
            wfDropsRoot.transform.localPosition = new Vector3(4.6f, 0f, 7.8f);
            for (int d = 0; d < 10; d++)
            {
                GameObject drop = GameObject.CreatePrimitive(PrimitiveType.Cube);
                drop.name = $"WaterfallStream_{d}";
                drop.transform.SetParent(wfDropsRoot.transform, false);
                drop.transform.localPosition = new Vector3((d % 3 - 1) * 0.22f, -0.4f + d * 0.36f, (d % 2) * 0.05f);
                drop.transform.localScale = new Vector3(0.18f, 0.45f, 0.04f);
                drop.GetComponent<Renderer>().sharedMaterial = snowMat;
                var dc = drop.GetComponent<Collider>(); if (dc != null) DestroyImmediate(dc);
            }

            sanctuaryRoot.SetActive(false); // Inicia en Passthrough MR por defecto
            return sanctuaryRoot;
        }

        private static GameObject BuildSakuraPetalsRain(Transform parent, Material petalMat)
        {
            GameObject rainRoot = new GameObject("SakuraPetals_Rain");
            rainRoot.transform.SetParent(parent, false);
            rainRoot.transform.localPosition = Vector3.zero;

            // 24 pétalos 3D de doble cara flotando y cayendo suavemente en bucle alrededor del bonsái
            for (int p = 0; p < 24; p++)
            {
                GameObject petal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                petal.name = $"FloatingPetal_{p}";
                petal.transform.SetParent(rainRoot.transform, false);
                float ang = p * 15f * Mathf.Deg2Rad;
                float r = 0.16f + (p % 5) * 0.055f;
                float y = -0.02f + (p * 0.020f);
                petal.transform.localPosition = new Vector3(Mathf.Cos(ang) * r, y, Mathf.Sin(ang) * r + 0.02f);
                petal.transform.localRotation = Quaternion.Euler(Random.Range(10f, 65f), Random.Range(0f, 360f), Random.Range(10f, 55f));
                petal.transform.localScale = new Vector3(0.016f, 0.0028f, 0.011f);
                petal.GetComponent<Renderer>().sharedMaterial = petalMat;
                var pc = petal.GetComponent<Collider>(); if (pc != null) DestroyImmediate(pc);
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
            // CAZO DE BAMBÚ INTERACTIVO (HISHAKU) ORIENTADO HACIA EL JUGADOR (COMO LAS HERRAMIENTAS)
            // Radio de agarre amplio y ergonómico (0.125m) para cogerlo sin esfuerzo
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
            ladleTool.grabDistance = 0.125f; // Radio amplio y cómodo para coger el cazo fácilmente

            // Mango largo de bambú orientado hacia la mano izquierda del jugador (-Z)
            GameObject ladleStick = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ladleStick.name = "Hishaku_BambooStick";
            ladleStick.transform.SetParent(ladleObj.transform, false);
            ladleStick.transform.localPosition = new Vector3(0f, 0f, -0.025f);
            ladleStick.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            ladleStick.transform.localScale = new Vector3(0.011f, 0.078f, 0.011f);
            ladleStick.GetComponent<Renderer>().sharedMaterial = bambooMat;
            var cLS = ladleStick.GetComponent<Collider>(); if (cLS != null) DestroyImmediate(cLS);

            // Nudo dorado decorativo en el mango de bambú
            GameObject ladleNode = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ladleNode.transform.SetParent(ladleObj.transform, false);
            ladleNode.transform.localPosition = new Vector3(0f, 0f, -0.055f);
            ladleNode.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            ladleNode.transform.localScale = new Vector3(0.013f, 0.004f, 0.013f);
            ladleNode.GetComponent<Renderer>().sharedMaterial = goldMat;
            var cLN = ladleNode.GetComponent<Collider>(); if (cLN != null) DestroyImmediate(cLN);

            // Cuenco/cazo de bambú en el extremo delantero (+Z) por donde sale el único chorro de agua
            GameObject ladleCup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ladleCup.name = "Hishaku_BambooCup";
            ladleCup.transform.SetParent(ladleObj.transform, false);
            ladleCup.transform.localPosition = new Vector3(0f, 0.004f, 0.058f);
            ladleCup.transform.localScale = new Vector3(0.036f, 0.019f, 0.036f);
            ladleCup.GetComponent<Renderer>().sharedMaterial = bambooMat;
            var cLC = ladleCup.GetComponent<Collider>(); if (cLC != null) DestroyImmediate(cLC);

            // Agua brillante dentro del cazo de bambú
            GameObject cupWater = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cupWater.transform.SetParent(ladleCup.transform, false);
            cupWater.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            cupWater.transform.localScale = new Vector3(0.84f, 0.15f, 0.84f);
            cupWater.GetComponent<Renderer>().sharedMaterial = waterMat;
            var cCW = cupWater.GetComponent<Collider>(); if (cCW != null) DestroyImmediate(cCW);

            GameObject ladleHandleAnchor = new GameObject("HandleAnchor");
            ladleHandleAnchor.transform.SetParent(ladleObj.transform, false);
            ladleHandleAnchor.transform.localPosition = new Vector3(0f, 0f, -0.045f);
            ladleTool.handleAnchor = ladleHandleAnchor.transform;

            // Punto exacto de vertido en el borde exterior del cuenco de bambú (Hishaku_BambooCup)
            GameObject ladlePourPoint = new GameObject("LadlePourPoint");
            ladlePourPoint.transform.SetParent(ladleCup.transform, false);
            ladlePourPoint.transform.localPosition = new Vector3(0f, -0.20f, 0.45f);
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

        #region 1. SAKURA (Cerezo Imperial Florido - Corteza Realista con Vena Shari y Flores de 5 Pétalos)

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

            // Ramas orgánicas continuas con sub-ramificación
            BranchSpline[] sakuraBranches = new BranchSpline[]
            {
                // Rama 1: Ichi-no-eda (Izquierda baja extendida)
                new BranchSpline(
                    new Vector3(-0.010f, 0.115f, 0.005f),
                    new Vector3(-0.048f, 0.125f, 0.012f),
                    new Vector3(-0.085f, 0.132f, 0.015f),
                    new Vector3(-0.118f, 0.145f, 0.018f),
                    0.016f, 0.0055f),
                // Rama 2: Ni-no-eda (Derecha elegante)
                new BranchSpline(
                    new Vector3(0.012f, 0.148f, -0.006f),
                    new Vector3(0.052f, 0.158f, -0.012f),
                    new Vector3(0.088f, 0.168f, -0.016f),
                    new Vector3(0.115f, 0.182f, -0.018f),
                    0.015f, 0.0050f),
                // Rama 3: Ura-eda (Trasera de profundidad)
                new BranchSpline(
                    new Vector3(0.004f, 0.178f, 0.010f),
                    new Vector3(0.002f, 0.188f, 0.042f),
                    new Vector3(-0.005f, 0.198f, 0.072f),
                    new Vector3(-0.008f, 0.212f, 0.095f),
                    0.013f, 0.0048f),
                // Rama 4: Frontal-Izquierda superior
                new BranchSpline(
                    new Vector3(-0.006f, 0.205f, -0.005f),
                    new Vector3(-0.035f, 0.215f, -0.028f),
                    new Vector3(-0.058f, 0.225f, -0.045f),
                    new Vector3(-0.072f, 0.236f, -0.052f),
                    0.011f, 0.0042f),
                // Rama 5: Jushin (Ápice imperial florido)
                new BranchSpline(
                    new Vector3(0.005f, 0.225f, 0.006f),
                    new Vector3(0.008f, 0.242f, 0.004f),
                    new Vector3(0.010f, 0.258f, 0.002f),
                    new Vector3(0.012f, 0.272f, 0.000f),
                    0.012f, 0.0045f)
            };

            Mesh sakuraWoodMesh = ZenBonsaiProceduralMeshes.CreateMasterTreeWoodMesh(
                "Mesh_Sakura_WoodMasterpiece",
                new Vector3(0.000f, 0.026f, 0.000f),
                new Vector3(-0.038f, 0.105f, 0.014f),
                new Vector3(0.032f, 0.175f, -0.012f),
                new Vector3(0.008f, 0.245f, 0.004f),
                0.038f, 0.022f, 0.010f,
                sakuraBranches,
                8, 0.85f, 0.14f, 6.5f, 101);

            trunk.AddComponent<MeshFilter>().sharedMesh = sakuraWoodMesh;
            trunk.AddComponent<MeshRenderer>().sharedMaterial = barkMat;

            // Musgo aterciopelado abrazando las raíces superficiales Nebari del tronco único de cerezo
            for (int m = 0; m < 4; m++)
            {
                float ang = (m * 90f + 20f) * Mathf.Deg2Rad;
                GameObject rootMoss = new GameObject($"Sakura_NebariMoss_{m}");
                rootMoss.transform.SetParent(trunk.transform, false);
                rootMoss.transform.localPosition = new Vector3(Mathf.Cos(ang) * 0.026f, 0.040f, Mathf.Sin(ang) * 0.026f);
                rootMoss.AddComponent<MeshFilter>().sharedMesh = ZenBonsaiProceduralMeshes.CreateSuisekiStoneMesh(
                    $"Mesh_SakuraRootMoss_{m}", new Vector3(0.022f, 0.011f, 0.019f), 170 + m);
                rootMoss.AddComponent<MeshRenderer>().sharedMaterial = mossMat;
            }

            Vector3[] padScales =
            {
                new Vector3(0.125f, 0.048f, 0.105f),
                new Vector3(0.120f, 0.046f, 0.102f),
                new Vector3(0.108f, 0.042f, 0.095f),
                new Vector3(0.098f, 0.040f, 0.088f),
                new Vector3(0.115f, 0.050f, 0.105f)
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
                    $"Mesh_Sakura_CloudPad_{i}", pScale, 14, 0.72f, 0.15f, 400 + i * 23);

                foliagePad.AddComponent<MeshFilter>().sharedMesh = padMesh;
                MeshRenderer padRen = foliagePad.AddComponent<MeshRenderer>();
                padRen.sharedMaterial = (i % 2 == 0) ? petalPinkMat : leafShootMat;

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

                // Corolas reales de 5 pétalos de Sakura con estambres dorados sobre la nube
                Mesh blossomMesh = ZenBonsaiProceduralMeshes.CreateSakuraBlossomCorollasMesh(
                    $"Mesh_Sakura_Corollas_{i}", pScale, 24, 500 + i * 31, out Mesh stamenMesh);

                GameObject corollasObj = new GameObject($"Sakura_Blossoms5Petal_{i}");
                corollasObj.transform.SetParent(foliagePad.transform, false);
                corollasObj.AddComponent<MeshFilter>().sharedMesh = blossomMesh;
                corollasObj.AddComponent<MeshRenderer>().sharedMaterial = (i % 2 == 0) ? petalWhiteMat : petalPinkMat;

                GameObject stamensObj = new GameObject($"Sakura_GoldStamens_{i}");
                stamensObj.transform.SetParent(foliagePad.transform, false);
                stamensObj.AddComponent<MeshFilter>().sharedMesh = stamenMesh;
                stamensObj.AddComponent<MeshRenderer>().sharedMaterial = goldMat;
            }

            // Pétalos de cerezo de 5 puntas caídos sobre el musgo
            Mesh fallenBlossomsMesh = ZenBonsaiProceduralMeshes.CreateSakuraBlossomCorollasMesh(
                "Mesh_Sakura_FallenPetals", new Vector3(0.22f, 0.008f, 0.16f), 14, 777, out Mesh _);
            GameObject fallenObj = new GameObject("Sakura_FallenBlossoms_OnMoss");
            fallenObj.transform.SetParent(treeObj.transform, false);
            fallenObj.transform.localPosition = new Vector3(0f, 0.036f, 0f);
            fallenObj.AddComponent<MeshFilter>().sharedMesh = fallenBlossomsMesh;
            fallenObj.AddComponent<MeshRenderer>().sharedMaterial = petalPinkMat;

            // Floración Plena (Recompensa Full Bloom)
            GameObject blossomRewardObj = new GameObject("Sakura_FullBloom_Reward");
            blossomRewardObj.transform.SetParent(treeObj.transform, false);
            for (int i = 0; i < sakuraBranches.Length; i++)
            {
                Mesh extraBloom = ZenBonsaiProceduralMeshes.CreateSakuraBlossomCorollasMesh(
                    $"Mesh_Sakura_FullBloom_{i}", padScales[i] * 1.12f, 18, 880 + i * 17, out Mesh _);
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
                    $"Mesh_Matsu_Needles_{i}", pScale, 36, 700 + i * 19);

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

                // Hojas estrelladas de 5 puntas en escarlata y ámbar dorado sobre cada copa
                Mesh starLeavesMesh = ZenBonsaiProceduralMeshes.CreateMomijiStarLeavesMesh(
                    $"Mesh_Momiji_StarLeaves_{i}", pScale, 26, 900 + i * 41);

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

        private static void BuildToolTray(Transform parent, Material trayMat, Material darkWoodMat, Material steelMat, Material copperMat, Material bambooMat, Material bristleMat, Material goldMat, Material potMat, Material mossMat, Material stoneMat, Material blossomMat)
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
            // 1. IZQUIERDA (x = -0.064m): TIJERAS SATSUKI HASAMI (MANGOS HACIA EL JUGADOR)
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
            shearsTool.grabDistance = 0.085f; // Radio ceñido para agarre intencional
            trayComp.shears = shearsTool;

            // Mangos en bucle tradicional japonés
            GameObject handleLoopL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handleLoopL.transform.SetParent(shearsObj.transform, false);
            handleLoopL.transform.localPosition = new Vector3(-0.02f, 0f, -0.05f);
            handleLoopL.transform.localScale = new Vector3(0.035f, 0.006f, 0.045f);
            handleLoopL.GetComponent<Renderer>().sharedMaterial = steelMat;
            var cHL = handleLoopL.GetComponent<Collider>(); if (cHL != null) DestroyImmediate(cHL);

            GameObject handleLoopR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handleLoopR.transform.SetParent(shearsObj.transform, false);
            handleLoopR.transform.localPosition = new Vector3(0.02f, 0f, -0.05f);
            handleLoopR.transform.localScale = new Vector3(0.035f, 0.006f, 0.045f);
            handleLoopR.GetComponent<Renderer>().sharedMaterial = steelMat;
            var cHR = handleLoopR.GetComponent<Collider>(); if (cHR != null) DestroyImmediate(cHR);

            GameObject pivot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pivot.transform.SetParent(shearsObj.transform, false);
            pivot.transform.localPosition = new Vector3(0f, 0.004f, 0f);
            pivot.transform.localScale = new Vector3(0.012f, 0.008f, 0.012f);
            pivot.GetComponent<Renderer>().sharedMaterial = goldMat;
            var cPiv = pivot.GetComponent<Collider>(); if (cPiv != null) DestroyImmediate(cPiv);

            GameObject bladeA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bladeA.name = "BladeA";
            bladeA.transform.SetParent(shearsObj.transform, false);
            bladeA.transform.localPosition = new Vector3(-0.008f, 0.002f, 0.045f);
            bladeA.transform.localScale = new Vector3(0.008f, 0.003f, 0.075f);
            bladeA.transform.localRotation = Quaternion.Euler(0f, 5f, 0f);
            bladeA.GetComponent<Renderer>().sharedMaterial = steelMat;
            var cBA = bladeA.GetComponent<Collider>(); if (cBA != null) DestroyImmediate(cBA);

            GameObject bladeB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bladeB.name = "BladeB";
            bladeB.transform.SetParent(shearsObj.transform, false);
            bladeB.transform.localPosition = new Vector3(0.008f, -0.002f, 0.045f);
            bladeB.transform.localScale = new Vector3(0.008f, 0.003f, 0.075f);
            bladeB.transform.localRotation = Quaternion.Euler(0f, -5f, 0f);
            bladeB.GetComponent<Renderer>().sharedMaterial = steelMat;
            var cBB = bladeB.GetComponent<Collider>(); if (cBB != null) DestroyImmediate(cBB);

            shearsTool.movingPartA = bladeA.transform;
            shearsTool.movingPartB = bladeB.transform;

            GameObject shearsHandleAnchor = new GameObject("HandleAnchor");
            shearsHandleAnchor.transform.SetParent(shearsObj.transform, false);
            shearsHandleAnchor.transform.localPosition = new Vector3(0f, 0f, -0.04f);
            shearsTool.handleAnchor = shearsHandleAnchor.transform;

            GameObject snipPoint = new GameObject("SnipPoint");
            snipPoint.transform.SetParent(shearsObj.transform, false);
            snipPoint.transform.localPosition = new Vector3(0f, 0f, 0.08f);
            shearsTool.actionPoint = snipPoint.transform;

            // =========================================================================
            // 2. DERECHA (x = +0.064m): PINCEL / ESCOBILLA DE BAMBÚ (HOKI - MANGO HACIA EL JUGADOR)
            // (La regadera se ha retirado para usar cómodamente el cazo Hishaku del otro lado)
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
            broomTool.grabDistance = 0.085f; // Radio ceñido para agarre intencional
            trayComp.broom = broomTool;

            GameObject bambooStick = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bambooStick.transform.SetParent(broomObj.transform, false);
            bambooStick.transform.localPosition = new Vector3(0f, 0f, -0.04f);
            bambooStick.transform.localScale = new Vector3(0.016f, 0.054f, 0.016f);
            bambooStick.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            bambooStick.GetComponent<Renderer>().sharedMaterial = bambooMat;
            var cBS = bambooStick.GetComponent<Collider>(); if (cBS != null) DestroyImmediate(cBS);

            GameObject goldFerrule = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            goldFerrule.transform.SetParent(broomObj.transform, false);
            goldFerrule.transform.localPosition = new Vector3(0f, 0f, 0.012f);
            goldFerrule.transform.localScale = new Vector3(0.024f, 0.009f, 0.024f);
            goldFerrule.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            goldFerrule.GetComponent<Renderer>().sharedMaterial = goldMat;
            var cGF = goldFerrule.GetComponent<Collider>(); if (cGF != null) DestroyImmediate(cGF);

            GameObject bristles = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bristles.transform.SetParent(broomObj.transform, false);
            bristles.transform.localPosition = new Vector3(0f, 0f, 0.048f);
            bristles.transform.localScale = new Vector3(0.048f, 0.018f, 0.066f);
            bristles.GetComponent<Renderer>().sharedMaterial = bristleMat;
            var cBr = bristles.GetComponent<Collider>(); if (cBr != null) DestroyImmediate(cBr);

            GameObject broomHandleAnchor = new GameObject("HandleAnchor");
            broomHandleAnchor.transform.SetParent(broomObj.transform, false);
            broomHandleAnchor.transform.localPosition = new Vector3(0f, 0f, -0.035f);
            broomTool.handleAnchor = broomHandleAnchor.transform;

            GameObject bristleTip = new GameObject("BristleTip");
            bristleTip.transform.SetParent(bristles.transform, false);
            bristleTip.transform.localPosition = Vector3.forward * 0.5f;
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
        /// Genera un material PBR de diseñador con textura Albedo 512x512 + Mapa de Normales 512x512 (_BumpMap)
        /// para que cada tronco único tenga relieve 3D hiperrealista de corteza natural, musgo en la base y nudos.
        /// </summary>
        private static Material CreateOrGetBarkMaterial(string matName, Color baseColor, Color furrowColor, Color highlightColor, float smoothness, int barkStyle)
        {
            Material mat = CreateOrGetPBRMaterial(matName, Color.white, smoothness, 0.0f);

            string texPath = $"Assets/ZenBonsai/Materials/{matName}_v17_Tex.png";
            string normPath = $"Assets/ZenBonsai/Materials/{matName}_v17_Norm.png";

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
                float bumpStrength = 3.2f;
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
                if (mat.HasProperty("_BumpScale")) mat.SetFloat("_BumpScale", 1.35f);
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
            box.size = new Vector3(size.x * 0.92f, size.y * 0.92f, 0.032f);
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

            // IMPORTANTE: LabelCanvas es hijo de btnObj (escala uniforme 1,1,1), NUNCA de plate,
            // para que el nombre del botón se vea grande, nítido y perfectamente centrado dentro del botón
            GameObject textCanvasObj = new GameObject("LabelCanvas");
            textCanvasObj.transform.SetParent(btnObj.transform, false);
            textCanvasObj.transform.localPosition = new Vector3(0f, 0f, -(size.z * 0.5f + 0.002f));
            textCanvasObj.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            Canvas canvas = textCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            textCanvasObj.AddComponent<CanvasScaler>();

            GameObject tChild = new GameObject("Text");
            tChild.transform.SetParent(textCanvasObj.transform, false);
            Text txt = tChild.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (txt.font == null) txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontStyle = FontStyle.Bold;
            txt.fontSize = Mathf.RoundToInt(fontSize);
            txt.text = label;
            txt.color = textColor;
            RectTransform rect = tChild.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size.x * 1000f, size.y * 1000f);

            ZenTouchButton touchBtn = btnObj.AddComponent<ZenTouchButton>();
            touchBtn.visualPlate = plate.transform;
            touchBtn.neonBorderRenderers = neonRenderers;
            touchBtn.buttonLabelText = txt;
            touchBtn.touchHalfSize = new Vector2(size.x * 0.52f, size.y * 0.52f);
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

                    // Ramificación secundaria y terciaria en abanico que sostiene cada nube de follaje
                    for (int s = -2; s <= 2; s++)
                    {
                        float tAttach = Mathf.Clamp(0.64f + Mathf.Abs(s) * 0.08f, 0.55f, 0.92f);
                        Vector3 twigStart = EvalBezier(br.p0, br.p1, br.p2, br.p3, tAttach);
                        Vector3 spreadDir = (tipDir + side * (s * 0.48f) + Vector3.up * (0.22f + (s % 2 == 0 ? 0.08f : 0f))).normalized;
                        Vector3 tp1 = twigStart + spreadDir * 0.016f;
                        Vector3 tp2 = twigStart + spreadDir * 0.030f + Vector3.up * 0.006f;
                        Vector3 tp3 = twigStart + spreadDir * 0.044f + Vector3.up * 0.011f;

                        AppendSplineTube(
                            verts, tris, uvs,
                            twigStart, tp1, tp2, tp3,
                            br.rEnd * 1.05f, br.rEnd * 0.70f, br.rEnd * 0.32f,
                            12, 10,
                            0, 0f, barkRidgeDepth * 0.35f, 1.2f, seed + 600 + b * 19 + s, true);
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

            for (int p = 0; p < puffCount; p++)
            {
                Vector3 offset;
                Vector3 subRadii;

                if (p == 0)
                {
                    offset = Vector3.zero;
                    subRadii = new Vector3(rx * 0.72f, ry * 0.85f, rz * 0.72f);
                }
                else
                {
                    float ang = (p / (float)(puffCount - 1)) * Mathf.PI * 2f + Random.Range(-0.25f, 0.25f);
                    float ringDist = (p % 2 == 0) ? Random.Range(0.35f, 0.68f) : Random.Range(0.15f, 0.45f);
                    float yOff = (1f - ringDist) * ry * 0.42f + Random.Range(-ry * 0.12f, ry * 0.18f);
                    offset = new Vector3(Mathf.Cos(ang) * rx * ringDist, yOff, Mathf.Sin(ang) * rz * ringDist);
                    float s = Random.Range(0.28f, 0.46f);
                    subRadii = new Vector3(rx * s, ry * (s + 0.18f), rz * s);
                }

                AppendBotanicalPuff(verts, tris, uvs, offset, subRadii, bottomFlatness, leafSpikiness, seed + p * 13);
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

                for (int k = 0; k < 6; k++) pUvs.Add(new Vector2(0.5f, 0.5f));

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
    }
}
#endif
