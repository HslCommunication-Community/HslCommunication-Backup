using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Melsec
{
    /// <summary>
    /// 三菱PLC的数据类型，此处包含了几个常用的类型
    /// </summary>
    public class MelsecMcDataType
    {
        /// <summary>
        /// 如果您清楚类型代号，可以根据值进行扩展
        /// </summary>
        /// <param name="code">数据类型的代号</param>
        /// <param name="type">0或1，默认为0</param>
        /// <param name="asciiCode">ASCII格式的类型信息</param>
        /// <param name="fromBase">指示地址的多少进制的，10或是16</param>
        public MelsecMcDataType( byte code, byte type, string asciiCode, int fromBase )
        {
            DataCode = code;
            AsciiCode = asciiCode;
            FromBase = fromBase;
            if (type < 2) DataType = type;
        }

        /// <summary>
        /// 类型的代号值
        /// </summary>
        public byte DataCode { get; private set; } = 0x00;

        /// <summary>
        /// 数据的类型，0代表按字，1代表按位
        /// </summary>
        public byte DataType { get; private set; } = 0x00;

        /// <summary>
        /// 当以ASCII格式通讯时的类型描述
        /// </summary>
        public string AsciiCode { get; private set; }

        /// <summary>
        /// 指示地址是10进制，还是16进制的
        /// </summary>
        public int FromBase { get; private set; }

        /// <summary>
        /// X输入继电器
        /// </summary>
        public readonly static MelsecMcDataType X = new MelsecMcDataType( 0x9C, 0x01, "X*", 16 );

        /// <summary>
        /// Y输出继电器
        /// </summary>
        public readonly static MelsecMcDataType Y = new MelsecMcDataType( 0x9D, 0x01, "Y*", 16 );

        /// <summary>
        /// M中间继电器
        /// </summary>
        public readonly static MelsecMcDataType M = new MelsecMcDataType( 0x90, 0x01, "M*", 10 );

        /// <summary>
        /// D数据寄存器
        /// </summary>
        public readonly static MelsecMcDataType D = new MelsecMcDataType( 0xA8, 0x00, "D*", 10 );

        /// <summary>
        /// W链接寄存器
        /// </summary>
        public readonly static MelsecMcDataType W = new MelsecMcDataType( 0xB4, 0x00, "W*", 16 );

        /// <summary>
        /// L锁存继电器
        /// </summary>
        public readonly static MelsecMcDataType L = new MelsecMcDataType( 0x92, 0x01, "L*", 10 );

        /// <summary>
        /// F报警器
        /// </summary>
        public readonly static MelsecMcDataType F = new MelsecMcDataType( 0x93, 0x01, "F*", 10 );

        /// <summary>
        /// V边沿继电器
        /// </summary>
        public readonly static MelsecMcDataType V = new MelsecMcDataType( 0x94, 0x01, "V*", 10 );

        /// <summary>
        /// B链接继电器
        /// </summary>
        public readonly static MelsecMcDataType B = new MelsecMcDataType( 0xA0, 0x01, "B*", 16 );

        /// <summary>
        /// R文件寄存器
        /// </summary>
        public readonly static MelsecMcDataType R = new MelsecMcDataType( 0xAF, 0x00, "R*", 10 );

        /// <summary>
        /// S步进继电器
        /// </summary>
        public readonly static MelsecMcDataType S = new MelsecMcDataType( 0x98, 0x01, "S*", 10 );

        /// <summary>
        /// 变址寄存器
        /// </summary>
        public readonly static MelsecMcDataType Z = new MelsecMcDataType( 0xCC, 0x00, "Z*", 10 );

        /// <summary>
        /// 定时器的当前值
        /// </summary>
        public readonly static MelsecMcDataType TN = new MelsecMcDataType( 0xC2, 0x00, "TN", 10 );

        /// <summary>
        /// 定时器的触点
        /// </summary>
        public readonly static MelsecMcDataType TS = new MelsecMcDataType( 0xC1, 0x01, "TS", 10 );

        /// <summary>
        /// 定时器的线圈
        /// </summary>
        public readonly static MelsecMcDataType TC = new MelsecMcDataType( 0xC0, 0x01, "TC", 10 );

        /// <summary>
        /// 累计定时器的触点
        /// </summary>
        public readonly static MelsecMcDataType SS = new MelsecMcDataType( 0xC7, 0x01, "SS", 10 );

        /// <summary>
        /// 累计定时器的线圈
        /// </summary>
        public readonly static MelsecMcDataType SC = new MelsecMcDataType( 0xC6, 0x01, "SC", 10 );

        /// <summary>
        /// 累计定时器的当前值
        /// </summary>
        public readonly static MelsecMcDataType SN = new MelsecMcDataType( 0xC8, 0x00, "SN", 100 );

        /// <summary>
        /// 计数器的当前值
        /// </summary>
        public readonly static MelsecMcDataType CN = new MelsecMcDataType( 0xC5, 0x00, "CN", 10 );

        /// <summary>
        /// 计数器的触点
        /// </summary>
        public readonly static MelsecMcDataType CS = new MelsecMcDataType( 0xC4, 0x01, "CS", 10 );

        /// <summary>
        /// 计数器的线圈
        /// </summary>
        public readonly static MelsecMcDataType CC = new MelsecMcDataType( 0xC3, 0x01, "CC", 10 );

        /// <summary>
        /// 文件寄存器ZR区
        /// </summary>
        public readonly static MelsecMcDataType ZR = new MelsecMcDataType( 0xB0, 0x00, "ZR", 16 );




        /// <summary>
        /// X输入继电器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_X = new MelsecMcDataType( 0x9C, 0x01, "X*", 16 );
        /// <summary>
        /// Y输出继电器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_Y = new MelsecMcDataType( 0x9D, 0x01, "Y*", 16 );
        /// <summary>
        /// 链接继电器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_B = new MelsecMcDataType( 0xA0, 0x01, "B*", 16 );
        /// <summary>
        /// 内部辅助继电器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_M = new MelsecMcDataType( 0x90, 0x01, "M*", 10 );
        /// <summary>
        /// 锁存继电器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_L = new MelsecMcDataType( 0x92, 0x01, "L*", 10 );
        /// <summary>
        /// 控制继电器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_SM = new MelsecMcDataType( 0x91, 0x01, "SM", 10 );
        /// <summary>
        /// 控制存储器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_SD = new MelsecMcDataType( 0xA9, 0x00, "SD", 10 );
        /// <summary>
        /// 数据存储器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_D = new MelsecMcDataType( 0xA8, 0x00, "D*", 10 );
        /// <summary>
        /// 文件寄存器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_R = new MelsecMcDataType( 0xAF, 0x00, "R*", 10 );
        /// <summary>
        /// 文件寄存器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_ZR = new MelsecMcDataType( 0xB0, 0x00, "ZR", 16 );
        /// <summary>
        /// 链路寄存器
        /// </summary>
        public readonly static MelsecMcDataType Keyence_W = new MelsecMcDataType( 0xB4, 0x00, "W*", 16 );
        /// <summary>
        /// 计时器（当前值）
        /// </summary>
        public readonly static MelsecMcDataType Keyence_TN = new MelsecMcDataType( 0xC2, 0x00, "TN", 10 );
        /// <summary>
        /// 计时器（接点）
        /// </summary>
        public readonly static MelsecMcDataType Keyence_TS = new MelsecMcDataType( 0xC1, 0x01, "TS", 10 );
        /// <summary>
        /// 计数器（当前值）
        /// </summary>
        public readonly static MelsecMcDataType Keyence_CN = new MelsecMcDataType( 0xC5, 0x00, "CN", 10 );
        /// <summary>
        /// 计数器（接点）
        /// </summary>
        public readonly static MelsecMcDataType Keyence_CS = new MelsecMcDataType( 0xC4, 0x01, "CS", 10 );


        /// <summary>
        /// 输入继电器
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_X = new MelsecMcDataType( 0x9C, 0x01, "X*", 10 );
        /// <summary>
        /// 输出继电器
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_Y = new MelsecMcDataType( 0x9D, 0x01, "Y*", 10 );
        /// <summary>
        /// 链接继电器
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_L = new MelsecMcDataType( 0xA0, 0x01, "L*", 10 );
        /// <summary>
        /// 内部继电器
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_R = new MelsecMcDataType( 0x90, 0x01, "R*", 10 );
        /// <summary>
        /// 数据存储器
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_DT = new MelsecMcDataType( 0xA8, 0x00, "D*", 10 );
        /// <summary>
        /// 链接存储器
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_LD = new MelsecMcDataType( 0xB4, 0x00, "W*", 10 );
        /// <summary>
        /// 计时器（当前值）
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_TN = new MelsecMcDataType( 0xC2, 0x00, "TN", 10 );
        /// <summary>
        /// 计时器（接点）
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_TS = new MelsecMcDataType( 0xC1, 0x01, "TS", 10 );
        /// <summary>
        /// 计数器（当前值）
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_CN = new MelsecMcDataType( 0xC5, 0x00, "CN", 10 );
        /// <summary>
        /// 计数器（接点）
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_CS = new MelsecMcDataType( 0xC4, 0x01, "CS", 10 );
        /// <summary>
        /// 特殊链接继电器
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_SM = new MelsecMcDataType( 0x91, 0x01, "SM", 10 );
        /// <summary>
        /// 特殊链接存储器
        /// </summary>
        public readonly static MelsecMcDataType Panasonic_SD = new MelsecMcDataType( 0xA9, 0x00, "SD", 10 );

    }
}
