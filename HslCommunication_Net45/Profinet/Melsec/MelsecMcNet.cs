using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Address;

namespace HslCommunication.Profinet.Melsec
{

    /// <summary>
    /// 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯
    /// </summary>
    /// <remarks>
    /// 目前组件测试通过的PLC型号列表，有些来自于网友的测试
    /// <list type="number">
    /// <item>Q06UDV PLC  感谢hwdq0012</item>
    /// <item>fx5u PLC  感谢山楂</item>
    /// <item>Q02CPU PLC </item>
    /// <item>L02CPU PLC </item>
    /// </list>
    /// 地址的输入的格式说明如下：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>示例</term>
    ///     <term>地址进制</term>
    ///     <term>字操作</term>
    ///     <term>位操作</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>内部继电器</term>
    ///     <term>M</term>
    ///     <term>M100,M200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输入继电器</term>
    ///     <term>X</term>
    ///     <term>X100,X1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输出继电器</term>
    ///     <term>Y</term>
    ///     <term>Y100,Y1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///    <item>
    ///     <term>锁存继电器</term>
    ///     <term>L</term>
    ///     <term>L100,L200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>报警器</term>
    ///     <term>F</term>
    ///     <term>F100,F200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>边沿继电器</term>
    ///     <term>V</term>
    ///     <term>V100,V200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>链接继电器</term>
    ///     <term>B</term>
    ///     <term>B100,B1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>步进继电器</term>
    ///     <term>S</term>
    ///     <term>S100,S200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>数据寄存器</term>
    ///     <term>D</term>
    ///     <term>D1000,D2000</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>链接寄存器</term>
    ///     <term>W</term>
    ///     <term>W100,W1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>文件寄存器</term>
    ///     <term>R</term>
    ///     <term>R100,R200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>ZR文件寄存器</term>
    ///     <term>ZR</term>
    ///     <term>ZR100,ZR2A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>变址寄存器</term>
    ///     <term>Z</term>
    ///     <term>Z100,Z200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器的触点</term>
    ///     <term>TS</term>
    ///     <term>TS100,TS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器的线圈</term>
    ///     <term>TC</term>
    ///     <term>TC100,TC200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器的当前值</term>
    ///     <term>TN</term>
    ///     <term>TN100,TN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>累计定时器的触点</term>
    ///     <term>SS</term>
    ///     <term>SS100,SS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>累计定时器的线圈</term>
    ///     <term>SC</term>
    ///     <term>SC100,SC200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>累计定时器的当前值</term>
    ///     <term>SN</term>
    ///     <term>SN100,SN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器的触点</term>
    ///     <term>CS</term>
    ///     <term>CS100,CS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器的线圈</term>
    ///     <term>CC</term>
    ///     <term>CC100,CC200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器的当前值</term>
    ///     <term>CN</term>
    ///     <term>CN100,CN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="Usage" title="简单的短连接使用" />
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="Usage2" title="简单的长连接使用" />
    /// </example>
    public class MelsecMcNet : NetworkDeviceBase<MelsecQnA3EBinaryMessage, RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        public MelsecMcNet( )
        {
            WordLength = 1;
        }

