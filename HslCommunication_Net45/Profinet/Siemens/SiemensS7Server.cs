using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Address;

namespace HslCommunication.Profinet.Siemens
{
    /// <summary>
    /// 西门子S7协议的虚拟服务器，支持TCP协议，无视PLC的型号，所以在客户端进行操作操作的时候，选择1200或是1500或是300或是400都是一样的。
    /// </summary>
    /// <remarks>
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
    ///     <term>中间寄存器</term>
    ///     <term>M</term>
    ///     <term>M100,M200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输入寄存器</term>
    ///     <term>I</term>
    ///     <term>I100,I200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输出寄存器</term>
    ///     <term>Q</term>
    ///     <term>Q100,Q200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>DB块寄存器</term>
    ///     <term>DB</term>
    ///     <term>DB1.100,DB1.200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>V寄存器</term>
    ///     <term>V</term>
    ///     <term>V100,V200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>V寄存器本质就是DB块1</term>
    ///   </item>
    ///   <item>
    ///     <term>定时器的值</term>
    ///     <term>T</term>
    ///     <term>T100,T200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>未测试通过</term>
    ///   </item>
    ///   <item>
    ///     <term>计数器的值</term>
    ///     <term>C</term>
    ///     <term>C100,C200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>未测试通过</term>
    ///   </item>
    /// </list>
    /// <note type="important">对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100</note>
    /// </remarks>
    /// <example>
    /// 你可以很快速并且简单的创建一个虚拟的s7服务器
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7ServerExample.cs" region="UseExample1" title="简单的创建服务器" />
    /// 当然如果需要高级的服务器，指定日志，限制客户端的IP地址，获取客户端发送的信息，在服务器初始化的时候就要参照下面的代码：
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7ServerExample.cs" region="UseExample4" title="定制服务器" />
    /// 服务器创建好之后，我们就可以对服务器进行一些读写的操作了，下面的代码是基础的BCL类型的读写操作。
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7ServerExample.cs" region="ReadWriteExample" title="基础的读写示例" />
    /// 高级的对于byte数组类型的数据进行批量化的读写操作如下：   
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7ServerExample.cs" region="BytesReadWrite" title="字节的读写示例" />
    /// 更高级操作请参见源代码。
    /// </example>
    public class SiemensS7Server : NetworkDataServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个S7协议的服务器，支持I，Q，M，DB1.X 数据区块的读写操作
        /// </summary>
        public SiemensS7Server( )
        {
            // 四个数据池初始化，输入寄存器，输出寄存器，中间寄存器，DB块寄存器
            inputBuffer             = new SoftBuffer( DataPoolLength );
            outputBuffer            = new SoftBuffer( DataPoolLength );
            memeryBuffer            = new SoftBuffer( DataPoolLength );
            dbBlockBuffer           = new SoftBuffer( DataPoolLength );
                                   
            WordLength              = 2;
            ByteTransform           = new ReverseBytesTransform( );
        }

        #endregion

        #region NetworkDataServerBase Override

