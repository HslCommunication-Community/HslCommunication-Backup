using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 支持长连接，短连接两个模式的通用客户端基类 ->
    /// Universal client base class that supports long connections and short connections to two modes
    /// </summary>
    /// <example>
    /// 无，请使用继承类实例化，然后进行数据交互，当前的类并没有具体的实现。
    /// </example>
    public class NetworkDoubleBase<TNetMessage, TTransform> : NetworkBase, IDisposable where TNetMessage : INetMessage, new() where TTransform : IByteTransform, new()
    {
        #region Constructor

        /// <summary>
        /// 默认的无参构造函数 -> Default no-parameter constructor
        /// </summary>
        public NetworkDoubleBase( )
        {
            ByteTransform = new TTransform( );                                           // 实例化变换类的对象
            InteractiveLock = new SimpleHybirdLock( );                                     // 实例化数据访问锁
            connectionId = BasicFramework.SoftBasic.GetUniqueStringByGuidAndRandom( );  // 设备的唯一的编号
        }

        #endregion

        #region Private Member

        private TTransform byteTransform;                // 数据变换的接口
        private string ipAddress = "127.0.0.1";          // 连接的IP地址
        private int port = 10000;                        // 端口号
        private int connectTimeOut = 10000;              // 连接超时时间设置
        private string connectionId = string.Empty;      // 当前连接
        private bool isUseSpecifiedSocket = false;       // 指示是否使用指定的网络套接字访问数据

        /// <summary>
        /// 接收数据的超时时间
        /// </summary>
        protected int receiveTimeOut = 10000;            // 数据接收的超时时间
        /// <summary>
        /// 是否是长连接的状态
        /// </summary>
        protected bool isPersistentConn = false;         // 是否处于长连接的状态
        /// <summary>
        /// 交互的混合锁
        /// </summary>
        protected SimpleHybirdLock InteractiveLock;      // 一次正常的交互的互斥锁
        /// <summary>
        /// 当前的socket是否发生了错误
        /// </summary>
        protected bool IsSocketError = false;            // 指示长连接的套接字是否处于错误的状态

        #endregion

        #region Public Member

        /// <summary>
        /// 当前客户端的数据变换机制，当你需要从字节数据转换类型数据的时候需要。->
        /// The current client's data transformation mechanism is required when you need to convert type data from byte data.
        /// </summary>
        /// <example>
        /// 主要是用来转换数据类型的，下面仅仅演示了2个方法，其他的类型转换，类似处理。
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ByteTransform" title="ByteTransform示例" />
        /// </example>
        public TTransform ByteTransform
        {
            get { return byteTransform; }
            set { byteTransform = value; }
        }

        /// <summary>
        /// 获取或设置连接的超时时间，单位是毫秒 -> Gets or sets the timeout for the connection, in milliseconds
        /// </summary>
        /// <example>
        /// 设置1秒的超时的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ConnectTimeOutExample" title="ConnectTimeOut示例" />
        /// </example>
        /// <remarks>
        /// 不适用于异形模式的连接。
        /// </remarks>
        public int ConnectTimeOut
        {
            get { return connectTimeOut; }
            set { if (value >= 0) connectTimeOut = value; }
        }

        /// <summary>
        /// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈 -> 
        /// Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback
        /// </summary>
        /// <example>
        /// 设置1秒的接收超时的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReceiveTimeOutExample" title="ReceiveTimeOut示例" />
        /// </example>
        /// <remarks>
        /// 超时的通常原因是服务器端没有配置好，导致访问失败，为了不卡死软件，所以有了这个超时的属性。
        /// </remarks>
        public int ReceiveTimeOut
        {
            get { return receiveTimeOut; }
            set { receiveTimeOut = value; }
        }

        /// <summary>
        /// 获取或是设置服务器的IP地址
        /// </summary>
        /// <remarks>
        /// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
        /// </remarks>
        /// <example>
        /// 以下举例modbus-tcp的短连接及动态更改ip地址的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="IpAddressExample" title="IpAddress示例" />
        /// </example>
        public virtual string IpAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                if (!string.IsNullOrEmpty( value ))
                {
                    if (!IPAddress.TryParse( value, out IPAddress address ))
                    {
                        throw new Exception( StringResources.Language.IpAddresError );
                    }
                    ipAddress = value;
                }
                else
                {
                    ipAddress = "127.0.0.1";
                }
            }
        }

        /// <summary>
        /// 获取或设置服务器的端口号
        /// </summary>
        /// <remarks>
        /// 最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改
        /// </remarks>
        /// <example>
        /// 动态更改请参照IpAddress属性的更改。
        /// </example>
        public virtual int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }

        /// <summary>
        /// 当前连接的唯一ID号，默认为长度20的guid码加随机数组成，方便列表管理，也可以自己指定
        /// </summary>
        /// <remarks>
        /// Current Connection ID, conclude guid and random data, also, you can spcified
        /// </remarks>
        public string ConnectionId
        {
            get { return connectionId; }
            set { connectionId = value; }
        }

        /// <summary>
        /// 当前的异形连接对象，如果设置了异形连接的话
        /// </summary>
        /// <remarks>
        /// 具体的使用方法请参照Demo项目中的异形modbus实现。
        /// </remarks>
        public AlienSession AlienSession { get; set; }

        #endregion

        #region Public Method

        /// <summary>
        /// 在读取数据之前可以调用本方法将客户端设置为长连接模式，相当于跳过了ConnectServer的结果验证，对异形客户端无效
        /// </summary>
        /// <example>
        /// 以下的方式演示了另一种长连接的机制
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="SetPersistentConnectionExample" title="SetPersistentConnection示例" />
        /// </example>
        public void SetPersistentConnection( )
        {
            isPersistentConn = true;
        }

        #endregion

        #region Connect Close

        /// <summary>
        /// 切换短连接模式到长连接模式，后面的每次请求都共享一个通道
        /// </summary>
        /// <returns>返回连接结果，如果失败的话（也即IsSuccess为False），包含失败信息</returns>
        /// <example>
        ///   简单的连接示例，调用该方法后，连接设备，创建一个长连接的对象，后续的读写操作均公用一个连接对象。
        ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="Connect1" title="连接设备" />
        ///   如果想知道是否连接成功，请参照下面的代码。
        ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="Connect2" title="判断连接结果" />
        /// </example> 
        public OperateResult ConnectServer( )
        {
            isPersistentConn = true;
            OperateResult result = new OperateResult( );

            // 重新连接之前，先将旧的数据进行清空
            CoreSocket?.Close( );

            OperateResult<Socket> rSocket = CreateSocketAndInitialication( );

            if (!rSocket.IsSuccess)
            {
                IsSocketError = true;
                rSocket.Content = null;
                result.Message = rSocket.Message;
            }
            else
            {
                CoreSocket = rSocket.Content;
                result.IsSuccess = true;
                LogNet?.WriteDebug( ToString( ), StringResources.Language.NetEngineStart );
            }

            return result;
        }

        /// <summary>
        /// 使用指定的套接字创建异形客户端
        /// </summary>
        /// <param name="session">异形客户端对象，查看<seealso cref="NetworkAlienClient"/>类型创建的客户端</param>
        /// <returns>通常都为成功</returns>
        /// <example>
        ///   简单的创建示例。
        ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="AlienConnect1" title="连接设备" />
        ///   如果想知道是否创建成功。通常都是成功。
        ///   <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="AlienConnect2" title="判断连接结果" />
        /// </example> 
        /// <remarks>
        /// 不能和之前的长连接和短连接混用，详细参考 Demo程序 
        /// </remarks>
        public OperateResult ConnectServer( AlienSession session )
        {
            isPersistentConn = true;
            isUseSpecifiedSocket = true;


            if (session != null)
            {
                AlienSession?.Socket?.Close( );

                if (string.IsNullOrEmpty( ConnectionId ))
                {
                    ConnectionId = session.DTU;
                }

                if (ConnectionId == session.DTU)
                {
                    CoreSocket = session.Socket;
                    IsSocketError = false;
                    AlienSession = session;
                    return InitializationOnConnect( session.Socket );
                }
                else
                {
                    IsSocketError = true;
                    return new OperateResult( );
                }
            }
            else
            {
                IsSocketError = true;
                return new OperateResult( );
            }
        }

        /// <summary>
        /// 在长连接模式下，断开服务器的连接，并切换到短连接模式
        /// </summary>
        /// <returns>关闭连接，不需要查看IsSuccess属性查看</returns>
        /// <example>
        /// 直接关闭连接即可，基本上是不需要进行成功的判定
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ConnectCloseExample" title="关闭连接结果" />
        /// </example>
        public OperateResult ConnectClose( )
        {
            OperateResult result = new OperateResult( );
            isPersistentConn = false;

            InteractiveLock.Enter( );
            // 额外操作
            result = ExtraOnDisconnect( CoreSocket );
            // 关闭信息
            CoreSocket?.Close( );
            CoreSocket = null;
            InteractiveLock.Leave( );

            LogNet?.WriteDebug( ToString( ), StringResources.Language.NetEngineClose );
            return result;
        }

        #endregion

        #region Initialization And Extra

        /// <summary>
        /// 连接上服务器后需要进行的初始化操作
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>是否初始化成功，依据具体的协议进行重写</returns>
        /// <example>
        /// 有些协议不需要握手信号，比如三菱的MC协议，Modbus协议，西门子和欧姆龙就存在握手信息，此处的例子是继承本类后重写的西门子的协议示例
        /// <code lang="cs" source="HslCommunication_Net45\Profinet\Siemens\SiemensS7Net.cs" region="NetworkDoubleBase Override" title="西门子重连示例" />
        /// </example>
        protected virtual OperateResult InitializationOnConnect( Socket socket )
        {
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 在将要和服务器进行断开的情况下额外的操作，需要根据对应协议进行重写
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <example>
        /// 目前暂无相关的示例，组件支持的协议都不用实现这个方法。
        /// </example>
        /// <returns>当断开连接时额外的操作结果</returns>
        protected virtual OperateResult ExtraOnDisconnect( Socket socket )
        {
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 和服务器交互完成的时候调用的方法，无论是成功或是失败，都将会调用，具体的操作需要重写实现
        /// </summary>
        /// <param name="read">读取结果</param>
        protected virtual void ExtraAfterReadFromCoreServer( OperateResult read )
        {

        }

        #endregion

        #region Account Control

        /************************************************************************************************
         * 
         *    这部分的内容是为了实现账户控制的，如果服务器按照hsl协议强制socket账户登录的话，本客户端类就需要额外指定账户密码
         *    
         *    The content of this part is for account control. If the server forces the socket account to log in according to the hsl protocol,
         *    the client class needs to specify the account password.
         *    
         *    适用于hsl实现的modbus服务器，三菱及西门子，NetSimplify服务器类等
         *    
         *    Modbus server for hsl implementation, Mitsubishi and Siemens, NetSimplify server class, etc.
         * 
         ************************************************************************************************/

        /// <summary>
        /// 是否使用账号登录
        /// </summary>
        protected bool isUseAccountCertificate = false;
        private string userName = string.Empty;
        private string password = string.Empty;

        /// <summary>
        /// 设置当前的登录的账户名和密码信息，账户名为空时设置不生效
        /// </summary>
        /// <param name="userName">账户名</param>
        /// <param name="password">密码</param>
        public void SetLoginAccount(string userName, string password )
        {
            if (!string.IsNullOrEmpty( userName.Trim( ) ))
            {
                isUseAccountCertificate = true;
                this.userName = userName;
                this.password = password;
            }
            else
            {
                isUseAccountCertificate = false;
            }
        }

        /// <summary>
        /// 认证账号，将使用已经设置的用户名和密码进行账号认证。
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <returns>认证结果</returns>
        protected OperateResult AccountCertificate(Socket socket )
        {
            OperateResult send = SendAccountAndCheckReceive( socket, 1, this.userName, this.password );
            if (!send.IsSuccess) return send;

            OperateResult<int, string[]> read = ReceiveStringArrayContentFromSocket( socket );
            if (!read.IsSuccess) return read;

            if (read.Content1 == 0) return new OperateResult( read.Content2[0] );
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Core Communication

        /***************************************************************************************
         * 
         *    主要的数据交互分为4步
         *    1. 连接服务器，或是获取到旧的使用的网络信息
         *    2. 发送数据信息
         *    3. 接收反馈的数据信息
         *    4. 关闭网络连接，如果是短连接的话
         * 
         **************************************************************************************/

        /// <summary>
        /// 获取本次操作的可用的网络套接字
        /// </summary>
        /// <returns>是否成功，如果成功，使用这个套接字</returns>
        protected OperateResult<Socket> GetAvailableSocket( )
        {
            if (isPersistentConn)
            {
                // 如果是异形模式
                if (isUseSpecifiedSocket)
                {
                    if (IsSocketError)
                    {
                        return new OperateResult<Socket>( StringResources.Language.ConnectionIsNotAvailable );
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult( CoreSocket );
                    }
                }
                else
                {
                    // 长连接模式
                    if (IsSocketError || CoreSocket == null)
                    {
                        OperateResult connect = ConnectServer( );
                        if (!connect.IsSuccess)
                        {
                            IsSocketError = true;
                            return OperateResult.CreateFailedResult<Socket>( connect );
                        }
                        else
                        {
                            IsSocketError = false;
                            return OperateResult.CreateSuccessResult( CoreSocket );
                        }
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult( CoreSocket );
                    }
                }
            }
            else
            {
                // 短连接模式
                return CreateSocketAndInitialication( );
            }
        }

        /// <summary>
        /// 连接并初始化网络套接字
        /// </summary>
        /// <returns>带有socket的结果对象</returns>
        private OperateResult<Socket> CreateSocketAndInitialication( )
        {
            OperateResult<Socket> result = CreateSocketAndConnect( new IPEndPoint( IPAddress.Parse( ipAddress ), port ), connectTimeOut );
            if (result.IsSuccess)
            {
                // 初始化
                OperateResult initi = InitializationOnConnect( result.Content );
                if (!initi.IsSuccess)
                {
                    result.Content?.Close( );
                    result.IsSuccess = initi.IsSuccess;
                    result.CopyErrorFromOther( initi );
                }
            }
            return result;
        }

        /// <summary>
        /// 在其他指定的套接字上，使用报文来通讯，传入需要发送的消息，返回一条完整的数据指令
        /// </summary>
        /// <param name="socket">指定的套接字</param>
        /// <param name="send">发送的完整的报文信息</param>
        /// <remarks>
        /// 无锁的基于套接字直接进行叠加协议的操作。
        /// </remarks>
        /// <example>
        /// 假设你有一个自己的socket连接了设备，本组件可以直接基于该socket实现modbus读取，三菱读取，西门子读取等等操作，前提是该服务器支持多协议，虽然这个需求听上去比较变态，但本组件支持这样的操作。
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReadFromCoreServerExample1" title="ReadFromCoreServer示例" />
        /// </example>
        /// <returns>接收的完整的报文信息</returns>
        public virtual OperateResult<byte[]> ReadFromCoreServer( Socket socket, byte[] send )
        {
            LogNet?.WriteDebug( ToString( ), StringResources.Language.Send + " : " + BasicFramework.SoftBasic.ByteToHexString( send, ' ' ) );

            TNetMessage netMessage = new TNetMessage( );
            netMessage.SendBytes = send;

            // send
            OperateResult sendResult = Send( socket, send );
            if (!sendResult.IsSuccess)
            {
                socket?.Close( );
                return OperateResult.CreateFailedResult<byte[]>( sendResult );
            }

            if (receiveTimeOut < 0) return OperateResult.CreateSuccessResult( new byte[0] );

            // receive msg
            OperateResult<byte[]> resultReceive = ReceiveByMessage( socket, receiveTimeOut, netMessage );
            if (!resultReceive.IsSuccess)
            {
                socket?.Close( );
                return new OperateResult<byte[]>( StringResources.Language.ReceiveDataTimeout + receiveTimeOut );
            }

            LogNet?.WriteDebug( ToString( ), StringResources.Language.Receive + " : " + BasicFramework.SoftBasic.ByteToHexString( resultReceive.Content, ' ' ) );

            // check
            if (!netMessage.CheckHeadBytesLegal( Token.ToByteArray( ) ))
            {
                socket?.Close( );
                return new OperateResult<byte[]>( StringResources.Language.CommandHeadCodeCheckFailed );
            }

            // Success
            return OperateResult.CreateSuccessResult( resultReceive.Content );
        }


        /// <summary>
        /// 使用底层的数据报文来通讯，传入需要发送的消息，返回一条完整的数据指令
        /// </summary>
        /// <param name="send">发送的完整的报文信息</param>
        /// <returns>接收的完整的报文信息</returns>
        /// <remarks>
        /// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
        /// </remarks>
        /// <example>
        /// 此处举例有个modbus服务器，有个特殊的功能码0x09，后面携带子数据0x01即可，发送字节为 0x00 0x00 0x00 0x00 0x00 0x03 0x01 0x09 0x01
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReadFromCoreServerExample2" title="ReadFromCoreServer示例" />
        /// </example>
        public OperateResult<byte[]> ReadFromCoreServer( byte[] send )
        {
            var result = new OperateResult<byte[]>( );

            InteractiveLock.Enter( );

            // 获取有用的网络通道，如果没有，就建立新的连接
            OperateResult<Socket> resultSocket = GetAvailableSocket( );
            if (!resultSocket.IsSuccess)
            {
                IsSocketError = true;
                if (AlienSession != null) AlienSession.IsStatusOk = false;
                InteractiveLock.Leave( );
                result.CopyErrorFromOther( resultSocket );
                return result;
            }

            OperateResult<byte[]> read = ReadFromCoreServer( resultSocket.Content, send );

            if (read.IsSuccess)
            {
                IsSocketError = false;
                result.IsSuccess = read.IsSuccess;
                result.Content = read.Content;
                result.Message = StringResources.Language.SuccessText;
            }
            else
            {
                IsSocketError = true;
                if (AlienSession != null) AlienSession.IsStatusOk = false;
                result.CopyErrorFromOther( read );
            }

            ExtraAfterReadFromCoreServer( read );

            InteractiveLock.Leave( );
            if (!isPersistentConn) resultSocket.Content?.Close( );
            return result;
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 释放当前的资源，并自动关闭长连接，如果设置了的话
        /// </summary>
        /// <param name="disposing">是否释放托管的资源信息</param>
        protected virtual void Dispose( bool disposing )
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    ConnectClose( );
                    InteractiveLock?.Dispose( );
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~NetworkDoubleBase()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        /// <summary>
        /// 释放当前的资源
        /// </summary>
        public void Dispose( )
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose( true );
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return $"NetworkDoubleBase<{typeof( TNetMessage )}, {typeof( TTransform )}>";
        }

        #endregion
    }
}
