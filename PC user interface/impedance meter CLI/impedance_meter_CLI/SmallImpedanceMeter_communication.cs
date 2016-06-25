using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.DeviceNotify;

namespace small_impedance_meter
{
    internal enum RequestType : byte
    {
        Ping = 0,
        Powerdown = 1,
        SetClockDivider = 2
    };

    public partial class SmallImpedanceMeter
    {
        private const byte USB_TYPE_VENDOR = (0x02 << 5);
        private const byte USB_RECIP_INTERFACE = 0x01;
        private const byte USB_DIR_IN = 0x80;
        private const byte USB_DIR_OUT = 0x00;

        public void Ping()
        {
            if (!connected) throw (new Exception("Device disconnected."));

            int bytesSent = 0;
            byte[] bufferSent = { 1, 2, 3, 4, 5 };
            UsbSetupPacket setup_out = new UsbSetupPacket((byte)(USB_TYPE_VENDOR | USB_RECIP_INTERFACE | USB_DIR_OUT), (byte)RequestType.Ping, 0, 0, (short)bufferSent.Length);
            bool result = device.ControlTransfer(ref setup_out, bufferSent, bufferSent.Length, out bytesSent);

            if (result == false) throw (new Exception("Ping error while sending request."));
            if (bytesSent != bufferSent.Length) throw (new Exception("Invalid number of bytes sent: " + bytesSent.ToString() + "."));

            int bytesReceived = 0;
            byte[] bufferReceived = { 0, 0, 0, 0, 0 };
            UsbSetupPacket setup_in = new UsbSetupPacket((byte)(USB_TYPE_VENDOR | USB_RECIP_INTERFACE | USB_DIR_IN), (byte)RequestType.Ping, 0, 0, (short)bufferReceived.Length);
            result = device.ControlTransfer(ref setup_in, bufferReceived, bufferReceived.Length, out bytesReceived);

            if (result == false) throw (new Exception("Ping error while receiving response."));
            if (bytesReceived != bufferReceived.Length) throw (new Exception("Invalid number of bytes received: " + bytesReceived.ToString() + "."));

            for (int i = 0; i < bufferReceived.Length; i++)
            {
                if (bufferReceived[i] != bufferSent[i]) throw (new Exception("Received bytes not equal to sent bytes."));
            }
        }

        public void Powerdown()
        {
            if (!connected) throw (new Exception("Device disconnected."));

            int bytesSent = 0;
            byte[] bufferSent = {};
            UsbSetupPacket setup_out = new UsbSetupPacket((byte)(USB_TYPE_VENDOR | USB_RECIP_INTERFACE | USB_DIR_OUT), (byte)RequestType.Powerdown, 0, 0, (short)bufferSent.Length);
            bool result = device.ControlTransfer(ref setup_out, bufferSent, bufferSent.Length, out bytesSent);

            if (result == false) throw (new Exception("Powerdown error while sending request."));
            if (bytesSent != bufferSent.Length) throw (new Exception("Invalid number of bytes sent: " + bytesSent.ToString() + "."));

            int bytesReceived = 0;
            byte[] bufferReceived = { 0 };
            UsbSetupPacket setup_in = new UsbSetupPacket((byte)(USB_TYPE_VENDOR | USB_RECIP_INTERFACE | USB_DIR_IN), (byte)RequestType.Powerdown, 0, 0, (short)bufferReceived.Length);
            result = device.ControlTransfer(ref setup_in, bufferReceived, bufferReceived.Length, out bytesReceived);

            if (result == false) throw (new Exception("Powerdown error while receiving response."));
            if (bytesReceived != bufferReceived.Length) throw (new Exception("Invalid number of bytes received: " + bytesReceived.ToString() + "."));

            if (bufferReceived[0] != 1) throw (new Exception("Device powedown failed."));
        }

        public void SetClockDivider(byte clockDividerSetting)
        {
            if (!connected) throw (new Exception("Device disconnected."));

            int bytesSent = 0;
            byte[] bufferSent = { clockDividerSetting };
            UsbSetupPacket setup_out = new UsbSetupPacket((byte)(USB_TYPE_VENDOR | USB_RECIP_INTERFACE | USB_DIR_OUT), (byte)RequestType.SetClockDivider, 0, 0, (short)bufferSent.Length);
            bool result = device.ControlTransfer(ref setup_out, bufferSent, bufferSent.Length, out bytesSent);

            if (result == false) throw (new Exception("SetClockDivider error while sending request."));
            if (bytesSent != bufferSent.Length) throw (new Exception("Invalid number of bytes sent: " + bytesSent.ToString() + "."));

            int bytesReceived = 0;
            byte[] bufferReceived = { 0 };
            UsbSetupPacket setup_in = new UsbSetupPacket((byte)(USB_TYPE_VENDOR | USB_RECIP_INTERFACE | USB_DIR_IN), (byte)RequestType.SetClockDivider, 0, 0, (short)bufferReceived.Length);
            result = device.ControlTransfer(ref setup_in, bufferReceived, bufferReceived.Length, out bytesReceived);

            if (result == false) throw (new Exception("SetClockDivider error while receiving response."));
            if (bytesReceived != bufferReceived.Length) throw (new Exception("Invalid number of bytes received: " + bytesReceived.ToString() + "."));

            if (bufferReceived[0] != clockDividerSetting) throw (new Exception("SetClockDivider failed: " + bufferReceived[0] + "."));
        }
    }
}
