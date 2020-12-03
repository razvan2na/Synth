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

		public bool FilterEnable {
			get => filterModule?.Enabled ?? false;
			set => filterModule.Enabled = value;
		}

		public bool TremoloEnable {
			get => tremoloModule?.Enabled ?? false;
			set => tremoloModule.Enabled = value;
		}

		public bool DelayEnable {
			get => delayModule?.Enabled ?? false;
			set => delayModule.Enabled = value;
		}
		
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

		public FilterType FilterType {
			get => filterModule?.Type ?? FilterType.LowPass;
			set => filterModule.Type = value;
		}

		public int Osc1Octave { get; set; } = 3;

		public int Osc2Octave { get; set; } = 3;

		public float Attack { get; set; } = 0.01f;

		public float Decay { get; set; }

		public float Sustain { get; set; } = 1;

		public float Release { get; set; } = 0.3f;

		public int Cutoff {
			get => filterModule?.Frequency ?? 8000;
			set => filterModule.Frequency = value;
		}

		public float Bandwidth {
			get => filterModule?.Bandwidth ?? 0.5f;
			set => filterModule.Bandwidth = value;
		}

		public int TremoloFrequency {
			get => tremoloModule?.Frequency ?? 5;
			set => tremoloModule.Frequency = value;
		}

		public float TremoloAmplitude {
			get => tremoloModule?.Amplitude ?? 0.2f;
			set => tremoloModule.Amplitude = value;
		}

		public double Delay {
			get => delayModule?.DelayMs ?? 1f;
			set => delayModule.DelayMs = value;
		}
		public float Feedback {
			get => delayModule?.Feedback ?? 0.5f;
			set => delayModule.Feedback = value;
		}
		public float Mix {
			get => delayModule?.Mix ?? 0.5f;
			set => delayModule.Mix = value;
		}
		public float Wet {
			get => delayModule?.OutputWet ?? 0.5f;
			set => delayModule.OutputWet = value;
		}
		public float Dry {
			get => delayModule?.OutputDry ?? 0.5f;
			set => delayModule.OutputDry = value;
		}

		private bool osc1Enable;
		private bool osc2Enable;

		private readonly EnvelopeModule[,] signals;
		private readonly MixingSampleProvider mixer1;
		private readonly MixingSampleProvider mixer2;
		private readonly VolumeModule volumeControl1;
		private readonly VolumeModule volumeControl2;
		private readonly MixingSampleProvider mixerAll;
		private readonly TremoloModule tremoloModule;
		private readonly DelayModule delayModule;
		private readonly FilterModule filterModule;
		private IWavePlayer player;

		private readonly List<double> frequencies = new List<double> {
			27.50, 29.14, 30.87, 
			32.70, 34.65, 36.71, 38.89,	41.20, 43.65, 46.25, 49.00,	51.91, 55.00, 58.27, 61.74,
			65.41, 69.30, 73.42, 77.78, 82.41, 87.31, 92.50, 98.00, 103.83, 110.00, 116.54, 123.47,
			130.81, 138.59, 146.83, 155.56,	164.81,	174.61,	185.00,	196.00,	207.65,	220.00,	233.08,	246.94,
			261.63,	277.18,	293.66,	311.13,	329.63,	349.23,	369.99,	392.00,	415.30,	440.00,	466.16,	493.88,
			523.25,	554.37,	587.33,	622.25,	659.25,	698.46,	739.99,	783.99,	830.61,	880.00,	932.33,	987.77,
			1046.50, 1108.73, 1174.66, 1244.51, 1318.51, 1396.91, 1479.98, 1567.98, 1661.22, 1760.00, 1864.66, 1975.53,
			2093.00, 2217.46, 2349.32, 2489.02, 2637.02, 2793.83, 2959.96, 3135.96, 3322.44, 3520.00, 3729.31, 3951.07,
			4186.01
		};

		public Controller() {
			var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
			signals = new EnvelopeModule[2, 88];
			mixer1 = new MixingSampleProvider(waveFormat) { ReadFully = true };
			mixer2 = new MixingSampleProvider(waveFormat) { ReadFully = true };
			volumeControl1 = new VolumeModule(mixer1);
			volumeControl2 = new VolumeModule(mixer2);
			mixerAll = new MixingSampleProvider(waveFormat) { ReadFully = true };
			tremoloModule = new TremoloModule(mixerAll);
			delayModule = new DelayModule(tremoloModule);
			filterModule = new FilterModule(delayModule);

			mixerAll.AddMixerInput(volumeControl1);
			mixerAll.AddMixerInput(volumeControl2);

			Osc1Volume = 0.25f;
			Osc2Volume = 0.25f;
		}

		public void NoteDown(int keyIndex1, int keyIndex2) {
			if (osc1Enable && keyIndex1 >= 0 && signals[0, keyIndex1] == null) {
				signals[0, keyIndex1] = new EnvelopeModule(
					new SignalModule(Osc1Waveform, frequencies[keyIndex1]), Attack, Decay, Sustain, Release);
				mixer1.AddMixerInput(signals[0, keyIndex1]);
			}

			if (osc2Enable && keyIndex2 >= 0 && signals[1, keyIndex2] == null) {
				signals[1, keyIndex2] = new EnvelopeModule(
					new SignalModule(Osc2Waveform, frequencies[keyIndex2]), Attack, Decay, Sustain, Release);
				mixer2.AddMixerInput(signals[1, keyIndex2]);
			}
		}

		public void NoteUp(int keyIndex1, int keyIndex2) {
			if (keyIndex1 >= 0 && signals[0, keyIndex1] != null) {
				signals[0, keyIndex1].Stop();
				signals[0, keyIndex1] = null;
			}

			if (keyIndex2 >= 0 && signals[1, keyIndex2] != null) {
				signals[1, keyIndex2].Stop();
				signals[1, keyIndex2] = null;
			}
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
			player.Init(new SampleToWaveProvider(filterModule));
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