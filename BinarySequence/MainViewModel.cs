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
        private int _amount;
        private int _invalidate;
        #endregion

        #region свойства 
        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
        public int Invalidate
        {
            get => _invalidate;
            set
            {
                _invalidate = value;
                OnPropertyChanged(nameof(Invalidate));
            }
        }
        #endregion 

        public ICommand Generate { get; set; }

        public List<DataPoint> PointsBinary { get; set; }
        public List<DataPoint> PointsCarrier { get; set; }
        public List<DataPoint> PointsASK { get; set; }
        public List<DataPoint> PointsFSK { get; set; }
        public List<DataPoint> PointsPSK { get; set; }

        Random random;

        public MainViewModel()
        {
            PointsBinary = new List<DataPoint>();
            PointsCarrier = new List<DataPoint>();
            PointsASK = new List<DataPoint>();
            PointsFSK = new List<DataPoint>();
            PointsPSK = new List<DataPoint>();

            random = new Random();

            Invalidate = 0;
            Amount = 150;

            Generate = new RelayCommand(o =>
            {
                GenerateBinary();
            });
        }

        private void GenerateBinary()
        {
            //PointsBinary.Clear();

            for (int i = 0; i < Amount; i++)
            {
                PointsBinary.Add(new DataPoint(i, random.Next(2)));
            }

            Invalidate++;
        }
    }
}
