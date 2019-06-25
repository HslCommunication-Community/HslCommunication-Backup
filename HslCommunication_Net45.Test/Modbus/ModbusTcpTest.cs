using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.ModBus;
using HslCommunication;
using HslCommunication.BasicFramework;

namespace HslCommunication_Net45.Test.Modbus
{
    /// <summary>
    /// 主要是测试报文的生成是否正确
    /// </summary>
    [TestClass]
    public class ModbusTcpTest
    {

        [TestMethod]
        public void ModbusTcpUnitTest( )
        {
            ModbusTcpNet modbus = new ModbusTcpNet( "127.0.0.1", 502, 1 );
            if (!modbus.ConnectServer( ).IsSuccess)
            {
                Console.WriteLine( "无法连接modbus，将跳过单元测试。等待网络正常时，再进行测试" );
                return;
            }

            // 开始单元测试，从coil类型开始测试
            string address = "1200";
            bool[] boolTmp = new bool[] { true, true, false, true, false, true, false };
            Assert.IsTrue( modbus.WriteCoil( address, true ).IsSuccess );
            Assert.IsTrue( modbus.ReadCoil( address ).Content == true );
            Assert.IsTrue( modbus.WriteCoil( address, boolTmp ).IsSuccess );
            bool[] readBool = modbus.ReadCoil( address, (ushort)boolTmp.Length ).Content;
            for (int i = 0; i < boolTmp.Length; i++)
            {
                Assert.IsTrue( readBool[i] == boolTmp[i] );
            }

            // bool[]类型
            readBool = modbus.ReadBool( address, (ushort)boolTmp.Length ).Content;
            for (int i = 0; i < boolTmp.Length; i++)
            {
                Assert.IsTrue( readBool[i] == boolTmp[i] );
            }

            address = "300";
            // short类型
            Assert.IsTrue( modbus.Write( address, (short)12345 ).IsSuccess );
            Assert.IsTrue( modbus.ReadInt16( address ).Content == 12345 );
            short[] shortTmp = new short[] { 123, 423, -124, 5313, 2361 };
            Assert.IsTrue( modbus.Write( address, shortTmp ).IsSuccess );
            short[] readShort = modbus.ReadInt16( address, (ushort)shortTmp.Length ).Content;
            for (int i = 0; i < readShort.Length; i++)
            {
                Assert.IsTrue( readShort[i] == shortTmp[i] );
            }

            // ushort类型
            Assert.IsTrue( modbus.Write( address, (ushort)51234 ).IsSuccess );
            Assert.IsTrue( modbus.ReadUInt16( address ).Content == 51234 );
            ushort[] ushortTmp = new ushort[] { 5, 231, 12354, 5313, 12352 };
            Assert.IsTrue( modbus.Write( address, ushortTmp ).IsSuccess );
            ushort[] readUShort = modbus.ReadUInt16( address, (ushort)ushortTmp.Length ).Content;
            for (int i = 0; i < ushortTmp.Length; i++)
            {
                Assert.IsTrue( readUShort[i] == ushortTmp[i] );
            }

            // int类型
            Assert.IsTrue( modbus.Write( address, 12342323 ).IsSuccess );
            Assert.IsTrue( modbus.ReadInt32( address ).Content == 12342323 );
            int[] intTmp = new int[] { 123812512, 123534, 976124, -1286742 };
            Assert.IsTrue( modbus.Write( address, intTmp ).IsSuccess );
            int[] readint = modbus.ReadInt32( address, (ushort)intTmp.Length ).Content;
            for (int i = 0; i < intTmp.Length; i++)
            {
                Assert.IsTrue( readint[i] == intTmp[i] );
            }

            // uint类型
            Assert.IsTrue( modbus.Write( address, (uint)416123237 ).IsSuccess );
            Assert.IsTrue( modbus.ReadUInt32( address ).Content == (uint)416123237 );
            uint[] uintTmp = new uint[] { 81623123, 91712749, 91273123, 123, 21242, 5324 };
            Assert.IsTrue( modbus.Write( address, uintTmp ).IsSuccess );
            uint[] readuint = modbus.ReadUInt32( address, (ushort)uintTmp.Length ).Content;
            for (int i = 0; i < uintTmp.Length; i++)
            {
                Assert.IsTrue( readuint[i] == uintTmp[i] );
            }

            // float类型
            Assert.IsTrue( modbus.Write( address, 123.45f ).IsSuccess );
            Assert.IsTrue( modbus.ReadFloat( address ).Content == 123.45f );
            float[] floatTmp = new float[] { 123, 5343, 1.45f, 563.3f, 586.2f };
            Assert.IsTrue( modbus.Write( address, floatTmp ).IsSuccess );
            float[] readFloat = modbus.ReadFloat( address, (ushort)floatTmp.Length ).Content;
            for (int i = 0; i < readFloat.Length; i++)
            {
                Assert.IsTrue( floatTmp[i] == readFloat[i] );
            }

            // double类型
            Assert.IsTrue( modbus.Write( address, 1234.5434d ).IsSuccess );
            Assert.IsTrue( modbus.ReadDouble( address ).Content == 1234.5434d );
            double[] doubleTmp = new double[] { 1.4213d, 1223d, 452.5342d, 231.3443d };
            Assert.IsTrue( modbus.Write( address, doubleTmp ).IsSuccess );
            double[] readDouble = modbus.ReadDouble( address, (ushort)doubleTmp.Length ).Content;
            for (int i = 0; i < doubleTmp.Length; i++)
            {
                Assert.IsTrue( readDouble[i] == doubleTmp[i] );
            }

            // long类型
            Assert.IsTrue( modbus.Write( address, 123617231235123L ).IsSuccess );
            Assert.IsTrue( modbus.ReadInt64( address ).Content == 123617231235123L );
            long[] longTmp = new long[] { 12312313123L, 1234L, 412323812368L, 1237182361238123 };
            Assert.IsTrue( modbus.Write( address, longTmp ).IsSuccess );
            long[] readLong = modbus.ReadInt64( address, (ushort)longTmp.Length ).Content;
            for (int i = 0; i < longTmp.Length; i++)
            {
                Assert.IsTrue( readLong[i] == longTmp[i] );
            }

            // ulong类型
            Assert.IsTrue( modbus.Write( address, 1283823681236123UL ).IsSuccess );
            Assert.IsTrue( modbus.ReadUInt64( address ).Content == 1283823681236123UL );
            ulong[] ulongTmp = new ulong[] { 21316UL, 1231239127323UL, 1238612361283123UL };
            Assert.IsTrue( modbus.Write( address, ulongTmp ).IsSuccess );
            ulong[] readULong = modbus.ReadUInt64( address, (ushort)ulongTmp.Length ).Content;
            for (int i = 0; i < readULong.Length; i++)
            {
                Assert.IsTrue( readULong[i] == ulongTmp[i] );
            }

            // string类型
            Assert.IsTrue( modbus.Write( address, "123123" ).IsSuccess );
            Assert.IsTrue( modbus.ReadString( address, 3 ).Content == "123123" );

            // byte类型
            byte[] byteTmp = new byte[] { 0x4F, 0x12, 0x72, 0xA7, 0x54, 0xB8 };
            Assert.IsTrue( modbus.Write( address, byteTmp ).IsSuccess );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( modbus.Read( address, 3 ).Content, byteTmp ) );

