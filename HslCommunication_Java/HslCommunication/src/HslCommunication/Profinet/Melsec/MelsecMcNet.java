package HslCommunication.Profinet.Melsec;

import HslCommunication.Core.IMessage.MelsecQnA3EBinaryMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.RegularByteTransform;
import HslCommunication.Core.Types.FunctionOperateExOne;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

/**
 * 三菱的实际数据交互类
 */
public class MelsecMcNet extends NetworkDeviceBase<MelsecQnA3EBinaryMessage,RegularByteTransform> {


    /**
     * 实例化三菱的Qna兼容3E帧协议的通讯对象
     */
    public MelsecMcNet() {
        super(MelsecQnA3EBinaryMessage.class, RegularByteTransform.class);
        WordLength = 1;
    }


    /**
     * 实例化一个三菱的Qna兼容3E帧协议的通讯对象
     *
     * @param ipAddress PLCd的Ip地址
     * @param port      PLC的端口
     */
    public MelsecMcNet(String ipAddress, int port) {
        super(MelsecQnA3EBinaryMessage.class, RegularByteTransform.class);
        WordLength = 1;
        super.setIpAddress(ipAddress);
        super.setPort(port);
    }


    private byte NetworkNumber = 0x00;                       // 网络号
    private byte NetworkStationNumber = 0x00;                // 网络站号

    /**
     * 获取网络号
     *
     * @return 网络号
     */
    public byte getNetworkNumber() {
        return NetworkNumber;
    }

    /**
     * 设置网络号
     *
     * @param networkNumber 网络号
     */
    public void setNetworkNumber(byte networkNumber) {
        NetworkNumber = networkNumber;
    }

    /**
     * 获取网络站号
     *
     * @return 网络站号
     */
    public byte getNetworkStationNumber() {
        return NetworkStationNumber;
    }

    /**
     * 设置网络站号
     *
     * @param networkStationNumber 网络站号
     */
    public void setNetworkStationNumber(byte networkStationNumber) {
        NetworkStationNumber = networkStationNumber;
    }

    /**
     * 分析地址的方法，允许派生类里进行重写操作
     * @param address 地址信息
     * @return 解析后的数据信息
     */
    protected OperateResultExTwo<MelsecMcDataType, Integer> McAnalysisAddress( String address )
    {
        return MelsecHelper.McAnalysisAddress( address );
    }

    /**
     * 从三菱PLC中读取想要的数据，返回读取结果
     * @param address 读取地址，格式为"M100","D100","W1A0"
     * @param length 读取的数据长度，字最大值960，位最大值7168
     * @return 带成功标志的结果数据对象
     */
    @Override
    public OperateResultExOne<byte[]> Read(String address, short length) {
        // 获取指令
        OperateResultExOne<byte[]> command = MelsecHelper.BuildReadMcCoreCommand( address, length, false, new FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>>(){
            @Override
            public OperateResultExTwo<MelsecMcDataType, Integer> Action(String content) {
                return McAnalysisAddress(content);
            }
        });
        if (!command.IsSuccess) return OperateResultExOne.CreateFailedResult( command );

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( PackMcCommand(command.Content,this.NetworkNumber, this.NetworkStationNumber) );
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult( read );

        // 错误代码验证
        int errorCode = Utilities.getShort(read.Content, 9);
        if (errorCode != 0) return new OperateResultExOne<>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 数据解析，需要传入是否使用位的参数
        return ExtractActualData( read.Content, false );
    }




