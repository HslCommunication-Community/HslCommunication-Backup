using System;
using HslCommunication.Profinet.Melsec;
using HslCommunication.ModBus;
using HslCommunication;
using HslCommunication.Profinet.AllenBradley;
using HslCommunication.Profinet.Siemens;
using System.Threading.Tasks;
using HslCommunication.Enthernet;
using HslCommunication.Enthernet.Redis;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using System.Collections.Generic;

namespace HslCommunicationCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //ModbusTcpNet modbus = new ModbusTcpNet( "127.0.0.1", 503, 1 );
            //bool[] values = modbus.ReadDiscrete( "32", 4 ).Content;
            //if(values == null)
            //{
            //    Console.WriteLine( "值为空，读取不正确！" );
            //}
            //else
            //{
            //    Console.WriteLine( $"[{values[0]},{values[1]},{values[2]},{values[3]}]" );
            //}

            SiemensUnitTest( );

            Console.ReadLine( );

            //Console.WriteLine( System.Globalization.CultureInfo.CurrentCulture.ToString() );


            //NetSimplifyClient AccountSimplifyClient = new NetSimplifyClient( "127.0.0.1", 23456 );
            //OperateResult<NetHandle,string> read = AccountSimplifyClient.ReadCustomerFromServer( 1, "" );
            //if (read.IsSuccess)
            //{
            //    Console.WriteLine( "Handle:" + read.Content1 );
            //    Console.WriteLine( read.Content2 );
            //}
            //else
            //{
            //    Console.WriteLine( "失败：" + read.Message );
            //}

            //RedisSubscribe subscribe = new RedisSubscribe( "127.0.0.1", 6379, new string[] { "WareHouse:HuiBo" } );
            //subscribe.CreatePush( ( m, n ) =>
            //{
            //    Console.WriteLine( DateTime.Now.ToString() + "  Key: " + m );
            //    Console.WriteLine( n );
            //} );

            //RedisClient redisClient = new RedisClient( "127.0.0.1", 6379, "" );
            //redisClient.SetPersistentConnection( );

            //MelsecMcNet melsecMc = new MelsecMcNet( "192.168.8.12", 6001 );
            //melsecMc.SetPersistentConnection( );

            //long countPlcSuccess = 0;
            //long countRedisSuccess = 0;
            //long countPlcFailed = 0;
            //long countRedisFailed = 0;

            //while (true)
            //{
            //    System.Threading.Thread.Sleep( 300 );
            //    OperateResult<int> read = melsecMc.ReadInt32( "D100" );
            //    if (!read.IsSuccess)
            //    {
            //        countPlcFailed++;
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine( $"{DateTime.Now.ToString( )} PLC Read Failed: {read.Message}  PLC成功:{countPlcSuccess}  PLC失败:{countPlcFailed}  Redis成功:{countRedisSuccess}  Redis失败:{countRedisFailed}" );
            //        continue;
            //    };
            //    countPlcSuccess++;

            //    OperateResult write = redisClient.WriteKey( "Test", read.Content.ToString( ) );
            //    if (write.IsSuccess)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Green;
            //        countRedisSuccess++;
            //        Console.WriteLine( $"{DateTime.Now.ToString( )} 存储redis成功!  PLC成功:{countPlcSuccess}  PLC失败:{countPlcFailed}  Redis成功:{countRedisSuccess}  Redis失败:{countRedisFailed}" );
            //    }
            //    else
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        countRedisFailed++;
            //        Console.WriteLine( $"{DateTime.Now.ToString( )} 存储redis失败!{write.Message}  PLC成功:{countPlcSuccess}  PLC失败:{countPlcFailed}  Redis成功:{countRedisSuccess}  Redis失败:{countRedisFailed}" );
            //    }
            //}


            //SimpleHybirdLock hybirdLock = new SimpleHybirdLock( );

            //int[] buffer = new int[1000];
            //DateTime start = DateTime.Now;
            //for (int i = 0; i < 1000000; i++)
            //{
            //    hybirdLock.Enter( );

            //    for (int j = 0; j < buffer.Length - 1; j++)
            //    {
            //        buffer[j] = buffer[j + 1];
            //    }

            //    buffer[999] = i;

            //    hybirdLock.Leave( );
            //}

            //Console.WriteLine( (DateTime.Now - start).TotalMilliseconds );



            //start = DateTime.Now;
            //for (int i = 0; i < 1000000; i++)
            //{
            //    hybirdLock.Enter( );

            //    int[] newbuffer = new int[1000];
            //    Array.Copy( buffer, 0, newbuffer, 0, 999 );
            //    newbuffer[999] = i;
            //    buffer = newbuffer;

            //    hybirdLock.Leave( );
            //}

            //Console.WriteLine( (DateTime.Now - start).TotalMilliseconds );


            //List<int> list = new List<int>( buffer );
            //start = DateTime.Now;
            //for (int i = 0; i < 1000000; i++)
            //{
            //    hybirdLock.Enter( );

            //    list.Add( i );
            //    list.RemoveAt( 0 );

            //    hybirdLock.Leave( );
            //}

            //Console.WriteLine( (DateTime.Now - start).TotalMilliseconds );


            //SharpList<int> sharpList = new SharpList<int>( 1000, true );
            //start = DateTime.Now;
            //for (int i = 0; i < 1000000; i++)
            //{
            //    sharpList.Add( i );
            //}
            //Console.WriteLine( (DateTime.Now - start).TotalMilliseconds );

            //int[] data = sharpList.ToArray( );

            //Console.ReadLine( );

            // =====================================================================================================
            // 并发的订阅测试

            //List<NetPushClient> netPushClients = new List<NetPushClient>( );
            //for (int i = 0; i < 100; i++)
            //{
            //    if (i > 60)
            //        netPushClients.Add( new NetPushClient( "127.0.0.1", 12345, "E" ) );
            //    else
            //        netPushClients.Add( new NetPushClient( "127.0.0.1", 12345, "D" ) );
            //}
            //netPushClients.ForEach( m => m.CreatePush( ( j, k ) =>
            //{
            //    Console.WriteLine( k );
            //} ) );


            //Console.ReadLine( );
            //netPushClients.ForEach( m => m.ClosePush( ) );
        }






        static void MelsecTest( )
        {
            MelsecMcAsciiNet melsec = new MelsecMcAsciiNet( "192.168.1.192", 6000 );
            HslCommunication.OperateResult<short> read = melsec.ReadInt16( "D100" );
            if (read.IsSuccess)
            {
                Console.WriteLine( read.Content );
            }
            else
            {
                Console.WriteLine( read.Message );
            }
        }

        public static async void SiemensUnitTest( )
        {
            SiemensS7Net plc = new SiemensS7Net( SiemensPLCS.S1200, "192.168.8.12" );
            if (!plc.ConnectServer( ).IsSuccess)
            {
                Console.WriteLine( "无法连接PLC，将跳过单元测试。等待网络正常时，再进行测试" );
                return;
            }

            Console.WriteLine( "测试开始" );
            // 开始单元测试，从bool类型开始测试
            string address = "M300";
            // short类型
            short[] shortTmp = new short[] { 123, 423, -124, 5313, 2361 };
            byte[] byteTmp = new byte[] { 0x1A, 0x2B, 0x36, 0x18, 0x52, 0xD1 };

            // 异步short类型
            for (int j = 0; j < 100; j++)
            {
                if (!(await plc.WriteAsync( address, byteTmp )).IsSuccess)
                {
                    Console.WriteLine( "Error!" );
                    return;
                }

                byte[] readBytes = (await plc.ReadAsync( address, (ushort)byteTmp.Length )).Content;
                for (int i = 0; i < readBytes.Length; i++)
                {
                    if (byteTmp[i] != readBytes[i])
                    {
                        Console.WriteLine( "Error!" );
                        return;
                    }
                }

                if (!(await plc.WriteAsync( address, (short)12345 )).IsSuccess)
                {
                    Console.WriteLine( "Error!" );
                    return;
                }
                if ((await plc.ReadInt16Async( address )).Content != 12345)
                {
                    Console.WriteLine( "Error!" );
                    return;
                }
                if (!(await plc.WriteAsync( address, shortTmp )).IsSuccess)
                {
                    Console.WriteLine( "Error!" );
                    return;
                }

                short[] readShort = (await plc.ReadInt16Async( address, (ushort)shortTmp.Length )).Content;
                for (int i = 0; i < readShort.Length; i++)
                {
                    if (readShort[i] != shortTmp[i])
                    {
                        Console.WriteLine( "Error!" );
                        return;
                    }
                }
            }

            plc.ConnectClose( );
            Console.WriteLine( "测试完成" );
        }

    }
}
