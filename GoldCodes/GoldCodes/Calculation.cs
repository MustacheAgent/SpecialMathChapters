using OxyPlot;
using System;
using System.Collections.Generic;

namespace GoldCodes
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

        public static double[] AddNoise(double[] signal, double signalNoise, Random random)
        {
            double[] result = new double[signal.Length];
            double[] noise = new double[signal.Length];
            int coefficient = 12;
            double energyS = 0, energyN = 0;

            for (int i = 0; i < noise.Length; i++)
            {
                for (int k = 0; k < coefficient; k++)
                {
                    noise[i] += ((random.NextDouble() * 2) - 1) * (1.0 / 12.0);
                }
                //noise[i] /= coefficient * 100;

                energyS += Math.Pow(signal[i], 2);
                energyN += Math.Pow(noise[i], 2);
            }

            double noiseCoef = Math.Sqrt(energyS / energyN * Math.Pow(10, -signalNoise / 10.0));

            for (int i = 0; i < signal.Length; i++)
            {
                result[i] = signal[i] + (noiseCoef * noise[i]);
            }

            return result;
        }

        public static double[] Rxx(double[] _arr1, double[] _arr2)
        {
            double[] arr1, arr2, to_return;

            if (_arr1.Length >= _arr2.Length)
            {
                arr1 = _arr1;
                arr2 = _arr2;
                to_return = new double[arr1.Length];
            }
            else
            {
                arr1 = _arr2;
                arr2 = _arr1;
                to_return = new double[arr1.Length];
            }

            for (int i = 0; i < arr1.Length; i++)
            {
                for (int j = 0; j < arr2.Length; j++)
                {
                    to_return[i] += arr1[(arr1.Length + i + j) % arr1.Length] * arr2[j];
                }
            }

            return to_return;
        }

        public static double[] Rxx(int[] _arr1, int[] _arr2)
        {
            int[] arr1, arr2;
            double[] to_return;

            if (_arr1.Length >= _arr2.Length)
            {
                arr1 = _arr1;
                arr2 = _arr2;
                to_return = new double[arr1.Length];
            }
            else
            {
                arr1 = _arr2;
                arr2 = _arr1;
                to_return = new double[arr1.Length];
            }

            for (int i = 0; i < arr1.Length; i++)
            {
                for (int j = 0; j < arr2.Length; j++)
                {
                    to_return[i] += arr1[(arr1.Length + i + j) % arr1.Length] * arr2[j];
                }
            }

            return to_return;
        }

        public static double[] Cut(double[] arr, int from, int to)
        {
            double[] to_return = new double[to - from];
            for (int i = from; i < to; i++)
            {
                to_return[i - from] = arr[i];
            }
            return to_return;
        }

        public static double GetMax(double[] arr)
        {
            double max = Double.MinValue;
            for (int i = 0; i < arr.Length; i++)
            {
                if (max < arr[i]) max = arr[i];
            }
            return max;
        }

        public static double[][] PM4(int[] bits, int sampleRate, int carryFreq, double _beta = -1, int A = 1, int CountsPerBit = 1)
        {
            double[] output = new double[(int)(CountsPerBit * (bits.Length + bits.Length % 2))];
            double[] I = new double[output.Length];
            double[] Q = new double[output.Length];
            double[] fi = new double[output.Length];

            //List<double> I = new List<double>();
            //List<double> Q = new List<double>();
            List<double> xI = new List<double>();
            List<double> xQ = new List<double>();

            double phase = 0;

            for (int i = 0; i < bits.Length; i += 2)
            {
                for (int j = 0; j < CountsPerBit * 2; j++)
                {
                    if (i < bits.Length)
                        I[i * CountsPerBit + j] = (bits[i] * A - 0.5 * A);
                    else
                        I[i * CountsPerBit + j] = 0;
                    if (i + 1 < bits.Length)
                        Q[i * CountsPerBit + j] = (bits[i + 1] * A - 0.5 * A);
                    else
                        Q[i * CountsPerBit + j] = 0;

                    xI.Add(1.0 / sampleRate * (i * CountsPerBit + j));
                    xQ.Add(xI[xI.Count - 1]);

                    var res = Math.Atan2(Q[i * CountsPerBit + j], I[i * CountsPerBit + j]);
                    fi[i * CountsPerBit + j] = res / Math.PI;

                    output[i * CountsPerBit + j] = I[i * CountsPerBit + j] * Math.Cos(phase) - Q[i * CountsPerBit + j] * Math.Sin(phase);
                    phase += carryFreq / sampleRate * Math.PI * 2;
                    if (phase > Math.PI * 2) phase -= Math.PI * 2;
                }
            }

            if (_beta != -1)
            {

            }

            double[][] to_return = new double[4][];
            to_return[0] = I;
            to_return[1] = Q;
            to_return[2] = fi;
            to_return[3] = output;
            return to_return;
        }
    }
}
