using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.DeviceNotify;

namespace small_impedance_meter
{
    public class DeviceNotFoundException : Exception, ISerializable
    {
        public DeviceNotFoundException() : base() { }

        public DeviceNotFoundException(string message) : base(message) { }

        public DeviceNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        // This constructor is needed for serialization.
        protected DeviceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public delegate void DeviceChangedEventHandler(object sender, EventArgs e);

    public partial class SmallImpedanceMeter
    {
        public const int VID = 0x03EB;
        public const int PID = 0x2423;
        public const string SerialString = "1.0";
        public const int SerialNum = 0100;

        private UsbDevice device = null;
        private UsbDeviceFinder usbFinder = null;
        private bool connected = false;

        public bool IsConnected
        {
            get
            {
                return connected;
            }
        }

        public event DeviceChangedEventHandler OnDeviceAttached;
        public event DeviceChangedEventHandler OnDeviceRemoved;

        private IDeviceNotifier UsbDeviceNotifier = null;

        public SmallImpedanceMeter()
        {
            UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier(); // Enable the device notifier
            UsbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent; // Hook the device notifier event

            usbFinder = new UsbDeviceFinder(VID, PID, SerialNum);
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

            if ((e.Device.IdVendor == VID) && (e.Device.IdProduct == PID) && (e.Device.SerialNumber == SerialString))
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
                if (device == null) throw new DeviceNotFoundException("Device Not Found.");

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

                connected = true;
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

                connected = false;
            }
        }
    }
}
