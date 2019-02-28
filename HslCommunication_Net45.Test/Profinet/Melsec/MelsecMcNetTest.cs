using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.Profinet.Melsec;

namespace HslCommunication_Net45.Test.Profinet.Melsec
{
    [TestClass]
    public class MelsecMcNetTest
    {
        [TestMethod]
        public void MelsecUnitTest( )
        {
            MelsecMcNet plc = new MelsecMcNet( "192.168.8.13", 6001 );
            if (!plc.ConnectServer( ).IsSuccess)
            {
                Console.WriteLine( "无法连接PLC，将跳过单元测试。等待网络正常时，再进行测试" );
                return;
            }

            // 开始单元测试
             

        }

    }
}
