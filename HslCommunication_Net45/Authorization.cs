using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HslCommunication
{
    /// <summary>
    /// 系统的基本授权类
    /// </summary>
    public class Authorization
    {
        static Authorization( )
        {
            niahdiahduasdbubfas = iashdagsdawbdawda( );
            if (naihsdadaasdasdiwid != 0)
            {
                naihsdadaasdasdiwid = 0;
            }

            if (nuasgdawydaishdgas != 8)
            {
                nuasgdawydaishdgas = 8;
            }
        }

        internal static bool nzugaydgwadawdibbas( )
        {
            moashdawidaisaosdas++;
            return true;
            //if (naihsdadaasdasdiwid == niasdhasdguawdwdad) return nuasduagsdwydbasudasd( );
            //if ((iashdagsdawbdawda( ) - niahdiahduasdbubfas).TotalHours < nuasgdawydaishdgas ) // .TotalHours < nuasgdawydaishdgas)
            //{
            //    return nuasduagsdwydbasudasd( );
            //}

            //return asdhuasdgawydaduasdgu( );
        }

        internal static bool nuasduagsdwydbasudasd( )
        {
            return true;
        }

        internal static bool asdhuasdgawydaduasdgu( )
        {
            return false;
        }

        internal static bool ashdadgawdaihdadsidas( )
        {
            return niasdhasdguawdwdad == 12345;
        }

        internal static DateTime iashdagsdawbdawda( )
        {
            return DateTime.Now;
        }
        internal static DateTime iashdagsaawbdawda( )
        {
            return DateTime.Now.AddDays(1);
        }

        internal static DateTime iashdagsaawadawda( )
        {
            return DateTime.Now.AddDays( 2 );
        }

        internal static string nasduabwduadawdb( string miawdiawduasdhasd )
        {
            StringBuilder asdnawdawdawd = new StringBuilder( );
            MD5 asndiawdniad = MD5.Create( );
            byte[] asdadawdawdas = asndiawdniad.ComputeHash( Encoding.Unicode.GetBytes( miawdiawduasdhasd ) );
            asndiawdniad.Clear( );
            for (int andiawbduawbda = 0; andiawbduawbda < asdadawdawdas.Length; andiawbduawbda++)
            {
                asdnawdawdawd.Append( (255 - asdadawdawdas[andiawbduawbda]).ToString( "X2" ) );
            }
            return asdnawdawdawd.ToString( );
        }

        /// <summary>
        /// 设置本组件系统的授权信息
        /// </summary>
        /// <param name="code">授权码</param>
        public static bool SetAuthorizationCode( string code )
        {
            if (nasduabwduadawdb( code ) == "047E463D69F6020ACA4CBF2B4D682070")
            {
                naihsdadaasdasdiwid = niasdhasdguawdwdad;
                return nuasduagsdwydbasudasd( );
            }
            return asdhuasdgawydaduasdgu( );
        }

        private static DateTime niahdiahduasdbubfas = DateTime.Now;
        internal static long naihsdadaasdasdiwid = 0;
        internal static long moashdawidaisaosdas = 0;
        internal static int nuasgdawydaishdgas = 8;
        internal static int nasidhadguawdbasd = 1000;
        internal static int niasdhasdguawdwdad = 12345;
        internal static int hidahwdauushduasdhu = 23456;
    }
}
