using OxyPlot;
using System;

namespace BinarySequence.ModulationTypes
{
    public class FSK
    {
        private double _currentX;
        public FSK()
        {
            _currentX = 0;
        }

        public DataPoint GeneratePoint(double phaseModul, double phaseCarrier)
        {
            DataPoint point = new DataPoint(_currentX, Math.Sin(phaseCarrier - 4 * Math.Cos(phaseModul)));
            _currentX++;
            return point;
        }
    }
}
