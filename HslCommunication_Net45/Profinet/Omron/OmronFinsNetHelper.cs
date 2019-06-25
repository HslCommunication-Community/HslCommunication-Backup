using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;

namespace HslCommunication.Profinet.Omron
{
    /// <summary>
    /// Omron PLC的FINS协议相关的辅助类
    /// </summary>
    public class OmronFinsNetHelper
    {
        #region Static Method Helper
        
        /// <summary>
        /// 解析数据地址，Omron手册第188页
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="isBit">是否是位地址</param>
        /// <returns>解析后的结果地址对象</returns>
        public static OperateResult<OmronFinsDataType, byte[]> AnalysisAddress( string address, bool isBit )
        {
            var result = new OperateResult<OmronFinsDataType, byte[]>( );
            try
            {
                switch (address[0])
                {
                    case 'D':
                    case 'd':
                        {
                            // DM区数据
                            result.Content1 = OmronFinsDataType.DM;
                            break;
                        }
                    case 'C':
                    case 'c':
                        {
                            // CIO区数据
                            result.Content1 = OmronFinsDataType.CIO;
                            break;
                        }
                    case 'W':
                    case 'w':
                        {
                            // WR区
                            result.Content1 = OmronFinsDataType.WR;
                            break;
                        }
                    case 'H':
                    case 'h':
                        {
                            // HR区
                            result.Content1 = OmronFinsDataType.HR;
                            break;
                        }
                    case 'A':
                    case 'a':
                        {
                            // AR区
                            result.Content1 = OmronFinsDataType.AR;
                            break;
                        }
                    case 'E':
                    case 'e':
                        {
                            // E区，比较复杂，需要专门的计算
                            string[] splits = address.Split( new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
                            int block = Convert.ToInt32( splits[0].Substring( 1 ), 16 );
                            if (block < 16)
                            {
                                result.Content1 = new OmronFinsDataType( (byte)(0x20 + block), (byte)(0xA0 + block) );
                            }
                            else
                            {
                                result.Content1 = new OmronFinsDataType( (byte)(0xE0 + block - 16), (byte)(0x60 + block - 16) );
                            }
                            break;
                        }
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
                }

                if (address[0] == 'E' || address[0] == 'e')
                {
                    string[] splits = address.Split( new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
                    if (isBit)
                    {
                        // 位操作
                        ushort addr = ushort.Parse( splits[1] );
                        result.Content2 = new byte[3];
                        result.Content2[0] = BitConverter.GetBytes( addr )[1];
                        result.Content2[1] = BitConverter.GetBytes( addr )[0];

                        if (splits.Length > 2)
                        {
                            result.Content2[2] = byte.Parse( splits[2] );
                            if (result.Content2[2] > 15)
                            {
                                throw new Exception( StringResources.Language.OmronAddressMustBeZeroToFiveteen );
                            }
                        }
                    }
                    else
                    {
                        // 字操作
                        ushort addr = ushort.Parse( splits[1] );
                        result.Content2 = new byte[3];
                        result.Content2[0] = BitConverter.GetBytes( addr )[1];
                        result.Content2[1] = BitConverter.GetBytes( addr )[0];
                    }
                }
                else
                {
                    if (isBit)
                    {
                        // 位操作
                        string[] splits = address.Substring( 1 ).Split( new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
                        ushort addr = ushort.Parse( splits[0] );
                        result.Content2 = new byte[3];
                        result.Content2[0] = BitConverter.GetBytes( addr )[1];
                        result.Content2[1] = BitConverter.GetBytes( addr )[0];

                        if (splits.Length > 1)
                        {
                            result.Content2[2] = byte.Parse( splits[1] );
                            if (result.Content2[2] > 15)
                            {
                                throw new Exception( StringResources.Language.OmronAddressMustBeZeroToFiveteen );
                            }
                        }
                    }
                    else
                    {
                        // 字操作
                        ushort addr = ushort.Parse( address.Substring( 1 ) );
                        result.Content2 = new byte[3];
                        result.Content2[0] = BitConverter.GetBytes( addr )[1];
                        result.Content2[1] = BitConverter.GetBytes( addr )[0];
                    }
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
        /// 根据读取的地址，长度，是否位读取创建Fins协议的核心报文
        /// </summary>
        /// <param name="address">地址，具体格式请参照示例说明</param>
        /// <param name="length">读取的数据长度</param>
        /// <param name="isBit">是否使用位读取</param>
        /// <returns>带有成功标识的Fins核心报文</returns>
        public static OperateResult<byte[]> BuildReadCommand( string address, ushort length, bool isBit )
        {
            var analysis = OmronFinsNetHelper.AnalysisAddress( address, isBit );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] _PLCCommand = new byte[8];
            _PLCCommand[0] = 0x01;    // 读取存储区数据
            _PLCCommand[1] = 0x01;
            if (isBit)
            {
                _PLCCommand[2] = analysis.Content1.BitCode;
            }
            else
            {
                _PLCCommand[2] = analysis.Content1.WordCode;
            }
            analysis.Content2.CopyTo( _PLCCommand, 3 );
            _PLCCommand[6] = (byte)(length / 256);                       // 长度
            _PLCCommand[7] = (byte)(length % 256);

            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        /// <summary>
        /// 根据写入的地址，数据，是否位写入生成Fins协议的核心报文
        /// </summary>
        /// <param name="address">地址内容，具体格式请参照示例说明</param>
        /// <param name="value">实际的数据</param>
        /// <param name="isBit">是否位数据</param>
        /// <returns>带有成功标识的Fins核心报文</returns>
        public static OperateResult<byte[]> BuildWriteWordCommand( string address, byte[] value, bool isBit )
        {
            var analysis = OmronFinsNetHelper.AnalysisAddress( address, isBit );
            if (!analysis.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysis );

            byte[] _PLCCommand = new byte[8 + value.Length];
            _PLCCommand[0] = 0x01;
            _PLCCommand[1] = 0x02;

            if (isBit)
            {
                _PLCCommand[2] = analysis.Content1.BitCode;
            }
            else
            {
                _PLCCommand[2] = analysis.Content1.WordCode;
            }

            analysis.Content2.CopyTo( _PLCCommand, 3 );
            if (isBit)
            {
                _PLCCommand[6] = (byte)(value.Length / 256);
                _PLCCommand[7] = (byte)(value.Length % 256);
            }
            else
            {
                _PLCCommand[6] = (byte)(value.Length / 2 / 256);
                _PLCCommand[7] = (byte)(value.Length / 2 % 256);
            }

            value.CopyTo( _PLCCommand, 8 );
            
            return OperateResult.CreateSuccessResult( _PLCCommand );
        }

        /// <summary>
        /// 验证欧姆龙的Fins-TCP返回的数据是否正确的数据，如果正确的话，并返回所有的数据内容
        /// </summary>
        /// <param name="response">来自欧姆龙返回的数据内容</param>
        /// <param name="isRead">是否读取</param>
        /// <returns>带有是否成功的结果对象</returns>
        public static OperateResult<byte[]> ResponseValidAnalysis( byte[] response, bool isRead )
        {
            if (response.Length >= 16)
            {
                // 提取错误码 -> Extracting error Codes
                byte[] buffer = new byte[4];
                buffer[0] = response[15];
                buffer[1] = response[14];
                buffer[2] = response[13];
                buffer[3] = response[12];

                int err = BitConverter.ToInt32( buffer, 0 );
                if (err > 0) return new OperateResult<byte[]>( err, GetStatusDescription( err ) );

                byte[] result = new byte[response.Length - 16];
                Array.Copy( response, 16, result, 0, result.Length );
                return UdpResponseValidAnalysis( result, isRead );
                //if (response.Length >= 30)
                //{
                //    err = response[28] * 256 + response[29];
                //    // if (err > 0) return new OperateResult<byte[]>( err, StringResources.Language.OmronReceiveDataError );

                //    if (!isRead)
                //    {
                //        OperateResult<byte[]> success = OperateResult.CreateSuccessResult( new byte[0] );
                //        success.ErrorCode = err;
                //        success.Message = GetStatusDescription( err );
                //        return success;
                //    }
                //    else
                //    {
                //        // 读取操作 -> read operate
                //        byte[] content = new byte[response.Length - 30];
                //        if (content.Length > 0) Array.Copy( response, 30, content, 0, content.Length );

                //        OperateResult<byte[]> success = OperateResult.CreateSuccessResult( content );
                //        if (content.Length == 0) success.IsSuccess = false;
                //        success.ErrorCode = err;
                //        success.Message = GetStatusDescription( err );
                //        return success;
                //    }
                //}
            }

            return new OperateResult<byte[]>( StringResources.Language.OmronReceiveDataError );
        }


        /// <summary>
        /// 验证欧姆龙的Fins-Udp返回的数据是否正确的数据，如果正确的话，并返回所有的数据内容
        /// </summary>
        /// <param name="response">来自欧姆龙返回的数据内容</param>
        /// <param name="isRead">是否读取</param>
        /// <returns>带有是否成功的结果对象</returns>
        public static OperateResult<byte[]> UdpResponseValidAnalysis( byte[] response, bool isRead )
        {
            if (response.Length >= 14)
            {
                int err = response[12] * 256 + response[13];
                // if (err > 0) return new OperateResult<byte[]>( err, StringResources.Language.OmronReceiveDataError );

                if (!isRead)
                {
                    OperateResult<byte[]> success = OperateResult.CreateSuccessResult( new byte[0] );
                    success.ErrorCode = err;
                    success.Message = GetStatusDescription( err ) + " Received:" + SoftBasic.ByteToHexString( response, ' ' );
                    return success;
                }
                else
                {
                    // 读取操作 -> read operate
                    byte[] content = new byte[response.Length - 14];
                    if (content.Length > 0) Array.Copy( response, 14, content, 0, content.Length );

                    OperateResult<byte[]> success = OperateResult.CreateSuccessResult( content );
                    if (content.Length == 0) success.IsSuccess = false;
                    success.ErrorCode = err;
                    success.Message = GetStatusDescription( err ) + " Received:" + SoftBasic.ByteToHexString( response, ' ' );
                    return success;
                }
            }

            return new OperateResult<byte[]>( StringResources.Language.OmronReceiveDataError );
        }

        /// <summary>
        /// 获取错误信息的字符串描述文本
        /// </summary>
        /// <param name="err">错误码</param>
        /// <returns>文本描述</returns>
        public static string GetStatusDescription( int err )
        {
            switch (err)
            {
                case 0: return StringResources.Language.OmronStatus0;
                case 1: return StringResources.Language.OmronStatus1;
                case 2: return StringResources.Language.OmronStatus2;
                case 3: return StringResources.Language.OmronStatus3;
                case 20: return StringResources.Language.OmronStatus20;
                case 21: return StringResources.Language.OmronStatus21;
                case 22: return StringResources.Language.OmronStatus22;
                case 23: return StringResources.Language.OmronStatus23;
                case 24: return StringResources.Language.OmronStatus24;
                case 25: return StringResources.Language.OmronStatus25;
                default: return StringResources.Language.UnknownError;
            }
        }

        #endregion

    }
}
