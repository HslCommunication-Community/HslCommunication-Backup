using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core.Address;
using HslCommunication.Profinet.Melsec;

namespace HslCommunication.Profinet.Keyence
{
    /// <summary>
    /// 基恩士PLC的数据通信类
    /// </summary>
    /// <remarks>
    /// 地址的输入的格式说明如下：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>示例</term>
    ///     <term>地址进制</term>
    ///     <term>字操作</term>
    ///     <term>位操作</term>
    ///     <term>KV-7500/7300</term>
    ///     <term>KV-5500/5000/3000</term>
    ///     <term>KV Nano</term>
    ///   </listheader>
    ///   <item>
    ///     <term>输入继电器</term>
    ///     <term>X</term>
    ///     <term>X100,X1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>R00000~R99915</term>
    ///     <term>R00000~R99915</term>
    ///     <term>R00000～R59915</term>
    ///   </item>
    ///   <item>
    ///     <term>输出继电器</term>
    ///     <term>Y</term>
    ///     <term>Y100,Y1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>R00000~R99915</term>
    ///     <term>R00000~R99915</term>
    ///     <term>R00000～R59915</term>
    ///   </item>
    ///   <item>
    ///     <term>链接继电器</term>
    ///     <term>B</term>
    ///     <term>B100,B1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>B0000~B7FFF</term>
    ///     <term>B0000~B3FFF</term>
    ///     <term>B0000~B1FFF</term>
    ///   </item>
    ///   <item>
    ///     <term>内部辅助继电器</term>
    ///     <term>M</term>
    ///     <term>M100,M200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>MR00000~MR99915</term>
    ///     <term>MR00000~MR99915</term>
    ///     <term>MR00000～MR59915</term>
    ///   </item>
    ///   <item>
    ///     <term>锁存继电器</term>
    ///     <term>L</term>
    ///     <term>L100,L200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>LR00000~LR99915</term>
    ///     <term>LR00000~LR99915</term>
    ///     <term>LR00000～LR19915</term>
    ///   </item>
    ///   <item>
    ///     <term>控制继电器</term>
    ///     <term>SM</term>
    ///     <term>SM100,SM200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>CR0000~CR7915</term>
    ///     <term>CR0000~CR3915</term>
    ///     <term>CR0000～CR8915</term>
    ///   </item>
    ///   <item>
    ///     <term>控制存储器</term>
    ///     <term>SD</term>
    ///     <term>SD100,SD200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>CM0000~CM5999</term>
    ///     <term>CM0000~CM5999</term>
    ///     <term>CM0000～CM8999</term>
    ///   </item>
    ///   <item>
    ///     <term>数据存储器</term>
    ///     <term>D</term>
    ///     <term>D100,D200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>DM00000~DM65534</term>
    ///     <term>DM00000~DM65534</term>
    ///     <term>DM00000～DM32767</term>
    ///   </item>
    ///   <item>
    ///     <term>扩展数据存储器</term>
    ///     <term>D</term>
    ///     <term>D100000~D165534</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>EM00000~EM65534</term>
    ///     <term>EM00000~EM65534</term>
    ///     <term>×</term>
    ///   </item>
    ///   <item>
    ///     <term>文件寄存器</term>
    ///     <term>R</term>
    ///     <term>R100,R200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>FM00000~FM32767</term>
    ///     <term>FM00000~FM32767</term>
    ///     <term>×</term>
    ///   </item>
    ///   <item>
    ///     <term>文件寄存器</term>
    ///     <term>ZR</term>
    ///     <term>ZR100,ZR1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>ZF000000~ZF524287</term>
    ///     <term>ZF000000~ZF131071</term>
    ///     <term>×</term>
    ///   </item>
    ///   <item>
    ///     <term>链路寄存器</term>
    ///     <term>W</term>
    ///     <term>W100,W1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>W0000~7FFF</term>
    ///     <term>W0000~3FFF</term>
    ///     <term>W0000~3FFF</term>
    ///   </item>
    ///   <item>
    ///     <term>定时器（当前值）</term>
    ///     <term>TN</term>
    ///     <term>TN100,TN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>T0000~T3999</term>
    ///     <term>T0000~T3999</term>
    ///     <term>T000～T511</term>
    ///   </item>
    ///   <item>
    ///     <term>定时器（接点）</term>
    ///     <term>TS</term>
    ///     <term>TS100,TS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>T0000~T3999</term>
    ///     <term>T0000~T3999</term>
    ///     <term>T000～T511</term>
    ///   </item>
    ///   <item>
    ///     <term>计数器（当前值）</term>
    ///     <term>CN</term>
    ///     <term>CN100,CN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term>C0000~C3999</term>
    ///     <term>C0000~C3999</term>
    ///     <term>C000～C255</term>
    ///   </item>
    ///   <item>
    ///     <term>计数器（接点）</term>
    ///     <term>CS</term>
    ///     <term>CS100,CS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>C0000~C3999</term>
    ///     <term>C0000~C3999</term>
    ///     <term>C000～C255</term>
    ///   </item>
    /// </list>
    /// </remarks>
    public class KeyenceMcNet : MelsecMcNet
    {
        #region Constructor

        /// <summary>
        /// 实例化基恩士的Qna兼容3E帧协议的通讯对象
        /// </summary>
        public KeyenceMcNet( ) : base( )
        {

        }

        /// <summary>
        /// 实例化一个基恩士的Qna兼容3E帧协议的通讯对象
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public KeyenceMcNet( string ipAddress, int port ) : base( ipAddress, port )
        {

        }

        #endregion

        #region Address Overeride

        /// <summary>
        /// 分析地址的方法，允许派生类里进行重写操作
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>解析后的数据信息</returns>
        protected override OperateResult<McAddressData> McAnalysisAddress( string address, ushort length )
        {
            return McAddressData.ParseKeyenceFrom( address, length );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 获取当前对象的字符串标识形式
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            return $"KeyenceMcNet[{IpAddress}:{Port}]";
        }
        
        #endregion
    }
}
