using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 所有三菱通讯类的通用辅助工具类，包含了一些通用的静态方法，可以使用本类来获取一些原始的报文信息。详细的操作参见例子
    /// </summary>
    public class MelsecHelper
    {
        #region Melsec Mc

        /// <summary>
        /// 解析A1E协议数据地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns></returns>
        public static OperateResult<MelsecA1EDataType, ushort> McA1EAnalysisAddress(string address)
        {
            var result = new OperateResult<MelsecA1EDataType, ushort>();
            try
            {
                switch (address[0])
                {
                    case 'X':
                    case 'x':
                        {
                            result.Content1 = MelsecA1EDataType.X;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.X.FromBase);
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            result.Content1 = MelsecA1EDataType.Y;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.Y.FromBase);
                            break;
                        }
                    case 'M':
                    case 'm':
                        {
                            result.Content1 = MelsecA1EDataType.M;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.M.FromBase);
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            result.Content1 = MelsecA1EDataType.S;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.S.FromBase);
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            result.Content1 = MelsecA1EDataType.D;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.D.FromBase);
                            break;
                        }
                    case 'R':
                    case 'r':
                        {
                            result.Content1 = MelsecA1EDataType.R;
                            result.Content2 = Convert.ToUInt16(address.Substring(1), MelsecA1EDataType.R.FromBase);
                            break;
                        }
                    default: throw new Exception(StringResources.Language.NotSupportedDataType);
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
        /// 解析数据地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析值</returns>
        public static OperateResult<MelsecMcDataType, int> McAnalysisAddress( string address )
        {
            var result = new OperateResult<MelsecMcDataType, int>( );
            try
            {
                switch (address[0])
                {
                    case 'M':
                    case 'm':
                        {
                            result.Content1 = MelsecMcDataType.M;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.M.FromBase );
                            break;
                        }
                    case 'X':
                    case 'x':
                        {
                            result.Content1 = MelsecMcDataType.X;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.X.FromBase );
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            result.Content1 = MelsecMcDataType.Y;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Y.FromBase );
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            result.Content1 = MelsecMcDataType.D;
                            result.Content2 = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.D.FromBase );
                            break;
                        }
                    case 'W':
                    case 'w':
                        {
                            result.Content1 = MelsecMcDataType.W;
                            result.Content2 = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.W.FromBase );
                            break;
                        }
                    case 'L':
                    case 'l':
                        {
                            result.Content1 = MelsecMcDataType.L;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.L.FromBase );
                            break;
                        }
                    case 'F':
                    case 'f':
                        {
                            result.Content1 = MelsecMcDataType.F;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.F.FromBase );
                            break;
                        }
                    case 'V':
                    case 'v':
                        {
                            result.Content1 = MelsecMcDataType.V;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.V.FromBase );
                            break;
                        }
                    case 'B':
                    case 'b':
                        {
                            result.Content1 = MelsecMcDataType.B;
                            result.Content2 = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.B.FromBase );
                            break;
                        }
                    case 'R':
                    case 'r':
                        {
                            result.Content1 = MelsecMcDataType.R;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.R.FromBase );
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                result.Content1 = MelsecMcDataType.SN;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.SN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                result.Content1 = MelsecMcDataType.SS;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.SS.FromBase );
                                break;
                            }
                            else if (address[1] == 'C' || address[1] == 'c')
                            {
                                result.Content1 = MelsecMcDataType.SC;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.SC.FromBase );
                                break;
                            }
                            else
                            {
                                result.Content1 = MelsecMcDataType.S;
                                result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.S.FromBase );
                                break;
                            }
                        }
                    case 'Z':
                    case 'z':
                        {
                            if (address.StartsWith( "ZR" ) || address.StartsWith( "zr" ))
                            {
                                result.Content1 = MelsecMcDataType.ZR;
                                result.Content2 = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.ZR.FromBase );
                                break;
                            }
                            else
                            {
                                result.Content1 = MelsecMcDataType.Z;
                                result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Z.FromBase );
                                break;
                            }
                        }
                    case 'T':
                    case 't':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                result.Content1 = MelsecMcDataType.TN;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.TN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                result.Content1 = MelsecMcDataType.TS;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.TS.FromBase );
                                break;
                            }
                            else if (address[1] == 'C' || address[1] == 'c')
                            {
                                result.Content1 = MelsecMcDataType.TC;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.TC.FromBase );
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
                                result.Content1 = MelsecMcDataType.CN;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.CN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                result.Content1 = MelsecMcDataType.CS;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.CS.FromBase );
                                break;
                            }
                            else if (address[1] == 'C' || address[1] == 'c')
                            {
                                result.Content1 = MelsecMcDataType.CC;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.CC.FromBase );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
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
            result.Message = StringResources.Language.SuccessText;
            return result;
        }


        /// <summary>
        /// 基恩士解析数据地址
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析值</returns>
        public static OperateResult<MelsecMcDataType, int> KeyenceAnalysisAddress( string address )
        {
            var result = new OperateResult<MelsecMcDataType, int>( );
            try
            {
                switch (address[0])
                {
                    case 'M':
                    case 'm':
                        {
                            result.Content1 = MelsecMcDataType.Keyence_M;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Keyence_M.FromBase );
                            break;
                        }
                    case 'X':
                    case 'x':
                        {
                            result.Content1 = MelsecMcDataType.Keyence_X;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Keyence_X.FromBase );
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            result.Content1 = MelsecMcDataType.Keyence_Y;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Keyence_Y.FromBase );
                            break;
                        }
                    case 'B':
                    case 'b':
                        {
                            result.Content1 = MelsecMcDataType.Keyence_B;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Keyence_B.FromBase );
                            break;
                        }
                    case 'L':
                    case 'l':
                        {
                            result.Content1 = MelsecMcDataType.Keyence_L;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Keyence_L.FromBase );
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            if (address[1] == 'M' || address[1] == 'm')
                            {
                                result.Content1 = MelsecMcDataType.Keyence_SM;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.Keyence_SM.FromBase );
                                break;
                            }
                            else if (address[1] == 'D' || address[1] == 'd')
                            {
                                result.Content1 = MelsecMcDataType.Keyence_SD;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.Keyence_SD.FromBase );
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
                            result.Content1 = MelsecMcDataType.Keyence_D;
                            result.Content2 = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Keyence_D.FromBase );
                            break;
                        }
                    case 'R':
                    case 'r':
                        {
                            result.Content1 = MelsecMcDataType.Keyence_R;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Keyence_R.FromBase );
                            break;
                        }
                    case 'Z':
                    case 'z':
                        {
                            if (address[1] == 'R' || address[1] == 'r')
                            {
                                result.Content1 = MelsecMcDataType.Keyence_ZR;
                                result.Content2 = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.Keyence_ZR.FromBase );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    case 'W':
                    case 'w':
                        {
                            result.Content1 = MelsecMcDataType.Keyence_W;
                            result.Content2 = Convert.ToUInt16( address.Substring( 1 ), MelsecMcDataType.Keyence_W.FromBase );
                            break;
                        }
                    case 'T':
                    case 't':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                result.Content1 = MelsecMcDataType.Keyence_TN;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.Keyence_TN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                result.Content1 = MelsecMcDataType.Keyence_TS;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.Keyence_TS.FromBase );
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
                                result.Content1 = MelsecMcDataType.Keyence_CN;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.Keyence_CN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                result.Content1 = MelsecMcDataType.Keyence_CS;
                                result.Content2 = Convert.ToUInt16( address.Substring( 2 ), MelsecMcDataType.Keyence_CS.FromBase );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
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
            result.Message = StringResources.Language.SuccessText;
            return result;
        }

        /// <summary>
        /// 从地址，长度，是否位读取进行创建读取的MC的核心报文
        /// </summary>
        /// <param name="address">三菱的地址信息，具体格式参照<seealso cref="MelsecMcNet"/> 的注释说明</param>
        /// <param name="length">读取的长度信息</param>
        /// <param name="isBit">是否进行了位读取操作</param>
        /// <param name="analysisAddress">对地址分析的委托方法</param>
        /// <returns>带有成功标识的报文对象</returns>
        public static OperateResult<byte[]> BuildReadMcCoreCommand(string address, ushort length, bool isBit, Func<string, OperateResult<MelsecMcDataType, int>> analysisAddress)
        {
            OperateResult<MelsecMcDataType, int> analysis = analysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] command = new byte[10];
            command[0] = 0x01;                                               // 批量读取数据命令
            command[1] = 0x04;
            command[2] = isBit ? (byte)0x01 : (byte)0x00;                    // 以点为单位还是字为单位成批读取
            command[3] = 0x00;
            command[4] = BitConverter.GetBytes( analysis.Content2 )[0];      // 起始地址的地位
            command[5] = BitConverter.GetBytes( analysis.Content2 )[1];
            command[6] = BitConverter.GetBytes( analysis.Content2 )[2];
            command[7] = analysis.Content1.DataCode;                         // 指明读取的数据
            command[8] = (byte)(length % 256);                               // 软元件的长度
            command[9] = (byte)(length / 256);

            return OperateResult.CreateSuccessResult( command );
        }

        /// <summary>
        /// 从地址，长度，是否位读取进行创建读取Ascii格式的MC的核心报文
        /// </summary>
        /// <param name="address">三菱的地址信息，具体格式参照<seealso cref="MelsecMcNet"/> 的注释说明</param>
        /// <param name="length">读取的长度信息</param>
        /// <param name="isBit">是否进行了位读取操作</param>
        /// <param name="analysisAddress">对地址分析的委托方法</param>
        /// <returns>带有成功标识的报文对象</returns>
        public static OperateResult<byte[]> BuildAsciiReadMcCoreCommand(string address, ushort length, bool isBit, Func<string, OperateResult<MelsecMcDataType, int>> analysisAddress )
        {
            OperateResult<MelsecMcDataType, int> analysis = analysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] command = new byte[20];
            command[ 0] = 0x30;                                                               // 批量读取数据命令
            command[ 1] = 0x34;
            command[ 2] = 0x30;
            command[ 3] = 0x31;
            command[ 4] = 0x30;                                                               // 以点为单位还是字为单位成批读取
            command[ 5] = 0x30;
            command[ 6] = 0x30;
            command[ 7] = isBit ? (byte)0x31 : (byte)0x30;
            command[ 8] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[0];          // 软元件类型
            command[ 9] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[1];
            command[10] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[0];            // 起始地址的地位
            command[11] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[1];
            command[12] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[2];
            command[13] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[3];
            command[14] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[4];
            command[15] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[5];
            command[16] = MelsecHelper.BuildBytesFromData( length )[0];                                             // 软元件点数
            command[17] = MelsecHelper.BuildBytesFromData( length )[1];
            command[18] = MelsecHelper.BuildBytesFromData( length )[2];
            command[19] = MelsecHelper.BuildBytesFromData( length )[3];

            return OperateResult.CreateSuccessResult( command );
        }

        /// <summary>
        /// 以字为单位，创建数据写入的核心报文
        /// </summary>
        /// <param name="address">三菱的地址信息，具体格式参照<seealso cref="MelsecMcNet"/> 的注释说明</param>
        /// <param name="value">实际的原始数据信息</param>
        /// <param name="analysisAddress">对地址分析的委托方法</param>
        /// <returns>带有成功标识的报文对象</returns>
        public static OperateResult<byte[]> BuildWriteWordCoreCommand(string address, byte[] value, Func<string, OperateResult<MelsecMcDataType, int>> analysisAddress )
        {
            OperateResult<MelsecMcDataType, int> analysis = analysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            if (value == null) value = new byte[0];
            byte[] command = new byte[10 + value.Length];
            command[0] = 0x01;                                                        // 批量读取数据命令
            command[1] = 0x14;
            command[2] = 0x00;                                                        // 以字为单位成批读取
            command[3] = 0x00;
            command[4] = BitConverter.GetBytes( analysis.Content2 )[0];               // 起始地址的地位
            command[5] = BitConverter.GetBytes( analysis.Content2 )[1];
            command[6] = BitConverter.GetBytes( analysis.Content2 )[2];
            command[7] = analysis.Content1.DataCode;                                  // 指明写入的数据
            command[8] = (byte)(value.Length / 2 % 256);                              // 软元件长度的地位
            command[9] = (byte)(value.Length / 2 / 256);
            value.CopyTo( command, 10 );

            return OperateResult.CreateSuccessResult( command );
        }

        /// <summary>
        /// 以字为单位，创建ASCII数据写入的核心报文
        /// </summary>
        /// <param name="address">三菱的地址信息，具体格式参照<seealso cref="MelsecMcNet"/> 的注释说明</param>
        /// <param name="value">实际的原始数据信息</param>
        /// <param name="analysisAddress">对地址分析的委托方法</param>
        /// <returns>带有成功标识的报文对象</returns>
        public static OperateResult<byte[]> BuildAsciiWriteWordCoreCommand(string address, byte[] value, Func<string, OperateResult<MelsecMcDataType, int>> analysisAddress )
        {
            OperateResult<MelsecMcDataType, int> analysis = analysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            if (value == null) value = new byte[0];
            byte[] buffer = new byte[value.Length * 2];
            for (int i = 0; i < value.Length / 2; i++)
            {
                MelsecHelper.BuildBytesFromData( BitConverter.ToUInt16( value, i * 2 ) ).CopyTo( buffer, 4 * i );
            }
            value = buffer;
            
            byte[] command = new byte[20 + value.Length];
            command[ 0] = 0x31;                                                                              // 批量写入的命令
            command[ 1] = 0x34;
            command[ 2] = 0x30;
            command[ 3] = 0x31;
            command[ 4] = 0x30;                                                                              // 子命令
            command[ 5] = 0x30;
            command[ 6] = 0x30;
            command[ 7] = 0x30;
            command[ 8] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[0];                         // 软元件类型
            command[ 9] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[1];
            command[10] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[0];     // 起始地址的地位
            command[11] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[1];
            command[12] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[2];
            command[13] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[3];
            command[14] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[4];
            command[15] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[5];
            command[16] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length / 4) )[0];              // 软元件点数
            command[17] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length / 4) )[1];
            command[18] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length / 4) )[2];
            command[19] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length / 4) )[3];
            value.CopyTo( command, 20 );

            return OperateResult.CreateSuccessResult( command );
        }

        /// <summary>
        /// 以位为单位，创建数据写入的核心报文
        /// </summary>
        /// <param name="address">三菱的地址信息，具体格式参照<seealso cref="MelsecMcNet"/> 的注释说明</param>
        /// <param name="value">原始的bool数组数据</param>
        /// <param name="analysisAddress">对地址分析的委托方法</param>
        /// <returns>带有成功标识的报文对象</returns>
        public static OperateResult<byte[]> BuildWriteBitCoreCommand( string address, bool[] value, Func<string, OperateResult<MelsecMcDataType, int>> analysisAddress )
        {
            OperateResult<MelsecMcDataType, int> analysis = analysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            if (value == null) value = new bool[0];
            byte[] buffer = MelsecHelper.TransBoolArrayToByteData( value );
            byte[] command = new byte[10 + buffer.Length];
            command[0] = 0x01;                                                        // 批量写入数据命令
            command[1] = 0x14;
            command[2] = 0x01;                                                        // 以位为单位成批写入
            command[3] = 0x00;
            command[4] = BitConverter.GetBytes( analysis.Content2 )[0];               // 起始地址的地位
            command[5] = BitConverter.GetBytes( analysis.Content2 )[1];
            command[6] = BitConverter.GetBytes( analysis.Content2 )[2];
            command[7] = analysis.Content1.DataCode;                                  // 指明写入的数据
            command[8] = (byte)(value.Length % 256);                                  // 软元件长度的地位
            command[9] = (byte)(value.Length / 256);
            buffer.CopyTo( command, 10 );

            return OperateResult.CreateSuccessResult( command );
        }

        /// <summary>
        /// 以位为单位，创建ASCII数据写入的核心报文
        /// </summary>
        /// <param name="address">三菱的地址信息，具体格式参照<seealso cref="MelsecMcNet"/> 的注释说明</param>
        /// <param name="value">原始的bool数组数据</param>
        /// <param name="analysisAddress">对地址分析的委托方法</param>
        /// <returns>带有成功标识的报文对象</returns>
        public static OperateResult<byte[]> BuildAsciiWriteBitCoreCommand( string address, bool[] value, Func<string, OperateResult<MelsecMcDataType, int>> analysisAddress )
        {
            OperateResult<MelsecMcDataType, int> analysis = analysisAddress( address );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            if (value == null) value = new bool[0];
            byte[] buffer = value.Select( m => m ? (byte)0x31 : (byte)0x30 ).ToArray( );
            
            byte[] command = new byte[20 + buffer.Length];
            command[0] = 0x31;                                                                              // 批量写入的命令
            command[1] = 0x34;
            command[2] = 0x30;
            command[3] = 0x31;
            command[4] = 0x30;                                                                              // 子命令
            command[5] = 0x30;
            command[6] = 0x30;
            command[7] = 0x31;
            command[8] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[0];                         // 软元件类型
            command[9] = Encoding.ASCII.GetBytes( analysis.Content1.AsciiCode )[1];
            command[10] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[0];     // 起始地址的地位
            command[11] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[1];
            command[12] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[2];
            command[13] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[3];
            command[14] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[4];
            command[15] = MelsecHelper.BuildBytesFromAddress( analysis.Content2, analysis.Content1 )[5];
            command[16] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length) )[0];              // 软元件点数
            command[17] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length) )[1];
            command[18] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length) )[2];
            command[19] = MelsecHelper.BuildBytesFromData( (ushort)(value.Length) )[3];
            buffer.CopyTo( command, 20 );

            return OperateResult.CreateSuccessResult( command );
        }

        #endregion

        #region Common Logic

        /// <summary>
        /// 从字节构建一个ASCII格式的地址字节
        /// </summary>
        /// <param name="value">字节信息</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( byte value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X2" ) );
        }

        /// <summary>
        /// 从short数据构建一个ASCII格式地址字节
        /// </summary>
        /// <param name="value">short值</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( short value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X4" ) );
        }

        /// <summary>
        /// 从ushort数据构建一个ASCII格式地址字节
        /// </summary>
        /// <param name="value">ushort值</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( ushort value )
        {
            return Encoding.ASCII.GetBytes( value.ToString( "X4" ) );
        }

        /// <summary>
        /// 从三菱的地址中构建MC协议的6字节的ASCII格式的地址
        /// </summary>
        /// <param name="address">三菱地址</param>
        /// <param name="type">三菱的数据类型</param>
        /// <returns>6字节的ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromAddress( int address, MelsecMcDataType type )
        {
            return Encoding.ASCII.GetBytes( address.ToString( type.FromBase == 10 ? "D6" : "X6" ) );
        }


        /// <summary>
        /// 从字节数组构建一个ASCII格式的地址字节
        /// </summary>
        /// <param name="value">字节信息</param>
        /// <returns>ASCII格式的地址</returns>
        internal static byte[] BuildBytesFromData( byte[] value )
        {
            byte[] buffer = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                BuildBytesFromData( value[i] ).CopyTo( buffer, 2 * i );
            }
            return buffer;
        }

        /// <summary>
        /// 将0，1，0，1的字节数组压缩成三菱格式的字节数组来表示开关量的
        /// </summary>
        /// <param name="value">原始的数据字节</param>
        /// <returns>压缩过后的数据字节</returns>
        internal static byte[] TransBoolArrayToByteData( byte[] value )
        {
            int length = (value.Length + 1) / 2;
            byte[] buffer = new byte[length];

            for (int i = 0; i < length; i++)
            {
                if (value[i * 2 + 0] != 0x00) buffer[i] += 0x10;
                if ((i * 2 + 1) < value.Length)
                {
                    if (value[i * 2 + 1] != 0x00) buffer[i] += 0x01;
                }
            }

            return buffer;
        }

        /// <summary>
        /// 将bool的组压缩成三菱格式的字节数组来表示开关量的
        /// </summary>
        /// <param name="value">原始的数据字节</param>
        /// <returns>压缩过后的数据字节</returns>
        internal static byte[] TransBoolArrayToByteData( bool[] value )
        {
            int length = (value.Length + 1) / 2;
            byte[] buffer = new byte[length];

            for (int i = 0; i < length; i++)
            {
                if (value[i * 2 + 0]) buffer[i] += 0x10;
                if ((i * 2 + 1) < value.Length)
                {
                    if (value[i * 2 + 1]) buffer[i] += 0x01;
                }
            }

            return buffer;
        }

        #endregion

        #region CRC Check

        /// <summary>
        /// 计算Fx协议指令的和校验信息
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <returns>校验之后的数据</returns>
        internal static byte[] FxCalculateCRC( byte[] data )
        {
            int sum = 0;
            for (int i = 1; i < data.Length - 2; i++)
            {
                sum += data[i];
            }
            return BuildBytesFromData( (byte)sum );
        }

        /// <summary>
        /// 检查指定的和校验是否是正确的
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <returns>是否成功</returns>
        internal static bool CheckCRC( byte[] data )
        {
            byte[] crc = FxCalculateCRC( data );
            if (crc[0] != data[data.Length - 2]) return false;
            if (crc[1] != data[data.Length - 1]) return false;
            return true;
        }

        #endregion
    }
}
