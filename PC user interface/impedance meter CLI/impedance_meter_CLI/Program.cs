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

            try
            {
                smallImpedanceMeter.Connect();
            }
            catch (Exception) 
            {
                Console.WriteLine("Waiting for device ...");
            }

            while (!Console.KeyAvailable) Application.DoEvents();

            smallImpedanceMeter.Ping();
            smallImpedanceMeter.Powerdown();
            smallImpedanceMeter.SetClockDivider(0x00);

            smallImpedanceMeter.Disconnect();

            smallImpedanceMeter.OnDeviceAttached -= OnDeviceAttached;
            smallImpedanceMeter.OnDeviceRemoved -= OnDeviceRemoved;

            Console.WriteLine("Device Disconnected.");

            Thread.Sleep(2000);
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