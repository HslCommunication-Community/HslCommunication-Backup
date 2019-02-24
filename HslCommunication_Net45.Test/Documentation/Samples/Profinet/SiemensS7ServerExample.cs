using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Profinet.Siemens;
using HslCommunication;

namespace HslCommunication_Net45.Test.Documentation.Samples.Profinet
{
    public class SiemensS7ServerExample
    {
        #region UseExample1

        private SiemensS7Server s7NetServer;
        public void S7Start()
        {
            try
            {
                s7NetServer = new SiemensS7Server();
                s7NetServer.ServerStart(102);
            }
            catch(Exception ex)
            {
                Console.Write("Failed:" + HslCommunication.BasicFramework.SoftBasic.GetExceptionMessage(ex));
            }
        }

        #endregion

        #region UseExample2
        
        public void S7Start2()
        {
            try
            {
                s7NetServer = new SiemensS7Server();
                s7NetServer.LogNet = new HslCommunication.LogNet.LogNetSingle("logs.txt");        // 配置日志信息
                s7NetServer.ServerStart(102);
            }
            catch (Exception ex)
            {
                Console.Write("Failed:" + HslCommunication.BasicFramework.SoftBasic.GetExceptionMessage(ex));
            }
        }

        #endregion


        #region UseExample3

        public void S7Start3()
        {
            try
            {
                s7NetServer = new SiemensS7Server();
                s7NetServer.LogNet = new HslCommunication.LogNet.LogNetSingle("logs.txt");        // 配置日志信息
                s7NetServer.SetTrustedIpAddress(new List<string>() { "127.0.0.1" });              // 仅仅限制本机客户端读写
                s7NetServer.ServerStart(102);
            }
            catch (Exception ex)
            {
                Console.Write("Failed:" + HslCommunication.BasicFramework.SoftBasic.GetExceptionMessage(ex));
            }
        }

        #endregion


        #region UseExample4

        public void S7Start4()
        {
            try
            {
                s7NetServer = new SiemensS7Server();
                s7NetServer.LogNet = new HslCommunication.LogNet.LogNetSingle("logs.txt");        // 配置日志信息
                s7NetServer.SetTrustedIpAddress(new List<string>() { "127.0.0.1" });              // 仅仅限制本机客户端读写
                s7NetServer.OnDataReceived += S7NetServer_OnDataReceived;
                s7NetServer.ServerStart(102);
            }
            catch (Exception ex)
            {
                Console.Write("Failed:" + HslCommunication.BasicFramework.SoftBasic.GetExceptionMessage(ex));
            }
        }

        private void S7NetServer_OnDataReceived(object sender, byte[] data)
        {
            Console.WriteLine(HslCommunication.BasicFramework.SoftBasic.ByteToHexString(data, ' ')); // 打印客户端发送的数据
        }

        #endregion

        #region ReadWriteExample

        private void ReadExample()
        {
            // 此处以M100寄存器作为示例
            bool bool_M100_0     = s7NetServer.ReadBool("M100.0").Content;
            byte byte_M100       = s7NetServer.ReadByte("M100").Content;         // 读取M100的值
            short short_M100     = s7NetServer.ReadInt16("M100").Content;        // 读取M100-M101组成的字
            ushort ushort_M100   = s7NetServer.ReadUInt16("M100").Content;       // 读取M100-M101组成的无符号的值
            int int_M100         = s7NetServer.ReadInt32("M100").Content;        // 读取M100-M103组成的有符号的数据
            uint uint_M100       = s7NetServer.ReadUInt32("M100").Content;       // 读取M100-M103组成的无符号的值
            float float_M100     = s7NetServer.ReadFloat("M100").Content;        // 读取M100-M103组成的单精度值
            long long_M100       = s7NetServer.ReadInt64("M100").Content;        // 读取M100-M107组成的大数据值
            ulong ulong_M100     = s7NetServer.ReadUInt64("M100").Content;       // 读取M100-M107组成的无符号大数据
            double double_M100   = s7NetServer.ReadDouble("M100").Content;       // 读取M100-M107组成的双精度值
            string string_M100   = s7NetServer.ReadString("M100", 10).Content;   // 读取M100-M109组成的ASCII字符串数据

            // 读取数组
            short[] short_M100_array      = s7NetServer.ReadInt16("M100", 10).Content;      // 读取M100-M101组成的字
            ushort[] ushort_M100_array    = s7NetServer.ReadUInt16("M100", 10).Content;     // 读取M100-M101组成的无符号的值
            int[] int_M100_array          = s7NetServer.ReadInt32("M100", 10).Content;      // 读取M100-M103组成的有符号的数据
            uint[] uint_M100_array        = s7NetServer.ReadUInt32("M100", 10).Content;     // 读取M100-M103组成的无符号的值
            float[] float_M100_array      = s7NetServer.ReadFloat("M100", 10).Content;      // 读取M100-M103组成的单精度值
            long[] long_M100_array        = s7NetServer.ReadInt64("M100", 10).Content;      // 读取M100-M107组成的大数据值
            ulong[] ulong_M100_array      = s7NetServer.ReadUInt64("M100", 10).Content;     // 读取M100-M107组成的无符号大数据
            double[] double_M100_array    = s7NetServer.ReadDouble("M100", 10).Content;     // 读取M100-M107组成的双精度值
        }

