using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HslCommunication;
using System.Reflection;
using HslCommunication.Core;

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

            HslReflectionHelper.SetPropertyExp( FloatProperty, myclass, 123.4 );
            Assert.IsTrue( myclass.FloatValue == 123.4 );
        }

    }

    public class Myclass
    {
        public bool BoolValue { get; set; }

        public int IntValue { get; set; }

        public float FloatValue { get; set; }
    }
}
