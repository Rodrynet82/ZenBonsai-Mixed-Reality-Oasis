using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Generador de efectos visuales y partículas zen (gotas de agua, hojas al podar, chispas de plagas).
    /// </summary>
    public class ZenVisualEffects : MonoBehaviour
    {
        public static ZenVisualEffects Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Efecto al podar una rama: pequeñas hojas que caen y se desvanecen.
        /// </summary>
        public void SpawnPruneLeaves(Vector3 position)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Quad);
                leaf.transform.position = position + Random.insideUnitSphere * 0.04f;
                leaf.transform.localScale = Vector3.one * 0.02f;
                var col = leaf.GetComponent<Collider>();
                if (col != null) Destroy(col);

                var ren = leaf.GetComponent<Renderer>();
                if (ren != null) ren.material.color = new Color(0.85f, 0.7f, 0.2f);

                Rigidbody rb = leaf.AddComponent<Rigidbody>();
                rb.mass = 0.01f;
                rb.linearDamping = 3f; // Resistencia del aire suave
                rb.AddForce(Random.insideUnitSphere * 0.05f, ForceMode.Impulse);

                Destroy(leaf, 1.5f);
            }
        }

        private float nextWaterDropTime = 0f;

        /// <summary>
        /// Desactivado para garantizar que el único chorro de agua salga exclusivamente del cuenco del cazo Hishaku.
        /// </summary>
        public void SpawnWaterDroplets(Vector3 position, Vector3 targetPot)
        {
            // No-op intencional: evita que exista un segundo chorro desde la fuente Tsukubai
        }

        /// <summary>
        /// Efecto al disipar una plaga: estallido de chispitas doradas.
        /// </summary>
        public void SpawnSparkles(Vector3 position)
        {
            for (int i = 0; i < 8; i++)
            {
                GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spark.transform.position = position;
                spark.transform.localScale = Vector3.one * 0.010f;
                var col = spark.GetComponent<Collider>();
                if (col != null) { col.enabled = false; Destroy(col); }

                var ren = spark.GetComponent<Renderer>();
                if (ren != null) ren.material.color = new Color(1f, 0.9f, 0.4f);

                Rigidbody rb = spark.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.linearDamping = 2f;
                rb.AddForce(Random.insideUnitSphere * 0.25f, ForceMode.Impulse);

                Destroy(spark, 0.7f);
            }
        }

        private LineRenderer activeWaterStreamLine;
        private float waterStreamActiveUntil = 0f;
        private Material waterStreamMat;

        private void EnsureWaterStreamRenderer()
        {
            if (activeWaterStreamLine != null) return;

            GameObject streamObj = new GameObject("Zen_Hishaku_WaterStreamLine");
            streamObj.transform.SetParent(transform, false);

            activeWaterStreamLine = streamObj.AddComponent<LineRenderer>();
            activeWaterStreamLine.positionCount = 8;
            activeWaterStreamLine.startWidth = 0.007f;
            activeWaterStreamLine.endWidth = 0.012f;
            activeWaterStreamLine.useWorldSpace = true;
            activeWaterStreamLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            activeWaterStreamLine.receiveShadows = false;

            Shader s = Shader.Find("Universal Render Pipeline/Unlit");
            if (s == null) s = Shader.Find("Unlit/Color");
            if (s == null) s = Shader.Find("Sprites/Default");
            waterStreamMat = new Material(s);
            waterStreamMat.color = new Color(0.48f, 0.88f, 1.0f, 0.85f);
            if (waterStreamMat.HasProperty("_BaseColor"))
                waterStreamMat.SetColor("_BaseColor", new Color(0.48f, 0.88f, 1.0f, 0.85f));
            activeWaterStreamLine.material = waterStreamMat;

            activeWaterStreamLine.enabled = false;
        }

        private void Update()
        {
            if (activeWaterStreamLine != null && activeWaterStreamLine.enabled)
            {
                if (Time.time > waterStreamActiveUntil)
                {
                    activeWaterStreamLine.enabled = false;
                }
            }
        }

        /// <summary>
        /// Efecto al verter con el cazo de bambú Hishaku: un ÚNICO hilo continuo y cristalino
        /// que une de forma permanente el cuenco de bambú con el musgo del bonsái.
        /// </summary>
        public void SpawnWaterStream(Vector3 spoutPos, Vector3 targetPotSoil)
        {
            EnsureWaterStreamRenderer();
            waterStreamActiveUntil = Time.time + 0.09f;
            activeWaterStreamLine.enabled = true;

            int count = activeWaterStreamLine.positionCount;
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / (count - 1);
                Vector3 pt = Vector3.Lerp(spoutPos, targetPotSoil, t);
                pt.y = Mathf.Lerp(spoutPos.y, targetPotSoil.y, t * t);
                float shimmer = Mathf.Sin(Time.time * 26f + i * 1.4f) * 0.0018f * (1f - t);
                pt.x += shimmer;
                activeWaterStreamLine.SetPosition(i, pt);
            }
        }

        /// <summary>
        /// Efecto al barrer con el pincel zen: suave halo de polvo purificador y hojas.
        /// </summary>
        public void SpawnBroomDust(Vector3 position)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject dust = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                dust.transform.position = position + Random.insideUnitSphere * 0.03f;
                dust.transform.localScale = Vector3.one * 0.008f;
                var col = dust.GetComponent<Collider>();
                if (col != null) Destroy(col);

                var ren = dust.GetComponent<Renderer>();
                if (ren != null) ren.material.color = new Color(0.8f, 0.95f, 0.8f, 0.7f);

                Rigidbody rb = dust.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.linearDamping = 3f;
                rb.linearVelocity = Vector3.up * 0.15f + Random.insideUnitSphere * 0.1f;

                Destroy(dust, 0.5f);
            }
        }
    }
}
