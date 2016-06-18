using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.DeviceNotify;

namespace small_impedance_meter
{
    public class SmallImpedanceMeter
    {
        private const int VID = 0x03EB;
        private const int PID = 0x2423;
        private const string Serial = "1.0";

        private UsbDevice device = null;
        private UsbDeviceFinder usbFinder = null;

        public delegate void DeviceChangedEventHandler(object sender, EventArgs e);

        public event DeviceChangedEventHandler OnDeviceAttached;
        public event DeviceChangedEventHandler OnDeviceRemoved;

        private IDeviceNotifier UsbDeviceNotifier = null;

        public SmallImpedanceMeter()
        {
            UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier(); // Enable the device notifier
            UsbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent; // Hook the device notifier event

            usbFinder = new UsbDeviceFinder(0x03EB, 0x2423, 0100);
        }

        ~SmallImpedanceMeter()
        {
            Disconnect(); // Disconnect from device. Put device in "Unconfigured" state.

            UsbDeviceNotifier.Enabled = false;  // Disable the device notifier
            UsbDeviceNotifier.OnDeviceNotify -= OnDeviceNotifyEvent; // Unhook the device notifier event
            UsbDeviceNotifier = null;

            usbFinder = null;
        }

        private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
        {
            // A Device system-level event has occured

            if ((e.Device.IdVendor == VID) && (e.Device.IdProduct == PID) && (e.Device.SerialNumber == Serial))
            {
                if (e.EventType == EventType.DeviceArrival)
                {
                    Connect();
                    OnDeviceAttached(this, null);
                }
                else if (e.EventType == EventType.DeviceRemoveComplete)
                {
                    OnDeviceRemoved(this, null);
                    Disconnect();
                }
            }
        }

        public void Connect()
        {
            try
            {
                // Find and open the usb device.
                device = UsbDevice.OpenUsbDevice(usbFinder);

                // If the device is open and ready
                if (device == null) throw new Exception("Device Not Found.");

                // If this is a "whole" usb device (libusb-win32, linux libusb)
                // it will have an IUsbDevice interface. If not (WinUSB) the 
                // variable will be null indicating this is an interface of a 
                // device.
                IUsbDevice wholeUsbDevice = device as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    wholeUsbDevice.ClaimInterface(0);
                    wholeUsbDevice.SetAltInterface(1);
                }

                // open read endpoint 1.
                UsbEndpointReader reader = device.OpenEndpointReader(ReadEndpointID.Ep01);

                // open write endpoint 1.
                UsbEndpointWriter writer = device.OpenEndpointWriter(WriteEndpointID.Ep02);
            }
            catch(Exception)
            {
                Disconnect();
                throw;
            }
        }

        public void Disconnect()
        {
            if (device != null)
            {
                if (device.IsOpen)
                {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = device as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release interface #0.
                        wholeUsbDevice.ReleaseInterface(0);
                        // Put device in "Unconfigured" state.
                        wholeUsbDevice.SetConfiguration(0);
                    }

                    device.Close();
                }
                device = null;

                // Free usb resources
                UsbDevice.Exit();
            }
        }
    }
}
