using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{
    /// <summary>
    /// Kuka机器人的 KRC4 控制器中的服务器KUKAVARPROXY
    /// </summary>
    public class KukaVarProxyMessage : INetMessage
    {
        /// <summary>
        /// 本协议的消息头长度
        /// </summary>
        public int ProtocolHeadBytesLength
        {
            get { return 4; }
        }

        /// <summary>
        /// 头子节信息
        /// </summary>
        public byte[] HeadBytes { get; set; }

        /// <summary>
        /// 内容字节信息
        /// </summary>
        public byte[] ContentBytes { get; set; }
        
        /// <summary>
        /// 检查接收的数据是否合法
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否合法</returns>
        public bool CheckHeadBytesLegal( byte[] token )
        {
            return true;
        }

        /// <summary>
        /// 从头子节信息中解析出接下来需要接收的数据长度
        /// </summary>
        /// <returns>接下来的数据长度</returns>
        public int GetContentLengthByHeadBytes()
        {
            if (HeadBytes?.Length >= 4)
            {
                return HeadBytes[2] * 256 + HeadBytes[3];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取头子节里的特殊标识
        /// </summary>
        /// <returns>标识信息</returns>
        public int GetHeadBytesIdentity()
        {
            if (HeadBytes?.Length >= 4)
            {
                return HeadBytes[0] * 256 + HeadBytes[1];
            }
            else
            {
                return 0;
            }
        }
        
        /// <summary>
        /// 发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }
    }
}
