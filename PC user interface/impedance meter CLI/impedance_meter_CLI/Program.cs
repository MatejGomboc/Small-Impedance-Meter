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
                smallImpedanceMeter.AD5933_CheckFrequencySweepComplete();
                smallImpedanceMeter.AD5933_CheckValidData();
                smallImpedanceMeter.AD5933_CheckValidTemp();
                smallImpedanceMeter.AD5933_GetFreqIncrement();
                smallImpedanceMeter.AD5933_GetImaginaryValue();
                smallImpedanceMeter.AD5933_GetNumOfIncrements();
                smallImpedanceMeter.AD5933_GetNumOfSettlingCycles();
                smallImpedanceMeter.AD5933_GetRealValue();
                smallImpedanceMeter.AD5933_GetStartFreq();
                smallImpedanceMeter.AD5933_GetTemp();
                smallImpedanceMeter.AD5933_IncrementFreq();
                smallImpedanceMeter.AD5933_Init();
                smallImpedanceMeter.AD5933_InitWithStartFreq();
                smallImpedanceMeter.AD5933_Powerdown();
                smallImpedanceMeter.AD5933_RepeatFreq();
                smallImpedanceMeter.AD5933_Reset();
                smallImpedanceMeter.AD5933_Standby();
                smallImpedanceMeter.SetToCalibrate();
                smallImpedanceMeter.SetToMeasure();
                smallImpedanceMeter.Ping();
                smallImpedanceMeter.InitClockDivider();
                

                //smallImpedanceMeter.AD5933_MeasureTemp();
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
    }
}