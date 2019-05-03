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
        public static int commCountNeed;
        public byte[] commRecvBuf = new byte[MAX_RECV_BUF + 1];
        public byte[] commSendBuf = new byte[MAX_SEND_BUF + 1];
        public static int commCountSend;
        public static int SOH = 0x1;
        public static int STX = 0x2; // start of text
        public static int ETX = 0x3; // end of text
        public static int EOT = 0x4; // end of transmission
        public static int ENQ = 0x5; // enquiry
        public static int ACK = 0x6; // acknowledge
        public static int LF = 0xA; // line feed
        public static int CR = 0xD; // carriage return
        public static int DLE = 0x10;
        public static int NAK = 0x15; // negative acknowledge
        public static int MAX_SEND_BUF = 256;
        public static int MAX_RECV_BUF = 1024;



        public enum DTYPE
        {
            DATA_TYPE_BIT,
            DATA_TYPE_WORD,
            DATA_TYPE_DWORD
        } // WORD ' WORD
        /// <summary>
        /// Take the address type PW  MW   
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="data_type"></param>
        /// <returns></returns>
        public static int GetDataType(string Type, ref int data_type)
        {
            if (string.Compare(Type, "PW") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_WORD);
            }
            else if (string.Compare(Type, "MW") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_WORD);
            }
            else if (string.Compare(Type, "LW") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_WORD);
            }
            else if (string.Compare(Type, "KW") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_WORD);
            }
            else if (string.Compare(Type, "FW") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_WORD);
            }
            else if (string.Compare(Type, "TW") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_WORD);
            }
            else if (string.Compare(Type, "CW") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_WORD);
            }
            else if (string.Compare(Type, "DW") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_WORD);
            }
            else if (string.Compare(Type, "SW") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_WORD);

            }
            else if (string.Compare(Type, "PX") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_BIT);
            }
            else if (string.Compare(Type, "MX") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_BIT);
            }
            else if (string.Compare(Type, "LX") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_BIT);
            }
            else if (string.Compare(Type, "KX") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_BIT);
            }
            else if (string.Compare(Type, "FX") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_BIT);
            }
            else if (string.Compare(Type, "TX") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_BIT);
            }
            else if (string.Compare(Type, "CX") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_BIT);

            }
            else if (string.Compare(Type, "PD") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_DWORD);
            }
            else if (string.Compare(Type, "MD") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_DWORD);
            }
            else if (string.Compare(Type, "LD") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_DWORD);
            }
            else if (string.Compare(Type, "KD") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_DWORD);
            }
            else if (string.Compare(Type, "FD") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_DWORD);
            }
            else if (string.Compare(Type, "TD") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_DWORD);
            }
            else if (string.Compare(Type, "CD") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_DWORD);
            }
            else if (string.Compare(Type, "DD") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_DWORD);
            }
            else if (string.Compare(Type, "SD") == 0)
            {
                data_type = Convert.ToInt32(DTYPE.DATA_TYPE_DWORD);
            }
            else
            {
                return 0;
            }

            return 1;
        }
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public XGBCnet()
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
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            return null; // to do
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override OperateResult Write(string address, byte[] value)
        {
            return null; // to do
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

        #region Private Member



        #endregion

        #region Static Helper

        /// <summary>
        /// 将命令进行打包传送
        /// </summary>
        /// <param name="mcCommand">mc协议的命令</param>
        /// <param name="station">PLC的站号</param>
        /// <returns>最终的原始报文信息</returns>
        public static byte[] PackCommand(byte[] mcCommand, byte station = 0)
        {
            return null; // to do
        }
        private static OperateResult<string> AnalysisAddress(string address)
        {
            // P,M,L,K,F,T
            // P,M,L,K,F,T,C,D,S
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("%");
                char[] types = new char[] { 'P', 'M', 'L', 'K', 'F', 'T', 'C', 'D', 'S' };
                bool exsist = false;

                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i] == address[0])
                    {
                        sb.Append(types[i]);
                        sb.Append("B");
                        if (address[1] == 'B')
                        {
                            sb.Append(int.Parse(address.Substring(2)));
                        }
                        else if (address[1] == 'W')
                        {
                            sb.Append(int.Parse(address.Substring(2)) * 2);
                        }
                        else if (address[1] == 'D')
                        {
                            sb.Append(int.Parse(address.Substring(2)) * 4);
                        }
                        else
                        {
                            sb.Append(int.Parse(address.Substring(1)));
                        }

                        exsist = true;
                        break;
                    }
                }

                if (!exsist) throw new Exception(StringResources.Language.NotSupportedDataType);
            }
            catch (Exception ex)
            {
                return new OperateResult<string>(ex.Message);
            }

            return OperateResult.CreateSuccessResult(sb.ToString());
        }

        /// <summary>
        /// reading address  Type of ReadByte
        /// </summary>
        /// <param name="station"></param>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static OperateResult<byte[]> BuildReadByteCommand(byte station ,string address, ushort length)
        {
            byte[] commSendBuf = new byte[MAX_SEND_BUF + 1];
            var analysisResult = AnalysisAddress(address);
            if (!analysisResult.IsSuccess) return OperateResult.CreateFailedResult<byte[]>(analysisResult);

            commSendBuf[0] = (byte)ENQ; // STX
            Encoding.ASCII.GetBytes($"{station:X2}").CopyTo(commSendBuf, 1); // status
            Encoding.ASCII.GetBytes("R").CopyTo(commSendBuf, 3); // status
            Encoding.ASCII.GetBytes("SB").CopyTo(commSendBuf, 4); // mode
            Encoding.ASCII.GetBytes($"{ analysisResult.Content.Length: X2}").CopyTo(commSendBuf, 6);
            Encoding.ASCII.GetBytes($"{analysisResult.Content}").CopyTo(commSendBuf, 8);
            commCountSend = 8 + analysisResult.Content.Length;
            Encoding.ASCII.GetBytes($"{length:X2}").CopyTo(commSendBuf,commCountSend); // module no
            commCountSend += 2;
            commSendBuf[commCountSend] = (byte)EOT; // STX
            commCountSend += 1;
            byte[] command = new byte[commCountSend];
            Array.Copy(commSendBuf, 0, command, 0, command.Length);

            return OperateResult.CreateSuccessResult(command);
        }
        /// <summary>
        /// Write-type MX0--
        /// </summary>
        /// <param name="station"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="data_type"></param>
        /// <returns></returns>
        public static byte[] WriteBit(int station, string address, ushort value, int data_type)
        {
            int commCountSend = 0;
            byte[] commSendBuf = new byte[MAX_SEND_BUF + 1];
            byte[] commRecvBuf = new byte[MAX_SEND_BUF + 1];
            string imsi = new string(new char[80]);

            commSendBuf[0] = (byte)ENQ; // STX
            Encoding.ASCII.GetBytes($"{station:X2}").CopyTo(commSendBuf, 1); // status
            Encoding.ASCII.GetBytes(string.Format("WSS", station)).CopyTo(commSendBuf, 3); // status
            Encoding.ASCII.GetBytes($"{(byte)1:X2}").CopyTo(commSendBuf, 6);

            Encoding.ASCII.GetBytes($"{address.Length:X2}").CopyTo(commSendBuf, 8);

            Encoding.ASCII.GetBytes($"{address}").CopyTo(commSendBuf, 10); // module no
            commCountSend = 10 + address.Length;


            if (data_type == 0)
            {
                Encoding.ASCII.GetBytes($"{(byte)value:X2}").CopyTo(commSendBuf, commCountSend); // module no
                commCountSend += 2;
            }
            else if (data_type == 2)
            {
                Encoding.ASCII.GetBytes($"{(uint)value:X8}").CopyTo(commSendBuf, commCountSend); // module no
                commCountSend += 8;
            }
            else // WORD
            {
                Encoding.ASCII.GetBytes($"{value:X4}").CopyTo(commSendBuf, commCountSend); // module no
                commCountSend += 4;
            }
            commSendBuf[commCountSend] = (byte)EOT; // STX
            commCountSend += 1;
            byte[] DATA = new byte[commCountSend];

            Array.Copy(commSendBuf, DATA, commCountSend);

            return DATA;
        }
        /// <summary>
        /// Write-type MW0--DW0---DD00
        /// </summary>
        /// <param name="station"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="data_type"></param>
        /// <returns></returns>
        public static byte[] WriteWord(int station, string address, double value, int data_type)
        {
            int commCountSend = 0;
            byte[] commSendBuf = new byte[MAX_SEND_BUF + 1];
            byte[] commRecvBuf = new byte[MAX_SEND_BUF + 1];
            string imsi = new string(new char[80]);

            commSendBuf[0] = (byte)ENQ; // STX
            Encoding.ASCII.GetBytes($"{station:X2}").CopyTo(commSendBuf, 1); // status
            Encoding.ASCII.GetBytes(string.Format("WSB", station)).CopyTo(commSendBuf, 3); // status

            Encoding.ASCII.GetBytes($"{address.Length:X2}").CopyTo(commSendBuf, 6);
            Encoding.ASCII.GetBytes($"{address}").CopyTo(commSendBuf, 8);
            commCountSend = 8 + address.Length;
            Encoding.ASCII.GetBytes($"{byte.Parse("1"):X2}").CopyTo(commSendBuf, commCountSend);
            commCountSend += 2;
            if (data_type == 0)
            {
                Encoding.ASCII.GetBytes($"{System.Convert.ToByte(Math.Truncate(value)):X2}").CopyTo(commSendBuf, commCountSend); // module no
                commCountSend += 2;
            }
            else if (data_type == 2)
            {
                Encoding.ASCII.GetBytes($"{System.Convert.ToUInt32(Math.Truncate(value)):X8}").CopyTo(commSendBuf, commCountSend); // module no
                commCountSend += 8;
            }
            else // WORD
            {
                Encoding.ASCII.GetBytes($"{System.Convert.ToUInt16(Math.Truncate(value)):X4}").CopyTo(commSendBuf, commCountSend); // module no
                commCountSend += 4;
            }
            commSendBuf[commCountSend] = (byte)EOT; // STX
            commCountSend += 1;



            byte[] DATA = new byte[commCountSend];

            Array.Copy(commSendBuf, DATA, commCountSend);

            return DATA;


        }

        #endregion
    } 
}
