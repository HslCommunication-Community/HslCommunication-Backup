using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication
{
    /// <summary>
    /// 应用于Hsl组件库读取的动态地址解析
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class HslDeviceAddressAttribute : Attribute
    {
        /// <summary>
        /// 设备的类似，这将决定是否使用当前的PLC地址
        /// </summary>
        public Type deviceType { get; set; }

        /// <summary>
        /// 数据的地址信息
        /// </summary>
        public string address { get; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public int length { get; }

        /// <summary>
        /// 实例化一个地址特性，指定地址信息
        /// </summary>
        /// <param name="address">真实的地址信息</param>
        public HslDeviceAddressAttribute(string address )
        {
            this.address = address;
            this.length = -1;
            this.deviceType = null;
        }

        /// <summary>
        /// 实例化一个地址特性，指定地址信息
        /// </summary>
        /// <param name="address">真实的地址信息</param>
        /// <param name="deviceType">设备的地址信息</param>
        public HslDeviceAddressAttribute( string address, Type deviceType )
        {
            this.address = address;
            this.length = -1;
            this.deviceType = deviceType;
        }

        /// <summary>
        /// 实例化一个地址特性，指定地址信息和数据长度，通常应用于数组的批量读取
        /// </summary>
        /// <param name="address">真实的地址信息</param>
        /// <param name="length">读取的数据长度</param>
        public HslDeviceAddressAttribute(string address, int length )
        {
            this.address = address;
            this.length = length;
            this.deviceType = null;
        }

        /// <summary>
        /// 实例化一个地址特性，指定地址信息和数据长度，通常应用于数组的批量读取
        /// </summary>
        /// <param name="address">真实的地址信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <param name="deviceType">设备类型</param>
        public HslDeviceAddressAttribute( string address, int length, Type deviceType )
        {
            this.address = address;
            this.length = length;
            this.deviceType = deviceType;
        }
    }
}
