using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Robot.KUKA
{
    /// <summary>
    /// Kuka机器人的数据交互对象，通讯支持的条件为KUKA 的 KRC4 控制器中运行KUKAVARPROXY 这个第三方软件，端口通常为7000
    /// </summary>
    /// <remarks>
    /// 非常感谢 昆山-LT 网友的测试和意见反馈。
    /// </remarks>
    public class KukaAvarProxyNet : NetworkDoubleBase<KukaVarProxyMessage, ReverseWordTransform>, IRobotNet
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的对象
        /// </summary>
        public KukaAvarProxyNet( )
        {
            softIncrementCount = new SoftIncrementCount( ushort.MaxValue );
        }

        /// <summary>
        /// 实例化一个默认的Kuka机器人对象，并指定IP地址和端口号，端口号通常为7000
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public KukaAvarProxyNet( string ipAddress, int port )
        {
            IpAddress = ipAddress;
            Port = port;

            softIncrementCount = new SoftIncrementCount( ushort.MaxValue );
        }

        #endregion
        
        #region Read Write Support


        /// <summary>
        /// 读取埃夫特机器人的原始的字节数据信息，该地址参数是没有任何作用的，随便填什么
        /// </summary>
        /// <param name="address">无效参数</param>
        /// <returns>带有成功标识的byte[]数组</returns>
        public OperateResult<byte[]> Read( string address )
        {
            OperateResult<byte[]> read = ReadFromCoreServer( PackCommand( BuildReadValueCommand( address ) ) );
            if (!read.IsSuccess) return read;

            return ExtractActualData( read.Content );
        }

        /// <summary>
        /// 读取机器人的所有的数据信息，返回JSON格式的数据对象，地址参数无效
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>带有成功标识的字符串数据</returns>
        public OperateResult<string> ReadString( string address )
        {
            OperateResult<byte[]> read = Read( address );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return OperateResult.CreateSuccessResult( Encoding.Default.GetString( read.Content ) );
        }

        /// <summary>
        /// 本机器人不支持该方法操作，将永远返回失败，无效的操作
        /// </summary>
        /// <param name="address">指定的地址信息，有些机器人可能不支持</param>
        /// <param name="value">原始的字节数据信息</param>
        /// <returns>是否成功的写入</returns>
        public OperateResult Write( string address, byte[] value )
        {
            return Write( address, Encoding.Default.GetString( value ) );
        }

        /// <summary>
        /// 本机器人支持该方法操作，根据实际的值记性返回
        /// </summary>
        /// <param name="address">指定的地址信息，有些机器人可能不支持</param>
        /// <param name="value">字符串的数据信息</param>
        /// <returns>是否成功的写入</returns>
        public OperateResult Write( string address, string value )
        {
            OperateResult<byte[]> read = ReadFromCoreServer( PackCommand( BuildWriteValueCommand( address, value ) ) );
            if (!read.IsSuccess) return read;

            return ExtractActualData( read.Content );
        }

        #endregion

        #region Command Build

        /// <summary>
        /// 将核心的指令打包成一个可用于发送的消息对象
        /// </summary>
        /// <param name="commandCore">核心命令</param>
        /// <returns>最终实现的可以发送的机器人的字节数据</returns>
        private byte[] PackCommand( byte[] commandCore )
        {
            byte[] buffer = new byte[commandCore.Length + 4];
            ByteTransform.TransByte( (ushort)softIncrementCount.GetCurrentValue( ) ).CopyTo( buffer, 0 );
            ByteTransform.TransByte( (ushort)commandCore.Length ).CopyTo( buffer, 2 );
            commandCore.CopyTo( buffer, 4 );

            return buffer;
        }

        private OperateResult<byte[]> ExtractActualData(byte[] response )
        {
            try
            {
                if(response[response.Length - 1] != 0x01) return new OperateResult<byte[]>( response[response.Length - 1], "Wrong: " + SoftBasic.ByteToHexString( response, ' ' ) );

                int length = response[5] * 256 + response[6];
                byte[] buffer = new byte[length];
                Array.Copy( response, 7, buffer, 0, length );
                return OperateResult.CreateSuccessResult( buffer );
            }
            catch(Exception ex)
            {
                return new OperateResult<byte[]>( "Wrong:" + ex.Message + " Code:" + SoftBasic.ByteToHexString( response, ' ' ) );
            }
        }

        private byte[] BuildCommands(byte function, string[] commands )
        {
            List<byte> buffer = new List<byte>( );
            buffer.Add( function );
            for (int i = 0; i < commands.Length; i++)
            {
                byte[] buffer_command = Encoding.Default.GetBytes( commands[i] );
                buffer.AddRange( ByteTransform.TransByte( (ushort)buffer_command.Length ) );
                buffer.AddRange( buffer_command );
            }
            return buffer.ToArray( );
        }

        private byte[] BuildReadValueCommand( string address )
        {
            return BuildCommands( 0x00, new string[] { address } );
        }

        private byte[] BuildWriteValueCommand( string address, string value )
        {
            return BuildCommands( 0x01, new string[] { address, value } );
        }

        #endregion

        #region Private Member

        private SoftIncrementCount softIncrementCount;              // 自增消息的对象

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return $"KukaAvarProxyNet Robot[{IpAddress}:{Port}]";
        }

        #endregion
    }
}
