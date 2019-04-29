using HslCommunication.Core;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.LSIS
{
    /// <summary>
    /// 
    /// </summary>
    public class XGBCnet : SerialDeviceBase<RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public XGBCnet( )
        {
            WordLength = 2;
        }

        #endregion

        #region Public Member



        #endregion

        #region Read Write Support

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            return null; // to do
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override OperateResult Write( string address, byte[] value )
        {
            return null; // to do
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString( )
        {
            return $"XGBCnet";
        }

        #endregion

        #region Private Member



        #endregion

        #region Static Helper

        /// <summary>
        /// 将命令进行打包传送
        /// </summary>
        /// <param name="mcCommand">mc协议的命令</param>
        /// <param name="station">PLC的站号</param>
        /// <returns>最终的原始报文信息</returns>
        public static byte[] PackCommand( byte[] mcCommand, byte station = 0 )
        {
            return null; // to do
        }

        private static OperateResult<string> AnalysisAddress( string address )
        {
            StringBuilder sb = new StringBuilder( );
            try
            {
                sb.Append( "%" );
                sb.Append( address );
            }
            catch (Exception ex)
            {
                return new OperateResult<string>( ex.Message );
            }

            return OperateResult.CreateSuccessResult( sb.ToString( ) );
        }

        public enum LSDataType
        {
            Bit, Byte, Word, DWord, LWord, Continuous
        }

        private static OperateResult<LSDataType> AnalysisAddressDataType( string address )
        {
            LSDataType lSDataType = LSDataType.Continuous;
            try
            {
                if (address[0] == 'D')
                {
                    if (address[1] == 'W')
                    {
                        lSDataType = LSDataType.Word;
                    }
                    else if (address[1] == 'D')
                    {
                        lSDataType = LSDataType.DWord;
                    }
                    else if (address[1] == 'L')
                    {
                        lSDataType = LSDataType.LWord;
                    }
                    else if (address[1] == 'B')
                    {
                        lSDataType = LSDataType.Continuous;
                    }
                }
                else if (address[0] == 'M')
                {

                    if (address[1] == 'X')
                    {
                        lSDataType = LSDataType.Bit;
                    }
                    else if (address[1] == 'W')
                    {
                        lSDataType = LSDataType.Word;
                    }
                    else if (address[1] == 'D')
                    {
                        lSDataType = LSDataType.DWord;
                    }
                    else if (address[1] == 'L')
                    {
                        lSDataType = LSDataType.LWord;
                    }
                    else if (address[1] == 'B')
                    {
                        lSDataType = LSDataType.Continuous;
                    }
                }
                else if (address[0] == 'T')
                {

                    if (address[1] == 'X')
                    {
                        lSDataType = LSDataType.Bit;
                    }
                    else if (address[1] == 'W')
                    {
                        lSDataType = LSDataType.Word;
                    }
                    else if (address[1] == 'D')
                    {
                        lSDataType = LSDataType.DWord;
                    }
                    else if (address[1] == 'L')
                    {
                        lSDataType = LSDataType.LWord;
                    }
                    else if (address[1] == 'B')
                    {
                        lSDataType = LSDataType.Continuous;
                    }
                }
                else if (address[0] == 'C')
                {

                    if (address[1] == 'X')
                    {
                        lSDataType = LSDataType.Bit;
                    }
                    else if (address[1] == 'W')
                    {
                        lSDataType = LSDataType.Word;
                    }
                    else if (address[1] == 'D')
                    {
                        lSDataType = LSDataType.DWord;
                    }
                    else if (address[1] == 'L')
                    {
                        lSDataType = LSDataType.LWord;
                    }
                    else if (address[1] == 'B')
                    {
                        lSDataType = LSDataType.Continuous;
                    }
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<LSDataType>( ex.Message );
            }

            return OperateResult.CreateSuccessResult( lSDataType );
        }


        private static OperateResult<byte[]> BuildReadByteCommand( string address, ushort length )
        {
            var analysisResult = AnalysisAddress( address );
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysisResult );


            var lSDataType = AnalysisAddressDataType( address );
            if(!lSDataType.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( lSDataType );

            byte[] command = new byte[12 + analysisResult.Content.Length];
            switch (lSDataType.Content)
            {
                case LSDataType.Bit: command[2] = 0x00; break;
                case LSDataType.Byte: command[2] = 0x01; break;
                case LSDataType.Word: command[2] = 0x02; break;
                case LSDataType.DWord: command[2] = 0x03; break;
                case LSDataType.LWord: command[2] = 0x04; break;
                case LSDataType.Continuous: command[2] = 0x14; break;
                default: break;
            }
            command[0] = 0x54; // read
            command[1] = 0x00;
            // command[2] = 0x14; // continuous reading
            command[3] = 0x00;
            command[4] = 0x00; // Reserved
            command[5] = 0x00;
            command[6] = 0x01; // Block No ?? i don't know what is the meaning
            command[7] = 0x00;
            command[8] = (byte)analysisResult.Content.Length; // Variable Length
            command[9] = 0x00;
            Encoding.ASCII.GetBytes( analysisResult.Content ).CopyTo( command, 10 );
            BitConverter.GetBytes( length ).CopyTo( command, command.Length - 2 );

            return OperateResult.CreateSuccessResult( command );
        }

        #endregion
    }
}