    /**
     * 从三菱PLC中批量读取位软元件，返回读取结果
     * @param address 起始地址
     * @param length 读取的长度
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<boolean[]> ReadBool(String address, short length) {
        // 获取指令
        OperateResultExOne<byte[]> command = MelsecHelper.BuildReadMcCoreCommand( address, length, true, new FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>>(){
            @Override
            public OperateResultExTwo<MelsecMcDataType, Integer> Action(String content) {
                return McAnalysisAddress(content);
            }
        });
        if (!command.IsSuccess) return OperateResultExOne.CreateFailedResult( command );

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( PackMcCommand(command.Content,this.NetworkNumber, this.NetworkStationNumber) );
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult( read );

        // 错误代码验证
        int errorCode = Utilities.getShort(read.Content, 9);
        if (errorCode != 0) return new OperateResultExOne<>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 数据解析，需要传入是否使用位的参数
        OperateResultExOne<byte[]> extract = ExtractActualData( read.Content, true );
        if(!extract.IsSuccess) return OperateResultExOne.CreateFailedResult( extract );

        // 转化bool数组
        boolean[] result = new boolean[extract.Content.length];
        for(int i=0;i<result.length;i++){
            if(extract.Content[i] == 0x01) result[i] = true;
        }
        return OperateResultExOne.CreateSuccessResult( result );
    }



    /**
     * 从三菱PLC中批量读取位软元件，返回读取结果
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Boolean> ReadBool(String address) {
        OperateResultExOne<boolean[]> read = ReadBool(address, (short) 1);
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult(read);

        return OperateResultExOne.CreateSuccessResult(read.Content[0]);
    }





    /**
     * 向PLC写入数据，数据格式为原始的字节类型
     * @param address 起始地址
     * @param value 原始数据
     * @return 结果
     */
    @Override
    public OperateResult Write(String address, byte[] value) {

        OperateResultExOne<byte[]> coreResult = MelsecHelper.BuildWriteWordCoreCommand( address, value, new FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>>(){
            @Override
            public OperateResultExTwo<MelsecMcDataType, Integer> Action(String content) {
                return McAnalysisAddress(content);
            }
        });
        if (!coreResult.IsSuccess) return coreResult;

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult.Content, NetworkNumber, NetworkStationNumber ) );
        if (!read.IsSuccess) return read;

        // 错误码校验
        short ErrorCode = Utilities.getShort(read.Content, 9);
        if (ErrorCode != 0) return new OperateResultExOne<byte[]>( ErrorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 成功
        return OperateResult.CreateSuccessResult( );
    }



    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据，长度为8的倍数
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean value) {
        return Write(address, new boolean[]{value});
    }



    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
     * @param address 要写入的数据地址
     * @param values 要写入的实际数据，可以指定任意的长度
     * @return 返回写入结果
     */
    public OperateResult Write(String address, boolean[] values) {
        OperateResultExOne<byte[]> coreResult = MelsecHelper.BuildWriteBitCoreCommand( address, values, new FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>>(){
            @Override
            public OperateResultExTwo<MelsecMcDataType, Integer> Action(String content) {
                return McAnalysisAddress(content);
            }
        } );
        if (!coreResult.IsSuccess) return coreResult;

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult.Content, NetworkNumber, NetworkStationNumber ) );
        if (!read.IsSuccess) return read;

        // 错误码校验
        short ErrorCode = Utilities.getShort(read.Content, 9);
        if (ErrorCode != 0) return new OperateResultExOne<byte[]>( ErrorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 成功
        return OperateResult.CreateSuccessResult( );
    }


    /**
     * 获取当前对象的字符串标识形式
     * @return 字符串信息
     */
    @Override
    public String toString() {
        return "MelsecMcNet";
    }


    /**
     * 将MC协议的核心报文打包成一个可以直接对PLC进行发送的原始报文
     * @param mcCore MC协议的核心报文
     * @param networkNumber 网络号
     * @param networkStationNumber 网络站号
     * @return 原始报文信息
     */
    public static byte[] PackMcCommand(byte[] mcCore, byte networkNumber, byte networkStationNumber)
    {
        byte[] _PLCCommand = new byte[11 + mcCore.length];
        _PLCCommand[0] = 0x50;                                               // 副标题
        _PLCCommand[1] = 0x00;
        _PLCCommand[2] = networkNumber;                                      // 网络号
        _PLCCommand[3] = (byte) 0xFF;                                               // PLC编号
        _PLCCommand[4] = (byte)0xFF;                                               // 目标模块IO编号
        _PLCCommand[5] = 0x03;
        _PLCCommand[6] = networkStationNumber;                               // 目标模块站号
        _PLCCommand[7] = (byte)((_PLCCommand.length - 9) % 256);             // 请求数据长度
        _PLCCommand[8] = (byte)((_PLCCommand.length - 9) / 256);
        _PLCCommand[9] = 0x0A;                                               // CPU监视定时器
        _PLCCommand[10] = 0x00;
        System.arraycopy(mcCore, 0, _PLCCommand, 11, mcCore.length);

        return _PLCCommand;
    }

    /**
     * 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
     * @param response 反馈的数据内容
     * @param isBit 是否位读取
     * @return 解析后的结果对象
     */
    public static OperateResultExOne<byte[]> ExtractActualData( byte[] response, boolean isBit )
    {
        if (isBit)
        {
            // 位读取
            byte[] Content = new byte[(response.length - 11) * 2];
            for (int i = 11; i < response.length; i++)
            {
                if ((response[i] & 0x10) == 0x10)
                {
                    Content[(i - 11) * 2 + 0] = 0x01;
                }

                if ((response[i] & 0x01) == 0x01)
                {
                    Content[(i - 11) * 2 + 1] = 0x01;
                }
            }

            return OperateResultExOne.CreateSuccessResult( Content );
        }
        else
        {
            // 字读取
            byte[] Content = new byte[response.length - 11];
            System.arraycopy(response,11,Content,0,Content.length);

            return OperateResultExOne.CreateSuccessResult( Content );
        }
    }

}
