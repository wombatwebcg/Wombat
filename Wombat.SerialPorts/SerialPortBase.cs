using System;
using System.IO.Ports;
using System.Text;
using Wombat.Infrastructure;
using Wombat.SerialPorts.Extensions;

namespace Wombat.SerialPorts
{
    /// <summary>
    /// SerialPorts Util Class.
    /// </summary>
    /// <example>
    /// SerialPorts serial= new SerialPorts("COM1",9600);
    /// serial.UseDataReceived((sp,bytes)=>{});
    /// serial.Open();
    /// </example>
    public class SerialPortBase : ISerialPortBase, IDisposable
    {
        #region Propertys

        private int tryReadNumber;



        public event Action<string> OnReceiveStringData = (data) => { };

        public event Action<byte[]> OnReceiveByteData = (data) => { };

        public event Action<SerialError> OnError = (error) => { };

        public event Action<SerialPinChange> OnPinChange = (ping) => { };

        /// <summary>
        /// 获取和设置数据格式
        /// </summary>
        /// <value>The data format.</value>
        //
        public SerialPortDataFormat DataFormat { get; set; } = SerialPortDataFormat.Hex;

        /// <summary>
        /// 获取和设置尝试读取的次数
        /// </summary>
        /// <value>The try count of receive,default is 3.</value>
        public int TryReadNumber
        {
            get => this.tryReadNumber;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(TryReadNumber), nameof(TryReadNumber) + " must be equal or greater than 1.");
                tryReadNumber = value;
            }
        }

        /// <summary>
        /// 获取和设置尝试读取每次的间隔时间
        /// </summary>
        /// <value>The try sleep time of receive,default is 10.</value>
        //
        public int TryReadSpanTime { get; set; } = 10;

        /// <summary>
        /// 获取和设置终止符号
        /// </summary>
        /// <value>
        /// The terminator.
        /// </value>
        /// <example>
        /// Terminator = "0D 0A"
        /// </example>
        //public byte[] Terminator { get; set; } = null;

        /// <summary>
        /// The serial port
        /// </summary>
        private SerialPort _serialPort;
        private bool disposedValue;

        /// <summary>
        /// SerialPort object.
        /// </summary>
        public SerialPort SerialPort => _serialPort;

        /// <summary>
        /// 获取串口是否打开
        /// </summary>
        /// <returns><c>true</c> if this serialport is open; otherwise, <c>false</c>.</returns>
        public bool IsOpen => _serialPort != null && _serialPort.IsOpen;


        /// <summary>
        /// 获取或设置中断信号状态
        /// </summary>
        /// <value><c>true</c> if [break state]; otherwise, <c>false</c>.</value>
        public bool BreakState
        {
            get => _serialPort.BreakState;
            set => _serialPort.BreakState = value;
        }

        /// <summary>
        /// 编码方式
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding
        {
            get => _serialPort.Encoding;
            set => _serialPort.Encoding = value;
        }

        /// <summary>
        /// 读取串口超时设置
        /// </summary>
        /// <value>The read timeout.</value>
        public int ReadTimeout
        {
            get => _serialPort.ReadTimeout;
            set => _serialPort.ReadTimeout = value;
        }

        /// <summary>
        /// 写入串口超时设置
        /// </summary>
        /// <value>The write timeout.</value>
        public int WriteTimeout
        {
            get => _serialPort.WriteTimeout;
            set => _serialPort.WriteTimeout = value;
        }

        /// <summary>
        /// 串行通信过程中启用数据终端就绪 (DTR) 信号
        /// </summary>
        /// <value><c>true</c> if [DTR enable]; otherwise, <c>false</c>.</value>
        public bool DtrEnable
        {
            get => _serialPort.DtrEnable;
            set => _serialPort.DtrEnable = value;
        }

        /// <summary>
        /// 串行通信中是否启用请求发送 (RTS) 信号
        /// </summary>
        /// <value><c>true</c> if [RTS enable]; otherwise, <c>false</c>.</value>
        public bool RtsEnable
        {
            get => _serialPort.RtsEnable;
            set => _serialPort.RtsEnable = value;
        }



        /// <summary>
        /// 获取“可以发送”行的状态
        /// </summary>
        /// <value><c>true</c> if [CTS holding]; otherwise, <c>false</c>.</value>
        public bool CtsHolding => _serialPort.CtsHolding;

        /// <summary>
        /// 获取数据设置就绪 (DSR) 信号的状态
        /// </summary>
        /// <value><c>true</c> if [DSR holding]; otherwise, <c>false</c>.</value>
        public bool DsrHolding => _serialPort.DsrHolding;

        /// <summary>
        /// 获取端口的载波检测行的状态
        /// </summary>
        /// <value><c>true</c> if [cd holding]; otherwise, <c>false</c>.</value>
        public bool CdHolding => _serialPort.CDHolding;

        /// <summary>
        /// 获取或设置一个值，该值指示 null 字节在端口和接收缓冲区之间传输时是否被忽略
        /// </summary>
        /// <value><c>true</c> if [discard null]; otherwise, <c>false</c>.</value>
        public bool DiscardNull
        {
            get => _serialPort.DiscardNull;
            set => _serialPort.DiscardNull = value;
        }

        /// <summary>
        /// 获取或设置 SerialPort 输入缓冲区的大小
        /// </summary>
        /// <value>The size of the read buffer.</value>
        public int ReadBufferSize
        {
            get => _serialPort.ReadBufferSize;
            set => _serialPort.ReadBufferSize = value;
        }

        public int BytesToRead => _serialPort.BytesToRead;


        /// <summary>
        ///获取或设置一个字节，该字节在发生奇偶校验错误时替换数据流中的无效字节
        /// </summary>
        /// <value>The parity replace.</value>
        public byte ParityReplace
        {
            get => _serialPort.ParityReplace;
            set => _serialPort.ParityReplace = value;
        }

        /// <summary>
        /// DataReceived 事件激发前内部输入缓冲区中的字节数
        /// </summary>
        /// <value>The received bytes threshold.</value>
        public int ReceivedBytesThreshold
        {
            get => _serialPort.ReceivedBytesThreshold;
            set => _serialPort.ReceivedBytesThreshold = value;
        }

        /// <summary>
        /// 获取或设置串行端口输出缓冲区的大小
        /// </summary>
        /// <value>The size of the write buffer.</value>
        public int WriteBufferSize
        {
            get => _serialPort.WriteBufferSize;
            set => _serialPort.WriteBufferSize = value;
        }

        /// <summary>
        /// 串口名称
        /// </summary>
        /// <value>
        /// The name of the port.
        /// </value>
        public string PortName { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        /// <value>
        /// The baud rate.
        /// </value>
        public int BaudRate { get; set; } = 9600;

        /// <summary>
        /// Gets or sets the data bits.
        /// </summary>
        /// <value>
        /// The data bits.
        /// </value>
        public int DataBits { get; set; } = 8;

        /// <summary>
        /// Gets or sets the stop bits.
        /// </summary>
        /// <value>
        /// The stop bits.
        /// </value>
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// Gets or sets the parity bits.
        /// </summary>
        /// <value>
        /// The parity bits.
        /// </value>
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>
        /// Gets or sets the handshake.
        /// </summary>
        /// <value>
        /// The handshake.
        /// </value>
        public Handshake Handshake { get; set; } = Handshake.None;


        #endregion


        public SerialPortBase()
        {
            _serialPort = new SerialPort();

        }
        public SerialPortBase(string portName, int baudRate = 9600, int dataBits = 8, StopBits stopBits = StopBits.One, Parity parity = Parity.None, Handshake handshake = Handshake.None) : this()
        {
            PortName = portName;
            BaudRate = baudRate;
            DataBits = dataBits;
            Handshake = handshake;
            Parity = parity;
            StopBits = stopBits;            
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            OnError?.Invoke(e.EventType);
        }

        private void _serialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            OnPinChange?.Invoke(e.EventType);
        }




        public bool Open()
        {
            _serialPort.PortName = PortName ?? throw new ArgumentNullException(nameof(PortName));
            _serialPort.BaudRate = this.BaudRate;
            _serialPort.DataBits = DataBits;
            _serialPort.Handshake = Handshake;
            _serialPort.Parity = Parity;
            _serialPort.StopBits = StopBits;
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
                _serialPort.PinChanged += _serialPort_PinChanged;
                _serialPort.ErrorReceived += _serialPort_ErrorReceived;

            }
            return _serialPort.IsOpen;
        }



        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }


        #region Reads method

        /// <summary>
        /// 从 SerialPort 输入缓冲区中同步读取一个字节
        /// </summary>
        /// <returns>The byte, cast to an Int32, or -1 if the end of the stream has been read.</returns>
        public int ReadByte()
        {
            int read = 0;
            if (IsOpen)
            {
                read = _serialPort.ReadByte();
            }
            return read;

        }

        /// <summary>
        /// 从 SerialPort 输入缓冲区中同步读取一个字符。
        /// </summary>
        /// <returns>The character that was read.</returns>
        public int ReadChar()
        {
            int read = 0;
            if (IsOpen)
            {
                read = _serialPort.ReadChar();
            }
            return read;

        }

        /// <summary>
        /// 一直读取到输入缓冲区中值直至收到换行符
        /// </summary>
        /// <returns>The contents of the input buffer up to the first occurrence of a NewLine value.</returns>
        public string ReadLine()
        {
            string read = string.Empty;
            if (IsOpen)
            {
                read = _serialPort.ReadLine();
                OnReceiveStringData?.Invoke(read);
            }
            return read;


        }
        /// <summary>
        /// 在编码的基础上，读取 SerialPort 对象的流和输入缓冲区中所有立即可用的字节
        /// </summary>
        /// <returns>The contents of the stream and the input buffer of the SerialPort object.</returns>
        public string ReadExisting()
        {
            string read = string.Empty;
            if (IsOpen)
            {
                read = _serialPort.ReadExisting();
                OnReceiveStringData?.Invoke(read);
            }
            return read;

        }

        /// <summary>
        /// 一直读取到输入缓冲区中的指定的字符串
        /// </summary>
        /// <param name="value">A value that indicates where the read operation stops.</param>
        /// <returns>The contents of the input buffer up to the specified value.</returns>
        public string ReadTo(string value)
        {
            string read = string.Empty;
            if (IsOpen)
            {
                read = _serialPort.ReadTo(value);
                OnReceiveStringData?.Invoke(read);
            }
            return read;


        }
        /// <summary>
        /// 根据设置的数据格式读取字符串
        /// </summary>
        /// <returns>System.String,hex or ascii format.</returns>
        public string ReadString()
        {
            try
            {
                string str = string.Empty;
                if (IsOpen)
                {
                    byte[] bytes = this.Read();
                    OnReceiveByteData?.Invoke(bytes);
                    if (bytes != null && bytes.Length > 0)
                    {
                        switch (DataFormat)
                        {
                            case SerialPortDataFormat.Char:
                                str = _serialPort.Encoding.GetString(bytes);
                                break;
                            case SerialPortDataFormat.Hex:
                                str = bytes.ToHexString();
                                break;
                        }
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 根据缓存区里的字节长度和终止符，尝试读取缓存数据
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] Read()
        {
            byte[] bytes = null;
            if(_serialPort.IsOpen)
            if (_serialPort?.BytesToRead > 0)
            {               
                int dataLength = _serialPort.BytesToRead < _serialPort.ReadBufferSize? _serialPort.BytesToRead: _serialPort.ReadBufferSize;
                bytes = new byte[dataLength];
                _serialPort.Read(bytes, 0, bytes.Length);
                OnReceiveByteData?.Invoke(bytes);
            }
            return bytes;
        }

        #endregion


        #region Writes method
        /// <summary>
        /// Writes the specified hex string.
        /// </summary>
        /// <param name="str">The hex string with space.example:'30 31 32'.</param>
        /// <example>sp.WriteHexString("30 31 32");</example>
        /// <returns>The <see cref="Int32"/> byte number to be written.</returns>
        public int WriteHexString(string str)
        {
            try
            {
                byte[] bytes = str.HexToByte();
                int count = 0;
                if (IsOpen)
                {
                    count = Write(bytes, 0, bytes.Length);
                }
               return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the specified ascii string.
        /// </summary>
        /// <param name="str">The ascii string.</param>
        /// <example>sp.WriteHexString("123");</example>
        /// <returns>The <see cref="Int32"/> byte number to be written.</returns>
        public int WriteAsciiString(string str)
        {
            try
            {
                byte[] bytes = _serialPort.Encoding.GetBytes(str);
                int count = 0;
                if (IsOpen)
                {
                    count = Write(bytes, 0, bytes.Length);
                }
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>The <see cref="Int32"/> byte number to be written.</returns>
        public int Write(byte[] bytes)
        {
            int count = 0;
            if (IsOpen)
            {
                count = Write(bytes, 0, bytes.Length);
            }
            return count;
        }

        /// <summary>
        /// Writes the byte array with offset.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="offset">The number of offset.</param>
        /// <param name="count">The length of write.</param>
        /// <returns>The <see cref="Int32"/> byte number to be written.</returns>
        public int Write(byte[] bytes, int offset, int count)
        {
            try
            {
                if (IsOpen)
                    _serialPort.Write(bytes, offset, count);

                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the specified string and the NewLine value to the output buffer.
        /// </summary>
        /// <param name="str"></param>
        public void WriteLine(string str)
        {
            if (IsOpen)
                _serialPort?.WriteLine(str);

        }


        public int Write(string str)
        {
            try
            {
                if(IsOpen)
                _serialPort?.Write(str);
                return str.Length;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion








        #region SerialPort other method







        /// <summary>
        /// 丢弃来自串行驱动程序的接收缓冲区的数据
        /// </summary>
        public void DiscardInBuffer()
        {
            if(IsOpen)
            _serialPort?.DiscardInBuffer();
        }

        /// <summary>
        /// 丢弃来自串行驱动程序的传输缓冲区的数据
        /// </summary>
        public void DiscardOutBuffer()
        {
            if (IsOpen)
                _serialPort?.DiscardOutBuffer();

        }

        /// <summary>
        /// 丢弃来自串行驱动程序的传输缓冲区的数据
        /// </summary>
        public void BaseStreamFlush() 
        {
            if (IsOpen)
                _serialPort?.BaseStream.Flush();

        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~SerialPorts()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion




    }
}
