﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace Semaphore.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private Attributes
        private int _displayValue;
        private bool _isButtonEnabled;
        private GpioController gpioController;
        private int[][] _numbers;
        private PinViewModel[] _digit1Pins;
        private PinViewModel[] _digit2Pins;
        private CancellationTokenSource _counterCancelationTokenSource;
        #endregion
        
        #region Properties
        public GpioPin ButtonPin { get; private set; }
        public PinViewModel[] CarroPins { get; private set; }
        public PinViewModel[] PedestrePins { get; private set; }
        public PinViewModel[] DisplayPins { get; private set; }
        public PinViewModel[] DisplayControlPins { get; private set; }
        public PinViewModel[] ScreenDigit1Pins
        {
            get { return _digit1Pins; }
            set { _digit1Pins = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Digit1Pins")); }
        }
        public PinViewModel[] ScreenDigit2Pins
        {
            get { return _digit2Pins; }
            set { _digit2Pins = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Digit2Pins")); }
        }
        public int DisplayValue
        {
            get { return _displayValue; }
            set { _displayValue = value; UpdateScreenDigits(); }
        }

        public bool IsButtonEnabled
        {
            get { return _isButtonEnabled; }
            set { _isButtonEnabled = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsButtonEnabled")); }
        }
        #endregion

        public MainPageViewModel()
        {
            var pins = new List<GpioPin>();
            gpioController = GpioController.GetDefault();

            //Inicializa 15 pinos
            for (int i = 2; i <= 13; i++)
            {
                var pin = gpioController?.OpenPin(i);
                pin?.SetDriveMode(GpioPinDriveMode.Output);
                pins.Add(pin);
            }
            for (int i = 17; i <= 18; i++)
            {
                var pin = gpioController?.OpenPin(i);
                pin?.SetDriveMode(GpioPinDriveMode.Output);
                pins.Add(pin);
            }

            var pin20 = gpioController?.OpenPin(20);
            pin20?.SetDriveMode(GpioPinDriveMode.Output);
            pins.Add(pin20);

            CarroPins = pins.Take(3).ToViewModelArray(); //Pins 0-2
            PedestrePins = pins.Skip(3).Take(2).ToViewModelArray(); //Pins 3-4

            DisplayPins = pins.Skip(5).Take(7).ToViewModelArray(); //Pins 5-11
            DisplayControlPins = pins.Skip(12).Take(2).ToViewModelArray(); //Pins 12-13

            ScreenDigit1Pins = new GpioPin[7].ToViewModelArray();
            ScreenDigit2Pins = new GpioPin[7].ToViewModelArray();

            ButtonPin = pins.Last(); //Pin 14
            ButtonPin?.SetDriveMode(GpioPinDriveMode.InputPullUp);

            _numbers = new int[][]
            {
                new[] { 0, 1, 2, 3, 4, 5 }, // 0
                new[] { 1, 2 },             // 1
                new[] { 0, 1, 3, 4, 6 },    // 2
                new[] { 0, 1, 2, 3, 6 },    // 3
                new[] { 1, 2, 5, 6 },       // 4
                new[] { 0, 2, 3, 5, 6 },    // 5
                new[] { 2, 3, 4, 5, 6 },    // 6
                new[] { 0, 1, 2, 5 },       // 7
                new[] { 0, 1, 2, 3, 4, 5, 6 }, // 8
                new[] { 0, 1, 2, 3, 5, 6 }  // 9
            };

            Reset();
        }

        public void Reset()
        {
            CarroPins.SwitchAll(false);
            CarroPins[0].IsOn = true;

            PedestrePins.SwitchAll(false);
            PedestrePins[1].IsOn = true;
            
            ScreenDigit1Pins.SwitchAll(false);
            ScreenDigit2Pins.SwitchAll(false);

            IsButtonEnabled = true;

            HideCounter();
        }

        public void ShowCounter()
        {
            _counterCancelationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                DisplayControlPins[0].IsOn = true;
                DisplayControlPins[1].IsOn = false;
                int[] digitNumbers;
                var sw = new Stopwatch();
                var ticks = 5;
                while (!_counterCancelationTokenSource.Token.IsCancellationRequested)
                {
                    digitNumbers = _numbers[DisplayValue / 10];
                    DisplayPins.SwitchAll(false);
                    DisplayControlPins.Invert();
                    DisplayPins.Where((p, i) => digitNumbers.Contains(i)).SwitchAll(true);
                    sw.Restart();
                    while (sw.ElapsedMilliseconds < ticks) ;

                    digitNumbers = _numbers[DisplayValue % 10];
                    DisplayPins.SwitchAll(false);
                    DisplayControlPins.Invert();
                    DisplayPins.Where((p, i) => digitNumbers.Contains(i)).SwitchAll(true);
                    sw.Restart();
                    while (sw.ElapsedMilliseconds < ticks) ;
                }
                sw.Stop();
                DisplayPins.SwitchAll(false);
                DisplayControlPins.SwitchAll(true);

            }, _counterCancelationTokenSource.Token);
        }

        public void HideCounter()
        {
            _counterCancelationTokenSource?.Cancel();
        }
        
        private void UpdateScreenDigits()
        {
            int[] digitNumbers = _numbers[DisplayValue / 10];
            ScreenDigit1Pins.SwitchAll(false);
            ScreenDigit1Pins.Where((p, i) => digitNumbers.Contains(i)).SwitchAll(true);

            digitNumbers = _numbers[DisplayValue % 10];
            ScreenDigit2Pins.SwitchAll(false);
            ScreenDigit2Pins.Where((p, i) => digitNumbers.Contains(i)).SwitchAll(true);
        }
    }
}
