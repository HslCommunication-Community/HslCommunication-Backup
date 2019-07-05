using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace HslCommunication.Core
{
    /// <summary>
    /// 反射的辅助类
    /// </summary>
    public class HslReflectionHelper
    {

        /// <summary>
        /// 从设备里读取支持Hsl特性的数据内容，该特性为<see cref="HslDeviceAddressAttribute"/>，详细参考论坛的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="readWrite">读写接口的实现</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<T> Read<T>( IReadWriteNet readWrite ) where T : class, new()
        {
            var type = typeof( T );
            // var constrcuor = type.GetConstructors( System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );
            var obj = type.Assembly.CreateInstance( type.FullName );

            var properties = type.GetProperties( System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public );
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes( typeof( HslDeviceAddressAttribute ), false );
                if (attribute == null) continue;

                HslDeviceAddressAttribute hslAttribute = null;
                for (int i = 0; i < attribute.Length; i++)
                {
                    HslDeviceAddressAttribute tmp = (HslDeviceAddressAttribute)attribute[i];
                    if (tmp.deviceType != null && tmp.deviceType == readWrite.GetType( ))
                    {
                        hslAttribute = tmp;
                        break;
                    }
                }

                if (hslAttribute == null)
                {
                    for (int i = 0; i < attribute.Length; i++)
                    {
                        HslDeviceAddressAttribute tmp = (HslDeviceAddressAttribute)attribute[i];
                        if (tmp.deviceType == null)
                        {
                            hslAttribute = tmp;
                            break;
                        }
                    }
                }

                if (hslAttribute == null) continue;

                Type propertyType = property.PropertyType;
                if (propertyType == typeof( short ))
                {
                    OperateResult<short> valueResult = readWrite.ReadInt16( hslAttribute.address );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( short[] ))
                {
                    OperateResult<short[]> valueResult = readWrite.ReadInt16( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( ushort ))
                {
                    OperateResult<ushort> valueResult = readWrite.ReadUInt16( hslAttribute.address );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( ushort[] ))
                {
                    OperateResult<ushort[]> valueResult = readWrite.ReadUInt16( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( int ))
                {
                    OperateResult<int> valueResult = readWrite.ReadInt32( hslAttribute.address );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( int[] ))
                {
                    OperateResult<int[]> valueResult = readWrite.ReadInt32( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( uint ))
                {
                    OperateResult<uint> valueResult = readWrite.ReadUInt32( hslAttribute.address );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( uint[] ))
                {
                    OperateResult<uint[]> valueResult = readWrite.ReadUInt32( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( long ))
                {
                    OperateResult<long> valueResult = readWrite.ReadInt64( hslAttribute.address );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( long[] ))
                {
                    OperateResult<long[]> valueResult = readWrite.ReadInt64( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( ulong ))
                {
                    OperateResult<ulong> valueResult = readWrite.ReadUInt64( hslAttribute.address );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( ulong[] ))
                {
                    OperateResult<ulong[]> valueResult = readWrite.ReadUInt64( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( float ))
                {
                    OperateResult<float> valueResult = readWrite.ReadFloat( hslAttribute.address );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( float[] ))
                {
                    OperateResult<float[]> valueResult = readWrite.ReadFloat( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( double ))
                {
                    OperateResult<double> valueResult = readWrite.ReadDouble( hslAttribute.address );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( double[] ))
                {
                    OperateResult<double[]> valueResult = readWrite.ReadDouble( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( string ))
                {
                    OperateResult<string> valueResult = readWrite.ReadString( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( byte[] ))
                {
                    OperateResult<byte[]> valueResult = readWrite.Read( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( bool ))
                {
                    OperateResult<bool> valueResult = readWrite.ReadBool( hslAttribute.address );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
                else if (propertyType == typeof( bool[] ))
                {
                    OperateResult<bool[]> valueResult = readWrite.ReadBool( hslAttribute.address, (ushort)(hslAttribute.length > 0 ? hslAttribute.length : 1) );
                    if (!valueResult.IsSuccess) return OperateResult.CreateFailedResult<T>( valueResult );

                    property.SetValue( obj, valueResult.Content, null );
                }
            }

            return OperateResult.CreateSuccessResult( (T)obj );
        }


        /// <summary>
        /// 从设备里读取支持Hsl特性的数据内容，该特性为<see cref="HslDeviceAddressAttribute"/>，详细参考论坛的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="data">自定义的数据对象</param>
        /// <param name="readWrite">数据读写对象</param>
        /// <returns>包含是否成功的结果对象</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static OperateResult Write<T>( T data, IReadWriteNet readWrite ) where T : class, new()
        {
            if (data == null) throw new ArgumentNullException( nameof( data ) );

            var type = typeof( T );
            var obj = data;

            var properties = type.GetProperties( System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public );
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes( typeof( HslDeviceAddressAttribute ), false );
                if (attribute == null) continue;

                HslDeviceAddressAttribute hslAttribute = null;
                for (int i = 0; i < attribute.Length; i++)
                {
                    HslDeviceAddressAttribute tmp = (HslDeviceAddressAttribute)attribute[i];
                    if (tmp.deviceType != null && tmp.deviceType == readWrite.GetType( ))
                    {
                        hslAttribute = tmp;
                        break;
                    }
                }

                if (hslAttribute == null)
                {
                    for (int i = 0; i < attribute.Length; i++)
                    {
                        HslDeviceAddressAttribute tmp = (HslDeviceAddressAttribute)attribute[i];
                        if (tmp.deviceType == null)
                        {
                            hslAttribute = tmp;
                            break;
                        }
                    }
                }

                if (hslAttribute == null) continue;


                Type propertyType = property.PropertyType;
                if (propertyType == typeof( short ))
                {
                    short value = (short)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( short[] ))
                {
                    short[] value = (short[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( ushort ))
                {
                    ushort value = (ushort)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( ushort[] ))
                {
                    ushort[] value = (ushort[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( int ))
                {
                    int value = (int)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( int[] ))
                {
                    int[] value = (int[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( uint ))
                {
                    uint value = (uint)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( uint[] ))
                {
                    uint[] value = (uint[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( long ))
                {
                    long value = (long)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( long[] ))
                {
                    long[] value = (long[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( ulong ))
                {
                    ulong value = (ulong)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( ulong[] ))
                {
                    ulong[] value = (ulong[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( float ))
                {
                    float value = (float)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( float[] ))
                {
                    float[] value = (float[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( double ))
                {
                    double value = (double)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( double[] ))
                {
                    double[] value = (double[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( string ))
                {
                    string value = (string)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( byte[] ))
                {
                    byte[] value = (byte[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( bool ))
                {
                    bool value = (bool)property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
                else if (propertyType == typeof( bool[] ))
                {
                    bool[] value = (bool[])property.GetValue( obj, null );

                    OperateResult writeResult = readWrite.Write( hslAttribute.address, value );
                    if (!writeResult.IsSuccess) return writeResult;
                }
            }

            return OperateResult.CreateSuccessResult( (T)obj );
        }

        /// <summary>
        /// 使用表达式树的方式来给一个属性赋值
        /// </summary>
        /// <param name="propertyInfo">属性信息</param>
        /// <param name="obj">对象信息</param>
        /// <param name="objValue">实际的值</param>
        public static void SetPropertyExp<T,K>(PropertyInfo propertyInfo, T obj, K objValue )
        {
            // propertyInfo.SetValue( obj, objValue, null );  下面就是实现这句话
            var invokeObjExpr = Expression.Parameter( typeof( T ), "obj" );
            var propValExpr = Expression.Parameter( propertyInfo.PropertyType, "objValue" );
            var setMethodExp = Expression.Call( invokeObjExpr, propertyInfo.GetSetMethod( ), propValExpr );
            var lambda = Expression.Lambda<Action<T,K>>( setMethodExp, invokeObjExpr, propValExpr );
            lambda.Compile( )( obj, objValue );
        }

    }
}
