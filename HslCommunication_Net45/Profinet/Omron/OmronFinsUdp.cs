using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core.Net;
using HslCommunication.Core;
using System.Net;

namespace HslCommunication.Profinet.Omron
{
    /// <summary>
    /// 欧姆龙的Udp的数据对象
    /// </summary>
    /// <remarks>
    /// <note type="warning">如果在测试的时候报错误码64，经网友 上海-Lex 指点，是因为PLC中产生了报警，如伺服报警，模块错误等产生的，但是数据还是能正常读到的，屏蔽64报警或清除plc错误可解决</note>
    /// 地址支持的列表如下：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>示例</term>
    ///     <term>地址进制</term>
    ///     <term>字操作</term>
    ///     <term>位操作</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>DM Area</term>
    ///     <term>D</term>
    ///     <term>D100,D200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>CIO Area</term>
    ///     <term>C</term>
    ///     <term>C100,C200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>Work Area</term>
    ///     <term>W</term>
    ///     <term>W100,W200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>Holding Bit Area</term>
    ///     <term>H</term>
    ///     <term>H100,H200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>Auxiliary Bit Area</term>
    ///     <term>A</term>
    ///     <term>A100,A200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// </remarks>
    public class OmronFinsUdp : NetworkUdpDeviceBase<ReverseWordTransform>
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的欧姆龙Udp的对象
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public OmronFinsUdp(string ipAddress, int port )
        {
            WordLength = 1;
            IpAddress = ipAddress;
            Port = port;
            ByteTransform.DataFormat = DataFormat.CDAB;
        }

        /// <summary>
        /// 实例化一个默认的欧姆龙Udp的对象
        /// </summary>
        public OmronFinsUdp( )
        {
            WordLength = 1;
            ByteTransform.DataFormat = DataFormat.CDAB;
        }

        #endregion

        #region IpAddress Override

        /// <summary>
        /// 设备的Ip地址信息
        /// </summary>
        public override string IpAddress {
            get => base.IpAddress;
            set
            {
                DA1 = Convert.ToByte( value.Substring( value.LastIndexOf( "." ) + 1 ) );
                base.IpAddress = value;
            }
        }

        #endregion

        #region Public Member

        /// <summary>
        /// 信息控制字段，默认0x80
        /// </summary>
        public byte ICF { get; set; } = 0x80;

        /// <summary>
        /// 系统使用的内部信息
        /// </summary>
        public byte RSV { get; private set; } = 0x00;

        /// <summary>
        /// 网络层信息，默认0x02，如果有八层消息，就设置为0x07
        /// </summary>
        public byte GCT { get; set; } = 0x02;

        /// <summary>
        /// PLC的网络号地址，默认0x00
        /// </summary>
        public byte DNA { get; set; } = 0x00;

        /// <summary>
        /// PLC的节点地址，这个值在配置了ip地址之后是默认赋值的，默认为Ip地址的最后一位
        /// </summary>
        /// <remarks>
        /// <note type="important">假如你的PLC的Ip地址为192.168.0.10，那么这个值就是10</note>
        /// </remarks>
        public byte DA1 { get; set; } = 0x13;

        /// <summary>
        /// PLC的单元号地址
        /// </summary>
        /// <remarks>
        /// <note type="important">通常都为0</note>
        /// </remarks>
        public byte DA2 { get; set; } = 0x00;

        /// <summary>
        /// 上位机的网络号地址
        /// </summary>
        public byte SNA { get; set; } = 0x00;

        /// <summary>
        /// 上位机的节点地址，假如你的电脑的Ip地址为192.168.0.13，那么这个值就是13
        /// </summary>
        /// <remarks>
        /// <note type="important">假如你的电脑的Ip地址为192.168.0.13，那么这个值就是13</note>
        /// </remarks>
        public byte SA1 { get; set; } = 13;

        /// <summary>
        /// 上位机的单元号地址
        /// </summary>
        public byte SA2 { get; set; }

        /// <summary>
        /// 设备的标识号
        /// </summary>
        public byte SID { get; set; } = 0x00;

        #endregion

        #region Build Command

        /// <summary>
        /// 将普通的指令打包成完整的指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private byte[] PackCommand( byte[] cmd )
        {
            byte[] buffer = new byte[10 + cmd.Length];
            buffer[0] = ICF;
            buffer[1] = RSV;
            buffer[2] = GCT;
            buffer[3] = DNA;
            buffer[4] = DA1;
            buffer[5] = DA2;
            buffer[6] = SNA;
            buffer[7] = SA1;
            buffer[8] = SA2;
            buffer[9] = SID;
            cmd.CopyTo( buffer, 10 );

            return buffer;
        }

