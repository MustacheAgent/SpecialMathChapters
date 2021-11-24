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
        private int _tau;
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
        public int Tau
        {
            get => _tau;
            set
            {
                _tau = value;
                OnPropertyChanged();
            }
        }
        #endregion 

        public ICommand GenerateBits { get; set; }
        public ICommand GenerateRandomBits { get; set; }
        public ICommand GenerateASK { get; set; }
        public ICommand GenerateFSK { get; set; }
        public ICommand GeneratePSK { get; set; }

        public List<DataPoint> PointsSourceBinary { get; set; }
        public List<DataPoint> PointsRandomBinary { get; set; }
        public List<DataPoint> PointsMainSignal { get; set; }
        public List<DataPoint> PointsResearchSignal { get; set; }

        Random random;

        public MainViewModel()
        {
            PointsSourceBinary = new List<DataPoint>();
            PointsRandomBinary = new List<DataPoint>();
            PointsMainSignal = new List<DataPoint>();
            PointsResearchSignal = new List<DataPoint>();

            random = new Random();

            Invalidate = 0;
            BitAmount = 64;
            Bitrate = 2;
            FrequencyDiscr = 250;
            FrequencyCarry = 1;
            Tau = 15;

            GenerateBits = new RelayCommand(o =>
            {
                GenerateSourceBinary();
                Invalidate++;
            });

            GenerateRandomBits = new RelayCommand(o =>
            {
                GenerateRandomBinary();
                Invalidate++;
            });

            GenerateASK = new RelayCommand(o =>
            {
                GenerateSourceAmp();
                Invalidate++;
            });

            GenerateFSK = new RelayCommand(o =>
            {
                GenerateSourceFreq();
                Invalidate++;
            });

            GeneratePSK = new RelayCommand(o =>
            {
                GenerateSourcePhase();
                Invalidate++;
            });
        }

        private void GenerateSourceBinary()
        {
            PointsSourceBinary.Clear();
            for (int i = 0; i < BitAmount; i++)
            {
                PointsSourceBinary.Add(new DataPoint(i, random.Next(2)));
            }
        }

        private void GenerateRandomBinary()
        {
            PointsRandomBinary.Clear();
            for (int i = 0; i < BitAmount * 2; i++)
            {
                PointsRandomBinary.Add(new DataPoint(i, random.Next(2)));
            }
        }

        private void GenerateSourceAmp()
        {
            PointsMainSignal.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * BitAmount;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (PointsSourceBinary[k].Y == 0)
                {
                    PointsMainSignal.Add(ASK.Generate(i, _freqCarry, _freqDiscr, 0.5));
                }
                else
                {
                    PointsMainSignal.Add(ASK.Generate(i, _freqCarry, _freqDiscr, 1));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        private void GenerateSourceFreq()
        {
            PointsMainSignal.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * BitAmount;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (PointsSourceBinary[k].Y == 0)
                {
                    PointsMainSignal.Add(FSK.Generate(i, _freqCarry, _freqDiscr, 0));
                }
                else
                {
                    PointsMainSignal.Add(FSK.Generate(i, _freqCarry, _freqDiscr, 2 * _freqCarry));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        private void GenerateSourcePhase()
        {
            PointsMainSignal.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * BitAmount;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (PointsSourceBinary[k].Y == 0)
                {
                    PointsMainSignal.Add(PSK.Generate(i, _freqCarry, _freqDiscr, -1));
                }
                else
                {
                    PointsMainSignal.Add(PSK.Generate(i, _freqCarry, _freqDiscr, 1));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }
    }
}
