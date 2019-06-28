using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
#if !NETSTANDARD2_0
using System.IO.Ports;
#endif
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Profinet.LSIS
{
    /// <summary>
    /// LSisServer
    /// </summary>
    public class LSisServer : NetworkDataServerBase
    {
        #region Constructor

        /// <summary>
        /// LSisServer  
        /// </summary>
        public LSisServer()
        {

            pBuffer = new SoftBuffer(DataPoolLength);
            qBuffer = new SoftBuffer(DataPoolLength);
            mBuffer = new SoftBuffer(DataPoolLength);
            dBuffer = new SoftBuffer(DataPoolLength);

            WordLength = 2;
            ByteTransform = new RegularByteTransform();

#if !NETSTANDARD2_0
            serialPort = new SerialPort();
#endif
        }

        #endregion

        #region NetworkDataServerBase Override

        /// <summary>
        /// 读取自定义的寄存器的值
        /// </summary>
        /// <param name="address">起始地址，示例："I100"，"M100"</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>byte数组值</returns>
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<string> analysis = XGBFastEnet.AnalysisAddress(address);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            int startIndex = CheckAddress(analysis.Content.Substring(3));
            switch (analysis.Content[1])
            {
                case 'P': return OperateResult.CreateSuccessResult(pBuffer.GetBytes(startIndex, length));
                case 'Q': return OperateResult.CreateSuccessResult(qBuffer.GetBytes(startIndex, length));
                case 'M': return OperateResult.CreateSuccessResult(SoftBasic.BoolArrayToByte(mBuffer.GetBool(startIndex , length *= 8)));
                case 'D': return OperateResult.CreateSuccessResult(dBuffer.GetBytes(startIndex==0 ? startIndex : startIndex*=2, length));
                default: return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
            }
        }

        /// <summary>
        /// 写入自定义的数据到数据内存中去
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功的结果对象</returns>
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<string> analysis = XGBFastEnet.AnalysisAddress(address);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);

            int startIndex = CheckAddress(analysis.Content.Substring(3));
            switch (analysis.Content[1])
            {
                case 'P': pBuffer.SetBytes(value, startIndex); return OperateResult.CreateSuccessResult();
                case 'Q': qBuffer.SetBytes(value, startIndex); return OperateResult.CreateSuccessResult();
                case 'M': mBuffer.SetBool(value[0]==1 ? true:false, startIndex); return OperateResult.CreateSuccessResult();
                case 'D': dBuffer.SetBytes(value, startIndex == 0 ? startIndex : startIndex *= 2); return OperateResult.CreateSuccessResult();
                default: return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
            }
        }

        #endregion

        #region Byte Read Write Operate

        /// <summary>
        /// 读取指定地址的字节数据
        /// </summary>
        /// <param name="address">西门子的地址信息</param>
        /// <returns>带有成功标志的结果对象</returns>
        public OperateResult<byte> ReadByte(string address)
        {
            OperateResult<byte[]> read = Read(address, 2);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte>(read);

            return OperateResult.CreateSuccessResult(read.Content[0]);
        }

        /// <summary>
        /// 将byte数据信息写入到指定的地址当中
        /// </summary>
        /// <param name="address">西门子的地址信息</param>
        /// <param name="value">字节数据信息</param>
        /// <returns>是否成功的结果</returns>
        public OperateResult Write(string address, byte value)
        {
            return Write(address, new byte[] { value });
        }

        #endregion

        #region Bool Read Write Operate

        /// <summary>
        /// 读取指定地址的bool数据对象
        /// </summary>
        /// <param name="address">西门子的地址信息</param>
        /// <returns>带有成功标志的结果对象</returns>
        public OperateResult<bool> ReadBool(string address)
        {
            OperateResult<string> analysis = XGBFastEnet.AnalysisAddress(address);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<bool>(analysis);

            // to do, this is not right
            int startIndex = CheckAddress(analysis.Content.Substring(3));
            switch (analysis.Content[1])
            {
                case 'P': return OperateResult.CreateSuccessResult(pBuffer.GetBool(startIndex));
                case 'Q': return OperateResult.CreateSuccessResult(qBuffer.GetBool(startIndex));
                case 'M': return OperateResult.CreateSuccessResult(mBuffer.GetBool(startIndex));
                case 'D': return OperateResult.CreateSuccessResult(dBuffer.GetBool(startIndex));
                default: return new OperateResult<bool>(StringResources.Language.NotSupportedDataType);
            }
        }

        /// <summary>
        /// 往指定的地址里写入bool数据对象
        /// </summary>
        /// <param name="address">西门子的地址信息</param>
        /// <param name="value">值</param>
        /// <returns>是否成功的结果</returns>
        public OperateResult Write(string address, bool value)
        {
            OperateResult<string> analysis = XGBFastEnet.AnalysisAddress(address);
            if (!analysis.IsSuccess) return analysis;

            // to do, this is not right
            int startIndex = CheckAddress(analysis.Content.Substring(3));
            switch (analysis.Content[1])
            {
                case 'P': pBuffer.SetBool(value, startIndex); return OperateResult.CreateSuccessResult();
                case 'Q': qBuffer.SetBool(value, startIndex); return OperateResult.CreateSuccessResult();
                case 'M': mBuffer.SetBool(value, startIndex); return OperateResult.CreateSuccessResult();
                case 'D': dBuffer.SetBool(value, startIndex); return OperateResult.CreateSuccessResult();
                default: return new OperateResult(StringResources.Language.NotSupportedDataType);
            }
        }

        #endregion

        #region NetServer Override

        /// <summary>
        /// 当客户端登录后，进行Ip信息的过滤，然后触发本方法，也就是说之后的客户端需要
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="endPoint">终端节点</param>
        protected override void ThreadPoolLoginAfterClientCheck(Socket socket, System.Net.IPEndPoint endPoint)
        {
            // 开始接收数据信息
            AppSession appSession = new AppSession();
            appSession.IpEndPoint = endPoint;
            appSession.WorkSocket = socket;
            try
            {
                socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(SocketAsyncCallBack), appSession);
                AddClient(appSession);
            }
            catch
            {
                socket.Close();
                LogNet?.WriteDebug(ToString(), string.Format(StringResources.Language.ClientOfflineInfo, endPoint));
            }
        }

        private void SocketAsyncCallBack(IAsyncResult ar)
        {
            if (ar.AsyncState is AppSession session)
            {
                try
                {
                    int receiveCount = session.WorkSocket.EndReceive(ar);

                    LsisFastEnetMessage fastEnetMessage = new LsisFastEnetMessage();
                    OperateResult<byte[]> read1 = ReceiveByMessage(session.WorkSocket, 5000, fastEnetMessage);
                    if (!read1.IsSuccess)
                    {
                        LogNet?.WriteDebug(ToString(), string.Format(StringResources.Language.ClientOfflineInfo, session.IpEndPoint));
                        RemoveClient(session);
                        return;
                    };

                    byte[] receive = read1.Content;
                    byte[] SendData = null;
                    if (receive[20] == 0x54)
                    {
                        // 读数据
                         SendData = ReadByMessage(receive);
                        RaiseDataReceived(SendData);
                        session.WorkSocket.Send(SendData);
                       
                    }
                    else if (receive[20] == 0x58)
                    {
                         SendData = WriteByMessage(receive);
                        RaiseDataReceived(SendData);
                        session.WorkSocket.Send(SendData);
                    }
                    else
                    {
                        session.WorkSocket.Close();
                    }

                    RaiseDataSend(receive);
                    session.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(SocketAsyncCallBack), session);
                }
                catch
                {
                    // 关闭连接，记录日志
                    session.WorkSocket?.Close();
                    LogNet?.WriteDebug(ToString(), string.Format(StringResources.Language.ClientOfflineInfo, session.IpEndPoint));
                    RemoveClient(session);
                    return;
                }
            }
        }

        private byte[] ReadByMessage(byte[] packCommand)
        {
            List<byte> content = new List<byte>();

            content.AddRange(ReadByCommand(packCommand));


            return content.ToArray();
        }

        private byte[] ReadByCommand(byte[] command)
        {
            var result = new List<byte>();

            result.AddRange(SoftBasic.BytesArraySelectBegin(command, 20));
            result[9] = 0x11;
            result[10] = 0x01;
            result[12] = 0xA0;
            result[13] = 0x11;
            result[18] = 0x03;
            result.AddRange(new byte[] { 0x55, 0x00, 0x14, 0x00, 0x08, 0x01, 0x00, 0x00, 0x01, 0x00 });

            int NameLength = command[28];
            ushort RequestCount = BitConverter.ToUInt16(command, 30 + NameLength);

            string DeviceAddress = Encoding.ASCII.GetString(command, 31, NameLength - 1);
            byte[] data = Read(DeviceAddress, RequestCount).Content;

            result.AddRange(BitConverter.GetBytes((ushort)data.Length));
            result.AddRange(data);
            result[16] = (byte)(result.Count - 20);
            return result.ToArray();
        }
        private byte[] WriteByMessage(byte[] packCommand)
        {
            var result = new List<byte>();

            result.AddRange(SoftBasic.BytesArraySelectBegin(packCommand, 20));
            result[9] = 0x11;
            result[10] = 0x01;
            result[12] = 0xA0;
            result[13] = 0x11;
            result[18] = 0x03;
            result.AddRange(new byte[] { 0x59, 0x00, 0x14, 0x00, 0x08, 0x01, 0x00, 0x00, 0x01, 0x00 });

            int NameLength = packCommand[28];
            var DeviceAddress = Encoding.ASCII.GetString(packCommand, 31, NameLength - 1);
            int RequestCount = BitConverter.ToUInt16(packCommand, 30 + NameLength);

            byte[] data = ByteTransform.TransByte(packCommand, 32 + NameLength, RequestCount);
            Write(DeviceAddress, data);
            result[16] = (byte)(result.Count - 20);
            return result.ToArray();
        }

        #endregion

        #region Data Save Load Override

        /// <summary>
        /// 从字节数据加载数据信息
        /// </summary>
        /// <param name="content">字节数据</param>
        protected override void LoadFromBytes(byte[] content)
        {
            if (content.Length < DataPoolLength * 4) throw new Exception("File is not correct");

            pBuffer.SetBytes(content, 0, 0, DataPoolLength);
            qBuffer.SetBytes(content, DataPoolLength, 0, DataPoolLength);
            mBuffer.SetBytes(content, DataPoolLength * 2, 0, DataPoolLength);
            dBuffer.SetBytes(content, DataPoolLength * 3, 0, DataPoolLength);
        }

        /// <summary>
        /// 将数据信息存储到字节数组去
        /// </summary>
        /// <returns>所有的内容</returns>
        protected override byte[] SaveToBytes()
        {
            byte[] buffer = new byte[DataPoolLength * 4];
            Array.Copy(pBuffer.GetBytes(), 0, buffer, 0, DataPoolLength);
            Array.Copy(qBuffer.GetBytes(), 0, buffer, DataPoolLength, DataPoolLength);
            Array.Copy(mBuffer.GetBytes(), 0, buffer, DataPoolLength * 2, DataPoolLength);
            Array.Copy(dBuffer.GetBytes(), 0, buffer, DataPoolLength * 3, DataPoolLength);

            return buffer;
        }

        public static int CheckAddress(string address)
        {
            int bitSelacdetAddress;
            switch (address)
            {
                case "A":
                    bitSelacdetAddress = 10;
                    break;
                case "B":
                    bitSelacdetAddress = 11;
                    break;
                case "C":
                    bitSelacdetAddress = 12;
                    break;
                case "D":
                    bitSelacdetAddress = 13;
                    break;
                case "E":
                    bitSelacdetAddress = 14;
                    break;
                case "F":
                    bitSelacdetAddress = 15;
                    break;

                default:
                    bitSelacdetAddress = int.Parse(address);
                    break;
            }
            return bitSelacdetAddress;
        }

        #endregion

        #region Private Member

        private SoftBuffer pBuffer;                    // p data type
        private SoftBuffer qBuffer;                    // q data type
        private SoftBuffer mBuffer;                    // 寄存器的数据池
        private SoftBuffer dBuffer;                    // 输入寄存器的数据池
        private const int DataPoolLength = 65536;      // 数据的长度

        #endregion

        #region Serial Support
        private int station = 1;
