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
            BitAmount = 16;
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
                PointsCarrier.Add(carrier.GeneratePoint(_freqCarry, i));
            }
        }
    }
}
