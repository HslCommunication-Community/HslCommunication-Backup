using HslCommunication.Profinet.Melsec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HslCommunication.Core.Address
{
    /// <summary>
    /// 三菱的数据地址表示形式
    /// </summary>
    public class McAddressData : DeviceAddressDataBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的对象
        /// </summary>
        public McAddressData( )
        {
            McDataType = MelsecMcDataType.D;
        }

        #endregion

        /// <summary>
        /// 三菱的数据地址信息
        /// </summary>
        public MelsecMcDataType McDataType { get; set; }

        /// <summary>
        /// 从指定的地址信息解析成真正的设备地址信息，默认是三菱的地址
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        public override void Parse( string address, ushort length )
        {
            OperateResult<McAddressData> addressData = ParseMelsecFrom( address, length );
            if (addressData.IsSuccess)
            {
                AddressStart    = addressData.Content.AddressStart;
                Length          = addressData.Content.Length;
                McDataType      = addressData.Content.McDataType;
            }
        }

        #region Static Method

        /// <summary>
        /// 从实际三菱的地址里面解析出
        /// </summary>
        /// <param name="address">三菱的地址数据信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<McAddressData> ParseMelsecFrom( string address, ushort length )
        {
            McAddressData addressData = new McAddressData( );
            addressData.Length = length;
            try
            {
                switch (address[0])
                {
                    case 'M':
                    case 'm':
                        {
                            addressData.McDataType = MelsecMcDataType.M;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.M.FromBase );
                            break;
                        }
                    case 'X':
                    case 'x':
                        {
                            addressData.McDataType = MelsecMcDataType.X;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.X.FromBase );
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            addressData.McDataType = MelsecMcDataType.Y;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Y.FromBase );
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            addressData.McDataType = MelsecMcDataType.D;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.D.FromBase );
                            break;
                        }
                    case 'W':
                    case 'w':
                        {
                            addressData.McDataType = MelsecMcDataType.W;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.W.FromBase );
                            break;
                        }
                    case 'L':
                    case 'l':
                        {
                            addressData.McDataType = MelsecMcDataType.L;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.L.FromBase );
                            break;
                        }
                    case 'F':
                    case 'f':
                        {
                            addressData.McDataType = MelsecMcDataType.F;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.F.FromBase );
                            break;
                        }
                    case 'V':
                    case 'v':
                        {
                            addressData.McDataType = MelsecMcDataType.V;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.V.FromBase );
                            break;
                        }
                    case 'B':
                    case 'b':
                        {
                            addressData.McDataType = MelsecMcDataType.B;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.B.FromBase );
                            break;
                        }
                    case 'R':
                    case 'r':
                        {
                            addressData.McDataType = MelsecMcDataType.R;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.R.FromBase );
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                addressData.McDataType = MelsecMcDataType.SN;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.SN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                addressData.McDataType = MelsecMcDataType.SS;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.SS.FromBase );
                                break;
                            }
                            else if (address[1] == 'C' || address[1] == 'c')
                            {
                                addressData.McDataType = MelsecMcDataType.SC;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.SC.FromBase );
                                break;
                            }
                            else
                            {
                                addressData.McDataType = MelsecMcDataType.S;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.S.FromBase );
                                break;
                            }
                        }
                    case 'Z':
                    case 'z':
                        {
                            if (address.StartsWith( "ZR" ) || address.StartsWith( "zr" ))
                            {
                                addressData.McDataType = MelsecMcDataType.ZR;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.ZR.FromBase );
                                break;
                            }
                            else
                            {
                                addressData.McDataType = MelsecMcDataType.Z;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Z.FromBase );
                                break;
                            }
                        }
                    case 'T':
                    case 't':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                addressData.McDataType = MelsecMcDataType.TN;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.TN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                addressData.McDataType = MelsecMcDataType.TS;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.TS.FromBase );
                                break;
                            }
                            else if (address[1] == 'C' || address[1] == 'c')
                            {
                                addressData.McDataType = MelsecMcDataType.TC;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.TC.FromBase );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    case 'C':
                    case 'c':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                addressData.McDataType = MelsecMcDataType.CN;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.CN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                addressData.McDataType = MelsecMcDataType.CS;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.CS.FromBase );
                                break;
                            }
                            else if (address[1] == 'C' || address[1] == 'c')
                            {
                                addressData.McDataType = MelsecMcDataType.CC;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.CC.FromBase );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<McAddressData>( ex.Message );
            }

            return OperateResult.CreateSuccessResult( addressData );
        }

        /// <summary>
        /// 从实际基恩士的地址里面解析出
        /// </summary>
        /// <param name="address">基恩士的地址数据信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<McAddressData> ParseKeyenceFrom(string address, ushort length )
        {
            McAddressData addressData = new McAddressData( );
            addressData.Length = length;
            try
            {
                switch (address[0])
                {
                    case 'M':
                    case 'm':
                        {
                            addressData.McDataType = MelsecMcDataType.Keyence_M;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Keyence_M.FromBase );
                            break;
                        }
                    case 'X':
                    case 'x':
                        {
                            addressData.McDataType = MelsecMcDataType.Keyence_X;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Keyence_X.FromBase );
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            addressData.McDataType = MelsecMcDataType.Keyence_Y;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Keyence_Y.FromBase );
                            break;
                        }
                    case 'B':
                    case 'b':
                        {
                            addressData.McDataType = MelsecMcDataType.Keyence_B;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Keyence_B.FromBase );
                            break;
                        }
                    case 'L':
                    case 'l':
                        {
                            addressData.McDataType = MelsecMcDataType.Keyence_L;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Keyence_L.FromBase );
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            if (address[1] == 'M' || address[1] == 'm')
                            {
                                addressData.McDataType = MelsecMcDataType.Keyence_SM;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.Keyence_SM.FromBase );
                                break;
                            }
                            else if (address[1] == 'D' || address[1] == 'd')
                            {
                                addressData.McDataType = MelsecMcDataType.Keyence_SD;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.Keyence_SD.FromBase );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    case 'D':
                    case 'd':
                        {
                            addressData.McDataType = MelsecMcDataType.Keyence_D;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Keyence_D.FromBase );
                            break;
                        }
                    case 'R':
                    case 'r':
                        {
                            addressData.McDataType = MelsecMcDataType.Keyence_R;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Keyence_R.FromBase );
                            break;
                        }
                    case 'Z':
                    case 'z':
                        {
                            if (address[1] == 'R' || address[1] == 'r')
                            {
                                addressData.McDataType = MelsecMcDataType.Keyence_ZR;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.Keyence_ZR.FromBase );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    case 'W':
                    case 'w':
                        {
                            addressData.McDataType = MelsecMcDataType.Keyence_W;
                            addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ), MelsecMcDataType.Keyence_W.FromBase );
                            break;
                        }
                    case 'T':
                    case 't':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                addressData.McDataType = MelsecMcDataType.Keyence_TN;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.Keyence_TN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                addressData.McDataType = MelsecMcDataType.Keyence_TS;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.Keyence_TS.FromBase );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    case 'C':
                    case 'c':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                addressData.McDataType = MelsecMcDataType.Keyence_CN;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.Keyence_CN.FromBase );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                addressData.McDataType = MelsecMcDataType.Keyence_CS;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ), MelsecMcDataType.Keyence_CS.FromBase );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<McAddressData>( ex.Message );
            }

            return OperateResult.CreateSuccessResult( addressData );
        }

        /// <summary>
        /// 计算松下的MC协议的偏移地址的机制
        /// </summary>
        /// <param name="address">字符串形式的地址</param>
        /// <returns>实际的偏移地址</returns>
        public static int GetPanasonicAddress( string address )
        {
            if (address.IndexOf( '.' ) > 0)
            {
                string[] values = address.Split( '.' );
                return Convert.ToInt32( values[0] ) * 16 + Convert.ToInt32( values[1] );
            }
            else
            {
                return Convert.ToInt32( address.Substring( 0, address.Length - 1 ) ) * 16 + Convert.ToInt32( address.Substring( address.Length - 1 ), 16 );
            }
        }


        /// <summary>
        /// 从实际松下的地址里面解析出
        /// </summary>
        /// <param name="address">松下的地址数据信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<McAddressData> ParsePanasonicFrom( string address, ushort length )
        {
            McAddressData addressData = new McAddressData( );
            addressData.Length = length;
            try
            {
                switch (address[0])
                {
                    case 'R':
                    case 'r':
                        {
                            int add = GetPanasonicAddress( address.Substring( 1 ) );
                            if (add < 14400)
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_R;
                                addressData.AddressStart = add;
                            }
                            else
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_SM;
                                addressData.AddressStart = add - 14400;
                            }
                            break;
                        }
                    case 'X':
                    case 'x':
                        {
                            addressData.McDataType = MelsecMcDataType.Panasonic_X;
                            addressData.AddressStart = GetPanasonicAddress( address.Substring( 1 ) );
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            addressData.McDataType = MelsecMcDataType.Panasonic_Y;
                            addressData.AddressStart = GetPanasonicAddress( address.Substring( 1 ) );
                            break;
                        }
                    case 'L':
                    case 'l':
                        {
                            if (address[1] == 'D' || address[1] == 'd')
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_LD;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ) );
                                break;
                            }
                            else
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_L;
                                addressData.AddressStart = GetPanasonicAddress( address.Substring( 1 ) );
                            }
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            int add = Convert.ToInt32( address.Substring( 1 ) );
                            if (add < 90000)
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_DT;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ) );
                            }
                            else
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_SD;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 1 ) ) - 90000;
                            }
                            break;
                        }
                    case 'T':
                    case 't':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_TN;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ) );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_TS;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ) );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    case 'C':
                    case 'c':
                        {
                            if (address[1] == 'N' || address[1] == 'n')
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_CN;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ) );
                                break;
                            }
                            else if (address[1] == 'S' || address[1] == 's')
                            {
                                addressData.McDataType = MelsecMcDataType.Panasonic_CS;
                                addressData.AddressStart = Convert.ToInt32( address.Substring( 2 ) );
                                break;
                            }
                            else
                            {
                                throw new Exception( StringResources.Language.NotSupportedDataType );
                            }
                        }
                    default: throw new Exception( StringResources.Language.NotSupportedDataType );
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<McAddressData>( ex.Message );
            }

            return OperateResult.CreateSuccessResult( addressData );
        }

        #endregion
    }
}
