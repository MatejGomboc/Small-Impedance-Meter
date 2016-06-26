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
    public class ControlRequestException : Exception, ISerializable
    {
        public ControlRequestException() : base() { }

        public ControlRequestException(string message) : base(message) { }

        public ControlRequestException(string message, Exception innerException) : base(message, innerException) { }

        // This constructor is needed for serialization.
        protected ControlRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class DataLengthException : Exception, ISerializable
    {
        public DataLengthException() : base() { }

        public DataLengthException(string message) : base(message) { }

        public DataLengthException(string message, Exception innerException) : base(message, innerException) { }

        // This constructor is needed for serialization.
        protected DataLengthException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public enum AD5933_VoutRange : byte
    {
        Range_2Vpp = 0x00,
        Range_200mVpp = 0x01,
        Range_400mVpp = 0x02,
        Range_1Vpp = 0x03
    }

    public enum AD5933_PGAGain : byte
    {
        X5 = 0,
        X1 = 1
    }

    public enum AD5933_MCLK : byte
    {
        Internal = 0,
        External = 1
    }

    internal enum ControlRequestType : byte
    {
        Ping = 0,
        InitClkGen = 1,
        SetClkGen = 2,
        SwitchToCalibrate = 3,
        SwitchToMeasure = 4,
        AD5933_Init = 5,
        AD5933_RegisterWrite = 6,
        AD5933_SetAddressPointer = 7,
        AD5933_BlockWrite = 8,
        AD5933_RegisterRead = 9,
        AD5933_BlockRead= 10,
        AD5933_InitWithStartFreq = 11,
        AD5933_IncrementFreq = 12,
        AD5933_RepeatFreq = 13,
        AD5933_StartMeasureTemp = 14,
        AD5933_Powerdown = 15,
        AD5933_Standby = 16,
        AD5933_SelectVoutRange = 17,
        AD5933_SetPGAGain = 18,
        AD5933_Reset = 19,
        AD5933_SelectMCLK = 20,
        AD5933_SetStartFreq = 21,
        AD5933_GetStartFreq = 22,
        AD5933_SetFreqIncrement = 23,
        AD5933_GetFreqIncrement = 24,
        AD5933_SetNumOfIncrements = 25,
        AD5933_GetNumOfIncrements = 26,
        AD5933_SetNumOfSettlingCycles = 27,
        AD5933_GetNumOfSettlingCycles = 28,
        AD5933_GetTemp = 29,
        AD5933_GetReal = 30,
        AD5933_GetImaginary = 31,
        AD5933_CheckValidTemp = 32,
        AD5933_CheckSweepComplete = 33,
        AD5933_CheckValidData = 34,
        AD5933_MeasureTemp = 35
    };

    public partial class SmallImpedanceMeter
    {
        public void SendRawControl(byte bRequest, byte[] bufferSent)
        {
            const byte USB_TYPE_VENDOR = (0x02 << 5);
            const byte USB_RECIP_INTERFACE = 0x01;
            const byte USB_DIR_OUT = 0x00;

            if (!connected) throw (new DeviceNotFoundException("Device disconnected."));

            int bytesSent = 0;
            UsbSetupPacket setup_out = new UsbSetupPacket((byte)(USB_TYPE_VENDOR | USB_RECIP_INTERFACE | USB_DIR_OUT), bRequest, 0, 0, (short)bufferSent.Length);
            bool result = device.ControlTransfer(ref setup_out, bufferSent, bufferSent.Length, out bytesSent);

            if (result == false) throw (new ControlRequestException("Error while sending raw request."));
            if (bytesSent != bufferSent.Length) throw (new DataLengthException("Invalid number of bytes sent: " + bytesSent.ToString() + "."));
        }

        public void ReceiveRawControl(byte bRequest, ref byte[] bufferReceived)
        {
            const byte USB_TYPE_VENDOR = (0x02 << 5);
            const byte USB_RECIP_INTERFACE = 0x01;
            const byte USB_DIR_IN = 0x80;
            
            if (!connected) throw (new DeviceNotFoundException("Device disconnected."));

            int bytesReceived = 0;
            UsbSetupPacket setup_in = new UsbSetupPacket((byte)(USB_TYPE_VENDOR | USB_RECIP_INTERFACE | USB_DIR_IN), bRequest, 0, 0, (short)bufferReceived.Length);
            bool result = device.ControlTransfer(ref setup_in, bufferReceived, bufferReceived.Length, out bytesReceived);

            if (result == false) throw (new ControlRequestException("Error while receiving raw response."));
            if (bytesReceived != bufferReceived.Length) throw (new DataLengthException("Invalid number of bytes received: " + bytesReceived.ToString() + "."));
        }

        public void Ping()
        {
            try
            {
                byte[] bufferSent = { 1, 2, 3, 4, 5 };

                SendRawControl((byte)ControlRequestType.Ping, bufferSent);

                byte[] bufferReceived = { 0, 0, 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.Ping, ref bufferReceived);

                for (int i = 0; i < bufferReceived.Length; i++)
                {
                    if (bufferReceived[i] != bufferSent[i]) throw (new DataLengthException("Received bytes not equal to sent bytes."));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ping error.", ex);
            }
        }

        public void InitClockDivider()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.InitClkGen, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.InitClkGen, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("InitClockDivider failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("InitClockDivider error.", ex);
            }
        }

        public void SetClockDivider(byte clockDividerSetting)
        {
            try
            {
                byte[] bufferSent = { clockDividerSetting };

                SendRawControl((byte)ControlRequestType.SetClkGen, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.SetClkGen, ref bufferReceived);

                if (bufferReceived[0] != clockDividerSetting) throw (new Exception("SetClockDivider failed: " + bufferReceived[0] + "."));
            }
            catch (Exception ex)
            {
                throw new Exception("SetClockDivider error.", ex);
            }
        }

        public void SetToCalibrate()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.SwitchToCalibrate, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.SwitchToCalibrate, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("SetToCalibrate failed: " + bufferReceived[0] + "."));
            }
            catch (Exception ex)
            {
                throw new Exception("SetToCalibrate error.", ex);
            }
        }

        public void SetToMeasure()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.SwitchToMeasure, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.SwitchToMeasure, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("SetToMeasure failed: " + bufferReceived[0] + "."));
            }
            catch (Exception ex)
            {
                throw new Exception("SetToMeasure error.", ex);
            }
        }

        public void AD5933_Init()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_Init, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_Init, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 initialization failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 initialization error.", ex);
            }
        }

        public void AD5933_RegisterWrite(byte registerAddress, byte registerData)
        {
            try
            {
                byte[] bufferSent = { registerAddress, registerData };

                SendRawControl((byte)ControlRequestType.AD5933_RegisterWrite, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_RegisterWrite, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 register write failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 register write error.", ex);
            }
        }

        public void AD5933_SetRegisterAddressPointer(byte registerAddress)
        {
            try
            {
                byte[] bufferSent = { registerAddress };

                SendRawControl((byte)ControlRequestType.AD5933_SetAddressPointer, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_SetAddressPointer, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("Setting AD5933 register address failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("Setting AD5933 register address error.", ex);
            }
        }

        public void AD5933_RegisterBlockWrite(byte[] registerData)
        {
            try
            {
                byte[] bufferSent = new byte[1 + registerData.Length];

                bufferSent[0] = (byte)registerData.Length;

                for (int i = 0; i < registerData.Length; i++)
                {
                    bufferSent[1 + i] = registerData[i];
                }

                SendRawControl((byte)ControlRequestType.AD5933_BlockWrite, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_BlockWrite, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 register block write failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 register block write error.", ex);
            }
        }

        public byte AD5933_RegisterRead(byte registerAddress)
        {
            try
            {
                byte[] bufferSent = { registerAddress };

                SendRawControl((byte)ControlRequestType.AD5933_RegisterRead, bufferSent);

                byte[] bufferReceived = { 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_RegisterRead, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 register read failed."));

                return bufferReceived[1]; // register data
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 register read error.", ex);
            }
        }

        public byte[] AD5933_RegisterBlockRead(byte length)
        {
            try
            {
                byte[] bufferSent = { length };

                SendRawControl((byte)ControlRequestType.AD5933_BlockRead, bufferSent);

                byte[] bufferReceived = new byte[length + 1];

                ReceiveRawControl((byte)ControlRequestType.AD5933_BlockRead, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 register block read failed."));

                byte[] registerData = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    registerData[i] = bufferReceived[1 + i];
                }

                return registerData; // register data
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 register block read error.", ex);
            }
        }

        public void AD5933_InitWithStartFreq()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_InitWithStartFreq, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_InitWithStartFreq, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 initialize with start frequency failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 initialize with start frequency error.", ex);
            }
        }

        public void AD5933_IncrementFreq()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_IncrementFreq, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_IncrementFreq, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 increment frequency failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 increment frequency error.", ex);
            }
        }

        public void AD5933_RepeatFreq()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_RepeatFreq, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_RepeatFreq, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 repeat frequency failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 repeat frequency error.", ex);
            }
        }

        public void AD5933_StartMeasureTemp()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_StartMeasureTemp, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_StartMeasureTemp, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 start temperature measurement failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 start temperature measurement error.", ex);
            }
        }

        public void AD5933_Powerdown()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_Powerdown, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_Powerdown, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 powedown failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 powedown error.", ex);
            }
        }

        public void AD5933_Standby()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_Standby, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_Standby, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 standby failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 standby error.", ex);
            }
        }

        public void AD5933_SelectVoutRange(AD5933_VoutRange VoutRange)
        {
            try
            {
                byte[] bufferSent = { (byte)VoutRange };

                SendRawControl((byte)ControlRequestType.AD5933_SelectVoutRange, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_SelectVoutRange, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 select Vout range failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 select Vout range error.", ex);
            }
        }

        public void AD5933_SetPGAGain(AD5933_PGAGain PGAGain)
        {
            try
            {
                byte[] bufferSent = { (byte)PGAGain };

                SendRawControl((byte)ControlRequestType.AD5933_SetPGAGain, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_SetPGAGain, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 select PGA gain failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 select PGA gain error.", ex);
            }
        }

        public void AD5933_Reset()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_Reset, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_Reset, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 reset failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 reset error.", ex);
            }
        }

        public void AD5933_SelectMCLK(AD5933_MCLK MCLK_Selection)
        {
            try
            {
                byte[] bufferSent = { (byte)MCLK_Selection };

                SendRawControl((byte)ControlRequestType.AD5933_SelectMCLK, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_SelectMCLK, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 select MCLK failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 select MCLK error.", ex);
            }
        }

        public void AD5933_SetStartFreq(UInt32 frequencySetting)
        {
            try
            {
                if (frequencySetting > 0x00FFFFFF) throw (new ArgumentOutOfRangeException("Frequency setting out of range."));

                byte[] bufferSent = { (byte)((frequencySetting & 0x00FF0000) >> 16),
                                      (byte)((frequencySetting & 0x0000FF00) >> 8),
                                      (byte)(frequencySetting & 0x000000FF) };

                SendRawControl((byte)ControlRequestType.AD5933_SetStartFreq, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_SetStartFreq, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 set start frequency failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 set start frequency error.", ex);
            }
        }

        public UInt32 AD5933_GetStartFreq()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_GetStartFreq, bufferSent);

                byte[] bufferReceived = { 0, 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_GetStartFreq, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 get start frequency failed."));

                UInt32 frequencySetting = (((UInt32)bufferReceived[1]) << 16) | (((UInt32)bufferReceived[2]) << 8) | ((UInt32)bufferReceived[3]);

                return frequencySetting;
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 get start frequency error.", ex);
            }
        }

        public void AD5933_SetFreqIncrement(UInt32 frequencyIncrementSetting)
        {
            try
            {
                if (frequencyIncrementSetting > 0x00FFFFFF) throw (new ArgumentOutOfRangeException("Frequency increment setting out of range."));

                byte[] bufferSent = { (byte)((frequencyIncrementSetting & 0x00FF0000) >> 16),
                                      (byte)((frequencyIncrementSetting & 0x0000FF00) >> 8),
                                      (byte)(frequencyIncrementSetting & 0x000000FF) };

                SendRawControl((byte)ControlRequestType.AD5933_SetFreqIncrement, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_SetFreqIncrement, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 set frequency increment failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 set frequency increment error.", ex);
            }
        }

        public UInt32 AD5933_GetFreqIncrement()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_GetFreqIncrement, bufferSent);

                byte[] bufferReceived = { 0, 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_GetFreqIncrement, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 get frequency increment failed."));

                UInt32 frequencyIncrementSetting = (((UInt32)bufferReceived[1]) << 16) | (((UInt32)bufferReceived[2]) << 8) | ((UInt32)bufferReceived[3]);

                return frequencyIncrementSetting;
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 get frequency increment error.", ex);
            }
        }

        public void AD5933_SetNumOfIncrements(UInt16 numOfIncrements)
        {
            try
            {
                byte[] bufferSent = { (byte)((numOfIncrements & 0xFF00) >> 8),
                                      (byte)(numOfIncrements & 0x00FF) };

                SendRawControl((byte)ControlRequestType.AD5933_SetNumOfIncrements, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_SetNumOfIncrements, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 set number of increments failed."));
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 set number of increments error.", ex);
            }
        }

        public UInt16 AD5933_GetNumOfIncrements()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_GetNumOfIncrements, bufferSent);

                byte[] bufferReceived = { 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_GetNumOfIncrements, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 get number of increments failed."));

                UInt16 numOfIncrements = (UInt16)((UInt16)(((UInt16)bufferReceived[1]) << 8) | ((UInt16)bufferReceived[2]));
                
                return numOfIncrements;
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 get number of increments error.", ex);
            }
        }

        public uint AD5933_SetNumOfSettlingCycles(uint numOfSettlingCycles)
        {
            try
            {
                UInt16 setting = 0;
                byte settlingCyclesMultiplier = 0;
                uint sentValue = 0;

                if ((numOfSettlingCycles > 0) && (numOfSettlingCycles <= 511))
                {
                    settlingCyclesMultiplier = 0x00;
                    sentValue = numOfSettlingCycles;
                    setting = (UInt16)sentValue;
                }
                else if ((numOfSettlingCycles > 511) && (numOfSettlingCycles <= 1022))
                {
                    settlingCyclesMultiplier = 0x02;
                    decimal fractional = (decimal)numOfSettlingCycles / (decimal)2;
                    setting = (UInt16)(Math.Round(fractional, MidpointRounding.AwayFromZero));
                    sentValue = (uint)setting * 2;
                }
                else if ((numOfSettlingCycles > 1022) && (numOfSettlingCycles <= 2044))
                {
                    settlingCyclesMultiplier = 0x03;
                    decimal fractional = (decimal)numOfSettlingCycles / (decimal)4;
                    setting = (UInt16)(Math.Round(fractional, MidpointRounding.AwayFromZero));
                    sentValue = (uint)setting * 4;
                }
                else throw (new ArgumentOutOfRangeException("Num of settling cycles out of range."));

                byte[] bufferSent = { (byte)(((byte)((setting & 0x0100) >> 8)) | ((byte)((settlingCyclesMultiplier & 0x03) << 1))),
                                      (byte)(setting & 0x00FF) };

                SendRawControl((byte)ControlRequestType.AD5933_SetNumOfSettlingCycles, bufferSent);

                byte[] bufferReceived = { 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_SetNumOfSettlingCycles, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 set number of settling cycles failed."));

                return sentValue;
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 set number of settling cycles error.", ex);
            }
        }

        public uint AD5933_GetNumOfSettlingCycles()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_GetNumOfSettlingCycles, bufferSent);

                byte[] bufferReceived = { 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_GetNumOfSettlingCycles, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 get number of settling cycles failed."));

                uint numOfSettlingCycles = (UInt16)((UInt16)((((UInt16)bufferReceived[1]) & 0x01) << 8) | ((UInt16)bufferReceived[2]));

                byte settlingCyclesMultiplier = (byte)((byte)((byte)(bufferReceived[1] & 0x07) >> 1) & 0x03);

                switch (settlingCyclesMultiplier)
                {
                    case 0x00:
                        return numOfSettlingCycles;
                    case 0x01:
                        return (numOfSettlingCycles * 2);
                    case 0x03:
                        return (numOfSettlingCycles * 4);
                    default:
                        throw (new ArgumentOutOfRangeException("Settling cycles multiplier out of range."));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 get number of settling cycles error.", ex);
            }
        }

        public UInt16 AD5933_GetTemp()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_GetTemp, bufferSent);

                byte[] bufferReceived = { 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_GetTemp, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 get temperature failed."));

                UInt16 temperatureWord = (UInt16)((((UInt16)bufferReceived[1]) << 8) | ((UInt16)bufferReceived[2]));

                return temperatureWord;
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 get temperature error.", ex);
            }
        }

        public UInt16 AD5933_GetRealValue()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_GetReal, bufferSent);

                byte[] bufferReceived = { 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_GetReal, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 get real value failed."));

                UInt16 realValue = (UInt16)((((UInt16)bufferReceived[1]) << 8) | ((UInt16)bufferReceived[2]));

                return realValue;
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 get real value error.", ex);
            }
        }

        public UInt16 AD5933_GetImaginaryValue()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_GetImaginary, bufferSent);

                byte[] bufferReceived = { 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_GetImaginary, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 get imaginary value failed."));

                UInt16 imaginaryValue = (UInt16)((((UInt16)bufferReceived[1]) << 8) | ((UInt16)bufferReceived[2]));

                return imaginaryValue;
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 get imaginary value error.", ex);
            }
        }

        public bool AD5933_CheckValidTemp()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_CheckValidTemp, bufferSent);

                byte[] bufferReceived = { 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_CheckValidTemp, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 check valid temperature failed."));

                return Convert.ToBoolean(bufferReceived[1]);
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 check valid temperature error.", ex);
            }
        }

        public bool AD5933_CheckFrequencySweepComplete()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_CheckSweepComplete, bufferSent);

                byte[] bufferReceived = { 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_CheckSweepComplete, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 check frequency sweep complete failed."));

                return Convert.ToBoolean(bufferReceived[1]);
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 check frequency sweep complete error.", ex);
            }
        }

        public bool AD5933_CheckValidData()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_CheckValidData, bufferSent);

                byte[] bufferReceived = { 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_CheckValidData, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 check valid data failed."));

                return Convert.ToBoolean(bufferReceived[1]);
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 check valid data error.", ex);
            }
        }

        public UInt16 AD5933_MeasureTemp()
        {
            try
            {
                byte[] bufferSent = { 0 };

                SendRawControl((byte)ControlRequestType.AD5933_MeasureTemp, bufferSent);

                byte[] bufferReceived = { 0, 0, 0 };

                ReceiveRawControl((byte)ControlRequestType.AD5933_MeasureTemp, ref bufferReceived);

                if (bufferReceived[0] != 1) throw (new Exception("AD5933 measure temperature failed."));

                UInt16 temperatureWord = (UInt16)((((UInt16)bufferReceived[1]) << 8) | ((UInt16)bufferReceived[2]));

                return temperatureWord;
            }
            catch (Exception ex)
            {
                throw new Exception("AD5933 measure temperature error.", ex);
            }
        }
    }
}
