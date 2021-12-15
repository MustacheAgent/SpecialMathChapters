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
        private int _periods;
        private int _noiseSignalStep;
        private int _noiseSignalBegin;
        private int _noiseSignalEnd;
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
        
        public int Periods
        {
            get => _periods;
            set
            {
                _periods = value;
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

        public int NoiseSignalBegin
        {
            get => _noiseSignalBegin;
            set
            {
                _noiseSignalBegin = value;
                OnPropertyChanged();
            }
        }
        
        public int NoiseSignalEnd
        {
            get => _noiseSignalEnd;
            set
            {
                _noiseSignalEnd = value;
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

        Random random, randASK, randFSK, randPSK;

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
            randASK = new Random();
            randFSK = new Random();
            randPSK = new Random();

            Invalidate = 0;
            BitAmount = 50;
            Bitrate = 2;
            FrequencyDiscr = 150;
            FrequencyCarry = 6;
            Tau = 2000;
            SignalNoise = 10;
            Periods = 3;
            NoiseSignalStep = 1;
            Repeat = 100;
            NoiseSignalBegin = 10;
            NoiseSignalEnd = -20;

            Research = new RelayCommand(o =>
            {
                new Thread(() => ResearchCorrelation(FSK, ModulationType.Frequency, randFSK))
                { IsBackground = true }.Start();

                new Thread(() => ResearchCorrelation(PSK, ModulationType.Phase, randPSK))
                { IsBackground = true }.Start();

                new Thread(() => ResearchCorrelation(ASK, ModulationType.Amplitude, randASK))
                { IsBackground = true }.Start();
            });

            GenerateModulation = new RelayCommand(o =>
            {
                Calculation.GenerateBinary(PointsSourceBinary, BitAmount, random);
                Calculation.GenerateBinary(PointsRandomBinary, BitAmount * Periods, random);

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
                Calculation.InsertSequence(PointsResearchSignal, PointsMainSignal, Tau);
                Calculation.AddNoise(PointsMainSignal, SignalNoise, random);
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

        private void ResearchCorrelation(List<DataPoint> points, ModulationType modulation, Random random)
        {
            points.Clear();

            double timeInterval = 1000d / _bitrate;
            double timePoint = timeInterval / (_freqDiscr / _bitrate);
            double time = Tau * timePoint;
            double currentCorr;
            double success = 0;

            List<DataPoint> binary = new List<DataPoint>();
            List<DataPoint> rand = new List<DataPoint>();
            List<DataPoint> main = new List<DataPoint>();
            List<DataPoint> research = new List<DataPoint>();

            for (int i = NoiseSignalBegin; i >= NoiseSignalEnd; i -= NoiseSignalStep)
            {
                for (int j = 0; j < Repeat; j++)
                {
                    Calculation.GenerateBinary(binary, BitAmount, random);
                    Calculation.GenerateBinary(rand, BitAmount * Periods, random);

                    switch (modulation)
                    {
                        case ModulationType.Amplitude:
                            Calculation.AmplitudeShiftKeying(binary, main, _freqDiscr, _bitrate, _freqCarry);
                            Calculation.AmplitudeShiftKeying(rand, research, _freqDiscr, _bitrate, _freqCarry);
                            break;
                        case ModulationType.Frequency:
                            Calculation.FrequencyShiftKeying(binary, main, _freqDiscr, _bitrate, _freqCarry);
                            Calculation.FrequencyShiftKeying(rand, research, _freqDiscr, _bitrate, _freqCarry);
                            break;
                        case ModulationType.Phase:
                            Calculation.PhaseShiftKeying(binary, main, _freqDiscr, _bitrate, _freqCarry);
                            Calculation.PhaseShiftKeying(rand, research, _freqDiscr, _bitrate, _freqCarry);
                            break;
                        default:
                            break;
                    }
                    Calculation.InsertSequence(research, main, Tau);
                    Calculation.AddNoise(main, i, random);
                    Calculation.AddNoise(research, i, random);

                    currentCorr = Calculation.Correlation(main, research) * timePoint;
                    if (currentCorr >= (time - (timeInterval * 0.5d)) && currentCorr <= (time + (timeInterval * 0.5d)))
                    {
                        success++;
                    }
                }

                points.Add(new DataPoint(i, success / Repeat));
                success = 0;
                Invalidate++;
            }
        }
    }
}