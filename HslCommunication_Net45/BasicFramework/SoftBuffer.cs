using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 一个线程安全的缓存数据块，支持批量动态修改，添加，并获取快照
    /// </summary>
    /// <remarks>
    /// 这个类可以实现什么功能呢，就是你有一个大的数组，作为你的应用程序的中间数据池，允许你往byte[]数组里存放指定长度的子byte[]数组，也允许从里面拿数据，
    /// 这些操作都是线程安全的，当然，本类扩展了一些额外的方法支持，也可以直接赋值或获取基本的数据类型对象。
    /// </remarks>
    /// <example>
    /// 此处举例一些数据的读写说明，可以此处的数据示例。
    /// <code lang="cs" source="HslCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBufferExample.cs" region="SoftBufferExample1" title="SoftBuffer示例" />
    /// </example>
    public class SoftBuffer : IDisposable
    {
        #region Constructor

        /// <summary>
        /// 使用默认的大小初始化缓存空间
        /// </summary>
        public SoftBuffer( )
        {
            buffer = new byte[capacity];
            hybirdLock = new SimpleHybirdLock( );
            byteTransform = new RegularByteTransform( );
        }

        /// <summary>
        /// 使用指定的容量初始化缓存数据块
        /// </summary>
        /// <param name="capacity">初始化的容量</param>
        public SoftBuffer(int capacity )
        {
            buffer = new byte[capacity];
            this.capacity = capacity;
            hybirdLock = new SimpleHybirdLock( );
            byteTransform = new RegularByteTransform( );
        }

        #endregion

        #region Bool Operate Support

        /// <summary>
        /// 设置指定的位置的数据块，如果超出，则丢弃数据
        /// </summary>
        /// <param name="value">bool值</param>
        /// <param name="destIndex">目标存储的索引</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void SetBool( bool value, int destIndex )
        {
            SetBool( new bool[] { value }, destIndex );
        }

        /// <summary>
        /// 设置指定的位置的数据块，如果超出，则丢弃数据
        /// </summary>
        /// <param name="value">bool数组值</param>
        /// <param name="destIndex">目标存储的索引</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void SetBool( bool[] value, int destIndex )
        {
            if (value != null)
            {
                try
                {
                    hybirdLock.Enter( );
                    for (int i = 0; i < value.Length; i++)
                    {
                        int byteIndex = (destIndex + i) / 8;
                        int offect = (destIndex + i) % 8;

                        if (value[i])
                        {
                            buffer[byteIndex] = (byte)(buffer[byteIndex] | getOrByte( offect ));
                        }
                        else
                        {
                            buffer[byteIndex] = (byte)(buffer[byteIndex] & getAndByte( offect ));
                        }
                    }

                    hybirdLock.Leave( );
                }
                catch
                {
                    hybirdLock.Leave( );
                    throw;
                }
            }

        }

        /// <summary>
        /// 获取指定的位置的bool值，如果超出，则引发异常
        /// </summary>
        /// <param name="destIndex">目标存储的索引</param>
        /// <returns>获取索引位置的bool数据值</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool GetBool( int destIndex )
        {
            return GetBool( destIndex, 1 )[0];
        }

        /// <summary>
        /// 获取指定位置的bool数组值，如果超过，则引发异常
        /// </summary>
        /// <param name="destIndex">目标存储的索引</param>
        /// <param name="length">读取的数组长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns>bool数组值</returns>
        public bool[] GetBool( int destIndex, int length )
        {
            bool[] result = new bool[length];
            try
            {
                hybirdLock.Enter( );
                for (int i = 0; i < length; i++)
                {
                    int byteIndex = (destIndex + i) / 8;
                    int offect = (destIndex + i) % 8;

                    result[i] = (buffer[byteIndex] & getOrByte( offect )) == getOrByte( offect );
                }

                hybirdLock.Leave( );
            }
            catch
            {
                hybirdLock.Leave( );
                throw;
            }
            return result;
        }

        private byte getAndByte(int offect )
        {
            switch (offect)
            {
                case 0: return 0xFE;
                case 1: return 0xFD;
                case 2: return 0xFB;
                case 3: return 0xF7;
                case 4: return 0xEF;
                case 5: return 0xDF;
                case 6: return 0xBF;
                case 7: return 0x7F;
                default: return 0xFF;
            }
        }


        private byte getOrByte( int offect )
        {
            switch (offect)
            {
                case 0: return 0x01;
                case 1: return 0x02;
                case 2: return 0x04;
                case 3: return 0x08;
                case 4: return 0x10;
                case 5: return 0x20;
                case 6: return 0x40;
                case 7: return 0x80;
                default: return 0x00;
            }
        }
        

        #endregion

        #region Byte Operate Support

        /// <summary>
        /// 设置指定的位置的数据块，如果超出，则丢弃数据
        /// </summary>
        /// <param name="data">数据块信息</param>
        /// <param name="destIndex">目标存储的索引</param>
        public void SetBytes( byte[] data, int destIndex )
        {
            if (destIndex < capacity && destIndex >= 0 && data != null)
            {
                hybirdLock.Enter( );

                if ((data.Length + destIndex) > buffer.Length)
                {
                    Array.Copy( data, 0, buffer, destIndex, (buffer.Length - destIndex) );
                }
                else
                {
                    data.CopyTo( buffer, destIndex );
                }

                hybirdLock.Leave( );
            }
        }

        /// <summary>
        /// 设置指定的位置的数据块，如果超出，则丢弃数据
        /// </summary>
        /// <param name="data">数据块信息</param>
        /// <param name="destIndex">目标存储的索引</param>
        /// <param name="length">准备拷贝的数据长度</param>
        public void SetBytes( byte[] data, int destIndex, int length )
        {
            if (destIndex < capacity && destIndex >= 0 && data != null)
            {
                if (length > data.Length) length = data.Length;

                hybirdLock.Enter( );

                if ((length + destIndex) > buffer.Length)
                {
                    Array.Copy( data, 0, buffer, destIndex, (buffer.Length - destIndex) );
                }
                else
                {
                    Array.Copy( data, 0, buffer, destIndex, length );
                }

                hybirdLock.Leave( );
            }
        }

        /// <summary>
        /// 设置指定的位置的数据块，如果超出，则丢弃数据
        /// </summary>
        /// <param name="data">数据块信息</param>
        /// <param name="sourceIndex">Data中的起始位置</param>
        /// <param name="destIndex">目标存储的索引</param>
        /// <param name="length">准备拷贝的数据长度</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void SetBytes( byte[] data, int sourceIndex, int destIndex, int length )
        {
            if (destIndex < capacity && destIndex >= 0 && data != null)
            {
                if (length > data.Length) length = data.Length;

                hybirdLock.Enter( );

                Array.Copy( data, sourceIndex, buffer, destIndex, length );

                hybirdLock.Leave( );
            }
        }

        /// <summary>
        /// 获取内存指定长度的数据信息
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>返回实际的数据信息</returns>
        public byte[] GetBytes(int index, int length )
        {
            byte[] result = new byte[length];
            if (length > 0)
            {
                hybirdLock.Enter( );
                if (index >= 0 && (index + length) <= buffer.Length)
                {
                    Array.Copy( buffer, index, result, 0, length );
                }
                hybirdLock.Leave( );
            }
            return result;
        }

        /// <summary>
        /// 获取内存所有的数据信息
        /// </summary>
        /// <returns>实际的数据信息</returns>
        public byte[] GetBytes( )
        {
            return GetBytes( 0, capacity );
        }

        #endregion

        #region BCL Set Support

        /// <summary>
        /// 设置byte类型的数据到缓存区
        /// </summary>
        /// <param name="value">byte数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue(byte value, int index )
        {
            SetBytes( new byte[] { value }, index );
        }
 
        /// <summary>
        /// 设置short类型的数据到缓存区
        /// </summary>
        /// <param name="values">short数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( short[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置short类型的数据到缓存区
        /// </summary>
        /// <param name="value">short数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( short value, int index )
        {
            SetValue( new short[] { value }, index );
        }

        /// <summary>
        /// 设置ushort类型的数据到缓存区
        /// </summary>
        /// <param name="values">ushort数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( ushort[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置ushort类型的数据到缓存区
        /// </summary>
        /// <param name="value">ushort数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( ushort value, int index )
        {
            SetValue( new ushort[] { value }, index );
        }

        /// <summary>
        /// 设置int类型的数据到缓存区
        /// </summary>
        /// <param name="values">int数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( int[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置int类型的数据到缓存区
        /// </summary>
        /// <param name="value">int数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( int value, int index )
        {
            SetValue( new int[] { value }, index );
        }

        /// <summary>
        /// 设置uint类型的数据到缓存区
        /// </summary>
        /// <param name="values">uint数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue(uint[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置uint类型的数据到缓存区
        /// </summary>
        /// <param name="value">uint数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( uint value, int index )
        {
            SetValue( new uint[] { value }, index );
        }

        /// <summary>
        /// 设置float类型的数据到缓存区
        /// </summary>
        /// <param name="values">float数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( float[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置float类型的数据到缓存区
        /// </summary>
        /// <param name="value">float数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( float value, int index )
        {
            SetValue( new float[] { value }, index );
        }

        /// <summary>
        /// 设置long类型的数据到缓存区
        /// </summary>
        /// <param name="values">long数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( long[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置long类型的数据到缓存区
        /// </summary>
        /// <param name="value">long数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( long value, int index )
        {
            SetValue( new long[] { value }, index );
        }

        /// <summary>
        /// 设置ulong类型的数据到缓存区
        /// </summary>
        /// <param name="values">ulong数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( ulong[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置ulong类型的数据到缓存区
        /// </summary>
        /// <param name="value">ulong数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( ulong value, int index )
        {
            SetValue( new ulong[] { value }, index );
        }

        /// <summary>
        /// 设置double类型的数据到缓存区
        /// </summary>
        /// <param name="values">double数组</param>
        /// <param name="index">索引位置</param>
        public void SetValue( double[] values, int index )
        {
            SetBytes( byteTransform.TransByte( values ), index );
        }

        /// <summary>
        /// 设置double类型的数据到缓存区
        /// </summary>
        /// <param name="value">double数值</param>
        /// <param name="index">索引位置</param>
        public void SetValue( double value, int index )
        {
            SetValue( new double[] { value }, index );
        }

        #endregion

        #region BCL Get Support

        /// <summary>
        /// 获取byte类型的数据
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>byte数值</returns>
        public byte GetByte( int index )
        {
            return GetBytes( index, 1 )[0];
        }

        /// <summary>
        /// 获取short类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>short数组</returns>
        public short[] GetInt16( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 2 );
            return byteTransform.TransInt16( tmp, 0, length );
        }

        /// <summary>
        /// 获取short类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>short数据</returns>
        public short GetInt16( int index )
        {
            return GetInt16( index, 1 )[0];
        }

        /// <summary>
        /// 获取ushort类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>ushort数组</returns>
        public ushort[] GetUInt16( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 2 );
            return byteTransform.TransUInt16( tmp, 0, length );
        }

        /// <summary>
        /// 获取ushort类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>ushort数据</returns>
        public ushort GetUInt16( int index )
        {
            return GetUInt16( index, 1 )[0];
        }

        /// <summary>
        /// 获取int类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>int数组</returns>
        public int[] GetInt32( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 4 );
            return byteTransform.TransInt32( tmp, 0, length );
        }

        /// <summary>
        /// 获取int类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>int数据</returns>
        public int GetInt32( int index )
        {
            return GetInt32( index, 1 )[0];
        }

        /// <summary>
        /// 获取uint类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>uint数组</returns>
        public uint[] GetUInt32( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 4 );
            return byteTransform.TransUInt32( tmp, 0, length );
        }

        /// <summary>
        /// 获取uint类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>uint数据</returns>
        public uint GetUInt32( int index )
        {
            return GetUInt32( index, 1 )[0];
        }

        /// <summary>
        /// 获取float类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>float数组</returns>
        public float[] GetSingle( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 4 );
            return byteTransform.TransSingle( tmp, 0, length );
        }

        /// <summary>
        /// 获取float类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>float数据</returns>
        public float GetSingle( int index )
        {
            return GetSingle( index, 1 )[0];
        }

        /// <summary>
        /// 获取long类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>long数组</returns>
        public long[] GetInt64( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 8 );
            return byteTransform.TransInt64( tmp, 0, length );
        }

        /// <summary>
        /// 获取long类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>long数据</returns>
        public long GetInt64( int index )
        {
            return GetInt64( index, 1 )[0];
        }

        /// <summary>
        /// 获取ulong类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>ulong数组</returns>
        public ulong[] GetUInt64( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 8 );
            return byteTransform.TransUInt64( tmp, 0, length );
        }

        /// <summary>
        /// 获取ulong类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>ulong数据</returns>
        public ulong GetUInt64( int index )
        {
            return GetUInt64( index, 1 )[0];
        }

        /// <summary>
        /// 获取double类型的数组到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="length">数组长度</param>
        /// <returns>ulong数组</returns>
        public double[] GetDouble( int index, int length )
        {
            byte[] tmp = GetBytes( index, length * 8 );
            return byteTransform.TransDouble( tmp, 0, length );
        }

        /// <summary>
        /// 获取double类型的数据到缓存区
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>double数据</returns>
        public double GetDouble( int index )
        {
            return GetDouble( index, 1 )[0];
        }

        #endregion

        #region Customer Support

        /// <summary>
        /// 读取自定义类型的数据，需要规定解析规则
        /// </summary>
        /// <typeparam name="T">类型名称</typeparam>
        /// <param name="index">起始索引</param>
        /// <returns>自定义的数据类型</returns>
        public T GetCustomer<T>( int index ) where T : IDataTransfer, new()
        {
            T Content = new T( );
            byte[] read = GetBytes( index, Content.ReadCount );
            Content.ParseSource( read );
            return Content;
        }

        /// <summary>
        /// 写入自定义类型的数据到缓存中去，需要规定生成字节的方法
        /// </summary>
        /// <typeparam name="T">自定义类型</typeparam>
        /// <param name="data">实例对象</param>
        /// <param name="index">起始地址</param>
        public void SetCustomer<T>( T data, int index ) where T : IDataTransfer, new()
        {
            SetBytes( data.ToSource( ), index );
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 获取或设置当前的数据缓存类的解析规则
        /// </summary>
        public IByteTransform ByteTransform
        {
            get => byteTransform;
            set => byteTransform = value;
        }

        #endregion

        #region Private Member

        private int capacity = 10;                      // 缓存的容量
        private byte[] buffer;                          // 缓存的数据
        private SimpleHybirdLock hybirdLock;            // 高效的混合锁
        private IByteTransform byteTransform;           // 数据转换类

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 释放当前的对象
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose( bool disposing )
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    hybirdLock?.Dispose( );
                    buffer = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~SoftBuffer()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。

        /// <summary>
        /// 释放当前的对象
        /// </summary>
        public void Dispose( )
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose( true );
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
