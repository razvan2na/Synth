using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Synth.Filter;
using Synth.Module;

namespace Synth {
	public partial class MainWindow : Window {
		private readonly Controller controller;

		private readonly List<Key> keyboard = new List<Key> {
			Key.Z, Key.S, Key.X, Key.D, Key.C, Key.V, Key.G, Key.B, Key.H, Key.N, Key.J, Key.M,
			Key.Q, Key.D2, Key.W, Key.D3, Key.E, Key.R, Key.D5, Key.T, Key.D6, Key.Y, Key.D7, Key.U
		};
		
		private enum KeyButtons {
			C1, CS1, D1, DS1, E1, F1, FS1, G1, GS1, A1, AS1, B1,
			C2, CS2, D2, DS2, E2, F2, FS2, G2, GS2, A2, AS2, B2,
			C3, CS3, D3, DS3, E3, F3, FS3, G3, GS3, A3, AS3, B3,
			C4, CS4, D4, DS4, E4, F4, FS4, G4, GS4, A4, AS4, B4
		}

		public MainWindow()
		{
			controller = new Controller();
			InitializeComponent();
			DataContext = this;
		}
		
		private void OnKeyDown(object sender, KeyEventArgs e) {
			var keyIndex = keyboard.IndexOf(e.Key);
			controller.NoteDown(
				keyIndex + ((int) SliderOsc1Octave.Value + 2) * 12 + 3,
				keyIndex + ((int) SliderOsc2Octave.Value + 2) * 12 + 3);
		}

		private void OnKeyUp(object sender, KeyEventArgs e) {
			var keyIndex = keyboard.IndexOf(e.Key);
			controller.NoteUp(
				keyIndex + ((int) SliderOsc1Octave.Value + 2) * 12 + 3,
				keyIndex + ((int) SliderOsc2Octave.Value + 2) * 12 + 3);
		}
		
		private void OnScreenKeyDown(object sender, MouseButtonEventArgs e) {
			var key = (sender as Button)?.Name;
			var keyIndex = (int) Enum.Parse(typeof(KeyButtons), key ?? "C1");
			controller.NoteDown(keyIndex + 15, keyIndex + 15);
		}
		
		private void OnScreenKeyUp(object sender, MouseButtonEventArgs e) {
			var key = (sender as Button)?.Name;
			var keyIndex = (int) Enum.Parse(typeof(KeyButtons), key ?? "C1");
			controller.NoteUp(keyIndex + 15, keyIndex + 15);
		}
		
		private void OnOsc1EnableCheck(object sender, RoutedEventArgs e) {
			controller.Osc1Enable = true;
		}
		
		private void OnOsc1EnableUncheck(object sender, RoutedEventArgs e) {
			controller.Osc1Enable = false;
		}
		
		private void OnOsc2EnableCheck(object sender, RoutedEventArgs e) {
			controller.Osc2Enable = true;
		}
		
		private void OnOsc2EnableUncheck(object sender, RoutedEventArgs e) {
			controller.Osc2Enable = false;
		}

		private void OnOsc1VolumeChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Osc1Volume = (float) SliderOsc1Volume.Value;
			LabelOsc1Volume.Content = $"Volume: {(int)(controller.Osc1Volume * 100.0)}%";
		}
		
		private void OnOsc1WaveformSelectionChange(object sender, SelectionChangedEventArgs selectionChangedEventArgs) {
			var selectedItem = ComboBoxOsc1Waveform?.SelectedItem as ComboBoxItem;
			var content = selectedItem?.Content.ToString();
			controller.Osc1Waveform = (SignalType) Enum.Parse(typeof(SignalType), content ?? "Sine");
		}

