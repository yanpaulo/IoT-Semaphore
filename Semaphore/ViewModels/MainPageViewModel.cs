using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace Semaphore.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private Attributes
        private GpioController gpioController;
        private PinViewModel[][] _numbers;
        private PinViewModel[] _digit1Pins;
        private PinViewModel[] _digit2Pins; 
        #endregion

        public int DisplayValue { get; set; }

        public GpioPin ButtonPin { get; private set; }
        public PinViewModel[] CarroPins { get; private set; }
        public PinViewModel[] PedestrePins { get; private set; }
        public PinViewModel[] DisplayPins { get; private set; }
        public PinViewModel[] DisplayControlPins { get; private set; }
        public PinViewModel[] Digit1Pins
        {
            get { return _digit1Pins; }
            set { _digit1Pins = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Digit1Pins")); }
        }
        public PinViewModel[] Digit2Pins
        {
            get { return _digit2Pins; }
            set { _digit2Pins = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Digit2Pins")); }
        }
        
        public MainPageViewModel()
        {
            var pins = new List<GpioPin>();
            gpioController = GpioController.GetDefault();

            //Inicializa 15 pinos
            for (int i = 2; i <= 16; i++)
            {
                var pin = gpioController?.OpenPin(i);
                pin?.SetDriveMode(GpioPinDriveMode.Output);
                pins.Add(pin);
            }

            CarroPins = pins.Take(3).ToViewModelArray(); //Pins 0-2
            PedestrePins = pins.Skip(3).Take(2).ToViewModelArray(); //Pins 3-4

            DisplayPins = pins.Skip(5).Take(7).ToViewModelArray(); //Pins 5-11
            DisplayControlPins = pins.Skip(12).Take(2).ToViewModelArray(); //Pins 12-13

            ButtonPin = pins.Last(); //Pin 14
            ButtonPin?.SetDriveMode(GpioPinDriveMode.InputPullUp);

            _numbers = new PinViewModel[][]
            {
                new[] { DisplayPins[0], DisplayPins[1], DisplayPins[2], DisplayPins[4], DisplayPins[5], DisplayPins[6] },
                new[] { DisplayPins[2], DisplayPins[5] },
                new[] { DisplayPins[1], DisplayPins[2], DisplayPins[3], DisplayPins[4], DisplayPins[6] },
                new[] { DisplayPins[1], DisplayPins[2], DisplayPins[3], DisplayPins[5], DisplayPins[6] },
                new[] { DisplayPins[0], DisplayPins[2], DisplayPins[3], DisplayPins[5] },
                new[] { DisplayPins[1], DisplayPins[0], DisplayPins[3], DisplayPins[5], DisplayPins[6] },
                new[] { DisplayPins[0], DisplayPins[4], DisplayPins[6], DisplayPins[6], DisplayPins[3] },
                new[] { DisplayPins[1], DisplayPins[2], DisplayPins[5] },
                new[] { DisplayPins[0], DisplayPins[1], DisplayPins[2], DisplayPins[3], DisplayPins[4], DisplayPins[5], DisplayPins[6] },
                new[] { DisplayPins[0], DisplayPins[1], DisplayPins[2], DisplayPins[3], DisplayPins[4], DisplayPins[6] }
            };

            Reset();

        }

        public void Reset()
        {
            CarroPins.SwitchAll(false);
            CarroPins[0].IsOn = true;

            PedestrePins.SwitchAll(false);
            PedestrePins[1].IsOn = true;
        }
        
        public async Task UpdateDisplayPins()
        {
            DisplayControlPins.SwitchAll(false);

            DisplayPins.SwitchAll(false);
            _numbers[DisplayValue / 10].SwitchAll(true);
            Digit1Pins = DisplayPins;
            DisplayControlPins[0].IsOn = true;

            await Task.Delay(25);
            DisplayPins.SwitchAll(false);
            _numbers[DisplayValue % 10].SwitchAll(true);
            Digit2Pins = DisplayPins;
            DisplayControlPins.Invert();
        }
    }
}
