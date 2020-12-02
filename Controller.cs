using System.Collections.Generic;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Synth.Filter;
using Synth.Module;

namespace Synth {
	public class Controller {
		public bool Osc1Enable {
			set {
				osc1Enable = value;
				CheckEnable();
			}
		}

		public bool Osc2Enable {
			set {
				osc2Enable = value;
				CheckEnable();
			}
		}

		public bool FilterEnable { get; set; }
		public float Osc1Volume {
			get => volumeControl1?.Volume ?? 0.25f;
			set => volumeControl1.Volume = value;
		}

		public float Osc2Volume {
			get => volumeControl2?.Volume ?? 0.25f;
			set => volumeControl2.Volume = value;
		}

		public SignalType Osc1Waveform { get; set; } = SignalType.Sine;

		public SignalType Osc2Waveform { get; set; } = SignalType.Sine;

		public FilterType FilterType { get; set; } = FilterType.LowPass;

		public int Osc1Octave { get; set; } = 3;

		public int Osc2Octave { get; set; } = 3;

		public float Attack { get; set; } = 0.01f;

		public float Decay { get; set; }

		public float Sustain { get; set; } = 1;

		public float Release { get; set; } = 0.3f;

		public int Cutoff { get; set; } = 8000;

		public float Bandwidth { get; set; } = 0.5f;

		private bool osc1Enable;
		private bool osc2Enable;

		private readonly EnvelopeModule[,] signals;
		private readonly MixingSampleProvider mixer1;
		private readonly MixingSampleProvider mixer2;
		private readonly VolumeModule volumeControl1;
		private readonly VolumeModule volumeControl2;
		private readonly MixingSampleProvider mixerAll;
		private readonly DelayModule delayModule;
		private IWavePlayer player;
		
		private readonly List<double> frequencies = new List<double> {
			16.35, 17.32, 18.35, 19.45, 20.60, 21.83, 23.12, 24.50, 25.96, 27.50, 29.14, 30.87, 
			32.70, 34.65, 36.71, 38.89,	41.20, 43.65, 46.25, 49.00,	51.91, 55.00, 58.27, 61.74,
			65.41, 69.30, 73.42, 77.78, 82.41, 87.31, 92.50, 98.00, 103.83, 110.00, 116.54, 123.47,
			130.81, 138.59, 146.83, 155.56,	164.81,	174.61,	185.00,	196.00,	207.65,	220.00,	233.08,	246.94,
			261.63,	277.18,	293.66,	311.13,	329.63,	349.23,	369.99,	392.00,	415.30,	440.00,	466.16,	493.88,
			523.25,	554.37,	587.33,	622.25,	659.25,	698.46,	739.99,	783.99,	830.61,	880.00,	932.33,	987.77,
			1046.50, 1108.73, 1174.66, 1244.51, 1318.51, 1396.91, 1479.98, 1567.98, 1661.22, 1760.00, 1864.66, 1975.53,
			2093.00, 2217.46, 2349.32, 2489.02, 2637.02, 2793.83, 2959.96, 3135.96, 3322.44, 3520.00, 3729.31, 3951.07,
			4186.01, 4434.92, 4698.63, 4978.03, 5274.04, 5587.65, 5919.91, 6271.93, 6644.88, 7040.00, 7458.62, 7902.13
		};

		public Controller() {
			var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
			signals = new EnvelopeModule[2, 24];
			mixer1 = new MixingSampleProvider(waveFormat) { ReadFully = true };
			mixer2 = new MixingSampleProvider(waveFormat) { ReadFully = true };
			volumeControl1 = new VolumeModule(mixer1);
			volumeControl2 = new VolumeModule(mixer2);
			mixerAll = new MixingSampleProvider(waveFormat) { ReadFully = true };
			delayModule = new DelayModule(mixerAll);

			mixerAll.AddMixerInput(volumeControl1);
			mixerAll.AddMixerInput(volumeControl2);

			Osc1Volume = 0.25f;
			Osc2Volume = 0.25f;
		}

		public void NoteDown(int keyIndex) {
			if (keyIndex < 0 || signals[0, keyIndex] != null || signals[1, keyIndex] != null) 
				return;
			
			signals[0, keyIndex] = new EnvelopeModule(
				new SignalModule(Osc1Waveform, frequencies[keyIndex + Osc1Octave * 12]),
				Attack, Decay, Sustain, Release);
			signals[1, keyIndex] = new EnvelopeModule(
				new SignalModule(Osc2Waveform, frequencies[keyIndex + Osc2Octave * 12]),
				Attack, Decay, Sustain, Release);

			if (osc1Enable) {
				ISampleProvider input1 = signals[0, keyIndex];
				if (FilterEnable)
					input1 = new FilterModule(input1, FilterType, Cutoff, Bandwidth);
				
				mixer1.AddMixerInput(input1);
			}

			if (osc2Enable) {
				ISampleProvider input2 = signals[1, keyIndex];
				if (FilterEnable)
					input2 = new FilterModule(input2, FilterType, Cutoff, Bandwidth);
				
				mixer2.AddMixerInput(input2);
			}
		}

		public void NoteUp(int keyIndex) {
			if (keyIndex < 0 || signals[0, keyIndex] == null || signals[1, keyIndex] == null)
				return;
			
			signals[0, keyIndex].Stop();
			signals[1, keyIndex].Stop();
			signals[0, keyIndex] = null;
			signals[1, keyIndex] = null;
		}

		private void CheckEnable() {
			if (osc1Enable || osc2Enable)
				EnablePlayer();
			else
				DisablePlayer();
		}

		private void EnablePlayer() {
			if (player != null)
				return;
			
			player = new WaveOutEvent { NumberOfBuffers = 2, DesiredLatency = 100 };
			player.Init(new SampleToWaveProvider(delayModule));
			player.Play();
		}

		private void DisablePlayer() {
			if (player == null)
				return;
			
			player.Dispose();
			player = null;
		}
	}
}