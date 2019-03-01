'''
MIT License

Copyright (c) 2017-2018 Richard.Hu

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
警告：以下代码只能在测试PLC中运行，禁止使用生产现场的PLC来测试，否则，后果自负
Warning: The following code can only be run in the Test plc, prohibit the use of the production site PLC to test, otherwise, the consequences
'''
from HslCommunication import MelsecMcNet
from HslCommunication import SoftBasic
import unittest


class MelsecTest(unittest.TestCase):
    def testAll(self):
        # 下面是单元测试
        plc = MelsecMcNet( "192.168.8.13", 6001 )
        if plc.ConnectServer( ).IsSuccess == False:
            print( "无法连接PLC，将跳过单元测试。等待网络正常时，再进行测试" )
            return

        # 开始单元测试，从bool类型开始测试
        address = "M200"
        boolTmp = [True, True, False, True, False, True, False]
        self.assertTrue( plc.WriteBool( address, True ).IsSuccess )
        self.assertTrue( plc.ReadBool( address ).Content == True )
        self.assertTrue( plc.WriteBool( address, boolTmp ).IsSuccess )
        readBool = plc.ReadBool( address, len(boolTmp) ).Content
        for i in range(len(boolTmp)):
            self.assertTrue( readBool[i] == boolTmp[i] )

        address = "D300"
        # short类型
        self.assertTrue( plc.WriteInt16( address, 12345 ).IsSuccess )
        self.assertTrue( plc.ReadInt16( address ).Content == 12345 )
        shortTmp = [123, 423, -124, 5313, 2361 ]
        self.assertTrue( plc.WriteInt16( address, shortTmp ).IsSuccess )
        readShort = plc.ReadInt16( address, len(shortTmp) ).Content
        for i in range(len(readShort)):
            self.assertTrue( readShort[i] == shortTmp[i] )

        # ushort类型
        self.assertTrue( plc.WriteUInt16( address, 51234 ).IsSuccess )
        self.assertTrue( plc.ReadUInt16( address ).Content == 51234 )
        ushortTmp = [ 5, 231, 12354, 5313, 12352 ]
        self.assertTrue( plc.WriteUInt16( address, ushortTmp ).IsSuccess )
        readUShort = plc.ReadUInt16( address, len(ushortTmp) ).Content
        for i in range(len(readUShort)):
            self.assertTrue( readUShort[i] == ushortTmp[i] )

        # int类型
        self.assertTrue( plc.WriteInt32( address, 12342323 ).IsSuccess )
        self.assertTrue( plc.ReadInt32( address ).Content == 12342323 )
        intTmp = [123812512, 123534, 976124, -1286742]
        self.assertTrue( plc.WriteInt32( address, intTmp ).IsSuccess )
        readint = plc.ReadInt32( address, len(intTmp )).Content
        for i in range(len(intTmp)):
            self.assertTrue( readint[i] == intTmp[i] )

        # uint类型
        self.assertTrue( plc.WriteUInt32( address, 416123237 ).IsSuccess )
        self.assertTrue( plc.ReadUInt32( address ).Content == 416123237 )
        uintTmp = [ 81623123, 91712749, 91273123, 123, 21242, 5324 ]
        self.assertTrue( plc.WriteUInt32( address, uintTmp ).IsSuccess )
        readuint = plc.ReadUInt32( address, len(uintTmp )).Content
        for i in range(len(uintTmp)):
            self.assertTrue( readuint[i] == uintTmp[i] )

        # float类型
        self.assertTrue( plc.WriteFloat( address, 123.45 ).IsSuccess )
        self.assertTrue( round(plc.ReadFloat( address ).Content,2) == 123.45 )
        floatTmp = [ 123, 5343, 1.45, 563.3, 586.2 ]
        self.assertTrue( plc.WriteFloat( address, floatTmp ).IsSuccess )
        readFloat = plc.ReadFloat( address, len(floatTmp )).Content
        for i in range(len(floatTmp)):
            self.assertTrue( floatTmp[i] == round(readFloat[i],2) )

        # double类型
        self.assertTrue( plc.WriteDouble( address, 1234.5434 ).IsSuccess )
        self.assertTrue( plc.ReadDouble( address ).Content == 1234.5434 )
        doubleTmp = [ 1.4213, 1223, 452.5342, 231.3443 ]
        self.assertTrue( plc.WriteDouble( address, doubleTmp ).IsSuccess )
        readDouble = plc.ReadDouble( address, len(doubleTmp )).Content
        for i in range(len(doubleTmp)):
            self.assertTrue( readDouble[i] == doubleTmp[i] )

        # long类型
        self.assertTrue( plc.WriteInt64( address, 123617231235123 ).IsSuccess )
        self.assertTrue( plc.ReadInt64( address ).Content == 123617231235123 )
        longTmp = [12312313123, 1234, 412323812368, 1237182361238123]
        self.assertTrue( plc.WriteInt64( address, longTmp ).IsSuccess )
        readLong = plc.ReadInt64( address, len(longTmp) ).Content
        for i in range(len(longTmp)):
            self.assertTrue( readLong[i] == longTmp[i] )

        # ulong类型
        self.assertTrue( plc.WriteUInt64( address, 1283823681236123 ).IsSuccess )
        self.assertTrue( plc.ReadUInt64( address ).Content == 1283823681236123 )
        ulongTmp = [ 21316, 1231239127323, 1238612361283123 ]
        self.assertTrue( plc.WriteUInt64( address, ulongTmp ).IsSuccess )
        readULong = plc.ReadUInt64( address, len(ulongTmp) ).Content
        for i in range(len(ulongTmp)):
            self.assertTrue( readULong[i] == ulongTmp[i] )

        # string类型
        self.assertTrue( plc.WriteString( address, "123123" ).IsSuccess )
        self.assertTrue( plc.ReadString( address, 3 ).Content == "123123" )

        # byte类型
        byteTmp = bytearray([0x4F, 0x12, 0x72, 0xA7, 0x54, 0xB8])
        self.assertTrue( plc.Write( address, byteTmp ).IsSuccess )
        self.assertTrue( SoftBasic.IsTwoBytesAllEquel( plc.Read( address, 3 ).Content, byteTmp ))


