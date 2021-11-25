using OxyPlot;
using System.Collections.Generic;
using System.ComponentModel;
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

        public int Invalidate
        {
            get => _invalidate;
            set
            {
                _invalidate = value;
                OnPropertyChanged();
            }
        }

        public List<DataPoint> PointsMainSignal { get; set; }
        public List<DataPoint> PointsResearchSignal { get; set; }

        public CorrelationViewModel(List<DataPoint> main, List<DataPoint> research)
        {
            Invalidate = 0;
            PointsMainSignal.AddRange(main);
            PointsResearchSignal.AddRange(research);
        }
    }
}
