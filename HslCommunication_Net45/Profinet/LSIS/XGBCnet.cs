using HslCommunication.Core;
using HslCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;

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
        public XGBCnet()
        {
            WordLength = 2;
            ByteTransform = new RegularByteTransform( );
        }

        #endregion

        #region Public Member

        /// <summary>
        /// PLC Station No.
        /// </summary>
        public byte Station { get; set; } = 0x05;

        #endregion

        #region Read Write Support

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[]> command = BuildReadByteCommand( Station, address, length );
            if (!command.IsSuccess) return command;

            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return read;

            return ExtractActualData( read.Content, true );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> command = BuildWriteByteCommand( Station, address, value );
            if (!command.IsSuccess) return command;

            OperateResult<byte[]> read = ReadBase( command.Content );
            if (!read.IsSuccess) return read;

            return ExtractActualData( read.Content, false );
        }

        #endregion

        #region Object Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"XGBCnet";
        }

        #endregion

        #region Static Helper

        /// <summary>
        /// reading address  Type of ReadByte
        /// </summary>
        /// <param name="station">plc station</param>
        /// <param name="address">address, for example: M100, D100, DW100</param>
        /// <param name="length">read length</param>
        /// <returns>command bytes</returns>
        private static OperateResult<byte[]> BuildReadByteCommand(byte station ,string address, ushort length)
        {
            var analysisResult = XGBFastEnet.AnalysisAddress(address);
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysisResult);

            List<byte> command = new List<byte>( );
            command.Add( 0x05 );    // ENQ
            command.AddRange( SoftBasic.BuildAsciiBytesFrom( station ) );
            command.Add( 0x72 );    // command r
            command.Add( 0x53 );    // command type: SB
            command.Add( 0x42 );
            command.AddRange( SoftBasic.BuildAsciiBytesFrom( (byte)analysisResult.Content.Length ) );
            command.AddRange( Encoding.ASCII.GetBytes( analysisResult.Content ) );
            command.AddRange( SoftBasic.BuildAsciiBytesFrom( (byte)length ) );
            command.Add( 0x04 );    // EOT

            int sum = 0;
            for (int i = 0; i < command.Count; i++)
            {
                sum += command[i];
            }
            command.AddRange( SoftBasic.BuildAsciiBytesFrom( (byte)sum ) );

            return OperateResult.CreateSuccessResult( command.ToArray( ) );
        }

        /// <summary>
        /// write data to address  Type of ReadByte
        /// </summary>
        /// <param name="station">plc station</param>
        /// <param name="address">address, for example: M100, D100, DW100</param>
        /// <param name="value">source value</param>
        /// <returns>command bytes</returns>
        private static OperateResult<byte[]> BuildWriteByteCommand( byte station, string address, byte[] value )
        {
            var analysisResult = XGBFastEnet.AnalysisAddress( address );
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysisResult );

            List<byte> command = new List<byte>( );
            command.Add( 0x05 );    // ENQ
            command.AddRange( SoftBasic.BuildAsciiBytesFrom( station ) );
            command.Add( 0x77 );    // command w
            command.Add( 0x53 );    // command type: SB
            command.Add( 0x42 );
            command.AddRange( SoftBasic.BuildAsciiBytesFrom( (byte)analysisResult.Content.Length ) );
            command.AddRange( Encoding.ASCII.GetBytes( analysisResult.Content ) );
            command.AddRange( SoftBasic.BuildAsciiBytesFrom( (byte)value.Length ) );
            command.AddRange( SoftBasic.BytesToAsciiBytes( value ) );
            command.Add( 0x04 );    // EOT

            int sum = 0;
            for (int i = 0; i < command.Count; i++)
            {
                sum += command[i];
            }
            command.AddRange( SoftBasic.BuildAsciiBytesFrom( (byte)sum ) );

            return OperateResult.CreateSuccessResult( command.ToArray( ) );
        }

        /// <summary>
        /// Extract actual data form plc response
        /// </summary>
        /// <param name="response">response data</param>
        /// <param name="isRead">read</param>
        /// <returns>result</returns>
        public static OperateResult<byte[]> ExtractActualData( byte[] response, bool isRead )
        {
            try
            {
                if (isRead)
                {
                    if(response[0] == 0x06)
                    {
                        byte[] buffer = new byte[response.Length - 13];
                        Array.Copy( response, 10, buffer, 0, buffer.Length );
                        return OperateResult.CreateSuccessResult( SoftBasic.AsciiBytesToBytes( buffer ) );
                    }
                    else
                    {
                        byte[] buffer = new byte[response.Length - 9];
                        Array.Copy( response, 6, buffer, 0, buffer.Length );
                        return new OperateResult<byte[]>( BitConverter.ToUInt16( SoftBasic.AsciiBytesToBytes( buffer ), 0 ), "Data:" + SoftBasic.ByteToHexString( response ) );
                    }
                }
                else
                {
                    if (response[0] == 0x06)
                    {
                        return OperateResult.CreateSuccessResult( new byte[0] );
                    }
                    else
                    {
                        byte[] buffer = new byte[response.Length - 9];
                        Array.Copy( response, 6, buffer, 0, buffer.Length );
                        return new OperateResult<byte[]>( BitConverter.ToUInt16( SoftBasic.AsciiBytesToBytes( buffer ), 0 ), "Data:" + SoftBasic.ByteToHexString( response ) );
                    }
                }
            }
            catch(Exception ex)
            {
                return new OperateResult<byte[]>( ex.Message );
            }
        }

        #endregion
    } 
}
