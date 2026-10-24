using UnityEngine;

namespace ZenBonsai
{
    /// <summary>
    /// Generador de audio procedural para Zen Bonsai.
    /// Crea sonidos zen relajantes (campana tibetana, agua, tijeras, campanas de viento)
    /// mediante síntesis de ondas, 100% original y sin necesidad de archivos externos.
    /// </summary>
    public class ZenSoundSynthesizer : MonoBehaviour
    {
        public static ZenSoundSynthesizer Instance { get; private set; }

        private AudioSource audioSource;
        private AudioSource ambientSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0.5f;

            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.playOnAwake = false;
            ambientSource.loop = true;
            ambientSource.volume = 0.22f;

            SetMusicStyle(0);
        }

        private int currentMusicStyle = 0;
        public int CurrentMusicStyle => currentMusicStyle;

        public void CycleMusicStyle()
        {
            SetMusicStyle((currentMusicStyle + 1) % 7);
        }

        public void SetMusicStyle(int style)
        {
            currentMusicStyle = Mathf.Clamp(style, 0, 6);
            if (ambientSource == null) return;

            if (currentMusicStyle == 6)
            {
                ambientSource.Stop();
                return;
            }

            AudioClip clip = null;
            switch (currentMusicStyle)
            {
                case 0:
                    clip = GenerateShakuhachiKotoMelody();
                    ambientSource.volume = 0.22f;
                    break;
                case 1:
                    clip = GenerateImperialKotoHarp();
                    ambientSource.volume = 0.24f;
                    break;
                case 2:
                    clip = GenerateTibetanSingingBowls();
                    ambientSource.volume = 0.24f;
                    break;
                case 3:
                    clip = GenerateKyotoRainAndShishiOdoshi();
                    ambientSource.volume = 0.20f;
                    break;
                case 4:
                    clip = GenerateWaterfallNatureAmbience();
                    ambientSource.volume = 0.18f;
                    break;
                case 5:
                    clip = GenerateTempleFurinAndGong();
                    ambientSource.volume = 0.22f;
                    break;
            }

            if (clip != null)
            {
                ambientSource.clip = clip;
                ambientSource.Play();
            }
        }