            modbus.ConnectClose( );
        }

        [TestMethod]
        public void ModbusRtuUnitTest( )
        {
            ModbusRtu modbus = new ModbusRtu( 1 );
            modbus.SerialPortInni( "COM2", 9600 );

            try
            {
                modbus.Open( );
            }
            catch
            {
                Console.WriteLine( "无法连接modbus，将跳过单元测试。等待网络正常时，再进行测试" );
                return;
            }

            // 开始单元测试，从coil类型开始测试
            string address = "1200";
            bool[] boolTmp = new bool[] { true, true, false, true, false, true, false };
            Assert.IsTrue( modbus.WriteCoil( address, true ).IsSuccess );
            Assert.IsTrue( modbus.ReadCoil( address ).Content == true );
            Assert.IsTrue( modbus.WriteCoil( address, boolTmp ).IsSuccess );
            bool[] readBool = modbus.ReadCoil( address, (ushort)boolTmp.Length ).Content;
            for (int i = 0; i < boolTmp.Length; i++)
            {
                Assert.IsTrue( readBool[i] == boolTmp[i] );
            }


            address = "300";
            // short类型
            Assert.IsTrue( modbus.Write( address, (short)12345 ).IsSuccess );
            Assert.IsTrue( modbus.ReadInt16( address ).Content == 12345 );
            short[] shortTmp = new short[] { 123, 423, -124, 5313, 2361 };
            Assert.IsTrue( modbus.Write( address, shortTmp ).IsSuccess );
            short[] readShort = modbus.ReadInt16( address, (ushort)shortTmp.Length ).Content;
            for (int i = 0; i < readShort.Length; i++)
            {
                Assert.IsTrue( readShort[i] == shortTmp[i] );
            }

            // ushort类型
            Assert.IsTrue( modbus.Write( address, (ushort)51234 ).IsSuccess );
            Assert.IsTrue( modbus.ReadUInt16( address ).Content == 51234 );
            ushort[] ushortTmp = new ushort[] { 5, 231, 12354, 5313, 12352 };
            Assert.IsTrue( modbus.Write( address, ushortTmp ).IsSuccess );
            ushort[] readUShort = modbus.ReadUInt16( address, (ushort)ushortTmp.Length ).Content;
            for (int i = 0; i < ushortTmp.Length; i++)
            {
                Assert.IsTrue( readUShort[i] == ushortTmp[i] );
            }

            // int类型
            Assert.IsTrue( modbus.Write( address, 12342323 ).IsSuccess );
            Assert.IsTrue( modbus.ReadInt32( address ).Content == 12342323 );
            int[] intTmp = new int[] { 123812512, 123534, 976124, -1286742 };
            Assert.IsTrue( modbus.Write( address, intTmp ).IsSuccess );
            int[] readint = modbus.ReadInt32( address, (ushort)intTmp.Length ).Content;
            for (int i = 0; i < intTmp.Length; i++)
            {
                Assert.IsTrue( readint[i] == intTmp[i] );
            }

            // uint类型
            Assert.IsTrue( modbus.Write( address, (uint)416123237 ).IsSuccess );
            Assert.IsTrue( modbus.ReadUInt32( address ).Content == (uint)416123237 );
            uint[] uintTmp = new uint[] { 81623123, 91712749, 91273123, 123, 21242, 5324 };
            Assert.IsTrue( modbus.Write( address, uintTmp ).IsSuccess );
            uint[] readuint = modbus.ReadUInt32( address, (ushort)uintTmp.Length ).Content;
            for (int i = 0; i < uintTmp.Length; i++)
            {
                Assert.IsTrue( readuint[i] == uintTmp[i] );
            }

            // float类型
            Assert.IsTrue( modbus.Write( address, 123.45f ).IsSuccess );
            Assert.IsTrue( modbus.ReadFloat( address ).Content == 123.45f );
            float[] floatTmp = new float[] { 123, 5343, 1.45f, 563.3f, 586.2f };
            Assert.IsTrue( modbus.Write( address, floatTmp ).IsSuccess );
            float[] readFloat = modbus.ReadFloat( address, (ushort)floatTmp.Length ).Content;
            for (int i = 0; i < readFloat.Length; i++)
            {
                Assert.IsTrue( floatTmp[i] == readFloat[i] );
            }

            // double类型
            Assert.IsTrue( modbus.Write( address, 1234.5434d ).IsSuccess );
            Assert.IsTrue( modbus.ReadDouble( address ).Content == 1234.5434d );
            double[] doubleTmp = new double[] { 1.4213d, 1223d, 452.5342d, 231.3443d };
            Assert.IsTrue( modbus.Write( address, doubleTmp ).IsSuccess );
            double[] readDouble = modbus.ReadDouble( address, (ushort)doubleTmp.Length ).Content;
            for (int i = 0; i < doubleTmp.Length; i++)
            {
                Assert.IsTrue( readDouble[i] == doubleTmp[i] );
            }

            // long类型
            Assert.IsTrue( modbus.Write( address, 123617231235123L ).IsSuccess );
            Assert.IsTrue( modbus.ReadInt64( address ).Content == 123617231235123L );
            long[] longTmp = new long[] { 12312313123L, 1234L, 412323812368L, 1237182361238123 };
            Assert.IsTrue( modbus.Write( address, longTmp ).IsSuccess );
            long[] readLong = modbus.ReadInt64( address, (ushort)longTmp.Length ).Content;
            for (int i = 0; i < longTmp.Length; i++)
            {
                Assert.IsTrue( readLong[i] == longTmp[i] );
            }

            // ulong类型
            Assert.IsTrue( modbus.Write( address, 1283823681236123UL ).IsSuccess );
            Assert.IsTrue( modbus.ReadUInt64( address ).Content == 1283823681236123UL );
            ulong[] ulongTmp = new ulong[] { 21316UL, 1231239127323UL, 1238612361283123UL };
            Assert.IsTrue( modbus.Write( address, ulongTmp ).IsSuccess );
            ulong[] readULong = modbus.ReadUInt64( address, (ushort)ulongTmp.Length ).Content;
            for (int i = 0; i < readULong.Length; i++)
            {
                Assert.IsTrue( readULong[i] == ulongTmp[i] );
            }

            // string类型
            Assert.IsTrue( modbus.Write( address, "123123" ).IsSuccess );
            Assert.IsTrue( modbus.ReadString( address, 3 ).Content == "123123" );

            // byte类型
            byte[] byteTmp = new byte[] { 0x4F, 0x12, 0x72, 0xA7, 0x54, 0xB8 };
            Assert.IsTrue( modbus.Write( address, byteTmp ).IsSuccess );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( modbus.Read( address, 3 ).Content, byteTmp ) );

