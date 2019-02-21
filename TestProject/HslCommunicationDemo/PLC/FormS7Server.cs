using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Profinet;
using HslCommunication;
using HslCommunication.ModBus;
using System.Threading;

namespace HslCommunicationDemo
{
    public partial class FormS7Server : Form
    {
        public FormS7Server( )
        {
            InitializeComponent( );
        }



        private void linkLabel1_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            try
            {
                System.Diagnostics.Process.Start( linkLabel1.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void FormSiemens_Load( object sender, EventArgs e )
        {
            panel2.Enabled = false;
        }
        
        
        
        private System.Windows.Forms.Timer timerSecond;

        private void FormSiemens_FormClosing( object sender, FormClosingEventArgs e )
        {
            s7NetServer?.ServerClose( );
        }

        /// <summary>
        /// 统一的读取结果的数据解析，显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="address"></param>
        /// <param name="textBox"></param>
        private void readResultRender<T>( OperateResult<T> result, string address, TextBox textBox )
        {
            if (result.IsSuccess)
            {
                textBox.AppendText( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] {result.Content}{Environment.NewLine}" );
            }
            else
            {
                MessageBox.Show( result.ToString( ) );
            }
        }

        /// <summary>
        /// 统一的数据写入的结果显示
        /// </summary>
        /// <param name="result"></param>
        /// <param name="address"></param>
        private void writeResultRender( string address )
        {
            MessageBox.Show( DateTime.Now.ToString( "[HH:mm:ss] " ) + $"[{address}] 写入成功" );
        }


        #region Server Start


        private HslCommunication.Profinet.Siemens.SiemensS7Server s7NetServer;

        private void button1_Click( object sender, EventArgs e )
        {
            if (!int.TryParse( textBox2.Text, out int port ))
            {
                MessageBox.Show( "端口输入不正确！" );
                return;
            }


            try
            {

                s7NetServer = new HslCommunication.Profinet.Siemens.SiemensS7Server( );                       // 实例化对象
                s7NetServer.LogNet = new HslCommunication.LogNet.LogNetSingle( "logs.txt" );        // 配置日志信息
                s7NetServer.LogNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
                s7NetServer.OnDataReceived += BusTcpServer_OnDataReceived;
                
                s7NetServer.ServerStart( port );

                button1.Enabled = false;
                panel2.Enabled = true;
                button4.Enabled = true;
                button11.Enabled = true;

                timerSecond?.Dispose( );
                timerSecond = new System.Windows.Forms.Timer( );
                timerSecond.Interval = 1000;
                timerSecond.Tick += TimerSecond_Tick;
                timerSecond.Start( );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }


        private void button11_Click( object sender, EventArgs e )
        {
            // 停止服务
            s7NetServer?.ServerClose( );
            button1.Enabled = true;
            button11.Enabled = false;
        }

        private void TimerSecond_Tick( object sender, EventArgs e )
        {
            label15.Text = s7NetServer.OnlineCount.ToString( ) ;
        }

        private void BusTcpServer_OnDataReceived( object sender, byte[] receive )
        {
            if (!checkBox1.Checked) return;

            if (InvokeRequired)
            {
                BeginInvoke( new Action<object,byte[]>( BusTcpServer_OnDataReceived ), sender, receive );
                return;
            }

            textBox1.AppendText( "接收数据：" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( receive, ' ' ) + Environment.NewLine );
        }

        /// <summary>
        /// 当有日志记录的时候，触发，将日志信息也在主界面进行输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogNet_BeforeSaveToFile( object sender, HslCommunication.LogNet.HslEventArgs e )
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke( new Action<object, HslCommunication.LogNet.HslEventArgs>( LogNet_BeforeSaveToFile ), sender, e );
                    return;
                }

                textBox1.AppendText( e.HslMessage.ToString( ) + Environment.NewLine );
            }
            catch
            {
                return;
            }
        }



        #endregion

        #region 单数据读取测试


        private void button_read_bool_Click( object sender, EventArgs e )
        {
            // 读取bool变量
            readResultRender( s7NetServer.ReadBool( textBox3.Text  ), textBox3.Text, textBox4 );
        }
        
        private void button6_Click( object sender, EventArgs e )
        {
            // 读取byte变量
            readResultRender( s7NetServer.ReadByte( textBox3.Text  ), textBox3.Text, textBox4 );
        }

