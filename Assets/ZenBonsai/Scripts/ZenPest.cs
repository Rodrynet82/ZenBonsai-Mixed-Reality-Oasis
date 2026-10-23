using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Oruga zen brillante de 3 segmentos que sube por el exterior del tronco del bonsái.
    /// Al barrerla con la escobilla Hoki o tocarla con la mano, se disuelve en chispas y emite campanillas.
    /// </summary>
    public class ZenPest : MonoBehaviour
    {
        [Header("Movimiento")]
        public Transform targetPosition;
        public float crawlSpeed = 0.038f;

        private Vector3 p0, p1, p2, p3;
        private float rBase, rMid, rTop;
        private float uBranchAttach = 0.65f;
        private Vector3 localBranchTip;
        private bool useLocalTrunkSpline = false;

        private Vector3 startPos;
        private Vector3 controlPos;
        private Vector3 targetPos;
        private float progress = 0f;
        private bool isDispelled = false;

        private Transform segHead;
        private Transform segMid;
        private Transform segTail;

        /// <summary>
        /// Inicializa la oruga como hija del tronco (trunkBase) para que trepe pegada milimétricamente
        /// a la corteza real del tronco (Sakura, Matsu o Momiji) y luego salga por la rama hacia las hojas.
        /// </summary>
        public void InitializeOnTrunk(TreeType species, Vector3 branchLeafLocalPos)
        {
            useLocalTrunkSpline = true;
            localBranchTip = branchLeafLocalPos + new Vector3(0f, -0.004f, -0.012f);

            switch (species)
            {
                case TreeType.Matsu:
                    p0 = new Vector3(0.000f, 0.026f, 0.000f);
                    p1 = new Vector3(-0.048f, 0.108f, 0.016f);
                    p2 = new Vector3(0.042f, 0.172f, -0.014f);
                    p3 = new Vector3(-0.006f, 0.245f, 0.005f);
                    rBase = 0.042f; rMid = 0.026f; rTop = 0.012f;
                    break;

                case TreeType.Momiji:
                    p0 = new Vector3(0.000f, 0.026f, 0.000f);
                    p1 = new Vector3(0.020f, 0.092f, 0.008f);
                    p2 = new Vector3(-0.012f, 0.148f, -0.006f);
                    p3 = new Vector3(0.004f, 0.205f, 0.000f);
                    rBase = 0.035f; rMid = 0.021f; rTop = 0.011f;
                    break;

                default: // Sakura
                    p0 = new Vector3(0.000f, 0.026f, 0.000f);
                    p1 = new Vector3(-0.038f, 0.105f, 0.014f);
                    p2 = new Vector3(0.032f, 0.175f, -0.012f);
                    p3 = new Vector3(0.008f, 0.245f, 0.004f);
                    rBase = 0.038f; rMid = 0.022f; rTop = 0.010f;
                    break;
            }

            // Calcular a qué altura 'u' del tronco nace la rama objetivo
            float trunkHeightRange = Mathf.Max(0.05f, p3.y - p0.y);
            uBranchAttach = Mathf.Clamp((localBranchTip.y - p0.y) / trunkHeightRange, 0.42f, 0.88f);

            transform.localPosition = EvaluateTrunkAndBranchSurface(0f);
            BuildCaterpillarSegments();
        }

        public void Initialize(Vector3 trunkOrigin, Vector3 leafEnd)
        {
            if (transform.parent != null)
            {
                TreeType sp = ZenBonsaiManager.Instance != null ? ZenBonsaiManager.Instance.ActiveSpecies : TreeType.Sakura;
                InitializeOnTrunk(sp, transform.parent.InverseTransformPoint(leafEnd));
                return;
            }

            useLocalTrunkSpline = false;
            startPos = trunkOrigin + new Vector3(0.004f, 0.042f, -0.022f);
            targetPos = leafEnd + new Vector3(0f, 0.005f, -0.012f);
            Vector3 mid = (startPos + targetPos) * 0.5f;
            controlPos = new Vector3(mid.x, mid.y, Mathf.Min(startPos.z, targetPos.z) - 0.015f);
            transform.position = startPos;
            BuildCaterpillarSegments();
        }

        private void BuildCaterpillarSegments()
        {
            if (segHead != null) return;

            MeshRenderer baseRen = GetComponent<MeshRenderer>();
            if (baseRen != null) baseRen.enabled = false;

            Shader lit = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material bodyMat = new Material(lit);
            bodyMat.color = new Color(0.55f, 0.95f, 0.22f);
            if (bodyMat.HasProperty("_EmissionColor"))
            {
                bodyMat.EnableKeyword("_EMISSION");
                bodyMat.SetColor("_EmissionColor", new Color(0.32f, 0.68f, 0.12f));
            }

            Material headMat = new Material(lit);
            headMat.color = new Color(0.98f, 0.86f, 0.18f);
            if (headMat.HasProperty("_EmissionColor"))
            {
                headMat.EnableKeyword("_EMISSION");
                headMat.SetColor("_EmissionColor", new Color(0.60f, 0.46f, 0.08f));
            }

            // Tamaño miniatura realista para bonsái (~7 mm de grosor y ~1.7 cm de largo total)
            segHead = CreateSegment("Caterpillar_Head", new Vector3(0f, 0f, 0.0052f),  0.0076f, headMat);
            segMid  = CreateSegment("Caterpillar_Mid",  Vector3.zero,                 0.0068f, bodyMat);
            segTail = CreateSegment("Caterpillar_Tail", new Vector3(0f, 0f, -0.0052f), 0.0058f, bodyMat);
        }

        private Transform CreateSegment(string segName, Vector3 localPos, float diameter, Material mat)
        {
            GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            seg.name = segName;
            seg.transform.SetParent(transform, false);
            seg.transform.localPosition = localPos;
            seg.transform.localScale = Vector3.one * diameter;
            Collider c = seg.GetComponent<Collider>();
            if (c != null) { c.enabled = false; Destroy(c); }
            Renderer r = seg.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = mat;
            return seg.transform;
        }

        private Vector3 EvalCubicBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float u)
        {
            float inv = 1f - u;
            return inv * inv * inv * a
                 + 3f * inv * inv * u * b
                 + 3f * inv * u * u * c
                 + u * u * u * d;
        }

        private float EvalTrunkRadius(float u)
        {
            return u < 0.5f
                ? Mathf.Lerp(rBase, rMid, u * 2f)
                : Mathf.Lerp(rMid, rTop, (u - 0.5f) * 2f);
        }

        private Vector3 EvaluateTrunkSurfaceAt(float u)
        {
            Vector3 center = EvalCubicBezier(p0, p1, p2, p3, u);
            float rad = EvalTrunkRadius(u);
            // Pegada exactamente a la corteza frontal (-Z) del tronco (nunca en el aire ni dentro de la madera)
            return center + new Vector3(0f, 0f, -(rad + 0.0036f));
        }

        /// <summary>
        /// De t = 0..0.65 sube pegada a la corteza curva del tronco; de t = 0.65..1.0 camina por la rama hasta la nube de hojas.
        /// </summary>
        private Vector3 EvaluateTrunkAndBranchSurface(float t)
        {
            const float trunkPhase = 0.65f;
            if (t <= trunkPhase)
            {
                float u = Mathf.Lerp(0.10f, uBranchAttach, t / trunkPhase);
                return EvaluateTrunkSurfaceAt(u);
            }
            else
            {
                float branchT = (t - trunkPhase) / (1f - trunkPhase);
                Vector3 branchStart = EvaluateTrunkSurfaceAt(uBranchAttach);
                Vector3 branchMid = Vector3.Lerp(branchStart, localBranchTip, 0.5f) + new Vector3(0f, 0.005f, -0.004f);
                float inv = 1f - branchT;
                return inv * inv * branchStart + 2f * inv * branchT * branchMid + branchT * branchT * localBranchTip;
            }
        }

        private Vector3 EvaluateQuadraticBezier(float t)
        {
            float u = 1f - t;
            return u * u * startPos + 2f * u * t * controlPos + t * t * targetPos;
        }

        private void Update()
        {
            if (isDispelled) return;

            progress += Time.deltaTime * crawlSpeed;
            float clampedT = Mathf.Clamp01(progress);

            if (useLocalTrunkSpline)
            {
                Vector3 localPos = EvaluateTrunkAndBranchSurface(clampedT);
                Vector3 nextLocalPos = EvaluateTrunkAndBranchSurface(Mathf.Clamp01(clampedT + 0.025f));
                transform.localPosition = localPos;

                Vector3 localDir = nextLocalPos - localPos;
                if (localDir.sqrMagnitude > 0.0000001f)
                {
                    transform.localRotation = Quaternion.LookRotation(localDir.normalized, Vector3.back);
                }
            }
            else if (targetPos != Vector3.zero)
            {
                Vector3 pos = EvaluateQuadraticBezier(clampedT);
                Vector3 nextPos = EvaluateQuadraticBezier(Mathf.Clamp01(clampedT + 0.03f));
                transform.position = pos;
                Vector3 dir = nextPos - pos;
                if (dir.sqrMagnitude > 0.000001f)
                {
                    transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
                }
            }

            // Ondulación peristáltica a escala bonsái en sus 3 segmentos
            float wave = Time.time * 9.5f;
            if (segHead != null)
            {
                segHead.localPosition = new Vector3(0.0008f * Mathf.Sin(wave), 0.0009f * Mathf.Abs(Mathf.Cos(wave)), 0.0054f);
            }
            if (segMid != null)
            {
                segMid.localPosition = new Vector3(-0.0008f * Mathf.Sin(wave), 0.0018f * Mathf.Abs(Mathf.Sin(wave)), 0f);
            }
            if (segTail != null)
            {
                segTail.localPosition = new Vector3(0.0008f * Mathf.Cos(wave), 0.0009f * Mathf.Abs(Mathf.Sin(wave - 1f)), -0.0054f);
            }

            if (progress >= 1f)
            {
                if (ZenBonsaiManager.Instance != null)
                {
                    ZenBonsaiManager.Instance.OnPestReachedGoal();
                }
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Disipar la oruga al ser barrida con la escobilla Hoki o tocada con la mano.
        /// </summary>
        public void Dispel()
        {
            if (isDispelled) return;
            isDispelled = true;

            if (ZenSoundSynthesizer.Instance != null)
            {
                ZenSoundSynthesizer.Instance.PlayWindChime();
            }

            if (ZenVisualEffects.Instance != null)
            {
                ZenVisualEffects.Instance.SpawnSparkles(transform.position);
            }

            if (ZenBonsaiManager.Instance != null)
            {
                ZenBonsaiManager.Instance.OnPestDefeated();
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null || isDispelled) return;

            // Ignorar colisionadores del propio árbol, ramas, agua, botones y decoración
            if (other.GetComponent<PrunableBranch>() != null || other.GetComponentInParent<PrunableBranch>() != null) return;
            if (other.GetComponent<ZenWaterSource>() != null || other.GetComponentInParent<ZenWaterSource>() != null) return;
            if (other.GetComponent<ZenTouchButton>() != null || other.GetComponentInParent<ZenTouchButton>() != null) return;

            string n = other.name;
            if (n.Contains("Branch") || n.Contains("Leaf") || n.Contains("Trunk") ||
                n.Contains("Pot") || n.Contains("Moss") || n.Contains("Table") || n.Contains("Basin"))
            {
                return;
            }

            // Si es una herramienta (especialmente la escobilla) o una mano/dedo del jugador, disipar
            ZenBonsaiTool tool = other.GetComponentInParent<ZenBonsaiTool>();
            if (tool != null)
            {
                Dispel();
                return;
            }

            if (n.Contains("Hand") || n.Contains("Index") || n.Contains("Thumb") ||
                n.Contains("Finger") || n.Contains("Palm") || n.Contains("Bone") || n.Contains("Capsule"))
            {
                Dispel();
            }
        }
    }
}

