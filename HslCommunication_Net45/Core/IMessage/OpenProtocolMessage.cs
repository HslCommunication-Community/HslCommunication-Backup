using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{
    /// <summary>
    /// OpenProtocol协议的消息
    /// </summary>
    public class OpenProtocolMessage : INetMessage
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
            if (HeadBytes == null) return false;

            return true;
        }

        /// <summary>
        /// 从头子节信息中解析出接下来需要接收的数据长度
        /// </summary>
        /// <returns>接下来的数据长度</returns>
        public int GetContentLengthByHeadBytes( )
        {
            if (HeadBytes?.Length >= 4)
            {
                return Convert.ToInt32( Encoding.ASCII.GetString( HeadBytes, 0, 4 ) ) - 4;
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
        public int GetHeadBytesIdentity( )
        {
            return 0;
        }

        /// <summary>
        /// 发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }
    }
}