		private void OnOsc1OctaveChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Osc1Octave = (int) SliderOsc1Octave.Value;
			LabelOsc1Octave.Content = $"Octave: {controller.Osc1Octave}";
		}
		
		private void OnOsc2VolumeChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Osc2Volume = (float) SliderOsc2Volume.Value;
			LabelOsc2Volume.Content = $"Volume: {(int)(controller.Osc2Volume * 100.0)}%";
		}
		
		private void OnOsc2WaveformSelectionChange(object sender, SelectionChangedEventArgs selectionChangedEventArgs) {
			var selectedItem = ComboBoxOsc2Waveform?.SelectedItem as ComboBoxItem;
			var content = selectedItem?.Content.ToString();
			controller.Osc2Waveform = (SignalType) Enum.Parse(typeof(SignalType), content ?? "Sine");
		}

		private void OnOsc2OctaveChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Osc2Octave = (int) SliderOsc2Octave.Value;
			LabelOsc2Octave.Content = $"Octave: {controller.Osc2Octave}";
		}

		private void OnAttackChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Attack = (float) SliderAttack.Value;
			LabelAttack.Content = $"{(int)(controller.Attack * 1000)}ms";
		}

		private void OnDecayChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Decay = (float) SliderDecay.Value;
			LabelDecay.Content = $"{(int)(controller.Decay * 1000)}ms";
		}

		private void OnSustainChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Sustain = (float) SliderSustain.Value;
			LabelSustain.Content = $"{(int)(controller.Sustain * 100)}%";
		}

		private void OnReleaseChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Release = (float) SliderRelease.Value;
			LabelRelease.Content = $"{(int)(controller.Release * 1000)}ms";
		}

		private void OnFilterEnableCheck(object sender, RoutedEventArgs e) {
			controller.FilterEnable = true;
		}
		
		private void OnFilterEnableUncheck(object sender, RoutedEventArgs e) {
			controller.FilterEnable = false;
		}
		
		private void OnFilterTypeSelectionChange(object sender, SelectionChangedEventArgs selectionChangedEventArgs) {
			var selectedItem = ComboBoxFilterType?.SelectedItem as ComboBoxItem;
			var content = selectedItem?.Content.ToString();
			controller.FilterType = (FilterType) Enum.Parse(typeof(FilterType), content ?? "LowPass");
		}
		
		private void OnDelayEnableCheck(object sender, RoutedEventArgs e) {
			controller.DelayEnable = true;
		}
		
		private void OnDelayEnableUncheck(object sender, RoutedEventArgs e) {
			controller.DelayEnable = false;
		}

		private void OnCutoffChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Cutoff = (int) SliderFilterCutoff.Value;
			LabelFilterCutoff.Content = $"{controller.Cutoff}Hz";
		}

		private void OnBandwidthChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Bandwidth = (float) SliderFilterBandwidth.Value;
			LabelFilterBandwidth.Content = $"{controller.Bandwidth}";
		}
		
		private void OnDelayChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Delay = SliderDelay.Value;
			LabelDelay.Content = $"{(int) (controller.Delay)}ms";
		}
		
		private void OnFeedbackChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Feedback = (float) SliderFeedback.Value;
			LabelFeedback.Content = $"{controller.Feedback}";
		}
		
		private void OnMixChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Mix = (float) SliderMix.Value;
			LabelMix.Content = $"{controller.Mix}";
		}
		
		private void OnWetChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Wet = (float) SliderWet.Value;
			LabelWet.Content = $"{controller.Wet}";
		}
		
		private void OnDryChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.Dry = (float) SliderDry.Value;
			LabelDry.Content = $"{controller.Dry}";
		}
		
		private void OnTremoloEnableCheck(object sender, RoutedEventArgs e) {
			controller.TremoloEnable = true;
		}
		
		private void OnTremoloEnableUncheck(object sender, RoutedEventArgs e) {
			controller.TremoloEnable = false;
		}
		
		private void OnTremoloFrequencyChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.TremoloFrequency = (int) SliderTremoloFrequency.Value;
			LabelTremoloFrequency.Content = $"{controller.TremoloFrequency}";
		}
		
		private void OnTremoloAmplitudeChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			controller.TremoloAmplitude = (float) SliderTremoloAmplitude.Value;
			LabelTremoloAmplitude.Content = $"{controller.TremoloAmplitude}";
		}
	}
}