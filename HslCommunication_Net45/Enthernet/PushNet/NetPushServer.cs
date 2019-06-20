using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using HslCommunication.Core;
using System.Net;



/**********************************************************************************
 * 
 *    发布订阅类的服务器类
 *    
 *    实现从客户端进行数据的订阅操作
 * 
 *********************************************************************************/




namespace HslCommunication.Enthernet
{
    /// <summary>
    /// 发布订阅服务器的类，支持按照关键字进行数据信息的订阅
    /// </summary>
    /// <remarks>
    /// 详细的使用说明，请参照博客<a href="http://www.cnblogs.com/dathlin/p/8992315.html">http://www.cnblogs.com/dathlin/p/8992315.html</a>
    /// </remarks>
    /// <example>
    /// 此处贴上了Demo项目的服务器配置的示例代码
    /// <code lang="cs" source="TestProject\PushNetServer\FormServer.cs" region="NetPushServer" title="NetPushServer示例" />
    /// </example>
    public class NetPushServer : NetworkServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个对象
        /// </summary>
        public NetPushServer()
        {
            dictPushClients        = new Dictionary<string, PushGroupClient>( );
            dictSendHistory        = new Dictionary<string, string>( );
            dicHybirdLock          = new SimpleHybirdLock( );
            dicSendCacheLock       = new SimpleHybirdLock( );
            sendAction             = new Action<AppSession, string>( SendString );

            hybirdLock             = new SimpleHybirdLock( );
            pushClients            = new List<NetPushClient>( );
        }


        #endregion

        #region Server Override

        /// <summary>
        /// 当接收到了新的请求的时候执行的操作
        /// </summary>
        /// <param name="socket">异步对象</param>
        /// <param name="endPoint">终结点</param>
        protected override void ThreadPoolLogin( Socket socket, IPEndPoint endPoint )
        {
            // 接收一条信息，指定当前请求的数据订阅信息的关键字
            OperateResult<int, string> receive = ReceiveStringContentFromSocket( socket );
            if (!receive.IsSuccess) return;

            // 判断当前的关键字在服务器是否有消息发布
            //if(!IsPushGroupOnline(receive.Content2))
            //{
            //    SendStringAndCheckReceive( socket, 1, StringResources.Language.KeyIsNotExist );
            //    LogNet?.WriteWarn( ToString( ), StringResources.Language.KeyIsNotExist );
            //    socket?.Close( );
            //    return;
            //}

            // 确认订阅的信息
            OperateResult check = SendStringAndCheckReceive( socket, 0, "" );
            if (!check.IsSuccess)
            {
                socket?.Close( );
                return;
            }

            // 允许发布订阅信息
            AppSession session = new AppSession
            {
                KeyGroup = receive.Content2,
                WorkSocket = socket
            };

            try
            {
                session.IpEndPoint = (System.Net.IPEndPoint)socket.RemoteEndPoint;
                session.IpAddress = session.IpEndPoint.Address.ToString( );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.GetClientIpaddressFailed, ex );
            }

