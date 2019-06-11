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
        /// 从指定的地址信息解析成真正的设备地址信息
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        public override void Parse( string address, ushort length )
        {
            OperateResult<McAddressData> addressData = ParseFrom( address, length );
            if (addressData.IsSuccess)
            {
                AddressStart    = addressData.Content.AddressStart;
                Length          = addressData.Content.Length;
                McDataType      = addressData.Content.McDataType;
            }
        }

        #region Static Method

        /// <summary>
        /// 从实际的西门子的地址里面
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<McAddressData> ParseFrom( string address )
        {
            return ParseFrom( address, 0 );
        }

        /// <summary>
        /// 从实际的西门子的地址里面
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<McAddressData> ParseFrom( string address, ushort length )
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

        #endregion
    }
}
