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

            try
            {
                smallImpedanceMeter.SetClockDivider(0);
                smallImpedanceMeter.SetToCalibrate();

                //smallImpedanceMeter.AD5933_SelectVoutRange(AD5933_VoutRange.Range_200mVpp);
                //Thread.Sleep(1000);

                DumpRegisterData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

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

        private static string ToBinaryString(byte decimalNumber)
        {
            int remainder = 0;
            string result = string.Empty;

            if (decimalNumber == 0) result = "0";

            while (decimalNumber > 0)
            {
                remainder = decimalNumber % 2;
                decimalNumber /= 2;
                result = remainder.ToString() + result;
            }

            result = result.PadLeft(8, '0');
            return result;
        }

        private static void DumpRegisterData()
        {
            Console.WriteLine();
            Console.WriteLine("ctrl:");
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x80)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x81)));
            Console.WriteLine();
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x80));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x81));

            Console.WriteLine();
            Console.WriteLine("start freq:");
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x82)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x83)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x84)));
            Console.WriteLine();
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x82));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x83));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x84));

            Console.WriteLine();
            Console.WriteLine("freq incrm:");
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x85)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x86)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x87)));
            Console.WriteLine();
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x85));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x86));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x87));

            Console.WriteLine();
            Console.WriteLine("num of incrm:");
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x88)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x89)));
            Console.WriteLine();
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x88));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x89));

            Console.WriteLine();
            Console.WriteLine("num of settl:");
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x8A)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x8B)));
            Console.WriteLine();
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x8A));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x8B));

            Console.WriteLine();
            Console.WriteLine("stat:");
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x8F)));
            Console.WriteLine();
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x8F));

            Console.WriteLine();
            Console.WriteLine("temp:");
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x92)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x93)));
            Console.WriteLine();
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x92));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x93));

            Console.WriteLine();
            Console.WriteLine("real:");
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x94)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x95)));
            Console.WriteLine();
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x94));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x95));

            Console.WriteLine();
            Console.WriteLine("imag:");
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x96)));
            Console.WriteLine(ToBinaryString(smallImpedanceMeter.AD5933_RegisterRead(0x97)));
            Console.WriteLine();
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x96));
            Console.WriteLine(smallImpedanceMeter.AD5933_RegisterRead(0x97));
        }
    }
}