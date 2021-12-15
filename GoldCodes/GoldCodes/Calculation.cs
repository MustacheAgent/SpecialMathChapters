using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinarySequence
{
    public static class Calculation
    {
        public static void GenerateBinary(List<DataPoint> points, int size, Random random)
        {
            points.Clear();
            for (int i = 0; i < size; i++)
            {
                points.Add(new DataPoint(i, random.Next(2)));
            }
        }

        public static void AddNoise(List<DataPoint> signal, double signalNoise, Random random)
        {
            double[] noise = new double[signal.Count];
            int coefficient = 12;
            double energyS = 0, energyN = 0;

            for (int i = 0; i < noise.Length; i++)
            {
                for (int k = 0; k < coefficient; k++)
                {
                    noise[i] += ((random.NextDouble() * 2) - 1) * (1.0 / 12.0);
                }
                //noise[i] /= coefficient * 100;

                energyS += Math.Pow(signal[i].Y, 2);
                energyN += Math.Pow(noise[i], 2);
            }

            double noiseCoef = Math.Sqrt(energyS / energyN * Math.Pow(10, -signalNoise / 10.0));

            for (int i = 0; i < signal.Count; i++)
            {
                signal[i] = new DataPoint(i, signal[i].Y + (noiseCoef * noise[i]));
            }
        }
    }
}
