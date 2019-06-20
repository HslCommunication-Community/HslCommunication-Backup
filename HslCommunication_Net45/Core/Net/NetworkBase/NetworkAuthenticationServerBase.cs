using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 带登录认证的服务器类
    /// </summary>
    public class NetworkAuthenticationServerBase : NetworkServerBase
    {
        /// <summary>
        /// 当客户端的socket登录的时候额外检查的信息
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="endPoint">终结点</param>
        /// <returns>验证的结果</returns>
        protected override OperateResult SocketAcceptExtraCheck( Socket socket, IPEndPoint endPoint )
        {
            if (IsUseAccountCertificate)
            {
                OperateResult<byte[], byte[]> receive = ReceiveAndCheckBytes( socket, 2000 );
                if (!receive.IsSuccess) return new OperateResult( string.Format( "Client login failed[{0}]", endPoint ) );

                if (BitConverter.ToInt32( receive.Content1, 0 ) != HslProtocol.ProtocolAccountLogin)
                {
                    LogNet?.WriteError( ToString( ), StringResources.Language.NetClientAccountTimeout );
                    socket?.Close( );
                    return new OperateResult( string.Format( "Client login failed[{0}]", endPoint ) );
                }

                string[] infos = HslProtocol.UnPackStringArrayFromByte( receive.Content2 );
                string ret = CheckAccountLegal( infos );
                SendStringAndCheckReceive( socket, ret == "success" ? 1 : 0, new string[] { ret } );

                if (ret != "success")
                {
                    return new OperateResult( string.Format( "Client login failed[{0}]:{1}", endPoint, ret ) );
                }

                LogNet?.WriteDebug( ToString( ), string.Format( "Account Login:{0} Endpoint:[{1}]", infos[0], endPoint ) );
            }
            return OperateResult.CreateSuccessResult( );
        }

        #region Account Certification

        /// <summary>
        /// 获取或设置是否对客户端启动账号认证
        /// </summary>
        public bool IsUseAccountCertificate { get; set; }

        private Dictionary<string, string> accounts = new Dictionary<string, string>( );
        private SimpleHybirdLock lockLoginAccount = new SimpleHybirdLock( );

        /// <summary>
        /// 新增账户，如果想要启动账户登录，比如将<see cref="IsUseAccountCertificate"/>设置为<c>True</c>。
        /// </summary>
        /// <param name="userName">账户名称</param>
        /// <param name="password">账户名称</param>
        public void AddAccount( string userName, string password )
        {
            if (!string.IsNullOrEmpty( userName ))
            {
                lockLoginAccount.Enter( );
                if (accounts.ContainsKey( userName ))
                {
                    accounts[userName] = password;
                }
                else
                {
                    accounts.Add( userName, password );
                }
                lockLoginAccount.Leave( );
            }
        }

        /// <summary>
        /// 删除一个账户的信息
        /// </summary>
        /// <param name="userName">账户名称</param>
        public void DeleteAccount( string userName )
        {
            lockLoginAccount.Enter( );
            if (accounts.ContainsKey( userName ))
            {
                accounts.Remove( userName );
            }
            lockLoginAccount.Leave( );
        }

        private string CheckAccountLegal( string[] infos )
        {
            if (infos?.Length < 2) return "User Name input wrong";
            string ret = "";
            lockLoginAccount.Enter( );
            if (!accounts.ContainsKey( infos[0] ))
            {
                ret = "User Name input wrong";
            }
            else
            {
                if (accounts[infos[0]] != infos[1])
                {
                    ret = "Password is not corrent";
                }
                else
                {
                    ret = "success";
                }
            }
            lockLoginAccount.Leave( );
            return ret;
        }

        #endregion


    }
}
