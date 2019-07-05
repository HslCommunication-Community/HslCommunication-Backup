using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication;
using System.Reflection;
using HslCommunication.Core;
using System.Diagnostics;

namespace HslCommunication_Net45.Test.Core.Reflection
{
    [TestClass]
    public class HslReflectionTest
    {
        [TestMethod]
        public void HslReflectionHelperTest( )
        {
            Myclass myclass = new Myclass( );
            Type type = typeof( Myclass );
            PropertyInfo BoolProperty = type.GetProperty( "BoolValue" );
            PropertyInfo IntProperty = type.GetProperty( "IntValue" );
            PropertyInfo FloatProperty = type.GetProperty( "FloatValue" );

            HslReflectionHelper.SetPropertyExp( BoolProperty, myclass, true );
            Assert.IsTrue( myclass.BoolValue );

            HslReflectionHelper.SetPropertyExp( IntProperty, myclass, 1234 );
            Assert.IsTrue( myclass.IntValue == 1234 );

            HslReflectionHelper.SetPropertyExp( FloatProperty, myclass, 123.4f );
            Assert.IsTrue( myclass.FloatValue == 123.4f );

            //var sw = new Stopwatch( );
            //sw.Start( );
            //for (int i = 0; i < 100000; i++)
            //{
            //    IntProperty.SetValue( myclass, 123, null );
            //}
            //sw.Stop( );
            //Console.WriteLine( "正常的情况：" + sw.ElapsedMilliseconds );
            //sw.Restart( );
            //for (int i = 0; i < 100000; i++)
            //{
            //    HslReflectionHelper.SetPropertyExp( IntProperty, myclass, 1234 );
            //}
            //sw.Stop( );
            //Console.WriteLine( "表达式树的情况：" + sw.ElapsedMilliseconds );
        }

    }

    public class Myclass
    {
        public bool BoolValue { get; set; }

        public int IntValue { get; set; }

        public float FloatValue { get; set; }
    }
}
