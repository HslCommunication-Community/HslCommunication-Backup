using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.Address
{
    /// <summary>
    /// 设备地址数据的信息，通常包含起始地址，数据类型，长度
    /// </summary>
    public class DeviceAddressDataBase
    {
        /// <summary>
        /// 数字的起始地址，也就是偏移地址
        /// </summary>
        public int AddressStart { get; set; }

        /// <summary>
        /// 读取的数据长度
        /// </summary>
        public ushort Length { get; set; }

        /// <summary>
        /// 从指定的地址信息解析成真正的设备地址信息
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        public virtual void Parse( string address, ushort length )
        {

        }

    }
}
