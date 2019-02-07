using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Robot.EFORT
{
    /// <summary>
    /// 埃夫特机器人对应型号为ER7B-C10，此协议为旧版的定制版，使用前请测试
    /// </summary>
    public class ER7BC10Previous : NetworkDoubleBase<EFORTMessagePrevious, RegularByteTransform>, IRobotNet
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的对象，并指定IP地址和端口号，端口号通常为8008
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public ER7BC10Previous( string ipAddress, int port )
        {
            IpAddress = ipAddress;
            Port = port;

            hybirdLock = new SimpleHybirdLock( );                                 // 实例化一个数据锁
        }

        #endregion

        #region Request Create

        /// <summary>
        /// 获取发送的消息的命令
        /// </summary>
        /// <returns>字节数组命令</returns>
        public byte[] GetReadCommand( )
        {
            byte[] command = new byte[36];

            Encoding.ASCII.GetBytes( "MessageHead" ).CopyTo( command, 0 );
            BitConverter.GetBytes( (ushort)command.Length ).CopyTo( command, 15 );
            BitConverter.GetBytes( (ushort)1001 ).CopyTo( command, 17 );
            BitConverter.GetBytes( GetHeartBeat( ) ).CopyTo( command, 19 );
            Encoding.ASCII.GetBytes( "MessageTail" ).CopyTo( command, 21 );

            return command;
        }

        private ushort GetHeartBeat( )
        {
            ushort result = 0;
            hybirdLock.Enter( );

            result = (ushort)heartbeat;
            heartbeat++;
            if (heartbeat > ushort.MaxValue)
            {
                heartbeat = 0;
            }

            hybirdLock.Leave( );
            return result;
        }

        #endregion

        #region Read Support

        /// <summary>
        /// 读取机器的详细信息，返回最原始的数据
        /// </summary>
        /// <returns>结果数据对象</returns>
        public OperateResult<byte[]> ReadBytes( )
        {
            return ReadFromCoreServer( GetReadCommand( ) );
        }

        /// <summary>
        /// 读取机器人的详细信息
        /// </summary>
        /// <returns>结果数据信息</returns>
        public OperateResult<EfortData> Read( )
        {
            OperateResult<byte[]> read = ReadBytes( );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<EfortData>( read );

            return EfortData.PraseFromPrevious( read.Content );
        }


        /// <summary>
        /// 读取机器人的详细信息，返回JSON格式的字符串信息
        /// </summary>
        /// <returns>结果数据信息</returns>
        public OperateResult<string> ReadJsonString( )
        {
            OperateResult<EfortData> read = Read( );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<string>( read );

            return OperateResult.CreateSuccessResult( Newtonsoft.Json.JsonConvert.SerializeObject( read.Content ) );
        }


        #endregion

        #region Private Member

        private int heartbeat = 0;
        private SimpleHybirdLock hybirdLock;             // 心跳值的锁

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return $"ER7BC10 Pre Robot[{IpAddress}:{Port}]";
        }

        #endregion
    }
}
