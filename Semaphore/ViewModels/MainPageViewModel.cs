using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Semaphore.ViewModels
{
    public class MainPageViewModel
    {
        private PinViewModel[] carroPins, pedestrePins;

        private int displayValue;
        private PinViewModel[] displayPins;
        private PinViewModel[] displayControlPins;

        private PinViewModel[][] numbers;
        
        private GpioController gpioController;
        private GpioPin buttonPin;

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

            carroPins = pins.Take(3).ToViewModelArray(); //Pins 0-2
            pedestrePins = pins.Skip(3).Take(2).ToViewModelArray(); //Pins 3-4

            displayPins = pins.Skip(5).Take(7).ToViewModelArray(); //Pins 5-11
            displayControlPins = pins.Skip(12).Take(2).ToViewModelArray(); //Pins 12-13

            buttonPin = pins.Last(); //Pin 14
            buttonPin?.SetDriveMode(GpioPinDriveMode.InputPullUp);

            numbers = new PinViewModel[][]
            {
                new[] { displayPins[0], displayPins[1], displayPins[2], displayPins[4], displayPins[5], displayPins[6] },
                new[] { displayPins[2], displayPins[5] },
                new[] { displayPins[1], displayPins[2], displayPins[3], displayPins[4], displayPins[6] },
                new[] { displayPins[1], displayPins[2], displayPins[3], displayPins[5], displayPins[6] },
                new[] { displayPins[0], displayPins[2], displayPins[3], displayPins[5] },
                new[] { displayPins[1], displayPins[0], displayPins[3], displayPins[5], displayPins[6] },
                new[] { displayPins[0], displayPins[4], displayPins[6], displayPins[6], displayPins[3] },
                new[] { displayPins[1], displayPins[2], displayPins[5] },
                new[] { displayPins[0], displayPins[1], displayPins[2], displayPins[3], displayPins[4], displayPins[5], displayPins[6] },
                new[] { displayPins[0], displayPins[1], displayPins[2], displayPins[3], displayPins[4], displayPins[6] }
            };
        }
    }
}