            modbus.Close( );
        }

        [TestMethod]
        public void BuildReadCoilCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildReadCoilCommand( "100", 6 );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if(command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x01 &&
                command.Content[7] == 0x01 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x64 &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x06)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }


        [TestMethod]
        public void BuildReadDiscreteCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildReadDiscreteCommand( "s=2;100", 10 );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x02 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x64 &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x0A)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildReadRegisterCommandTest1( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildReadRegisterCommand( "s=2;123", 10 );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x03 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x0A)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }


        [TestMethod]
        public void BuildReadRegisterCommandTest2( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildReadRegisterCommand( "x=4;s=2;123", 10 );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x04 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x0A)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteOneCoilCommandTest1( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteOneCoilCommand( "123", true );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x01 &&
                command.Content[7] == 0x05 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0xFF &&
                command.Content[11] == 0x00)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteOneCoilCommandTest2( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteOneCoilCommand( "s=2;123", false );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x05 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x00)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteOneRegisterCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteOneRegisterCommand( "s=2;123", new byte[] { 0x01, 0x10 } );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x06 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x06 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x01 &&
                command.Content[11] == 0x10)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteCoilCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteCoilCommand( "s=2;123", new bool[] { true, false, false, true } );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x08 &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x0F &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x04 &&
                command.Content[12] == 0x01 &&
                command.Content[13] == 0x09)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }

        [TestMethod]
        public void BuildWriteRegisterCommandTest( )
        {
            ModbusTcpNet modbusTcp = new ModbusTcpNet( "127.0.0.1" );

            OperateResult<byte[]> command = modbusTcp.BuildWriteRegisterCommand( "s=2;123", new byte[] { 0x12, 0x34, 0x56, 0x78 } );
            Assert.IsTrue( command.IsSuccess, "command create failed" );

            if (command.Content[2] == 0x00 &&
                command.Content[3] == 0x00 &&
                command.Content[4] == 0x00 &&
                command.Content[5] == 0x0B &&
                command.Content[6] == 0x02 &&
                command.Content[7] == 0x10 &&
                command.Content[8] == 0x00 &&
                command.Content[9] == 0x7B &&
                command.Content[10] == 0x00 &&
                command.Content[11] == 0x02 &&
                command.Content[12] == 0x04 &&
                command.Content[13] == 0x12 &&
                command.Content[14] == 0x34 &&
                command.Content[15] == 0x56 &&
                command.Content[16] == 0x78)
            {

            }
            else
            {
                Assert.Fail( "command check failed : " + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( command.Content, ' ' ) );
            }
        }
    }
}
