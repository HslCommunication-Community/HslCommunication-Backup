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
        /// 读取机器人的所有的原始的字节数据信息
        /// </summary>
        /// <returns>带有成功标识的byte[]数组</returns>
        OperateResult<byte[]> ReadBytes( );

        /// <summary>
        /// 读取机器人的json格式的字节数据信息
        /// </summary>
        /// <returns>带有成功标识的json数据</returns>
        OperateResult<string> ReadJsonString( );
    }
}
