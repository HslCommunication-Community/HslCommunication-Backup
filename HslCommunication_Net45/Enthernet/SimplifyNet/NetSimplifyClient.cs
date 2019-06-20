using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core.Net;
using HslCommunication.Core.IMessage;
using HslCommunication.Core;
using System.Net.Sockets;
#if !NET35
using System.Threading.Tasks;
#endif

namespace HslCommunication.Enthernet
{
    /// <summary>
    /// 同步访问数据的客户端类，用于向服务器请求一些确定的数据信息
    /// </summary>
    /// <remarks>
    /// 详细的使用说明，请参照博客<a href="http://www.cnblogs.com/dathlin/p/7697782.html">http://www.cnblogs.com/dathlin/p/7697782.html</a>
    /// </remarks>
    /// <example>
    /// 此处贴上了Demo项目的服务器配置的示例代码
    /// <code lang="cs" source="TestProject\HslCommunicationDemo\FormSimplifyNet.cs" region="FormSimplifyNet" title="FormSimplifyNet示例" />
    /// </example>
    public class NetSimplifyClient : NetworkDoubleBase<HslMessage, RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个客户端的对象，用于和服务器通信
        /// </summary>
        /// <param name="ipAddress">服务器的ip地址</param>
        /// <param name="port">服务器的端口号</param>
        public NetSimplifyClient( string ipAddress, int port )
        {
            IpAddress = ipAddress;
            Port = port;
        }

        /// <summary>
        /// 实例化一个客户端对象，需要手动指定Ip地址和端口
        /// </summary>
        public NetSimplifyClient( )
        {

        }

        #endregion

        #region Override NetworkDoubleBase

        /// <summary>
        /// 连接上服务器后需要进行的初始化操作，无论是否允许操作都要进行验证
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>是否初始化成功，依据具体的协议进行重写</returns>
        protected override OperateResult InitializationOnConnect( Socket socket )
        {
            if (isUseAccountCertificate)
            {
                return AccountCertificate( socket );
            }

            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据，忽略了自定义消息反馈
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<string> ReadFromServer( NetHandle customer, string send )
        {
            var read = ReadFromServerBase( HslProtocol.CommandBytes( customer, Token, send ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return OperateResult.CreateSuccessResult( Encoding.Unicode.GetString( read.Content ) );
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数组，忽略了自定义消息反馈
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<string[]> ReadFromServer( NetHandle customer, string[] send )
        {
            var read = ReadFromServerBase( HslProtocol.CommandBytes( customer, Token, send ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string[]>( read );

            return OperateResult.CreateSuccessResult( HslProtocol.UnPackStringArrayFromByte( read.Content ) );
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字节数据
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送的字节内容</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<byte[]> ReadFromServer( NetHandle customer, byte[] send )
        {
            return ReadFromServerBase( HslProtocol.CommandBytes( customer, Token, send ) );
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据，并返回状态信息
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<NetHandle, string> ReadCustomerFromServer( NetHandle customer, string send )
        {
            var read = ReadCustomerFromServerBase( HslProtocol.CommandBytes( customer, Token, send ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<NetHandle, string>( read );

            return OperateResult.CreateSuccessResult( read.Content1, Encoding.Unicode.GetString( read.Content2 ) );
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据，并返回状态信息
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<NetHandle, string[]> ReadCustomerFromServer( NetHandle customer, string[] send )
        {
            var read = ReadCustomerFromServerBase( HslProtocol.CommandBytes( customer, Token, send ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<NetHandle, string[]>( read );

            return OperateResult.CreateSuccessResult( read.Content1, HslProtocol.UnPackStringArrayFromByte( read.Content2 ) );
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据，并返回状态信息
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<NetHandle, byte[]> ReadCustomerFromServer( NetHandle customer, byte[] send )
        {
            return ReadCustomerFromServerBase( HslProtocol.CommandBytes( customer, Token, send ) );
        }

        /// <summary>
        /// 需要发送的底层数据
        /// </summary>
        /// <param name="send">需要发送的底层数据</param>
        /// <returns>带返回消息的结果对象</returns>
        private OperateResult<byte[]> ReadFromServerBase( byte[] send )
        {
            var read = ReadCustomerFromServerBase( send );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            return OperateResult.CreateSuccessResult( read.Content2 );
        }

        /// <summary>
        /// 需要发送的底层数据
        /// </summary>
        /// <param name="send">需要发送的底层数据</param>
        /// <returns>带返回消息的结果对象</returns>
        private OperateResult<NetHandle, byte[]> ReadCustomerFromServerBase( byte[] send )
        {
            // 核心数据交互
            var read = ReadFromCoreServer( send );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<NetHandle, byte[]>( read );

            // 提炼数据信息
            byte[] headBytes = new byte[HslProtocol.HeadByteLength];
            byte[] contentBytes = new byte[read.Content.Length - HslProtocol.HeadByteLength];

            Array.Copy( read.Content, 0, headBytes, 0, HslProtocol.HeadByteLength );
            if (contentBytes.Length > 0) Array.Copy( read.Content, HslProtocol.HeadByteLength, contentBytes, 0, read.Content.Length - HslProtocol.HeadByteLength );

            if(BitConverter.ToInt32(headBytes, 0) == HslProtocol.ProtocolErrorMsg)
            {
                return new OperateResult<NetHandle, byte[]>( Encoding.ASCII.GetString( contentBytes ) );
            }

            int customer = BitConverter.ToInt32( headBytes, 4 );
            contentBytes = HslProtocol.CommandAnalysis( headBytes, contentBytes );
            return OperateResult.CreateSuccessResult( (NetHandle)customer, contentBytes );
        }

#if !NET35

        /// <summary>
        /// 客户端向服务器进行异步请求，请求字符串数据
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        public Task<OperateResult<string>> ReadFromServerAsync( NetHandle customer, string send )
        {
            return Task.Run( ( ) => ReadFromServer( customer, send ) );
        }

        /// <summary>
        /// 客户端向服务器进行异步请求，请求字节数据
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送的字节内容</param>
        /// <returns>带返回消息的结果对象</returns>
        public Task<OperateResult<byte[]>> ReadFromServerAsync( NetHandle customer, byte[] send )
        {
            return Task.Run( ( ) => ReadFromServer( customer, send ) );
        }

        /// <summary>
        /// 客户端向服务器进行异步请求，请求字符串数据
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="sends">发送数据</param>
        public Task<OperateResult<string[]>> ReadFromServerAsync( NetHandle customer, string[] sends )
        {
            return Task.Run( ( ) => ReadFromServer( customer, sends ) );
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据，并返回状态信息
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public Task<OperateResult<NetHandle, string>> ReadCustomerFromServerAsync( NetHandle customer, string send )
        {
            return Task.Run( ( ) => ReadCustomerFromServer( customer, send ) );
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据，并返回状态信息
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public Task<OperateResult<NetHandle, string[]>> ReadCustomerFromServerAsync( NetHandle customer, string[] send )
        {
            return Task.Run( ( ) => ReadCustomerFromServer( customer, send ) );
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据，并返回状态信息
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public Task<OperateResult<NetHandle, byte[]>> ReadCustomerFromServerAsync( NetHandle customer, byte[] send )
        {
            return Task.Run( ( ) => ReadCustomerFromServer( customer, send ) );
        }

#endif

        #region Object Override

        /// <summary>
        /// 获取本对象的字符串表示形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return $"NetSimplifyClient[{IpAddress}:{Port}]";
        }

        #endregion

    }

}
