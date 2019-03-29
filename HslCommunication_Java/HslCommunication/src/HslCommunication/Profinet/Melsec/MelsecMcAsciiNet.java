package HslCommunication.Profinet.Melsec;

import HslCommunication.Core.IMessage.MelsecQnA3EAsciiMessage;
import HslCommunication.Core.Net.NetworkBase.NetworkDeviceBase;
import HslCommunication.Core.Transfer.RegularByteTransform;
import HslCommunication.Core.Types.FunctionOperateExOne;
import HslCommunication.Core.Types.OperateResult;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.StringResources;
import HslCommunication.Utilities;


/**
 * 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式
 */
public class MelsecMcAsciiNet extends NetworkDeviceBase<MelsecQnA3EAsciiMessage, RegularByteTransform> {

    /**
     * 实例化三菱的Qna兼容3E帧协议的通讯对象
     */
    public MelsecMcAsciiNet()
    {
        super(MelsecQnA3EAsciiMessage.class, RegularByteTransform.class);
        WordLength = 1;
    }


    /**
     * 实例化一个三菱的Qna兼容3E帧协议的通讯对象
     * @param ipAddress PLC的Ip地址
     * @param port PLC的端口
     */
    public MelsecMcAsciiNet(String ipAddress, int port) {
        super(MelsecQnA3EAsciiMessage.class, RegularByteTransform.class);
        WordLength = 1;
        setIpAddress(ipAddress);
        setPort(port);
    }


    private byte NetworkNumber = 0x00;                       // 网络号
    private byte NetworkStationNumber = 0x00;                // 网络站号

    /**
     * 获取网络号
     *
     * @return
     */
    public byte getNetworkNumber() {
        return NetworkNumber;
    }

    /**
     * 设置网络号
     *
     * @param networkNumber
     */
    public void setNetworkNumber(byte networkNumber) {
        NetworkNumber = networkNumber;
    }

    /**
     * 获取网络站号
     *
     * @return
     */
    public byte getNetworkStationNumber() {
        return NetworkStationNumber;
    }

    /**
     * 设置网络站号
     *
     * @param networkStationNumber
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
        // 地址分析
        OperateResultExOne<byte[]> coreResult = MelsecHelper.BuildAsciiReadMcCoreCommand( address, length, false, new FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>>(){
            @Override
            public OperateResultExTwo<MelsecMcDataType, Integer> Action(String content) {
                return McAnalysisAddress(content);
            }
        });
        if (!coreResult.IsSuccess) return coreResult;

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult.Content, NetworkNumber, NetworkStationNumber ) );
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult( read );

        // 错误代码验证
        short errorCode = (short) Integer.parseInt(Utilities.getString(read.Content,18,4,"ASCII"), 16 );
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

        // 地址分析
        OperateResultExOne<byte[]> coreResult = MelsecHelper.BuildAsciiReadMcCoreCommand( address, length, true, new FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>>(){
            @Override
            public OperateResultExTwo<MelsecMcDataType, Integer> Action(String content) {
                return McAnalysisAddress(content);
            }
        } );
        if (!coreResult.IsSuccess) return OperateResultExOne.CreateFailedResult( coreResult );

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult.Content, NetworkNumber, NetworkStationNumber ) );
        if (!read.IsSuccess) return OperateResultExOne.CreateFailedResult( read );

        // 错误代码验证
        short errorCode = (short) Integer.parseInt(Utilities.getString(read.Content,18,4,"ASCII"), 16 );
        if (errorCode != 0) return new OperateResultExOne<boolean[]>( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 数据解析，需要传入是否使用位的参数
        OperateResultExOne<byte[]> extract = ExtractActualData( read.Content, true );
        if(!extract.IsSuccess) return OperateResultExOne.CreateFailedResult( extract );

        // 转化bool数组
        boolean[] content = new boolean[extract.Content.length];
        for (int i = 0; i < extract.Content.length; i++) {
            content[i] = extract.Content[i] == 0x01;
        }
        return OperateResultExOne.CreateSuccessResult( content );
    }



    /**
     * 从三菱PLC中批量读取位软元件，返回读取结果
     * @param address 起始地址
     * @return 带成功标志的结果数据对象
     */
    public OperateResultExOne<Boolean> ReadBool(String address) {
        OperateResultExOne<boolean[]> read = ReadBool(address, (short) 1);
        if (!read.IsSuccess) return OperateResultExOne.<Boolean>CreateFailedResult(read);

        return OperateResultExOne.<Boolean>CreateSuccessResult(read.Content[0]);
    }