        /// <summary>
        /// 根据类型地址长度确认需要读取的指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">长度</param>
        /// <param name="isBit">是否是位读取</param>
        /// <returns>带有成功标志的报文数据</returns>
        public OperateResult<byte[]> BuildReadCommand( string address, ushort length, bool isBit )
        {
            var command = OmronFinsNetHelper.BuildReadCommand( address, length, isBit );
            if (!command.IsSuccess) return command;

            return OperateResult.CreateSuccessResult( PackCommand( command.Content ) );
        }

        /// <summary>
        /// 根据类型地址以及需要写入的数据来生成指令头
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">真实的数据值信息</param>
        /// <param name="isBit">是否是位操作</param>
        /// <returns>带有成功标志的报文数据</returns>
        public OperateResult<byte[]> BuildWriteCommand( string address, byte[] value, bool isBit )
        {
            var command = OmronFinsNetHelper.BuildWriteWordCommand( address, value, isBit );
            if (!command.IsSuccess) return command;

            return OperateResult.CreateSuccessResult( PackCommand( command.Content ) );
        }

        #endregion
        
        #region Read Support

        /// <summary>
        /// 从欧姆龙PLC中读取想要的数据，返回读取结果，读取单位为字
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102,D103存储了产量计数，读取如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 获取指令
            var command = BuildReadCommand( address, length, false );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( command );

            // 核心数据交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 数据有效性分析
            OperateResult<byte[]> valid = OmronFinsNetHelper.UdpResponseValidAnalysis( read.Content, true );
            if (!valid.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( valid );

            // 读取到了正确的数据
            return OperateResult.CreateSuccessResult( valid.Content );
        }
        
        /// <summary>
        /// 从欧姆龙PLC中批量读取位软元件，返回读取结果
        /// </summary>
        /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
        /// <param name="length">读取的长度</param>
        /// <returns>带成功标志的结果数据对象</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="ReadBool" title="ReadBool示例" />
        /// </example>
        public override OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            // 获取指令
            var command = BuildReadCommand( address, length, true );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( command );

            // 核心数据交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( read );

            // 数据有效性分析
            OperateResult<byte[]> valid = OmronFinsNetHelper.UdpResponseValidAnalysis( read.Content, true );
            if (!valid.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( valid );

            // 返回正确的数据信息
            return OperateResult.CreateSuccessResult( valid.Content.Select( m => m != 0x00 ? true : false ).ToArray( ) );
        }
        
        #endregion

        #region Write Base
        
        /// <summary>
        /// 向PLC写入数据，数据格式为原始的字节类型
        /// </summary>
        /// <param name="address">初始地址</param>
        /// <param name="value">原始的字节数据</param>
        /// <returns>结果</returns>
        /// <example>
        /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102,D103存储了产量计数，读取如下：
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="WriteExample2" title="Write示例" />
        /// 以下是写入不同类型数据的示例
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        public override OperateResult Write( string address, byte[] value )
        {
            // 获取指令
            var command = BuildWriteCommand( address, value, false );
            if (!command.IsSuccess) return command;

            // 核心数据交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;

            // 数据有效性分析
            OperateResult<byte[]> valid = OmronFinsNetHelper.UdpResponseValidAnalysis( read.Content, false );
            if (!valid.IsSuccess) return valid;

            // 成功
            return OperateResult.CreateSuccessResult( );
        }


        #endregion

        #region Write bool[]

        /// <summary>
        /// 向PLC中位软元件写入bool数组，返回值说明，比如你写入D100,values[0]对应D100.0
        /// </summary>
        /// <param name="address">要写入的数据地址</param>
        /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
        /// <returns>返回写入结果</returns>
        /// <example>
        /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="WriteBool" title="WriteBool示例" />
        /// </example>
        public override OperateResult Write( string address, bool[] values )
        {
            // 获取指令
            var command = BuildWriteCommand( address, values.Select( m => m ? (byte)0x01 : (byte)0x00 ).ToArray( ), true );
            if (!command.IsSuccess) return command;

            // 核心数据交互
            OperateResult<byte[]> read = ReadFromCoreServer( command.Content );
            if (!read.IsSuccess) return read;

            // 数据有效性分析
            OperateResult<byte[]> valid = OmronFinsNetHelper.UdpResponseValidAnalysis( read.Content, false );
            if (!valid.IsSuccess) return valid;

            // 写入成功
            return OperateResult.CreateSuccessResult( );
        }

        #endregion
        
        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return $"OmronFinsUdp[{IpAddress}:{Port}]";
        }

        #endregion
    }
}
