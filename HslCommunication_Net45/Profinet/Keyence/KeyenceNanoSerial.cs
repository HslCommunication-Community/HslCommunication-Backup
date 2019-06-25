using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Keyence
{
    /// <summary>
    /// 基恩士KV上位链路串口通信的对象,适用于Nano系列串口数据,以及L20V通信模块
    /// </summary>
    /// <remarks>
    /// 地址的输入的格式说明如下：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>示例</term>
    ///     <term>地址进制</term>
    ///     <term>字操作</term>
    ///     <term>位操作</term>
    ///     <term>KV-7500/7300</term>
    ///     <term>KV-5500/5000/3000</term>
    ///     <term>KV Nano</term>
    ///   </listheader>
    ///   <item>
    ///     <term>输入继电器</term>
    ///     <term>X</term>
    ///     <term>X100,X1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>R00000~R99915</term>
    ///     <term>R00000~R99915</term>
    ///     <term>R00000～R59915</term>
    ///   </item>
    ///   <item>
    ///     <term>输出继电器</term>
    ///     <term>Y</term>
    ///     <term>Y100,Y1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>R00000~R99915</term>
    ///     <term>R00000~R99915</term>
    ///     <term>R00000～R59915</term>
    ///   </item>
    ///   <item>
    ///     <term>内部辅助继电器</term>
    ///     <term>MR</term>
    ///     <term>MR100,M200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>MR00000~MR99915</term>
    ///     <term>MR00000~MR99915</term>
    ///     <term>MR00000～MR59915</term>
    ///   </item>
    ///   <item>
    ///     <term>数据存储器</term>
    ///     <term>DM</term>
    ///     <term>DM100,DM200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>DM00000~DM65534</term>
    ///     <term>DM00000~DM65534</term>
    ///     <term>DM00000～DM32767</term>
    ///   </item>
    ///   <item>
    ///     <term>定时器（当前值）</term>
    ///     <term>TN</term>
    ///     <term>TN100,TN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>T0000~T3999</term>
    ///     <term>T0000~T3999</term>
    ///     <term>T000～T511</term>
    ///   </item>
    ///   <item>
    ///     <term>定时器（接点）</term>
    ///     <term>TS</term>
    ///     <term>TS100,TS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>T0000~T3999</term>
    ///     <term>T0000~T3999</term>
    ///     <term>T000～T511</term>
    ///   </item>
    ///   <item>
    ///     <term>计数器（当前值）</term>
    ///     <term>CN</term>
    ///     <term>CN100,CN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>C0000~C3999</term>
    ///     <term>C0000~C3999</term>
    ///     <term>C000～C255</term>
    ///   </item>
    ///   <item>
    ///     <term>计数器（接点）</term>
    ///     <term>CS</term>
    ///     <term>CS100,CS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>C0000~C3999</term>
    ///     <term>C0000~C3999</term>
    ///     <term>C000～C255</term>
    ///   </item>
    /// </list>
    /// </remarks>
    public class KeyenceNanoSerial : SerialDeviceBase<KeyenceNanoByteTransform>
    { 
        #region Constructor

        /// <summary>
        /// 实例化基恩士的串口协议的通讯对象
        /// </summary>
        public KeyenceNanoSerial()
        {
            WordLength = 1;
        }

        /// <summary>
        /// 初始化后建立通讯连接
        /// </summary>
        /// <returns>是否初始化成功</returns>
        protected override OperateResult InitializationOnOpen()
        {
            // 建立通讯连接{CR/r}
            var result = ReadBase(_buildConnectCmd);
            if (!result.IsSuccess) return result;

            return OperateResult.CreateSuccessResult();
        }
        #endregion

        #region Check Response

       /// <summary>
       /// 校验读取返回数据状态
       /// </summary>
       /// <param name="ack"></param>
       /// <returns></returns>
        private OperateResult CheckPlcReadResponse(byte[] ack)
        {
            if (ack.Length == 0) return new OperateResult(StringResources.Language.MelsecFxReceiveZore);
            if (ack[0] == 0x45) return new OperateResult(StringResources.Language.MelsecFxAckWrong + " Actual: " + SoftBasic.ByteToHexString(ack, ' '));
            if ((ack[ack.Length - 1]!=0x0A) &&(ack[ack.Length -2]!=0x0D)) return new OperateResult(StringResources.Language.MelsecFxAckWrong + " Actual: " + SoftBasic.ByteToHexString(ack, ' '));
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 校验写入返回数据状态
        /// </summary>
        /// <param name="ack"></param>
        /// <returns></returns>
        private OperateResult CheckPlcWriteResponse(byte[] ack)
        {
            if (ack.Length == 0) return new OperateResult(StringResources.Language.MelsecFxReceiveZore);
        
            return OperateResult.CreateSuccessResult();
        }

        #endregion

        #region Read Support

        /// <summary>
        /// 建立读取指令
        /// </summary>
        /// <param name="address">软元件地址</param>
        /// <param name="length">读取长度</param>
        /// <returns>是否建立成功</returns>
        private OperateResult<byte[]> BuildReadCommand(string address, ushort length)
        {
            var addressResult = KvCalculateWordStartAddress(address);
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(addressResult);

            ushort startAddress = addressResult.Content;

            StringBuilder StrCommand = new StringBuilder();
            StrCommand.Append("RDS");                        //批量读取
            StrCommand.Append(" ");                          //空格符
            StrCommand.Append(address);                      //软元件地址，如DM100
            StrCommand.Append(" ");                          //空格符
            StrCommand.Append(length.ToString());
            StrCommand.Append("\r");                         //结束符

            byte[] _PLCCommand = Encoding.ASCII.GetBytes(StrCommand.ToString().ToCharArray());

            return OperateResult.CreateSuccessResult(_PLCCommand);

        }

        /// <summary>
        /// 读取设备的short类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public new OperateResult<short> ReadInt16(string address)
        {
            var result= ReadInt16(address, 1);
            if (!result.IsSuccess) return OperateResult.CreateFailedResult<short>(result); 
            return OperateResult.CreateSuccessResult(result.Content[0]); 
        }

        /// <summary>
        /// 读取设备的short类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public new OperateResult<short[]> ReadInt16(string address, ushort length)
        { 
            address += ".S";
            return base.ReadInt16(address, length);
        }

        /// <summary>
        /// 读取设备的ushort数据类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public new OperateResult<ushort> ReadUInt16(string address)
        {
            var result = ReadUInt16(address, 1);
            if (!result.IsSuccess) return OperateResult.CreateFailedResult<ushort>(result);
            return OperateResult.CreateSuccessResult(result.Content[0]);
        }

        /// <summary>
        /// 读取设备的ushort类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public new OperateResult<ushort[]> ReadUInt16(string address, ushort length)
        { 
            address += ".U";
            return base.ReadUInt16(address, length);
        }

        /// <summary>
        /// 读取设备的int类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public new OperateResult<int> ReadInt32(string address)
        {
            var result = ReadInt32(address, 1);
            if (!result.IsSuccess) return OperateResult.CreateFailedResult<int>(result);
            return OperateResult.CreateSuccessResult(result.Content[0]);
        }

        /// <summary>
        /// 读取设备的int类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public new OperateResult<int[]> ReadInt32(string address, ushort length)
        { 
            address += ".L";
            return base.ReadInt32(address, length);
        }

        /// <summary>
        /// 读取设备的uint类型的数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public new OperateResult<uint> ReadUInt32(string address)
        {
            var result = ReadUInt32(address, 1);
            if (!result.IsSuccess) return OperateResult.CreateFailedResult<uint>(result);
            return OperateResult.CreateSuccessResult(result.Content[0]);
        }

        /// <summary>
        /// 读取设备的uint类型的数组
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public new OperateResult<uint[]> ReadUInt32(string address, ushort length)
        { 
            address += ".D";
            return base.ReadUInt32(address, length);
        }

        /// <summary>
        /// 从PLC中读取想要的数据，返回读取结果
        /// </summary>
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            // 获取指令
            OperateResult<byte[]> command = BuildReadCommand(address, length);
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(command);

            // 核心交互
            OperateResult<byte[]> read = ReadBase(command.Content);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(read);

            // 反馈检查
            OperateResult ackResult = CheckPlcReadResponse(read.Content);
            if (!ackResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(ackResult);

            // 数据提炼
            return ExtractActualData(read.Content);
        }

        /// <summary>
        /// 成批读取Bool值
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            var strBuffer = Encoding.Default.GetString(Read(address, length).Content).Split(' ');
         
            var result = new bool[strBuffer.Length];
            for (int i = 0; i < length; i++)
            {
                result[i] = strBuffer[i] == "1" ? true : false;
            }
            return OperateResult.CreateSuccessResult(result);
        }

        #endregion

        #region Write Support

        /// <summary>
        /// 写入转换后的数据值
        /// </summary>
        /// <param name="address">软元件地址</param>
        /// <param name="value">转换后的Byte[]数据</param>
        /// <returns>是否成功写入的结果</returns>
        public override OperateResult Write(string address, byte[] value)
        {
            // 获取写入
            OperateResult<byte[]> command = BuildWriteCommand(address, value);
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadBase(command.Content);
            if (!read.IsSuccess) return read;

            // 结果验证
            OperateResult checkResult = CheckPlcWriteResponse(read.Content);
            if (!checkResult.IsSuccess) return checkResult;

            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        ///  写入位数据的通断，支持的类型参考文档说明
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">是否为通</param>
        /// <returns>是否写入成功的结果对象</returns>
        public override OperateResult Write(string address, bool value)
        {
            //value=true时:指令尾部命令为" 1 1";value=false:指令尾部命令为" 1 0";
            var byteTemp = value ? new byte[] {0x20, 0x31,0x20,0x31 } : new byte[] { 0x20, 0x31, 0x20, 0x30 };
            // 先获取指令
            OperateResult<byte[]> command = BuildWriteCommand(address, byteTemp);
            if (!command.IsSuccess) return command;

            // 和串口进行核心的数据交互
            OperateResult<byte[]> read = ReadBase(command.Content);
            if (!read.IsSuccess) return read;

            // 检查结果是否正确
            OperateResult checkResult = CheckPlcWriteResponse(read.Content);
            if (!checkResult.IsSuccess) return checkResult;

            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 建立写入指令
        /// </summary>
        /// <param name="address">软元件地址</param>
        /// <param name="value">转换后的数据</param>
        /// <returns></returns>
        private OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
        {
            var addressResult = KvCalculateWordStartAddress(address);
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(addressResult);

            ushort startAddress = addressResult.Content;

            StringBuilder StrCommandHead = new StringBuilder();
            StrCommandHead.Append("WRS");           //批量读取
            StrCommandHead.Append(" ");                 //空格符
            StrCommandHead.Append(address);        //软元件地址，如DM100；可加格式后缀，如DM100.S（表示有符号16位十进制）


            byte[] _CommandHead = Encoding.ASCII.GetBytes(StrCommandHead.ToString().ToCharArray());
            byte[] result = new byte[value.Length + _CommandHead.Length + 1];
            Array.Copy(_CommandHead, result, _CommandHead.Length);
            Array.Copy(value, 0, result, _CommandHead.Length, value.Length);
            result[result.Length - 1] = 0x0d;
            return OperateResult.CreateSuccessResult(result);                  // Return 
        }

        #endregion

        #region Private Member

        private byte[] _buildConnectCmd = new byte[3] { 0x43, 0x52, 0x0d };     // 建立通讯连接{CR/r}
        private byte[] _writeOkReturn = new byte[] { 0x4f, 0x4b, 0x0d, 0x0a };  // 写入数据成功返回指令

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return $"KeyenceNanoSerial[{PortName}:{BaudRate}]";
        }

        #endregion

        #region Static Method Helper

        /// <summary>
        /// 从PLC反馈的数据进行提炼操作
        /// </summary>
        /// <param name="response">PLC反馈的真实数据</param>
        /// <returns>数据提炼后的真实数据</returns>
        public static OperateResult<byte[]> ExtractActualData(byte[] response)
        {
            try
            {
                string strResponse = Encoding.Default.GetString(response);
                byte[] data = new byte[response.Length - 2];
                Array.Copy(response, data, response.Length - 2);
                return OperateResult.CreateSuccessResult(data);
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>()
                {
                    Message = "Extract Msg：" + ex.Message + Environment.NewLine +
                    "Data: " + BasicFramework.SoftBasic.ByteToHexString(response)
                };
            }
        }

        /// <summary>
        /// 返回读取的地址及长度信息
        /// </summary>
        /// <param name="address">读取的地址信息</param>
        /// <returns>带起始地址的结果对象</returns>
        private static OperateResult<ushort> KvCalculateWordStartAddress(string address)
        {
            // 初步解析，失败就返回
            var analysis = KvAnalysisAddress(address);
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<ushort>(analysis);
            // 二次解析
            ushort startAddress = analysis.Content2;
            return OperateResult.CreateSuccessResult(startAddress);
        }

        /// <summary>
        /// 解析数据地址成不同的Keyence地址类型
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>地址结果对象</returns>
        private static OperateResult<KeyenceDataType, ushort> KvAnalysisAddress(string address)
        {
            var result = new OperateResult<KeyenceDataType, ushort>();
            try
            {
                //删除软元件后缀，如DM100.L—>DM100
                if (address.Contains("."))
                {
                    address = address.Substring(0, address.Length - 2);
                }
                switch (address[0])
                {
                    case 'M':
                    case 'm':
                        {
                            result.Content1 = KeyenceDataType.M;

                            if (address[1] == 'R' || address[1] == 'r')
                            {
                                result.Content2 = Convert.ToUInt16(address.Substring(2), KeyenceDataType.M.FromBase);

                            }
                            else
                            {
                                result.Content2 = Convert.ToUInt16(address.Substring(1), KeyenceDataType.M.FromBase);
                            }
                            break;
                        }
                    case 'R':
                    case 'r':
                        {
                            result.Content1 = KeyenceDataType.R;

                            result.Content2 = Convert.ToUInt16(address.Substring(1), KeyenceDataType.R.FromBase);

                            break;
                        }
                    case 'X':
                    case 'x':
                        {
                            result.Content1 = KeyenceDataType.X;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), 8);
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            result.Content1 = KeyenceDataType.Y;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), 8);
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            result.Content1 = KeyenceDataType.D;
                            if (address[1] == 'M' || address[1] == 'm')
                            {
                                result.Content2 = Convert.ToUInt16(address.Substring(2), KeyenceDataType.D.FromBase);

                            }
                            else
                            {
                                result.Content2 = Convert.ToUInt16(address.Substring(1), KeyenceDataType.D.FromBase);
                            }
                            break;
                        }
                    case 'T':
                    case 't':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                result.Content1 = KeyenceDataType.TN;
                                result.Content2 = Convert.ToUInt16(address.Substring(2), KeyenceDataType.TN.FromBase);
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                result.Content1 = KeyenceDataType.TS;
                                result.Content2 = Convert.ToUInt16(address.Substring(2), KeyenceDataType.TS.FromBase);
                                break;
                            }
                            else
                            {
                                throw new Exception(StringResources.Language.NotSupportedDataType);
                            }
                        }
                    case 'C':
                    case 'c':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                result.Content1 = KeyenceDataType.CN;
                                result.Content2 = Convert.ToUInt16(address.Substring(2), KeyenceDataType.CN.FromBase);
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                result.Content1 = KeyenceDataType.CS;
                                result.Content2 = Convert.ToUInt16(address.Substring(2), KeyenceDataType.CS.FromBase);
                                break;
                            }
                            else
                            {
                                throw new Exception(StringResources.Language.NotSupportedDataType);
                            }
                        }
                    default: throw new Exception(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }

            result.IsSuccess = true;
            return result;
        }

        #endregion

    }
}
