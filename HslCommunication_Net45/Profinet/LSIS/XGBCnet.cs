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
    /// XGB Cnet I/F module supports Serial Port.
    /// </summary>
    public class XGBCnet : SerialDeviceBase<RegularByteTransform>
    {
        #region Constructor

        /// <summary>
        /// Instantiate a Default object
        /// </summary>
        public XGBCnet()
        {
            WordLength = 2;
            ByteTransform = new RegularByteTransform();
        }

        #endregion

        #region Public Member

        /// <summary>
        /// PLC Station No.
        /// </summary>
        public byte Station { get; set; } = 0x05;

        #endregion

        #region Read Write Byte

        /// <summary>
        /// Read single byte value from plc
        /// </summary>
        /// <param name="address">Start address</param>
        /// <returns>result</returns>
        public OperateResult<byte> ReadByte(string address)
        {
            var read = Read(address, 2);
            if (!read.IsSuccess) return OperateResult.CreateFailedResult<byte>(read);

            return OperateResult.CreateSuccessResult(read.Content[0]);
        }

        /// <summary>
        /// Write single byte value to plc
        /// </summary>
        /// <param name="address">Start address</param>
        /// <param name="value">value</param>
        /// <returns>Whether to write the successful</returns>
        public OperateResult Write(string address, byte value)
        {
            return Write(address, new byte[] { value });
        }

        /// <summary>
        /// WriteCoil
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public OperateResult WriteCoil(string address, bool value)
        {

            return Write(address, new byte[] { (byte)(value ? 0x01 : 0x00) });
        }

        #endregion

        #region Read Write Support

        /// <summary>
        /// Read Bytes From PLC, you should specify the length
        /// </summary>
        /// <param name="address">the address of the data</param>
        /// <param name="length">the length of the data, in byte unit</param>
        /// <returns>result contains whether success.</returns>
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[]> command = BuildReadByteCommand(Station, address, length);
            if (!command.IsSuccess) return command;

            OperateResult<byte[]> read = ReadBase(command.Content);
            if (!read.IsSuccess) return read;

            return ExtractActualData(read.Content, true);
        }

        /// <summary>
        /// Write Data into plc, , you should specify the address
        /// </summary>
        /// <param name="address">the address of the data</param>
        /// <param name="value">source data</param>
        /// <returns>result contains whether success.</returns>
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> command = BuildWriteByteCommand(Station, address, value);
            if (!command.IsSuccess) return command;

            OperateResult<byte[]> read = ReadBase(command.Content);
            if (!read.IsSuccess) return read;

            return ExtractActualData(read.Content, false);
        }

        #endregion

        #region Object Override

        /// <summary>
        /// Returns a string representing the current object
        /// </summary>
        /// <returns>×Ö·û´®ÐÅÏ¢</returns>
        public override string ToString()
        {
            return $"XGBCnet[{PortName}:{BaudRate}]";
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
        private static OperateResult<byte[]> BuildReadByteCommand(byte station, string address, ushort length)
        {
            var analysisResult = XGBFastEnet.AnalysisAddress(address);
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysisResult);

            List<byte> command = new List<byte>();
            command.Add(0x05);    // ENQ
            command.AddRange(SoftBasic.BuildAsciiBytesFrom(station));
            command.Add(0x72);    // command r
            command.Add(0x53);    // command type: SB
            command.Add(0x42);
            command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)analysisResult.Content.Length));
            command.AddRange(Encoding.ASCII.GetBytes(analysisResult.Content));
            command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)length));
            command.Add(0x04);    // EOT

            int sum = 0;
            for (int i = 0; i < command.Count; i++)
            {
                sum += command[i];
            }
            command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)sum));

            return OperateResult.CreateSuccessResult(command.ToArray());
        }
        /// <summary>
        /// One reading address  Type of ReadByte
        /// </summary>
        /// <param name="station">plc station</param>
        /// <param name="address">address, for example: MX100, DW100, TW100</param>
        /// <param name="length">read length</param>
        /// <returns></returns>
        private static OperateResult<byte[]> BuildReadOneCommand(byte station, string address, ushort length)
        {
            var analysisResult = XGBFastEnet.AnalysisAddress(address);
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysisResult);

            List<byte> command = new List<byte>();
            command.Add(0x05);    // ENQ
            command.AddRange(SoftBasic.BuildAsciiBytesFrom(station));
            command.Add(0x72);    // command r
            command.Add(0x53);    // command type: SS
            command.Add(0x53);
            command.Add(0x01);    // Number of blocks
            command.Add(0x00);
            command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)analysisResult.Content.Length));
            command.AddRange(Encoding.ASCII.GetBytes(analysisResult.Content));
            command.Add(0x04);    // EOT

            int sum = 0;
            for (int i = 0; i < command.Count; i++)
            {
                sum += command[i];
            }
            command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)sum));

            return OperateResult.CreateSuccessResult(command.ToArray());
        }



        /// <summary>
        /// write data to address  Type of ReadByte
        /// </summary>
        /// <param name="station">plc station</param>
        /// <param name="address">address, for example: M100, D100, DW100</param>
        /// <param name="value">source value</param>
        /// <returns>command bytes</returns>
        private static OperateResult<byte[]> BuildWriteByteCommand(byte station, string address, byte[] value)
        {
            var analysisResult = XGBFastEnet.AnalysisAddress(address);
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysisResult);
            var DataTypeResult = XGBFastEnet.GetDataTypeToAddress(address);
            if (!DataTypeResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(DataTypeResult);

            List<byte> command = new List<byte>();
            command.Add(0x05);    // ENQ
            command.AddRange(SoftBasic.BuildAsciiBytesFrom(station));
            command.Add(0x77);    // command w
            command.Add(0x53);    // command type: S
            switch (DataTypeResult.Content)
            {
                case "Bit":
                    command.Add(0x53);     // command type: SS
                    command.Add(0x30);    // Number of blocks
                    command.Add(0x31);
                    command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)analysisResult.Content.Length));
                    command.AddRange(Encoding.ASCII.GetBytes(analysisResult.Content));
                    break;
                case "Byte":
                case "Word":
                case "DWord":
                case "LWord":
                case "Continuous":
                    command.Add(0x42);       // command type: SB
                    command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)analysisResult.Content.Length));
                    command.AddRange(Encoding.ASCII.GetBytes(analysisResult.Content));
                    command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)value.Length));
                    break;
                default: break;
            }
            command.AddRange(SoftBasic.BytesToAsciiBytes(value));
            command.Add(0x04);    // EOT
            int sum = 0;
            for (int i = 0; i < command.Count; i++)
            {
                sum += command[i];
            }
            command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)sum));

            return OperateResult.CreateSuccessResult(command.ToArray());
        }

        /// <summary>
        /// Extract actual data form plc response
        /// </summary>
        /// <param name="response">response data</param>
        /// <param name="isRead">read</param>
        /// <returns>result</returns>
        public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isRead)
        {
            try
            {
                if (isRead)
                {
                    if (response[0] == 0x06)
                    {
                        byte[] buffer = new byte[response.Length - 13];
                        Array.Copy(response, 10, buffer, 0, buffer.Length);
                        return OperateResult.CreateSuccessResult(SoftBasic.AsciiBytesToBytes(buffer));
                    }
                    else
                    {
                        byte[] buffer = new byte[response.Length - 9];
                        Array.Copy(response, 6, buffer, 0, buffer.Length);
                        return new OperateResult<byte[]>(BitConverter.ToUInt16(SoftBasic.AsciiBytesToBytes(buffer), 0), "Data:" + SoftBasic.ByteToHexString(response));
                    }
                }
                else
                {
                    if (response[0] == 0x06)
                    {
                        return OperateResult.CreateSuccessResult(new byte[0]);
                    }
                    else
                    {
                        byte[] buffer = new byte[response.Length - 9];
                        Array.Copy(response, 6, buffer, 0, buffer.Length);
                        return new OperateResult<byte[]>(BitConverter.ToUInt16(SoftBasic.AsciiBytesToBytes(buffer), 0), "Data:" + SoftBasic.ByteToHexString(response));
                    }
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>(ex.Message);
            }
        }

        #endregion
    }
}
