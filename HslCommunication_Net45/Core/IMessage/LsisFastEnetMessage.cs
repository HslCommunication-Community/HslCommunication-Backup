using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.IMessage
{
    /// <summary>
    /// LSIS的PLC的FastEnet的消息定义
    /// </summary>
    public class LsisFastEnetMessage : INetMessage
    {

        /// <summary>
        /// 西门子头字节的长度
        /// </summary>
        public int ProtocolHeadBytesLength
        {
            get { return 20; }
        }


        /// <summary>
        /// 头子节的数据
        /// </summary>
        public byte[] HeadBytes { get; set; }


        /// <summary>
        /// 内容字节的数据
        /// </summary>
        public byte[] ContentBytes { get; set; }


        /// <summary>
        /// 检查头子节是否合法的判断
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>是否合法的</returns>
        public bool CheckHeadBytesLegal( byte[] token )
        {
            if (HeadBytes == null) return false;

            if (HeadBytes[0] == 0x4C )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 获取剩余的内容长度
        /// </summary>
        /// <returns>数据内容长度</returns>
        public int GetContentLengthByHeadBytes( )
        {
            if (HeadBytes?.Length >= 20)
            {
                return BitConverter.ToUInt16( HeadBytes, 16 );
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取消息号，此处无效
        /// </summary>
        /// <returns>消息标识</returns>
        public int GetHeadBytesIdentity( )
        {
            return BitConverter.ToUInt16( HeadBytes, 14 );
        }


        /// <summary>
        /// 发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }
    }
}
