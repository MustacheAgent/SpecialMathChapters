using BinarySequence.ModulationTypes;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BinarySequence
{
    public enum ModulationType
    {
        Amplitude,
        Frequency,
        Phase
    }

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
        private int _signalNoise;
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
        public int SignalNoise
        {
            get => _signalNoise;
            set
            {
                _signalNoise = value;
                OnPropertyChanged();
            }
        }
        #endregion 

        public ICommand GenerateBits { get; set; }
        public ICommand GenerateModulation { get; set; }
        public ICommand Correlation { get; set; }

        public List<DataPoint> PointsSourceBinary { get; set; }
        public List<DataPoint> PointsRandomBinary { get; set; }
        public List<DataPoint> PointsMainSignal { get; set; }
        public List<DataPoint> PointsResearchSignal { get; set; }

        public ModulationType Modulation { get; set; }

        Random random;

        public MainViewModel()
        {
            PointsSourceBinary = new List<DataPoint>();
            PointsRandomBinary = new List<DataPoint>();
            PointsMainSignal = new List<DataPoint>();
            PointsResearchSignal = new List<DataPoint>();

            Modulation = ModulationType.Amplitude;

            random = new Random();

            Invalidate = 0;
            BitAmount = 32;
            Bitrate = 2;
            FrequencyDiscr = 250;
            FrequencyCarry = 1;
            Tau = 1000;
            SignalNoise = 10;

            GenerateBits = new RelayCommand(o =>
            {
                Calculation.GenerateBinary(PointsSourceBinary, BitAmount, random);
                Calculation.GenerateBinary(PointsRandomBinary, BitAmount * 2, random);
                Invalidate++;
            });

            GenerateModulation = new RelayCommand(o =>
            {
                switch(Modulation)
                {
                    case ModulationType.Amplitude:
                        Calculation.AmplitudeShiftKeying(PointsSourceBinary, PointsMainSignal, _freqDiscr, _bitrate, _freqCarry);
                        Calculation.AmplitudeShiftKeying(PointsRandomBinary, PointsResearchSignal, _freqDiscr, _bitrate, _freqCarry);
                        break;
                    case ModulationType.Frequency:
                        Calculation.FrequencyShiftKeying(PointsSourceBinary, PointsMainSignal, _freqDiscr, _bitrate, _freqCarry);
                        Calculation.FrequencyShiftKeying(PointsRandomBinary, PointsResearchSignal, _freqDiscr, _bitrate, _freqCarry);
                        break;
                    case ModulationType.Phase:
                        Calculation.PhaseShiftKeying(PointsSourceBinary, PointsMainSignal, _freqDiscr, _bitrate, _freqCarry);
                        Calculation.PhaseShiftKeying(PointsRandomBinary, PointsResearchSignal, _freqDiscr, _bitrate, _freqCarry);
                        break;
                }
                Calculation.InsertSequence(PointsResearchSignal, PointsMainSignal, Tau);
                Calculation.AddNoise(PointsResearchSignal, SignalNoise, random);
                Invalidate++;
            });

            Correlation = new RelayCommand(o =>
            {
                CorrelationWindow window = new CorrelationWindow
                {
                    DataContext = new ViewModels.CorrelationViewModel(PointsMainSignal, PointsResearchSignal)
                };

                window.ShowDialog();
            });
        }
    }
}