def printReadResult(result, addr):
    if result.IsSuccess:
    	print("success[" + addr + "]   " + str(result.Content))
    else:
    	print("failed[" + addr + "]   " + result.Message)
def printWriteResult(result, addr):
    if result.IsSuccess:
        print("success[" + addr + "]")
    else:
        print("falied[" + addr + "]  " + result.Message)

if __name__ == "__main__":
    print(SoftBasic.GetUniqueStringByGuidAndRandom())
    melsecNet = MelsecMcNet("192.168.8.13",6002)
    if melsecNet.ConnectServer().IsSuccess == False:
        print("connect falied  ")
    else:
        # bool read write test
        melsecNet.WriteBool("M200",True)
        printReadResult(melsecNet.ReadBool("M200"),"M200")

        # bool array read write test
        melsecNet.WriteBool("M300",[True,False,True,True,False])
        printReadResult(melsecNet.ReadBool("M300",5),"M300")

        # int16 read write test
        melsecNet.WriteInt16("D200", 12358)
        printReadResult(melsecNet.ReadInt16("D200"), "D200")

        # int16 read write test
        melsecNet.WriteInt16("D201", -12358)
        printReadResult(melsecNet.ReadInt16("D201"), "D201")

        # uint16 read write test
        melsecNet.WriteUInt16("D202", 52358)
        printReadResult(melsecNet.ReadUInt16("D202"), "D202")

        # int32 read write test
        melsecNet.WriteInt32("D210", 12345678)
        printReadResult(melsecNet.ReadInt32("D210"), "D210")

        # int32 read write test
        melsecNet.WriteInt32("D212", -12345678)
        printReadResult(melsecNet.ReadInt32("D212"), "D212")

        # uint32 read write test
        melsecNet.WriteUInt32("D214", 123456789)
        printReadResult(melsecNet.ReadInt32("D214"), "D214")

        # int64 read write test
        melsecNet.WriteInt64("D220", 12345678901234)
        printReadResult(melsecNet.ReadInt64("D220"), "D220")

        # float read write test
        melsecNet.WriteFloat("D230", 123.456)
        printReadResult(melsecNet.ReadFloat("D230"), "D230")

        # double read write test
        melsecNet.WriteDouble("D240", 123.456789)
        printReadResult(melsecNet.ReadDouble("D240"), "D240")

        # string read write test
        melsecNet.WriteString("D250", '123456')
        printReadResult(melsecNet.ReadString("D250",3), "D250")

        # int16 array read write test
        melsecNet.WriteInt16("D260", [123,456,789,-1234])
        printReadResult(melsecNet.ReadInt16("D260",4), "D260")

        melsecNet.ConnectClose()

        # 启动单元测试
        unittest.main()