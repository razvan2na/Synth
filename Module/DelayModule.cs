using System;
using NAudio.Wave;

namespace Synth.Module {
    public class DelayModule : ISampleProvider {
        float delaypos;
        float odelay;
        float delaylen;
        float wetmix;
        float drymix;
        float wetmix2;
        float drymix2;
        float rspos;
        int rspos2;
        float drspos;
        int tpos;
        float[] buffer = new float[500000];
        private double delayMs;
        private double feedbackDb;
        private ISampleProvider sourceProvider;
        private double mixInDb;
        private double outputWetDb;
        private double outputDryDb;
        private bool resampleOnLengthChange;

        public DelayModule(ISampleProvider sourceProvider) {
            this.sourceProvider = sourceProvider;
            // AddSlider(300, 0, 4000, 20, "delay (ms)"); // slider1
            //AddSlider(-5, -120, 6, 1, "feedback (dB)"); // slider2
            //AddSlider(0, -120, 6, 1, "mix in (dB)"); // slider 3
            //AddSlider(-6, -120, 6, 1, "output wet (dB)"); // slider 4
            //AddSlider(0, -120, 6, 1, "output dry (dB)"); // slider 5
            // Slider resampleSlider = AddSlider(0, 0, 1, 1, "resample on length change"); // slider 6
            Init();
            DelayMs = 300;
            FeedbackDb = -5;
            MixInDb = 0;
            OutputWetDb = -6;
            OutputDryDb = 0;
            ResampleOnLengthChange = true;
        }

        public bool ResampleOnLengthChange {
            get { return resampleOnLengthChange; }
            set { resampleOnLengthChange = value; Slider(); }
        }

        public double DelayMs {
            get { return delayMs; }
            set { delayMs = value; Slider();}
        }

        public double FeedbackDb {
            get { return feedbackDb; }
            set { feedbackDb = value; Slider(); }
        }

        public double MixInDb {
            get { return mixInDb; }
            set { mixInDb = value; Slider(); }
        }

        public double OutputWetDb {
            get { return outputWetDb; }
            set { outputWetDb = value; Slider(); }
        }

        public double OutputDryDb {
            get { return outputDryDb; }
            set { outputDryDb = value;
                Slider();
            }
        }


        public void Init() {
            delaypos = 0;
        }

        public void Slider() {
            odelay = delaylen;
            delaylen = (float)Math.Min(DelayMs * WaveFormat.SampleRate / 1000, 500000);
            if (odelay != delaylen) {
                if (ResampleOnLengthChange && odelay > delaylen) {
                    // resample down delay buffer, heh
                    rspos = 0; rspos2 = 0;
                    drspos = odelay / delaylen;
                    for (int n = 0; n < delaylen; n++) {
                        tpos = ((int)rspos) * 2;
                        buffer[rspos2 + 0] = buffer[tpos + 0];
                        buffer[rspos2 + 1] = buffer[tpos + 1];
                        rspos2 += 2;
                        rspos += drspos;
                    }
                    delaypos /= drspos;
                    delaypos = (int)delaypos;
                    if (delaypos < 0) delaypos = 0;
                }
                else
                {
                    if (ResampleOnLengthChange  && odelay < delaylen) {
                        // resample up delay buffer, heh
                        drspos = odelay / delaylen;
                        rspos = odelay;
                        rspos2 = (int)delaylen * 2;
                        for (int n = 0; n < (int)delaylen; n++) {
                            rspos -= drspos;
                            rspos2 -= 2;

                            tpos = ((int)(rspos)) * 2;
                            buffer[rspos2 + 0] = buffer[tpos + 0];
                            buffer[rspos2 + 1] = buffer[tpos + 1];
                        }
                        delaypos /= drspos;
                        delaypos = (int)delaypos;
                        if (delaypos < 0) delaypos = 0;
                    }
                    else {
                        if (ResampleOnLengthChange  && delaypos >= delaylen) delaypos = 0;
                    }
                }
                //freembuf(delaylen*2);
            }
            wetmix = (float)Math.Pow(2, (FeedbackDb / 6));
            drymix = (float)Math.Pow(2, (MixInDb / 6));
            wetmix2 = (float)Math.Pow(2, (OutputWetDb / 6));
            drymix2 = (float)Math.Pow(2, (OutputDryDb / 6));
        }

        public float Sample(float spl0) {
            int dpint = (int)delaypos;
            float os1 = buffer[dpint + 0];

            buffer[dpint + 0] = Math.Min(Math.Max(spl0 * drymix + os1 * wetmix, -4), 4);

            if ((delaypos += 1) >= delaylen) {
                delaypos = 0;
            }

            return spl0 * drymix2 + os1 * wetmix2;
        }

        public int Read(float[] sampleBuffer, int offset, int count) {
            var read = sourceProvider.Read(sampleBuffer, offset, count);
            
            for (int n = 0; n < read; n++) {
                sampleBuffer[offset + n] = Sample(sampleBuffer[offset + n]);
            }
            
            return read;
        }

        public WaveFormat WaveFormat { get { return sourceProvider.WaveFormat; } }
    }
}