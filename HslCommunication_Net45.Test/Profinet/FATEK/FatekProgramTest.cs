using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication;
using HslCommunication.Profinet.FATEK;
using HslCommunication.BasicFramework;

namespace HslCommunication_Net45.Test.Profinet.FATEK
{
    [TestClass]
    public class FatekProgramTest
    {
        [TestMethod]
        public void BuildReadBoolTest( )
        {
            OperateResult<byte[]> read = FatekProgram.BuildReadCommand( 1, "X50", 6, true );
            Assert.IsTrue( read.IsSuccess );

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( read.Content, new byte[] { 0x02, 0x30, 0x31, 0x34, 0x34, 0x30, 0x36, 0x58, 0x30, 0x30, 0x35, 0x30, 0x34, 0x45, 0x03 } ) );
        }

        [TestMethod]
        public void BuildWriteBoolTest( )
        {
            OperateResult<byte[]> read = FatekProgram.BuildWriteBoolCommand( 1, "Y0", new bool[] { true, false, false, true } );
            Assert.IsTrue( read.IsSuccess );

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( read.Content, new byte[] { 0x02, 0x30, 0x31, 0x34, 0x35, 0x30, 0x34, 0x59, 0x30, 0x30, 0x30, 0x30,
                0x31, 0x30, 0x30, 0x31, 0x30, 0x42, 0x03 } ) );
        }

        [TestMethod]
        public void BuildReadTest( )
        {
            OperateResult<byte[]> read = FatekProgram.BuildReadCommand( 1, "R12", 3, false );
            Assert.IsTrue( read.IsSuccess );

            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( read.Content, new byte[] { 0x02, 0x30, 0x31, 0x34, 0x36, 0x30, 0x33, 0x52,
                0x30, 0x30, 0x30, 0x31, 0x32, 0x37, 0x35, 0x03 } ) );
        }


        [TestMethod]
        public void BuildWriteTest( )
        {
            OperateResult<byte[]> read = FatekProgram.BuildWriteByteCommand( 1, "Y8", new byte[] { 0xAA, 0xAA, 0x55, 0x55 } );
            Assert.IsTrue( read.IsSuccess );
            
            Assert.IsTrue( SoftBasic.IsTwoBytesEquel( read.Content, new byte[] { 0x02, 0x30, 0x31, 0x34, 0x37, 0x30, 0x32, 0x57, 0x59, 0x30, 0x30, 0x30, 0x38,
                0x41, 0x41, 0x41, 0x41, 0x35, 0x35, 0x35, 0x35, 0x38, 0x30, 0x03 } ) );
        }
    }
}
