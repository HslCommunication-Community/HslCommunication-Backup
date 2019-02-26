using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication.Enthernet.Redis;

namespace HslCommunication_Net45.Test.Enthernet
{
    [TestClass]
    public class RedisClientTest
    {
        [TestMethod]
        public void RedisClientTest1( )
        {
            RedisClient redisClient = new RedisClient( "127.0.0.1", 6379, string.Empty );
            if (!redisClient.ConnectServer( ).IsSuccess) { Console.WriteLine( "Redis Can't Test! " ); return; }

            // 开始单元测试
            redisClient.DeleteKey( "UnitTest:1#" );
            Assert.IsTrue( redisClient.WriteKey( "UnitTest:1#", "123542dasd四个" ).IsSuccess );
            Assert.IsTrue( redisClient.ReadKey( "UnitTest:1#" ).Content == "123542dasd四个" );
            Assert.IsTrue( redisClient.DeleteKey( "UnitTest:1#" ).IsSuccess );

            Assert.IsTrue( redisClient.WriteKey( new string[]
            {
                "UnitTest:1#",
                "UnitTest:2#",
                "UnitTest:3#",
            }, new string[] {
                 "123542dasd四个",
                 "hi晒sdhi",
                 "asdhnoiw地"
            } ).IsSuccess );
            string[] readStrings = redisClient.ReadKey( new string[]
            {
                "UnitTest:1#",
                "UnitTest:2#",
                "UnitTest:3#",
            } ).Content;
            Assert.IsTrue( readStrings[0] == "123542dasd四个" );
            Assert.IsTrue( readStrings[1] == "hi晒sdhi" );
            Assert.IsTrue( readStrings[2] == "asdhnoiw地" );

            Assert.IsTrue( redisClient.DeleteKey( new string[]
            {
                "UnitTest:1#",
                "UnitTest:2#",
                "UnitTest:3#",
            } ).Content == 3 );

            Assert.IsTrue( redisClient.WriteKey( "UnitTest:1#", "123542dasd四个" ).IsSuccess );
            Assert.IsTrue( redisClient.ExistsKey( "UnitTest:1#" ).Content == 1 );
            Assert.IsTrue( redisClient.ReadKeyType( "UnitTest:1#" ).Content == "string" );
            Assert.IsTrue( redisClient.RenameKey( "UnitTest:1#", "UnitTest:2#" ).IsSuccess );
            Assert.IsTrue( redisClient.DeleteKey( "UnitTest:2#" ).Content == 1 );


            Assert.IsTrue( redisClient.AppendKey( "UnitTest:1#", "1234567890" ).Content == 10 );
            Assert.IsTrue( redisClient.ReadKeyRange( "UnitTest:1#", 3, 6 ).Content == "4567" );
            Assert.IsTrue( redisClient.WriteKeyRange( "UnitTest:1#", "123", 5 ).Content == 10 );
            Assert.IsTrue( redisClient.ReadKeyLength( "UnitTest:1#" ).Content == 10 );
            Assert.IsTrue( redisClient.DeleteKey( "UnitTest:1#" ).Content == 1 );

            Assert.IsTrue( redisClient.IncrementKey( "UnitTest:1#" ).Content == 1 );
            Assert.IsTrue( redisClient.IncrementKey( "UnitTest:1#", 5 ).Content == 6 );
            Assert.IsTrue( redisClient.DecrementKey( "UnitTest:1#" ).Content == 5 );
            Assert.IsTrue( redisClient.DecrementKey( "UnitTest:1#", 5 ).Content == 0 );
            Assert.IsTrue( redisClient.DeleteKey( "UnitTest:1#" ).Content == 1 );

            Assert.IsTrue( redisClient.ListLeftPush( "UnitTest:1#", "1234" ).Content == 1 );
            Assert.IsTrue( redisClient.ListLeftPush( "UnitTest:1#", "a" ).Content == 2 );
            Assert.IsTrue( redisClient.ListRightPush( "UnitTest:1#", "b" ).Content == 3 );
            Assert.IsTrue( redisClient.ReadListByIndex( "UnitTest:1#", 2 ).Content == "b" );
            Assert.IsTrue( redisClient.ListLeftPush( "UnitTest:1#", new string[] { "m", "n", "l" } ).Content == 6 );
            Assert.IsTrue( redisClient.ListRightPush( "UnitTest:1#", new string[] { "x", "y", "z" } ).Content == 9 );
            Assert.IsTrue( redisClient.ReadListByIndex( "UnitTest:1#", 8 ).Content == "z" );
            Assert.IsTrue( redisClient.ListLeftPop( "UnitTest:1#" ).Content == "l" );
            Assert.IsTrue( redisClient.ListRightPop( "UnitTest:1#" ).Content == "z" );
            Assert.IsTrue( redisClient.GetListLength( "UnitTest:1#" ).Content == 7 );
            Assert.IsTrue( redisClient.ListSet( "UnitTest:1#", 5, "zxc" ).IsSuccess );
            Assert.IsTrue( redisClient.ReadListByIndex( "UnitTest:1#", 5 ).Content == "zxc" );
            Assert.IsTrue( redisClient.ListTrim( "UnitTest:1#", 3, 5 ).IsSuccess );
            Assert.IsTrue( redisClient.GetListLength( "UnitTest:1#" ).Content == 3 );
            Assert.IsTrue( redisClient.ListInsertBefore( "UnitTest:1#", "bbb", "b" ).Content == 4 );
            Assert.IsTrue( redisClient.ReadListByIndex( "UnitTest:1#", 1 ).Content == "bbb" );
            Assert.IsTrue( redisClient.ListInsertAfter( "UnitTest:1#", "ccc", "b" ).Content == 5 );
            Assert.IsTrue( redisClient.ReadListByIndex( "UnitTest:1#", 3 ).Content == "ccc" );
            Assert.IsTrue( redisClient.DeleteKey( "UnitTest:1#" ).Content == 1 );

            Assert.IsTrue( redisClient.WriteHashKey( "UnitTest:1#", "test1", "1" ).Content == 1 );
            Assert.IsTrue( redisClient.WriteHashKey( "UnitTest:1#", "test1", "101" ).Content == 0 );
            Assert.IsTrue( redisClient.WriteHashKey( "UnitTest:1#", new string[] { "test2", "test3", "test4" }, new string[] { "102", "103", "104" } ).IsSuccess );
            readStrings = redisClient.ReadHashKeyAll( "UnitTest:1#" ).Content;

            Assert.IsTrue( readStrings[0] == "test1" );
            Assert.IsTrue( readStrings[1] == "101" );
            Assert.IsTrue( readStrings[2] == "test2" );
            Assert.IsTrue( readStrings[3] == "102" );
            Assert.IsTrue( readStrings[4] == "test3" );
            Assert.IsTrue( readStrings[5] == "103" );
            Assert.IsTrue( readStrings[6] == "test4" );
            Assert.IsTrue( readStrings[7] == "104" );

            Assert.IsTrue( redisClient.ReadHashKeyLength( "UnitTest:1#" ).Content == 4 );
            Assert.IsTrue( redisClient.ExistsHashKey( "UnitTest:1#", "test3" ).Content == 1 );
            Assert.IsTrue( redisClient.ExistsHashKey( "UnitTest:1#", "test10" ).Content == 0 );
            Assert.IsTrue( redisClient.DeleteKey( "UnitTest:1#" ).Content == 1 );

        }

    }
}
