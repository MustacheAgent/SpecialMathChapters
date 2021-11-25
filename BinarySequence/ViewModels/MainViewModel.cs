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

            GenerateBits = new RelayCommand(o =>
            {
                GenerateBinary(PointsSourceBinary, BitAmount);
                Invalidate++;
            });

            GenerateRandomBits = new RelayCommand(o =>
            {
                GenerateBinary(PointsRandomBinary, BitAmount * 2);
                Invalidate++;
            });

            GenerateModulation = new RelayCommand(o =>
            {
                switch(Modulation)
                {
                    case ModulationType.Amplitude:
                        AmplitudeShiftKeying(PointsSourceBinary, PointsMainSignal);
                        AmplitudeShiftKeying(PointsRandomBinary, PointsResearchSignal);
                        break;
                    case ModulationType.Frequency:
                        FrequencyShiftKeying(PointsSourceBinary, PointsMainSignal);
                        FrequencyShiftKeying(PointsRandomBinary, PointsResearchSignal);
                        break;
                    case ModulationType.Phase:
                        PhaseShiftKeying(PointsSourceBinary, PointsMainSignal);
                        PhaseShiftKeying(PointsRandomBinary, PointsResearchSignal);
                        break;
                }
                InsertSequence(PointsResearchSignal, PointsMainSignal);
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

        private void GenerateBinary(List<DataPoint> points, int size)
        {
            points.Clear();
            for (int i = 0; i < size; i++)
            {
                points.Add(new DataPoint(i, random.Next(2)));
            }
        }

        private void AmplitudeShiftKeying(List<DataPoint> main, List<DataPoint> modulated)
        {
            modulated.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * main.Count;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (main[k].Y == 0)
                {
                    modulated.Add(ASK.Generate(i, _freqCarry, _freqDiscr, 0.5));
                }
                else
                {
                    modulated.Add(ASK.Generate(i, _freqCarry, _freqDiscr, 1));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        private void FrequencyShiftKeying(List<DataPoint> main, List<DataPoint> modulated)
        {
            modulated.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * main.Count;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (main[k].Y == 0)
                {
                    modulated.Add(FSK.Generate(i, _freqCarry, _freqDiscr, 0));
                }
                else
                {
                    modulated.Add(FSK.Generate(i, _freqCarry, _freqDiscr, 2 * _freqCarry));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        private void PhaseShiftKeying(List<DataPoint> main, List<DataPoint> modulated)
        {
            modulated.Clear();
            int p = _freqDiscr / _bitrate;
            int size = p * main.Count;
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                if (main[k].Y == 0)
                {
                    modulated.Add(PSK.Generate(i, _freqCarry, _freqDiscr, -1));
                }
                else
                {
                    modulated.Add(PSK.Generate(i, _freqCarry, _freqDiscr, 1));
                }
                if ((i % p == 0) && (i != 0))
                {
                    k++;
                }
            }
        }

        private void InsertSequence(List<DataPoint> main, List<DataPoint> inserted)
        {
            for (int i = 0; i < inserted.Count; i++)
            {
                main[i + Tau] = new DataPoint(main[i + Tau].X, inserted[i].Y);
            }
        }
    }
}
