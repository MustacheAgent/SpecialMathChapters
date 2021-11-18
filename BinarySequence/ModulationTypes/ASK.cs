using OxyPlot;
using System;

namespace BinarySequence.ModulationTypes
{
    public class ASK
    {
        private double _currentX;

        public ASK()
        {
            _currentX = 0;
        }

        public DataPoint GeneratePoint(double phaseModul, double phaseCarrier)
        {
            DataPoint point = new DataPoint(_currentX, Math.Sin(phaseModul) * Math.Sin(phaseCarrier));
            _currentX++;
            return point;
        }
    }
}
