'''
MIT License

Copyright (c) 2017-2019 Richard.Hu

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
'''
'''
警告：以下代码只能在测试Redis服务器中运行，禁止使用实际的Redis服务器来测试，否则，后果自负
Warning: The following code can only be run on the test Redis server and is prohibited from being tested using the actual Redis server, otherwise the consequences are conceited
'''
import unittest
from HslCommunication import RedisClient

def printReadResult(result, addr):
    if result.IsSuccess:
    	print("success[" + addr + "]   " + str(result.Content))
    else:
    	print("failed[" + addr + "]   "+result.Message)
def printWriteResult(result, addr):
    if result.IsSuccess:
        print("success[" + addr + "]")
    else:
        print("falied[" + addr + "]  " + result.Message)

class RedisTest(unittest.TestCase):
    def testAll(self):
        redisClient = RedisClient( "127.0.0.1", 6379, "" )
        if redisClient.ConnectServer( ).IsSuccess == False:
            print( "Redis Can't Test! " )
            return

        # 开始单元测试
        redisClient.DeleteKey( [ "UnitTest:1#", "UnitTest:2#", "UnitTest:3#" ] )
        self.assertTrue( redisClient.WriteKey( "UnitTest:1#", "123542dasd四个" ).IsSuccess )
        self.assertTrue( redisClient.ReadKey( "UnitTest:1#" ).Content == "123542dasd四个" )
        self.assertTrue( redisClient.DeleteKey( "UnitTest:1#" ).IsSuccess )

        self.assertTrue( redisClient.WriteKeys( [ "UnitTest:1#", "UnitTest:2#", "UnitTest:3#" ], 
        [ "123542dasd四个", "hi晒sdhi", "asdhnoiw地" ] ).IsSuccess )
        readStrings = redisClient.ReadKey( [ "UnitTest:1#", "UnitTest:2#", "UnitTest:3#" ] ).Content
        self.assertTrue( readStrings[0] == "123542dasd四个" )
        self.assertTrue( readStrings[1] == "hi晒sdhi" )
        self.assertTrue( readStrings[2] == "asdhnoiw地" )

        self.assertTrue( redisClient.DeleteKey( [ "UnitTest:1#", "UnitTest:2#", "UnitTest:3#" ] ).Content == 3 )

        self.assertTrue( redisClient.WriteKey( "UnitTest:1#", "123542dasd四个" ).IsSuccess )
        self.assertTrue( redisClient.ExistsKey( "UnitTest:1#" ).Content == 1 )
        self.assertTrue( redisClient.ReadKeyType( "UnitTest:1#" ).Content == "string" )
        self.assertTrue( redisClient.RenameKey( "UnitTest:1#", "UnitTest:2#" ).IsSuccess )
        self.assertTrue( redisClient.DeleteKey( "UnitTest:2#" ).Content == 1 )


        self.assertTrue( redisClient.AppendKey( "UnitTest:1#", "1234567890" ).Content == 10 )
        self.assertTrue( redisClient.ReadKeyRange( "UnitTest:1#", 3, 6 ).Content == "4567" )
        self.assertTrue( redisClient.WriteKeyRange( "UnitTest:1#", "123", 5 ).Content == 10 )
        self.assertTrue( redisClient.ReadKeyLength( "UnitTest:1#" ).Content == 10 )
        self.assertTrue( redisClient.DeleteKey( "UnitTest:1#" ).Content == 1 )

        self.assertTrue( redisClient.IncrementKey( "UnitTest:1#" ).Content == 1 )
        self.assertTrue( redisClient.IncrementKey( "UnitTest:1#", 5 ).Content == 6 )
        self.assertTrue( redisClient.DecrementKey( "UnitTest:1#" ).Content == 5 )
        self.assertTrue( redisClient.DecrementKey( "UnitTest:1#", 5 ).Content == 0 )
        self.assertTrue( redisClient.DeleteKey( "UnitTest:1#" ).Content == 1 )

        self.assertTrue( redisClient.ListLeftPush( "UnitTest:1#", "1234" ).Content == 1 )
        self.assertTrue( redisClient.ListLeftPush( "UnitTest:1#", "a" ).Content == 2 )
        self.assertTrue( redisClient.ListRightPush( "UnitTest:1#", "b" ).Content == 3 )
        self.assertTrue( redisClient.ReadListByIndex( "UnitTest:1#", 2 ).Content == "b" )
        self.assertTrue( redisClient.ListLeftPush( "UnitTest:1#", [ "m", "n", "l" ] ).Content == 6 )
        self.assertTrue( redisClient.ListRightPush( "UnitTest:1#", [ "x", "y", "z" ] ).Content == 9 )
        self.assertTrue( redisClient.ReadListByIndex( "UnitTest:1#", 8 ).Content == "z" )
        self.assertTrue( redisClient.ListLeftPop( "UnitTest:1#" ).Content == "l" )
        self.assertTrue( redisClient.ListRightPop( "UnitTest:1#" ).Content == "z" )
        self.assertTrue( redisClient.GetListLength( "UnitTest:1#" ).Content == 7 )
        self.assertTrue( redisClient.ListSet( "UnitTest:1#", 5, "zxc" ).IsSuccess )
        self.assertTrue( redisClient.ReadListByIndex( "UnitTest:1#", 5 ).Content == "zxc" )
        self.assertTrue( redisClient.ListTrim( "UnitTest:1#", 3, 5 ).IsSuccess )
        self.assertTrue( redisClient.GetListLength( "UnitTest:1#" ).Content == 3 )
        self.assertTrue( redisClient.ListInsertBefore( "UnitTest:1#", "bbb", "b" ).Content == 4 )
        self.assertTrue( redisClient.ReadListByIndex( "UnitTest:1#", 1 ).Content == "bbb" )
        self.assertTrue( redisClient.ListInsertAfter( "UnitTest:1#", "ccc", "b" ).Content == 5 )
        self.assertTrue( redisClient.ReadListByIndex( "UnitTest:1#", 3 ).Content == "ccc" )
        self.assertTrue( redisClient.DeleteKey( "UnitTest:1#" ).Content == 1 )

        self.assertTrue( redisClient.WriteHashKey( "UnitTest:1#", "test1", "1" ).Content == 1 )
        self.assertTrue( redisClient.WriteHashKey( "UnitTest:1#", "test1", "101" ).Content == 0 )
        self.assertTrue( redisClient.WriteHashKeys( "UnitTest:1#", [ "test2", "test3", "test4" ], [ "102", "103", "104" ] ).IsSuccess )
        readStrings = redisClient.ReadHashKeyAll( "UnitTest:1#" ).Content

        self.assertTrue( readStrings[0] == "test1" )
        self.assertTrue( readStrings[1] == "101" )
        self.assertTrue( readStrings[2] == "test2" )
        self.assertTrue( readStrings[3] == "102" )
        self.assertTrue( readStrings[4] == "test3" )
        self.assertTrue( readStrings[5] == "103" )
        self.assertTrue( readStrings[6] == "test4" )
        self.assertTrue( readStrings[7] == "104" )

        self.assertTrue( redisClient.ReadHashKeyLength( "UnitTest:1#" ).Content == 4 )
        self.assertTrue( redisClient.ExistsHashKey( "UnitTest:1#", "test3" ).Content == 1 )
        self.assertTrue( redisClient.ExistsHashKey( "UnitTest:1#", "test10" ).Content == 0 )
        self.assertTrue( redisClient.DeleteKey( "UnitTest:1#" ).Content == 1 )
        
        redisClient.ConnectClose()
if __name__ == "__main__":
    # 简单的使用如下：
    # redisClient = RedisClient("127.0.0.1", 6379, "")
    # redisClient.SetPersistentConnection()
    # printReadResult(redisClient.ReadKey("abcde"), "abcde")

    # 以下是单元测试
    unittest.main()