using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.Robot.YASKAWA
{
    /// <summary>
    /// 安川机器人的Ethernet 服务器功能的通讯类
    /// </summary>
    public class YRC1000TcpNet : NetworkDoubleBase<HslMessage, ReverseWordTransform>, IRobotNet
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的对象
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public YRC1000TcpNet( string ipAddress, int port )
        {

        }

        #endregion

        #region IRobot Interface

        /// <summary>
        /// 根据地址读取机器人的原始的字节数据信息
        /// </summary>
        /// <param name="address">指定的地址信息，对于某些机器人无效</param>
        /// <returns>带有成功标识的byte[]数组</returns>
        public OperateResult<byte[]> Read( string address )
        {
            OperateResult<string> read = ReadString( address );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetBytes( read.Content ) );
        }

        /// <summary>
        /// 根据地址读取机器人的字符串的数据信息
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>带有成功标识的字符串数据</returns>
        public OperateResult<string> ReadString( string address )
        {
            if (address.IndexOf( '.' ) >= 0 || address.IndexOf( ':' ) >= 0 || address.IndexOf( ';' ) >= 0)
            {
                string[] commands = address.Split( new char[] { '.', ':', ';' } );
                return ReadByCommand( commands[0], commands[1] );
            }
            else
            {
                return ReadByCommand( address, null );
            }
        }

        /// <summary>
        /// 根据地址，来写入设备的相关的数据
        /// </summary>
        /// <param name="address">指定的地址信息，有些机器人可能不支持</param>
        /// <param name="value">原始的字节数据信息</param>
        /// <returns>是否成功的写入</returns>
        public OperateResult Write( string address, byte[] value )
        {
            return Write( address, Encoding.ASCII.GetString( value ) );
        }

        /// <summary>
        /// 根据地址，来写入设备相关的数据
        /// </summary>
        /// <param name="address">指定的地址信息，有些机器人可能不支持</param>
        /// <param name="value">字符串的数据信息</param>
        /// <returns>是否成功的写入</returns>
        public OperateResult Write( string address, string value )
        {
            return ReadByCommand( address, value );
        }

        #endregion

        #region Initialization Override

        /// <summary>
        /// before read data , the connection should be Initialized
        /// </summary>
        /// <param name="socket">connected socket</param>
        /// <returns>whether is the Initialization is success.</returns>
        protected override OperateResult InitializationOnConnect( Socket socket )
        {
            var read = ReadFromCoreServer( socket, "CONNECT Robot_access KeepAlive:-1\r\n" );
            if (!read.IsSuccess) return read;

            if (read.Content != "OK:YR Information Server(Ver) Keep-Alive:-1.\r\n")
                return new OperateResult( read.Content );

            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Override Read

        /// <summary>
        /// 重写父类的数据交互方法，接收的时候采用标识符来接收
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="send">发送的数据</param>
        /// <returns>发送结果对象</returns>
        public override OperateResult<byte[]> ReadFromCoreServer( Socket socket, byte[] send )
        {
            LogNet?.WriteDebug( ToString( ), StringResources.Language.Send + " : " + BasicFramework.SoftBasic.ByteToHexString( send, ' ' ) );

            // send
            OperateResult sendResult = Send( socket, send );
            if (!sendResult.IsSuccess)
            {
                socket?.Close( );
                return OperateResult.CreateFailedResult<byte[]>( sendResult );
            }

            if (ReceiveTimeOut < 0) return OperateResult.CreateSuccessResult( new byte[0] );

            // receive msg
            OperateResult<byte[]> resultReceive = NetSupport.ReceiveCommandLineFromSocket( socket, (byte)'\r', (byte)'\n' );
            if (!resultReceive.IsSuccess) return new OperateResult<byte[]>( StringResources.Language.ReceiveDataTimeout + ReceiveTimeOut );

            LogNet?.WriteDebug( ToString( ), StringResources.Language.Receive + " : " + BasicFramework.SoftBasic.ByteToHexString( resultReceive.Content, ' ' ) );

            // Success
            return OperateResult.CreateSuccessResult( resultReceive.Content );
        }

        /// <summary>
        /// Read string value from socket
        /// </summary>
        /// <param name="socket">connected socket</param>
        /// <param name="send">string value</param>
        /// <returns>received string value with is successfully</returns>
        protected OperateResult<string> ReadFromCoreServer( Socket socket, string send )
        {
            var read = ReadFromCoreServer( socket, Encoding.Default.GetBytes( send ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return OperateResult.CreateSuccessResult( Encoding.Default.GetString( read.Content ) );
        }

        /// <summary>
        /// 根据指令来读取设备的信息，如果命令数据为空，则传入null即可，注意，所有的命令不带换行符
        /// </summary>
        /// <param name="command">命令的内容</param>
        /// <param name="commandData">命令数据内容</param>
        /// <returns>最终的结果内容，需要对IsSuccess进行验证</returns>
        public OperateResult<string> ReadByCommand( string command, string commandData )
        {
            InteractiveLock.Enter( );

            // 获取有用的网络通道，如果没有，就建立新的连接
            OperateResult<Socket> resultSocket = GetAvailableSocket( );
            if (!resultSocket.IsSuccess)
            {
                IsSocketError = true;
                if (AlienSession != null) AlienSession.IsStatusOk = false;
                InteractiveLock.Leave( );
                return OperateResult.CreateFailedResult<string>( resultSocket );
            }

            // 先发送命令
            string sendCommand = string.IsNullOrEmpty( commandData ) ? $"HOSTCTRL_REQUEST {command} 0\r\n" : $"HOSTCTRL_REQUEST {command} {commandData.Length}\r\n";
            OperateResult<string> readCommand = ReadFromCoreServer( resultSocket.Content, sendCommand );
            if (!readCommand.IsSuccess)
            {
                IsSocketError = true;
                if (AlienSession != null) AlienSession.IsStatusOk = false;
                InteractiveLock.Leave( );
                return OperateResult.CreateFailedResult<string>( readCommand );
            }

            // 检查命令是否返回成功的状态
            if (!readCommand.Content.StartsWith( "OK:" ))
            {
                if (!isPersistentConn) resultSocket.Content?.Close( );
                InteractiveLock.Leave( );
                return new OperateResult<string>( readCommand.Content.Remove( readCommand.Content.Length - 2 ) );
            }

            // 在必要的情况下发送命令数据
            if(!string.IsNullOrEmpty( commandData ))
            {
                byte[] send2 = Encoding.ASCII.GetBytes( $"{commandData}\r" );
                LogNet?.WriteDebug( ToString( ), StringResources.Language.Send + " : " + BasicFramework.SoftBasic.ByteToHexString( send2, ' ' ) );

                OperateResult sendResult2 = Send( resultSocket.Content, send2 );
                if (!sendResult2.IsSuccess)
                {
                    resultSocket.Content?.Close( );
                    IsSocketError = true;
                    if (AlienSession != null) AlienSession.IsStatusOk = false;
                    InteractiveLock.Leave( );
                    return OperateResult.CreateFailedResult<string>( sendResult2 );
                }
            }

            // 接收数据信息，先接收到\r为止，再根据实际情况决定是否接收\r
            OperateResult<byte[]> resultReceive2 = NetSupport.ReceiveCommandLineFromSocket( resultSocket.Content, (byte)'\r' );
            if (!resultReceive2.IsSuccess)
            {
                IsSocketError = true;
                if (AlienSession != null) AlienSession.IsStatusOk = false;
                InteractiveLock.Leave( );
                return OperateResult.CreateFailedResult<string>( resultReceive2 );
            }

            string commandDataReturn = Encoding.ASCII.GetString( resultReceive2.Content );
            if (commandDataReturn.StartsWith( "ERROR:" ))
            {
                if (!isPersistentConn) resultSocket.Content?.Close( );
                InteractiveLock.Leave( );
                NetSupport.ReadBytesFromSocket( resultSocket.Content, 1 );

                return new OperateResult<string>( commandDataReturn );
            }
            else if (commandDataReturn.StartsWith( "0000\r" ))
            {
                if (!isPersistentConn) resultSocket.Content?.Close( );
                NetSupport.ReadBytesFromSocket( resultSocket.Content, 1 );

                InteractiveLock.Leave( );
                return OperateResult.CreateSuccessResult( "0000" );
            }
            else
            {
                if (!isPersistentConn) resultSocket.Content?.Close( );

                InteractiveLock.Leave( );
                return OperateResult.CreateSuccessResult( commandDataReturn.Remove( commandDataReturn.Length - 1 ) );
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 读取机器人的报警信息
        /// </summary>
        /// <returns>原始的报警信息</returns>
        public OperateResult<string> ReadRALARM( )
        {
            return ReadByCommand( "RALARM", null );
        }

        /// <summary>
        /// 读取机器人的坐标数据信息
        /// </summary>
        /// <returns>原始的报警信息</returns>
        public OperateResult<string> ReadRPOSJ( )
        {
            return ReadByCommand( "RPOSJ", null );
        }


        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return $"YRC1000TcpNet Robot[{IpAddress}:{Port}]";
        }

        #endregion

        #region Static Member


        #endregion
    }
}
