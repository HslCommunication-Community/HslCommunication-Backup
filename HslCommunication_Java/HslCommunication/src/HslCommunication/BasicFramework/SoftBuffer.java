package HslCommunication.BasicFramework;

import HslCommunication.Core.Thread.SimpleHybirdLock;
import HslCommunication.Core.Transfer.IByteTransform;
import HslCommunication.Core.Transfer.RegularByteTransform;
import HslCommunication.Core.Types.IDataTransfer;

public class SoftBuffer {
    private int capacity = 10;                      // 缓存的容量
    private byte[] buffer;                          // 缓存的数据
    private SimpleHybirdLock hybirdLock;            // 高效的混合锁
    private IByteTransform byteTransform;           // 数据转换类


    /**
     * 使用默认的大小初始化缓存空间
     */
    public SoftBuffer( )
    {
        buffer = new byte[capacity];
        hybirdLock = new SimpleHybirdLock( );
        byteTransform = new RegularByteTransform( );
    }

    /**
     * 使用指定的容量初始化缓存数据块
     * @param capacity 初始化的容量
     */
    public SoftBuffer(int capacity )
    {
        buffer = new byte[capacity];
        this.capacity = capacity;
        hybirdLock = new SimpleHybirdLock( );
        byteTransform = new RegularByteTransform( );
    }


    /**
     * 设置指定的位置的数据块，如果超出，则丢弃数据
     * @param value 数据块信息
     * @param destIndex 目标存储的索引
     */
    public void SetBool( boolean value, int destIndex )
    {
        if (destIndex < capacity * 8 && destIndex >= 0 )
        {
            hybirdLock.Enter( );

            int byteIndex = destIndex / 8;
            int offect = destIndex % 8;

            if (value)
            {
                buffer[byteIndex] = (byte)(buffer[byteIndex] | getOrByte( offect ));
            }
            else
            {
                buffer[byteIndex] = (byte)(buffer[byteIndex] & getAndByte( offect ));
            }

            hybirdLock.Leave( );
        }
    }

    /**
     * 获取指定的位置的bool值，如果超出，则引发异常
     * @param destIndex 目标存储的索引
     * @return 获取索引位置的bool数据值
     * @throws IndexOutOfBoundsException
     */
    public boolean GetBool( int destIndex ) throws IndexOutOfBoundsException
    {
        boolean result = false;
        if (destIndex < capacity * 8 && destIndex >= 0)
        {
            hybirdLock.Enter( );

            int byteIndex = destIndex / 8;
            int offect = destIndex % 8;

            result = (buffer[byteIndex] & getOrByte( offect )) == getOrByte( offect );

            hybirdLock.Leave( );
        }
        else
        {
            throw new IndexOutOfBoundsException( "destIndex" );
        }

        return result;
    }

    private byte getAndByte(int offect )
    {
        switch (offect)
        {
            case 0: return (byte) 0xFE;
            case 1: return (byte) 0xFD;
            case 2: return (byte) 0xFB;
            case 3: return (byte) 0xF7;
            case 4: return (byte) 0xEF;
            case 5: return (byte) 0xDF;
            case 6: return (byte) 0xBF;
            case 7: return (byte) 0x7F;
            default: return (byte) 0xFF;
        }
    }


    private byte getOrByte( int offect )
    {
        switch (offect)
        {
            case 0: return (byte) 0x01;
            case 1: return (byte) 0x02;
            case 2: return (byte) 0x04;
            case 3: return (byte) 0x08;
            case 4: return (byte) 0x10;
            case 5: return (byte) 0x20;
            case 6: return (byte) 0x40;
            case 7: return (byte) 0x80;
            default: return (byte) 0x00;
        }
    }

    /**
     * 设置指定的位置的数据块，如果超出，则丢弃数据
     * @param data 数据块信息
     * @param destIndex 目标存储的索引
     */
    public void SetBytes( byte[] data, int destIndex )
    {
        if (destIndex < capacity && destIndex >= 0 && data != null)
        {
            hybirdLock.Enter( );

            if ((data.length + destIndex) > buffer.length)
            {
                System.arraycopy( data, 0, buffer, destIndex, (buffer.length - destIndex));
            }
            else
            {
                System.arraycopy( data, 0, buffer, destIndex, data.length);
            }

            hybirdLock.Leave( );
        }
    }


