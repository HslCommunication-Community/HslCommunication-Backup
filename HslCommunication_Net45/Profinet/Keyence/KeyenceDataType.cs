using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Profinet.Keyence
{
    /// <summary>
    /// Keyence PLC的数据类型，此处包含了几个常用的类型
    /// </summary>
    public class KeyenceDataType
    {
        /// <summary>
        /// 如果您清楚类型代号，可以根据值进行扩展
        /// </summary>
        /// <param name="code">数据类型的代号</param>
        /// <param name="type">0或1，默认为0</param>
        /// <param name="asciiCode">ASCII格式的类型信息</param>
        /// <param name="fromBase">指示地址的多少进制的，10或是16</param>
        public KeyenceDataType(byte code, byte type, string asciiCode, int fromBase)
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
        public readonly static KeyenceDataType  X = new KeyenceDataType(0x9C, 0x01, "X*", 16);
        /// <summary>
        /// Y输出继电器
        /// </summary>
        public readonly static KeyenceDataType  Y = new KeyenceDataType(0x9D, 0x01, "Y*", 16);
        /// <summary>
        /// 链接继电器
        /// </summary>
        public readonly static KeyenceDataType  B = new KeyenceDataType(0xA0, 0x01, "B*", 16);
        /// <summary>
        /// 内部辅助继电器
        /// </summary>
        public readonly static KeyenceDataType  M = new KeyenceDataType(0x90, 0x01, "M*", 10);
        /// <summary>
        /// 锁存继电器
        /// </summary>
        public readonly static KeyenceDataType  L = new KeyenceDataType(0x92, 0x01, "L*", 10);
        /// <summary>
        /// 控制继电器
        /// </summary>
        public readonly static KeyenceDataType  SM = new KeyenceDataType(0x91, 0x01, "SM", 10);
        /// <summary>
        /// 控制存储器
        /// </summary>
        public readonly static KeyenceDataType  SD = new KeyenceDataType(0xA9, 0x00, "SD", 10);
        /// <summary>
        /// 数据存储器
        /// </summary>
        public readonly static KeyenceDataType  D = new KeyenceDataType(0xA8, 0x00, "D*", 10);
        /// <summary>
        /// 文件寄存器
        /// </summary>
        public readonly static KeyenceDataType  R = new KeyenceDataType(0xAF, 0x00, "R*", 10);
        /// <summary>
        /// 文件寄存器
        /// </summary>
        public readonly static KeyenceDataType  ZR = new KeyenceDataType(0xB0, 0x00, "ZR", 16);
        /// <summary>
        /// 链路寄存器
        /// </summary>
        public readonly static KeyenceDataType  W = new KeyenceDataType(0xB4, 0x00, "W*", 16);
        /// <summary>
        /// 计时器（当前值）
        /// </summary>
        public readonly static KeyenceDataType  TN = new KeyenceDataType(0xC2, 0x00, "TN", 10);
        /// <summary>
        /// 计时器（接点）
        /// </summary>
        public readonly static KeyenceDataType  TS = new KeyenceDataType(0xC1, 0x01, "TS", 10);
        /// <summary>
        /// 计数器（当前值）
        /// </summary>
        public readonly static KeyenceDataType  CN = new KeyenceDataType(0xC5, 0x00, "CN", 10);
        /// <summary>
        /// 计数器（接点）
        /// </summary>
        public readonly static KeyenceDataType  CS = new KeyenceDataType(0xC4, 0x01, "CS", 10);

    }
}
