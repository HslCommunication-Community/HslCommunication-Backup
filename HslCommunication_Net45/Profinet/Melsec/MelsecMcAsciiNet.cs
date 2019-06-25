using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式
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
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="Usage" title="简单的短连接使用" />
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="Usage2" title="简单的长连接使用" />
    /// </example>
    public class MelsecMcAsciiNet : NetworkDeviceBase<MelsecQnA3EAsciiMessage, RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        public MelsecMcAsciiNet( )
        {
            WordLength = 1;
        }

        /// <summary>
        /// 实例化一个三菱的Qna兼容3E帧协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public MelsecMcAsciiNet( string ipAddress, int port )
        {
            WordLength = 1;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Public Member

        /// <summary>
        /// 网络号
        /// </summary>
        public byte NetworkNumber { get; set; } = 0x00;

        /// <summary>
        /// 网络站号
        /// </summary>
        public byte NetworkStationNumber { get; set; } = 0x00;


        #endregion

        #region Address Analysis

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

        #region Read Write Override

        /// <summary>
        /// 从三菱PLC中读取想要的数据，返回读取结果，读取的单位为字
        /// </summary>
        /// <param name="address">读取地址，格式为"M100","D100","W1A0"</param>
        /// <param name="length">读取的数据长度，字最大值960，位最大值7168</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <remarks>
        /// 地址支持的列表参考 <seealso cref="MelsecMcAsciiNet"/> 的备注说明
        /// </remarks>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，读取如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadExample1" title="Read示例" />
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
                ushort readLength = (ushort)Math.Min( length - alreadyFinished, 450 );
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
            // 地址分析
            byte[] coreResult = MelsecHelper.BuildAsciiReadMcCoreCommand( addressData, false );

            // 核心交互
            var read = ReadFromCoreServer( PackMcCommand( coreResult, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 错误代码验证
            ushort errorCode = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, 18, 4 ), 16 );
            if (errorCode != 0) return new OperateResult<byte[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 数据解析，需要传入是否使用位的参数
            return ExtractActualData( read.Content, false );
        }

        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="address">初始地址</param>
        /// <param name="value">原始的字节数据</param>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，写入如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="WriteExample2" title="Write示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        /// <returns>结果</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAnalysisAddress( address, 0 );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            // 地址分析
            byte[] coreResult = MelsecHelper.BuildAsciiWriteWordCoreCommand( addressResult.Content, value );
            
            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return read;

            // 错误码验证
            ushort errorCode = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, 18, 4 ), 16 );
            if (errorCode != 0) return new OperateResult<byte[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 写入成功
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
        /// 地址支持的列表参考 <seealso cref="MelsecMcAsciiNet"/> 的备注说明
        /// </remarks>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="ReadBool" title="Bool类型示例" />
        /// </example>
        public override OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAnalysisAddress( address, length );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( addressResult );

            // 地址分析
            byte[] coreResult = MelsecHelper.BuildAsciiReadMcCoreCommand( addressResult.Content, true );
            
            // 核心交互
            var read = ReadFromCoreServer( PackMcCommand( coreResult, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 错误代码验证
            ushort errorCode = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, 18, 4 ), 16 );
            if (errorCode != 0) return new OperateResult<bool[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 数据解析，需要传入是否使用位的参数
            var extract = ExtractActualData( read.Content, true );
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
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="WriteBool" title="Write示例" />
        /// </example>
        /// <returns>返回写入结果</returns>
        public override OperateResult Write( string address, bool[] values )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAnalysisAddress( address, 0 );
            if (!addressResult.IsSuccess) return addressResult;

            // 解析指令
            byte[] coreResult = MelsecHelper.BuildAsciiWriteBitCoreCommand( addressResult.Content, values );

            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult, NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return read;

            // 错误码验证
            ushort errorCode = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, 18, 4 ), 16 );
            if (errorCode != 0) return new OperateResult<byte[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 写入成功
            return OperateResult.CreateSuccessResult( );
        }

        #endregion
        
        #region Remote Operate

        /// <summary>
        /// 远程Run操作
        /// </summary>
        /// <returns>是否成功</returns>
        public OperateResult RemoteRun()
        {
            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( Encoding.ASCII.GetBytes("1001000000010000"), NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return read;

            // 错误码校验
            ushort errorCode = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, 18, 4 ), 16 );
            if (errorCode != 0) return new OperateResult( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 成功
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 远程Stop操作
        /// </summary>
        /// <returns>是否成功</returns>
        public OperateResult RemoteStop()
        {
            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( Encoding.ASCII.GetBytes( "100200000001" ), NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return read;

            // 错误码校验
            ushort errorCode = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, 18, 4 ), 16 );
            if (errorCode != 0) return new OperateResult( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 成功
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 读取PLC的型号信息
        /// </summary>
        /// <returns>返回型号的结果对象</returns>
        public OperateResult<string> ReadPlcType()
        {
            // 核心交互
            OperateResult<byte[]> read = ReadFromCoreServer( PackMcCommand( Encoding.ASCII.GetBytes( "01010000" ), NetworkNumber, NetworkStationNumber ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            // 错误码校验
            ushort errorCode = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, 18, 4 ), 16 );
            if (errorCode != 0) return new OperateResult<string>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument );

            // 成功
            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetString( read.Content, 22, 16 ).TrimEnd( ) );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return $"MelsecMcAsciiNet[{IpAddress}:{Port}]";
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
        public static byte[] PackMcCommand( byte[] mcCore, byte networkNumber = 0, byte networkStationNumber = 0 )
        {
            byte[] plcCommand = new byte[22 + mcCore.Length];
            plcCommand[ 0] = 0x35;                                                                        // 副标题
            plcCommand[ 1] = 0x30;
            plcCommand[ 2] = 0x30;
            plcCommand[ 3] = 0x30;
            plcCommand[ 4] = SoftBasic.BuildAsciiBytesFrom( networkNumber )[0];                         // 网络号
            plcCommand[ 5] = SoftBasic.BuildAsciiBytesFrom( networkNumber )[1];
            plcCommand[ 6] = 0x46;                                                                        // PLC编号
            plcCommand[ 7] = 0x46;
            plcCommand[ 8] = 0x30;                                                                        // 目标模块IO编号
            plcCommand[ 9] = 0x33;
            plcCommand[10] = 0x46;
            plcCommand[11] = 0x46;
            plcCommand[12] = SoftBasic.BuildAsciiBytesFrom( networkStationNumber )[0];                  // 目标模块站号
            plcCommand[13] = SoftBasic.BuildAsciiBytesFrom( networkStationNumber )[1];
            plcCommand[14] = SoftBasic.BuildAsciiBytesFrom( (ushort)(plcCommand.Length - 18) )[0];     // 请求数据长度
            plcCommand[15] = SoftBasic.BuildAsciiBytesFrom( (ushort)(plcCommand.Length - 18) )[1];
            plcCommand[16] = SoftBasic.BuildAsciiBytesFrom( (ushort)(plcCommand.Length - 18) )[2];
            plcCommand[17] = SoftBasic.BuildAsciiBytesFrom( (ushort)(plcCommand.Length - 18) )[3];
            plcCommand[18] = 0x30;                                                                        // CPU监视定时器
            plcCommand[19] = 0x30;
            plcCommand[20] = 0x31;
            plcCommand[21] = 0x30;
            mcCore.CopyTo( plcCommand, 22 );

            return plcCommand;
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
                byte[] Content = new byte[response.Length - 22];
                for (int i = 22; i < response.Length; i++)
                {
                    Content[i - 22] = response[i] == 0x30 ? (byte)0x00 : (byte)0x01;
                }

                return OperateResult.CreateSuccessResult( Content );
            }
            else
            {
                // 字读取
                byte[] Content = new byte[(response.Length - 22) / 2];
                for (int i = 0; i < Content.Length / 2; i++)
                {
                    ushort tmp = Convert.ToUInt16( Encoding.ASCII.GetString( response, i * 4 + 22, 4 ), 16 );
                    BitConverter.GetBytes( tmp ).CopyTo( Content, i * 2 );
                }

                return OperateResult.CreateSuccessResult( Content );
            }
        }

        #endregion
    }
}