    /**
     * 设置指定的位置的数据块，如果超出，则丢弃数据
     * @param data 数据块信息
     * @param destIndex 目标存储的索引
     * @param length 准备拷贝的数据长度
     */
    public void SetBytes( byte[] data, int destIndex, int length )
    {
        if (destIndex < capacity && destIndex >= 0 && data != null)
        {
            if (length > data.length) length = data.length;

            hybirdLock.Enter( );

            if ((length + destIndex) > buffer.length)
            {
                System.arraycopy( data, 0, buffer, destIndex, (buffer.length - destIndex));
            }
            else
            {
                System.arraycopy( data, 0, buffer, destIndex, length );
            }

            hybirdLock.Leave( );
        }
    }

    /**
     * 设置指定的位置的数据块，如果超出，则丢弃数据
     * @param data 数据块信息
     * @param sourceIndex Data中的起始位置
     * @param destIndex 目标存储的索引
     * @param length 准备拷贝的数据长度
     */
    public void SetBytes( byte[] data, int sourceIndex, int destIndex, int length )
    {
        if (destIndex < capacity && destIndex >= 0 && data != null)
        {
            if (length > data.length) length = data.length;

            hybirdLock.Enter( );

            System.arraycopy( data, sourceIndex, buffer, destIndex, length );

            hybirdLock.Leave( );
        }
    }

    /**
     * 获取内存指定长度的数据信息
     * @param index 起始位置
     * @param length 数组长度
     * @return 返回实际的数据信息
     */
    public byte[] GetBytes(int index, int length )
    {
        byte[] result = new byte[length];
        if (length > 0)
        {
            hybirdLock.Enter( );
            if (index >= 0 && (index + length) <= buffer.length)
            {
                System.arraycopy( buffer, index, result, 0, length );
            }
            hybirdLock.Leave( );
        }
        return result;
    }

    /**
     * 获取内存所有的数据信息
     * @return 实际的数据信息
     */
    public byte[] GetBytes( )
    {
        return GetBytes( 0, capacity );
    }


    /**
     * 设置byte类型的数据到缓存区
     * @param value byte数值
     * @param index 索引位置
     */
    public void SetValue(byte value, int index )
    {
        SetBytes( new byte[] { value }, index );
    }

    /**
     * 设置short类型的数据到缓存区
     * @param values short数组
     * @param index 索引位置
     */
    public void SetValue( short[] values, int index )
    {
        SetBytes( byteTransform.TransByte( values ), index );
    }

    /**
     * 设置short类型的数据到缓存区
     * @param value short数值
     * @param index 索引位置
     */
    public void SetValue( short value, int index )
    {
        SetValue( new short[] { value }, index );
    }

    /**
     * 设置int类型的数据到缓存区
     * @param values int数组
     * @param index 索引位置
     */
    public void SetValue( int[] values, int index )
    {
        SetBytes( byteTransform.TransByte( values ), index );
    }

    /**
     * 设置int类型的数据到缓存区
     * @param value int数值
     * @param index 索引位置
     */
    public void SetValue( int value, int index )
    {
        SetValue( new int[] { value }, index );
    }

    /**
     * 设置float类型的数据到缓存区
     * @param values float数组
     * @param index 索引位置
     */
    public void SetValue( float[] values, int index )
    {
        SetBytes( byteTransform.TransByte( values ), index );
    }

    /**
     * 设置float类型的数据到缓存区
     * @param value float数值
     * @param index 索引位置
     */
    public void SetValue( float value, int index )
    {
        SetValue( new float[] { value }, index );
    }

    /**
     * 设置long类型的数据到缓存区
     * @param values long数组
     * @param index 索引位置
     */
    public void SetValue( long[] values, int index )
    {
        SetBytes( byteTransform.TransByte( values ), index );
    }

    /**
     * 设置long类型的数据到缓存区
     * @param value long数值
     * @param index 索引位置
     */
    public void SetValue( long value, int index )
    {
        SetValue( new long[] { value }, index );
    }

    /**
     * 设置double类型的数据到缓存区
     * @param values double数组
     * @param index 索引位置
     */
    public void SetValue( double[] values, int index )
    {
        SetBytes( byteTransform.TransByte( values ), index );
    }

