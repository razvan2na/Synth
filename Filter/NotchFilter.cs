using System;

namespace Synth.Filter {
    public class NotchFilter : Filter {
        public NotchFilter(float sampleRate, float center, float bandwidth) {
            var w0 = 2 * Math.PI * center / sampleRate;
            var cosw0 = Math.Cos(w0);
            var sinw0 = Math.Sin(w0);
            var alpha = sinw0 / (2 * bandwidth);
            
            // b0 = 1
            var b1 = -2 * cosw0;
            // b2 = 1
            var aa0 = 1 + alpha;
            var aa1 = -2 * cosw0;
            var aa2 = 1 - alpha;
            
            A0 = 1 / aa0;
            A1 = b1 / aa0;
            A2 = 1 / aa0;
            A3 = aa1 / aa0;
            A4 = aa2 / aa0;
        }
    }
}