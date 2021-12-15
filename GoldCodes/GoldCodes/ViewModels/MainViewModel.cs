using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GoldCodes.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #region поля
        private int _invalidate;
        private int _bitAmount;
        private int _bitrate;
        private int _sampleRate;
        private int _snr;
        #endregion

        #region свойства
        public int Invalidate
        {
            get => _invalidate;
            set
            {
                _invalidate = value;
                OnPropertyChanged();
            }
        }
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
            get => _bitrate;
            set
            {
                _bitrate = value;
                OnPropertyChanged();
            }
        }
        public int SampleRate
        {
            get => _sampleRate / 1000;
            set
            {
                _sampleRate = value * 1000;
                OnPropertyChanged();
            }
        }
        public int SNR
        {
            get => _snr;
            set
            {
                _snr = value;
                OnPropertyChanged();
            }
        }
        #endregion

        Random random;


        public MainViewModel()
        {
            Invalidate = 0;
            BitAmount = 64;
            Bitrate = 2000;
            SampleRate = 400;

            random = new Random();
        }
    }
}
