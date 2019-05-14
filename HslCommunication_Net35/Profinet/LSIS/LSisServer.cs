using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;

namespace HslCommunication.Profinet.LSIS
{     /// <summary>
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

            inputBuffer = new SoftBuffer(DataPoolLength);
            outputBuffer = new SoftBuffer(DataPoolLength);
            memeryBuffer = new SoftBuffer(DataPoolLength);
            dbBlockBuffer = new SoftBuffer(DataPoolLength);

            WordLength = 2;
            ByteTransform = new ReverseBytesTransform();

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
            OperateResult<string> analysis = XGBFastEnet.AnalysisAddress(address, true);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);
            int startIndex = int.Parse(analysis.Content.Remove(0, 3));
            switch (analysis.Content[1])
            {
                case 'P': return OperateResult.CreateSuccessResult(inputBuffer.GetBytes(startIndex / 8, length));
                case 'Q': return OperateResult.CreateSuccessResult(outputBuffer.GetBytes(startIndex / 8, length));
                case 'M': return OperateResult.CreateSuccessResult(memeryBuffer.GetBytes(startIndex / 8, length));
                case 'D': return OperateResult.CreateSuccessResult(dbBlockBuffer.GetBytes(startIndex / 8, length));
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
            OperateResult<string> analysis = XGBFastEnet.AnalysisAddress(address, false);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysis);
            int startIndex = int.Parse(analysis.Content.Remove(0, 3));
            switch (analysis.Content[1])
            {
                case 'P': inputBuffer.SetBytes(value, startIndex / 8); return OperateResult.CreateSuccessResult();
                case 'Q': outputBuffer.SetBytes(value, startIndex / 8); return OperateResult.CreateSuccessResult();
                case 'M': memeryBuffer.SetBytes(value, startIndex / 8); return OperateResult.CreateSuccessResult();
                case 'D': dbBlockBuffer.SetBytes(value, startIndex / 8); return OperateResult.CreateSuccessResult();
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
            OperateResult<string> analysis = XGBFastEnet.AnalysisAddress(address, true);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<bool>(analysis);
            int startIndex = int.Parse(analysis.Content.Remove(0, 3));
            switch (analysis.Content[1])
            {
                case 'P': return OperateResult.CreateSuccessResult(inputBuffer.GetBool(startIndex));
                case 'Q': return OperateResult.CreateSuccessResult(outputBuffer.GetBool(startIndex));
                case 'M': return OperateResult.CreateSuccessResult(memeryBuffer.GetBool(startIndex));
                case 'D': return OperateResult.CreateSuccessResult(dbBlockBuffer.GetBool(startIndex));
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
            OperateResult<string> analysis = XGBFastEnet.AnalysisAddress(address, false);
            if (!analysis.IsSuccess) return analysis;
            int startIndex = int.Parse(analysis.Content.Remove(0, 3));
            switch (analysis.Content[1])
            {
                case 'P': inputBuffer.SetBool(value, startIndex); return OperateResult.CreateSuccessResult();
                case 'Q': outputBuffer.SetBool(value, startIndex); return OperateResult.CreateSuccessResult();
                case 'M': memeryBuffer.SetBool(value, startIndex); return OperateResult.CreateSuccessResult();
                case 'D': dbBlockBuffer.SetBool(value, startIndex); return OperateResult.CreateSuccessResult();
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

                    LsisFastEnetMessage s7Message = new LsisFastEnetMessage();
                    OperateResult<byte[]> read1 = ReceiveByMessage(session.WorkSocket, 5000, s7Message);
                    if (!read1.IsSuccess)
                    {
                        LogNet?.WriteDebug(ToString(), string.Format(StringResources.Language.ClientOfflineInfo, session.IpEndPoint));
                        RemoveClient(session);
                        return;
                    };

                    byte[] receive = read1.Content;

                    if (receive[20] == 0x54)
                    {
                        // 读数据
                        session.WorkSocket.Send(ReadByMessage(receive));
                    }
                    else if (receive[20] == 0x58)
                    {
                        // 写数据
                        session.WorkSocket.Send(WriteByMessage(receive));
                    }

                    else
                    {
                        session.WorkSocket.Close();
                    }

                    RaiseDataReceived(receive);
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
            bool[] data = null;
            var data2 = new byte[20];
            var result = new List<byte>();
            Array.Copy(command, 0, data2, 0, 20);
            var resultLength = new List<byte>();
            data2[9] = 0x11;
            data2[10] = 0x01;
            data2[12] = 0xA0;
            data2[13] = 0x11;
            data2[18] = 0x03;

            resultLength.Add(85);
            resultLength.Add(0);
            resultLength.Add(20);
            resultLength.Add(0);
            resultLength.Add(0x08);
            resultLength.Add(0x01);
            resultLength.Add(0);
            resultLength.Add(0);
            resultLength.Add(1);
            resultLength.Add(0);
            int NameLength = command[28];
            int RequestCount = BitConverter.ToUInt16(command, 30 + NameLength);
            resultLength.AddRange(BitConverter.GetBytes((ushort)RequestCount));
            string DeviceAddress = Encoding.ASCII.GetString(command, 30, NameLength);
            var StartAddress = DeviceAddress.Remove(0, 3);
            if (DeviceAddress.Substring(1, 2) == "MB" || DeviceAddress.Substring(1, 2) == "PB")
            {
                var dbint = RequestCount * 8;
                int startIndex = int.Parse(DeviceAddress.Remove(0, 3));
                switch (DeviceAddress[1])
                {
                    case 'P':
                        data = inputBuffer.GetBool(startIndex, dbint);
                        break;
                    case 'Q':
                        data = outputBuffer.GetBool(startIndex, dbint);
                        break;
                    case 'M':
                        data = memeryBuffer.GetBool(startIndex, dbint);
                        break;
                    case 'D':
                        data = dbBlockBuffer.GetBool(startIndex, dbint);
                        break;
                    default: throw new Exception(StringResources.Language.NotSupportedDataType);
                }
                var data3 = SoftBasic.BoolArrayToByte(data);
                resultLength.AddRange(data3);
                data2[16] = (byte)resultLength.Count;
                result.AddRange(data2);
                result.AddRange(resultLength);
                return result.ToArray();
            }
            else
            {
                byte[] dataW = null;
                var subAddress = int.Parse(StartAddress) / 2;
                var sublength = RequestCount / 2;
                int startIndex = subAddress;
                switch (DeviceAddress[1])
                {
                    case 'P':
                        dataW = inputBuffer.GetBytes(startIndex, (ushort)RequestCount);
                        break;
                    case 'Q':
                        dataW = outputBuffer.GetBytes(startIndex, (ushort)RequestCount);
                        break;
                    case 'M':
                        dataW = memeryBuffer.GetBytes(startIndex, (ushort)RequestCount);
                        break;
                    case 'D':
                        dataW = dbBlockBuffer.GetBytes(startIndex, (ushort)RequestCount);
                        break;
                    default: throw new Exception(StringResources.Language.NotSupportedDataType);
                }
                resultLength.AddRange(dataW);
                data2[16] = (byte)resultLength.Count;
                result.AddRange(data2);
                result.AddRange(resultLength);
                return result.ToArray();
            }
        }



        private byte[] WriteByMessage(byte[] packCommand)
        {
            int NameLength = packCommand[28];
            int RequestCount = BitConverter.ToUInt16(packCommand, 30 + NameLength);
            int _byte2 = 12 + (int)RequestCount;
            var result = new List<byte>();
            var data5 = new byte[30];
            Array.Copy(packCommand, 0, data5, 0, 30);
            data5[9] = 0x11;
            data5[10] = 0x01;
            data5[12] = 0xA0;
            data5[13] = 0x11;
            data5[16] = (byte)_byte2;
            data5[18] = 0x03;
            data5[20] = 89;
            data5[21] = 0;
            data5[22] = 20;
            data5[23] = 0;
            data5[24] = 0x08;
            data5[25] = 0x01;
            data5[26] = 0;
            data5[27] = 0;
            data5[28] = 1;
            data5[29] = 0;
            result.AddRange(data5);
            result.AddRange(BitConverter.GetBytes((ushort)RequestCount));
            var bitSelacdetAddress = 0;
            var DeviceAddress = Encoding.ASCII.GetString(packCommand, 30, NameLength);
            var AddressLength = BitConverter.ToUInt16(packCommand, 30 + NameLength);

            var tempStrgSelacdetAddress = DeviceAddress.Remove(0, 3);
            switch (tempStrgSelacdetAddress)
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
                    bitSelacdetAddress = int.Parse(DeviceAddress.Remove(0, 3));
                    break;
            }

            int startIndex = int.Parse(DeviceAddress.Remove(0, 3));
            if (DeviceAddress.Substring(1, 2) == "DW" || DeviceAddress.Substring(1, 2) == "DB")
            {

                byte[] data = ByteTransform.TransByte(packCommand, 30 + NameLength + AddressLength, RequestCount);
                switch (DeviceAddress[1])
                {
                    case 'C': inputBuffer.SetBytes(data, startIndex); break;
                    case 'T': outputBuffer.SetBytes(data, startIndex); break;
                    case 'M': memeryBuffer.SetBytes(data, startIndex); break;
                    case 'D': dbBlockBuffer.SetBytes(data, startIndex); break;
                    default: throw new Exception(StringResources.Language.NotSupportedDataType);
                }
                result.AddRange(data);
                return result.ToArray();
            }
            else
            {

                bool value = BitConverter.ToBoolean(packCommand, 30 + NameLength + AddressLength);

                switch (DeviceAddress[1])
                {
                    //case 'M': inputBuffer.SetBool(value, startIndex); break;
                    //case 'M': outputBuffer.SetBool(value, startIndex); break;
                    case 'M': memeryBuffer.SetBool(value, startIndex); break;
                    case 'D': dbBlockBuffer.SetBool(value, startIndex); break;
                    default: throw new Exception(StringResources.Language.NotSupportedDataType);
                }
                result.AddRange(new byte[] { 0, 0 });
                return result.ToArray();
            }
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

            inputBuffer.SetBytes(content, 0, 0, DataPoolLength);
            outputBuffer.SetBytes(content, DataPoolLength, 0, DataPoolLength);
            memeryBuffer.SetBytes(content, DataPoolLength * 2, 0, DataPoolLength);
            dbBlockBuffer.SetBytes(content, DataPoolLength * 3, 0, DataPoolLength);
        }

        /// <summary>
        /// 将数据信息存储到字节数组去
        /// </summary>
        /// <returns>所有的内容</returns>
        protected override byte[] SaveToBytes()
        {
            byte[] buffer = new byte[DataPoolLength * 4];
            Array.Copy(inputBuffer.GetBytes(), 0, buffer, 0, DataPoolLength);
            Array.Copy(outputBuffer.GetBytes(), 0, buffer, DataPoolLength, DataPoolLength);
            Array.Copy(memeryBuffer.GetBytes(), 0, buffer, DataPoolLength * 2, DataPoolLength);
            Array.Copy(dbBlockBuffer.GetBytes(), 0, buffer, DataPoolLength * 3, DataPoolLength);

            return buffer;
        }


        #endregion

        #region Private Member

        private SoftBuffer inputBuffer;                // 输入寄存器的数据池
        private SoftBuffer outputBuffer;               // 离散输入的数据池
        private SoftBuffer memeryBuffer;               // 寄存器的数据池
        private SoftBuffer dbBlockBuffer;              // 输入寄存器的数据池
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
        private byte[] bufferReceiver;
        /// <summary>
        /// 接收到串口数据的时候触发
        /// </summary>
        /// <param name="sender">串口对象</param>
        /// <param name="e">消息</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            if (sp.BytesToRead >= 5)
            {
                bufferReceiver = new byte[serialPort.BytesToRead];
                var result = serialPort.Read(bufferReceiver, 0, serialPort.BytesToRead);
                ProcessReceivedData(bufferReceiver);
            }
 
          

             
        }
        private readonly object LockObject = new object();
        public bool ResponseReceived = false;
        private void ProcessReceivedData(byte[] receive)
        {
            byte[] modbusCore = SoftBasic.BytesArrayRemoveLast(receive, 2);
            lock (LockObject)
            {
                var bufferMsgReceiver = string.Empty;

                if (receive == null || receive.Length < 1)
                {

                    ResponseReceived = true;
                }
                else
                {

                    bufferMsgReceiver = Encoding.UTF8.GetString(modbusCore, 0, modbusCore.Length);
                    byte[] copy = ReadFromModbusCore(modbusCore);

                    serialPort.Write(copy, 0, copy.Length);
                    if (IsStarted) RaiseDataReceived(receive);

                }
            }

        }
 
        public static string GetValStr(byte[] Buff, int iStart, int iDataSize)
        {
            var strVal = string.Empty;
            var strByteVal = string.Empty;
            var i = 0;

            for (i = 0; i < iDataSize; i++)
            {
                strByteVal = Convert.ToString(Buff[i + iStart], 16).ToUpper();
                if (strByteVal.Length == 1) strByteVal = "0" + strByteVal;
                strVal = strByteVal + strVal;
            }

            return strVal;
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
        private byte[] ReadFromModbusCore(byte[] packet)
        {
            List<byte> command = new List<byte>();
            command.Clear();
            var StartAddress = string.Empty;
             var station = Encoding.ASCII.GetString(packet, 1, 2);
            var Read = Encoding.ASCII.GetString(packet, 3, 3);
            var nameLength = Encoding.ASCII.GetString(packet, 6, 2);
            var DeviceAddress = Encoding.ASCII.GetString(packet, 8, int.Parse(nameLength));
            var size = Encoding.ASCII.GetString(packet, 8 + int.Parse(nameLength), 2);
            //=====================================================================================
            // Read Response
            if (Read.Substring(0, 2) == "rS")
            {

                command.Add(0x06);    // ENQ
                command.AddRange(SoftBasic.BuildAsciiBytesFrom(byte.Parse(station)));
                command.Add(0x72);    // command r
                command.Add(0x53);    // command type: SB
                command.Add(0x42);
                command.AddRange(Encoding.ASCII.GetBytes("01"));
                StartAddress = DeviceAddress.Remove(0, 3);
                bool[] data;
                string txtValue;
                switch (DeviceAddress.Substring(1, 2))
                {
                    case "MB":
                    case "PB":
                        var dbint = Convert.ToInt32(size, 16) * 8;
                        int startIndex = int.Parse(StartAddress);
                        switch (DeviceAddress[1])
                        {
                            case 'P':
                                data = inputBuffer.GetBool(startIndex, dbint);
                                break;
                            case 'Q':
                                data = outputBuffer.GetBool(startIndex, dbint);
                                break;
                            case 'M':
                                data = memeryBuffer.GetBool(startIndex, dbint);
                                break;
                            case 'D':
                                data = dbBlockBuffer.GetBool(startIndex, dbint);
                                break;
                            default: throw new Exception(StringResources.Language.NotSupportedDataType);
                        }
                        var data3 = SoftBasic.BoolArrayToByte(data);
                        txtValue = GetValStr(data3, 0, Convert.ToInt32(size, 16));
                        command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)data3.Length));
                        command.AddRange(SoftBasic.BytesToAsciiBytes(data3));
                        command.Add(0x03);    // ETX
                        int sum1 = 0;
                        for (int i = 0; i < command.Count; i++)
                        {
                            sum1 += command[i];
                        }
                        command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)sum1));
                        break;
                    case "DB":
                    case "TB":
                        var RequestCount = Convert.ToInt32(size, 16);
                        byte[] dataW;
                        var startIndexW = int.Parse(StartAddress);
                        switch (DeviceAddress[1])
                        {
                            case 'P':
                                dataW = inputBuffer.GetBytes(startIndexW, (ushort)RequestCount);
                                break;
                            case 'Q':
                                dataW = outputBuffer.GetBytes(startIndexW, (ushort)RequestCount);
                                break;
                            case 'M':
                                dataW = memeryBuffer.GetBytes(startIndexW, (ushort)RequestCount);
                                break;
                            case 'D':
                                dataW = dbBlockBuffer.GetBytes(startIndexW, (ushort)RequestCount);
                                break;
                            default: throw new Exception(StringResources.Language.NotSupportedDataType);
                        }
                        txtValue = GetValStr(dataW, 0, Convert.ToInt32(size, 16));
                        command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)dataW.Length));
                        command.AddRange(SoftBasic.BytesToAsciiBytes(dataW));
                        command.Add(0x03);    // ETX
                        int sum = 0;
                        for (int i = 0; i < command.Count; i++)
                        {
                            sum += command[i];
                        }
                        command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)sum));
                        break;
                }
                return command.ToArray();
            }
            else
            {
                StartAddress = DeviceAddress.Remove(0, 3);
                command.Add(0x06);    // ENQ
                command.AddRange(SoftBasic.BuildAsciiBytesFrom(byte.Parse(station)));
                command.Add(0x77);    // command w
                command.Add(0x53);    // command type: SB
                command.Add(0x42);
                command.Add(0x03);    // EOT
                string Value;
                if (Read.Substring(0, 3) == "WSS")
                {
                    //nameLength = packet.Substring(8, 1);
                    //DeviceAddress = packet.Substring(9, Convert.ToInt16(nameLength));
                    //AddressLength = packet.Substring(9 + Convert.ToInt16(nameLength), 1);
                    nameLength = Encoding.ASCII.GetString(packet, 8, 2);
                    DeviceAddress = Encoding.ASCII.GetString(packet, 10, int.Parse(nameLength));
                    Value = Encoding.ASCII.GetString(packet, 10 + int.Parse(nameLength), 2);
                }
                else
                {
                    //Value = Encoding.ASCII.GetString(packet, 10 + int.Parse(nameLength), int.Parse(size));
                    Value = Encoding.ASCII.GetString(packet, 8 + int.Parse(nameLength)+ int.Parse(size), int.Parse(size) * 2);
                    var wdArys = HexToBytes(Value);
                }

                int bitSelacdetAddress;
                switch (StartAddress)
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
                        bitSelacdetAddress = int.Parse(StartAddress);
                        break;
                }
                var startIndex = bitSelacdetAddress;
                switch (DeviceAddress.Substring(1, 2))
                {
                    case "MX": // Bit X
                        Value = Encoding.ASCII.GetString(packet, 8 + int.Parse(nameLength) + int.Parse(size), int.Parse(size));
                        switch (DeviceAddress[1])
                        {
                            //case 'M': inputBuffer.SetBool(value, startIndex); break;
                            //case 'M': outputBuffer.SetBool(value, startIndex); break;
                            case 'M': memeryBuffer.SetBool(Value == "01" ? true : false, startIndex); break;
                            case 'D': dbBlockBuffer.SetBool(Value == "01" ? true : false, startIndex); break;
                            default: throw new Exception(StringResources.Language.NotSupportedDataType);
                        }
                        return command.ToArray();
                    case "DW": //Word
                        Value = Encoding.ASCII.GetString(packet, 8 + int.Parse(nameLength) + int.Parse(size), int.Parse(size) * 2);
                        var wdArys = HexToBytes(Value);
                        switch (DeviceAddress[1])
                        {
                            case 'C': inputBuffer.SetBytes(wdArys, startIndex); break;
                            case 'T': outputBuffer.SetBytes(wdArys, startIndex); break;
                            case 'M': memeryBuffer.SetBytes(wdArys, startIndex); break;
                            case 'D': dbBlockBuffer.SetBytes(wdArys, startIndex); break;
                            default: throw new Exception(StringResources.Language.NotSupportedDataType);
                        }
                        return command.ToArray();
                    case "DD": //DWord


                        break;
                    case "DL": //LWord

                        break;

                    default:

                        return null;
                }
            }
            return command.ToArray();
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
