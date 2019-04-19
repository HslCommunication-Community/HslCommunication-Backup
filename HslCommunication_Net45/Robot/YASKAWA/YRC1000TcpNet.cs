using System;
using System.Collections.Generic;
using System.Linq;
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





        #region Private Member

        private string connectStr = "CONNECT Robot_access KeepAlive:-1\r\n";

        #endregion
    }
}
