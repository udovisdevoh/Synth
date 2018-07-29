using Synth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            BeepSaver beepSaver = new BeepSaver();

            double frequency = 64;

            while (frequency < 11000)
            {
                beepSaver.SaveBeepFile((int)frequency, 16, "sine_" + (int)frequency + ".wav");
                frequency *= 1.08;
            }
        }
    }
}
