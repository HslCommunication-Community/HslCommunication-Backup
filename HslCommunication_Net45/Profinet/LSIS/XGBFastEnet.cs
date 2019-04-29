using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;

namespace HslCommunication.Profinet.LSIS
{
    /// <summary>
    /// XGB Fast Enet I/F module supports open Ethernet. It provides network configuration that is to connect LSIS and other company PLC, PC on network
    /// </summary>
    public class XGBFastEnet : NetworkDeviceBase<LsisFastEnetMessage, RegularByteTransform>
    {
        #region Constractor

        /// <summary>
        /// Instantiate a Default object
        /// </summary>
        public XGBFastEnet( )
        {
            WordLength = 2;
            IpAddress = string.Empty;
            Port = 2004;
        }

        /// <summary>
        /// Instantiate a object by ipaddress and port
        /// </summary>
        /// <param name="ipAddress">the ip address of the plc</param>
        /// <param name="port">the port of the plc, default is 2004</param>
        public XGBFastEnet( string ipAddress, int port )
        {
            WordLength = 2;
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// CPU TYPE
        /// </summary>
        public string CpuType { get; private set; }

        /// <summary>
        /// Cpu is error
        /// </summary>
        public bool CpuError { get; private set; }

        /// <summary>
        /// RUN, STOP, ERROR, DEBUG
        /// </summary>
        public LSCpuStatus LSCpuStatus { get; private set; }

        /// <summary>
        /// FEnet I/F module’s Base No.
        /// </summary>
        public byte BaseNo
        {
            get => baseNo;
            set => baseNo = value;
        }

        /// <summary>
        /// FEnet I/F module’s Slot No.
        /// </summary>
        public byte SlotNo
        {
            get => slotNo;
            set => slotNo = value;
        }

        #endregion

        #region Read Write

        /// <summary>
        /// Read Bytes from plc, you should specify address
        /// </summary>
        /// <param name="address">Start Address, for example: M100</param>
        /// <param name="length">Array of data Lengths</param>
        /// <returns>Whether to read the successful result object</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <remarks>
        /// </remarks>
        /// <example>
        /// </example>
        public override OperateResult<byte[]> Read( string address, ushort length )
        {
            // build read command
            OperateResult<byte[]> coreResult = BuildReadByteCommand( address, length );
            if (!coreResult.IsSuccess) return coreResult;

            // communication
            var read = ReadFromCoreServer( PackCommand( coreResult.Content ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // analysis read result
            return ExtractActualData( read.Content );
        }

        /// <summary>
        /// Write bytes to plc, you should specify bytes, can't be null
        /// </summary>
        /// <param name="address">Start Address, for example: M100</param>
        /// <param name="value">source dara</param>
        /// <returns>Whether to write the successful result object</returns>
        /// <exception cref="NullReferenceException"></exception>
        public override OperateResult Write( string address, byte[] value )
        {
            // build write command
            OperateResult<byte[]> coreResult = BuildWriteByteCommand( address, value );
            if (!coreResult.IsSuccess) return coreResult;

            // communication
            var read = ReadFromCoreServer( PackCommand( coreResult.Content ) );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( read );

            // analysis read result
            return ExtractActualData( read.Content );
        }

        #endregion

        #region Read Write Byte

        /// <summary>
        /// Read single byte value from plc
        /// </summary>
        /// <param name="address">Start address</param>
        /// <returns>result</returns>
        public OperateResult<byte> ReadByte( string address )
        {
            var read = Read( address, 2 );
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte>( read );

            return OperateResult.CreateSuccessResult( read.Content[0] );
        }

        /// <summary>
        /// Write single byte value to plc
        /// </summary>
        /// <param name="address">Start address</param>
        /// <param name="value">value</param>
        /// <returns>Whether to write the successful</returns>
        public OperateResult Write( string address, byte value )
        {
            return Write( address, new byte[] { value } );
        }

        #endregion

        #region Private Member

        private byte[] PackCommand( byte[] coreCommand )
        {
            byte[] command = new byte[coreCommand.Length + 20];
            Encoding.ASCII.GetBytes( CompanyID1 ).CopyTo( command, 0 );
            switch (cpuInfo)
            {
                case LSCpuInfo.XGK: command[12] = 0xA0; break;
                case LSCpuInfo.XGI: command[12] = 0xA4; break;
                case LSCpuInfo.XGR: command[12] = 0xA8; break;
                case LSCpuInfo.XGB_MK: command[12] = 0xB0; break;
                case LSCpuInfo.XGB_IEC: command[12] = 0xB4; break;
                default: break;
            }
            command[13] = 0x33;
            BitConverter.GetBytes( (short)coreCommand.Length ).CopyTo( command, 16 );
            command[18] = (byte)(baseNo * 16 + slotNo);

            int count = 0;
            for (int i = 0; i < 19; i++)
            {
                count += command[i];
            }
            command[19] = (byte)count;

            coreCommand.CopyTo( command, 20 );

            string hex = SoftBasic.ByteToHexString( command, ' ' );
            return command;
        }

        #endregion

        #region Const Value

        private const string CompanyID1 = "LSIS-XGT";
        private const string CompanyID2 = "LGIS-GLOGA";
        private LSCpuInfo cpuInfo = LSCpuInfo.XGK;
        private byte baseNo = 0;
        private byte slotNo = 3;

        #endregion

        #region Static Helper

        private static OperateResult<string> AnalysisAddress( string address )
        {
            StringBuilder sb = new StringBuilder( );
            try
            {
                sb.Append( "%" );
                if (address[0] == 'D')
                {
                    sb.Append( "DB" );
                    if (address[1] == 'B')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) );
                    }
                    else if (address[1] == 'W')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 2 );
                    }
                    else if (address[1] == 'D')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 4 );
                    }
                    else
                    {
                        sb.Append( int.Parse( address.Substring( 1 ) ) );
                    }
                }
                else if (address[0] == 'M')
                {
                    sb.Append( "MB" );
                    if (address[1] == 'B')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) );
                    }
                    else if (address[1] == 'W')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 2 );
                    }
                    else if (address[1] == 'D')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 4 );
                    }
                    else
                    {
                        sb.Append( int.Parse( address.Substring( 1 ) ) );
                    }
                }
                else if (address[0] == 'T')
                {
                    sb.Append( "TB" );
                    if (address[1] == 'B')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) );
                    }
                    else if (address[1] == 'W')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 2 );
                    }
                    else if (address[1] == 'D')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 4 );
                    }
                    else
                    {
                        sb.Append( int.Parse( address.Substring( 1 ) ) );
                    }
                }
                else if (address[0] == 'C')
                {
                    sb.Append( "CB" );
                    if (address[1] == 'B')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) );
                    }
                    else if (address[1] == 'W')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 2 );
                    }
                    else if (address[1] == 'D')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 4 );
                    }
                    else
                    {
                        sb.Append( int.Parse( address.Substring( 1 ) ) );
                    }
                }
                else if (address[0] == 'I')
                {
                    sb.Append( "IB" );
                    if (address[1] == 'B')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) );
                    }
                    else if (address[1] == 'W')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 2 );
                    }
                    else if (address[1] == 'D')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 4 );
                    }
                    else
                    {
                        sb.Append( int.Parse( address.Substring( 1 ) ) );
                    }
                }
                else if (address[0] == 'Q')
                {
                    sb.Append( "QB" );
                    if (address[1] == 'B')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) );
                    }
                    else if (address[1] == 'W')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 2 );
                    }
                    else if (address[1] == 'D')
                    {
                        sb.Append( int.Parse( address.Substring( 2 ) ) * 4 );
                    }
                    else
                    {
                        sb.Append( int.Parse( address.Substring( 1 ) ) );
                    }
                }
                else
                {
                    throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<string>( ex.Message );
            }

            return OperateResult.CreateSuccessResult( sb.ToString( ) );
        }

        private static OperateResult<byte[]> BuildReadByteCommand( string address, ushort length )
        {
            var analysisResult = AnalysisAddress( address );
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysisResult );

            byte[] command = new byte[12 + analysisResult.Content.Length];
            command[0] = 0x54;    // read
            command[1] = 0x00;
            command[2] = 0x14;    // continuous reading
            command[3] = 0x00;
            command[4] = 0x00;    // Reserved
            command[5] = 0x00;
            command[6] = 0x01;    // Block No         ?? i don't know what is the meaning
            command[7] = 0x00;
            command[8] = (byte)analysisResult.Content.Length;    //  Variable Length
            command[9] = 0x00;

            Encoding.ASCII.GetBytes( analysisResult.Content ).CopyTo( command, 10 );
            BitConverter.GetBytes( length ).CopyTo( command, command.Length - 2 );

            return OperateResult.CreateSuccessResult( command );
        }

        private static OperateResult<byte[]> BuildWriteByteCommand( string address, byte[] data )
        {
            var analysisResult = AnalysisAddress( address );
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>( analysisResult );

            byte[] command = new byte[12 + analysisResult.Content.Length + data.Length];
            command[0] = 0x58;    // write
            command[1] = 0x00;
            command[2] = 0x14;    // continuous reading
            command[3] = 0x00;
            command[4] = 0x00;    // Reserved
            command[5] = 0x00;
            command[6] = 0x01;    // Block No         ?? i don't know what is the meaning
            command[7] = 0x00;
            command[8] = (byte)analysisResult.Content.Length;    //  Variable Length
            command[9] = 0x00;

            Encoding.ASCII.GetBytes( analysisResult.Content ).CopyTo( command, 10 );
            BitConverter.GetBytes( data.Length ).CopyTo( command, command.Length - 2 - data.Length );
            data.CopyTo( command, command.Length - data.Length );

            return OperateResult.CreateSuccessResult( command );
        }

        /// <summary>
        /// 返回真是的数据内容，支持读写返回
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public OperateResult<byte[]> ExtractActualData( byte[] response )
        {
            if (response.Length < 20) return new OperateResult<byte[]>( "Length is less than 20:" + SoftBasic.ByteToHexString( response ) );

            ushort plcInfo = BitConverter.ToUInt16( response, 10 );
            BitArray array_plcInfo = new BitArray( BitConverter.GetBytes( plcInfo ) );

            switch (plcInfo % 32)
            {
                case 1: CpuType = "XGK/R-CPUH"; break;
                case 2: CpuType = "XGK-CPUS"; break;
                case 5: CpuType = "XGK/R-CPUH"; break;
            }

            CpuError = array_plcInfo[7];
            if (array_plcInfo[8]) LSCpuStatus = LSCpuStatus.RUN;
            if (array_plcInfo[9]) LSCpuStatus = LSCpuStatus.STOP;
            if (array_plcInfo[10]) LSCpuStatus = LSCpuStatus.ERROR;
            if (array_plcInfo[11]) LSCpuStatus = LSCpuStatus.DEBUG;

            if (response.Length < 28) return new OperateResult<byte[]>( "Length is less than 28:" + SoftBasic.ByteToHexString( response ) );
            ushort error = BitConverter.ToUInt16( response, 26 );
            if (error > 0) return new OperateResult<byte[]>( error, "Error:" + SoftBasic.ByteToHexString( response ) );

            if (response[20] == 0x59) return OperateResult.CreateSuccessResult( new byte[0] );  // write

            if (response[20] == 0x55)  // read
            {
                try
                {
                    ushort length = BitConverter.ToUInt16( response, 30 );
                    byte[] content = new byte[length];
                    Array.Copy( response, 32, content, 0, length );
                    return OperateResult.CreateSuccessResult( content );
                }
                catch (Exception ex)
                {
                    return new OperateResult<byte[]>( ex.Message );
                }
            }

            return new OperateResult<byte[]>( StringResources.Language.NotSupportedFunction );
        }

        #endregion

        #region Override

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString( )
        {
            return base.ToString( );
        }

        #endregion
    }
}
