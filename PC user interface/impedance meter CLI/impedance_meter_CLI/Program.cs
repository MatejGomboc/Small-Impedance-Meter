using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using small_impedance_meter;

namespace impedance_meter_CLI
{
    internal class Program
    {
        private static SmallImpedanceMeter smallImpedanceMeter = new SmallImpedanceMeter();

        private static void Main(string[] args)
        {
            smallImpedanceMeter.OnDeviceAttached += OnDeviceAttached;
            smallImpedanceMeter.OnDeviceRemoved += OnDeviceRemoved;

            Console.WriteLine("Waiting for device ...");

            while (!Console.KeyAvailable) Application.DoEvents();

            smallImpedanceMeter.Disconnect();

            smallImpedanceMeter.OnDeviceAttached -= OnDeviceAttached;
            smallImpedanceMeter.OnDeviceRemoved -= OnDeviceRemoved;

            Console.WriteLine("Device Disconnected.");
        }

        private static void OnDeviceAttached(object sender, EventArgs e)
        {
            // A Device system-level event has occured

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Device Attached.");
        }

        private static void OnDeviceRemoved(object sender, EventArgs e)
        {
            // A Device system-level event has occured

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Device Removed.");
        }
    }
}