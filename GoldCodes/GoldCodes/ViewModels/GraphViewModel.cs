using System;
using System.Collections.Generic;
using System.ComponentModel;
using OxyPlot;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GoldCodes.Converters;

namespace GoldCodes.ViewModels
{
    public class GraphViewModel : INotifyPropertyChanged
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

        public List<DataPoint> BitSequence { get; set; }
        public List<DataPoint> Gold1 { get; set; }
        public List<DataPoint> Gold2 { get; set; }
        public List<DataPoint> Gold3 { get; set; }
        public List<DataPoint> Gold4 { get; set; }

        public GraphViewModel() { }

        public GraphViewModel(int[] bits, double[] gold1, double[] gold2, double[] gold3, double[] gold4)
        {
            BitSequence = new List<DataPoint>();
            Gold1 = new List<DataPoint>();
            Gold2 = new List<DataPoint>();
            Gold3 = new List<DataPoint>();
            Gold4 = new List<DataPoint>();
            bits.ToPoints(BitSequence);
            gold1.ToPoints(Gold1);
            gold2.ToPoints(Gold2);
            gold3.ToPoints(Gold3);
            gold4.ToPoints(Gold4);
            Invalidate++;
        }
    }
}
