using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.Net
{
    /// <summary>
    /// 所有的机器人的统一读写标准
    /// </summary>
    public interface IRobotNet
    {
        /// <summary>
        /// 根据地址读取机器人的原始的字节数据信息
        /// </summary>
        /// <param name="address">指定的地址信息，对于某些机器人无效</param>
        /// <returns>带有成功标识的byte[]数组</returns>
        OperateResult<byte[]> Read( string address );

        /// <summary>
        /// 根据地址读取机器人的字符串的数据信息
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>带有成功标识的字符串数据</returns>
        OperateResult<string> ReadString( string address );

        /// <summary>
        /// 根据地址，来写入设备的相关的数据
        /// </summary>
        /// <param name="address">指定的地址信息，有些机器人可能不支持</param>
        /// <param name="value">原始的字节数据信息</param>
        /// <returns>是否成功的写入</returns>
        OperateResult Write( string address, byte[] value );

        /// <summary>
        /// 根据地址，来写入设备相关的数据
        /// </summary>
        /// <param name="address">指定的地址信息，有些机器人可能不支持</param>
        /// <param name="value">字符串的数据信息</param>
        /// <returns>是否成功的写入</returns>
        OperateResult Write( string address, string value );
    }
}