            try
            {
                socket.BeginReceive( session.BytesHead, 0, session.BytesHead.Length, SocketFlags.None, new AsyncCallback( ReceiveCallback ), session );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.SocketReceiveException, ex );
                return;
            }

            LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOnlineInfo, session.IpEndPoint ) );
            PushGroupClient push = GetPushGroupClient( receive.Content2 );
            if (push != null)
            {
                System.Threading.Interlocked.Increment( ref onlineCount );
                push.AddPushClient( session );

                dicSendCacheLock.Enter( );
                if (dictSendHistory.ContainsKey( receive.Content2 ))
                {
                    if (isPushCacheAfterConnect) SendString( session, dictSendHistory[receive.Content2] );
                }
                dicSendCacheLock.Leave( );
            }
        }

        /// <summary>
        /// 关闭服务器的引擎
        /// </summary>
        public override void ServerClose( )
        {
            base.ServerClose( );
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 主动推送数据内容
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="content">数据内容</param>
        public void PushString( string key, string content )
        {
            dicSendCacheLock.Enter( );
            if (dictSendHistory.ContainsKey( key ))
            {
                dictSendHistory[key] = content;
            }
            else
            {
                dictSendHistory.Add( key, content );
            }
            dicSendCacheLock.Leave( );


            AddPushKey( key );
            GetPushGroupClient( key )?.PushString( content, sendAction );
        }

        /// <summary>
        /// 移除关键字信息，通常应用于一些特殊临时用途的关键字
        /// </summary>
        /// <param name="key">关键字</param>
        public void RemoveKey( string key )
        {
            dicHybirdLock.Enter( );

            if (dictPushClients.ContainsKey( key ))
            {
                int count = dictPushClients[key].RemoveAllClient( );
                for (int i = 0; i < count; i++)
                {
                    System.Threading.Interlocked.Decrement( ref onlineCount );
                }
            }

            dicHybirdLock.Leave( );
        }


        /// <summary>
        /// 创建一个远程服务器的数据推送操作，以便推送给子客户端
        /// </summary>
        /// <param name="ipAddress">远程的IP地址</param>
        /// <param name="port">远程的端口号</param>
        /// <param name="key">订阅的关键字</param>
        public OperateResult CreatePushRemote( string ipAddress, int port, string key )
        {
            OperateResult result;

            hybirdLock.Enter( );


            if (pushClients.Find( m => m.KeyWord == key ) == null)
            {
                NetPushClient pushClient = new NetPushClient( ipAddress, port, key );
                result = pushClient.CreatePush( GetPushFromServer );
                pushClients.Add( pushClient );
            }
            else
            {
                result = new OperateResult( StringResources.Language.KeyIsExistAlready );
            }

            hybirdLock.Leave( );

            return result;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// 在线客户端的数量
        /// </summary>
        public int OnlineCount
        {
            get => onlineCount;
        }

        /// <summary>
        /// 在客户端上线之后，是否推送缓存的数据，默认设置为true
        /// </summary>
        public bool PushCacheAfterConnect
        {
            get { return isPushCacheAfterConnect; }
            set { isPushCacheAfterConnect = value; }
        }

        #endregion

        #region Private Method


        private void ReceiveCallback( IAsyncResult ar )
        {
            if (ar.AsyncState is AppSession session)
            {
                try
                {
                    Socket client = session.WorkSocket;
                    int bytesRead = client.EndReceive( ar );

                    // 正常下线退出
                    LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, session.IpEndPoint ) );
                    RemoveGroupOnlien( session.KeyGroup, session.ClientUniqueID );
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains( StringResources.Language.SocketRemoteCloseException ))
                    {
                        // 正常下线
                        LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, session.IpEndPoint ) );
                        RemoveGroupOnlien( session.KeyGroup, session.ClientUniqueID );
                    }
                    else
                    {
                        LogNet?.WriteException( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, session.IpEndPoint ), ex );
                        RemoveGroupOnlien( session.KeyGroup, session.ClientUniqueID );
                    }
                }
            }
        }



        /// <summary>
        /// 判断当前的关键字订阅是否在服务器的词典里面
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsPushGroupOnline(string key)
        {
            bool result = false;

            dicHybirdLock.Enter( );

            if (dictPushClients.ContainsKey( key )) result = true;

            dicHybirdLock.Leave( );

            return result;
        }

        private void AddPushKey(string key)
        {
            dicHybirdLock.Enter( );

            if (!dictPushClients.ContainsKey( key ))
            {
                dictPushClients.Add( key, new PushGroupClient( ) );
            }

            dicHybirdLock.Leave( );
        }

        private PushGroupClient GetPushGroupClient(string key)
        {
            PushGroupClient result = null;
            dicHybirdLock.Enter( );

            if (dictPushClients.ContainsKey( key ))
            {
                result = dictPushClients[key];
            }
            else
            {
                result = new PushGroupClient( );
                dictPushClients.Add( key, result );
            }

            dicHybirdLock.Leave( );

            return result;
        }

        /// <summary>
        /// 移除客户端的数据信息
        /// </summary>
        /// <param name="key">指定的客户端</param>
        /// <param name="clientID">指定的客户端唯一的id信息</param>
        private void RemoveGroupOnlien( string key, string clientID )
        {
            PushGroupClient push = GetPushGroupClient( key );
            if (push != null)
            {
                if (push.RemovePushClient( clientID ))
                {
                    // 移除成功
                    System.Threading.Interlocked.Decrement( ref onlineCount );
                }
            }
        }


        private void SendString(AppSession appSession, string content)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(m =>
            {
                PushSendAsync(appSession, HslProtocol.CommandBytes(0, Token, content));
            }), null);
        }


        #region Send Bytes Async

        /// <summary>
        /// 发送数据的方法
        /// </summary>
        /// <param name="session">通信用的核心对象</param>
        /// <param name="content">完整的字节信息</param>
        internal void PushSendAsync( AppSession session, byte[] content )
        {
            try
            {
                // 进入发送数据的锁，然后开启异步的数据发送
                session.HybirdLockSend.Enter( );

                // 启用另外一个网络封装对象进行发送数据
                AsyncStateSend state = new AsyncStateSend( )
                {
                    WorkSocket          = session.WorkSocket,
                    Content             = content,
                    AlreadySendLength   = 0,
                    HybirdLockSend      = session.HybirdLockSend,
                    Key                 = session.KeyGroup,
                    ClientId            = session.ClientUniqueID,
                };

                state.WorkSocket.BeginSend(
                    state.Content,
                    state.AlreadySendLength,
                    state.Content.Length - state.AlreadySendLength,
                    SocketFlags.None,
                    new AsyncCallback( PushSendCallBack ),
                    state );
            }
            catch (ObjectDisposedException)
            {
                // 不操作
                session.HybirdLockSend.Leave( );
                RemoveGroupOnlien( session.KeyGroup, session.ClientUniqueID );
            }
            catch (Exception ex)
            {
                session.HybirdLockSend.Leave( );
                if (!ex.Message.Contains( StringResources.Language.SocketRemoteCloseException ))
                {
                    LogNet?.WriteException( ToString( ), StringResources.Language.SocketSendException, ex );
                }
                RemoveGroupOnlien( session.KeyGroup, session.ClientUniqueID );
            }
        }

        /// <summary>
        /// 发送回发方法
        /// </summary>
        /// <param name="ar">异步数据</param>
        internal void PushSendCallBack( IAsyncResult ar )
        {
            if (ar.AsyncState is AsyncStateSend stateone)
            {
                try
                {
                    stateone.AlreadySendLength += stateone.WorkSocket.EndSend( ar );
                    if (stateone.AlreadySendLength < stateone.Content.Length)
                    {
                        // 继续发送
                        stateone.WorkSocket.BeginSend( stateone.Content,
                        stateone.AlreadySendLength,
                        stateone.Content.Length - stateone.AlreadySendLength,
                        SocketFlags.None,
                        new AsyncCallback( PushSendCallBack ),
                        stateone );
                    }
                    else
                    {
                        stateone.HybirdLockSend.Leave( );
                        // 发送完成
                        stateone = null;
                    }
                }
                catch (ObjectDisposedException)
                {
                    stateone.HybirdLockSend.Leave( );
                    RemoveGroupOnlien( stateone.Key, stateone.ClientId );
                }
                catch (Exception ex)
                {
                    LogNet?.WriteException( ToString( ), StringResources.Language.SocketEndSendException, ex );
                    stateone.HybirdLockSend.Leave( );
                    RemoveGroupOnlien( stateone.Key, stateone.ClientId );
                }
            }
        }

        #endregion


        private void GetPushFromServer( NetPushClient pushClient, string data )
        {
            // 推送给其他的客户端，当然也有可能是工作站
            PushString( pushClient.KeyWord, data );
        }


        #endregion

        #region Private Member

        private Dictionary<string, string> dictSendHistory;                  // 词典缓存的数据发送对象
        private Dictionary<string, PushGroupClient> dictPushClients;         // 系统的数据词典
        private SimpleHybirdLock dicHybirdLock;                              // 词典锁
        private SimpleHybirdLock dicSendCacheLock;                           // 缓存数据的锁
        private Action<AppSession, string> sendAction;                       // 发送数据的委托
        private int onlineCount = 0;                                         // 在线客户端的数量，用于监视显示
        private List<NetPushClient> pushClients;                             // 客户端列表
        private SimpleHybirdLock hybirdLock;                                 // 客户端列表的锁
        private bool isPushCacheAfterConnect = true;                         // 在客户端上线之后，是否推送缓存的数据

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return "NetPushServer";
        }


        #endregion
    }

}