        private void WriteExample()
        {

            // 此处以M100寄存器作为示例
            s7NetServer.Write("M100", true);                       // 写入M100  bool值
            s7NetServer.Write("M100", (byte)123);                  // 写入M100  byte值
            s7NetServer.Write("M100", (short)1234);                // 写入M100  short值
            s7NetServer.Write("M100", (ushort)45678);              // 写入M100  ushort值
            s7NetServer.Write("M100", 1234566);                    // 写入M100  int值
            s7NetServer.Write("M100", (uint)1234566);              // 写入M100  uint值
            s7NetServer.Write("M100", 123.456f);                   // 写入M100  float值
            s7NetServer.Write("M100", 123.456d);                   // 写入M100  double值
            s7NetServer.Write("M100", 123456661235123534L);        // 写入M100  long值
            s7NetServer.Write("M100", 523456661235123534UL);       // 写入M100  ulong值
            s7NetServer.Write("M100", "K123456789");               // 写入M100  string值

            // 读取数组
            s7NetServer.Write("M100", new short[] { 123, 3566, -123 });                                      // 写入M100  short值  ,W3C0,R3C0 效果是一样的
            s7NetServer.Write("M100", new ushort[] { 12242, 42321, 12323 });                                 // 写入M100  ushort值
            s7NetServer.Write("M100", new int[] { 1234312312, 12312312, -1237213 });                         // 写入M100  int值
            s7NetServer.Write("M100", new uint[] { 523123212, 213, 13123 });                                 // 写入M100  uint值
            s7NetServer.Write("M100", new float[] { 123.456f, 35.3f, -675.2f });                             // 写入M100  float值
            s7NetServer.Write("M100", new double[] { 12343.542312d, 213123.123d, -231232.53432d });          // 写入M100  double值
            s7NetServer.Write("M100", new long[] { 1231231242312, 34312312323214, -1283862312631823 });      // 写入M100  long值
            s7NetServer.Write("M100", new ulong[] { 1231231242312, 34312312323214, 9731283862312631823 });   // 写入M100  ulong值

        }

        #endregion

        #region BytesReadWrite
        
        public void ReadExample2()
        {

            OperateResult<byte[]> read = s7NetServer.Read("M100", 8);
            if (read.IsSuccess)
            {
                float temp   = s7NetServer.ByteTransform.TransInt16(read.Content, 0) / 10f;
                float press  = s7NetServer.ByteTransform.TransInt16(read.Content, 2) / 100f;
                int count    = s7NetServer.ByteTransform.TransInt32(read.Content, 2);

                // do something
            }
            else
            {
                // failed
            }
        }

        public void WriteExample2()
        {
            // 拼凑数据，这样的话，一次通讯就完成数据的全部写入
            byte[] buffer = new byte[8];
            s7NetServer.ByteTransform.TransByte((short)1234).CopyTo(buffer, 0);
            s7NetServer.ByteTransform.TransByte((short)2100).CopyTo(buffer, 2);
            s7NetServer.ByteTransform.TransByte(12353423).CopyTo(buffer, 4);

            OperateResult write = s7NetServer.Write("M100", buffer);
            if (write.IsSuccess)
            {
                // success
            }
            else
            {
                // failed
            }

            // 上面的功能等同于三个数据分别写入，下面的方式性能稍微差一点点，几乎看不出来
            // s7NetServer.Write( "M100", (short)1234 );
            // s7NetServer.Write( "M100", (short)2100 );
            // s7NetServer.Write( "M100", 12353423 );
            
        }


        #endregion
    }
}
