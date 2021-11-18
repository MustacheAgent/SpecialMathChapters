using OxyPlot;
using System;

namespace BinarySequence
{
    public class SignalCarrier
    {
        private double _phase;
        private int _currentX;

        public double Phase
        {
            get
            {
                return _phase;
            }
        }

        public SignalCarrier()
        {
            _phase = 0;
            _currentX = 0;
        }

        /// <summary>
        /// Сгенерировать точку графика несущего сигнала.
        /// </summary>
        /// <param name="freq">Частота.</param>
        /// <param name="period">Период дискретизации.</param>
        /// <returns>Точка несущего сигнала.</returns>
        public DataPoint GeneratePoint(double freq, double period)
        {
            DataPoint point = new DataPoint(_currentX, Math.Sin(_phase));
            _phase += 2 * Math.PI * freq / period;
            _phase %= 2 * Math.PI;

            _currentX++;

            return point;
        }

        /// <summary>
        /// Сгенерировать точку графика несущего сигнала.
        /// </summary>
        /// <param name="freq">Частота.</param>
        /// <param name="x">Период дискретизации.</param>
        /// <returns>Точка несущего сигнала.</returns>
        public DataPoint GeneratePoint(double freq, int x)
        {
            return new DataPoint(x, Math.Sin(x * freq));
        }
    }
}
