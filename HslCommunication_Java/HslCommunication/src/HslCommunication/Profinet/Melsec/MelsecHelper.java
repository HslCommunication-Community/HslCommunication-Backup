package HslCommunication.Profinet.Melsec;

import HslCommunication.Core.Types.FunctionOperateExOne;
import HslCommunication.Core.Types.OperateResultExOne;
import HslCommunication.Core.Types.OperateResultExTwo;
import HslCommunication.StringResources;
import HslCommunication.Utilities;

/**
 * 所有三菱通讯类的通用辅助工具类，包含了一些通用的静态方法，可以使用本类来获取一些原始的报文信息。详细的操作参见例子
 */
public class MelsecHelper {

    /**
     * 解析A1E协议数据地址
     * @param address 数据地址
     * @return 解析值
     */
    public static OperateResultExTwo<MelsecA1EDataType, Short> McA1EAnalysisAddress( String address )
    {
        OperateResultExTwo<MelsecA1EDataType, Short> result = new OperateResultExTwo<MelsecA1EDataType, Short>();
        try {
            switch (address.charAt(0)) {
                case 'X':
                case 'x': {
                    result.Content1 = MelsecA1EDataType.X;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.X.getFromBase());
                    break;
                }
                case 'Y':
                case 'y': {
                    result.Content1 = MelsecA1EDataType.Y;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.Y.getFromBase());
                    break;
                }
                case 'M':
                case 'm': {
                    result.Content1 = MelsecA1EDataType.M;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.M.getFromBase());
                    break;
                }
                case 'S':
                case 's': {
                    result.Content1 = MelsecA1EDataType.S;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.S.getFromBase());
                    break;
                }
                case 'D':
                case 'd': {
                    result.Content1 = MelsecA1EDataType.D;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.D.getFromBase());
                    break;
                }
                case 'R':
                case 'r': {
                    result.Content1 = MelsecA1EDataType.R;
                    result.Content2 = Short.parseShort(address.substring(1), MelsecA1EDataType.R.getFromBase());
                    break;
                }
                default:
                    throw new Exception("输入的类型不支持，请重新输入");
            }
        } catch (Exception ex) {
            result.Message = "地址格式填写错误：" + ex.getMessage();
            return result;
        }

        result.IsSuccess = true;
        return result;
    }

    /**
     * 解析数据地址
     * @param address 数据地址
     * @return 解析值
     */
    public static OperateResultExTwo<MelsecMcDataType, Integer> McAnalysisAddress( String address )
    {
        OperateResultExTwo<MelsecMcDataType, Integer> result = new OperateResultExTwo<MelsecMcDataType, Integer>( );
        try
        {
            switch (address.charAt(0))
            {
                case 'M':
                case 'm':
                {
                    result.Content1 = MelsecMcDataType.M;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.M.getFromBase() );
                    break;
                }
                case 'X':
                case 'x':
                {
                    result.Content1 = MelsecMcDataType.X;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.X.getFromBase() );
                    break;
                }
                case 'Y':
                case 'y':
                {
                    result.Content1 = MelsecMcDataType.Y;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Y.getFromBase() );
                    break;
                }
                case 'D':
                case 'd':
                {
                    result.Content1 = MelsecMcDataType.D;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.D.getFromBase() );
                    break;
                }
                case 'W':
                case 'w':
                {
                    result.Content1 = MelsecMcDataType.W;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.W.getFromBase() );
                    break;
                }
                case 'L':
                case 'l':
                {
                    result.Content1 = MelsecMcDataType.L;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.L.getFromBase() );
                    break;
                }
                case 'F':
                case 'f':
                {
                    result.Content1 = MelsecMcDataType.F;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.F.getFromBase() );
                    break;
                }
                case 'V':
                case 'v':
                {
                    result.Content1 = MelsecMcDataType.V;
                    result.Content2 =Integer.parseInt( address.substring( 1 ), MelsecMcDataType.V.getFromBase() );
                    break;
                }
                case 'B':
                case 'b':
                {
                    result.Content1 = MelsecMcDataType.B;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.B.getFromBase() );
                    break;
                }
                case 'R':
                case 'r':
                {
                    result.Content1 = MelsecMcDataType.R;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.R.getFromBase() );
                    break;
                }
                case 'S':
                case 's':
                {
                    if (address.charAt(1) == 'N' || address.charAt(1) == 'n')
                    {
                        result.Content1 = MelsecMcDataType.SN;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.SN.getFromBase() );
                        break;
                    }
                    else if (address.charAt(1) == 'S' || address.charAt(1) == 's')
                    {
                        result.Content1 = MelsecMcDataType.SS;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.SS.getFromBase() );
                        break;
                    }
                    else if (address.charAt(1) == 'C' || address.charAt(1) == 'c')
                    {
                        result.Content1 = MelsecMcDataType.SC;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.SC.getFromBase() );
                        break;
                    }
                    else
                    {
                        result.Content1 = MelsecMcDataType.S;
                        result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.S.getFromBase() );
                        break;
                    }
                }
                case 'Z':
                case 'z':
                {
                    if (address.startsWith( "ZR" ) || address.startsWith( "zr" ))
                    {
                        result.Content1 = MelsecMcDataType.ZR;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.ZR.getFromBase() );
                        break;
                    }
                    else
                    {
                        result.Content1 = MelsecMcDataType.Z;
                        result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Z.getFromBase() );
                        break;
                    }
                }
                case 'T':
                case 't':
                {
                    if (address.charAt(1) == 'N' || address.charAt(1) == 'n')
                    {
                        result.Content1 = MelsecMcDataType.TN;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.TN.getFromBase() );
                        break;
                    }
                    else if (address.charAt(1) == 'S' || address.charAt(1) == 's')
                    {
                        result.Content1 = MelsecMcDataType.TS;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.TS.getFromBase() );
                        break;
                    }
                    else if (address.charAt(1) == 'C' || address.charAt(1) == 'c')
                    {
                        result.Content1 = MelsecMcDataType.TC;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.TC.getFromBase() );
                        break;
                    }
                    else
                    {
                        throw new Exception( StringResources.Language.NotSupportedDataType() );
                    }
                }
                case 'C':
                case 'c':
                {
                    if (address.charAt(1) == 'N' || address.charAt(1) == 'n')
                    {
                        result.Content1 = MelsecMcDataType.CN;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.CN.getFromBase() );
                        break;
                    }
                    else if (address.charAt(1) == 'S' || address.charAt(1) == 's')
                    {
                        result.Content1 = MelsecMcDataType.CS;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.CS.getFromBase() );
                        break;
                    }
                    else if (address.charAt(1) == 'C' || address.charAt(1) == 'c')
                    {
                        result.Content1 = MelsecMcDataType.CC;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.CC.getFromBase() );
                        break;
                    }
                    else
                    {
                        throw new Exception( StringResources.Language.NotSupportedDataType() );
                    }
                }
                default: throw new Exception( StringResources.Language.NotSupportedDataType() );
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.getMessage();
            return result;
        }

        result.IsSuccess = true;
        result.Message = StringResources.Language.SuccessText();
        return result;
    }


    /**
     * 基恩士解析数据地址
     * @param address 数据地址
     * @return 解析值
     */
    public static OperateResultExTwo<MelsecMcDataType, Integer> KeyenceAnalysisAddress( String address )
    {
        OperateResultExTwo<MelsecMcDataType, Integer> result = new OperateResultExTwo<MelsecMcDataType, Integer>( );
        try
        {
            switch (address.charAt(0))
            {
                case 'M':
                case 'm':
                {
                    result.Content1 = MelsecMcDataType.Keyence_M;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Keyence_M.getFromBase() );
                    break;
                }
                case 'X':
                case 'x':
                {
                    result.Content1 = MelsecMcDataType.Keyence_X;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Keyence_X.getFromBase() );
                    break;
                }
                case 'Y':
                case 'y':
                {
                    result.Content1 = MelsecMcDataType.Keyence_Y;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Keyence_Y.getFromBase() );
                    break;
                }
                case 'B':
                case 'b':
                {
                    result.Content1 = MelsecMcDataType.Keyence_B;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Keyence_B.getFromBase() );
                    break;
                }
                case 'L':
                case 'l':
                {
                    result.Content1 = MelsecMcDataType.Keyence_L;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Keyence_L.getFromBase() );
                    break;
                }
                case 'S':
                case 's':
                {
                    if (address.charAt(1) == 'M' || address.charAt(1) == 'm')
                    {
                        result.Content1 = MelsecMcDataType.Keyence_SM;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.Keyence_SM.getFromBase() );
                        break;
                    }
                    else if (address.charAt(1) == 'D' || address.charAt(1) == 'd')
                    {
                        result.Content1 = MelsecMcDataType.Keyence_SD;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.Keyence_SD.getFromBase() );
                        break;
                    }
                    else
                    {
                        throw new Exception( StringResources.Language.NotSupportedDataType() );
                    }
                }
                case 'D':
                case 'd':
                {
                    result.Content1 = MelsecMcDataType.Keyence_D;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Keyence_D.getFromBase() );
                    break;
                }
                case 'R':
                case 'r':
                {
                    result.Content1 = MelsecMcDataType.Keyence_R;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Keyence_R.getFromBase() );
                    break;
                }
                case 'Z':
                case 'z':
                {
                    if (address.charAt(1) == 'R' || address.charAt(1) == 'r')
                    {
                        result.Content1 = MelsecMcDataType.Keyence_ZR;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.Keyence_ZR.getFromBase() );
                        break;
                    }
                    else
                    {
                        throw new Exception( StringResources.Language.NotSupportedDataType() );
                    }
                }
                case 'W':
                case 'w':
                {
                    result.Content1 = MelsecMcDataType.Keyence_W;
                    result.Content2 = Integer.parseInt( address.substring( 1 ), MelsecMcDataType.Keyence_W.getFromBase() );
                    break;
                }
                case 'T':
                case 't':
                {
                    if (address.charAt(1) == 'N' || address.charAt(1) == 'n')
                    {
                        result.Content1 = MelsecMcDataType.Keyence_TN;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.Keyence_TN.getFromBase() );
                        break;
                    }
                    else if (address.charAt(1) == 'S' || address.charAt(1) == 's')
                    {
                        result.Content1 = MelsecMcDataType.Keyence_TS;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.Keyence_TS.getFromBase() );
                        break;
                    }
                    else
                    {
                        throw new Exception( StringResources.Language.NotSupportedDataType() );
                    }
                }
                case 'C':
                case 'c':
                {
                    if (address.charAt(1) == 'N' || address.charAt(1) == 'n')
                    {
                        result.Content1 = MelsecMcDataType.Keyence_CN;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.Keyence_CN.getFromBase() );
                        break;
                    }
                    else if (address.charAt(1) == 'S' || address.charAt(1) == 's')
                    {
                        result.Content1 = MelsecMcDataType.Keyence_CS;
                        result.Content2 = Integer.parseInt( address.substring( 2 ), MelsecMcDataType.Keyence_CS.getFromBase() );
                        break;
                    }
                    else
                    {
                        throw new Exception( StringResources.Language.NotSupportedDataType() );
                    }
                }
                default: throw new Exception( StringResources.Language.NotSupportedDataType() );
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.getMessage();
            return result;
        }

        result.IsSuccess = true;
        result.Message = StringResources.Language.SuccessText();
        return result;
    }


    /**
     * 从字节构建一个ASCII格式的地址字节
     * @param value 字节信息
     * @return ASCII格式的地址
     */
    public static byte[] BuildBytesFromData(byte value )
    {
        return Utilities.getBytes(String.format("%02x",value),"ASCII");
    }


    /**
     * 从short数据构建一个ASCII格式地址字节
     * @param value short值
     * @return ASCII格式的地址
     */
    public static byte[] BuildBytesFromData( short value )
    {
        return Utilities.getBytes(String.format("%04x",value),"ASCII");
    }

    /**
     * 从int数据构建一个ASCII格式地址字节
     * @param value int值
     * @return ASCII格式的地址
     */
    public static byte[] BuildBytesFromData( int value )
    {
        return Utilities.getBytes(String.format("%04x",value),"ASCII");
    }


    /**
     * 从三菱的地址中构建MC协议的6字节的ASCII格式的地址
     * @param address 三菱地址
     * @param type 三菱的数据类型
     * @return 6字节的ASCII格式的地址
     */
    public static byte[] BuildBytesFromAddress( int address, MelsecMcDataType type )
    {
        return Utilities.getBytes(String.format(type.getFromBase() == 10 ? "%06d" : "%06x",address),"ASCII");
    }


    /**
     * 从字节数组构建一个ASCII格式的地址字节
     * @param value 字节信息
     * @return ASCII格式的地址
     */
    public static byte[] BuildBytesFromData( byte[] value )
    {
        byte[] buffer = new byte[value.length * 2];
        for (int i = 0; i < value.length; i++)
        {
            byte[] data = BuildBytesFromData( value[i] );
            buffer[2*i+0] = data[0];
            buffer[2*i+1] = data[1];
        }
        return buffer;
    }


    /**
     * 将0，1，0，1的字节数组压缩成三菱格式的字节数组来表示开关量的
     * @param value 原始的数据字节
     * @return 压缩过后的数据字节
     */
    public static byte[] TransBoolArrayToByteData( byte[] value )
    {
        int length = value.length % 2 == 0 ? value.length / 2 : (value.length / 2) + 1;
        byte[] buffer = new byte[length];

        for (int i = 0; i < length; i++)
        {
            if (value[i * 2 + 0] != 0x00) buffer[i] += 0x10;
            if ((i * 2 + 1) < value.length)
            {
                if (value[i * 2 + 1] != 0x00) buffer[i] += 0x01;
            }
        }

        return buffer;
    }

    /**
     * 将bool的组压缩成三菱格式的字节数组来表示开关量的
     * @param value 原始的数据字节
     * @return 压缩过后的数据字节
     */
    public static byte[] TransBoolArrayToByteData( boolean[] value )
    {
        int length = (value.length + 1) / 2;
        byte[] buffer = new byte[length];

        for (int i = 0; i < length; i++)
        {
            if (value[i * 2 + 0]) buffer[i] += 0x10;
            if ((i * 2 + 1) < value.length)
            {
                if (value[i * 2 + 1]) buffer[i] += 0x01;
            }
        }

        return buffer;
    }


    /**
     * 计算Fx协议指令的和校验信息
     * @param data 字节数据
     * @return 校验之后的数据
     */
    public static byte[] FxCalculateCRC( byte[] data )
    {
        int sum = 0;
        for (int i = 1; i < data.length - 2; i++)
        {
            sum += data[i];
        }
        return BuildBytesFromData( (byte)sum );
    }


    /**
     * 检查指定的和校验是否是正确的
     * @param data 字节数据
     * @return 是否成功
     */
    public static boolean CheckCRC( byte[] data )
    {
        byte[] crc = FxCalculateCRC( data );
        if (crc[0] != data[data.length - 2]) return false;
        if (crc[1] != data[data.length - 1]) return false;
        return true;
    }


    /**
     * 从地址，长度，是否位读取进行创建读取的MC的核心报文
     * @param address 三菱的地址信息，具体格式参照 MelsecMcNet 的注释说明
     * @param length 读取的长度信息
     * @param isBit 是否进行了位读取操作
     * @param analysisAddress 对地址分析的委托方法
     * @return 带有成功标识的报文对象
     */
    public static OperateResultExOne<byte[]> BuildReadMcCoreCommand(String address, short length, boolean isBit, FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>> analysisAddress)
    {
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = analysisAddress.Action( address );
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult( analysis );

        byte[] command = new byte[10];
        command[0] = 0x01;                                               // 批量读取数据命令
        command[1] = 0x04;
        command[2] = isBit ? (byte)0x01 : (byte)0x00;                    // 以点为单位还是字为单位成批读取
        command[3] = 0x00;
        command[4] = (byte) (analysis.Content2 % 256);                   // 起始地址的地位
        command[5] = (byte) (analysis.Content2 / 256 % 256);
        command[6] = (byte) (analysis.Content2 / 256 / 256);
        command[7] = analysis.Content1.getDataCode();                    // 指明读取的数据
        command[8] = (byte)(length % 256);                               // 软元件的长度
        command[9] = (byte)(length / 256);

        return OperateResultExOne.CreateSuccessResult( command );
    }

    /**
     * 从地址，长度，是否位读取进行创建读取Ascii格式的MC的核心报文
     * @param address 三菱的地址信息，具体格式参照 MelsecMcNet 的注释说明
     * @param length 读取的长度信息
     * @param isBit 是否进行了位读取操作
     * @param analysisAddress 对地址分析的委托方法
     * @return 带有成功标识的报文对象
     */
    public static OperateResultExOne<byte[]> BuildAsciiReadMcCoreCommand(String address, short length, boolean isBit, FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>> analysisAddress )
    {
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = analysisAddress.Action( address );
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult( analysis );

        try {
            byte[] command = new byte[20];
            command[0 ] = 0x30;                                                               // 批量读取数据命令
            command[1 ] = 0x34;
            command[2 ] = 0x30;
            command[3 ] = 0x31;
            command[4 ] = 0x30;                                                               // 以点为单位还是字为单位成批读取
            command[5 ] = 0x30;
            command[6 ] = 0x30;
            command[7 ] = isBit ? (byte) 0x31 : (byte) 0x30;
            command[8 ] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[0];          // 软元件类型
            command[9 ] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[1];
            command[10] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[0];            // 起始地址的地位
            command[11] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[1];
            command[12] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[2];
            command[13] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[3];
            command[14] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[4];
            command[15] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[5];
            command[16] = MelsecHelper.BuildBytesFromData(length)[0];                                             // 软元件点数
            command[17] = MelsecHelper.BuildBytesFromData(length)[1];
            command[18] = MelsecHelper.BuildBytesFromData(length)[2];
            command[19] = MelsecHelper.BuildBytesFromData(length)[3];

            return OperateResultExOne.CreateSuccessResult(command);
        }
        catch (Exception ex){
            return new OperateResultExOne<byte[]>(ex.getMessage());
        }
    }

    /**
     * 以字为单位，创建数据写入的核心报文
     * @param address 三菱的地址信息，具体格式参照 MelsecMcNet 的注释说明
     * @param value 实际的原始数据信息
     * @param analysisAddress 对地址分析的委托方法
     * @return 带有成功标识的报文对象
     */
    public static OperateResultExOne<byte[]> BuildWriteWordCoreCommand(String address, byte[] value, FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>> analysisAddress )
    {
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = analysisAddress.Action( address );
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult( analysis );

        if (value == null) value = new byte[0];
        byte[] command = new byte[10 + value.length];
        command[0] = 0x01;                                                        // 批量读取数据命令
        command[1] = 0x14;
        command[2] = 0x00;                                                        // 以字为单位成批读取
        command[3] = 0x00;
        command[4] = (byte) (analysis.Content2 % 256);                            // 起始地址的地位
        command[5] = (byte) (analysis.Content2 / 256 % 256);
        command[6] = (byte) (analysis.Content2 / 256 / 256);
        command[7] = analysis.Content1.getDataCode();                             // 指明写入的数据
        command[8] = (byte)(value.length / 2 % 256);                              // 软元件长度的地位
        command[9] = (byte)(value.length / 2 / 256);
        System.arraycopy(value, 0, command,10,value.length);

        return OperateResultExOne.CreateSuccessResult( command );
    }

    /**
     * 以字为单位，创建ASCII数据写入的核心报文
     * @param address 三菱的地址信息，具体格式参照 MelsecMcNet 的注释说明
     * @param value 实际的原始数据信息
     * @param analysisAddress 对地址分析的委托方法
     * @return 带有成功标识的报文对象
     */
    public static OperateResultExOne<byte[]> BuildAsciiWriteWordCoreCommand(String address, byte[] value, FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>> analysisAddress )
    {
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = analysisAddress.Action( address );
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult( analysis );

        if (value == null) value = new byte[0];
        byte[] buffer = new byte[value.length * 2];
        for (int i = 0; i < value.length / 2; i++)
        {
            short tmpValue = Utilities.getShort(value, i * 2);
            byte[] tmpBuffer = MelsecHelper.BuildBytesFromData( tmpValue );
            System.arraycopy(tmpBuffer, 0, buffer, 4*i, tmpBuffer.length);
        }
        value = buffer;

        try {
            byte[] command = new byte[20 + value.length];
            command[0] = 0x31;                                                                              // 批量写入的命令
            command[1] = 0x34;
            command[2] = 0x30;
            command[3] = 0x31;
            command[4] = 0x30;                                                                              // 子命令
            command[5] = 0x30;
            command[6] = 0x30;
            command[7] = 0x30;
            command[8] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[0];                         // 软元件类型
            command[9] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[1];
            command[10] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[0];     // 起始地址的地位
            command[11] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[1];
            command[12] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[2];
            command[13] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[3];
            command[14] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[4];
            command[15] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[5];
            command[16] = MelsecHelper.BuildBytesFromData((short) (value.length / 4))[0];              // 软元件点数
            command[17] = MelsecHelper.BuildBytesFromData((short) (value.length / 4))[1];
            command[18] = MelsecHelper.BuildBytesFromData((short) (value.length / 4))[2];
            command[19] = MelsecHelper.BuildBytesFromData((short) (value.length / 4))[3];
            System.arraycopy(value, 0, command, 20, value.length);

            return OperateResultExOne.CreateSuccessResult(command);
        }
        catch (Exception ex){
            return new OperateResultExOne<byte[]>(ex.getMessage());
        }
    }

    /**
     * 以位为单位，创建数据写入的核心报文
     * @param address 三菱的地址信息，具体格式参照 MelsecMcNet 的注释说明
     * @param value 原始的bool数组数据
     * @param analysisAddress 对地址分析的委托方法
     * @return 带有成功标识的报文对象
     */
    public static OperateResultExOne<byte[]> BuildWriteBitCoreCommand( String address, boolean[] value, FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>> analysisAddress )
    {
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = analysisAddress.Action( address );
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult( analysis );

        if (value == null) value = new boolean[0];
        byte[] buffer = MelsecHelper.TransBoolArrayToByteData( value );
        byte[] command = new byte[10 + buffer.length];
        command[0] = 0x01;                                                        // 批量写入数据命令
        command[1] = 0x14;
        command[2] = 0x01;                                                        // 以位为单位成批写入
        command[3] = 0x00;
        command[4] = (byte) (analysis.Content2 % 256);                            // 起始地址的地位
        command[5] = (byte) (analysis.Content2 / 256 % 256);
        command[6] = (byte) (analysis.Content2 / 256 / 256);
        command[7] = analysis.Content1.getDataCode();                                  // 指明写入的数据
        command[8] = (byte)(value.length % 256);                                  // 软元件长度的地位
        command[9] = (byte)(value.length / 256);
        System.arraycopy(buffer,0,command,10,buffer.length);

        return OperateResultExOne.CreateSuccessResult( command );
    }

    /**
     * 以位为单位，创建ASCII数据写入的核心报文
     * @param address 三菱的地址信息，具体格式参照 MelsecMcNet 的注释说明
     * @param value 原始的bool数组数据
     * @param analysisAddress 对地址分析的委托方法
     * @return 带有成功标识的报文对象
     */
    public static OperateResultExOne<byte[]> BuildAsciiWriteBitCoreCommand( String address, boolean[] value, FunctionOperateExOne<String, OperateResultExTwo<MelsecMcDataType, Integer>> analysisAddress )
    {
        OperateResultExTwo<MelsecMcDataType, Integer> analysis = analysisAddress.Action( address );
        if (!analysis.IsSuccess) return OperateResultExOne.CreateFailedResult( analysis );

        if (value == null) value = new boolean[0];
        byte[] buffer = new byte[value.length];
        for(int i=0;i<buffer.length;i++){
            buffer[i] = value[i] ? (byte)0x31 : (byte)0x30;
        }

        try {
            byte[] command = new byte[20 + buffer.length];
            command[0] = 0x31;                                                                              // 批量写入的命令
            command[1] = 0x34;
            command[2] = 0x30;
            command[3] = 0x31;
            command[4] = 0x30;                                                                              // 子命令
            command[5] = 0x30;
            command[6] = 0x30;
            command[7] = 0x31;
            command[8] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[0];            // 软元件类型
            command[9] = (analysis.Content1.getAsciiCode().getBytes("ASCII"))[1];
            command[10] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[0];     // 起始地址的地位
            command[11] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[1];
            command[12] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[2];
            command[13] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[3];
            command[14] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[4];
            command[15] = MelsecHelper.BuildBytesFromAddress(analysis.Content2, analysis.Content1)[5];
            command[16] = MelsecHelper.BuildBytesFromData((short) (value.length))[0];                      // 软元件点数
            command[17] = MelsecHelper.BuildBytesFromData((short) (value.length))[1];
            command[18] = MelsecHelper.BuildBytesFromData((short) (value.length))[2];
            command[19] = MelsecHelper.BuildBytesFromData((short) (value.length))[3];
            System.arraycopy(buffer, 0, command, 20, value.length);

            return OperateResultExOne.CreateSuccessResult(command);
        }
        catch (Exception ex){
            return new OperateResultExOne<>(ex.getMessage());
        }
    }

}