        /// <summary>
        /// 实例化一个三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public MelsecMcNet( string ipAddress, int port )
        {
            WordLength = 1;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Public Member

        /// <summary>
        /// 网络号，通常为0
        /// </summary>
        /// <remarks>
        /// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
        /// </remarks>
        public byte NetworkNumber { get; set; } = 0x00;

        /// <summary>
        /// 网络站号，通常为0
        /// </summary>
        /// <remarks>
        /// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
        /// </remarks>
        public byte NetworkStationNumber { get; set; } = 0x00;


        #endregion

        #region Virtual Address Analysis

        /// <summary>
        /// 分析地址的方法，允许派生类里进行重写操作
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>解析后的数据信息</returns>
        protected virtual OperateResult<McAddressData> McAnalysisAddress( string address, ushort length )
        {
            return McAddressData.ParseMelsecFrom( address, length );
        }

        #endregion

        #region Read Write Support

        /// <summary>
        /// 从三菱PLC中读取想要的数据，输入地址，按照字单位读取，返回读取结果
        /// </summary>
        /// <param name="address">读取地址，格式为"M100","D100","W1A0"</param>
        /// <param name="length">读取的数据长度，字最大值960，位最大值7168</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <remarks>
        /// 地址支持的列表参考 <seealso cref="MelsecMcNet"/> 的备注说明
        /// </remarks>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，读取如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAnalysisAddress( address, length );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            List<byte> bytesContent = new List<byte>( );
            ushort alreadyFinished = 0;
            while (alreadyFinished < length)
            {
                ushort readLength = (ushort)Math.Min( length - alreadyFinished, 900 );
                addressResult.Content.Length = readLength;
                OperateResult<byte[]> read = ReadAddressData( addressResult.Content );
                if (!read.IsSuccess) return read;

                bytesContent.AddRange( read.Content );
                alreadyFinished += readLength;

                // 字的话就是正常的偏移位置，如果是位的话，就转到位的数据
                if (addressResult.Content.McDataType.DataType == 0)
                    addressResult.Content.AddressStart += readLength;
                else
                    addressResult.Content.AddressStart += readLength * 16;
            }
            return OperateResult.CreateSuccessResult( bytesContent.ToArray( ) );
        }

