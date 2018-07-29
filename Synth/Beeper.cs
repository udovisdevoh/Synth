using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Synth
{
    public class Beeper
    {
        public void Beep(int frequency, int duration)
        {
            float Tick = 1f / 44100f;
            int Samples = (44100 * duration) / 1000;
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
                    using (SoundPlayer soundPlayer = new SoundPlayer(memoryStream))
                    {
                        soundPlayer.PlaySync();
                    }
                }
            }
        }
    }
}
