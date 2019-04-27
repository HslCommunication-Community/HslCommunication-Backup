using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.LSIS
{
    /// <summary>
    /// XGB Fast Enet I/F module supports open Ethernet. It provides network configuration that is to connect LSIS and other company PLC, PC on network
    /// </summary>
    public class XGBFastEnet : NetworkDeviceBase<LsisFastEnetMessage, RegularByteTransform>
    {
        #region Constractor

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

        #region Private Member

        private byte[] PackCommand(byte[] coreCommand )
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
            command[19] = BitConverter.GetBytes( count )[0];

            coreCommand.CopyTo( command, 20 );
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
