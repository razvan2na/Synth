using System;
using NAudio.Wave;

namespace Synth.Module {
	public enum SignalType {
		Sine, Square, Triangle, Sawtooth
	}
	
	public class SignalModule : ISampleProvider
    {
	    public WaveFormat WaveFormat { get; }
	    public double Frequency { get; set; }
	    public SignalType Type { get; set; }

	    private readonly double sineAngular;
	    private readonly double angular;
	    private int nSample;

	    public SignalModule(SignalType type = SignalType.Sine, double frequency = 440f) {
	        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);

	        Type = type;
	        Frequency = frequency;

	        sineAngular = 2 * Math.PI * frequency / WaveFormat.SampleRate;
	        angular = 2 * frequency / WaveFormat.SampleRate;
	    }
        
        public int Read(float[] buffer, int offset, int count) {
	        for (var i = 0; i < count / WaveFormat.Channels; i++, nSample++) {
	            var sample = Type switch {
		            SignalType.Sine => Sine(nSample),
		            SignalType.Square => Square(nSample),
		            SignalType.Triangle => Triangle(nSample),
		            SignalType.Sawtooth => Sawtooth(nSample),
		            _ => 0
	            };
	            
		        buffer[offset++] = (float) sample;
            }
            
            return count;
        }

        private double Sine(int i) {
	        return Math.Sin(i * sineAngular);
        }

        private double Square(int i) {
	        return Math.Sign(((i * angular) % 2) - 1);
        }

        private double Triangle(int i) {
	        var sample = 2 * ((i * angular) % 2);
	                    
	        if (sample > 1)
		        sample = 2 - sample;
	        
	        if (sample < -1)
		        sample = -2 - sample;

	        return sample;
        }

        private double Sawtooth(int i) {
	        return ((i * angular) % 2) - 1;
        }
    }
}