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
    /// Inverter Of PC
    /// </summary>
    public class LS_Bus : SerialDeviceBase<RegularByteTransform>
    {

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public LS_Bus()
        {
            WordLength = 1;
            ByteTransform = new RegularByteTransform();
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
            OperateResult<byte[]> command = BuildReadByteCommand(Station, address, length);
            if (!command.IsSuccess) return command;

            OperateResult<byte[]> read = ReadBase(command.Content);
            if (!read.IsSuccess) return read;

            return ExtractActualData(read.Content, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"LS_Bus";
        }

        #endregion

        #region Static Helper

        /// <summary>
        /// Inverter continuous reading (R)
        /// This is a function of continuous reading of designated amount of PLC data from designated address number.
        /// </summary>
        /// <param name="station">plc station</param>
        /// <param name="address">address, for example: 0100</param>
        /// <param name="length">read length</param>
        /// <returns>command bytes</returns>
        private static OperateResult<byte[]> BuildReadByteCommand(byte station, string address, ushort length)
        {
            var analysisResult = XGBFastEnet.AnalysisAddress(address);
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysisResult);

            List<byte> command = new List<byte>();
            command.Add(0x05);    // ENQ
            command.AddRange(SoftBasic.BuildAsciiBytesFrom(station));
            command.Add(0x52);    // command R  Read inverter variable of Word.
            command.AddRange(Encoding.ASCII.GetBytes(analysisResult.Content));//Address of inverter
            command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)length));
            int sum = 0;
            for (int i = 1; i < command.Count; i++)
            {
                sum += command[i];
            }
            command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)sum));
            command.Add(0x04);    // EOT

         

            return OperateResult.CreateSuccessResult(command.ToArray());
        }

        /// <summary>
        /// Continuous writing to inverter device (W)
        /// </summary>
        /// <param name="station">plc station</param>
        /// <param name="address">address, for example: 0100 </param>
        /// <param name="value">source value</param>
        /// <returns>command bytes</returns>
        private static OperateResult<byte[]> BuildWriteByteCommand(byte station, string address, byte[] value)
        {
            var analysisResult = XGBFastEnet.AnalysisAddress(address);
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysisResult);

            List<byte> command = new List<byte>();
            command.Add(0x05);    // ENQ
            command.AddRange(SoftBasic.BuildAsciiBytesFrom(station));
            command.Add(0x57);    // command W  Write inverter variable of Word.
            command.Add(0x06);    // Device Length
            command.AddRange(Encoding.ASCII.GetBytes(analysisResult.Content));
            command.AddRange(SoftBasic.BytesToAsciiBytes(value));

            int sum = 0;
            for (int i = 1; i < command.Count; i++)
            {
                sum += command[i];
            }
            command.AddRange(SoftBasic.BuildAsciiBytesFrom((byte)sum));

            command.Add(0x04);    // EOT

           

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
