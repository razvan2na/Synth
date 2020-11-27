using System;

namespace Synth {
	public class Envelope {
		private const float ATTACK_RATIO = 0.3f;
		private const float DECAY_RATIO = 0.0001f;
		
		public enum EnvelopeState {
			Idle = 0, Attack, Decay, Sustain, Release
		};

		public EnvelopeState State { get; private set; }

		public float AttackRate {
			get => attackRate;
			set {
				attackRate = value;
				attackCoef = ComputeCoeficient(value, ATTACK_RATIO);
				attackBase = (1f + ATTACK_RATIO) * (1f - attackCoef);
			}
		}
        
		public float DecayRate {
			get => decayRate;
			set {
				decayRate = value;
				decayCoef = ComputeCoeficient(value, DECAY_RATIO);
				decayBase = (sustainLevel - DECAY_RATIO) * (1f - decayCoef);
			}
		}
		
		public float SustainLevel {
			get => sustainLevel;
			set {
				sustainLevel = value;
				decayBase = (sustainLevel - DECAY_RATIO) * (1f - decayCoef);
			}
		}
        
		public float ReleaseRate {
			get => releaseRate;
			set {
				releaseRate = value;
				releaseCoef = ComputeCoeficient(value, DECAY_RATIO);
				releaseBase = -DECAY_RATIO * (1f - releaseCoef);
			}
		}
		
		private float output;
        private float attackRate;
        private float decayRate;
        private float releaseRate;
        private float attackCoef;
        private float decayCoef;
        private float releaseCoef;
        private float attackBase;
        private float decayBase;
        private float releaseBase;
        private float sustainLevel;
        
        public Envelope() {
	        State = EnvelopeState.Idle;
	        output = 0f;
	        
            AttackRate = 0f;
            DecayRate = 0f;
            ReleaseRate = 0f;
            SustainLevel = 1f;
        }
        
        public float Process() {
            switch (State) {
                case EnvelopeState.Idle: break;
                case EnvelopeState.Attack:
                    output = attackBase + output * attackCoef;
                    
                    if (output >= 1f) {
                        output = 1f;
                        State = EnvelopeState.Decay;
                    }
                    
                    break;
                case EnvelopeState.Decay:
                    output = decayBase + output * decayCoef;
                    
                    if (output <= sustainLevel) {
                        output = sustainLevel;
                        State = EnvelopeState.Sustain;
                    }
                    
                    break;
                case EnvelopeState.Sustain: break;
                case EnvelopeState.Release:
                    output = releaseBase + output * releaseCoef;
                    
                    if (output <= 0f) {
                        output = 0f;
                        State = EnvelopeState.Idle;
                    }
                    
                    break;
                default:
	                throw new ArgumentOutOfRangeException();
            }
            
            return output;
        }

        public void GateOn() {
	        State = EnvelopeState.Attack;
        }

        public void GateOff() {
	        if (State != EnvelopeState.Idle)
		        State = EnvelopeState.Release;
        }
        
        private static float ComputeCoeficient(float rate, float ratio)  {
	        return (float)Math.Exp(-Math.Log((1f + ratio) / ratio) / rate);
        }
	}
}