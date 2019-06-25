using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.Profinet.Siemens;
using HslCommunication.BasicFramework;
using HslCommunication;

namespace HslCommunication_Net45.Test.Profinet.Siemens
{
    [TestClass]
    public class SiemensTest
    {

        [TestMethod]
        public async Task SiemensUnitTest( )
        {
            SiemensS7Net plc = new SiemensS7Net( SiemensPLCS.S1200, "192.168.8.12" );// "192.168.8.12"
            if (!plc.ConnectServer( ).IsSuccess)
            {
                Console.WriteLine( "无法连接PLC，将跳过单元测试。等待网络正常时，再进行测试" );
                return;
            }

            // 开始单元测试，从bool类型开始测试
            string address = "M200.4";
            Assert.IsTrue( plc.Write( address, true ).IsSuccess );
            Assert.IsTrue( plc.ReadBool( address ).Content == true );

            address = "M300";
            // short类型
            Assert.IsTrue( plc.Write( address, (short)12345 ).IsSuccess );
            Assert.IsTrue( plc.ReadInt16( address ).Content == 12345 );
            short[] shortTmp = new short[] { 123, 423, -124, 5313, 2361 };
            Assert.IsTrue( plc.Write( address, shortTmp ).IsSuccess );
            short[] readShort = plc.ReadInt16( address, (ushort)shortTmp.Length ).Content;
            for (int i = 0; i < readShort.Length; i++)
            {
                Assert.IsTrue( readShort[i] == shortTmp[i] );
            }

            // 异步short类型
            for (int j = 0; j < 100; j++)
            {
                Assert.IsTrue( (await plc.WriteAsync( address, (short)12345 )).IsSuccess );
                Assert.IsTrue( (await plc.ReadInt16Async( address )).Content == 12345 );
                Assert.IsTrue( (await plc.WriteAsync( address, shortTmp )).IsSuccess );
                readShort = (await plc.ReadInt16Async( address, (ushort)shortTmp.Length )).Content;
                for (int i = 0; i < readShort.Length; i++)
                {
                    Assert.IsTrue( readShort[i] == shortTmp[i] );
                }
            }

            // ushort类型
            Assert.IsTrue( plc.Write( address, (ushort)51234 ).IsSuccess );
            Assert.IsTrue( plc.ReadUInt16( address ).Content == 51234 );
            ushort[] ushortTmp = new ushort[] { 5, 231, 12354, 5313, 12352 };
            Assert.IsTrue( plc.Write( address, ushortTmp ).IsSuccess );
            ushort[] readUShort = plc.ReadUInt16( address, (ushort)ushortTmp.Length ).Content;
            for (int i = 0; i < ushortTmp.Length; i++)
            {
                Assert.IsTrue( readUShort[i] == ushortTmp[i] );
            }

            // int类型
            Assert.IsTrue( plc.Write( address, 12342323 ).IsSuccess );
            Assert.IsTrue( plc.ReadInt32( address ).Content == 12342323 );
            int[] intTmp = new int[] { 123812512, 123534, 976124, -1286742 };
            Assert.IsTrue( plc.Write( address, intTmp ).IsSuccess );
            int[] readint = plc.ReadInt32( address, (ushort)intTmp.Length ).Content;
            for (int i = 0; i < intTmp.Length; i++)
            {
                Assert.IsTrue( readint[i] == intTmp[i] );
            }

            // uint类型
            Assert.IsTrue( plc.Write( address, (uint)416123237 ).IsSuccess );
            Assert.IsTrue( plc.ReadUInt32( address ).Content == (uint)416123237 );
            uint[] uintTmp = new uint[] { 81623123, 91712749, 91273123, 123, 21242, 5324 };
            Assert.IsTrue( plc.Write( address, uintTmp ).IsSuccess );
            uint[] readuint = plc.ReadUInt32( address, (ushort)uintTmp.Length ).Content;
            for (int i = 0; i < uintTmp.Length; i++)
            {
                Assert.IsTrue( readuint[i] == uintTmp[i] );
            }

            // float类型
            Assert.IsTrue( plc.Write( address, 123.45f ).IsSuccess );
            Assert.IsTrue( plc.ReadFloat( address ).Content == 123.45f );
            float[] floatTmp = new float[] { 123, 5343, 1.45f, 563.3f, 586.2f };
            Assert.IsTrue( plc.Write( address, floatTmp ).IsSuccess );
            float[] readFloat = plc.ReadFloat( address, (ushort)floatTmp.Length ).Content;
            for (int i = 0; i < readFloat.Length; i++)
            {
                Assert.IsTrue( floatTmp[i] == readFloat[i] );
            }

            // double类型
            Assert.IsTrue( plc.Write( address, 1234.5434d ).IsSuccess );
            Assert.IsTrue( plc.ReadDouble( address ).Content == 1234.5434d );
            double[] doubleTmp = new double[] { 1.4213d, 1223d, 452.5342d, 231.3443d };
            Assert.IsTrue( plc.Write( address, doubleTmp ).IsSuccess );
            double[] readDouble = plc.ReadDouble( address, (ushort)doubleTmp.Length ).Content;
            for (int i = 0; i < doubleTmp.Length; i++)
            {
                Assert.IsTrue( readDouble[i] == doubleTmp[i] );
            }

            // long类型
            Assert.IsTrue( plc.Write( address, 123617231235123L ).IsSuccess );
            Assert.IsTrue( plc.ReadInt64( address ).Content == 123617231235123L );
            long[] longTmp = new long[] { 12312313123L, 1234L, 412323812368L, 1237182361238123 };
            Assert.IsTrue( plc.Write( address, longTmp ).IsSuccess );
            long[] readLong = plc.ReadInt64( address, (ushort)longTmp.Length ).Content;
            for (int i = 0; i < longTmp.Length; i++)
            {
                Assert.IsTrue( readLong[i] == longTmp[i] );
            }

            // ulong类型
            Assert.IsTrue( plc.Write( address, 1283823681236123UL ).IsSuccess );
            Assert.IsTrue( plc.ReadUInt64( address ).Content == 1283823681236123UL );
            ulong[] ulongTmp = new ulong[] { 21316UL, 1231239127323UL, 1238612361283123UL };
            Assert.IsTrue( plc.Write( address, ulongTmp ).IsSuccess );
            ulong[] readULong = plc.ReadUInt64( address, (ushort)ulongTmp.Length ).Content;
            for (int i = 0; i < readULong.Length; i++)
            {
                Assert.IsTrue( readULong[i] == ulongTmp[i] );
            }

            // string类型
            Assert.IsTrue( plc.Write( address, "123123" ).IsSuccess );
            Assert.IsTrue( plc.ReadString( address ).Content == "123123" );

            // 中文，编码可以自定义
            Assert.IsTrue( plc.Write( address, "测试信息123", Encoding.Unicode ).IsSuccess );
            Assert.IsTrue( plc.ReadString( address, 14, Encoding.Unicode ).Content == "测试信息123" );

            // byte类型
            byte[] byteTmp = new byte[] { 0x4F, 0x12, 0x72, 0xA7, 0x54, 0xB8 };
            Assert.IsTrue( plc.Write( address, byteTmp ).IsSuccess );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( plc.Read( address, 6 ).Content, byteTmp ) );

