using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;

namespace small_impedance_meter
{
    public class SmallImpedanceMeter
    {
        private const int VID = 0x03EB;
        private const int PID = 0x2423;
        private const string Serial = "1.0";

        public delegate void DeviceChangedEventHandler(object sender, EventArgs e);

        public event DeviceChangedEventHandler OnDeviceAttached;
        public event DeviceChangedEventHandler OnDeviceRemoved;

        private IDeviceNotifier UsbDeviceNotifier = null;

        public SmallImpedanceMeter()
        {
            UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
            UsbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent; // Hook the device notifier event
        }

        ~SmallImpedanceMeter()
        {
            UsbDeviceNotifier.Enabled = false;  // Disable the device notifier
            UsbDeviceNotifier.OnDeviceNotify -= OnDeviceNotifyEvent; // Unhook the device notifier event
        }

        private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
        {
            // A Device system-level event has occured

            if ((e.Device.IdVendor == VID) && (e.Device.IdProduct == PID) && (e.Device.SerialNumber == Serial))
            {
                if (e.EventType == EventType.DeviceArrival)
                {
                    OnDeviceAttached(this, null);
                }
                else if (e.EventType == EventType.DeviceRemoveComplete)
                {
                    OnDeviceRemoved(this, null);
                }
            }
        }
    }
}
