using System;

namespace Synth.Filter {
    public enum FilterType {
        LowPass, HighPass, Notch
    }

    public abstract class Filter {
        protected double A0;
        protected double A1;
        protected double A2;
        protected double A3;
        protected double A4;

        private float x1;
        private float x2;
        private float y1;
        private float y2;

        public float Transform(float inSample) {
            var result = A0 * inSample + A1 * x1 + A2 * x2 - A3 * y1 - A4 * y2;
            
            x2 = x1;
            x1 = inSample;
            
            y2 = y1;
            y1 = (float) result;

            return y1;
        }
    }
}