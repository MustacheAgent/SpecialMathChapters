using OxyPlot;
using System;

namespace BinarySequence.ModulationTypes
{
    public class PSK
    {
        public static DataPoint Generate(int x, double freq, double freqDiscr, double multiplier)
        {
            return new DataPoint(x, Math.Sin(2 * Math.PI * x * freq / freqDiscr * multiplier));
        }
    }
}