        private OperateResult<byte[]> ReadAddressData( McAddressData addressData )
        {
            byte[] coreResult = MelsecHelper.BuildReadMcCoreCommand( addressData, false );

            // 核心交互
            var read = ReadFromCoreServer( PackMcCommand( coreResult, this.NetworkNumber, this.NetworkStationNumber ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 错误代码验证
            ushort errorCode = BitConverter.ToUInt16( read.Content, 9 );
            if (errorCode != 0) return new OperateResult<byte[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 数据解析，需要传入是否使用位的参数
            return ExtractActualData( SoftBasic.BytesArrayRemoveBegin( read.Content, 11 ), false );
        }

        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="address">初始地址</param>
        /// <param name="value">原始的字节数据</param>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，写入如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="WriteExample2" title="Write示例" />
        /// 以下是写入不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        /// <returns>结果</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAnalysisAddress( address, 0 );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            return WriteAddressData( addressResult.Content, value );
        }

        private OperateResult WriteAddressData( McAddressData addressData, byte[] value )
        {
            // 创建核心报文
            byte[] coreResult = MelsecHelper.BuildWriteWordCoreCommand( addressData, value );

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return read;

            // 错误码校验
            ushort ErrorCode = BitConverter.ToUInt16( read.Content, 9 );
            if (ErrorCode != 0) return new OperateResult<byte[]>( ErrorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 成功
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Bool Operate Support

        /// <summary>
        /// 从三菱PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <remarks>
        /// 地址支持的列表参考 <seealso cref="MelsecMcNet"/> 的备注说明
        /// </remarks>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadBool" title="Bool类型示例" />
        /// </example>
        public override OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAnalysisAddress( address, length );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( addressResult );

            // 获取指令
            byte[] coreResult = MelsecHelper.BuildReadMcCoreCommand( addressResult.Content, true );

            // 核心交互
            var read = ReadFromCoreServer( PackMcCommand( coreResult, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 错误代码验证
            ushort errorCode = BitConverter.ToUInt16( read.Content, 9 );
            if (errorCode != 0) return new OperateResult<bool[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 数据解析，需要传入是否使用位的参数
            var extract = ExtractActualData( SoftBasic.BytesArrayRemoveBegin( read.Content, 11 ), true );
            if(!extract.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( extract );

            // 转化bool数组
            return OperateResult.CreateSuccessResult( extract.Content.Select( m => m == 0x01 ).Take( length ).ToArray( ) );
        }

        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="WriteBool" title="Write示例" />
        /// </example>
        /// <returns>返回写入结果</returns>
        public override OperateResult Write( string address, bool[] values )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAnalysisAddress( address, 0 );
            if (!addressResult.IsSuccess) return addressResult;

            byte[] coreResult = MelsecHelper.BuildWriteBitCoreCommand( addressResult.Content, values );

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return read;

            // 错误码校验
            ushort ErrorCode = BitConverter.ToUInt16( read.Content, 9 );
            if (ErrorCode != 0) return new OperateResult<byte[]>( ErrorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 成功
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Remote Operate

        /// <summary>
        /// 远程Run操作
        /// </summary>
        /// <returns>是否成功</returns>
        public OperateResult RemoteRun( )
        {
            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( new byte[] { 0x01, 0x10, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return read;

            // 错误码校验
            ushort ErrorCode = BitConverter.ToUInt16( read.Content, 9 );
            if (ErrorCode != 0) return new OperateResult( ErrorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 成功
            return OperateResult.CreateSuccessResult( );
        }
        
        /// <summary>
        /// 远程Stop操作
        /// </summary>
        /// <returns>是否成功</returns>
        public OperateResult RemoteStop( )
        {
            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( new byte[] { 0x02, 0x10, 0x00, 0x00, 0x01, 0x00 }, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return read;

            // 错误码校验
            ushort ErrorCode = BitConverter.ToUInt16( read.Content, 9 );
            if (ErrorCode != 0) return new OperateResult( ErrorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 成功
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 远程Reset操作
        /// </summary>
        /// <returns>是否成功</returns>
        public OperateResult RemoteReset()
        {
            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( new byte[] { 0x06, 0x10, 0x00, 0x00, 0x01, 0x00 }, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return read;

            // 错误码校验
            ushort ErrorCode = BitConverter.ToUInt16( read.Content, 9 );
            if (ErrorCode != 0) return new OperateResult( ErrorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 成功
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 读取PLC的型号信息
        /// </summary>
        /// <returns>返回型号的结果对象</returns>
        public OperateResult<string> ReadPlcType( )
        {
            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( new byte[] { 0x01, 0x01, 0x00, 0x00 }, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            // 错误码校验
            ushort ErrorCode = BitConverter.ToUInt16( read.Content, 9 );
            if (ErrorCode != 0) return new OperateResult<string>( ErrorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 成功
            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetString( read.Content, 11, 16 ).TrimEnd( ) );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return $"MelsecMcNet[{IpAddress}:{Port}]";
        }

        #endregion

        #region Static Method Helper

        /// <summary>
        /// 将MC协议的核心报文打包成一个可以直接对PLC进行发送的原始报文
        /// </summary>
        /// <param name="mcCore">MC协议的核心报文</param>
        /// <param name="networkNumber">网络号</param>
        /// <param name="networkStationNumber">网络站号</param>
        /// <returns>原始报文信息</returns>
        public static byte[] PackMcCommand(byte[] mcCore, byte networkNumber = 0, byte networkStationNumber = 0)
        {
            byte[] _PLCCommand = new byte[11 + mcCore.Length];
            _PLCCommand[0] = 0x50;                                               // 副标题
            _PLCCommand[1] = 0x00;
            _PLCCommand[2] = networkNumber;                                      // 网络号
            _PLCCommand[3] = 0xFF;                                               // PLC编号
            _PLCCommand[4] = 0xFF;                                               // 目标模块IO编号
            _PLCCommand[5] = 0x03;
            _PLCCommand[6] = networkStationNumber;                               // 目标模块站号
            _PLCCommand[7] = (byte)((_PLCCommand.Length - 9) % 256);             // 请求数据长度
            _PLCCommand[8] = (byte)((_PLCCommand.Length - 9) / 256);
            _PLCCommand[9] = 0x0A;                                               // CPU监视定时器
            _PLCCommand[10] = 0x00;
            mcCore.CopyTo( _PLCCommand, 11 );

            return _PLCCommand;
        }

        /// <summary>
        /// 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
        /// </summary>
        /// <param name="response">反馈的数据内容</param>
        /// <param name="isBit">是否位读取</param>
        /// <returns>解析后的结果对象</returns>
        public static OperateResult<byte[]> ExtractActualData( byte[] response, bool isBit )
        {
            if (isBit)
            {
                // 位读取
                byte[] Content = new byte[response.Length * 2];
                for (int i = 0; i < response.Length; i++)
                {
                    if ((response[i] & 0x10) == 0x10)
                    {
                        Content[i * 2 + 0] = 0x01;
                    }

                    if ((response[i] & 0x01) == 0x01)
                    {
                        Content[i * 2 + 1] = 0x01;
                    }
                }

                return OperateResult.CreateSuccessResult( Content );
            }
            else
            {
                // 字读取
                return OperateResult.CreateSuccessResult( response );
            }
        }
        
        #endregion
    }
}
