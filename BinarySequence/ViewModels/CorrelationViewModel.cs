using OxyPlot;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BinarySequence.ViewModels
{
    public class CorrelationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private int _invalidate;
        private int _foundTau;

        public int Invalidate
        {
            get => _invalidate;
            set
            {
                _invalidate = value;
                OnPropertyChanged();
            }
        }
        public int FoundTau
        {
            get => _foundTau;
            set
            {
                _foundTau = value;
                OnPropertyChanged();
            }
        }

        public List<DataPoint> PointsMainSignal { get; set; }
        public List<DataPoint> PointsResearchSignal { get; set; }

        public List<DataPoint> PointsCorrelation { get; set; }

        public CorrelationViewModel() { }

        public CorrelationViewModel(List<DataPoint> main, List<DataPoint> research)
        {
            Invalidate = 0;

            PointsMainSignal = new List<DataPoint>();
            PointsMainSignal.AddRange(main);

            PointsResearchSignal = new List<DataPoint>();
            PointsResearchSignal.AddRange(research);

            PointsCorrelation = new List<DataPoint>();

            Correlation(PointsMainSignal, PointsResearchSignal);
        }

        private void Correlation(List<DataPoint> main, List<DataPoint> research)
        {
            double correlation = 0;
            for (int i = 0; i < research.Count - main.Count; i++)
            {
                for (int k = 0; k < main.Count; k++)
                {
                    correlation += main[k].Y * research[i + k].Y;
                }
                PointsCorrelation.Add(new DataPoint(i, correlation));
                correlation = 0;
            }

            FoundTau = (int)PointsCorrelation.First(x => x.Y == PointsCorrelation.Max(t => t.Y)).X;
        }
    }
}
