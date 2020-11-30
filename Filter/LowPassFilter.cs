using System;

namespace Synth.Filter {
    public class LowPassFilter : Filter {
        public LowPassFilter(float sampleRate, float cutoff, float bandwidth) {
            var w0 = 2 * Math.PI * cutoff / sampleRate;
            var cosw0 = Math.Cos(w0);
            var alpha = Math.Sin(w0) / (2 * bandwidth);

            var b0 = (1 - cosw0) / 2;
            var b1 = 1 - cosw0;
            var b2 = (1 - cosw0) / 2;
            var aa0 = 1 + alpha;
            var aa1 = -2 * cosw0;
            var aa2 = 1 - alpha;

            A0 = b0 / aa0;
            A1 = b1 / aa0;
            A2 = b2 / aa0;
            A3 = aa1 / aa0;
            A4 = aa2 / aa0;
        }
    }
}