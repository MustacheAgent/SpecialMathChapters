using BinarySequence.ModulationTypes;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BinarySequence
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #region параметры
        private int _freqDiscr;
        private int _freqCarry;
        private int _bitAmount;
        private int _invalidate;
        private int _bitrate;
        #endregion

        #region свойства 
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
            get => _bitrate / 1000;
            set
            {
                _bitrate = value * 1000;
                OnPropertyChanged();
            }
        }
        public int Invalidate
        {
            get => _invalidate;
            set
            {
                _invalidate = value;
                OnPropertyChanged();
            }
        }
        public int FrequencyDiscr
        {
            get => _freqDiscr / 1000;
            set
            {
                _freqDiscr = value * 1000;
                OnPropertyChanged();
            }
        }
        public int FrequencyCarry
        {
            get => _freqCarry / 1000;
            set
            {
                _freqCarry = value * 1000;
                OnPropertyChanged();
            }
        }
        #endregion 

        public ICommand GenerateBits { get; set; }
        public ICommand GenerateSignal { get; set; }
        public ICommand GenerateASK { get; set; }
        public ICommand GenerateFSK { get; set; }
        public ICommand GeneratePSK { get; set; }

        public List<DataPoint> PointsBinary { get; set; }
        public List<DataPoint> PointsCarrier { get; set; }
        public List<DataPoint> PointsASK { get; set; }
        public List<DataPoint> PointsFSK { get; set; }
        public List<DataPoint> PointsPSK { get; set; }

        Random random;
        SignalCarrier carrier;

        public MainViewModel()
        {
            PointsBinary = new List<DataPoint>();
            PointsCarrier = new List<DataPoint>();
            PointsASK = new List<DataPoint>();
            PointsFSK = new List<DataPoint>();
            PointsPSK = new List<DataPoint>();

            random = new Random();

            carrier = new SignalCarrier();

            Invalidate = 0;
            BitAmount = 64;
            Bitrate = 2;
            FrequencyDiscr = 250;
            FrequencyCarry = 3;

            GenerateBits = new RelayCommand(o =>
            {
                GenerateBinary();
                Invalidate++;
            });

            GenerateSignal = new RelayCommand(o =>
            {
                GenerateCarrier();
                Invalidate++;
            });

            GenerateASK = new RelayCommand(o =>
            {
                GenerateAmp();
                Invalidate++;
            });

            GenerateFSK = new RelayCommand(o =>
            {
                GenerateFreq();
                Invalidate++;
            });

            GeneratePSK = new RelayCommand(o =>
            {
                GeneratePhase();
                Invalidate++;
            });
        }

        private void GenerateBinary()
        {
            PointsBinary.Clear();
            for (int i = 0; i < BitAmount; i++)
            {
                PointsBinary.Add(new DataPoint(i, random.Next(2)));
            }
        }

        private void GenerateCarrier()
        {
            PointsCarrier.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * BitAmount;
            for (int i = 0; i < size; i++)
            {
                PointsCarrier.Add(carrier.GeneratePoint(_freqCarry, i, _freqDiscr));
            }
        }

        private void GenerateAmp()
        {
            PointsASK.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * BitAmount;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (PointsBinary[k].Y == 0)
                {
                    PointsASK.Add(ASK.Generate(i, _freqCarry, _freqDiscr, 0.5));
                }
                else
                {
                    PointsASK.Add(ASK.Generate(i, _freqCarry, _freqDiscr, 1));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        private void GenerateFreq()
        {
            PointsFSK.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * BitAmount;
            int k = 0;
            int delta = _freqCarry / 2;
            for (int i = 0; i < size; i++)
            {
                if (PointsBinary[k].Y == 0)
                {
                    PointsFSK.Add(FSK.Generate(i, _freqCarry, _freqDiscr, -delta));
                }
                else
                {
                    PointsFSK.Add(FSK.Generate(i, _freqCarry, _freqDiscr, delta));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        private void GeneratePhase()
        {
            PointsPSK.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * BitAmount;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (PointsBinary[k].Y == 0)
                {
                    PointsPSK.Add(PSK.Generate(i, _freqCarry, _freqDiscr, -1));
                }
                else
                {
                    PointsPSK.Add(PSK.Generate(i, _freqCarry, _freqDiscr, 1));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }
    }
}
