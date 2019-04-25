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
    [Obsolete("还没有完成")]
    public class YRC1000TcpNet : NetworkDoubleBase<KukaVarProxyMessage, ReverseWordTransform>, IRobotNet
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的对象
        /// </summary>
        public YRC1000TcpNet( )
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
            throw new NotImplementedException( );
        }

        /// <summary>
        /// 根据地址读取机器人的字符串的数据信息
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>带有成功标识的字符串数据</returns>
        public OperateResult<string> ReadString( string address )
        {
            throw new NotImplementedException( );
        }

        /// <summary>
        /// 根据地址，来写入设备的相关的数据
        /// </summary>
        /// <param name="address">指定的地址信息，有些机器人可能不支持</param>
        /// <param name="value">原始的字节数据信息</param>
        /// <returns>是否成功的写入</returns>
        public OperateResult Write( string address, byte[] value )
        {
            throw new NotImplementedException( );
        }

        /// <summary>
        /// 根据地址，来写入设备相关的数据
        /// </summary>
        /// <param name="address">指定的地址信息，有些机器人可能不支持</param>
        /// <param name="value">字符串的数据信息</param>
        /// <returns>是否成功的写入</returns>
        public OperateResult Write( string address, string value )
        {
            throw new NotImplementedException( );
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

        #endregion

        #region Private Member

        private string connectStr = "CONNECT Robot_access KeepAlive:-1\r\n";

        #endregion
    }
}