#if !NETSTANDARD2_0

        private SerialPort serialPort;            // 核心的串口对象

        /// <summary>
        /// 使用默认的参数进行初始化串口，9600波特率，8位数据位，无奇偶校验，1位停止位
        /// </summary>
        /// <param name="com">串口信息</param>
        public void StartSerialPort(string com)
        {
            StartSerialPort(com, 9600);
        }

        /// <summary>
        /// 使用默认的参数进行初始化串口，8位数据位，无奇偶校验，1位停止位
        /// </summary>
        /// <param name="com">串口信息</param>
        /// <param name="baudRate">波特率</param>
        public void StartSerialPort(string com, int baudRate)
        {
            StartSerialPort(sp =>
            {
                sp.PortName = com;
                sp.BaudRate = baudRate;
                sp.DataBits = 8;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;
            });
        }

        /// <summary>
        /// 使用自定义的初始化方法初始化串口的参数
        /// </summary>
        /// <param name="inni">初始化信息的委托</param>
        public void StartSerialPort(Action<SerialPort> inni)
        {
            if (!serialPort.IsOpen)
            {
                inni?.Invoke(serialPort);

                serialPort.ReadBufferSize = 1024;
                serialPort.ReceivedBytesThreshold = 1;
                serialPort.Open();
                serialPort.DataReceived += SerialPort_DataReceived;
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void CloseSerialPort()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
       
        /// <summary>
        /// 接收到串口数据的时候触发
        /// </summary>
        /// <param name="sender">串口对象</param>
        /// <param name="e">消息</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           
              var sp = (SerialPort)sender;

            int rCount = 0;
            byte[] buffer = new byte[1024];
            byte[] receive = null;

            while (true)
            {
                System.Threading.Thread.Sleep(20);           
                int count = sp.Read(buffer, rCount, sp.BytesToRead);
                rCount += count;
                if (count == 0) break;

                receive = new byte[rCount];
                Array.Copy(buffer, 0, receive, 0, count);
            }

            if (receive == null) return;
            byte[] modbusCore = SoftBasic.BytesArrayRemoveLast(receive, 2);
            byte[] SendData = null;
            if (modbusCore[3] == 0x72)
            {
                // Read
                SendData = ReadSerialByCommand(modbusCore);
                RaiseDataReceived(SendData);
                serialPort.Write(SendData, 0, SendData.Length);

            }
            else if (modbusCore[3] == 0x77)
            {
                // Write
                SendData = WriteSerialByMessage(modbusCore);
                RaiseDataReceived(SendData);
                serialPort.Write(SendData, 0, SendData.Length);
                
            }
            else
            {
                serialPort.Close();
            }
           
            if (IsStarted) RaiseDataSend(receive);
            
        }
        public byte[] HexToBytes(string hex)
        {
            if (hex == null)
                throw new ArgumentNullException("The data is null");

            if (hex.Length % 2 != 0)
                throw new FormatException("Hex Character Count Not Even");

            var bytes = new byte[hex.Length / 2];

            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return bytes;
        }
        private byte[] ReadSerialByCommand(byte[] command)
        {
            var result = new List<byte>();


            result.Add(0x06);    // ENQ
            result.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)station));
            result.Add(0x72);    // command r
            result.Add(0x53);    // command type: SB
            result.Add(0x42);
            result.AddRange(Encoding.ASCII.GetBytes("01"));
           

            int NameLength =int.Parse(Encoding.ASCII.GetString(command, 6, 2));
            int RequestCount = Convert.ToInt32(Encoding.ASCII.GetString(command, 8 + NameLength, 2),16);

            string DeviceAddress = Encoding.ASCII.GetString(command, 9, NameLength - 1);
            byte[] data = Read(DeviceAddress,(ushort) RequestCount).Content;

            result.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)data.Length));
            result.AddRange(SoftBasic.BytesToAsciiBytes(data));
            result.Add(0x03);    // ETX

            int sum1 = 0;
            for (int i = 0; i < result.Count; i++)
            {
                sum1 += result[i];
            }
            result.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)sum1));
            return result.ToArray();
        }
        private byte[] WriteSerialByMessage(byte[] packCommand)
        {
            var result = new List<byte>();
            string NameLength, DeviceAddress;
            var Read = Encoding.ASCII.GetString(packCommand, 3, 3);
            result.Add(0x06);    // ENQ
            result.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)station));
            result.Add(0x77);    // command w
            result.Add(0x53);    // command type: SB
            result.Add(0x42);
            result.Add(0x03);    // EOT

            if (Read == "wSS")
            {
                NameLength = Encoding.ASCII.GetString(packCommand, 8, 2);
                DeviceAddress = Encoding.ASCII.GetString(packCommand, 11, int.Parse(NameLength) - 1);

                string data = Encoding.ASCII.GetString(packCommand, 10 + int.Parse(NameLength), 2);
                Write(DeviceAddress, new byte[] { (byte)(data == "01" ? 0x01 : 0x00) });
            }
            else
            {
                NameLength = Encoding.ASCII.GetString(packCommand, 6, 2);
                DeviceAddress = Encoding.ASCII.GetString(packCommand,9, int.Parse(NameLength)-1);
                
                int RequestCount = int.Parse( Encoding.ASCII.GetString(packCommand, 8 + int.Parse(NameLength), 2));
                string Value = Encoding.ASCII.GetString(packCommand, 8 + int.Parse(NameLength) +  RequestCount, RequestCount * 2);
                var wdArys = HexToBytes(Value);
                Write(DeviceAddress, wdArys);
             }


          
            
            return result.ToArray();
        }

#endif

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return $"LSisServer[{Port}]";
        }

        #endregion
    }
}