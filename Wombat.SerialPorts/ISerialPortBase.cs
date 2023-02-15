using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using Wombat.SerialPorts.Extensions;

namespace Wombat.SerialPorts
{
    public interface ISerialPortBase: ISerialPortsBaseParms
    {
        SerialPort SerialPort { get; }
        bool IsOpen { get; }
        bool BreakState { get; set; }
        Encoding Encoding { get; set; }
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }
        bool DtrEnable { get; set; }
        bool RtsEnable { get; set; }
        bool CtsHolding { get; }
        bool DsrHolding { get; }
        bool CdHolding { get; }
        bool DiscardNull { get; set; }
        int ReadBufferSize { get; set; }
        int BytesToRead { get; }
        byte ParityReplace { get; set; }
        int ReceivedBytesThreshold { get; set; }
        int WriteBufferSize { get; set; }
        int TryReadSpanTime { get; set; }
        SerialPortDataFormat DataFormat { get; set; }
        int TryReadNumber { get; set; }

        event Action<SerialPinChange> OnPinChange;
        event Action<SerialError> OnError;
        event Action<byte[]> OnReceiveByteData;
        event Action<string> OnReceiveStringData;

        void Close();
        void DiscardInBuffer();
        void DiscardOutBuffer();
        void Dispose();
        bool Open();
        byte[] Read();
        int ReadByte();
        int ReadChar();
        string ReadExisting();
        string ReadLine();
        string ReadString();
        string ReadTo(string value);
        int Write(byte[] bytes);
        int Write(byte[] bytes, int offset, int count);
        int Write(string str);
        int WriteAsciiString(string str);
        int WriteHexString(string str);
        void WriteLine(string str);

    }
}
