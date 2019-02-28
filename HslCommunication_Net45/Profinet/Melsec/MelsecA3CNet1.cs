using HslCommunication.Core;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 基于Qna 兼容3C帧的格式一的通讯，具体的地址需要参照三菱的基本地址
    /// </summary>
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
            // 解析指令
            OperateResult<byte[]> command = MelsecHelper.BuildAsciiReadMcCoreCommand( address, length, false );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadBase( PackCommand( command.Content, this.station ) );
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
            // 解析指令
            OperateResult<byte[]> command = MelsecHelper.BuildAsciiWriteWordCoreCommand( address, value );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadBase( PackCommand( command.Content, this.station ) );
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
        public OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            // 解析指令
            OperateResult<byte[]> command = MelsecHelper.BuildAsciiReadMcCoreCommand( address, length, true );
            if (!command.IsSuccess) OperateResult.CreateFailedResult<bool[]>( command );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( PackCommand( command.Content, this.station ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 结果验证
            if (read.Content[0] != 0x02) return new OperateResult<bool[]>( read.Content[0], "Read Faild:" + Encoding.ASCII.GetString( read.Content, 1, read.Content.Length - 1 ) );

            // 提取结果
            byte[] buffer = new byte[length];
            Array.Copy( read.Content, 11, buffer, 0, length );
            return OperateResult.CreateSuccessResult( buffer.Select( m => m == 0x31 ).ToArray( ) );
        }

        /// <summary>
        /// 批量读取bool类型数据，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
        /// </summary>
        /// <param name="address">地址信息，比如X10,Y17，注意X，Y的地址是8进制的</param>
        /// <returns>读取结果信息</returns>
        public OperateResult<bool> ReadBool( string address )
        {
            OperateResult<bool[]> read = ReadBool( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return OperateResult.CreateSuccessResult( read.Content[0] );
        }

        /// <summary>
        /// 批量写入bool类型的数值，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
        /// </summary>
        /// <param name="address">PLC的地址信息</param>
        /// <param name="value">数据信息</param>
        /// <returns>是否写入成功</returns>
        public OperateResult Write( string address, bool value )
        {
            return Write( address, new bool[] { value } );
        }

        /// <summary>
        /// 批量写入bool类型的数组，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型
        /// </summary>
        /// <param name="address">PLC的地址信息</param>
        /// <param name="value">数据信息</param>
        /// <returns>是否写入成功</returns>
        public OperateResult Write( string address, bool[] value )
        {
            // 解析指令
            OperateResult<byte[]> command = MelsecHelper.BuildAsciiWriteBitCoreCommand( address, value );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadBase( PackCommand( command.Content, this.station ) );
            if (!read.IsSuccess) return read;

            // 结果验证
            if (read.Content[0] != 0x06) return new OperateResult( read.Content[0], "Write Faild:" + Encoding.ASCII.GetString( read.Content, 1, read.Content.Length - 1 ) );

            // 提取结果
            return OperateResult.CreateSuccessResult( );
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
            command[3] = MelsecHelper.BuildBytesFromData( station )[0];
            command[4] = MelsecHelper.BuildBytesFromData( station )[1];
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
            command[command.Length - 2] = MelsecHelper.BuildBytesFromData( (byte)sum )[0];
            command[command.Length - 1] = MelsecHelper.BuildBytesFromData( (byte)sum )[1];

            return command;
        }

        #endregion
    }
}