        /// <summary>
        /// Sonido de cuenco tibetano o campana zen (para eventos relajantes o armonía).
        /// </summary>
        public void PlayTibetanBell(float baseFreq = 432f, float duration = 3.0f, float volume = 0.7f)
        {
            AudioClip clip = GenerateHarmonicBell(baseFreq, duration);
            audioSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Sonido de gota de agua o rocío hidratante.
        /// </summary>
        public void PlayWaterDrop(float volume = 0.6f)
        {
            AudioClip clip = GenerateWaterDrop();
            audioSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Sonido de tijera de podar (corte limpio de rama seca).
        /// </summary>
        public void PlaySnip(float volume = 0.5f)
        {
            AudioClip clip = GenerateSnip();
            audioSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Sonido de golpe seco de madera zen (Hyoshigi / bloque de templo) para botones táctiles.
        /// </summary>
        public void PlayWoodClack(float volume = 0.6f)
        {
            AudioClip clip = GenerateWoodClack();
            audioSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Sonido de campanillas de viento pentatónicas (disolver plagas o floración).
        /// </summary>
        public void PlayWindChime(float volume = 0.6f)
        {
            AudioClip clip = GenerateWindChime();
            audioSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Sonido continuo o suave de regadera vertiendo agua (lluvia fina sobre bonsái).
        /// </summary>
        public void PlayWateringStream(float volume = 0.5f)
        {
            AudioClip clip = GenerateWateringStream();
            audioSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Sonido de cepillado suave del musgo y hojas (Bonsai Broom).
        /// </summary>
        public void PlayBroomSweep(float volume = 0.55f)
        {
            AudioClip clip = GenerateBroomSweep();
            audioSource.PlayOneShot(clip, volume);
        }

        private AudioClip GenerateShakuhachiKotoMelody()
        {
            int sampleRate = 44100;
            float duration = 12.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            // Escala japonesa pentatónica Insen (Re, Mib, Sol, La, Do): 293.66, 311.13, 392.00, 440.00, 523.25
            float[] scale = { 293.66f, 311.13f, 392.00f, 440.00f, 523.25f, 587.33f };
            float[] noteTimes = { 0.4f, 2.2f, 4.0f, 5.8f, 7.6f, 9.4f, 10.8f };
            int[] notePitches = { 0, 2, 3, 1, 4, 3, 2 };

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float sample = 0f;

                // Drone armónico de fondo (Koto continuo suave)
                float kotoBass = Mathf.Sin(2f * Mathf.PI * 146.83f * t) * 0.05f +
                                 Mathf.Sin(2f * Mathf.PI * 220.00f * t) * 0.03f;
                sample += kotoBass;

                // Melodía de flauta de bambú Shakuhachi con vibrato y aire suave
                for (int n = 0; n < noteTimes.Length; n++)
                {
                    float noteStart = noteTimes[n];
                    float noteDuration = 1.6f;
                    if (t >= noteStart && t < noteStart + noteDuration)
                    {
                        float localT = t - noteStart;
                        float freq = scale[notePitches[n]];
                        float vibrato = 1f + 0.012f * Mathf.Sin(2f * Mathf.PI * 4.5f * localT);
                        float breathNoise = (Random.value * 2f - 1f) * 0.04f;
                        float fluteWave = Mathf.Sin(2f * Mathf.PI * (freq * vibrato) * localT);
                        float harmonic = Mathf.Sin(4f * Mathf.PI * (freq * vibrato) * localT) * 0.25f;
                        float envelope = Mathf.Sin(Mathf.PI * (localT / noteDuration));
                        sample += (fluteWave + harmonic + breathNoise) * envelope * 0.18f;
                    }
                }

                samples[i] = Mathf.Clamp(sample, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create("ShakuhachiKotoMelody", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateTibetanSingingBowls()
        {
            int sampleRate = 44100;
            float duration = 10.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float lfo1 = 0.5f + 0.5f * Mathf.Sin(2f * Mathf.PI * 0.15f * t);
                float lfo2 = 0.5f + 0.5f * Mathf.Sin(2f * Mathf.PI * 0.25f * t);

                float bowl1 = (Mathf.Sin(2f * Mathf.PI * 216f * t) * 0.14f +
                               Mathf.Sin(2f * Mathf.PI * 432f * t) * 0.08f +
                               Mathf.Sin(2f * Mathf.PI * 648f * t) * 0.03f) * lfo1;

                float bowl2 = (Mathf.Sin(2f * Mathf.PI * 264f * t) * 0.12f +
                               Mathf.Sin(2f * Mathf.PI * 528f * t) * 0.07f) * lfo2;

                samples[i] = (bowl1 + bowl2);
            }

            AudioClip clip = AudioClip.Create("TibetanBowls", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateWaterfallNatureAmbience()
        {
            int sampleRate = 44100;
            float duration = 8.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            float filter = 0f;
            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float white = (Random.value * 2f - 1f);
                filter = Mathf.Lerp(filter, white, 0.08f);

                // Modulación de agua fluyendo en arroyo de piedras
                float ripple = Mathf.Sin(2f * Mathf.PI * 0.4f * t) * 0.05f;
                float brook = (filter * 0.14f) * (1f + ripple);

                // Tintineo ocasional de gota zen
                float chime = 0f;
                if ((i % (sampleRate * 2)) < (sampleRate * 0.3f))
                {
                    float ct = (float)(i % (sampleRate * 2)) / sampleRate;
                    chime = Mathf.Sin(2f * Mathf.PI * 1200f * ct) * Mathf.Exp(-8f * ct) * 0.04f;
                }

                samples[i] = brook + chime;
            }

            AudioClip clip = AudioClip.Create("WaterfallNature", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateImperialKotoHarp()
        {
            int sampleRate = 44100;
            float duration = 12.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            // Escala japonesa Hirajoshi (La, Si, Do, Mi, Fa, La octava): 220, 246.94, 261.63, 329.63, 349.23, 440, 493.88, 523.25
            float[] kotoScale = { 220.00f, 246.94f, 261.63f, 329.63f, 349.23f, 440.00f, 493.88f, 523.25f };
            float[] pluckTimes = { 0.2f, 1.0f, 1.8f, 2.8f, 3.8f, 4.6f, 5.4f, 6.6f, 7.6f, 8.4f, 9.4f, 10.6f };
            int[] pluckNotes = { 0, 2, 3, 5, 4, 3, 2, 0, 3, 5, 7, 5 };

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float sample = Mathf.Sin(2f * Mathf.PI * 110.0f * t) * 0.04f; // Bordón grave de seda

                for (int p = 0; p < pluckTimes.Length; p++)
                {
                    float start = pluckTimes[p];
                    if (t >= start && t < start + 2.4f)
                    {
                        float lt = t - start;
                        float f = kotoScale[pluckNotes[p]];
                        // Cuerda pulsada con plectro de marfil (ataque rápido y caída resonante cálida)
                        float env = Mathf.Exp(-2.6f * lt) * (1f - Mathf.Exp(-140f * lt));
                        float wave = Mathf.Sin(2f * Mathf.PI * f * lt) * 0.65f +
                                     Mathf.Sin(4f * Mathf.PI * f * lt) * 0.24f +
                                     Mathf.Sin(6f * Mathf.PI * f * lt) * 0.11f;
                        sample += wave * env * 0.24f;
                    }
                }
                samples[i] = Mathf.Clamp(sample, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create("ImperialKotoHarp", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateKyotoRainAndShishiOdoshi()
        {
            int sampleRate = 44100;
            float duration = 10.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            float lp1 = 0f;
            float lp2 = 0f;
            float[] bambooStrikes = { 2.4f, 7.4f };

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                // Lluvia suave filtrada sobre hojas y musgo
                float noise = (Random.value * 2f - 1f);
                lp1 = Mathf.Lerp(lp1, noise, 0.14f);
                lp2 = Mathf.Lerp(lp2, lp1, 0.09f);
                float rain = lp2 * 0.12f * (0.9f + 0.1f * Mathf.Sin(2f * Mathf.PI * 0.2f * t));

                // Golpe hueco de bambú con agua (Shishi-odoshi)
                float clack = 0f;
                for (int b = 0; b < bambooStrikes.Length; b++)
                {
                    float st = bambooStrikes[b];
                    if (t >= st && t < st + 0.35f)
                    {
                        float lt = t - st;
                        float env = Mathf.Exp(-22f * lt);
                        clack += (Mathf.Sin(2f * Mathf.PI * 390f * lt) * 0.7f +
                                  Mathf.Sin(2f * Mathf.PI * 760f * lt) * 0.3f) * env * 0.22f;
                    }
                }

                samples[i] = Mathf.Clamp(rain + clack, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create("KyotoRainShishiOdoshi", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateTempleFurinAndGong()
        {
            int sampleRate = 44100;
            float duration = 10.0f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            float[] furinTimes = { 0.8f, 2.5f, 4.7f, 6.3f, 8.2f };
            float[] furinFreqs = { 1567.98f, 1760.00f, 2093.00f, 1567.98f, 2349.32f };

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                // Gong budista grave de fondo (Bonsho)
                float gongEnv = Mathf.Exp(-0.35f * (t % 5.0f)) * (1f - Mathf.Exp(-25f * (t % 5.0f)));
                float gong = (Mathf.Sin(2f * Mathf.PI * 98.0f * t) * 0.6f +
                              Mathf.Sin(2f * Mathf.PI * 147.0f * t) * 0.25f +
                              Mathf.Sin(2f * Mathf.PI * 272.0f * t) * 0.15f) * gongEnv * 0.14f;

                // Campanillas de viento de cristal japonés (Edo Furin)
                float furin = 0f;
                for (int f = 0; f < furinTimes.Length; f++)
                {
                    if (t >= furinTimes[f] && t < furinTimes[f] + 1.8f)
                    {
                        float lt = t - furinTimes[f];
                        float freq = furinFreqs[f];
                        float env = Mathf.Exp(-3.2f * lt);
                        furin += (Mathf.Sin(2f * Mathf.PI * freq * lt) +
                                  Mathf.Sin(2f * Mathf.PI * (freq * 2.41f) * lt) * 0.28f) * env * 0.08f;
                    }
                }

                samples[i] = Mathf.Clamp(gong + furin, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create("TempleFurinAndGong", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateHarmonicBell(float freq, float duration)
        {
            int sampleRate = 44100;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            float[] harmonics = { 1.0f, 2.76f, 5.4f, 8.9f };
            float[] weights = { 0.6f, 0.25f, 0.12f, 0.05f };

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float decay = Mathf.Exp(-3.5f * (t / duration));
                float sample = 0f;

                for (int h = 0; h < harmonics.Length; h++)
                {
                    sample += Mathf.Sin(2f * Mathf.PI * freq * harmonics[h] * t) * weights[h];
                }

                samples[i] = sample * decay;
            }

            AudioClip clip = AudioClip.Create("ZenBell", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateWaterDrop()
        {
            int sampleRate = 44100;
            float duration = 0.35f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                // Modulación de tono ascendente típica de gota de agua
                float currentFreq = Mathf.Lerp(400f, 1600f, Mathf.Pow(t / duration, 0.5f));
                float env = Mathf.Sin(Mathf.PI * (t / duration)) * Mathf.Exp(-6f * (t / duration));
                samples[i] = Mathf.Sin(2f * Mathf.PI * currentFreq * t) * env;
            }

            AudioClip clip = AudioClip.Create("WaterDrop", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateSnip()
        {
            int sampleRate = 44100;
            float duration = 0.12f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float noise = (Random.value * 2f - 1f) * 0.4f;
                float click = Mathf.Sin(2f * Mathf.PI * 2400f * t) * 0.6f;
                float env = Mathf.Exp(-35f * t);
                samples[i] = (noise + click) * env;
            }

            AudioClip clip = AudioClip.Create("Snip", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateWindChime()
        {
            int sampleRate = 44100;
            float duration = 1.8f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            // Escala pentatónica japonesa (Insen / Hirajoshi): 523Hz (C5), 587Hz (D5), 659Hz (E5), 784Hz (G5), 880Hz (A5), 1046Hz (C6)
            float[] chimeFreqs = { 523.25f, 659.25f, 783.99f, 1046.5f };

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float sum = 0f;

                for (int c = 0; c < chimeFreqs.Length; c++)
                {
                    float noteOffset = c * 0.12f;
                    if (t >= noteOffset)
                    {
                        float noteT = t - noteOffset;
                        float decay = Mathf.Exp(-5f * noteT);
                        sum += Mathf.Sin(2f * Mathf.PI * chimeFreqs[c] * noteT) * decay * 0.25f;
                    }
                }

                samples[i] = sum;
            }

            AudioClip clip = AudioClip.Create("WindChime", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateWoodClack()
        {
            int sampleRate = 44100;
            float duration = 0.09f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float body = Mathf.Sin(2f * Mathf.PI * 480f * t) * 0.7f + Mathf.Sin(2f * Mathf.PI * 960f * t) * 0.3f;
                float transient = (Random.value * 2f - 1f) * 0.5f;
                float env = Mathf.Exp(-45f * t);
                samples[i] = (body + transient) * env;
            }

            AudioClip clip = AudioClip.Create("WoodClack", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateWateringStream()
        {
            int sampleRate = 44100;
            float duration = 0.8f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            float lastNoise = 0f;
            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                // Ruido rosa filtrado que simula el susurro de agua cayendo en lluvia fina
                float white = Random.value * 2f - 1f;
                lastNoise = Mathf.Lerp(lastNoise, white, 0.25f);
                float ripple = Mathf.Sin(2f * Mathf.PI * 650f * t) * 0.15f;
                float env = Mathf.Sin(Mathf.PI * (t / duration));
                samples[i] = (lastNoise * 0.45f + ripple) * env;
            }

            AudioClip clip = AudioClip.Create("WaterStream", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip GenerateBroomSweep()
        {
            int sampleRate = 44100;
            float duration = 0.3f;
            int totalSamples = (int)(sampleRate * duration);
            float[] samples = new float[totalSamples];

            float filter = 0f;
            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float white = (Random.value * 2f - 1f) * 0.6f;
                filter = Mathf.Lerp(filter, white, 0.15f);
                float env = Mathf.Sin(Mathf.PI * (t / duration)) * Mathf.Exp(-2f * (t / duration));
                samples[i] = filter * env;
            }

            AudioClip clip = AudioClip.Create("BroomSweep", totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
