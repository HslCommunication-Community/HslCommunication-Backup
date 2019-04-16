using HslCommunication.BasicFramework;
using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HslCommunication.Profinet.Keyence
{
    /// <summary>
    /// 基恩士Nano串口Bytes数据转换规则
    /// </summary>  
    ///以数据格式“位”读取R100~R103时, []表示空格，发送指令如下：
    ///ACSII码：   R       D      S        []       R      1        0        0      []     4       /r
    ///16进制码：0x52,0x44,0x53,0x20,0x52,0x31,0x30,0x30,0x20,0x34,0x0d
    ///响应如下
    ///ACSII码：   1        []      0        []       1       []        0     /r      /n
    ///16进制码：0x31,0x20,0x30,0x20,0x31,0x20,0x30,0x0d,0x0a 
    public class KeyenceNanoByteTransform : IByteTransform
    {

        #region Constructor

        /// <summary>
        /// 实例化一个默认的对象
        /// </summary>
        public KeyenceNanoByteTransform()
        {

        }

        /// <summary>
        /// 数据格式
        /// </summary>
        public DataFormat DataFormat { get; set; }
        #endregion

        #region Get Value From Bytes
        /// <summary>
        /// Nano响应的Bytes转换为string数组
        /// </summary>
        /// <param name="buffer">缓存数据 </param>
        /// <returns>字符串数组 </returns>
        private string[] BytesToStringArray(byte[] buffer)
        {
            string strBuffer = Encoding.Default.GetString(buffer);
            return strBuffer.Split(' ');

        }

        /// <summary>
        /// 从缓存中提取出bool结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">位的索引</param>
        /// <returns>bool对象</returns>
        public virtual bool TransBool(byte[] buffer, int index)
        {
            return BytesToStringArray(buffer)[index] == "1" ? true : false;
        }
        
        /// <summary>
        /// 从缓存中提取出bool数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">位的索引</param>
        /// <param name="length">bool长度</param>
        /// <returns>bool数组</returns>
        public bool[] TransBool(byte[] buffer, int index, int length)
        {
            bool[] result = new bool[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = BytesToStringArray(buffer)[index + i] == "1" ? true : false;
            }
            return result;
        }

        /// <summary>
        /// 从缓存中提取byte结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>byte对象</returns>
        public virtual byte TransByte(byte[] buffer, int index)
        {
            return Convert.ToByte(BytesToStringArray(buffer)[index]);
        }

        /// <summary>
        /// 从缓存中提取byte数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>byte数组对象</returns>
        public virtual byte[] TransByte(byte[] buffer, int index, int length)
        {
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToByte(BytesToStringArray(buffer)[index + i]);
            }
            return result;
        }
        
        /// <summary>
        /// 从缓存中提取short结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>short对象</returns>
        public virtual short TransInt16(byte[] buffer, int index)
        {
            return Convert.ToInt16(BytesToStringArray(buffer)[index]);
        }

        /// <summary>
        /// 从缓存中提取short数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>short数组对象</returns>
        public virtual short[] TransInt16(byte[] buffer, int index, int length)
        {
            short[] result = new short[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToInt16(BytesToStringArray(buffer)[i + index]);
            }
            return result;
        }
        
        /// <summary>
        /// 从缓存中提取ushort结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ushort对象</returns>
        public virtual ushort TransUInt16(byte[] buffer, int index)
        {
            return Convert.ToUInt16(BytesToStringArray(buffer)[index]);
        }
        /// <summary>
        /// 从缓存中提取ushort数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>ushort数组对象</returns>
        public virtual ushort[] TransUInt16(byte[] buffer, int index, int length)
        {
            ushort[] result = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToUInt16(BytesToStringArray(buffer)[i + index]);
            }
            return result;
        }
        /// <summary>
        /// 从缓存中提取int结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>int对象</returns>
        public virtual int TransInt32(byte[] buffer, int index)
        {
            return Convert.ToInt32(BytesToStringArray(buffer)[index]);
        }
        /// <summary>
        /// 从缓存中提取int数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>int数组对象</returns>
        public virtual int[] TransInt32(byte[] buffer, int index, int length)
        {
            int[] result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToInt32(BytesToStringArray(buffer)[i + index]);
            }
            return result;
        }
        /// <summary>
        /// 从缓存中提取uint结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>uint对象</returns>
        public virtual uint TransUInt32(byte[] buffer, int index)
        {
            return Convert.ToUInt32(BytesToStringArray(buffer)[index]);
        }

        /// <summary>
        /// 从缓存中提取uint数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>uint数组对象</returns>
        public virtual uint[] TransUInt32(byte[] buffer, int index, int length)
        {
            uint[] result = new uint[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToUInt32(BytesToStringArray(buffer)[i + index]);
            }
            return result;
        }

        /// <summary>
        /// 从缓存中提取long结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>long对象</returns>
        public virtual long TransInt64(byte[] buffer, int index)
        {
            return Convert.ToInt64(BytesToStringArray(buffer)[index]);
        }

        /// <summary>
        /// 从缓存中提取long数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>long数组对象</returns>
        public virtual long[] TransInt64(byte[] buffer, int index, int length)
        {
            long[] result = new long[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToInt64(BytesToStringArray(buffer)[i + index]);
            }
            return result;
        }


        /// <summary>
        /// 从缓存中提取ulong结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <returns>ulong对象</returns>
        public virtual ulong TransUInt64(byte[] buffer, int index)
        {
            return Convert.ToUInt64(BytesToStringArray(buffer)[index]);
        }

        /// <summary>
        /// 从缓存中提取ulong数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>ulong数组对象</returns>
        public virtual ulong[] TransUInt64(byte[] buffer, int index, int length)
        {
            ulong[] result = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToUInt64(BytesToStringArray(buffer)[i + index]);
            }
            return result;
        }

        /// <summary>
        /// 从缓存中提取float结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>float对象</returns>
        public virtual float TransSingle(byte[] buffer, int index)
        {
            return Convert.ToSingle(BytesToStringArray(buffer)[index]);
        }

        /// <summary>
        /// 从缓存中提取float数组结果
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>float数组对象</returns>
        public virtual float[] TransSingle(byte[] buffer, int index, int length)
        {
            float[] result = new float[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToSingle(BytesToStringArray(buffer)[i + index]);
            }
            return result;
        }


        /// <summary>
        /// 从缓存中提取double结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <returns>double对象</returns>
        public virtual double TransDouble(byte[] buffer, int index)
        {
            return Convert.ToDouble(BytesToStringArray(buffer)[index]);
        }

        /// <summary>
        /// 从缓存中提取double数组结果
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>double数组对象</returns>
        public virtual double[] TransDouble(byte[] buffer, int index, int length)
        {
            double[] result = new double[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToDouble(BytesToStringArray(buffer)[i + index]);
            }
            return result;
        }


        /// <summary>
        /// 从缓存中提取string结果，使用指定的编码
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="index">索引位置</param>
        /// <param name="length">byte数组长度</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>string对象</returns>
        public virtual string TransString(byte[] buffer, int index, int length, Encoding encoding)
        {
            byte[] tmp = TransByte(buffer, index, length);
            return encoding.GetString(tmp);
        }


        #endregion

        #region Get Bytes From Value


        /// <summary>
        /// bool变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(bool value)
        {
            return TransByte(new bool[] { value });
        }

        /// <summary>
        /// bool数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(bool[] values)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string strLength = values.Length.ToString();
            for (int i = 0; i < values.Length; i++)
            {
                stringBuilder.Append(" ");
                stringBuilder.Append(values[i] ? "1" : "0");
            }
            return Encoding.ASCII.GetBytes(stringBuilder.ToString());
        }


        /// <summary>
        /// byte变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(byte value)
        {
            return new byte[] { value };
        }


        /// <summary>
        /// short变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(short value)
        {
            return TransByte(new short[] { value });
        }


        /// <summary>
        /// short数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(short[] values)
        { 
            return Trans<short>.ToBytes(values, ".S");
        }


        /// <summary>
        /// ushort变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(ushort value)
        {
            return TransByte(new ushort[] { value });
        }


        /// <summary>
        /// ushort数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(ushort[] values)
        { 
            return Trans<ushort>.ToBytes(values, ".U");
        }


        /// <summary>
        /// int变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(int value)
        {
            return TransByte(new int[] { value });
        }


        /// <summary>
        /// int数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(int[] values)
        {
            return Trans<int>.ToBytes(values, ".L");
        }

        /// <summary>
        /// uint变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(uint value)
        {
            return TransByte(new uint[] { value });
        }


        /// <summary>
        /// uint数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(uint[] values)
        {

            return Trans<uint>.ToBytes(values, ".D");
        }


        /// <summary>
        /// long变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(long value)
        {
            return TransByte(new long[] { value });
        }

        /// <summary>
        /// long数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(long[] values)
        {
            return Trans<long>.ToBytes(values, ".L");
        }

        /// <summary>
        /// ulong变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(ulong value)
        {
            return TransByte(new ulong[] { value });
        }

        /// <summary>
        /// ulong数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(ulong[] values)
        {

            return Trans<ulong>.ToBytes(values, ".L");
        }

        /// <summary>
        /// float变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(float value)
        {
            return TransByte(new float[] { value });
        }

        /// <summary>
        /// float数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(float[] values)
        {
            return Trans<float>.ToBytes(values, ".F");
        }

        /// <summary>
        /// double变量转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(double value)
        {
            return TransByte(new double[] { value });
        }

        /// <summary>
        /// double数组变量转化缓存数据
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(double[] values)
        {
            return Trans<double>.ToBytes(values,".DF");
        }

        /// <summary>
        /// 使用指定的编码字符串转化缓存数据
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <param name="encoding">字符串的编码方式</param>
        /// <returns>buffer数据</returns>
        public virtual byte[] TransByte(string value, Encoding encoding)
        {
            if (value == null) return null;

            return encoding.GetBytes(value);
        }

        /// <summary>
        /// 字节转换类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class Trans<T> where T : struct
        {
            /// <summary>
            /// 泛型对象转换为字节数组
            /// </summary>
            /// <param name="values"></param>
            /// <param name="dataFormat"></param>
            /// <returns></returns>
            public static byte[] ToBytes(T[] values,string dataFormat)
            {
                if (values == null) return null;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(dataFormat);
                stringBuilder.Append(" ");
                stringBuilder.Append(values.Length.ToString());
                for (int i = 0; i < values.Length; i++)
                { 
                    stringBuilder.Append(" "); 
                    if (typeof(T)==typeof(int) )
                    {
                        var  value = Convert.ToInt32(values[i]);
                        stringBuilder.Append(value.ToString());
                    } 
                    else if (typeof(T)== typeof(uint))
                    {
                        var value = Convert.ToUInt32(values[i]);
                        stringBuilder.Append(value.ToString());
                    }
                    else if (typeof(T) == typeof(short))
                    {
                        var value = Convert.ToInt16(values[i]);
                        stringBuilder.Append(value.ToString());
                    }
                    else if (typeof(T) == typeof(ushort))
                    {
                        var value = Convert.ToUInt16(values[i]);
                        stringBuilder.Append(value.ToString());
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        var value = Convert.ToInt64(values[i]);
                        stringBuilder.Append(value.ToString());
                    }
                    else if (typeof(T) == typeof(ulong))
                    {
                        var value = Convert.ToUInt64(values[i]);
                        stringBuilder.Append(value.ToString());
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        var value = Convert.ToSingle(values[i]);
                        stringBuilder.Append(value.ToString());
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        var value = Convert.ToDouble(values[i]);
                        stringBuilder.Append(value.ToString());
                    }
                    else
                    {
                        return null;
                    }
                }
                return Encoding.ASCII.GetBytes(stringBuilder.ToString());
            }
        }

        #endregion


    } 
  
}
