using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synth
{
    public class SineBackgroundBeeper : IDisposable
    {
        #region Constants
        private const int duration = 16;
        #endregion

        #region Members
        private int currentFrequency = 500;

        private object startStopLock = new object();

        private bool isStopping = false;

        private float tick = 1f / 44100f;

        private int samples = (44100 * duration) / 1000;

        private SoundPlayer soundPlayer;

        private int bytes;

        private int[] header;

        private SignalGenerator signalGenerator;
        #endregion

        public SineBackgroundBeeper()
        {
            this.bytes = samples * sizeof(short);
            this.soundPlayer = new SoundPlayer();
            this.signalGenerator = new SignalGenerator();
            this.header = new int[] { 0x46464952, 0x24 + bytes, 0x45564157, 0x20746d66, 0x10, 0x10001, 0xac44, 0x15880, 0x100002, 0x61746164, bytes};
        }

        public void Play()
        {
            Thread thread = new Thread(StartWorker);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Stop()
        {
            this.isStopping = true;
        }

        private void StartWorker()
        {
            while (!this.isStopping)
            {
                this.Beep(this.currentFrequency);
            }
        }

        private void Beep(int frequency)
        {
            using (MemoryStream memoryStream = new MemoryStream(0x24 + bytes))
            {
                memoryStream.Seek(0L, SeekOrigin.Begin);
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    signalGenerator.Amplitude = short.MaxValue;
                    signalGenerator.Frequency = frequency;

                    for (int i = 0; i <= header.Length - 1; i++)
                    {
                        binaryWriter.Write(header[i]);
                    }

                    for (int i = 0; i <= samples - 1; i++)
                    {
                        binaryWriter.Write(Convert.ToInt16(signalGenerator.GetValue(i * tick)));
                    }

                    binaryWriter.Flush();

                    memoryStream.Seek(0L, SeekOrigin.Begin);

                    soundPlayer.Stream = memoryStream;

                    soundPlayer.PlaySync();
                }
            }
        }

        public void Dispose()
        {
            this.soundPlayer.Dispose();
        }
    }
}
