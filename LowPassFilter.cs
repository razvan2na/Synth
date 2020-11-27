using NAudio.Dsp;
using NAudio.Wave;

namespace Synth {
	public class LowPassFilter : ISampleProvider {
		public WaveFormat WaveFormat => source.WaveFormat;

		private readonly ISampleProvider source;
		private readonly BiQuadFilter filter;

		public LowPassFilter(ISampleProvider source, int cutoff = 3000, float q = 0.3f) {
			this.source = source;
			filter = BiQuadFilter.LowPassFilter(source.WaveFormat.SampleRate, cutoff, q);
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