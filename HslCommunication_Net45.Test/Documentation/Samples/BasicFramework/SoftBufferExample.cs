using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.BasicFramework;

namespace HslCommunication_Net45.Test.Documentation.Samples.BasicFramework
{
    #region SoftBufferExample1

    public class SoftBufferExample
    {
        private SoftBuffer softBuffer = new SoftBuffer( 1000 );    // 实例化个1000个byte长度的缓冲区
        private Random random = new Random( );


        public void SoftBufferExample1( )
        {
            // 举例设置100-199长度的数据值
            byte[] buffer = new byte[100];
            random.NextBytes( buffer );

            // 本语句是线程安全的，可以在任意的线程进行操作
            softBuffer.SetBytes( buffer, 100 );

            // 然后我们把数据进行读取出来，和上述的buffer是一致的
            byte[] data = softBuffer.GetBytes( 100, 100 );
        }
        
        public void SoftBufferExample2( )
        {
            // 举例设置读取指定的数据类型的方法
            int[] buffer = new int[10];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = random.Next( 10000 );
            }

            // 本语句是线程安全的，可以在任意的线程进行操作
            softBuffer.SetValue( buffer, 100 );

            // 然后我们把数据进行读取出来，和上述的buffer是一致的
            int[] data = softBuffer.GetInt32( 100, 10 );
        }

        // 其他的类型读写也是类似的，如果是自定义类型
        public class UserData : IDataTransfer
        {
            public float temp1 = 0;
            public float temp2 = 0;
            public ushort temp3 = 0;


            public ushort ReadCount => 10;

            public void ParseSource( byte[] Content )
            {
                temp1 = BitConverter.ToSingle( Content, 0 );
                temp2 = BitConverter.ToSingle( Content, 4 );
                temp3 = BitConverter.ToUInt16( Content, 8 );
            }

            public byte[] ToSource( )
            {
                byte[] buffer = new byte[10];
                BitConverter.GetBytes( temp1 ).CopyTo( buffer, 0 );
                BitConverter.GetBytes( temp2 ).CopyTo( buffer, 4 );
                BitConverter.GetBytes( temp3 ).CopyTo( buffer, 8 );
                return buffer;
            }
        }

        public void SoftBufferExample3( )
        {
            // 举例设置读取自定义的数据类型
            UserData userData = new UserData( )
            {
                temp1 = 123.456f,
                temp2 = 1.23f,
                temp3 = 12345,
            };

            // 本语句是线程安全的，可以在任意的线程进行操作
            softBuffer.SetCustomer( userData, 100 );

            // 然后我们把数据进行读取出来，和上述的buffer是一致的
            UserData data = softBuffer.GetCustomer<UserData>( 100 );
        }

    }

    #endregion
}