    /**
     * 设置double类型的数据到缓存区
     * @param value double数值
     * @param index 索引位置
     */
    public void SetValue( double value, int index )
    {
        SetValue( new double[] { value }, index );
    }

    /**
     * 获取byte类型的数据
     * @param index 索引位置
     * @return byte数值
     */
    public byte GetByte( int index )
    {
        return GetBytes( index, 1 )[0];
    }

    /**
     * 获取short类型的数组到缓存区
     * @param index 索引位置
     * @param length 数组长度
     * @return short数组
     */
    public short[] GetInt16( int index, int length )
    {
        byte[] tmp = GetBytes( index, length * 2 );
        return byteTransform.TransInt16( tmp, 0, length );
    }

    /**
     * 获取short类型的数据到缓存区
     * @param index 索引位置
     * @return short数据
     */
    public short GetInt16( int index )
    {
        return GetInt16( index, 1 )[0];
    }

    /**
     * 获取int类型的数组到缓存区
     * @param index 索引位置
     * @param length 数组长度
     * @return int数组
     */
    public int[] GetInt32( int index, int length )
    {
        byte[] tmp = GetBytes( index, length * 4 );
        return byteTransform.TransInt32( tmp, 0, length );
    }

    /**
     * 获取int类型的数据到缓存区
     * @param index 索引位置
     * @return int数据
     */
    public int GetInt32( int index )
    {
        return GetInt32( index, 1 )[0];
    }

    /**
     * 获取float类型的数组到缓存区
     * @param index 索引位置
     * @param length 数组长度
     * @return float数组
     */
    public float[] GetSingle( int index, int length )
    {
        byte[] tmp = GetBytes( index, length * 4 );
        return byteTransform.TransSingle( tmp, 0, length );
    }

    /**
     * 获取float类型的数据到缓存区
     * @param index 索引位置
     * @return float数据
     */
    public float GetSingle( int index )
    {
        return GetSingle( index, 1 )[0];
    }

    /**
     * 获取long类型的数组到缓存区
     * @param index 索引位置
     * @param length 数组长度
     * @return long数组
     */
    public long[] GetInt64( int index, int length )
    {
        byte[] tmp = GetBytes( index, length * 8 );
        return byteTransform.TransInt64( tmp, 0, length );
    }

    /**
     * 获取long类型的数据到缓存区
     * @param index 索引位置
     * @return long数据
     */
    public long GetInt64( int index )
    {
        return GetInt64( index, 1 )[0];
    }

    /**
     * 获取double类型的数组到缓存区
     * @param index 索引位置
     * @param length 数组长度
     * @return ulong数组
     */
    public double[] GetDouble( int index, int length )
    {
        byte[] tmp = GetBytes( index, length * 8 );
        return byteTransform.TransDouble( tmp, 0, length );
    }

    /**
     * 获取double类型的数据到缓存区
     * @param index 索引位置
     * @return double数据
     */
    public double GetDouble( int index )
    {
        return GetDouble( index, 1 )[0];
    }

    /**
     * 读取自定义类型的数据，需要规定解析规则
     * @param type 类型名称
     * @param index 起始索引
     * @param <T> 类型对象
     * @return 自定义的数据类型
     * @throws InstantiationException
     * @throws IllegalAccessException
     */
    public <T extends IDataTransfer> T GetCustomer(Class<T> type,int index ) throws InstantiationException, IllegalAccessException
    {
        T Content = type.newInstance();
        byte[] read = GetBytes( index, Content.getReadCount() );
        Content.ParseSource( read );
        return Content;
    }

    /**
     * 写入自定义类型的数据到缓存中去，需要规定生成字节的方法
     * @param data 自定义类型
     * @param index 实例对象
     * @param <T> 起始地址
     */
    public <T extends IDataTransfer> void SetCustomer( T data, int index )
    {
        SetBytes( data.ToSource( ), index );
    }

    /**
     * 获取字节转换关系
     * @return
     */
    public IByteTransform getByteTransform() {
        return byteTransform;
    }

    /**
     * 设置字节转换关系
     * @param byteTransform
     */
    public void setByteTransform(IByteTransform byteTransform){
        this.byteTransform = byteTransform;
    }

}
