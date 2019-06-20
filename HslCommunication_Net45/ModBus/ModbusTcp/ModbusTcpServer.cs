using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;
using HslCommunication;
using HslCommunication.Core.Net;
using System.Net.Sockets;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Address;
using HslCommunication.BasicFramework;

#if !NETSTANDARD2_0
using System.IO.Ports;
#endif

namespace HslCommunication.ModBus
{
    /// <summary>
    /// Modbus的虚拟服务器，同时支持Tcp和Rtu的机制，支持线圈，离散输入，寄存器和输入寄存器的读写操作，可以用来当做系统的数据交换池
    /// </summary>
    /// <remarks>
    /// 可以基于本类实现一个功能复杂的modbus服务器，在传统的.NET版本里，还支持modbus-rtu指令的收发，.NET Standard版本服务器不支持rtu操作。服务器支持的数据池如下：
    /// <list type="number">
    /// <item>线圈，功能码对应01，05，15</item>
    /// <item>离散输入，功能码对应02</item>
    /// <item>寄存器，功能码对应03，06，16</item>
    /// <item>输入寄存器，功能码对应04，输入寄存器在服务器端可以实现读写的操作</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// 读写的地址格式为富文本地址，具体请参照下面的示例代码。
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\Modbus\ModbusTcpServer.cs" region="ModbusTcpServerExample" title="ModbusTcpServer示例" />
    /// </example>
    public class ModbusTcpServer : NetworkDataServerBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个Modbus Tcp的服务器，支持数据读写操作
        /// </summary>
        public ModbusTcpServer( )
        {
            // 四个数据池初始化，线圈，输入线圈，寄存器，只读寄存器
            coilBuffer               = new SoftBuffer( DataPoolLength );
            inputBuffer              = new SoftBuffer( DataPoolLength );
            registerBuffer           = new SoftBuffer( DataPoolLength * 2 );
            inputRegisterBuffer      = new SoftBuffer( DataPoolLength * 2 );
            
            subscriptions            = new List<ModBusMonitorAddress>( );
            subcriptionHybirdLock    = new SimpleHybirdLock( );
            ByteTransform            = new ReverseWordTransform( );
            WordLength               = 1;

#if !NETSTANDARD2_0
            serialPort               = new SerialPort( );
#endif
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 获取或设置数据解析的格式，默认ABCD，可选BADC，CDAB，DCBA格式
        /// </summary>
        /// <remarks>
        /// 对于Int32,UInt32,float,double,Int64,UInt64类型来说，存在多地址的电脑情况，需要和服务器进行匹配
        /// </remarks>
        public DataFormat DataFormat
        {
            get { return ByteTransform.DataFormat; }
            set { ByteTransform.DataFormat = value; }
        }

        /// <summary>
        /// 字符串数据是否按照字来反转
        /// </summary>
        public bool IsStringReverse
        {
            get { return ((ReverseWordTransform)ByteTransform).IsStringReverse; }
            set { ((ReverseWordTransform)ByteTransform).IsStringReverse = value; }
        }

        /// <summary>
        /// 获取或设置服务器的站号信息，对于rtu模式，只有站号对了，才会反馈回数据信息。默认为1。
        /// </summary>
        public int Station
        {
            get { return station; }
            set { station = value; }
        }

        #endregion

        #region Data Persistence

        /// <summary>
        /// 将数据源的内容生成原始数据，等待缓存
        /// </summary>
        /// <returns>原始的数据内容</returns>
        protected override byte[] SaveToBytes( )
        {
            byte[] buffer = new byte[DataPoolLength * 6];
            Array.Copy( coilBuffer.GetBytes( ),           0, buffer, 0,                   DataPoolLength );
            Array.Copy( inputBuffer.GetBytes( ),          0, buffer, DataPoolLength,      DataPoolLength );
            Array.Copy( registerBuffer.GetBytes( ),       0, buffer, DataPoolLength * 2,  DataPoolLength * 2 );
            Array.Copy( inputRegisterBuffer.GetBytes( ),  0, buffer, DataPoolLength * 4,  DataPoolLength * 2 );
            return buffer;
        }

        /// <summary>
        /// 从原始的数据复原数据
        /// </summary>
        /// <param name="content">原始的数据</param>
        protected override void LoadFromBytes( byte[] content )
        {
            if (content.Length < DataPoolLength * 6) throw new Exception( "File is not correct" );

            coilBuffer.SetBytes(          content, 0,                  0, DataPoolLength );
            inputBuffer.SetBytes(         content, DataPoolLength,     0, DataPoolLength );
            registerBuffer.SetBytes(      content, DataPoolLength * 2, 0, DataPoolLength * 2 );
            inputRegisterBuffer.SetBytes( content, DataPoolLength * 4, 0, DataPoolLength * 2 );
        }

        #endregion

        #region Coil Read Write
        
        /// <summary>
        /// 读取地址的线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool ReadCoil( string address )
        {
            ushort add = ushort.Parse( address );
            return coilBuffer.GetByte( add ) != 0x00;
        }

        /// <summary>
        /// 批量读取地址的线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="length">读取长度</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool[] ReadCoil( string address, ushort length )
        {
            ushort add = ushort.Parse( address );
            return coilBuffer.GetBytes( add, length ).Select( m => m != 0x00 ).ToArray( );
        }

        /// <summary>
        /// 写入线圈的通断值
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteCoil( string address, bool data )
        {
            ushort add = ushort.Parse( address );
            coilBuffer.SetValue( (byte)(data ? 0x01 : 0x00), add );
        }

        /// <summary>
        /// 写入线圈数组的通断值
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteCoil( string address, bool[] data )
        {
            if (data == null) return;

            ushort add = ushort.Parse( address );
            coilBuffer.SetBytes( data.Select( m => (byte)(m ? 0x01 : 0x00) ).ToArray( ), add );
        }

        #endregion

        #region Discrete Read Write

        /// <summary>
        /// 读取地址的离散线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool ReadDiscrete( string address )
        {
            ushort add = ushort.Parse( address );
            return inputBuffer.GetByte( add ) != 0x00;
        }

        /// <summary>
        /// 批量读取地址的离散线圈的通断情况
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="length">读取长度</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool[] ReadDiscrete( string address, ushort length )
        {
            ushort add = ushort.Parse( address );
            return inputBuffer.GetBytes( add, length ).Select( m => m != 0x00 ).ToArray( );
        }

        /// <summary>
        /// 写入离散线圈的通断值
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteDiscrete( string address, bool data )
        {
            ushort add = ushort.Parse( address );
            inputBuffer.SetValue( (byte)(data ? 0x01 : 0x00), add );
        }

        /// <summary>
        /// 写入离散线圈数组的通断值
        /// </summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteDiscrete( string address, bool[] data )
        {
            if (data == null) return;

            ushort add = ushort.Parse( address );
            inputBuffer.SetBytes( data.Select( m => (byte)(m ? 0x01 : 0x00) ).ToArray( ), add );
        }

        #endregion

        #region NetworkDataServerBase Override

        /// <summary>
        /// 读取自定义的寄存器的值。按照字为单位
        /// </summary>
        /// <param name="address">起始地址，示例："100"，"x=4;100"</param>
        /// <param name="length">数据长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>byte数组值</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress( address, true, ModbusInfo.ReadRegister );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            if(analysis.Content.Function == ModbusInfo.ReadRegister)
            {
                return OperateResult.CreateSuccessResult( registerBuffer.GetBytes( analysis.Content.Address * 2, length * 2 ) );
            }
            else if(analysis.Content.Function == ModbusInfo.ReadInputRegister)
            {
                return OperateResult.CreateSuccessResult( inputRegisterBuffer.GetBytes( analysis.Content.Address * 2, length * 2 ) );
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
            OperateResult<ModbusAddress> analysis = ModbusInfo.AnalysisAddress( address, true, ModbusInfo.ReadRegister );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            if (analysis.Content.Function == ModbusInfo.ReadRegister)
            {
                registerBuffer.SetBytes( value, analysis.Content.Address * 2 );
                return OperateResult.CreateSuccessResult( );
            }
            else if (analysis.Content.Function == ModbusInfo.ReadInputRegister)
            {
                inputRegisterBuffer.SetBytes( value, analysis.Content.Address * 2 );
                return OperateResult.CreateSuccessResult( );
            }
            else
            {
                return new OperateResult<byte[]>( StringResources.Language.NotSupportedDataType );
            }
        }

        /// <summary>
        /// 写入寄存器数据，指定字节数据
        /// </summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="high">高位数据</param>
        /// <param name="low">地位数据</param>
        public void Write( string address, byte high, byte low )
        {
            Write( address, new byte[] { high, low } );
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

                    ModbusTcpMessage mdMessage = new ModbusTcpMessage( );
                    OperateResult<byte[]> read1 = ReceiveByMessage( session.WorkSocket, 5000, mdMessage );
                    if (!read1.IsSuccess)
                    {
                        LogNet?.WriteDebug( ToString( ), string.Format( StringResources.Language.ClientOfflineInfo, session.IpEndPoint ) );
                        RemoveClient( session );
                        return;
                    };

                    ushort id = (ushort)(read1.Content[0] * 256 + read1.Content[1]);
                    byte[] back = ModbusInfo.PackCommandToTcp( ReadFromModbusCore( SoftBasic.BytesArrayRemoveBegin( read1.Content, 6 ) ), id );
                    if (back != null)
                    {
                        session.WorkSocket.Send( back );
                    }
                    else
                    {
                        session.WorkSocket.Close( );
                        RemoveClient( session );
                        return;
                    }

                    RaiseDataReceived( read1.Content );
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

        #endregion

        #region Function Process Center

        /// <summary>
        /// 创建特殊的功能标识，然后返回该信息
        /// </summary>
        /// <param name="modbusCore">modbus核心报文</param>
        /// <param name="error">错误码</param>
        /// <returns>携带错误码的modbus报文</returns>
        private byte[] CreateExceptionBack( byte[] modbusCore, byte error )
        {
            byte[] buffer = new byte[3];
            buffer[0] = modbusCore[0];
            buffer[1] = (byte)(modbusCore[1] + 0x80);
            buffer[2] = error;
            return buffer;
        }

        /// <summary>
        /// 创建返回消息
        /// </summary>
        /// <param name="modbusCore">modbus核心报文</param>
        /// <param name="content">返回的实际数据内容</param>
        /// <returns>携带内容的modbus报文</returns>
        private byte[] CreateReadBack( byte[] modbusCore, byte[] content )
        {
            byte[] buffer = new byte[3 + content.Length];
            buffer[0] = modbusCore[0];
            buffer[1] = modbusCore[1];
            buffer[2] = (byte)content.Length;
            Array.Copy( content, 0, buffer, 3, content.Length );
            return buffer;
        }

        /// <summary>
        /// 创建写入成功的反馈信号
        /// </summary>
        /// <param name="modbus">modbus核心报文</param>
        /// <returns>携带成功写入的信息</returns>
        private byte[] CreateWriteBack( byte[] modbus )
        {
            byte[] buffer = new byte[6];
            Array.Copy( modbus, 0, buffer, 0, 6 );
            return buffer;
        }


        private byte[] ReadCoilBack( byte[] modbus )
        {
            try
            {
                ushort address = ByteTransform.TransUInt16( modbus, 2 );
                ushort length = ByteTransform.TransUInt16( modbus, 4 );

                // 越界检测
                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                // 地址长度检测
                if (length > 2040)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                bool[] read = ReadCoil( address.ToString( ), length );
                byte[] buffer = BasicFramework.SoftBasic.BoolArrayToByte( read );
                return CreateReadBack( modbus, buffer );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpReadCoilException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }

        private byte[] ReadDiscreteBack( byte[] modbus )
        {
            try
            {
                ushort address = ByteTransform.TransUInt16( modbus, 2 );
                ushort length = ByteTransform.TransUInt16( modbus, 4 );

                // 越界检测
                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                // 地址长度检测
                if (length > 2040)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                bool[] read = ReadDiscrete( address.ToString( ), length );
                byte[] buffer = BasicFramework.SoftBasic.BoolArrayToByte( read );
                return CreateReadBack( modbus, buffer );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpReadCoilException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }


        private byte[] ReadRegisterBack( byte[] modbus )
        {
            try
            {
                ushort address = ByteTransform.TransUInt16( modbus, 2 );
                ushort length = ByteTransform.TransUInt16( modbus, 4 );

                // 越界检测
                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                // 地址长度检测
                if (length > 127)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                byte[] buffer = Read( address.ToString( ), length ).Content;
                return CreateReadBack( modbus, buffer );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpReadRegisterException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }

        private byte[] ReadInputRegisterBack( byte[] modbus )
        {
            try
            {
                ushort address = ByteTransform.TransUInt16( modbus, 2 );
                ushort length = ByteTransform.TransUInt16( modbus, 4 );

                // 越界检测
                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                // 地址长度检测
                if (length > 127)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                byte[] buffer = Read( "x=4;" + address.ToString( ), length ).Content;
                return CreateReadBack( modbus, buffer );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpReadRegisterException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }

        private byte[] WriteOneCoilBack( byte[] modbus )
        {
            try
            {
                ushort address = ByteTransform.TransUInt16( modbus, 2 );

                if (modbus[4] == 0xFF && modbus[5] == 0x00)
                {
                    WriteCoil( address.ToString( ), true );
                }
                else if (modbus[4] == 0x00 && modbus[5] == 0x00)
                {
                    WriteCoil( address.ToString( ), false );
                }
                return CreateWriteBack( modbus );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpWriteCoilException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }



        private byte[] WriteOneRegisterBack( byte[] modbus )
        {
            try
            {
                ushort address = ByteTransform.TransUInt16( modbus, 2 );
                short ValueOld = ReadInt16( address.ToString( ) ).Content;
                // 写入到寄存器
                Write( address.ToString( ), modbus[4], modbus[5] );
                short ValueNew = ReadInt16( address.ToString( ) ).Content;
                // 触发写入请求
                OnRegisterBeforWrite( address, ValueOld, ValueNew );

                return CreateWriteBack( modbus );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpWriteRegisterException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }

        private byte[] WriteCoilsBack( byte[] modbus )
        {
            try
            {
                ushort address = ByteTransform.TransUInt16( modbus, 2 );
                ushort length = ByteTransform.TransUInt16( modbus, 4 );

                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                if (length > 2040)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                byte[] buffer = new byte[modbus.Length - 7];
                Array.Copy( modbus, 7, buffer, 0, buffer.Length );
                bool[] value = BasicFramework.SoftBasic.ByteToBoolArray( buffer, length );
                WriteCoil( address.ToString( ), value );
                return CreateWriteBack( modbus );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpWriteCoilException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }


        private byte[] WriteRegisterBack( byte[] modbus )
        {
            try
            {
                ushort address = ByteTransform.TransUInt16( modbus, 2 );
                ushort length = ByteTransform.TransUInt16( modbus, 4 );

                if ((address + length) > ushort.MaxValue + 1)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeOverBound );
                }

                if (length > 127)
                {
                    return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeQuantityOver );
                }

                byte[] buffer = new byte[modbus.Length - 7];

                // 为了使服务器的数据订阅更加的准确，决定将设计改为等待所有的数据写入完成后，再统一触发订阅，2018年3月4日 20:56:47
                MonitorAddress[] addresses = new MonitorAddress[length];
                for (ushort i = 0; i < length; i++)
                {
                    short ValueOld = ReadInt16( (address + i).ToString( ) ).Content;
                    Write( (address + i).ToString( ), modbus[2 * i + 7], modbus[2 * i + 8] );
                    short ValueNew = ReadInt16( (address + i).ToString( ) ).Content;
                    // 触发写入请求
                    addresses[i] = new MonitorAddress( )
                    {
                        Address = (ushort)(address + i),
                        ValueOrigin = ValueOld,
                        ValueNew = ValueNew
                    };
                }

                // 所有数据都更改完成后，再触发消息
                for (int i = 0; i < addresses.Length; i++)
                {
                    OnRegisterBeforWrite( addresses[i].Address, addresses[i].ValueOrigin, addresses[i].ValueNew );
                }

                return CreateWriteBack( modbus );
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), StringResources.Language.ModbusTcpWriteRegisterException, ex );
                return CreateExceptionBack( modbus, ModbusInfo.FunctionCodeReadWriteException );
            }
        }


        #endregion

        #region Subscription Support

        // 本服务器端支持指定地址的数据订阅器，目前仅支持寄存器操作

        private List<ModBusMonitorAddress> subscriptions;     // 数据订阅集合
        private SimpleHybirdLock subcriptionHybirdLock;       // 集合锁

        /// <summary>
        /// 新增一个数据监视的任务，针对的是寄存器
        /// </summary>
        /// <param name="monitor">监视地址对象</param>
        public void AddSubcription( ModBusMonitorAddress monitor )
        {
            subcriptionHybirdLock.Enter( );
            subscriptions.Add( monitor );
            subcriptionHybirdLock.Leave( );
        }

        /// <summary>
        /// 移除一个数据监视的任务
        /// </summary>
        /// <param name="monitor"></param>
        public void RemoveSubcrption( ModBusMonitorAddress monitor )
        {
            subcriptionHybirdLock.Enter( );
            subscriptions.Remove( monitor );
            subcriptionHybirdLock.Leave( );
        }

        /// <summary>
        /// 在数据变更后，进行触发是否产生订阅
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="before">修改之前的数</param>
        /// <param name="after">修改之后的数</param>
        private void OnRegisterBeforWrite( ushort address, short before, short after )
        {
            subcriptionHybirdLock.Enter( );
            for (int i = 0; i < subscriptions.Count; i++)
            {
                if (subscriptions[i].Address == address)
                {
                    subscriptions[i].SetValue( after );
                    if (before != after)
                    {
                        subscriptions[i].SetChangeValue( before, after );
                    }
                }
            }
            subcriptionHybirdLock.Leave( );
        }

        #endregion

        #region Modbus Core Logic

        /// <summary>
        /// 检测当前的Modbus接收的指定是否是合法的
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <returns>是否合格</returns>
        private bool CheckModbusMessageLegal( byte[] buffer )
        {
            try
            {
                if (buffer[1] == ModbusInfo.ReadCoil ||
                    buffer[1] == ModbusInfo.ReadDiscrete ||
                    buffer[1] == ModbusInfo.ReadRegister ||
                    buffer[1] == ModbusInfo.ReadInputRegister ||
                    buffer[1] == ModbusInfo.WriteOneCoil ||
                    buffer[1] == ModbusInfo.WriteOneRegister)
                {
                    if (buffer.Length != 0x06)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (
                    buffer[1] == ModbusInfo.WriteCoil ||
                    buffer[1] == ModbusInfo.WriteRegister)
                {
                    if (buffer.Length < 7)
                    {
                        return false;
                    }
                    else
                    {
                        if (buffer[6] == (buffer.Length - 7))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogNet?.WriteException( ToString( ), ex );
                return false;
            }
        }

        /// <summary>
        /// Modbus核心数据交互方法，允许重写自己来实现，报文只剩下核心的Modbus信息，去除了MPAB报头信息
        /// </summary>
        /// <param name="modbusCore">核心的Modbus报文</param>
        /// <returns>进行数据交互之后的结果</returns>
        protected virtual byte[] ReadFromModbusCore( byte[] modbusCore )
        {
            byte[] buffer = null;

            switch (modbusCore[1])
            {
                case ModbusInfo.ReadCoil:
                    {
                        buffer = ReadCoilBack( modbusCore ); break;
                    }
                case ModbusInfo.ReadDiscrete:
                    {
                        buffer = ReadDiscreteBack( modbusCore ); break;
                    }
                case ModbusInfo.ReadRegister:
                    {
                        buffer = ReadRegisterBack( modbusCore ); break;
                    }
                case ModbusInfo.ReadInputRegister:
                    {
                        buffer = ReadInputRegisterBack( modbusCore ); break;
                    }
                case ModbusInfo.WriteOneCoil:
                    {
                        buffer = WriteOneCoilBack( modbusCore ); break;
                    }
                case ModbusInfo.WriteOneRegister:
                    {
                        buffer = WriteOneRegisterBack( modbusCore ); break;
                    }
                case ModbusInfo.WriteCoil:
                    {
                        buffer = WriteCoilsBack( modbusCore ); break;
                    }
                case ModbusInfo.WriteRegister:
                    {
                        buffer = WriteRegisterBack( modbusCore ); break;
                    }
                default:
                    {
                        buffer = CreateExceptionBack( modbusCore, ModbusInfo.FunctionCodeNotSupport ); break;
                    }
            }

            return buffer;
        }

        #endregion

        #region Serial Support

#if !NETSTANDARD2_0

        private SerialPort serialPort;            // 核心的串口对象

        /// <summary>
        /// 使用默认的参数进行初始化串口，9600波特率，8位数据位，无奇偶校验，1位停止位
        /// </summary>
        /// <param name="com">串口信息</param>
        public void StartSerialPort( string com )
        {
            StartSerialPort( com, 9600 );
        }

        /// <summary>
        /// 使用默认的参数进行初始化串口，8位数据位，无奇偶校验，1位停止位
        /// </summary>
        /// <param name="com">串口信息</param>
        /// <param name="baudRate">波特率</param>
        public void StartSerialPort( string com, int baudRate )
        {
            StartSerialPort( sp =>
            {
                sp.PortName = com;
                sp.BaudRate = baudRate;
                sp.DataBits = 8;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;
            } );
        }

        /// <summary>
        /// 使用自定义的初始化方法初始化串口的参数
        /// </summary>
        /// <param name="inni">初始化信息的委托</param>
        public void StartSerialPort( Action<SerialPort> inni )
        {
            if (!serialPort.IsOpen)
            {
                inni?.Invoke( serialPort );

                serialPort.ReadBufferSize = 1024;
                serialPort.ReceivedBytesThreshold = 1;
                serialPort.Open( );
                serialPort.DataReceived += SerialPort_DataReceived;
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void CloseSerialPort( )
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close( );
            }
        }

        /// <summary>
        /// 接收到串口数据的时候触发
        /// </summary>
        /// <param name="sender">串口对象</param>
        /// <param name="e">消息</param>
        private void SerialPort_DataReceived( object sender, SerialDataReceivedEventArgs e )
        {
            int rCount = 0;
            byte[] buffer = new byte[1024];
            byte[] receive = null;

            while(true)
            {
                System.Threading.Thread.Sleep( 20 );            // 此处做个微小的延时，等待数据接收完成
                int count = serialPort.Read( buffer, rCount, serialPort.BytesToRead );
                rCount += count;
                if(count == 0) break;

                receive = new byte[rCount];
                Array.Copy( buffer, 0, receive, 0, count );
            }

            if(receive == null) return;
            
            if (receive.Length < 3)
            {
                LogNet?.WriteError( ToString( ), $"Uknown Data：" + SoftBasic.ByteToHexString( receive, ' ' ) );
                return;
            }

            if (Serial.SoftCRC16.CheckCRC16( receive ))
            {
                byte[] modbusCore = SoftBasic.BytesArrayRemoveLast( receive, 2 );

                if (!CheckModbusMessageLegal( modbusCore ))
                {
                    // 指令长度验证错误，关闭网络连接
                    LogNet?.WriteError( ToString( ), $"Receive Nosense Modbus-rtu : " + SoftBasic.ByteToHexString( receive, ' ' ) );
                    return;
                }

                // 验证站号是否一致
                if(station >= 0 && station != modbusCore[0])
                {
                    LogNet?.WriteError( ToString( ), $"Station not match Modbus-rtu : " + SoftBasic.ByteToHexString( receive, ' ' ) );
                    return;
                }

                // LogNet?.WriteError( ToString( ), $"Success：" + BasicFramework.SoftBasic.ByteToHexString( receive, ' ' ) );
                // 需要回发消息
                byte[] copy = ModbusInfo.PackCommandToRtu( ReadFromModbusCore( modbusCore ) );

                serialPort.Write( copy, 0, copy.Length );

                if (IsStarted) RaiseDataReceived( receive );
            }
            else
            {
                LogNet?.WriteWarn( "CRC Check Failed : " + SoftBasic.ByteToHexString( receive, ' ' ) );
            }
        }

#endif

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
                subcriptionHybirdLock?.Dispose( );
                subscriptions?.Clear( );
                coilBuffer?.Dispose( );
                inputBuffer?.Dispose( );
                registerBuffer?.Dispose( );
                inputRegisterBuffer?.Dispose( );
#if !NETSTANDARD2_0
                serialPort?.Dispose( );
#endif
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Private Member

        private SoftBuffer coilBuffer;                // 线圈的数据池
        private SoftBuffer inputBuffer;               // 离散输入的数据池
        private SoftBuffer registerBuffer;            // 寄存器的数据池
        private SoftBuffer inputRegisterBuffer;       // 输入寄存器的数据池

        private const int DataPoolLength = 65536;     // 数据的长度
        private int station = 1;                      // 服务器的站号数据，对于tcp无效，对于rtu来说，如果小于0，则忽略站号信息

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return $"ModbusTcpServer[{Port}]";
        }

        #endregion
    }
}