            // 批量写入测试
            short[] shortValues = new short[50];
            for (int i = 0; i < 50; i++)
            {
                shortValues[i] = (short)(i * 5 - 3);
            }
            Assert.IsTrue( plc.Write( "M300", shortValues ).IsSuccess );

            string[] addresses = new string[50];
            ushort[] lengths = new ushort[50];

            for (int i = 0; i < 50; i++)
            {
                addresses[i] = "M" + (i * 2 + 300);
                lengths[i] = 2;
            }
            OperateResult<byte[]> readBytes = plc.Read( addresses, lengths );

            Assert.IsTrue( readBytes.IsSuccess );
            Assert.IsTrue( readBytes.Content.Length == 100 );
            for (int i = 0; i < 50; i++)
            {
                short shortTmp1 = plc.ByteTransform.TransInt16( readBytes.Content, i * 2 );
                Assert.IsTrue( shortValues[i] == shortTmp1 );
            }

            // 自定义类的测试
            DataTest test = new DataTest( )
            {
                Data1 = 425,
                Data2 = 123.53f,
                Data3 = new byte[] { 2, 4, 6, 8, 100, 123 }
            };
            Assert.IsTrue( plc.Write( test ).IsSuccess );
            Assert.IsTrue( plc.ReadInt16( "M100" ).Content == 425 );
            Assert.IsTrue( plc.ReadFloat( "M200" ).Content == 123.53f );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( plc.Read( "M300", 6 ).Content, test.Data3 ) );
            DataTest test1 = plc.Read<DataTest>( ).Content;
            Assert.IsTrue( test1.Data1 == test.Data1 );
            Assert.IsTrue( test1.Data2 == test.Data2 );
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( test1.Data3, test.Data3 ) );

            // 大数据写入测试
            Assert.IsTrue( plc.Write( "M100", (short)12345 ).IsSuccess );
            Assert.IsTrue( plc.Write( "M500", (short)12345 ).IsSuccess );
            Assert.IsTrue( plc.Write( "M800", (short)12345 ).IsSuccess );
            OperateResult<short[]> readBatchResult = plc.ReadInt16( "M100", 351 );
            Assert.IsTrue( readBatchResult.IsSuccess );
            Assert.IsTrue( readBatchResult.Content[0] == 12345 );
            Assert.IsTrue( readBatchResult.Content[200] == 12345 );
            Assert.IsTrue( readBatchResult.Content[350] == 12345 );

            plc.ConnectClose( );
        }

        private class DataTest
        {
            [HslDeviceAddress( "M100" )]
            public short Data1 { get; set; }

            [HslDeviceAddress( "M200" )]
            public float Data2 { get; set; }

            [HslDeviceAddress( "M300", 6 )]
            public byte[] Data3 { get; set; }
        }
    }
}
