using Semaphore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Semaphore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GpioController _gpioController;
        private GpioPin _buttonPin;

        private PinViewModel[] _carroPins, _pedestrePins;

        private int _displayValue;
        private PinViewModel[] _displayPins;
        private PinViewModel[] _displayControlPins;

        private PinViewModel[][] _numbers;
        

        public MainPage()
        {
            var pins = new List<GpioPin>();
            _gpioController = GpioController.GetDefault();

            //Inicializa 15 pinos
            for (int i = 2; i <= 16; i++)
            {
                var pin = _gpioController?.OpenPin(i);
                pin?.SetDriveMode(GpioPinDriveMode.Output);
                pins.Add(pin);
            }
            
            _carroPins = pins.Take(3).ToViewModelArray(); //Pins 0-2
            _pedestrePins = pins.Skip(3).Take(2).ToViewModelArray(); //Pins 3-4

            _displayPins = pins.Skip(5).Take(7).ToViewModelArray(); //Pins 5-11
            _displayControlPins = pins.Skip(12).Take(2).ToViewModelArray(); //Pins 12-13

            _buttonPin = pins.Last(); //Pin 14
            _buttonPin?.SetDriveMode(GpioPinDriveMode.InputPullUp);

            _numbers = new PinViewModel[][]
            {
                new[] { _displayPins[0], _displayPins[1], _displayPins[2], _displayPins[4], _displayPins[5], _displayPins[6] },
                new[] { _displayPins[2], _displayPins[5] },
                new[] { _displayPins[1], _displayPins[2], _displayPins[3], _displayPins[4], _displayPins[6] },
                new[] { _displayPins[1], _displayPins[2], _displayPins[3], _displayPins[5], _displayPins[6] },
                new[] { _displayPins[0], _displayPins[2], _displayPins[3], _displayPins[5] },
                new[] { _displayPins[1], _displayPins[0], _displayPins[3], _displayPins[5], _displayPins[6] },
                new[] { _displayPins[0], _displayPins[4], _displayPins[6], _displayPins[6], _displayPins[3] },
                new[] { _displayPins[1], _displayPins[2], _displayPins[5] },
                new[] { _displayPins[0], _displayPins[1], _displayPins[2], _displayPins[3], _displayPins[4], _displayPins[5], _displayPins[6] },
                new[] { _displayPins[0], _displayPins[1], _displayPins[2], _displayPins[3], _displayPins[4], _displayPins[6] }
            };

            this.InitializeComponent();
        }

    }
    
}
