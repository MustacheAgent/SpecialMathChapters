using OxyPlot;
using System;

namespace BinarySequence
{
    public static class SignalCarrier
    {
        /// <summary>
        /// Сгенерировать точку графика несущего сигнала.
        /// </summary>
        /// <param name="freq">Частота.</param>
        /// <param name="x">Период дискретизации.</param>
        /// <returns>Точка несущего сигнала.</returns>
        public static DataPoint GeneratePoint(double freq, int x, double freqDiscr)
        {
            return new DataPoint(x, Math.Sin(2 * Math.PI * x * freq / freqDiscr));
        }
    }
}
