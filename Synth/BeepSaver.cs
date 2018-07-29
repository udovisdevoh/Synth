using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synth
{
    public class BeepSaver
    {
        public void SaveBeepFile(int frequency, int duration, string fileName)
        {
            float Tick = 1f / 44100f;

            double durationDouble = this.RoundDurationToFrequencyCycle(duration, frequency);

            int Samples = (int)Math.Round((44100.0 * durationDouble) / 1000.0);

            //int Samples = (44100 * duration) / 1000;

            int Bytes = Samples * sizeof(short);
            int[] Header = new int[] { 0x46464952, 0x24 + Bytes, 0x45564157, 0x20746d66,
                0x10, 0x10001, 0xac44, 0x15880, 0x100002, 0x61746164, Bytes };

            using (MemoryStream memoryStream = new MemoryStream(0x24 + Bytes))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    SignalGenerator signalGenerator = new SignalGenerator();
                    signalGenerator.Amplitude = short.MaxValue;
                    signalGenerator.Frequency = frequency;

                    for (int i = 0; i <= Header.Length - 1; i++)
                    {
                        binaryWriter.Write(Header[i]);
                    }

                    for (int i = 0; i <= Samples - 1; i++)
                    {
                        binaryWriter.Write(Convert.ToInt16(signalGenerator.GetValue(i * Tick)));
                    }

                    binaryWriter.Flush();

                    memoryStream.Seek(0L, SeekOrigin.Begin);

                    using (FileStream file = new FileStream(fileName, FileMode.Create, System.IO.FileAccess.Write))
                    {
                        byte[] bytes = new byte[memoryStream.Length];
                        memoryStream.Read(bytes, 0, (int)memoryStream.Length);
                        file.Write(bytes, 0, bytes.Length);
                        memoryStream.Close();
                    }
                }
            }
        }

        private double RoundDurationToFrequencyCycle(double duration, double frequency)
        {
            double cycleLengthMs = this.GetCycleLengthMs(frequency);

            if (cycleLengthMs == 0)
            {
                return duration;
            }

            duration = Math.Round(duration / cycleLengthMs);
            duration = (double)duration * cycleLengthMs;

            if (duration == 0)
            {
                duration = (int)Math.Ceiling(cycleLengthMs);
            }

            return duration;
        }

        private double GetCycleLengthMs(double frequency)
        {
            return 1000.0 / frequency;
        }
    }
}
