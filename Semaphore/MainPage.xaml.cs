﻿using Semaphore.ViewModels;
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
        private MainPageViewModel _viewModel;
        private DispatcherTimer _verdeTimer, _amareloTimer, _vermelhoTimer;
        
        public MainPage()
        {
            
            this.DataContext = _viewModel = new MainPageViewModel();

            _verdeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _verdeTimer.Tick += VerdeTimer_Tick;

            _amareloTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _amareloTimer.Tick += AmareloTimer_Tick;

            _vermelhoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _vermelhoTimer.Tick += VermelhoTimer_Tick;

            if (_viewModel.ButtonPin != null)
            {
                _viewModel.ButtonPin.ValueChanged += ButtonPin_ValueChanged; 
            }
            
            this.InitializeComponent();
        }
        
        private void VerdeTimer_Tick(object sender, object e)
        {
            _viewModel.CarroPins[0].IsOn = false; //Desliga o verde
            _viewModel.CarroPins[1].IsOn = true; //Liga o amarelo
            _verdeTimer.Stop();
            _amareloTimer.Start();
        }
        
        private void AmareloTimer_Tick(object sender, object e)
        {
            _viewModel.CarroPins[1].IsOn = false; //Desliga o amarelo
            _viewModel.CarroPins[2].IsOn = true; //Liga o vermelho

            _viewModel.PedestrePins.Invert(); //Inverte a saída do pedestre

            _viewModel.DisplayValue = 20;
            _amareloTimer.Stop();
            _vermelhoTimer.Start();
            _viewModel.ShowCounter();
            
        }
        
        private void VermelhoTimer_Tick(object sender, object e)
        {
            if (_viewModel.DisplayValue > 0)
            {
                _viewModel.DisplayValue--;
            }
            else
            {
                _viewModel.Reset();
                _vermelhoTimer.Stop();
                _viewModel.HideCounter();
            }
        }


        private async void ButtonPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                StartSemaphore();
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartSemaphore();
        }

        private void StartSemaphore()
        {
            _viewModel.Reset();
            _verdeTimer.Start();
        }
    }
    
}
