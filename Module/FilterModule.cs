using System;
using NAudio.Wave;
using Synth.Filter;

namespace Synth.Module {
    public class FilterModule : ISampleProvider {
        public WaveFormat WaveFormat => source.WaveFormat;

        private readonly ISampleProvider source;
        private readonly Filter.Filter filter;

        public FilterModule(ISampleProvider source, FilterType type = FilterType.LowPass, int frequency = 3000,
            float bandwidth = 0.3f) {
            this.source = source;
            filter = type switch {
                FilterType.LowPass => new LowPassFilter(WaveFormat.SampleRate, frequency, bandwidth),
                FilterType.HighPass => new HighPassFilter(WaveFormat.SampleRate, frequency, bandwidth),
                FilterType.Notch => new NotchFilter(WaveFormat.SampleRate, frequency, bandwidth),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
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