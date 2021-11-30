using BinarySequence.ModulationTypes;
using OxyPlot;
using System;
using System.Collections.Generic;

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

        public static void AmplitudeShiftKeying(List<DataPoint> main, List<DataPoint> modulated, int freqDiscr, int bitrate, int freqCarry)
        {
            modulated.Clear();
            int p = freqDiscr / bitrate;
            int size = p * main.Count;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (main[k].Y == 0)
                {
                    modulated.Add(ASK.Generate(i, freqCarry, freqDiscr, 0.25));
                }
                else
                {
                    modulated.Add(ASK.Generate(i, freqCarry, freqDiscr, 1));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        public static void FrequencyShiftKeying(List<DataPoint> main, List<DataPoint> modulated, int freqDiscr, int bitrate, int freqCarry)
        {
            modulated.Clear();
            int p = freqDiscr / bitrate;
            int size = p * main.Count;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (main[k].Y == 0)
                {
                    modulated.Add(FSK.Generate(i, freqCarry, freqDiscr, 0));
                }
                else
                {
                    modulated.Add(FSK.Generate(i, freqCarry, freqDiscr, 2 * freqCarry));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        public static void PhaseShiftKeying(List<DataPoint> main, List<DataPoint> modulated, int freqDiscr, int bitrate, int freqCarry)
        {
            modulated.Clear();
            int p = freqDiscr / bitrate;
            int size = p * main.Count;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (main[k].Y == 0)
                {
                    modulated.Add(PSK.Generate(i, freqCarry, freqDiscr, -1));
                }
                else
                {
                    modulated.Add(PSK.Generate(i, freqCarry, freqDiscr, 1));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        public static void InsertSequence(List<DataPoint> main, List<DataPoint> inserted, int tau)
        {
            for (int i = 0; i < inserted.Count; i++)
            {
                main[i + tau] = new DataPoint(main[i + tau].X, inserted[i].Y);
            }
        }

        public static void AddNoise(List<DataPoint> signal, int signalNoise, Random random)
        {
            double[] noise = new double[signal.Count];
            int coefficient = 12;
            double energyS = 0, energyN = 0;

            for (int i = 0; i < noise.Length; i++)
            {
                for (int k = 0; k < coefficient; k++)
                {
                    noise[i] += random.Next(-100, 101);
                }
                noise[i] /= coefficient * 100;

                energyS += Math.Pow(signal[i].Y, 2);
                energyN += Math.Pow(noise[i], 2);
            }

            double noiseCoef = Math.Sqrt(energyS / energyN * Math.Pow(10, -signalNoise / 10));

            for (int i = 0; i < signal.Count; i++)
            {
                signal[i] = new DataPoint(i, signal[i].Y + (noiseCoef * noise[i]));
            }
        }
    }
}