        /// <summary>
        /// 读取自定义的寄存器的值
        /// </summary>
        /// <param name="address">起始地址，示例："I100"，"M100"</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>byte数组值</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            OperateResult<S7AddressData> analysis = S7AddressData.ParseFrom( address, length );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            switch (analysis.Content.DataCode)
            {
                case 0x81: return OperateResult.CreateSuccessResult( inputBuffer.GetBytes( analysis.Content.AddressStart / 8, length ) );
                case 0x82: return OperateResult.CreateSuccessResult( outputBuffer.GetBytes( analysis.Content.AddressStart / 8, length ) );
                case 0x83: return OperateResult.CreateSuccessResult( memeryBuffer.GetBytes( analysis.Content.AddressStart / 8, length ) );
                case 0x84: return OperateResult.CreateSuccessResult( dbBlockBuffer.GetBytes( analysis.Content.AddressStart / 8, length ) );
                default: return new OperateResult<byte[]>( StringResources.Language.NotSupportedDataType );
            }
        }

        /// <summary>
        /// 写入自定义的数据到数据内存中去
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功的结果对象</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            OperateResult<S7AddressData> analysis = S7AddressData.ParseFrom( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            switch (analysis.Content.DataCode)
            {
                case 0x81: inputBuffer.SetBytes( value, analysis.Content.AddressStart / 8 ); return OperateResult.CreateSuccessResult( );
                case 0x82: outputBuffer.SetBytes(value, analysis.Content.AddressStart / 8 ); return OperateResult.CreateSuccessResult( );
                case 0x83: memeryBuffer.SetBytes(value, analysis.Content.AddressStart / 8 ); return OperateResult.CreateSuccessResult( );
                case 0x84: dbBlockBuffer.SetBytes( value, analysis.Content.AddressStart / 8 ); return OperateResult.CreateSuccessResult( );
                default: return new OperateResult<byte[]>( StringResources.Language.NotSupportedDataType );
            }
        }

        #endregion

        #region Byte Read Write Operate

        /// <summary>
        /// 读取指定地址的字节数据
        /// </summary>
        /// <param name="address">西门子的地址信息</param>
        /// <returns>带有成功标志的结果对象</returns>
        public OperateResult<byte> ReadByte(string address )
        {
            OperateResult<byte[]> read = Read( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte>( read );

            return OperateResult.CreateSuccessResult( read.Content[0] );
        }

        /// <summary>
        /// 将byte数据信息写入到指定的地址当中
        /// </summary>
        /// <param name="address">西门子的地址信息</param>
        /// <param name="value">字节数据信息</param>
        /// <returns>是否成功的结果</returns>
        public OperateResult Write(string address, byte value )
        {
            return Write( address, new byte[] { value } );
        }

        #endregion

        #region Bool Read Write Operate

        /// <summary>
        /// 读取指定地址的bool数据对象
        /// </summary>
        /// <param name="address">西门子的地址信息</param>
        /// <returns>带有成功标志的结果对象</returns>
        public OperateResult<bool> ReadBool(string address )
        {
            OperateResult<S7AddressData> analysis = S7AddressData.ParseFrom( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<bool>( analysis );

            switch (analysis.Content.DataCode)
            {
                case 0x81: return OperateResult.CreateSuccessResult( inputBuffer.GetBool( analysis.Content.AddressStart ) );
                case 0x82: return OperateResult.CreateSuccessResult( outputBuffer.GetBool( analysis.Content.AddressStart ) );
                case 0x83: return OperateResult.CreateSuccessResult( memeryBuffer.GetBool( analysis.Content.AddressStart ) );
                case 0x84: return OperateResult.CreateSuccessResult( dbBlockBuffer.GetBool( analysis.Content.AddressStart ) );
                default: return new OperateResult<bool>( StringResources.Language.NotSupportedDataType );
            }
        }

        /// <summary>
        /// 往指定的地址里写入bool数据对象
        /// </summary>
        /// <param name="address">西门子的地址信息</param>
        /// <param name="value">值</param>
        /// <returns>是否成功的结果</returns>
        public OperateResult Write(string address, bool value )
        {
            OperateResult<S7AddressData> analysis = S7AddressData.ParseFrom( address );
            if (!analysis.IsSuccess) return analysis;

            switch (analysis.Content.DataCode)
            {
                case 0x81: inputBuffer.SetBool( value, analysis.Content.AddressStart ); return OperateResult.CreateSuccessResult( );
                case 0x82: outputBuffer.SetBool( value, analysis.Content.AddressStart ); return OperateResult.CreateSuccessResult( );
                case 0x83: memeryBuffer.SetBool( value, analysis.Content.AddressStart ); return OperateResult.CreateSuccessResult( );
                case 0x84: dbBlockBuffer.SetBool( value, analysis.Content.AddressStart ); return OperateResult.CreateSuccessResult( );
                default: return new OperateResult( StringResources.Language.NotSupportedDataType );
            }
        }

        #endregion

        #region NetServer Override

        /// <summary>
        /// 当客户端登录后，进行Ip信息的过滤，然后触发本方法，也就是说之后的客户端需要
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="endPoint">终端节点</param>
        protected override void ThreadPoolLoginAfterClientCheck( Socket socket, System.Net.IPEndPoint endPoint )
        {
            // 接收2次的握手协议
            S7Message s7Message = new S7Message( );
            OperateResult<byte[]> read1 = ReceiveByMessage( socket, 5000, s7Message );
            if (!read1.IsSuccess) return;

            OperateResult send1 = Send( socket, SoftBasic.HexStringToBytes( "03 00 00 16 11 D0 00 01 00 0C 00 C0 01 0A C1 02 01 02 C2 02 01 00" ) );
            if (!send1.IsSuccess) return;

            OperateResult<byte[]> read2 = ReceiveByMessage( socket, 5000, s7Message );
            if (!read1.IsSuccess) return;

            OperateResult send2 = Send( socket, SoftBasic.HexStringToBytes( "03 00 00 1B 02 F0 80 32 03 00 00 04 00 00 08 00 00 00 00 F0 00 00 01 00 01 00 F0" ) );
            if (!read2.IsSuccess) return;

            // 开始接收数据信息
            AppSession appSession = new AppSession( );
            appSession.IpEndPoint = endPoint;
            appSession.WorkSocket = socket;
            try
            {
                socket.BeginReceive( new byte[0], 0, 0, SocketFlags.None, new AsyncCallback( SocketAsyncCallBack ), appSession );
                AddClient( appSession );
            }
            catch
            {
                socket.Close( );
                LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, endPoint ) );
            }
        }
        
        private void SocketAsyncCallBack( IAsyncResult ar )
        {
            if (ar.AsyncState is AppSession session)
            {
                try
                {
                    int receiveCount = session.WorkSocket.EndReceive( ar );

                    S7Message s7Message = new S7Message( );
                    OperateResult<byte[]> read1 = ReceiveByMessage( session.WorkSocket, 5000, s7Message );
                    if (!read1.IsSuccess)
                    {
                        LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, session.IpEndPoint ) );
                        RemoveClient( session );
                        return;
                    };

                    byte[] receive = read1.Content;
                    
                    if (receive[17] == 0x04)
                    {
                        // 读数据
                        session.WorkSocket.Send( ReadByMessage( receive ) );
                    }
                    else if (receive[17] == 0x05)
                    {
                        // 写数据
                        session.WorkSocket.Send( WriteByMessage( receive ) );
                    }
                    else if(receive[17] == 0x00)
                    {
                        // 请求订货号
                        session.WorkSocket.Send( SoftBasic.HexStringToBytes( "03 00 00 7D 02 F0 80 32 07 00 00 00 01 00 0C 00 60 00 01 12 08 12 84 01 01 00 00 00 00 FF" +
                            " 09 00 5C 00 11 00 00 00 1C 00 03 00 01 36 45 53 37 20 32 31 35 2D 31 41 47 34 30 2D 30 58 42 30 20 00 00 00 06 20 20 00 06 36 45 53 37 20" +
                            " 32 31 35 2D 31 41 47 34 30 2D 30 58 42 30 20 00 00 00 06 20 20 00 07 36 45 53 37 20 32 31 35 2D 31 41 47 34 30 2D 30 58 42 30 20 00 00 56 04 02 01" ) );
                    }
                    else
                    {
                        session.WorkSocket.Close( );
                    }

                    RaiseDataReceived( receive );
                    session.WorkSocket.BeginReceive( new byte[0], 0, 0, SocketFlags.None, new AsyncCallback( SocketAsyncCallBack ), session );
                }
                catch
                {
                    // 关闭连接，记录日志
                    session.WorkSocket?.Close( );
                    LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, session.IpEndPoint ) );
                    RemoveClient( session );
                    return;
                }
            }
        }

        private byte[] ReadByMessage( byte[] packCommand )
        {
            List<byte> content = new List<byte>( );
            int count = packCommand[18];
            int index = 19;
            for(int i=0; i < count; i++)
            {
                byte length = packCommand[index + 1];
                byte[] command = ByteTransform.TransByte( packCommand, index, length + 2 );
                index += length + 2;

                content.AddRange( ReadByCommand( command ) );
            }

            byte[] back = new byte[21 + content.Count];
            SoftBasic.HexStringToBytes( "03 00 00 1A 02 F0 80 32 03 00 00 00 01 00 02 00 05 00 00 04 01" ).CopyTo( back, 0 );
            back[ 2] = (byte)(back.Length / 256);
            back[ 3] = (byte)(back.Length % 256);
            back[15] = (byte)(packCommand.Length / 256);
            back[16] = (byte)(packCommand.Length % 256);
            back[20] = packCommand[18];
            content.CopyTo( back, 21 );
            return back;
        }

        private byte[] ReadByCommand(byte[] command )
        {
            if(command[3] == 0x01)
            {
                // 位读取
                int startIndex = command[9] * 65536 + command[10] * 256 + command[11];
                switch (command[8])
                {
                    case 0x81: return PackReadBitCommandBack( inputBuffer.GetBool( startIndex ) );
                    case 0x82: return PackReadBitCommandBack( outputBuffer.GetBool( startIndex ) );
                    case 0x83: return PackReadBitCommandBack( memeryBuffer.GetBool( startIndex ) );
                    case 0x84: return PackReadBitCommandBack( dbBlockBuffer.GetBool( startIndex ) );
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
            else
            {
                // 字读取
                ushort length = ByteTransform.TransUInt16( command, 4 );
                int startIndex = (command[9] * 65536 + command[10] * 256 + command[11]) / 8;
                switch (command[8])
                {
                    case 0x81: return PackReadWordCommandBack( inputBuffer.GetBytes( startIndex, length ) );
                    case 0x82: return PackReadWordCommandBack( outputBuffer.GetBytes( startIndex, length ) );
                    case 0x83: return PackReadWordCommandBack( memeryBuffer.GetBytes( startIndex, length ) );
                    case 0x84: return PackReadWordCommandBack( dbBlockBuffer.GetBytes( startIndex, length ) );
                    default: throw new Exception( StringResources.Language.NotSupportedDataType ); 
                }
            }
        }

        private byte[] PackReadWordCommandBack( byte[] result )
        {
            byte[] back = new byte[4 + result.Length];
            back[0] = 0xFF;
            back[1] = 0x04;

            ByteTransform.TransByte( (ushort)result.Length ).CopyTo( back, 2 );
            result.CopyTo( back, 4 );
            return back;
        }

        private byte[] PackReadBitCommandBack( bool value )
        {
            byte[] back = new byte[5];
            back[0] = 0xFF;
            back[1] = 0x03;
            back[2] = 0x00;
            back[3] = 0x01;
            back[4] = (byte)(value ? 0x01 : 0x00);
            return back;
        }

        private byte[] WriteByMessage( byte[] packCommand )
        {
            if(packCommand[22] == 0x02)
            {
                // 字写入
                int count = ByteTransform.TransInt16( packCommand, 23 );
                int startIndex = (packCommand[28] * 65536 + packCommand[29] * 256 + packCommand[30]) / 8;
                byte[] data = ByteTransform.TransByte( packCommand, 35, count );
                switch (packCommand[27])
                {
                    case 0x81: inputBuffer.SetBytes( data, startIndex );break;
                    case 0x82: outputBuffer.SetBytes( data, startIndex ); break;
                    case 0x83: memeryBuffer.SetBytes( data, startIndex ); break;
                    case 0x84: dbBlockBuffer.SetBytes( data, startIndex ); break;
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
                }
                return SoftBasic.HexStringToBytes( "03 00 00 16 02 F0 80 32 03 00 00 00 01 00 02 00 01 00 00 05 01 FF" );
            }
            else
            {
                // 位写入
                int startIndex = packCommand[28] * 65536 + packCommand[29] * 256 + packCommand[30];
                bool value = packCommand[35] != 0x00;
                switch (packCommand[27])
                {
                    case 0x81: inputBuffer.SetBool( value, startIndex ); break;
                    case 0x82: outputBuffer.SetBool( value, startIndex ); break;
                    case 0x83: memeryBuffer.SetBool( value, startIndex ); break;
                    case 0x84: dbBlockBuffer.SetBool( value, startIndex ); break;
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
                }
                return SoftBasic.HexStringToBytes( "03 00 00 16 02 F0 80 32 03 00 00 00 01 00 02 00 01 00 00 05 01 FF" );
            }
        }

        #endregion

        #region Data Save Load Override

        /// <summary>
        /// 从字节数据加载数据信息
        /// </summary>
        /// <param name="content">字节数据</param>
        protected override  void LoadFromBytes( byte[] content )
        {
            if (content.Length < DataPoolLength * 4) throw new Exception( "File is not correct" );

            inputBuffer.SetBytes( content, 0, 0, DataPoolLength );
            outputBuffer.SetBytes( content, DataPoolLength, 0, DataPoolLength );
            memeryBuffer.SetBytes( content, DataPoolLength * 2, 0, DataPoolLength );
            dbBlockBuffer.SetBytes( content, DataPoolLength * 3, 0, DataPoolLength );
        }

        /// <summary>
        /// 将数据信息存储到字节数组去
        /// </summary>
        /// <returns>所有的内容</returns>
        protected override byte[] SaveToBytes( )
        {
            byte[] buffer = new byte[DataPoolLength * 4];
            Array.Copy( inputBuffer.GetBytes( ), 0, buffer, 0, DataPoolLength );
            Array.Copy( outputBuffer.GetBytes( ), 0, buffer, DataPoolLength, DataPoolLength );
            Array.Copy( memeryBuffer.GetBytes( ), 0, buffer, DataPoolLength * 2, DataPoolLength );
            Array.Copy( dbBlockBuffer.GetBytes( ), 0, buffer, DataPoolLength * 3, DataPoolLength);

            return buffer;
        }


        #endregion

        #region IDisposable Support

        /// <summary>
        /// 释放当前的对象
        /// </summary>
        /// <param name="disposing">是否托管对象</param>
        protected override void Dispose( bool disposing )
        {
            if (disposing)
            {
                inputBuffer?.Dispose( );
                outputBuffer?.Dispose( );
                memeryBuffer?.Dispose( );
                dbBlockBuffer?.Dispose( );
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Private Member

        private SoftBuffer inputBuffer;                // 输入寄存器的数据池
        private SoftBuffer outputBuffer;               // 离散输入的数据池
        private SoftBuffer memeryBuffer;               // 寄存器的数据池
        private SoftBuffer dbBlockBuffer;              // 输入寄存器的数据池
        private const int DataPoolLength = 65536;      // 数据的长度

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return $"SiemensS7Server[{Port}]";
        }

        #endregion
    }
}
