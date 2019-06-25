using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 基于Qna 兼容3C帧的格式一的通讯，具体的地址需要参照三菱的基本地址
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
    public class MelsecA3CNet1 : SerialDeviceBase<RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化默认的构造方法
        /// </summary>
        public MelsecA3CNet1( )
        {
            WordLength = 1;
        }

        #endregion

        #region Public Member

        /// <summary>
        /// PLC的站号信息
        /// </summary>
        public byte Station { get => station; set => station = value; }

        #endregion

        #region Read Write Support

        /// <summary>
        /// 批量读取PLC的数据，以字为单位，支持读取X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>读取结果信息</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom( address, length );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            // 解析指令
            byte[] command = MelsecHelper.BuildAsciiReadMcCoreCommand( addressResult.Content, false );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( PackCommand( command, this.station ) );
            if (!read.IsSuccess) return read;

            // 结果验证
            if (read.Content[0] != 0x02) return new OperateResult<byte[]>( read.Content[0], "Read Faild:" + Encoding.ASCII.GetString( read.Content, 1, read.Content.Length - 1 ) );

            // 提取结果
            byte[] Content = new byte[length * 2];
            for (int i = 0; i < Content.Length / 2; i++)
            {
                ushort tmp = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, i * 4 + 11, 4 ), 16 );
                BitConverter.GetBytes( tmp ).CopyTo( Content, i * 2 );
            }
            return OperateResult.CreateSuccessResult( Content );
        }

        /// <summary>
        /// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom( address, 0 );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressResult );

            // 解析指令
            byte[] command = MelsecHelper.BuildAsciiWriteWordCoreCommand( addressResult.Content, value );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( PackCommand( command, this.station ) );
            if (!read.IsSuccess) return read;

            // 结果验证
            if (read.Content[0] != 0x06) return new OperateResult( read.Content[0], "Write Faild:" + Encoding.ASCII.GetString( read.Content, 1, read.Content.Length - 1 ) );

            // 提取结果
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Bool Read Write

        /// <summary>
        /// 批量读取bool类型数据，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
        /// </summary>
        /// <param name="address">地址信息，比如X10,Y17，注意X，Y的地址是8进制的</param>
        /// <param name="length">读取的长度</param>
        /// <returns>读取结果信息</returns>
        public override OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom( address, length );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( addressResult );

            // 解析指令
            byte[] command = MelsecHelper.BuildAsciiReadMcCoreCommand( addressResult.Content, true );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( PackCommand( command, this.station ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 结果验证
            if (read.Content[0] != 0x02) return new OperateResult<bool[]>( read.Content[0], "Read Faild:" + Encoding.ASCII.GetString( read.Content, 1, read.Content.Length - 1 ) );

            // 提取结果
            byte[] buffer = new byte[length];
            Array.Copy( read.Content, 11, buffer, 0, length );
            return OperateResult.CreateSuccessResult( buffer.Select( m => m == 0x31 ).ToArray( ) );
        }

        /// <summary>
        /// 批量写入bool类型的数组，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
        /// </summary>
        /// <param name="address">PLC的地址信息</param>
        /// <param name="value">数据信息</param>
        /// <returns>是否写入成功</returns>
        public override OperateResult Write( string address, bool[] value )
        {
            // 分析地址
            OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom( address, 0 );
            if (!addressResult.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( addressResult );

            // 解析指令
            byte[] command = MelsecHelper.BuildAsciiWriteBitCoreCommand( addressResult.Content, value );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( PackCommand( command, this.station ) );
            if (!read.IsSuccess) return read;

            // 结果验证
            if (read.Content[0] != 0x06) return new OperateResult( read.Content[0], "Write Faild:" + Encoding.ASCII.GetString( read.Content, 1, read.Content.Length - 1 ) );

            // 提取结果
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
            OperateResult<byte[]> read = ReadBase( PackCommand( Encoding.ASCII.GetBytes( "1001000000010000" ), this.station ) );
            if (!read.IsSuccess) return read;

            // 结果验证
            if (read.Content[0] != 0x06 && read.Content[0] != 0x02) return new OperateResult( read.Content[0], "Faild:" + Encoding.ASCII.GetString( read.Content, 1, read.Content.Length - 1 ) );

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
            OperateResult<byte[]> read = ReadBase( PackCommand( Encoding.ASCII.GetBytes( "100200000001" ), this.station ) );
            if (!read.IsSuccess) return read;

            // 结果验证
            if (read.Content[0] != 0x06 && read.Content[0] != 0x02) return new OperateResult( read.Content[0], "Faild:" + Encoding.ASCII.GetString( read.Content, 1, read.Content.Length - 1 ) );

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
            OperateResult<byte[]> read = ReadBase( PackCommand( Encoding.ASCII.GetBytes( "01010000" ), this.station ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            // 结果验证
            if (read.Content[0] != 0x06 && read.Content[0] != 0x02) return new OperateResult<string>( read.Content[0], "Faild:" + Encoding.ASCII.GetString( read.Content, 1, read.Content.Length - 1 ) );

            // 成功
            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetString( read.Content, 11, 16 ).TrimEnd( ) );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return $"MelsecA3CNet1";
        }

        #endregion

        #region Private Member

        private byte station = 0x00;                 // PLC的站号信息

        #endregion

        #region Static Helper

        /// <summary>
        /// 将命令进行打包传送
        /// </summary>
        /// <param name="mcCommand">mc协议的命令</param>
        /// <param name="station">PLC的站号</param>
        /// <returns>最终的原始报文信息</returns>
        public static byte[] PackCommand( byte[] mcCommand, byte station = 0)
        {
            byte[] command = new byte[13 + mcCommand.Length];
            command[0] = 0x05;
            command[1] = 0x46;
            command[2] = 0x39;
            command[3] = SoftBasic.BuildAsciiBytesFrom( station )[0];
            command[4] = SoftBasic.BuildAsciiBytesFrom( station )[1];
            command[5] = 0x30;
            command[6] = 0x30;
            command[7] = 0x46;
            command[8] = 0x46;
            command[9] = 0x30;
            command[10] = 0x30;
            mcCommand.CopyTo( command, 11 );

            // 计算和校验
            int sum = 0;
            for (int i = 1; i < command.Length - 3; i++)
            {
                sum += command[i];
            }
            command[command.Length - 2] = SoftBasic.BuildAsciiBytesFrom( (byte)sum )[0];
            command[command.Length - 1] = SoftBasic.BuildAsciiBytesFrom( (byte)sum )[1];

            return command;
        }

        #endregion
    }
}
