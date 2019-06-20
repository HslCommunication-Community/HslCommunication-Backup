using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using HslCommunication.Core;
using HslCommunication.LogNet;

namespace HslCommunication.Serial
{
    /// <summary>
    /// 所有串行通信类的基类，提供了一些基础的服务
    /// </summary>
    public class SerialBase : IDisposable
    {
        #region Constructor

        /// <summary>
        /// 实例化一个无参的构造方法
        /// </summary>
        public SerialBase( )
        {
            SP_ReadData = new SerialPort( );
            hybirdLock = new SimpleHybirdLock( );
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 初始化串口信息，9600波特率，8位数据位，1位停止位，无奇偶校验
        /// </summary>
        /// <param name="portName">端口号信息，例如"COM3"</param>
        public void SerialPortInni( string portName )
        {
            SerialPortInni( portName, 9600 );
        }

        /// <summary>
        /// 初始化串口信息，波特率，8位数据位，1位停止位，无奇偶校验
        /// </summary>
        /// <param name="portName">端口号信息，例如"COM3"</param>
        /// <param name="baudRate">波特率</param>
        public void SerialPortInni( string portName, int baudRate )
        {
            SerialPortInni( portName, baudRate, 8, StopBits.One, Parity.None );
        }

        /// <summary>
        /// 初始化串口信息，波特率，数据位，停止位，奇偶校验需要全部自己来指定
        /// </summary>
        /// <param name="portName">端口号信息，例如"COM3"</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">奇偶校验</param>
        public void SerialPortInni( string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity )
        {
            if (SP_ReadData.IsOpen)
            {
                return;
            }
            SP_ReadData.PortName     = portName;    // 串口
            SP_ReadData.BaudRate     = baudRate;    // 波特率
            SP_ReadData.DataBits     = dataBits;    // 数据位
            SP_ReadData.StopBits     = stopBits;    // 停止位
            SP_ReadData.Parity       = parity;      // 奇偶校验
            PortName                 = SP_ReadData.PortName;
            BaudRate                 = SP_ReadData.BaudRate;
        }

        /// <summary>
        /// 根据自定义初始化方法进行初始化串口信息
        /// </summary>
        /// <param name="initi">初始化的委托方法</param>
        public void SerialPortInni( Action<SerialPort> initi )
        {
            if (SP_ReadData.IsOpen)
            {
                return;
            }
            SP_ReadData.PortName      = "COM5";
            SP_ReadData.BaudRate      = 9600;
            SP_ReadData.DataBits      = 8;
            SP_ReadData.StopBits      = StopBits.One;
            SP_ReadData.Parity        = Parity.None;

            initi.Invoke( SP_ReadData );

            PortName                  = SP_ReadData.PortName;
            BaudRate                  = SP_ReadData.BaudRate;
        }

        /// <summary>
        /// 打开一个新的串行端口连接
        /// </summary>
        public void Open( )
        {
            if (!SP_ReadData.IsOpen)
            {
                SP_ReadData.Open( );
                InitializationOnOpen( );
            }
        }

        /// <summary>
        /// 获取一个值，指示串口是否处于打开状态
        /// </summary>
        /// <returns>是或否</returns>
        public bool IsOpen( )
        {
            return SP_ReadData.IsOpen;
        }

        /// <summary>
        /// 关闭端口连接
        /// </summary>
        public void Close( )
        {
            if(SP_ReadData.IsOpen)
            {
                ExtraOnClose( );
                SP_ReadData.Close( );
            }
        }

        /// <summary>
        /// 读取串口的数据
        /// </summary>
        /// <param name="send">发送的原始字节数据</param>
        /// <returns>带接收字节的结果对象</returns>
        public OperateResult<byte[]> ReadBase(byte[] send)
        {
            hybirdLock.Enter( );

            if (IsClearCacheBeforeRead) ClearSerialCache( );

            OperateResult sendResult = SPSend( SP_ReadData, send );
            if (!sendResult.IsSuccess)
            {
                hybirdLock.Leave( );
                return OperateResult.CreateFailedResult<byte[]>( sendResult );
            }

            OperateResult<byte[]> receiveResult = SPReceived( SP_ReadData, true );
            hybirdLock.Leave( );

            return receiveResult;
        }

        /// <summary>
        /// 清除串口缓冲区的数据，并返回该数据，如果缓冲区没有数据，返回的字节数组长度为0
        /// </summary>
        /// <returns>是否操作成功的方法</returns>
        public OperateResult<byte[]> ClearSerialCache( )
        {
            return SPReceived( SP_ReadData, false );
        }

        #endregion

        #region virtual Method

        /// <summary>
        /// 检查当前接收的字节数据是否正确的
        /// </summary>
        /// <param name="rBytes">输入字节</param>
        /// <returns>检查是否正确</returns>
        protected virtual bool CheckReceiveBytes(byte[] rBytes )
        {
            return true;
        }

        #endregion

        #region Initialization And Extra

        /// <summary>
        /// 在打开端口时的初始化方法，按照协议的需求进行必要的重写
        /// </summary>
        /// <returns>是否初始化成功</returns>
        protected virtual OperateResult InitializationOnOpen( )
        {
            return OperateResult.CreateSuccessResult( );
        }

        /// <summary>
        /// 在将要和服务器进行断开的情况下额外的操作，需要根据对应协议进行重写
        /// </summary>
        /// <returns>当断开连接时额外的操作结果</returns>
        protected virtual OperateResult ExtraOnClose( )
        {
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Private Method
        
        /// <summary>
        /// 发送数据到串口里去
        /// </summary>
        /// <param name="serialPort">串口对象</param>
        /// <param name="data">字节数据</param>
        /// <returns>是否发送成功</returns>
        protected virtual OperateResult SPSend( SerialPort serialPort, byte[] data )
        {
            if (data != null && data.Length > 0)
            {
                if (!Authorization.nzugaydgwadawdibbas( )) return new OperateResult<byte[]>( StringResources.Language.AuthorizationFailed );

                try
                {
                    serialPort.Write( data, 0, data.Length );
                    return OperateResult.CreateSuccessResult( );
                }
                catch(Exception ex)
                {
                    return new OperateResult( ex.Message );
                }
            }
            else
            {
                return OperateResult.CreateSuccessResult( );
            }
        }

        /// <summary>
        /// 从串口接收一串数据信息，可以指定是否一定要接收到数据
        /// </summary>
        /// <param name="serialPort">串口对象</param>
        /// <param name="awaitData">是否必须要等待数据返回</param>
        /// <returns>结果数据对象</returns>
        protected virtual OperateResult<byte[]> SPReceived( SerialPort serialPort, bool awaitData )
        {
            if (!Authorization.nzugaydgwadawdibbas( )) return new OperateResult<byte[]>( StringResources.Language.AuthorizationFailed );

            byte[] buffer = new byte[1024];
            System.IO.MemoryStream ms = new System.IO.MemoryStream( );
            DateTime start = DateTime.Now;                                  // 开始时间，用于确认是否超时的信息
            while (true)
            {
                Thread.Sleep( sleepTime );
                try
                {
                    if (serialPort.BytesToRead < 1)
                    {
                        if ((DateTime.Now - start).TotalMilliseconds > ReceiveTimeout)
                        {
                            ms.Dispose( );
                            return new OperateResult<byte[]>( $"Time out: {ReceiveTimeout}" );
                        }
                        else if (ms.Length > 0)
                        {
                            break;
                        }
                        else if (awaitData)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // 继续接收数据
                    int sp_receive = serialPort.Read( buffer, 0, buffer.Length );
                    ms.Write( buffer, 0, sp_receive );
                }
                catch (Exception ex)
                {
                    ms.Dispose( );
                    return new OperateResult<byte[]>( ex.Message );
                }
            }

            // resetEvent.Set( );
            byte[] result = ms.ToArray( );
            ms.Dispose( );
            return OperateResult.CreateSuccessResult( result );
        }
        
        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return "SerialBase";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 当前的日志情况
        /// </summary>
        public ILogNet LogNet
        {
            get { return logNet; }
            set { logNet = value; }
        }

        /// <summary>
        /// 接收数据的超时时间，默认5000ms
        /// </summary>
        public int ReceiveTimeout
        {
            get { return receiveTimeout; }
            set { receiveTimeout = value; }
        }

        /// <summary>
        /// 连续串口缓冲数据检测的间隔时间，默认20ms
        /// </summary>
        public int SleepTime
        {
            get { return sleepTime; }
            set { if (value > 0) sleepTime = value; }
        }

        /// <summary>
        /// 是否在发送数据前清空缓冲数据，默认是false
        /// </summary>
        public bool IsClearCacheBeforeRead
        {
            get { return isClearCacheBeforeRead; }
            set { isClearCacheBeforeRead = value; }
        }

        /// <summary>
        /// 本连接对象的端口号名称
        /// </summary>
        public string PortName { get; private set; }

        /// <summary>
        /// 本连接对象的波特率
        /// </summary>
        public int BaudRate { get; private set; }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 释放当前的对象
        /// </summary>
        /// <param name="disposing">是否在</param>
        protected virtual void Dispose( bool disposing )
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    hybirdLock?.Dispose( );
                    SP_ReadData?.Dispose( );
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~SerialBase()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        /// <summary>
        /// 释放当前的对象
        /// </summary>
        public void Dispose( )
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose( true );
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion


        #region Private Member

        private SerialPort SP_ReadData = null;                    // 串口交互的核心
        private SimpleHybirdLock hybirdLock;                      // 数据交互的锁
        private ILogNet logNet;                                   // 日志存储
        private int receiveTimeout = 5000;                        // 接收数据的超时时间
        private int sleepTime = 20;                               // 睡眠的时间
        private bool isClearCacheBeforeRead = false;              // 是否在发送前清除缓冲

        #endregion
    }
}
