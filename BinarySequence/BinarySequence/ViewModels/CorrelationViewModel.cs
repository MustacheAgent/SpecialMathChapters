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

        public List<DataPoint> PointsCorrelation { get; set; }

        public CorrelationViewModel() { }

        public CorrelationViewModel(List<DataPoint> main, List<DataPoint> research)
        {
            Invalidate = 0;
            PointsCorrelation = new List<DataPoint>();
            FoundTau = Calculation.Correlation(main, research, PointsCorrelation);
            Invalidate++;
        }
    }
}