    /**
     * 向PLC写入数据，数据格式为原始的字节类型
     * @param address 起始地址
     * @param value 原始数据
     * @return 结果
     */
    @Override
    public OperateResult Write(String address, byte[] value) {

        // 地址分析
        OperateResultExOne<byte[]> coreResult = MelsecHelper.BuildAsciiWriteWordCoreCommand( address, value, new FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>>(){
            @Override
            public OperateResultExTwo<MelsecMcDataType, Integer> Action(String content) {
                return McAnalysisAddress(content);
            }
        } );
        if (!coreResult.IsSuccess) return coreResult;

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult.Content, NetworkNumber, NetworkStationNumber ) );
        if (!read.IsSuccess) return read;

        // 错误码验证
        short errorCode = (short) Integer.parseInt(Utilities.getString(read.Content,18,4,"ASCII"), 16 );
        if (errorCode != 0) return new OperateResult( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 写入成功
        return OperateResult.CreateSuccessResult( );
    }


    /**
     * 向PLC中位软元件写入bool数组，返回值说明，比如你写入M100,values[0]对应M100
     * @param address 要写入的数据地址
     * @param value 要写入的实际数据，true 或者是 false
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
        // 解析指令
        OperateResultExOne<byte[]> coreResult = MelsecHelper.BuildAsciiWriteBitCoreCommand( address, values, new FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>>(){
            @Override
            public OperateResultExTwo<MelsecMcDataType, Integer> Action(String content) {
                return McAnalysisAddress(content);
            }
        }  );
        if (!coreResult.IsSuccess) return coreResult;

        // 核心交互
        OperateResultExOne<byte[]> read = ReadFromCoreServer( PackMcCommand( coreResult.Content, NetworkNumber, NetworkStationNumber ) );
        if (!read.IsSuccess) return read;

        // 错误码验证
        short errorCode = (short) Integer.parseInt(Utilities.getString(read.Content,18,4,"ASCII"), 16 );
        if (errorCode != 0) return new OperateResult( errorCode, StringResources.Language.MelsecPleaseReferToManulDocument() );

        // 写入成功
        return OperateResult.CreateSuccessResult( );
    }


    /**
     * 返回表示当前对象的字符串
     * @return 字符串
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
    public static byte[] PackMcCommand( byte[] mcCore, byte networkNumber, byte networkStationNumber )
    {
        byte[] plcCommand = new byte[22 + mcCore.length];
        plcCommand[ 0] = 0x35;                                                                        // 副标题
        plcCommand[ 1] = 0x30;
        plcCommand[ 2] = 0x30;
        plcCommand[ 3] = 0x30;
        plcCommand[ 4] = MelsecHelper.BuildBytesFromData( networkNumber )[0];                         // 网络号
        plcCommand[ 5] = MelsecHelper.BuildBytesFromData( networkNumber )[1];
        plcCommand[ 6] = 0x46;                                                                        // PLC编号
        plcCommand[ 7] = 0x46;
        plcCommand[ 8] = 0x30;                                                                        // 目标模块IO编号
        plcCommand[ 9] = 0x33;
        plcCommand[10] = 0x46;
        plcCommand[11] = 0x46;
        plcCommand[12] = MelsecHelper.BuildBytesFromData( networkStationNumber )[0];                  // 目标模块站号
        plcCommand[13] = MelsecHelper.BuildBytesFromData( networkStationNumber )[1];
        plcCommand[14] = MelsecHelper.BuildBytesFromData( (short)(plcCommand.length - 18) )[0];     // 请求数据长度
        plcCommand[15] = MelsecHelper.BuildBytesFromData( (short)(plcCommand.length - 18) )[1];
        plcCommand[16] = MelsecHelper.BuildBytesFromData( (short)(plcCommand.length - 18) )[2];
        plcCommand[17] = MelsecHelper.BuildBytesFromData( (short)(plcCommand.length - 18) )[3];
        plcCommand[18] = 0x30;                                                                        // CPU监视定时器
        plcCommand[19] = 0x30;
        plcCommand[20] = 0x31;
        plcCommand[21] = 0x30;
        System.arraycopy(mcCore, 0, plcCommand, 22, mcCore.length);

        return plcCommand;
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
            byte[] Content = new byte[response.length - 22];
            for (int i = 22; i < response.length; i++)
            {
                if (response[i] == 0x30)
                {
                    Content[i - 22] = 0x00;
                }
                else
                {
                    Content[i - 22] = 0x01;
                }
            }

            return OperateResultExOne.CreateSuccessResult( Content );
        }
        else
        {
            // 字读取
            byte[] Content = new byte[(response.length - 22) / 2];
            for (int i = 0; i < Content.length / 2; i++)
            {
                int tmp = Integer.parseInt( Utilities.getString( response, i * 4 + 22, 4 ,"ASCII"), 16 );
                byte[] buffer = Utilities.getBytes(tmp);

                Content[i*2+0] = buffer[0];
                Content[i*2+1] = buffer[1];
            }

            return OperateResultExOne.CreateSuccessResult( Content );
        }
    }
}