        private void button_read_short_Click( object sender, EventArgs e )
        {
            // 读取short变量
            readResultRender( s7NetServer.ReadInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ushort_Click( object sender, EventArgs e )
        {
            // 读取ushort变量
            readResultRender( s7NetServer.ReadUInt16( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_int_Click( object sender, EventArgs e )
        {
            // 读取int变量
            readResultRender( s7NetServer.ReadInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_uint_Click( object sender, EventArgs e )
        {
            // 读取uint变量
            readResultRender( s7NetServer.ReadUInt32( textBox3.Text ), textBox3.Text, textBox4 );
        }
        private void button_read_long_Click( object sender, EventArgs e )
        {
            // 读取long变量
            readResultRender( s7NetServer.ReadInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_ulong_Click( object sender, EventArgs e )
        {
            // 读取ulong变量
            readResultRender( s7NetServer.ReadUInt64( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_float_Click( object sender, EventArgs e )
        {
            // 读取float变量
            readResultRender( s7NetServer.ReadFloat( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_double_Click( object sender, EventArgs e )
        {
            // 读取double变量
            readResultRender( s7NetServer.ReadDouble( textBox3.Text ), textBox3.Text, textBox4 );
        }

        private void button_read_string_Click( object sender, EventArgs e )
        {
            // 读取字符串
            readResultRender( s7NetServer.ReadString( textBox3.Text, ushort.Parse( textBox5.Text ) ), textBox3.Text, textBox4 );
        }


        #endregion

        #region 单数据写入测试


        private void button24_Click( object sender, EventArgs e )
        {
            // bool写入
            try
            {
                s7NetServer.Write( textBox8.Text, bool.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button7_Click( object sender, EventArgs e )
        {
            // 离散bool写入
            try
            {
                s7NetServer.Write( textBox8.Text, byte.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button22_Click( object sender, EventArgs e )
        {
            // short写入
            try
            {
                s7NetServer.Write( textBox8.Text, short.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button21_Click( object sender, EventArgs e )
        {
            // ushort写入
            try
            {
                s7NetServer.Write(textBox8.Text, ushort.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }


        private void button20_Click( object sender, EventArgs e )
        {
            // int写入
            try
            {
                s7NetServer.Write( textBox8.Text, int.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button19_Click( object sender, EventArgs e )
        {
            // uint写入
            try
            {
                s7NetServer.Write( textBox8.Text , uint.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button18_Click( object sender, EventArgs e )
        {
            // long写入
            try
            {
                s7NetServer.Write( textBox8.Text, long.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button17_Click( object sender, EventArgs e )
        {
            // ulong写入
            try
            {
                s7NetServer.Write(textBox8.Text , ulong.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button16_Click( object sender, EventArgs e )
        {
            // float写入
            try
            {
                s7NetServer.Write( textBox8.Text, float.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }

        private void button15_Click( object sender, EventArgs e )
        {
            // double写入
            try
            {
                s7NetServer.Write( textBox8.Text, double.Parse( textBox7.Text ) );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }


        private void button14_Click( object sender, EventArgs e )
        {
            // string写入
            try
            {
                s7NetServer.Write( textBox8.Text, textBox7.Text );
                writeResultRender( textBox8.Text );
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message );
            }
        }



        #endregion
        

        private void linkLabel2_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            HslCommunication.BasicFramework.FormSupport form = new HslCommunication.BasicFramework.FormSupport( );
            form.ShowDialog( );
        }


        private void Test1( )
        {
            short Short100      = s7NetServer.ReadInt16( "100" ).Content;               // 读取寄存器值
            ushort UShort100    = s7NetServer.ReadUInt16( "100" ).Content;              // 读取寄存器ushort值
            int Int100          = s7NetServer.ReadInt32( "100" ).Content;               // 读取寄存器int值
            uint UInt100        = s7NetServer.ReadUInt32( "100" ).Content;              // 读取寄存器uint值
            float Float100      = s7NetServer.ReadFloat( "100" ).Content;               // 读取寄存器Float值
            long Long100        = s7NetServer.ReadInt64( "100" ).Content;               // 读取寄存器long值
            ulong ULong100      = s7NetServer.ReadUInt64( "100" ).Content;              // 读取寄存器ulong值
            double Double100    = s7NetServer.ReadDouble( "100" ).Content;              // 读取寄存器double值


            s7NetServer.Write( "100", (short)5 );                          // 写入short值
            s7NetServer.Write( "100", (ushort)45678 );                     // 写入ushort值
            s7NetServer.Write( "100", 12345667 );                          // 写入int值
            s7NetServer.Write( "100", (uint)12312312 );                    // 写入uint值
            s7NetServer.Write( "100", 123.456f );                          // 写入float值
            s7NetServer.Write( "100", 1231231231233L );                    // 写入long值
            s7NetServer.Write( "100", 1212312313UL );                      // 写入ulong值
            s7NetServer.Write( "100", 123.456d );                          // 写入double值
        }

        private void button4_Click( object sender, EventArgs e )
        {
            // 连接异形客户端
            using (FormInputAlien form = new FormInputAlien( ))
            {
                if (form.ShowDialog( ) == DialogResult.OK)
                {
                    OperateResult connect = s7NetServer.ConnectHslAlientClient( form.IpAddress, form.Port, form.DTU );
                    if (connect.IsSuccess)
                    {
                        MessageBox.Show( "连接成功！" );
                    }
                    else
                    {
                        MessageBox.Show( "连接失败！原因：" + connect.Message );
                    }
                }
            }
        }
        

        private void button9_Click( object sender, EventArgs e )
        {
            // 将服务器的数据池存储起来
            if (s7NetServer != null)
            {
                s7NetServer.SaveDataPool( "123.txt" );
                MessageBox.Show( "存储完成" );
            }
        }

        private void button8_Click( object sender, EventArgs e )
        {
            // 从文件加载服务器的数据池
            if (s7NetServer != null)
            {
                if (System.IO.File.Exists( "123.txt" ))
                {
                    s7NetServer.LoadDataPool( "123.txt" );
                    MessageBox.Show( "加载完成" );
                }
                else
                {
                    MessageBox.Show( "文件不存在！" );
                }
            }
        }



        private string timerAddress = string.Empty;
        private ushort timerValue = 0;
        private System.Windows.Forms.Timer timerWrite = null;
        private void button10_Click( object sender, EventArgs e )
        {
            // 定时写
            timerWrite = new System.Windows.Forms.Timer( );
            timerWrite.Interval = 300;
            timerWrite.Tick += TimerWrite_Tick;
            timerWrite.Start( );
            timerAddress = textBox8.Text;
            button10.Enabled = false;
        }

        private void TimerWrite_Tick( object sender, EventArgs e )
        {
            s7NetServer.Write( timerAddress, timerValue );
            timerValue++;
        }

    }
}
