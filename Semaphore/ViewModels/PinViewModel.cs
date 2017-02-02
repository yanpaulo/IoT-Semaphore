using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Semaphore.ViewModels
{
    public class PinViewModel : INotifyPropertyChanged
    {
        private bool _isOn;
        public event PropertyChangedEventHandler PropertyChanged;

        public PinViewModel()
        {
            PropertyChanged += SegmentDisplayViewModelItem_PropertyChanged;
        }

        public bool IsOn
        {
            get { return _isOn; }
            set
            {
                _isOn = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsOn"));
            }
        }

        public GpioPin GpioPin { get; set; }

        private void SegmentDisplayViewModelItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GpioPin?.Write(IsOn ? GpioPinValue.High : GpioPinValue.Low);
        }

    }
}
