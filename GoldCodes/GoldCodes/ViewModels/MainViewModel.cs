﻿using GoldCodes.Converters;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GoldCodes.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #region поля
        private int _invalidate;
        private int _bitAmount;
        private int _bitrate;
        private int _sampleRate;
        private int _carryFreq;
        private int _snr;

        private string _binaryData;
        private string _decodedData;
        private double _successProbability;

        private double beta;
        #endregion

        #region массивы
        private int[] bits, changed_bits;
        private int[][] gold;
        private double[][] decs;
        private double[][] gold_qpsk;
        private double[] I, Q, fi, signal;
        private double[] noisedI, noisedQ, noisedFi;
        #endregion

        #region свойства
        public int Invalidate
        {
            get => _invalidate;
            set
            {
                _invalidate = value;
                OnPropertyChanged();
            }
        }
        public int BitAmount
        {
            get => _bitAmount;
            set
            {
                _bitAmount = value;
                OnPropertyChanged();
            }
        }
        public int Bitrate
        {
            get => _bitrate;
            set
            {
                _bitrate = value;
                OnPropertyChanged();
            }
        }
        public int SampleRate
        {
            get => _sampleRate;
            set
            {
                _sampleRate = value;
                OnPropertyChanged();
            }
        }
        public int CarryFrequency
        {
            get => _carryFreq;
            set
            {
                _carryFreq = value;
                OnPropertyChanged();
            }
        }
        public int SNR
        {
            get => _snr;
            set
            {
                _snr = value;
                OnPropertyChanged();
            }
        }
        public string BinaryData
        {
            get => _binaryData;
            set
            {
                _binaryData = value;
                OnPropertyChanged();
            }
        }
        public string DecodedData
        {
            get => _decodedData;
            set
            {
                _decodedData = value;
                OnPropertyChanged();
            }
        }
        public double Success
        {
            get => _successProbability;
            set
            {
                _successProbability = value;
                OnPropertyChanged();
            }
        }
        #endregion

        Random random;

        public List<DataPoint> ComponentI { get; set; }
        public List<DataPoint> ComponentQ { get; set; }
        public List<DataPoint> GoldCorrelation1 { get; set; }
        public List<DataPoint> GoldCorrelation2 { get; set; }
        public List<DataPoint> GoldCorrelation3 { get; set; }
        public List<DataPoint> GoldCorrelation4 { get; set; }

        public ICommand Modulate { get; set; }

        public MainViewModel()
        {
            Invalidate = 0;
            BitAmount = 64;
            Bitrate = 2000;
            SampleRate = 32800;
            CarryFrequency = 6200;
            SNR = 100;
            BinaryData = "0x27";

            beta = 0.9;

            ComponentI = new List<DataPoint>();
            ComponentQ = new List<DataPoint>();
            GoldCorrelation1 = new List<DataPoint>();
            GoldCorrelation2 = new List<DataPoint>();
            GoldCorrelation3 = new List<DataPoint>();
            GoldCorrelation4 = new List<DataPoint>();

            random = new Random();

            gold = new int[4][];
            gold_qpsk = new double[4][];
            decs = new double[4][];

            int[] m1 = Sequences.MSeq(new int[] { 0, 1, 0, 0, 0, 0 });
            int[] m2 = Sequences.MSeq(new int[] { 0, 1, 1, 1, 0, 0 });
            gold[0] = Sequences.GoldSequence(m1, m2, 5, true);
            gold[1] = Sequences.GoldSequence(m1, m2, 8, true);
            gold[2] = Sequences.GoldSequence(m1, m2, 17, true);
            gold[3] = Sequences.GoldSequence(m1, m2, 33, true);

            bits = Bit.FromString(BinaryData);
            changed_bits = Sequences.GoldTransform(bits, gold);

            Modulate = new RelayCommand(o =>
            {
                GetSignal();
                noisedFi = Calculation.AddNoise(fi, SNR, random);
                Experiment();

                I.ToPoints(ComponentI);
                Q.ToPoints(ComponentQ);
                decs[0].ToPoints(GoldCorrelation1);
                decs[1].ToPoints(GoldCorrelation2);
                decs[2].ToPoints(GoldCorrelation3);
                decs[3].ToPoints(GoldCorrelation4);
                Invalidate++;
            });
        }

        private void GetSignal()
        {
            double[][] result = Calculation.PM4(changed_bits, SampleRate, CarryFrequency);
            I = result[0];
            Q = result[1];
            fi = result[2];
            signal = result[3];

            for (int i = 0; i < 4; i++)
            {
                gold_qpsk[i] = Calculation.PM4(gold[i], SampleRate, CarryFrequency)[2];
            }
        }

        private void Experiment(int CountsPerBit = 1)
        {
            for (int i = 0; i < decs.Length; i++)
            {
                decs[i] = Calculation.Rxx(noisedFi, gold_qpsk[i]);
            }

            int[] max_idx = new int[bits.Length / 2];
            for (int i = 0; i < max_idx.Length; i++)
            {
                double[][] cut_of_rxx = new double[4][];
                double max = double.MinValue;
                int idx = -1;
                for (int k = 0; k < 4; k++)
                {
                    cut_of_rxx[k] = Calculation.Cut(decs[k], i * gold[0].Length * CountsPerBit, (i + 1) * gold[0].Length * CountsPerBit);
                    double potentional_max = Calculation.GetMax(cut_of_rxx[k]);
                    if (potentional_max > max)
                    {
                        max = potentional_max;
                        idx = k;
                    }
                }
                max_idx[i] = idx;
            }

            int right_bits = 0;
            int[] decoded_bits = Sequences.GoldDetransform(max_idx);
            for (int i = 0; i < bits.Length; i++)
            {
                right_bits += bits[i] == decoded_bits[i] ? 1 : 0;
            }
            Success = (double)right_bits / bits.Length * 100;

            DecodedData = Bit.ToString(decoded_bits, 16);

        }
    }
}
