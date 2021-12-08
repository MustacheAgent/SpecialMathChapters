using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using System.Threading;

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
        private int _repeat;
        private int _noiseSignalStep;
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
        public int Repeat
        {
            get => _repeat;
            set
            {
                _repeat = value;
                OnPropertyChanged();
            }
        }
        public int NoiseSignalStep
        {
            get => _noiseSignalStep;
            set
            {
                _noiseSignalStep = value;
                OnPropertyChanged();
            }
        }
        #endregion 

        public ICommand GenerateBits { get; set; }
        public ICommand GenerateModulation { get; set; }
        public ICommand Correlation { get; set; }
        public ICommand Research { get; set; }

        public List<DataPoint> PointsSourceBinary { get; set; }
        public List<DataPoint> PointsRandomBinary { get; set; }
        public List<DataPoint> PointsMainSignal { get; set; }
        public List<DataPoint> PointsResearchSignal { get; set; }

        public List<DataPoint> ASK { get; set; }
        public List<DataPoint> FSK { get; set; }
        public List<DataPoint> PSK { get; set; }

        public ModulationType Modulation { get; set; }

        Random random;

        Thread threadFSK, threadPSK;

        public MainViewModel()
        {
            PointsSourceBinary = new List<DataPoint>();
            PointsRandomBinary = new List<DataPoint>();
            PointsMainSignal = new List<DataPoint>();
            PointsResearchSignal = new List<DataPoint>();

            ASK = new List<DataPoint>();
            FSK = new List<DataPoint>();
            PSK = new List<DataPoint>();

            Modulation = ModulationType.Amplitude;

            random = new Random();

            Invalidate = 0;
            BitAmount = 50;
            Bitrate = 2;
            FrequencyDiscr = 150;
            FrequencyCarry = 1;
            Tau = 500;
            SignalNoise = 10;
            NoiseSignalStep = 4;
            Repeat = 10;

            Research = new RelayCommand(o =>
            {
                ResearchCorrelation(ASK, ModulationType.Amplitude);
                //ResearchCorrelation(FSK, ModulationType.Frequency);
                //ResearchCorrelation(PSK, ModulationType.Phase);
            });

            GenerateModulation = new RelayCommand(o =>
            {
                Calculation.GenerateBinary(PointsSourceBinary, BitAmount, random);
                Calculation.GenerateBinary(PointsRandomBinary, BitAmount * 2, random);

                switch (Modulation)
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
                    default:
                        break;
                }
                Calculation.AddNoise(PointsMainSignal, SignalNoise, random);
                Calculation.InsertSequence(PointsResearchSignal, PointsMainSignal, Tau);
                Calculation.AddNoise(PointsResearchSignal, SignalNoise, random);
                Invalidate++;
            });

            Correlation = new RelayCommand(o =>
            {
                CorrelationView window = new CorrelationView
                {
                    DataContext = new ViewModels.CorrelationViewModel(PointsMainSignal, PointsResearchSignal)
                };

                window.ShowDialog();
            });
        }

        private void ResearchCorrelation(List<DataPoint> points, ModulationType modulation)
        {
            points.Clear();
            //int noise = 10;
            List<double> correlations = new List<double>();
            List<DataPoint> corr = new List<DataPoint>();
            double timeInterval = 1000d / _bitrate;
            double timePoint = timeInterval / (_freqDiscr / _bitrate);
            double time = Tau * timePoint;

            //int steps = (10 - (-10)) / NoiseSignalStep;

            for (int i = 10; i >= -10; i -= NoiseSignalStep)
            {
                for (int j = 0; j < Repeat; j++)
                {
                    Calculation.GenerateBinary(PointsSourceBinary, BitAmount, random);
                    Calculation.GenerateBinary(PointsRandomBinary, BitAmount * 2, random);

                    switch (modulation)
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
                        default:
                            break;
                    }
                    Calculation.AddNoise(PointsMainSignal, 10, random);
                    Calculation.InsertSequence(PointsResearchSignal, PointsMainSignal, Tau);
                    Calculation.AddNoise(PointsResearchSignal, i, random);
                    correlations.Add(Calculation.Correlation(PointsMainSignal, PointsResearchSignal) * timePoint);
                    //corr.Clear();
                }

                points.Add(new DataPoint(i, correlations.FindAll(t => t > time - (timeInterval * 0.5d) && t < time + (timeInterval * 0.5d)).Count));
                //noise -= NoiseSignalStep;
                correlations.Clear();
            }
            Invalidate++;
        }
    }
}