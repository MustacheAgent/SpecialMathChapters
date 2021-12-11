﻿using OxyPlot;
using System;

namespace BinarySequence.ModulationTypes
{
    public static class FSK
    {
        public static DataPoint Generate(int x, double freq, double freqDiscr, double delta, double phase = 0)
        {
            return new DataPoint(x, Math.Sin(2 * Math.PI * (freq + delta) * (x / freqDiscr) + phase));
        }
    }
}
