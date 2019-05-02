using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 三菱MC协议的虚拟服务器，支持M,X,Y,D,W的数据池读写操作，使用二进制进行读写操作
    /// </summary>
    public class MelsecMcServer : NetworkDataServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个mc协议的服务器
        /// </summary>
        public MelsecMcServer( )
        {
            // 共计使用了五个数据池
            xBuffer = new SoftBuffer( DataPoolLength );
            yBuffer = new SoftBuffer( DataPoolLength );
            mBuffer = new SoftBuffer( DataPoolLength );
            dBuffer = new SoftBuffer( DataPoolLength * 2 );
            wBuffer = new SoftBuffer( DataPoolLength * 2 );

            WordLength = 1;
            ByteTransform = new RegularByteTransform( );
        }

        #endregion

        #region NetworkDataServerBase Override

        /// <summary>
        /// 读取自定义的寄存器的值。按照字为单位
        /// </summary>
        /// <param name="address">起始地址，示例："D100"，"M100"</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>byte数组值</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            OperateResult<MelsecMcDataType, int> analysis = MelsecHelper.McAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            if(analysis.Content1.DataCode == MelsecMcDataType.M.DataCode)
            {
                bool[] buffer = mBuffer.GetBytes( analysis.Content2, length * 16 ).Select( m => m != 0x00 ).ToArray( );
                return OperateResult.CreateSuccessResult( SoftBasic.BoolArrayToByte( buffer ) );
            }
            else if(analysis.Content1.DataCode == MelsecMcDataType.X.DataCode)
            {
                bool[] buffer = xBuffer.GetBytes( analysis.Content2, length * 16 ).Select( m => m != 0x00 ).ToArray( );
                return OperateResult.CreateSuccessResult( SoftBasic.BoolArrayToByte( buffer ) );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.Y.DataCode)
            {
                bool[] buffer = yBuffer.GetBytes( analysis.Content2, length * 16 ).Select( m => m != 0x00 ).ToArray( );
                return OperateResult.CreateSuccessResult( SoftBasic.BoolArrayToByte( buffer ) );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.D.DataCode)
            {
                return OperateResult.CreateSuccessResult( dBuffer.GetBytes( analysis.Content2 * 2, length * 2 ) );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.W.DataCode)
            {
                return OperateResult.CreateSuccessResult( wBuffer.GetBytes( analysis.Content2 * 2, length * 2 ) );
            }
            else
            {
                return new OperateResult<byte[]>( StringResources.Language.NotSupportedDataType );
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
            OperateResult<MelsecMcDataType, int> analysis = MelsecHelper.McAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            if (analysis.Content1.DataCode == MelsecMcDataType.M.DataCode)
            {
                byte[] buffer = SoftBasic.ByteToBoolArray( value ).Select( m => m ? (byte)1 : (byte)0 ).ToArray( );
                mBuffer.SetBytes( buffer, analysis.Content2 );
                return OperateResult.CreateSuccessResult( );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.X.DataCode)
            {
                byte[] buffer = SoftBasic.ByteToBoolArray( value ).Select( m => m ? (byte)1 : (byte)0 ).ToArray( );
                xBuffer.SetBytes( buffer, analysis.Content2 );
                return OperateResult.CreateSuccessResult( );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.Y.DataCode)
            {
                byte[] buffer = SoftBasic.ByteToBoolArray( value ).Select( m => m ? (byte)1 : (byte)0 ).ToArray( );
                yBuffer.SetBytes( buffer, analysis.Content2 );
                return OperateResult.CreateSuccessResult( );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.D.DataCode)
            {
                dBuffer.SetBytes( value, analysis.Content2 * 2 );
                return OperateResult.CreateSuccessResult( );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.W.DataCode)
            {
                wBuffer.SetBytes( value, analysis.Content2 * 2 );
                return OperateResult.CreateSuccessResult( );
            }
            else
            {
                return new OperateResult<byte[]>( StringResources.Language.NotSupportedDataType );
            }
        }

        #endregion

        #region Bool Read Write Operate

        /// <summary>
        /// 读取指定地址的bool数据对象
        /// </summary>
        /// <param name="address">西门子的地址信息</param>
        /// <returns>带有成功标志的结果对象</returns>
        public OperateResult<bool> ReadBool( string address )
        {
            OperateResult<bool[]> read = ReadBool( address, 1 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<bool>( read );

            return OperateResult.CreateSuccessResult( read.Content[0] );
        }

        /// <summary>
        /// 读取指定地址的bool数据对象
        /// </summary>
        /// <param name="address">三菱的地址信息</param>
        /// <param name="length">数组的长度</param>
        /// <returns>带有成功标志的结果对象</returns>
        public OperateResult<bool[]> ReadBool( string address, ushort length )
        {
            OperateResult<MelsecMcDataType, int> analysis = MelsecHelper.McAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( analysis );

            if (analysis.Content1.DataType == 0) return new OperateResult<bool[]>( StringResources.Language.MelsecCurrentTypeNotSupportedWordOperate );

            if (analysis.Content1.DataCode == MelsecMcDataType.M.DataCode)
            {
                return OperateResult.CreateSuccessResult( mBuffer.GetBytes( analysis.Content2, length ).Select( m => m != 0x00 ).ToArray( ) );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.X.DataCode)
            {
                return OperateResult.CreateSuccessResult( xBuffer.GetBytes( analysis.Content2, length ).Select( m => m != 0x00 ).ToArray( ) );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.Y.DataCode)
            {
                return OperateResult.CreateSuccessResult( yBuffer.GetBytes( analysis.Content2, length ).Select( m => m != 0x00 ).ToArray( ) );
            }
            else
            {
                return new OperateResult<bool[]>( StringResources.Language.NotSupportedDataType );
            }
        }

        /// <summary>
        /// 往指定的地址里写入bool数据对象
        /// </summary>
        /// <param name="address">三菱的地址信息</param>
        /// <param name="value">值</param>
        /// <returns>是否成功的结果</returns>
        public OperateResult Write( string address, bool value )
        {
            return Write( address, new bool[] { value } );
        }

        /// <summary>
        /// 往指定的地址里写入bool数组对象
        /// </summary>
        /// <param name="address">三菱的地址信息</param>
        /// <param name="value">值</param>
        /// <returns>是否成功的结果</returns>
        public OperateResult Write( string address, bool[] value )
        {
            OperateResult<MelsecMcDataType, int> analysis = MelsecHelper.McAnalysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<bool[]>( analysis );

            if (analysis.Content1.DataType == 0) return new OperateResult<bool[]>( StringResources.Language.MelsecCurrentTypeNotSupportedWordOperate );

            if (analysis.Content1.DataCode == MelsecMcDataType.M.DataCode)
            {
                mBuffer.SetBytes( value.Select( m => m ? (byte)1 : (byte)0 ).ToArray( ), analysis.Content2 );
                return OperateResult.CreateSuccessResult( );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.X.DataCode)
            {
                xBuffer.SetBytes( value.Select( m => m ? (byte)1 : (byte)0 ).ToArray( ), analysis.Content2 );
                return OperateResult.CreateSuccessResult( );
            }
            else if (analysis.Content1.DataCode == MelsecMcDataType.Y.DataCode)
            {
                yBuffer.SetBytes( value.Select( m => m ? (byte)1 : (byte)0 ).ToArray( ), analysis.Content2 );
                return OperateResult.CreateSuccessResult( );
            }
            else
            {
                return new OperateResult<bool[]>( StringResources.Language.NotSupportedDataType );
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

                    MelsecQnA3EBinaryMessage mcMessage = new MelsecQnA3EBinaryMessage( );
                    OperateResult<byte[]> read1 = ReceiveByMessage( session.WorkSocket, 5000, mcMessage );
                    if (!read1.IsSuccess)
                    {
                        LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, session.IpEndPoint ) );
                        RemoveClient( session );
                        return;
                    };

                    byte[] receive = read1.Content;

                    if (receive[11] == 0x01 && receive[12] == 0x04)
                    {
                        // 读数据
                        session.WorkSocket.Send( PackCommand( ReadByCommand( SoftBasic.BytesArrayRemoveBegin( receive, 11 ) ) ) );
                    }
                    else if (receive[11] == 0x01 && receive[12] == 0x14)
                    {
                        // 写数据
                        session.WorkSocket.Send( PackCommand( WriteByMessage( SoftBasic.BytesArrayRemoveBegin( receive, 11 ) ) ) );
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

        private byte[] PackCommand( byte[] data )
        {
            byte[] back = new byte[11 + data.Length];
            SoftBasic.HexStringToBytes( "D0 00 00 FF FF 03 00 00 00 00 00" ).CopyTo( back, 0 );
            if (data.Length > 0) data.CopyTo( back, 11 );

            BitConverter.GetBytes( (short)(data.Length + 2) ).CopyTo( back, 7 );
            return back;
        }

        private byte[] ReadByCommand( byte[] command )
        {
            if (command[2] == 0x01)
            {
                // 位读取
                ushort length = ByteTransform.TransUInt16( command, 8 );
                int startIndex = (command[6] * 65536 + command[5] * 256 + command[4]);

                if (command[7] == MelsecMcDataType.M.DataCode)
                {
                    byte[] buffer = mBuffer.GetBytes( startIndex, length );
                    return MelsecHelper.TransBoolArrayToByteData( buffer );
                }
                else if (command[7] == MelsecMcDataType.X.DataCode)
                {
                    byte[] buffer = xBuffer.GetBytes( startIndex, length );
                    return MelsecHelper.TransBoolArrayToByteData( buffer );
                }
                else if (command[7] == MelsecMcDataType.Y.DataCode)
                {
                    byte[] buffer = yBuffer.GetBytes( startIndex, length );
                    return MelsecHelper.TransBoolArrayToByteData( buffer );
                }
                else
                {
                    throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
            else
            {
                // 字读取
                ushort length = ByteTransform.TransUInt16( command, 8 );
                int startIndex = (command[6] * 65536 + command[5] * 256 + command[4]);
                if (command[7] == MelsecMcDataType.M.DataCode)
                {
                    bool[] buffer = mBuffer.GetBytes( startIndex, length * 16 ).Select( m => m != 0x00 ).ToArray( );
                    return SoftBasic.BoolArrayToByte( buffer );
                }
                else if(command[7] == MelsecMcDataType.X.DataCode)
                {
                    bool[] buffer = xBuffer.GetBytes( startIndex, length * 16 ).Select( m => m != 0x00 ).ToArray( );
                    return SoftBasic.BoolArrayToByte( buffer );
                }
                else if (command[7] == MelsecMcDataType.Y.DataCode)
                {
                    bool[] buffer = yBuffer.GetBytes( startIndex, length * 16 ).Select( m => m != 0x00 ).ToArray( );
                    return SoftBasic.BoolArrayToByte( buffer );
                }
                else if (command[7] == MelsecMcDataType.D.DataCode)
                {
                    return dBuffer.GetBytes( startIndex * 2, length * 2 );
                }
                else if (command[7] == MelsecMcDataType.W.DataCode)
                {
                    return wBuffer.GetBytes( startIndex * 2, length * 2 );
                }
                else
                {
                    throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
        }


        private byte[] WriteByMessage( byte[] command )
        {
            if (command[2] == 0x01)
            {
                // 位写入
                ushort length = ByteTransform.TransUInt16( command, 8 );
                int startIndex = (command[6] * 65536 + command[5] * 256 + command[4]);

                if (command[7] == MelsecMcDataType.M.DataCode)
                {
                    byte[] buffer = MelsecMcNet.ExtractActualData( SoftBasic.BytesArrayRemoveBegin( command, 10 ), true ).Content;
                    mBuffer.SetBytes( buffer.Take( length ).ToArray( ), startIndex );
                    return new byte[0];
                }
                else if (command[7] == MelsecMcDataType.X.DataCode)
                {
                    byte[] buffer = MelsecMcNet.ExtractActualData( SoftBasic.BytesArrayRemoveBegin( command, 10 ), true ).Content;
                    xBuffer.SetBytes( buffer.Take( length ).ToArray( ), startIndex );
                    return new byte[0];
                }
                else if (command[7] == MelsecMcDataType.Y.DataCode)
                {
                    byte[] buffer = MelsecMcNet.ExtractActualData( SoftBasic.BytesArrayRemoveBegin( command, 10 ), true ).Content;
                    yBuffer.SetBytes( buffer.Take( length ).ToArray( ), startIndex );
                    return new byte[0];
                }
                else
                {
                    throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
            else
            {
                // 字写入
                ushort length = ByteTransform.TransUInt16( command, 8 );
                int startIndex = (command[6] * 65536 + command[5] * 256 + command[4]);

                if (command[7] == MelsecMcDataType.M.DataCode)
                {
                    byte[] buffer = SoftBasic.ByteToBoolArray( SoftBasic.BytesArrayRemoveBegin( command, 10 ) ).Select( m => m ? (byte)1 : (byte)0 ).ToArray( );
                    mBuffer.SetBytes( buffer, startIndex );
                    return new byte[0];
                }
                else if (command[7] == MelsecMcDataType.X.DataCode)
                {
                    byte[] buffer = SoftBasic.ByteToBoolArray( SoftBasic.BytesArrayRemoveBegin( command, 10 ) ).Select( m => m ? (byte)1 : (byte)0 ).ToArray( );
                    xBuffer.SetBytes( buffer, startIndex );
                    return new byte[0];
                }
                else if (command[7] == MelsecMcDataType.Y.DataCode)
                {
                    byte[] buffer = SoftBasic.ByteToBoolArray( SoftBasic.BytesArrayRemoveBegin( command, 10 ) ).Select( m => m ? (byte)1 : (byte)0 ).ToArray( );
                    yBuffer.SetBytes( buffer, startIndex );
                    return new byte[0];
                }
                else if (command[7] == MelsecMcDataType.D.DataCode)
                {
                    dBuffer.SetBytes( SoftBasic.BytesArrayRemoveBegin( command, 10 ), startIndex * 2 );
                    return new byte[0];
                }
                else if (command[7] == MelsecMcDataType.W.DataCode)
                {
                    wBuffer.SetBytes( SoftBasic.BytesArrayRemoveBegin( command, 10 ), startIndex * 2 );
                    return new byte[0];
                }
                else
                {
                    throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
        }

        #endregion

        #region Data Save Load Override

        /// <summary>
        /// 从字节数据加载数据信息
        /// </summary>
        /// <param name="content">字节数据</param>
        protected override void LoadFromBytes( byte[] content )
        {
            if (content.Length < DataPoolLength * 7) throw new Exception( "File is not correct" );

            mBuffer.SetBytes( content, 0, 0, DataPoolLength );
            xBuffer.SetBytes( content, DataPoolLength, 0, DataPoolLength );
            yBuffer.SetBytes( content, DataPoolLength * 2, 0, DataPoolLength );
            dBuffer.SetBytes( content, DataPoolLength * 3, 0, DataPoolLength );
            wBuffer.SetBytes( content, DataPoolLength * 5, 0, DataPoolLength );
        }

        /// <summary>
        /// 将数据信息存储到字节数组去
        /// </summary>
        /// <returns>所有的内容</returns>
        protected override byte[] SaveToBytes( )
        {
            byte[] buffer = new byte[DataPoolLength * 7];
            Array.Copy( mBuffer.GetBytes( ), 0, buffer, 0, DataPoolLength );
            Array.Copy( xBuffer.GetBytes( ), 0, buffer, DataPoolLength, DataPoolLength );
            Array.Copy( yBuffer.GetBytes( ), 0, buffer, DataPoolLength * 2, DataPoolLength );
            Array.Copy( dBuffer.GetBytes( ), 0, buffer, DataPoolLength * 3, DataPoolLength );
            Array.Copy( wBuffer.GetBytes( ), 0, buffer, DataPoolLength * 5, DataPoolLength );

            return buffer;
        }


        #endregion

        #region Private Member

        private SoftBuffer xBuffer;                    // x寄存器的数据池
        private SoftBuffer yBuffer;                    // y寄存器的数据池
        private SoftBuffer mBuffer;                    // m寄存器的数据池
        private SoftBuffer dBuffer;                    // d寄存器的数据池
        private SoftBuffer wBuffer;                    // w寄存器的数据池

        private const int DataPoolLength = 65536;      // 数据的长度

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString( )
        {
            return $"MelsecMcServer[{Port}]";
        }

        #endregion
    }
}
