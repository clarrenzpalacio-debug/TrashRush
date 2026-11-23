using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace TrashRush.Audio
{
    public static class ProceduralAudioGenerator
    {
        public static void GenerateWav(string filePath, float[] samples, int sampleRate = 44100)
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                short channels = 1;
                short bitsPerSample = 16;
                int subChunk1Size = 16;
                int byteRate = sampleRate * channels * (bitsPerSample / 8);
                short blockAlign = (short)(channels * (bitsPerSample / 8));
                int dataSize = samples.Length * (bitsPerSample / 8);
                int chunkSize = 36 + dataSize;

                // RIFF chunk
                bw.Write(Encoding.ASCII.GetBytes("RIFF"));
                bw.Write(chunkSize);
                bw.Write(Encoding.ASCII.GetBytes("WAVE"));

                // fmt chunk
                bw.Write(Encoding.ASCII.GetBytes("fmt "));
                bw.Write(subChunk1Size);
                bw.Write((short)1); // PCM
                bw.Write(channels);
                bw.Write(sampleRate);
                bw.Write(byteRate);
                bw.Write(blockAlign);
                bw.Write(bitsPerSample);

                // data chunk
                bw.Write(Encoding.ASCII.GetBytes("data"));
                bw.Write(dataSize);

                for (int i = 0; i < samples.Length; i++)
                {
                    float clamped = Mathf.Clamp(samples[i], -1f, 1f);
                    short val = (short)(clamped * 32767f);
                    bw.Write(val);
                }
            }
        }

        // 1. Background Music (melodic peaceful upbeat loop)
        public static float[] CreateBGM(float duration = 16f, int sampleRate = 44100)
        {
            int totalSamples = (int)(duration * sampleRate);
            float[] samples = new float[totalSamples];

            // Chords: C - G - Am - F progression (4 beats each, 120 BPM -> 2s per chord, 8 chords total for 16s)
            float[] chordRootFreqs = { 261.63f, 196.00f, 220.00f, 174.61f, 261.63f, 196.00f, 220.00f, 174.61f };
            float chordDuration = 2.0f;

            // Arpeggio pattern offsets (semitones from root: 0, 4, 7, 12, 16, 12, 7, 4)
            int[] arpeggioPattern = { 0, 4, 7, 12, 16, 12, 7, 4, 0, 7, 12, 16, 19, 16, 12, 7 };
            float noteDuration = chordDuration / arpeggioPattern.Length; // 0.125s per note

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                int chordIdx = Mathf.Clamp((int)(t / chordDuration), 0, chordRootFreqs.Length - 1);
                float chordTime = t - chordIdx * chordDuration;

                // Bass note (warm triangle / sine)
                float bassFreq = chordRootFreqs[chordIdx] * 0.5f;
                float bass = Mathf.Sin(2f * Mathf.PI * bassFreq * t) * 0.25f;

                // Arpeggio note
                int noteIdx = Mathf.Clamp((int)(chordTime / noteDuration), 0, arpeggioPattern.Length - 1);
                float noteTime = chordTime - noteIdx * noteDuration;
                float semitone = arpeggioPattern[noteIdx];
                // For minor chords (Am at idx 2 and 6), use minor third
                if ((chordIdx == 2 || chordIdx == 6) && (semitone == 4 || semitone == 16)) semitone -= 1f;

                float noteFreq = chordRootFreqs[chordIdx] * Mathf.Pow(2f, semitone / 12f);
                float noteEnv = Mathf.Clamp01(1f - (noteTime / noteDuration) * 1.5f); // Pluck envelope
                float lead = (Mathf.Sin(2f * Mathf.PI * noteFreq * t) + 0.3f * Mathf.Sin(4f * Mathf.PI * noteFreq * t)) * noteEnv * 0.2f;

                // Gentle pad chord
                float pad1 = Mathf.Sin(2f * Mathf.PI * chordRootFreqs[chordIdx] * t);
                float pad2 = Mathf.Sin(2f * Mathf.PI * chordRootFreqs[chordIdx] * 1.5f * t); // 5th
                float pad = (pad1 + pad2) * 0.08f;

                // Soft marimba / percussion tick on beats
                float beatTime = t % 0.5f;
                float clickEnv = Mathf.Exp(-beatTime * 30f);
                float tick = ((float)new System.Random(i % 1000).NextDouble() * 2f - 1f) * clickEnv * 0.03f;

                // Master fade in / fade out at loop boundaries
                float loopFade = 1f;
                if (t < 0.1f) loopFade = t / 0.1f;
                else if (t > duration - 0.1f) loopFade = (duration - t) / 0.1f;

                samples[i] = (bass + lead + pad + tick) * loopFade * 0.6f;
            }

            return samples;
        }

        // 2. Pickup SFX (pleasant rising bubble pop)
        public static float[] CreatePickupSFX(float duration = 0.25f, int sampleRate = 44100)
        {
            int totalSamples = (int)(duration * sampleRate);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float p = t / duration;
                float freq = Mathf.Lerp(400f, 950f, p * p);
                float env = Mathf.Sin(p * Mathf.PI); // Smooth rise and fall
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                float harmonic = 0.3f * Mathf.Sin(4f * Mathf.PI * freq * t);
                samples[i] = (wave + harmonic) * env * 0.6f;
            }
            return samples;
        }

        // 3. Correct Sort SFX (rewarding bright two-tone major chime: C6 -> G6)
        public static float[] CreateCorrectSortSFX(float duration = 0.5f, int sampleRate = 44100)
        {
            int totalSamples = (int)(duration * sampleRate);
            float[] samples = new float[totalSamples];

            float f1 = 1046.50f; // C6
            float f2 = 1567.98f; // G6
            float mid = duration * 0.35f;

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float s = 0f;
                if (t < mid)
                {
                    float env = Mathf.Exp(-t * 8f);
                    s = Mathf.Sin(2f * Mathf.PI * f1 * t) * env;
                }
                else
                {
                    float t2 = t - mid;
                    float env = Mathf.Exp(-t2 * 6f);
                    s = (Mathf.Sin(2f * Mathf.PI * f2 * t2) + 0.3f * Mathf.Sin(4f * Mathf.PI * f2 * t2)) * env;
                }
                samples[i] = s * 0.7f;
            }
            return samples;
        }

        // 4. Incorrect Sort SFX (low soft dissonant buzz: F3 + F#3)
        public static float[] CreateIncorrectSortSFX(float duration = 0.45f, int sampleRate = 44100)
        {
            int totalSamples = (int)(duration * sampleRate);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Clamp01(1f - (t / duration));
                // Low dissonant tone with saw-like pulse
                float s1 = Mathf.Sin(2f * Mathf.PI * 140f * t);
                float s2 = Mathf.Sin(2f * Mathf.PI * 148f * t); // minor second dissonance
                float buzz = (t % (1f / 70f) > (0.5f / 70f)) ? 1f : -1f; // Square wave rasp
                samples[i] = (s1 * 0.4f + s2 * 0.4f + buzz * 0.2f) * env * 0.5f;
            }
            return samples;
        }

        // 5. Button Click SFX (crisp UI tick)
        public static float[] CreateButtonClickSFX(float duration = 0.08f, int sampleRate = 44100)
        {
            int totalSamples = (int)(duration * sampleRate);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 60f);
                float wave = Mathf.Sin(2f * Mathf.PI * 1200f * t) + 0.5f * Mathf.Sin(2f * Mathf.PI * 2400f * t);
                samples[i] = wave * env * 0.4f;
            }
            return samples;
        }

        // 6. Victory Jingle (fanfare C5 -> E5 -> G5 -> C6)
        public static float[] CreateVictoryJingle(float duration = 1.8f, int sampleRate = 44100)
        {
            int totalSamples = (int)(duration * sampleRate);
            float[] samples = new float[totalSamples];

            float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f };
            float[] noteTimes = { 0f, 0.25f, 0.5f, 0.85f };
            float[] noteDurs = { 0.22f, 0.22f, 0.32f, 0.95f };

            for (int n = 0; n < notes.Length; n++)
            {
                int startIdx = (int)(noteTimes[n] * sampleRate);
                int endIdx = Mathf.Min(startIdx + (int)(noteDurs[n] * sampleRate), totalSamples);
                float freq = notes[n];

                for (int i = startIdx; i < endIdx; i++)
                {
                    float noteT = (float)(i - startIdx) / sampleRate;
                    float env = Mathf.Exp(-noteT * (n == notes.Length - 1 ? 2.5f : 5.0f));
                    float s = (Mathf.Sin(2f * Mathf.PI * freq * noteT)
                             + 0.4f * Mathf.Sin(4f * Mathf.PI * freq * noteT)
                             + 0.2f * Mathf.Sin(6f * Mathf.PI * freq * noteT)) * env * 0.45f;
                    samples[i] += s;
                }
            }
            return samples;
        }

        // 7. Game Over Sound (descending sad trombone notes)
        public static float[] CreateGameOverSFX(float duration = 1.6f, int sampleRate = 44100)
        {
            int totalSamples = (int)(duration * sampleRate);
            float[] samples = new float[totalSamples];

            float[] notes = { 392.00f, 369.99f, 349.23f, 311.13f }; // G4 -> F#4 -> F4 -> Eb4
            float step = duration / 4f;

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                int n = Mathf.Clamp((int)(t / step), 0, 3);
                float noteT = t - n * step;
                float freq = notes[n];
                // Add gentle downward pitch bend to each note
                freq *= (1f - (noteT / step) * 0.05f);
                float env = Mathf.Sin(Mathf.PI * (noteT / step));
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t) + 0.3f * Mathf.Sin(4f * Mathf.PI * freq * t);
                samples[i] = wave * env * 0.4f;
            }
            return samples;
        }

        // 8. Footstep SFX (soft thud on grass)
        public static float[] CreateFootstepSFX(float duration = 0.12f, int sampleRate = 44100)
        {
            int totalSamples = (int)(duration * sampleRate);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 35f);
                float thud = Mathf.Sin(2f * Mathf.PI * 90f * t);
                float rustle = ((float)new System.Random(i % 500).NextDouble() * 2f - 1f) * 0.3f;
                samples[i] = (thud * 0.7f + rustle * 0.3f) * env * 0.4f;
            }
            return samples;
        }
    }
}
