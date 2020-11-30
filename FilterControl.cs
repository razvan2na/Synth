using System;
using NAudio.Wave;
using Synth.Filter;

namespace Synth {
    public class FilterControl : ISampleProvider {
        public WaveFormat WaveFormat => source.WaveFormat;

        private readonly ISampleProvider source;
        private readonly Filter.Filter filter;

        public FilterControl(ISampleProvider source, FilterType type, int frequency = 3000, float bandwidth = 0.3f) {
            this.source = source;
            switch (type) {
                case FilterType.LowPass:
                    filter = new Filter.LowPassFilter(WaveFormat.SampleRate, frequency, bandwidth);
                    break;
                case FilterType.HighPass:
                    filter = new HighPassFilter(WaveFormat.SampleRate, frequency, bandwidth);
                    break;
                case FilterType.Notch:
                    filter = new NotchFilter(WaveFormat.SampleRate, frequency, bandwidth);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
		
        public int Read(float[] buffer, int offset, int count) {
            var samples = source.Read(buffer, offset, count);

            for (var i = 0; i < samples; i++) {
                buffer[offset + i] = filter.Transform(buffer[offset + i]);
            }
			
            return samples;
        }
    }
}