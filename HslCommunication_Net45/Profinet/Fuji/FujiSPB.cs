using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Fuji
{
    /// <summary>
    /// 富士PLC的SPB协议
    /// </summary>
    public class FujiSPB : SerialDeviceBase<RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 使用默认的构造方法实例化对象
        /// </summary>
        public FujiSPB( )
        {
            WordLength = 1;
        }

        #endregion

        #region Public Member

        /// <summary>
        /// PLC的站号信息
        /// </summary>
        public byte Station { get => station; set => station = value; }

        #endregion

        #region Read Write Support

        /// <summary>
        /// 批量读取PLC的数据，以字为单位，支持读取X,Y,L,M,D,TN,CN,TC,CC,R具体的地址范围需要根据PLC型号来确认
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>读取结果信息</returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // 解析指令
            OperateResult<byte[]> command = BuildReadCommand( this.station, address, length, false );
            if (!command.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( command );

            // 核心交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // 结果验证
            if (read.Content[0] != ':') return new OperateResult<byte[]>( read.Content[0], "Read Faild:" + BasicFramework.SoftBasic.ByteToHexString( read.Content, ' ' ) );
            if (Encoding.ASCII.GetString(read.Content, 9, 2) != "00") return new OperateResult<byte[]>( read.Content[5], GetErrorDescriptionFromCode( Encoding.ASCII.GetString( read.Content, 9, 2 ) ) );

            // 提取结果
            byte[] Content = new byte[length * 2];
            for (int i = 0; i < Content.Length / 2; i++)
            {
                ushort tmp = Convert.ToUInt16( Encoding.ASCII.GetString( read.Content, i * 4 + 6, 4 ), 16 );
                BitConverter.GetBytes( tmp ).CopyTo( Content, i * 2 );
            }
            return OperateResult.CreateSuccessResult( Content );
        }

        /// <summary>
        /// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持读取X,Y,L,M,D,TN,CN,TC,CC,R具体的地址范围需要根据PLC型号来确认
        /// </summary>
        /// <param name="address">地址信息，举例，D100，R200，RC100，RT200</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功</returns>
        public override OperateResult Write( string address, byte[] value )
        {
            // 解析指令
            OperateResult<byte[]> command = BuildWriteByteCommand( this.station, address, value );
            if (!command.IsSuccess) return command;

            // 核心交互
            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return read;

            // 结果验证
            if (read.Content[0] != ':') return new OperateResult<byte[]>( read.Content[0], "Read Faild:" + BasicFramework.SoftBasic.ByteToHexString( read.Content, ' ' ) );
            if (Encoding.ASCII.GetString( read.Content, 9, 2 ) != "00") return new OperateResult<byte[]>( read.Content[5], GetErrorDescriptionFromCode( Encoding.ASCII.GetString( read.Content, 9, 2 ) ) );

            // 提取结果
            return OperateResult.CreateSuccessResult( );
        }

        #endregion

        #region Private Member

        private byte station = 0x01;                 // PLC的站号信息

        #endregion

        #region Object Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return $"FujiSPB[{PortName}:{BaudRate}]";
        }

        #endregion

        #region Static Helper

        private static string AnalysisIntegerAddress( int address )
        {
            string tmp = address.ToString( "D4" );
            return tmp.Substring( 2 ) + tmp.Substring( 0, 2 );
        }

        /// <summary>
        /// 解析数据地址成不同的三菱地址类型
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>地址结果对象</returns>
        private static OperateResult<string> FujikAnalysisAddress( string address )
        {
            var result = new OperateResult<string>( );
            try
            {
                switch (address[0])
                {
                    case 'X':
                    case 'x':
                        {
                            result.Content = "01" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            result.Content = "00" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                            break;
                        }
                    case 'M':
                    case 'm':
                        {
                            result.Content = "02" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                            break;
                        }
                    case 'L':
                    case 'l':
                        {
                            result.Content = "03" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                            break;
                        }
                    case 'T':
                    case 't':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                result.Content = "0A" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                                break;
                            }
                            else if (address[1] == 'C' || address[1] == 'c')
                            {
                                result.Content = "04" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    case 'C':
                    case 'c':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                result.Content = "0B" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                                break;
                            }
                            else if (address[1] == 'C' || address[1] == 'c')
                            {
                                result.Content = "05" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    case 'D':
                    case 'd':
                        {
                            result.Content = "0C" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                            break;
                        }
                    case 'R':
                    case 'r':
                        {
                            result.Content = "0D" + AnalysisIntegerAddress( Convert.ToUInt16( address.Substring( 1 ), 10 ) );
                            break;
                        }
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 计算指令的和校验码
        /// </summary>
        /// <param name="data">指令</param>
        /// <returns>校验之后的信息</returns>
        public static string CalculateAcc( string data )
        {
            byte[] buffer = Encoding.ASCII.GetBytes( data );

            int count = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                count += buffer[i];
            }

            return count.ToString( "X4" ).Substring( 2 );
        }

        /// <summary>
        /// 创建一条读取的指令信息，需要指定一些参数
        /// </summary>
        /// <param name="station">PLCd的站号</param>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <param name="isBool">是否位读取</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildReadCommand( byte station, string address, ushort length, bool isBool )
        {
            OperateResult<string> addressAnalysis = FujikAnalysisAddress( address );
            if (!addressAnalysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressAnalysis );

            StringBuilder stringBuilder = new StringBuilder( );
            stringBuilder.Append( ':' );
            stringBuilder.Append( station.ToString( "X2" ) );
            stringBuilder.Append( "09" );
            stringBuilder.Append( "FFFF" );
            stringBuilder.Append( "00" );
            stringBuilder.Append( "00" );
            stringBuilder.Append( addressAnalysis.Content );
            stringBuilder.Append( length.ToString( "D4" ) );
            stringBuilder.Append( "\r\n" );
            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetBytes( stringBuilder.ToString( ) ) );
        }

        /// <summary>
        /// 创建一条别入byte数据的指令信息，需要指定一些参数，按照字单位
        /// </summary>
        /// <param name="station">站号</param>
        /// <param name="address">地址</param>
        /// <param name="value">数组值</param>
        /// <returns>是否创建成功</returns>
        public static OperateResult<byte[]> BuildWriteByteCommand( byte station, string address, byte[] value )
        {
            OperateResult<string> addressAnalysis = FujikAnalysisAddress( address );
            if (!addressAnalysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( addressAnalysis );

            StringBuilder stringBuilder = new StringBuilder( );
            stringBuilder.Append( ':' );
            stringBuilder.Append( station.ToString( "X2" ) );
            stringBuilder.Append( (9 + value.Length / 2).ToString( "X2" ) );
            stringBuilder.Append( "FFFF" );
            stringBuilder.Append( "01" );
            stringBuilder.Append( "00" );
            stringBuilder.Append( addressAnalysis.Content );
            stringBuilder.Append( (value.Length / 2).ToString( "D4" ) );

            byte[] buffer = new byte[value.Length * 2];
            for (int i = 0; i < value.Length / 2; i++)
            {
                SoftBasic.BuildAsciiBytesFrom( BitConverter.ToUInt16( value, i * 2 ) ).CopyTo( buffer, 4 * i );
            }
            stringBuilder.Append( Encoding.ASCII.GetString( buffer ) );
            stringBuilder.Append( "\r\n" );
            return OperateResult.CreateSuccessResult( Encoding.ASCII.GetBytes( stringBuilder.ToString( ) ) );
        }

        /// <summary>
        /// 根据错误码获取到真实的文本信息
        /// </summary>
        /// <param name="code">错误码</param>
        /// <returns>错误的文本描述</returns>
        public static string GetErrorDescriptionFromCode( string code )
        {
            switch (code)
            {
                case "01": return StringResources.Language.FujiSpbStatus01;
                case "02": return StringResources.Language.FujiSpbStatus02;
                case "03": return StringResources.Language.FujiSpbStatus03;
                case "04": return StringResources.Language.FujiSpbStatus04;
                case "05": return StringResources.Language.FujiSpbStatus05;
                case "06": return StringResources.Language.FujiSpbStatus06;
                case "07": return StringResources.Language.FujiSpbStatus07;
                case "09": return StringResources.Language.FujiSpbStatus09;
                case "0C": return StringResources.Language.FujiSpbStatus0C;
                default: return StringResources.Language.UnknownError;
            }
        }

        #endregion
    }
}
