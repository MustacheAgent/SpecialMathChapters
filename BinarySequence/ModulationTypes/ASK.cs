using OxyPlot;
using System;

namespace BinarySequence.ModulationTypes
{
    public static class ASK
    {
        public static DataPoint Generate(int x, double freq, double freqDiscr, double amp)
        {
            return new DataPoint(x, amp * Math.Sin(2 * Math.PI * x * freq / freqDiscr));
        }
    }
}
