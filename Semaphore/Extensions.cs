using Semaphore.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Gpio;

namespace Semaphore
{
    public static class PinListExtensions
    {
        public static void SwitchAll(this PinViewModel[] list, bool value)
        {
            foreach (var item in list)
            {
                item.IsOn = value;
            }
        }

        public static void Invert(this PinViewModel[] list)
        {
            foreach (var item in list)
            {
                item.IsOn = !item.IsOn;
            }
        }
    }

    public static class GpioPinEnumerableExtensions
    {
        public static PinViewModel[] ToViewModelArray(this IEnumerable<GpioPin> e)
        {
            return e.Select(p => new PinViewModel { GpioPin = p }).ToArray();
        }
    }
}