using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
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

            switch (analysis.Content[1])
            {
                case 'P': return OperateResult.CreateSuccessResult(inputBuffer.GetBytes(int.Parse(analysis.Content.Remove(0, 3)) / 8, length));
                case 'Q': return OperateResult.CreateSuccessResult(outputBuffer.GetBytes(int.Parse(analysis.Content.Remove(0, 3)) / 8, length));
                case 'M': return OperateResult.CreateSuccessResult(memeryBuffer.GetBytes(int.Parse(analysis.Content.Remove(0, 3)) / 8, length));
                case 'D': return OperateResult.CreateSuccessResult(dbBlockBuffer.GetBytes(int.Parse(analysis.Content.Remove(0, 3)) / 8, length));
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

            switch (analysis.Content[0])
            {
                case 'P': inputBuffer.SetBytes(value, int.Parse(analysis.Content.Remove(0, 3)) / 8); return OperateResult.CreateSuccessResult();
                case 'Q': outputBuffer.SetBytes(value, int.Parse(analysis.Content.Remove(0, 3)) / 8); return OperateResult.CreateSuccessResult();
                case 'M': memeryBuffer.SetBytes(value, int.Parse(analysis.Content.Remove(0, 3)) / 8); return OperateResult.CreateSuccessResult();
                case 'D': dbBlockBuffer.SetBytes(value, int.Parse(analysis.Content.Remove(0, 3)) / 8); return OperateResult.CreateSuccessResult();
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

            switch (analysis.Content[1])
            {
                case 'P': return OperateResult.CreateSuccessResult(inputBuffer.GetBool(int.Parse(analysis.Content.Remove(0, 3))));
                case 'Q': return OperateResult.CreateSuccessResult(outputBuffer.GetBool(int.Parse(analysis.Content.Remove(0, 3))));
                case 'M': return OperateResult.CreateSuccessResult(memeryBuffer.GetBool(int.Parse(analysis.Content.Remove(0, 3))));
                case 'D': return OperateResult.CreateSuccessResult(dbBlockBuffer.GetBool(int.Parse(analysis.Content.Remove(0, 3))));
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

            switch (analysis.Content[1])
            {
                case 'P': inputBuffer.SetBool(value, int.Parse(analysis.Content.Remove(0, 3))); return OperateResult.CreateSuccessResult();
                case 'Q': outputBuffer.SetBool(value, int.Parse(analysis.Content.Remove(0, 3))); return OperateResult.CreateSuccessResult();
                case 'M': memeryBuffer.SetBool(value, int.Parse(analysis.Content.Remove(0, 3))); return OperateResult.CreateSuccessResult();
                case 'D': dbBlockBuffer.SetBool(value, int.Parse(analysis.Content.Remove(0, 3))); return OperateResult.CreateSuccessResult();
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

                //int count = ByteTransform.TransInt16(packCommand, 23);

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
